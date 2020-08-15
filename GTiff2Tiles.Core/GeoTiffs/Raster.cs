using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BitMiracle.LibTiff.Classic;
using System.Threading.Channels;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NetVips;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.GeoTiffs
{
    /// <summary>
    /// Class, representing <see cref="Raster"/> GeoTiff.
    /// Used for creating <see cref="RasterTile"/>s
    /// </summary>
    public class Raster : IGeoTiff
    {
        #region Properties

        #region Public

        /// <inheritdoc />
        public Size Size { get; }

        /// <inheritdoc />
        public GeoCoordinate MinCoordinate { get; }

        /// <inheritdoc />
        public GeoCoordinate MaxCoordinate { get; }

        /// <inheritdoc />
        public CoordinateSystem GeoCoordinateSystem { get; }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// This <see cref="Raster"/>'s data
        /// </summary>
        public Image Data { get; }

        #endregion

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Creates new <see cref="Raster"/> object
        /// <remarks><para/>Use this version ONLY if you don't know the <see cref="CoordinateSystem"/>
        /// of this <see cref="Raster"/>. In other cases, prefer using other constructors!</remarks>
        /// </summary>
        /// <inheritdoc cref="Raster(string,CoordinateSystem,long)"/>
        /// <param name="inputFilePath"></param>
        /// <param name="maxMemoryCache"></param>
        public Raster(string inputFilePath, long maxMemoryCache = 2147483648)
        {
            #region Preconditions checks

            CheckHelper.CheckFile(inputFilePath, true, FileExtensions.Tif);

            if (maxMemoryCache <= 0) throw new ArgumentOutOfRangeException(nameof(maxMemoryCache));

            #endregion

            // Disable NetVips warnings for tiff
            NetVipsHelper.DisableLog();

            // Get coordinate system of input geotiff from gdal
            string proj4String = GdalWorker.GetProjString(inputFilePath);
            CoordinateSystem coordinateSystem = GdalWorker.GetCoordinateSystem(proj4String);

            if (coordinateSystem == CoordinateSystem.Other)
                throw new NotSupportedException($"{coordinateSystem} is not supported");

            bool memory = new FileInfo(inputFilePath).Length <= maxMemoryCache;
            Data = Image.NewFromFile(inputFilePath, memory, NetVips.Enums.Access.Random);

            // Get border coordinates и raster sizes
            Size = new Size(Data.Width, Data.Height);

            GeoCoordinateSystem = coordinateSystem;
            (MinCoordinate, MaxCoordinate) = GdalWorker.GetImageBorders(inputFilePath, Size, GeoCoordinateSystem);
        }

        /// <summary>
        /// Creates new <see cref="Raster"/> object
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path
        /// <remarks><para/>Must have .tif extension</remarks></param>
        /// <param name="coordinateSystem">GeoTiff's coordinate system
        /// <remarks><para/>If set to <see cref="CoordinateSystem.Other"/>
        /// throws <see cref="ArgumentOutOfRangeException"/></remarks></param>
        /// <param name="maxMemoryCache">Max size of input image to store in RAM
        /// <remarks><para/>Must be > 0. 2GB by default</remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="NotSupportedException"/>
        public Raster(string inputFilePath, CoordinateSystem coordinateSystem, long maxMemoryCache = 2147483648)
        {
            #region Preconditions checks

            CheckHelper.CheckFile(inputFilePath, true, FileExtensions.Tif);

            if (coordinateSystem == CoordinateSystem.Other) throw new NotSupportedException($"{coordinateSystem} is not supported");
            if (maxMemoryCache <= 0) throw new ArgumentOutOfRangeException(nameof(maxMemoryCache));

            #endregion

            // Disable NetVips warnings for tiff
            NetVipsHelper.DisableLog();

            bool memory = new FileInfo(inputFilePath).Length <= maxMemoryCache;
            Data = Image.NewFromFile(inputFilePath, memory, NetVips.Enums.Access.Random);

            // Get border coordinates и raster sizes
            Size = new Size(Data.Width, Data.Height);

            GeoCoordinateSystem = coordinateSystem;
            (MinCoordinate, MaxCoordinate) = GdalWorker.GetImageBorders(inputFilePath, Size, GeoCoordinateSystem);
        }

        /// <inheritdoc cref="Raster(string,CoordinateSystem,long)"/>
        /// <param name="inputStream"><see cref="Stream"/> with GeoTiff</param>
        /// <param name="coordinateSystem"></param>
        public Raster(Stream inputStream, CoordinateSystem coordinateSystem)
        {
            #region Preconditions checks

            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            if (coordinateSystem == CoordinateSystem.Other) throw new NotSupportedException($"{coordinateSystem} is not supported");

            #endregion

            // Disable NetVips warnings for tiff
            NetVipsHelper.DisableLog();

            (MinCoordinate, MaxCoordinate) = GetBorders(inputStream, coordinateSystem);
            Data = Image.NewFromStream(inputStream, access: NetVips.Enums.Access.Random);

            // Get border coordinates и raster sizes
            Size = new Size(Data.Width, Data.Height);

            GeoCoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> on this <see cref="Raster"/>
        /// </summary>
        ~Raster() => Dispose(false);

        #endregion

        #region Methods

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="Dispose()"/>
        /// <param name="disposing">Dispose static fields?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                // Occurs only if called by programmer. Dispose static things here
            }

            Data?.Dispose();

            IsDisposed = true;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
#pragma warning disable CA1031 // Do not catch general exception types

            try
            {
                Dispose();

                return default;
            }
            catch (Exception exception)
            {
                // Weird bug -- Doesn't work in CI
                //return ValueTask.FromException(exception);
                return new ValueTask(Task.FromException(exception));
            }

#pragma warning restore CA1031 // Do not catch general exception types
        }

        #endregion

        #region Create tile image

        /// <summary>
        /// Create <see cref="Image"/> for one <see cref="RasterTile"/>
        /// from input <see cref="Image"/> or tile cache
        /// </summary>
        /// <param name="tileCache">Source <see cref="Image"/>
        /// or tile cache</param>
        /// <param name="tile">Target <see cref="RasterTile"/></param>
        /// <returns>Ready <see cref="Image"/> for <see cref="RasterTile"/></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public Image CreateTileImage(Image tileCache, RasterTile tile)
        {
            #region Preconditions checks

            if (tileCache == null) throw new ArgumentNullException(nameof(tileCache));
            if (tile == null) throw new ArgumentNullException(nameof(tile));

            string interpolation = tile.Interpolation switch
            {
#pragma warning disable CA1308 // Normalize strings to uppercase
                Interpolation.Nearest => nameof(Interpolation.Nearest).ToLowerInvariant(),
                Interpolation.Linear => nameof(Interpolation.Linear).ToLowerInvariant(),
                Interpolation.Cubic => nameof(Interpolation.Cubic).ToLowerInvariant(),
                Interpolation.Mitchell => nameof(Interpolation.Mitchell).ToLowerInvariant(),
                Interpolation.Lanczos2 => nameof(Interpolation.Lanczos2).ToLowerInvariant(),
                Interpolation.Lanczos3 => nameof(Interpolation.Lanczos3).ToLowerInvariant(),
                _ => throw new ArgumentException("Tile has wrong interpolation", nameof(tile))
#pragma warning restore CA1308 // Normalize strings to uppercase
            };

            #endregion

            // Get postitions and sizes for current tile
            (Area readArea, Area writeArea) = Area.GetAreas(this, tile);

            // Scaling calculations
            double xScale = (double)writeArea.Size.Width / readArea.Size.Width;
            double yScale = (double)writeArea.Size.Height / readArea.Size.Height;

            // Crop and resize tile
            Image tempTileImage = tileCache.Crop((int)readArea.OriginCoordinate.X, (int)readArea.OriginCoordinate.Y,
                                                 readArea.Size.Width, readArea.Size.Height)
                                           .Resize(xScale, interpolation, yScale);

            // Add alpha channel if needed
            Band.AddDefaultBands(ref tempTileImage, tile.BandsCount);

            // Make transparent image and insert tile
            return Image.Black(tile.Size.Width, tile.Size.Height).NewFromImage(new int[tile.BandsCount])
                        .Insert(tempTileImage, (int)writeArea.OriginCoordinate.X,
                                (int)writeArea.OriginCoordinate.Y);
        }

        #endregion

        #region WriteTile

        /// <summary>
        /// Gets data from source <see cref="Image"/>
        /// or tile cache for specified <see cref="RasterTile"/>
        /// and writes it to ready file
        /// </summary>
        /// <param name="tileCache">Source <see cref="Image"/>
        /// or tile cache</param>
        /// <param name="tile">Target <see cref="RasterTile"/>
        /// <remarks><para/><see cref="Tile.Path"/> should not be null or whitespace</remarks></param>
        /// <exception cref="ArgumentNullException"/>
        public void WriteTileToFile(Image tileCache, RasterTile tile)
        {
            #region Preconditions checks

            if (tile == null) throw new ArgumentNullException(nameof(tile));

            CheckHelper.CheckFile(tile.Path, false);

            #endregion

            // Preconditions checked inside CreateTileImage, don't need to check anything here

            using Image tileImage = CreateTileImage(tileCache, tile);

            //TODO: validate size before writing
            tileImage.WriteToFile(tile.Path);
        }

        /// <inheritdoc cref="WriteTileToFile"/>
        public Task WriteTileToFileAsync(Image tileCache, RasterTile tile) =>
            Task.Run(() => WriteTileToFile(tileCache, tile));

        /// <summary>
        /// Gets data from source <see cref="Image"/>
        /// or tile cache for specified <see cref="RasterTile"/>
        /// and writes it to <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="tileCache">Source <see cref="Image"/>
        /// or tile cache</param>
        /// <param name="tile">Target <see cref="RasterTile"/></param>
        /// <returns><see cref="RasterTile"/>'s <see cref="byte"/>s</returns>
        public IEnumerable<byte> WriteTileToEnumerable(Image tileCache, RasterTile tile)
        {
            // Preconditions checked inside CreateTileImage, don't need to check anything here

            using Image tileImage = CreateTileImage(tileCache, tile);

            //TODO: validate size before writing
            return tileImage.WriteToMemory();
        }

        /// <summary>
        /// Gets data from source <see cref="Image"/>
        /// or tile cache for specified <see cref="RasterTile"/>
        /// and writes it to <see cref="ChannelWriter{T}"/>
        /// </summary>
        /// <param name="tileCache">Source <see cref="Image"/>
        /// or tile cache</param>
        /// <param name="tile">Target <see cref="RasterTile"/></param>
        /// <param name="channelWriter">Target <see cref="ChannelWriter{T}"/></param>
        /// <returns><see langword="true"/> if <see cref="ITile"/> was written;
        /// <see langword="false"/> otherwise</returns>
        /// <exception cref="ArgumentNullException"/>
        public bool WriteTileToChannel(Image tileCache, RasterTile tile, ChannelWriter<ITile> channelWriter)
        {
            #region Preconditions checks

            // tileCache and interpolation checked inside WriteTileToEnumerable

            if (tile == null) throw new ArgumentNullException(nameof(tile));
            if (channelWriter == null) throw new ArgumentNullException(nameof(channelWriter));

            #endregion

            tile.Bytes = WriteTileToEnumerable(tileCache, tile);

            return tile.Validate(false) && channelWriter.TryWrite(tile);
        }

        /// <returns></returns>
        /// <inheritdoc cref="WriteTileToChannel"/>
        public ValueTask WriteTileToChannelAsync(Image tileCache, RasterTile tile, ChannelWriter<ITile> channelWriter)
        {
            #region Preconditions checks

            // tileCache and interpolation checked inside WriteTileToEnumerable

            if (tile == null) throw new ArgumentNullException(nameof(tile));
            if (channelWriter == null) throw new ArgumentNullException(nameof(channelWriter));

            #endregion

            tile.Bytes = WriteTileToEnumerable(tileCache, tile);

            // Weird bug -- Doesn't work in CI
            //ValueTask.CompletedTask;
            return tile.Validate(false) ? channelWriter.WriteAsync(tile) : new ValueTask(Task.CompletedTask);
        }

        #endregion

        #region WriteTiles

        /// <summary>
        /// Crops current <see cref="RasterTile"/> on <see cref="RasterTile"/>s
        /// and writes them to <paramref name="outputDirectoryPath"/>
        /// </summary>
        /// <param name="outputDirectoryPath">Directory for output <see cref="RasterTile"/>s</param>
        /// <param name="tileExtension">Extension of ready <see cref="RasterTile"/>s
        /// <remarks><para/>.png by default</remarks></param>
        /// <inheritdoc cref="WriteTilesToAsyncEnumerable"/>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="tileSize"></param>
        /// <param name="interpolation"></param>
        /// <param name="bandsCount"></param>
        /// <param name="tileCacheCount"></param>
        /// <param name="threadsCount">T</param>
        /// <param name="progress"></param>
        /// <param name="printTimeAction"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="RasterException"/>
        public void WriteTilesToDirectory(string outputDirectoryPath, int minZ, int maxZ,
                                          bool tmsCompatible = false, Size tileSize = null,
                                          TileExtension tileExtension = TileExtension.Png,
                                          Interpolation interpolation = Interpolation.Lanczos3,
                                          int bandsCount = RasterTile.DefaultBandsCount,
                                          int tileCacheCount = 1000, int threadsCount = 0,
                                          IProgress<double> progress = null,
                                          Action<string> printTimeAction = null)
        {
            #region Preconditions checks

            CheckHelper.CheckDirectory(outputDirectoryPath, true);
            // minZ and maxZ checked inside Number.GetCount
            tileSize ??= Tile.DefaultSize;

            // interpolation is checked on lower levels
            // bandsCount checked inside RasterTile ctor
            if (tileCacheCount <= 0) throw new ArgumentOutOfRangeException(nameof(tileCacheCount));

            ParallelOptions parallelOptions = new ParallelOptions();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            // It's safe to set progress to null

            Stopwatch stopwatch = printTimeAction == null ? null : Stopwatch.StartNew();

            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);

            // if there's no tiles to crop
            if (tilesCount <= 0) throw new RasterException("No tiles to crop in this raster");

            double counter = 0.0;

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            void MakeTile(int x, int y, int z)
            {
                // Create directories for the tile
                // The overall structure looks like: outputDirectory/z/x/y.extension
                string tileDirectoryPath = Path.Combine(outputDirectoryPath, $"{z}", $"{x}");
                CheckHelper.CheckDirectory(tileDirectoryPath);

                Number tileNumber = new Number(x, y, z);
                RasterTile tile = new RasterTile(tileNumber, GeoCoordinateSystem, extension: tileExtension,
                                                 tmsCompatible: tmsCompatible, size: tileSize,
                                                 bandsCount: bandsCount, interpolation: interpolation);

                // Important: OpenLayers requires replacement of tileY to tileY+1
                tile.Path = Path.Combine(tileDirectoryPath, $"{y}{tile.GetExtensionString()}");

                // tile is validated inside of WriteTileToFile
                // ReSharper disable once AccessToDisposedClosure
                WriteTileToFile(tileCache, tile);

                // Report progress
                counter++;
                double percentage = counter / tilesCount * 100.0;
                progress?.Report(percentage);

                ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch, printTimeAction);
            }

            // For each zoom
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                        zoom, tileSize, tmsCompatible);

                // For each tile on given zoom calculate positions/sizes and save as file
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    int y = tileY;
                    int z = zoom;

                    Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, x => MakeTile(x, y, z));
                }
            }
        }

        /// <inheritdoc cref="WriteTilesToDirectory"/>
        public Task WriteTilesToDirectoryAsync(string outputDirectoryPath, int minZ, int maxZ,
                                               bool tmsCompatible = false, Size tileSize = null,
                                               TileExtension tileExtension = TileExtension.Png,
                                               Interpolation interpolation = Interpolation.Lanczos3,
                                               int bandsCount = RasterTile.DefaultBandsCount,
                                               int tileCacheCount = 1000, int threadsCount = 0,
                                               IProgress<double> progress = null,
                                               Action<string> printTimeAction = null) =>
            Task.Run(() => WriteTilesToDirectory(outputDirectoryPath, minZ, maxZ, tmsCompatible, tileSize,
                                             tileExtension, interpolation, bandsCount, tileCacheCount,
                                             threadsCount, progress, printTimeAction));

        /// <summary>
        /// Crops current <see cref="Raster"/> on <see cref="ITile"/>s
        /// and writes them to <paramref name="channelWriter"/>
        /// </summary>
        /// <param name="channelWriter"><see cref="Channel"/> to write <see cref="ITile"/> to</param>
        /// <inheritdoc cref="WriteTilesToAsyncEnumerable"/>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="tileSize"></param>
        /// <param name="interpolation"></param>
        /// <param name="bandsCount"></param>
        /// <param name="tileCacheCount"></param>
        /// <param name="threadsCount"></param>
        /// <param name="progress"></param>
        /// <param name="printTimeAction"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="RasterException"/>
        public void WriteTilesToChannel(ChannelWriter<ITile> channelWriter, int minZ, int maxZ,
                                        bool tmsCompatible = false, Size tileSize = null,
                                        Interpolation interpolation = Interpolation.Lanczos3,
                                        int bandsCount = RasterTile.DefaultBandsCount,
                                        int tileCacheCount = 1000, int threadsCount = 0,
                                        IProgress<double> progress = null,
                                        Action<string> printTimeAction = null)
        {
            #region Preconditions checks

            // channelWriter is checked on lower levels
            // minZ and maxZ checked inside Number.GetCount
            tileSize ??= Tile.DefaultSize;

            // interpolation is checked on lower levels
            // bandsCount checked inside RasterTile ctor
            if (tileCacheCount <= 0) throw new ArgumentOutOfRangeException(nameof(tileCacheCount));

            ParallelOptions parallelOptions = new ParallelOptions();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            // It's safe to set progress to null

            Stopwatch stopwatch = printTimeAction == null ? null : Stopwatch.StartNew();

            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);

            // if there's no tiles to crop
            if (tilesCount <= 0) throw new RasterException("No tiles to crop in this raster");

            double counter = 0.0;

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            void MakeTile(int x, int y, int z)
            {
                Number tileNumber = new Number(x, y, z);
                RasterTile tile = new RasterTile(tileNumber, GeoCoordinateSystem, tmsCompatible: tmsCompatible,
                    size: tileSize, bandsCount: bandsCount, interpolation: interpolation);

                // Should not throw exception if tile was skipped
                // ReSharper disable once AccessToDisposedClosure
                if (!WriteTileToChannel(tileCache, tile, channelWriter)) return;

                // Report progress
                counter++;
                double percentage = counter / tilesCount * 100.0;
                progress?.Report(percentage);

                ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch, printTimeAction);
            }

            // For each zoom
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                    zoom, tileSize, tmsCompatible);

                // For each tile on given zoom calculate positions/sizes and save as file
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    int y = tileY;
                    int z = zoom;

                    Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, x => MakeTile(x, y, z));
                }
            }
        }

        /// <inheritdoc cref="WriteTilesToChannel"/>
        public Task WriteTilesToChannelAsync(ChannelWriter<ITile> channelWriter, int minZ, int maxZ,
                                             bool tmsCompatible = false, Size tileSize = null,
                                             Interpolation interpolation = Interpolation.Lanczos3,
                                             int bandsCount = RasterTile.DefaultBandsCount,
                                             int tileCacheCount = 1000, int threadsCount = 0,
                                             IProgress<double> progress = null,
                                             Action<string> printTimeAction = null) =>
            Task.Run(() => WriteTilesToChannel(channelWriter, minZ, maxZ, tmsCompatible, tileSize,
                                           interpolation, bandsCount, tileCacheCount, threadsCount,
                                           progress, printTimeAction));

        /// <summary>
        /// Crops current <see cref="Raster"/> on <see cref="ITile"/>s
        /// and writes them to <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="ITile"/>s</returns>
        /// <inheritdoc cref="WriteTilesToAsyncEnumerable"/>
        public IEnumerable<ITile> WriteTilesToEnumerable(int minZ, int maxZ,
                                                         bool tmsCompatible = false, Size tileSize = null,
                                                         Interpolation interpolation = Interpolation.Lanczos3,
                                                         int bandsCount = RasterTile.DefaultBandsCount,
                                                         int tileCacheCount = 1000,
                                                         IProgress<double> progress = null,
                                                         Action<string> printTimeAction = null)
        {
            #region Preconditions checks

            // minZ and maxZ checked inside Number.GetCount
            tileSize ??= Tile.DefaultSize;

            // interpolation is checked on lower levels
            // bandsCount checked inside RasterTile ctor
            if (tileCacheCount <= 0) throw new ArgumentOutOfRangeException(nameof(tileCacheCount));

            // It's safe to set progress to null

            Stopwatch stopwatch = printTimeAction == null ? null : Stopwatch.StartNew();

            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);

            // if there's no tiles to crop
            if (tilesCount <= 0) throw new RasterException("No tiles to crop in this raster");

            double counter = 0.0;

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            ITile MakeTile(int x, int y, int z)
            {
                Number tileNumber = new Number(x, y, z);
                RasterTile tile = new RasterTile(tileNumber, GeoCoordinateSystem, tmsCompatible: tmsCompatible,
                    size: tileSize, bandsCount: bandsCount, interpolation: interpolation);

                tile.Bytes = WriteTileToEnumerable(tileCache, tile);

                // Report progress
                counter++;
                double percentage = counter / tilesCount * 100.0;
                progress?.Report(percentage);

                ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch, printTimeAction);

                return tile;
            }

            // For each specified zoom
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                        zoom, tileSize, tmsCompatible);

                // For each tile on given zoom calculate positions/sizes and save as file
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    for (int tileX = minNumber.X; tileX <= maxNumber.X; tileX++)
                        yield return MakeTile(tileX, tileY, zoom);
                }
            }
        }

        /// <summary>
        /// Crops current <see cref="Raster"/> on <see cref="ITile"/>s
        /// and writes them to <see cref="IAsyncEnumerable{T}"/>
        /// </summary>
        /// <param name="minZ">Minimum cropped zoom
        /// <remarks><para/>Should be >= 0 and lesser or equal, than <paramref name="maxZ"/>
        /// </remarks></param>
        /// <param name="maxZ">Maximum cropped zoom
        /// <remarks><para/>Should be >= 0 and bigger or equal, than <paramref name="minZ"/>
        /// </remarks></param>
        /// <param name="tmsCompatible">Do you want to create tms-compatible <see cref="ITile"/>s?
        /// <remarks><para/><see langword="false"/> by default</remarks></param>
        /// <param name="tileSize"><see cref="Images.Size"/> of <see cref="ITile"/>s
        /// <remarks><para/>256x256 by default</remarks></param>
        /// <param name="interpolation">Interpolation of ready tiles
        /// <remarks><para/><see cref="Interpolation.Lanczos3"/> by default</remarks></param>
        /// <param name="bandsCount">Count of <see cref="Band"/>s in ready <see cref="ITile"/>s
        /// <remarks><para/>4 by default</remarks></param>
        /// <param name="tileCacheCount">Count of <see cref="ITile"/> to be in cache
        /// <remarks><para/>1000 by default</remarks></param>
        /// <param name="threadsCount">Threads count
        /// <remarks><para/>Calculates automatically by default</remarks></param>
        /// <param name="progress">Progress-reporter
        /// <remarks><para/><see langword="null"/> by default</remarks></param>
        /// <param name="printTimeAction"><see cref="Action{T}"/> to print estimated time
        /// <remarks><para/><see langword="null"/> by default;
        /// set to <see langword="null"/> if you don't want output</remarks></param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> of <see cref="ITile"/>s</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="RasterException"/>
        public IAsyncEnumerable<ITile> WriteTilesToAsyncEnumerable(int minZ, int maxZ,
                                                                   bool tmsCompatible = false, Size tileSize = null,
                                                                   Interpolation interpolation = Interpolation.Lanczos3,
                                                                   int bandsCount = RasterTile.DefaultBandsCount,
                                                                   int tileCacheCount = 1000, int threadsCount = 0,
                                                                   IProgress<double> progress = null,
                                                                   Action<string> printTimeAction = null)
        {
            // All preconditions checks are done in WriteTilesToChannelAsync method

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            WriteTilesToChannelAsync(channel.Writer, minZ, maxZ, tmsCompatible, tileSize,
                                     interpolation, bandsCount, tileCacheCount,
                                     threadsCount, progress, printTimeAction)
               .ContinueWith(_ => channel.Writer.Complete(), TaskScheduler.Current);

            return channel.Reader.ReadAllAsync();
        }

        #endregion

        #region GetBorders

        /// <summary>
        /// Gets minimal and maximal coordinates from input GeoTiff's stream
        /// </summary>
        /// <param name="inputStream">Any kind of stream with GeoTiff's data</param>
        /// <param name="coordinateSystem">GeoTiff's <see cref="CoordinateSystem"/>
        /// <remarks><para/>If set to <see cref="CoordinateSystem.Other"/> throws
        /// <see cref="NotSupportedException"/></remarks></param>
        /// <returns><see cref="ValueTuple{T1,T2}"/> of
        /// <see cref="GeoCoordinate"/>s of image's borders</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"/>
        /// <exception cref="ArgumentException"/>
        public static (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) GetBorders(
            Stream inputStream, CoordinateSystem coordinateSystem)
        {
            #region Preconditions checks

            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            if (!inputStream.CanRead) throw new ArgumentException($"{nameof(inputStream)} is broken");
            // CoordinateSystem checked lower

            #endregion

            // Disable warnings from libtiff
            Tiff.SetErrorHandler(new LibTiffHelper());

            Tiff tiff = Tiff.ClientOpen(string.Empty, "r", inputStream, new TiffStream());

            if (tiff == null) throw new ArgumentException($"{nameof(inputStream)} is broken");

            // Get origin coordinates
            FieldValue[] tiePointTag = tiff.GetField(TiffTag.GEOTIFF_MODELTIEPOINTTAG);

            // Get pixel scale
            FieldValue[] pixScaleTag = tiff.GetField(TiffTag.GEOTIFF_MODELPIXELSCALETAG);

            // Image's sizes
            int width = tiff.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
            int height = tiff.GetField(TiffTag.IMAGELENGTH)[0].ToInt();

            byte[] tiePoints = tiePointTag[1].GetBytes();
            double pixelScale = BitConverter.ToDouble(pixScaleTag[1].GetBytes(), 0);

            double minX = BitConverter.ToDouble(tiePoints, 24);
            double maxY = BitConverter.ToDouble(tiePoints, 32);
            double maxX = minX + width * pixelScale;
            double minY = maxY - height * pixelScale;

            switch (coordinateSystem)
            {
                case CoordinateSystem.Epsg4326:
                {
                    GeodeticCoordinate minCoordinate = new GeodeticCoordinate(minX, minY);
                    GeodeticCoordinate maxCoordinate = new GeodeticCoordinate(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
                case CoordinateSystem.Epsg3857:
                {
                    MercatorCoordinate minCoordinate = new MercatorCoordinate(minX, minY);
                    MercatorCoordinate maxCoordinate = new MercatorCoordinate(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
                default: throw new NotSupportedException($"{coordinateSystem} is not supported");
            }
        }

        #endregion

        #endregion
    }
}
