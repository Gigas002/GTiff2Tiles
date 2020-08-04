#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable CA1062 // Check args

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
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Tiles;
using NetVips;

// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// Class for creating <see cref="RasterTile"/>s
    /// </summary>
    public class Raster : IImage
    {
        #region Properties

        #region Private

        /// <summary>
        /// This <see cref="Raster"/>'s data
        /// </summary>
        private Image Data { get; }

        #endregion

        #region Public

        /// <inheritdoc />
        public Size Size { get; }

        /// <inheritdoc />
        public GeoCoordinate MinCoordinate { get; }

        /// <inheritdoc />
        public GeoCoordinate MaxCoordinate { get; }

        /// <inheritdoc />
        public CoordinateType GeoCoordinateType { get; }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        #endregion

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Creates new <see cref="Raster"/> object
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff image</param>
        /// <param name="maxMemoryCache">Max size of input image to store in RAM
        /// <remarks><para/>2GB by default</remarks></param>
        /// <param name="coordinateType">Type of coordinates
        /// <remarks><para/><see cref="GeodeticCoordinate"/> by default</remarks></param>
        public Raster(FileInfo inputFileInfo, long maxMemoryCache = 2147483648,
                      CoordinateType coordinateType = CoordinateType.Geodetic)
        {
            // Disable NetVips warnings for tiff
            NetVipsHelper.DisableLog();

            #region Check parameters

            CheckHelper.CheckFile(inputFileInfo.FullName, true);

            #endregion

            bool memory = inputFileInfo.Length <= maxMemoryCache;
            Data = Image.NewFromFile(inputFileInfo.FullName, memory, NetVips.Enums.Access.Random);

            // Get border coordinates и raster sizes
            Size = new Size(Data.Width, Data.Height);

            GeoCoordinateType = coordinateType;
            (MinCoordinate, MaxCoordinate) = GdalWorker.GetImageBorders(inputFileInfo.FullName, Size, GeoCoordinateType);
        }

        /// <summary>
        /// Creates new <see cref="Raster"/> object
        /// </summary>
        /// <param name="inputBytes"><see cref="IEnumerable{T}"/> of GeoTiff's
        /// <see cref="byte"/>s</param>
        /// <param name="coordinateType">Type of coordinates
        /// <remarks><para/><see cref="GeodeticCoordinate"/> by default</remarks></param>
        public Raster(IEnumerable<byte> inputBytes, CoordinateType coordinateType = CoordinateType.Geodetic)
        {
            // Disable NetVips warnings for tiff
            NetVipsHelper.DisableLog();

            Data = Image.NewFromBuffer(inputBytes.ToArray(), access: NetVips.Enums.Access.Random);

            // Get border coordinates и raster sizes
            Size = new Size(Data.Width, Data.Height);

            GeoCoordinateType = coordinateType;

            // TODO: get coordinates without fileinfo
            FileInfo inputFileInfo = new FileInfo("tmp.tif");
            Data.WriteToFile(inputFileInfo.FullName);
            (MinCoordinate, MaxCoordinate) = GdalWorker.GetImageBorders(inputFileInfo.FullName, Size, GeoCoordinateType);
            inputFileInfo.Delete();
        }

        /// <summary>
        /// Creates new <see cref="Raster"/> object
        /// </summary>
        /// <param name="inputStream"><see cref="Stream"/> with GeoTiff</param>
        /// <param name="coordinateType">Type of coordinates
        /// <remarks><para/><see cref="GeodeticCoordinate"/> by default</remarks></param>
        public Raster(Stream inputStream, CoordinateType coordinateType = CoordinateType.Geodetic)
        {
            // Disable NetVips warnings for tiff
            NetVipsHelper.DisableLog();

            Data = Image.NewFromStream(inputStream, access: NetVips.Enums.Access.Random);

            // Get border coordinates и raster sizes
            Size = new Size(Data.Width, Data.Height);

            GeoCoordinateType = coordinateType;

            // TODO: get coordinates without fileinfo
            FileInfo inputFileInfo = new FileInfo("tmp.tif");
            Data.WriteToFile(inputFileInfo.FullName);
            (MinCoordinate, MaxCoordinate) = GdalWorker.GetImageBorders(inputFileInfo.FullName, Size, GeoCoordinateType);
            inputFileInfo.Delete();
        }

        /// <summary>
        /// Destructor
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
                //Occurs only if called by programmer. Dispose static things here.
            }

            Data.Dispose();

            IsDisposed = true;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            try
            {
                Dispose();

                return default;
            }
            catch (Exception exception)
            {
                return new ValueTask(Task.FromException(exception));
            }
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
        /// <param name="interpolation">Interpolation of ready tiles</param>
        /// <returns>Ready <see cref="Image"/> for <see cref="RasterTile"/></returns>
        public Image CreateTileImage(Image tileCache, RasterTile tile, string interpolation)
        {
            if (tileCache == null) throw new ArgumentNullException(nameof(tileCache));
            if (tile == null) throw new ArgumentNullException(nameof(tile));

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
            return Image.Black(tile.Size.Width, tile.Size.Height).NewFromImage(0, 0, 0, 0)
                        .Insert(tempTileImage, (int)writeArea.OriginCoordinate.X,
                                (int)writeArea.OriginCoordinate.Y);
        }

        #endregion

        #region WriteTile

        /// <inheritdoc/>
        public void WriteTileToFile(Image tileCache, RasterTile tile, string interpolation)
        {
            using Image tileImage = CreateTileImage(tileCache, tile, interpolation);

            tileImage.WriteToFile(tile.FileInfo.FullName);
        }

        /// <inheritdoc/>
        public async ValueTask WriteTileToFileAsync(Image tileCache, RasterTile tile, string interpolation) =>
            await Task.Run(() => WriteTileToFile(tileCache, tile, interpolation)).ConfigureAwait(false);

        /// <inheritdoc/>
        public IEnumerable<byte> WriteTileToEnumerable(Image tileCache, RasterTile tile,
                                                       string interpolation)
        {
            using Image tileImage = CreateTileImage(tileCache, tile, interpolation);

            // TODO: test this methods
            //return tileImage.WriteToBuffer(tile.Extension);
            return tileImage.WriteToMemory();
        }

        /// <inheritdoc/>
        public bool WriteTileToChannel(Image tileCache, RasterTile tile, ChannelWriter<ITile> channelWriter,
                                       string interpolation)
        {
            if (tile == null) throw new ArgumentNullException(nameof(tile));
            if (channelWriter == null) throw new ArgumentNullException(nameof(channelWriter));

            tile.Bytes = WriteTileToEnumerable(tileCache, tile, interpolation);

            return tile.Validate(false) && channelWriter.TryWrite(tile);
        }

        /// <inheritdoc/>
        public ValueTask WriteTileToChannelAsync(Image tileCache, RasterTile tile,
                                                 ChannelWriter<ITile> channelWriter, string interpolation)
        {
            if (tile == null) throw new ArgumentNullException(nameof(tile));
            if (channelWriter == null) throw new ArgumentNullException(nameof(channelWriter));

            tile.Bytes = WriteTileToEnumerable(tileCache, tile, interpolation);

            return channelWriter.WriteAsync(tile);
        }

        #endregion

        #region WriteTiles

        /// <inheritdoc />
        public void WriteTilesToDirectory(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                          bool tmsCompatible = false, Size tileSize = null,
                                          string tileExtension = FileExtensions.Png,
                                          string interpolation = Interpolations.Lanczos3,
                                          int bandsCount = RasterTile.DefaultBandsCount,
                                          int tileCacheCount = 1000, int threadsCount = 0,
                                          IProgress<double> progress = null,
                                          bool isPrintEstimatedTime = false)
        {
            #region Parameters checking

            progress ??= new Progress<double>();
            tileSize ??= Tile.DefaultSize;

            ParallelOptions parallelOptions = new ParallelOptions();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            #region Progress stuff

            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);
            double counter = 0.0;

            if (tilesCount <= 0) return;

            #endregion

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            void MakeTile(int x, int y, int z)
            {
                // Create directories for the tile
                // The overall structure looks like: outputDirectory/zoom/x/y.png
                DirectoryInfo tileDirectoryInfo = new DirectoryInfo(Path.Combine(outputDirectoryInfo.FullName, $"{z}", $"{x}"));
                CheckHelper.CheckDirectory(tileDirectoryInfo.FullName);

                Number tileNumber = new Number(x, y, z);
                RasterTile tile = new RasterTile(tileNumber, extension: tileExtension, tmsCompatible: tmsCompatible,
                    size: tileSize, bandsCount: bandsCount, coordinateType: GeoCoordinateType)
                {
                    // Warning: OpenLayers requires replacement of tileY to tileY+1
                    FileInfo = new FileInfo(Path.Combine(tileDirectoryInfo.FullName, $"{y}{tileExtension}"))
                };

                // ReSharper disable once AccessToDisposedClosure
                WriteTileToFile(tileCache, tile, interpolation);

                // Report progress
                counter++;
                double percentage = counter / tilesCount * 100.0;
                progress.Report(percentage);

                // Estimated time left calculation
                ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch);
            }

            // For each zoom
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                        zoom, tileSize.Width, tmsCompatible);

                // For each tile on given zoom calculate positions/sizes and save as file
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    int y = tileY;
                    int z = zoom;

                    Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, x => MakeTile(x, y, z));
                }
            }
        }

        /// <inheritdoc />
        public async ValueTask WriteTilesToDirectoryAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                          bool tmsCompatible = false, Size tileSize = null,
                                                          string tileExtension = FileExtensions.Png,
                                                          string interpolation = Interpolations.Lanczos3,
                                                          int bandsCount = RasterTile.DefaultBandsCount,
                                                          int tileCacheCount = 1000, int threadsCount = 0,
                                                          IProgress<double> progress = null,
                                                          bool isPrintEstimatedTime = false) =>
            await Task.Run(() => WriteTilesToDirectory(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileSize,
                                                       tileExtension, interpolation, bandsCount, tileCacheCount,
                                                       threadsCount, progress, isPrintEstimatedTime))
                      .ConfigureAwait(false);

        /// <inheritdoc />
        public void WriteTilesToChannel(ChannelWriter<ITile> channelWriter, int minZ, int maxZ,
                                        bool tmsCompatible = false, Size tileSize = null,
                                        string interpolation = Interpolations.Lanczos3,
                                        int bandsCount = RasterTile.DefaultBandsCount,
                                        int tileCacheCount = 1000, int threadsCount = 0,
                                        IProgress<double> progress = null,
                                        bool isPrintEstimatedTime = false)
        {
            #region Parameters checking

            progress ??= new Progress<double>();
            tileSize ??= Tile.DefaultSize;

            ParallelOptions parallelOptions = new ParallelOptions();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            #region Progress stuff

            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);
            double counter = 0.0;

            if (tilesCount <= 0) return;

            #endregion

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            void MakeTile(int x, int y, int z)
            {
                Number tileNumber = new Number(x, y, z);
                RasterTile tile = new RasterTile(tileNumber, tmsCompatible: tmsCompatible,
                    size: tileSize, bandsCount: bandsCount, coordinateType: GeoCoordinateType);

                // ReSharper disable once AccessToDisposedClosure
                if (!WriteTileToChannel(tileCache, tile, channelWriter, interpolation)) return;

                // Report progress
                counter++;
                double percentage = counter / tilesCount * 100.0;
                progress.Report(percentage);

                // Estimated time left calculation
                ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch);
            }

            // For each zoom
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                    zoom, tileSize.Width, tmsCompatible);

                // For each tile on given zoom calculate positions/sizes and save as file
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    int y = tileY;
                    int z = zoom;

                    Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, x => MakeTile(x, y, z));
                }
            }
        }

        /// <inheritdoc />
        public async ValueTask WriteTilesToChannelAsync(ChannelWriter<ITile> channelWriter, int minZ, int maxZ,
                                                        bool tmsCompatible = false, Size tileSize = null,
                                                        string interpolation = Interpolations.Lanczos3,
                                                        int bandsCount = RasterTile.DefaultBandsCount,
                                                        int tileCacheCount = 1000, int threadsCount = 0,
                                                        IProgress<double> progress = null,
                                                        bool isPrintEstimatedTime = false) =>
            await Task.Run(() => WriteTilesToChannel(channelWriter, minZ, maxZ, tmsCompatible, tileSize, interpolation,
                                                     bandsCount, tileCacheCount, threadsCount, progress,
                                                     isPrintEstimatedTime)).ConfigureAwait(false);

        /// <inheritdoc />
        public IEnumerable<ITile> WriteTilesToEnumerable(int minZ, int maxZ,
                                                         bool tmsCompatible = false, Size tileSize = null,
                                                         string interpolation = Interpolations.Lanczos3,
                                                         int bandsCount = RasterTile.DefaultBandsCount,
                                                         int tileCacheCount = 1000,
                                                         IProgress<double> progress = null,
                                                         bool isPrintEstimatedTime = false)
        {
            #region Parameters checking

            progress ??= new Progress<double>();
            tileSize ??= Tile.DefaultSize;

            #region Progress stuff

            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);
            double counter = 0.0;

            if (tilesCount <= 0) yield break;

            #endregion

            #endregion

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            ITile MakeTile(int x, int y, int z)
            {
                Number tileNumber = new Number(x, y, z);
                RasterTile tile = new RasterTile(tileNumber, tmsCompatible: tmsCompatible,
                    size: tileSize, bandsCount: bandsCount, coordinateType: GeoCoordinateType);

                tile.Bytes = WriteTileToEnumerable(tileCache, tile, interpolation);

                // Report progress
                counter++;
                double percentage = counter / tilesCount * 100.0;
                progress.Report(percentage);

                // Estimated time left calculation
                ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch);

                return tile;
            }

            // For each specified zoom
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                        zoom, tileSize.Width, tmsCompatible);

                // For each tile on given zoom calculate positions/sizes and save as file
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    for (int tileX = minNumber.X; tileX <= maxNumber.X; tileX++)
                    {
                        yield return MakeTile(tileX, tileY, zoom);
                    }
                }
            }
        }

        /// <inheritdoc />
        public IAsyncEnumerable<ITile> WriteTilesToAsyncEnumerable(int minZ, int maxZ,
                                                                   bool tmsCompatible = false, Size tileSize = null,
                                                                   string interpolation = Interpolations.Lanczos3,
                                                                   int bandsCount = RasterTile.DefaultBandsCount,
                                                                   int tileCacheCount = 1000, int threadsCount = 0,
                                                                   IProgress<double> progress = null,
                                                                   bool isPrintEstimatedTime = false)
        {
            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            WriteTilesToChannelAsync(channel.Writer, minZ, maxZ, tmsCompatible, tileSize,
                                     interpolation, bandsCount, tileCacheCount,
                                     threadsCount, progress, isPrintEstimatedTime)
               .AsTask().ContinueWith(_ => channel.Writer.Complete(), TaskScheduler.Current);

            return channel.Reader.ReadAllAsync();
        }

        #endregion

        #endregion
    }
}

#pragma warning restore CA1031 // Do not catch general exception types
#pragma warning restore CA1062 // Check args
