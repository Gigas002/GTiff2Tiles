using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;
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
    public class Raster : GeoTiff
    {
        #region Properties

        /// <summary>
        /// This <see cref="Raster"/>'s data
        /// </summary>
        public Image Data { get; }

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
            {
                string err = string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem);

                throw new NotSupportedException(err);
            }

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

            if (coordinateSystem == CoordinateSystem.Other)
            {
                string err = string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem);

                throw new NotSupportedException(err);
            }

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

            if (coordinateSystem == CoordinateSystem.Other)
            {
                string err = string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem);

                throw new NotSupportedException(err);
            }

            #endregion

            // Disable NetVips warnings for tiff
            NetVipsHelper.DisableLog();

            (MinCoordinate, MaxCoordinate) = GetBorders(inputStream, coordinateSystem);
            Data = Image.NewFromStream(inputStream, access: NetVips.Enums.Access.Random);

            // Reset stream reading position
            inputStream.Seek(0, SeekOrigin.Begin);

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
        protected override void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            base.Dispose(disposing);

            if (disposing)
            {
                // Occurs only if called by programmer. Dispose static things here
            }

            Data?.Dispose();
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

            #endregion

            // Get postitions and sizes for current tile
            (Area readArea, Area writeArea) = Area.GetAreas(this, tile);

            // Scaling calculations
            double xScale = (double)writeArea.Size.Width / readArea.Size.Width;
            double yScale = (double)writeArea.Size.Height / readArea.Size.Height;

            // Crop and resize tile
            Image tempTileImage = tileCache.Crop((int)readArea.OriginCoordinate.X, (int)readArea.OriginCoordinate.Y,
                                                 readArea.Size.Width, readArea.Size.Height)
                                           .Resize(xScale, tile.Interpolation, yScale);

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

            return tileImage.WriteToBuffer(tile.GetExtensionString());
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
        /// <returns><see langword="true"/> if <see cref="RasterTile"/> was written;
        /// <see langword="false"/> otherwise</returns>
        /// <exception cref="ArgumentNullException"/>
        public bool WriteTileToChannel(Image tileCache, RasterTile tile, ChannelWriter<RasterTile> channelWriter)
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
        public ValueTask WriteTileToChannelAsync(Image tileCache, RasterTile tile, ChannelWriter<RasterTile> channelWriter)
        {
            #region Preconditions checks

            // tileCache and interpolation checked inside WriteTileToEnumerable

            if (tile == null) throw new ArgumentNullException(nameof(tile));
            if (channelWriter == null) throw new ArgumentNullException(nameof(channelWriter));

            #endregion

            tile.Bytes = WriteTileToEnumerable(tileCache, tile);

            return tile.Validate(false) ? channelWriter.WriteAsync(tile) : ValueTask.CompletedTask;
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
                                          NetVips.Enums.Kernel interpolation = NetVips.Enums.Kernel.Lanczos3,
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

            ParallelOptions parallelOptions = new();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            // It's safe to set progress to null

            Stopwatch stopwatch = printTimeAction == null ? null : Stopwatch.StartNew();

            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);

            // if there's no tiles to crop
            if (tilesCount <= 0) throw new RasterException(Strings.NoTilesToCrop);

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

                Number tileNumber = new(x, y, z);
                RasterTile tile = new(tileNumber, GeoCoordinateSystem, tileSize, tmsCompatible)
                {
                    Extension = tileExtension,
                    BandsCount = bandsCount,
                    Interpolation = interpolation
                };

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
                                               NetVips.Enums.Kernel interpolation = NetVips.Enums.Kernel.Lanczos3,
                                               int bandsCount = RasterTile.DefaultBandsCount,
                                               int tileCacheCount = 1000, int threadsCount = 0,
                                               IProgress<double> progress = null,
                                               Action<string> printTimeAction = null) =>
            Task.Run(() => WriteTilesToDirectory(outputDirectoryPath, minZ, maxZ, tmsCompatible, tileSize,
                                             tileExtension, interpolation, bandsCount, tileCacheCount,
                                             threadsCount, progress, printTimeAction));

        /// <summary>
        /// Crops current <see cref="Raster"/> on <see cref="RasterTile"/>s
        /// and writes them to <paramref name="channelWriter"/>
        /// </summary>
        /// <param name="channelWriter"><see cref="Channel"/> to write <see cref="RasterTile"/> to</param>
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
        public void WriteTilesToChannel(ChannelWriter<RasterTile> channelWriter, int minZ, int maxZ,
                                        bool tmsCompatible = false, Size tileSize = null,
                                        NetVips.Enums.Kernel interpolation = NetVips.Enums.Kernel.Lanczos3,
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

            ParallelOptions parallelOptions = new();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            // It's safe to set progress to null

            Stopwatch stopwatch = printTimeAction == null ? null : Stopwatch.StartNew();

            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);

            // if there's no tiles to crop
            if (tilesCount <= 0) throw new RasterException(Strings.NoTilesToCrop);

            double counter = 0.0;

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            void MakeTile(int x, int y, int z)
            {
                Number tileNumber = new(x, y, z);
                RasterTile tile =
                    new(tileNumber, GeoCoordinateSystem, tileSize, tmsCompatible)
                    {
                        BandsCount = bandsCount, Interpolation = interpolation
                    };

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
        public Task WriteTilesToChannelAsync(ChannelWriter<RasterTile> channelWriter, int minZ, int maxZ,
                                             bool tmsCompatible = false, Size tileSize = null,
                                             NetVips.Enums.Kernel interpolation = NetVips.Enums.Kernel.Lanczos3,
                                             int bandsCount = RasterTile.DefaultBandsCount,
                                             int tileCacheCount = 1000, int threadsCount = 0,
                                             IProgress<double> progress = null,
                                             Action<string> printTimeAction = null) =>
            Task.Run(() => WriteTilesToChannel(channelWriter, minZ, maxZ, tmsCompatible, tileSize,
                                           interpolation, bandsCount, tileCacheCount, threadsCount,
                                           progress, printTimeAction));

        /// <summary>
        /// Crops current <see cref="Raster"/> on <see cref="RasterTile"/>s
        /// and writes them to <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="RasterTile"/>s</returns>
        /// <inheritdoc cref="WriteTilesToAsyncEnumerable"/>
        public IEnumerable<RasterTile> WriteTilesToEnumerable(int minZ, int maxZ,
                                                         bool tmsCompatible = false, Size tileSize = null,
                                                         NetVips.Enums.Kernel interpolation = NetVips.Enums.Kernel.Lanczos3,
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
            if (tilesCount <= 0) throw new RasterException(Strings.NoTilesToCrop);

            double counter = 0.0;

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            RasterTile MakeTile(int x, int y, int z)
            {
                Number tileNumber = new(x, y, z);
                RasterTile tile =
                    new(tileNumber, GeoCoordinateSystem, tileSize, tmsCompatible)
                    {
                        BandsCount = bandsCount, Interpolation = interpolation
                    };

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
        /// Crops current <see cref="Raster"/> on <see cref="RasterTile"/>s
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
        /// <returns><see cref="IAsyncEnumerable{T}"/> of <see cref="RasterTile"/>s</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="RasterException"/>
        public IAsyncEnumerable<RasterTile> WriteTilesToAsyncEnumerable(int minZ, int maxZ,
                                                                        bool tmsCompatible = false, Size tileSize = null,
                                                                        NetVips.Enums.Kernel interpolation = NetVips.Enums.Kernel.Lanczos3,
                                                                        int bandsCount = RasterTile.DefaultBandsCount,
                                                                        int tileCacheCount = 1000, int threadsCount = 0,
                                                                        IProgress<double> progress = null,
                                                                        Action<string> printTimeAction = null)
        {
            // All preconditions checks are done in WriteTilesToChannelAsync method

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            WriteTilesToChannelAsync(channel.Writer, minZ, maxZ, tmsCompatible, tileSize,
                                     interpolation, bandsCount, tileCacheCount,
                                     threadsCount, progress, printTimeAction)
               .ContinueWith(_ => channel.Writer.Complete(), TaskScheduler.Current);

            return channel.Reader.ReadAllAsync();
        }

        #endregion

        #region Join tiles

        #region Create overview tiles

        /// <summary>
        /// Create overview <see cref="RasterTile"/>s for specified
        /// <see cref="GeoCoordinate"/>s using finding lower tiles inside
        /// <see cref="HashSet{T}"/> of <see cref="RasterTile"/>s
        /// </summary>
        /// <inheritdoc cref="JoinTilesIntoBytes"/>
        /// <param name="channelWriter"><see cref="Channel{T}"/> to write
        /// <see cref="RasterTile"/>s</param>
        /// <param name="minZ">Minimal overview zoom</param>
        /// <param name="maxZ">Maximal overview zoom</param>
        /// <param name="tiles">Input <see cref="RasterTile"/>s from which
        /// overview will be created</param>
        /// <param name="coordinateSystem">Target <see cref="RasterTile"/>s coordinate system</param>
        /// <param name="isBuffered">Is input <see cref="RasterTile"/>s contains
        /// data inside <see cref="ITile.Bytes"/> property?
        /// <remarks><para/>If set to <see langword="false"/>, will use
        /// <see cref="ITile.Path"/> to get input tiles's data</remarks></param>
        /// <param name="tileSize"></param>
        /// <param name="extension"></param>
        /// <param name="tmsCompatible">Are ready <see cref="RasterTile"/>s tms-compatible?
        /// <remarks><para/><see langword="false"/> by default</remarks></param>
        /// <param name="bandsCount"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void CreateOverviewTiles(ChannelWriter<RasterTile> channelWriter, int minZ, int maxZ,
                                        HashSet<RasterTile> tiles, bool isBuffered, CoordinateSystem coordinateSystem,
                                        Size tileSize = null, TileExtension extension = TileExtension.Png,
                                        bool tmsCompatible = false, int bandsCount = 4)
        {
            #region Preconditions checks

            if (channelWriter == null) throw new ArgumentNullException(nameof(channelWriter));
            if (minZ < 0) throw new ArgumentOutOfRangeException(nameof(minZ));
            if (maxZ < minZ) throw new ArgumentOutOfRangeException(nameof(maxZ));

            #endregion

            for (int z = minZ; z <= maxZ; z++)
            {
                (Number minNumber, Number maxNumber) =
                    GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate, z, Tile.DefaultSize, false);

                for (int x = minNumber.X; x <= maxNumber.X; x++)
                {
                    int x1 = x;
                    int z1 = z;
                    Parallel.For(minNumber.Y, maxNumber.Y + 1, y =>
                    {
                        Number number = new(x1, y, z1);

                        RasterTile tile =
                            new(number, coordinateSystem, tileSize, tmsCompatible)
                            {
                                Extension = extension, BandsCount = bandsCount
                            };
                        CreateOverviewTile(ref tile, tiles, isBuffered);

                        channelWriter.TryWrite(tile);
                    });
                }
            }
        }

        /// <inheritdoc cref="CreateOverviewTiles"/>
        public Task CreateOverviewTilesAsync(ChannelWriter<RasterTile> channelWriter, int minZ, int maxZ,
                                        HashSet<RasterTile> tiles, bool isBuffered, CoordinateSystem coordinateSystem,
                                        Size tileSize = null, TileExtension extension = TileExtension.Png,
                                        bool tmsCompatible = false, int bandsCount = 4) => Task.Run(() => CreateOverviewTiles(channelWriter, minZ, maxZ, tiles, isBuffered, coordinateSystem,
            tileSize, extension, tmsCompatible, bandsCount));

        #endregion

        #region Create overview tile

        /// <summary>
        /// Creates specified overview <see cref="RasterTile"/>
        /// from array of lower <see cref="RasterTile"/>s
        /// </summary>
        /// <param name="targetTile">Target <see cref="RasterTile"/></param>
        /// <param name="baseTiles">Collection of lower <see cref="RasterTile"/>s</param>
        /// <param name="isBuffered">Is input <see cref="RasterTile"/>s contains
        /// data inside <see cref="ITile.Bytes"/> property?
        /// <remarks><para/>If set to <see langword="false"/>, will use
        /// <see cref="ITile.Path"/> to get input tiles's data</remarks></param>
        /// <exception cref="ArgumentNullException"/>
        public static void CreateOverviewTile(ref RasterTile targetTile, HashSet<RasterTile> baseTiles,
                                              bool isBuffered)
        {
            #region Preconditions checks

            if (targetTile == null) throw new ArgumentNullException(nameof(targetTile));
            if (baseTiles == null) throw new ArgumentNullException(nameof(baseTiles));

            #endregion

            Number[] numbers = targetTile.Number.GetLowerNumbers();

            using RasterTile lower0 = baseTiles.FirstOrDefault(t => t.Number == numbers[0]);
            using RasterTile lower1 = baseTiles.FirstOrDefault(t => t.Number == numbers[1]);
            using RasterTile lower2 = baseTiles.FirstOrDefault(t => t.Number == numbers[2]);
            using RasterTile lower3 = baseTiles.FirstOrDefault(t => t.Number == numbers[3]);

            CreateOverviewTile(ref targetTile, lower0, lower1, lower2, lower3, isBuffered);
        }

        /// <summary>
        /// Creates specified overview <see cref="RasterTile"/>
        /// from 4 lower <see cref="RasterTile"/>s
        /// </summary>
        /// <inheritdoc cref="JoinTilesIntoImage(RasterTile,RasterTile,RasterTile,RasterTile,bool,Images.Size,int)"/>
        /// <param name="targetTile">Target <see cref="RasterTile"/></param>
        /// <param name="tile0"></param>
        /// <param name="tile1"></param>
        /// <param name="tile2"></param>
        /// <param name="tile3"></param>
        /// <param name="isBuffered"></param>
        /// <exception cref="ArgumentNullException"/>
        public static void CreateOverviewTile(ref RasterTile targetTile, RasterTile tile0, RasterTile tile1,
                                              RasterTile tile2, RasterTile tile3, bool isBuffered)
        {
            #region Preconditions checks

            if (targetTile == null) throw new ArgumentNullException(nameof(targetTile));

            #endregion

            targetTile.Bytes = JoinTilesIntoBytes(tile0, tile1, tile2, tile3, isBuffered, targetTile.Size,
                                                  targetTile.BandsCount, targetTile.Extension);
        }

        #endregion

        #region Join tiles into bytes

        /// <summary>
        /// Join 4 <see cref="RasterTile"/>s into
        /// collection of <see cref="byte"/>s
        /// <para/>if all <see cref="RasterTile"/>s are
        /// <see langword="null"/> -- returns <see langword="null"/>
        /// </summary>
        /// <returns>Collection of ready image's <see cref="byte"/>s</returns>
        /// <inheritdoc cref="JoinTilesIntoImage(RasterTile,RasterTile,RasterTile,RasterTile,bool,Images.Size,int)"/>
        /// <param name="tile0"></param>
        /// <param name="tile1"></param>
        /// <param name="tile2"></param>
        /// <param name="tile3"></param>
        /// <param name="isBuffered"></param>
        /// <param name="tileSize"></param>
        /// <param name="bandsCount"></param>
        /// <param name="extension"><see cref="TileExtension"/> of ready <see cref="RasterTile"/></param>
        public static IEnumerable<byte> JoinTilesIntoBytes(RasterTile tile0, RasterTile tile1, RasterTile tile2,
                                                           RasterTile tile3, bool isBuffered, Size tileSize,
                                                           int bandsCount, TileExtension extension)
        {
            Image image = JoinTilesIntoImage(tile0, tile1, tile2, tile3, isBuffered, tileSize, bandsCount);

            byte[] result = image?.WriteToBuffer(Tile.GetExtensionString(extension));
            image?.Dispose();

            return result;
        }

        #endregion

        #region Join tiles into image

        /// <summary>
        /// Join 4 <see cref="RasterTile"/>s into
        /// one <see cref="Image"/>;
        /// <para/>if all <see cref="RasterTile"/>s are
        /// <see langword="null"/> -- returns <see langword="null"/>
        /// </summary>
        /// <returns>Ready <see cref="Image"/></returns>
        /// <param name="tile0">Upper-left <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile1">Upper-right <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile2">Lower-left <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile3">Lower-right <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="isBuffered">Is input <see cref="RasterTile"/>s contains
        /// data inside <see cref="ITile.Bytes"/> property?
        /// <remarks><para/>If set to <see langword="false"/>, will use
        /// <see cref="ITile.Path"/> to get input tiles's data</remarks></param>
        /// <param name="tileSize"><see cref="Images.Size"/> of input
        /// and target <see cref="RasterTile"/></param>
        /// <param name="bandsCount">Count of bands in target <see cref="RasterTile"/>
        /// <remarks><para/>must be in range (0-5)</remarks></param>
        /// <returns>Ready <see cref="Image"/></returns>
        public static Image JoinTilesIntoImage(RasterTile tile0, RasterTile tile1,
                                               RasterTile tile2, RasterTile tile3,
                                               bool isBuffered, Size tileSize,
                                               int bandsCount)
        {
            // ReSharper disable once RemoveRedundantBraces
            if (isBuffered)
            {
                return JoinTilesIntoImage(tile0?.Bytes, tile1?.Bytes,
                                          tile2?.Bytes, tile3?.Bytes,
                                          tileSize, bandsCount);
            }

            return JoinTilesIntoImage(tile0?.Path, tile1?.Path, tile2?.Path, tile3?.Path,
                                      tileSize, bandsCount);
        }

        /// <inheritdoc cref="JoinTilesIntoImage(RasterTile,RasterTile,RasterTile,RasterTile,bool,Images.Size,int)"/>
        public static Task<Image> JoinTilesIntoImageAsync(RasterTile tile0, RasterTile tile1, RasterTile tile2,
                                                          RasterTile tile3, bool isBuffered, Size tileSize,
                                                          int bandsCount) => Task.Run(() => JoinTilesIntoImage(tile0, tile1, tile2, tile3, isBuffered, tileSize, bandsCount));

        /// <summary>
        /// Join arrays of <see cref="byte"/> of
        /// 4 <see cref="RasterTile"/>s into
        /// one <see cref="Image"/>
        /// <remarks><para/>if all arrays are <see langword="null"/>
        /// or empty -- returns <see langword="null"/></remarks>
        /// </summary>
        /// <param name="tile0Bytes">Bytes of upper-left <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile1Bytes">Bytes of upper-right <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile2Bytes">Bytes of lower-left <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile3Bytes">Bytes of lower-right <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tileSize"><see cref="Images.Size"/> of input
        /// and target <see cref="RasterTile"/></param>
        /// <param name="bandsCount">Count of bands in target <see cref="RasterTile"/>
        /// <remarks><para/>must be in range (0-5)</remarks></param>
        /// <returns>Ready <see cref="Image"/></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static Image JoinTilesIntoImage(IEnumerable<byte> tile0Bytes, IEnumerable<byte> tile1Bytes,
                                               IEnumerable<byte> tile2Bytes, IEnumerable<byte> tile3Bytes,
                                               Size tileSize, int bandsCount)
        {
            #region Preconditions checks

            if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));
            if (bandsCount < 1 || bandsCount > 4) throw new ArgumentOutOfRangeException(nameof(bandsCount));

            #endregion

            byte[][] bytes = new byte[4][];
            bytes[0] = tile0Bytes?.ToArray() ?? Array.Empty<byte>();
            bytes[1] = tile1Bytes?.ToArray() ?? Array.Empty<byte>();
            bytes[2] = tile2Bytes?.ToArray() ?? Array.Empty<byte>();
            bytes[3] = tile3Bytes?.ToArray() ?? Array.Empty<byte>();

            Image[] images = new Image[4];

            Size size = new(tileSize.Width / 2, tileSize.Height / 2);
            bool empty = true;

            for (int i = 0; i < 4; i++)
            {
                if (bytes[i].Any())
                {
                    empty = false;
                    images[i] = Image.NewFromBuffer(bytes[i]).ThumbnailImage(size.Width, size.Height);
                }
                else
                {
                    images[i] = Image.Black(size.Width, size.Height, bandsCount);
                }
            }

            return empty ? null : Image.Arrayjoin(images, 2);
        }

        /// <inheritdoc cref="JoinTilesIntoImage(IEnumerable{byte},IEnumerable{byte},IEnumerable{byte},IEnumerable{byte},Images.Size,int)"/>
        public static Task<Image> JoinTilesIntoImageAsync(IEnumerable<byte> tile0Bytes, IEnumerable<byte> tile1Bytes,
                                                          IEnumerable<byte> tile2Bytes, IEnumerable<byte> tile3Bytes,
                                                          Size tileSize, int bandsCount) => Task.Run(() => JoinTilesIntoImage(tile0Bytes, tile1Bytes, tile2Bytes, tile3Bytes, tileSize, bandsCount));

        /// <summary>
        /// Join 4 <see cref="RasterTile"/>s into
        /// one <see cref="Image"/>
        /// <remarks><para/>if all 4 paths are <see langword="null"/>
        /// or empty <see cref="string"/>s -- returns <see langword="null"/></remarks>
        /// </summary>
        /// <param name="tile0Path">Path of upper-left <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile1Path">Path of upper-right <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile2Path">Path of lower-left <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tile3Path">Path of lower-right <see cref="RasterTile"/>
        /// <remarks><para/>if set to <see langword="null"/>,
        /// empty tile will be created</remarks></param>
        /// <param name="tileSize"><see cref="Images.Size"/> of input
        /// and target <see cref="RasterTile"/></param>
        /// <param name="bandsCount">Count of bands in target <see cref="RasterTile"/>
        /// <remarks><para/>must be in range (0-5)</remarks></param>
        /// <returns>Ready <see cref="Image"/></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static Image JoinTilesIntoImage(string tile0Path, string tile1Path,
                                               string tile2Path, string tile3Path,
                                               Size tileSize, int bandsCount)
        {
            #region Preconditions checks

            if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));
            if (bandsCount < 1 || bandsCount > 4) throw new ArgumentOutOfRangeException(nameof(bandsCount));

            #endregion

            string[] paths = new string[4];
            paths[0] = string.IsNullOrWhiteSpace(tile0Path) ? string.Empty : tile0Path;
            paths[1] = string.IsNullOrWhiteSpace(tile1Path) ? string.Empty : tile1Path;
            paths[2] = string.IsNullOrWhiteSpace(tile2Path) ? string.Empty : tile2Path;
            paths[3] = string.IsNullOrWhiteSpace(tile3Path) ? string.Empty : tile3Path;

            Image[] images = new Image[4];

            Size size = new(tileSize.Width / 2, tileSize.Height / 2);
            bool empty = true;

            for (int i = 0; i < 4; i++)
            {
                if (File.Exists(paths[i]))
                {
                    empty = false;

                    // TODO: image not closing
                    // See https://github.com/kleisauke/net-vips/issues/84
                    //images[i] = Image.NewFromFile(paths[i], true).ThumbnailImage(size.Width, size.Height);

                    byte[] bytes = File.ReadAllBytes(paths[i]);
                    images[i] = Image.NewFromBuffer(bytes).ThumbnailImage(size.Width, size.Height);
                }
                else
                {
                    images[i] = Image.Black(size.Width, size.Height, bandsCount);
                }
            }

            return empty ? null : Image.Arrayjoin(images, 2);
        }

        /// <inheritdoc cref="JoinTilesIntoImage(string,string,string,string,Images.Size,int)"/>
        public static Task<Image> JoinTilesIntoImageAsync(string tile0Path, string tile1Path, string tile2Path, string tile3Path,
                                                          Size tileSize, int bandsCount) => Task.Run(() => JoinTilesIntoImage(tile0Path, tile1Path, tile2Path, tile3Path, tileSize,
            bandsCount));

        #endregion

        #endregion

        #endregion
    }
}
