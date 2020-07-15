#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Extensions;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Tiles;
using NetVips;

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// Class for creating raster tiles.
    /// </summary>
    public sealed class Raster : IImage
    {
        #region Properties

        #region Private

        /// <summary>
        /// This image's data.
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

        public CoordinateType GeoCoordinateType { get; }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        #endregion

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Creates new <see cref="Raster"/> object.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff image.</param>
        public Raster(FileInfo inputFileInfo, long maxMemoryCache = 2147483648,
                      CoordinateType coordinateType = CoordinateType.Geodetic)
        {
            //Disable NetVips warnings for tiff.
            NetVipsHelper.DisableLog();

            #region Check parameters

            CheckHelper.CheckFile(inputFileInfo, true);

            #endregion

            bool memory = inputFileInfo.Length <= maxMemoryCache;
            Data = Image.NewFromFile(inputFileInfo.FullName, memory, NetVips.Enums.Access.Random);

            //Get border coordinates и raster sizes.
            Size = new Size(Data.Width, Data.Height);

            GeoCoordinateType = coordinateType;

            (MinCoordinate, MaxCoordinate) = Gdal.Gdal.GetImageBorders(inputFileInfo, Size, GeoCoordinateType);
        }

        public Raster(byte[] inputBytes)
        {
            //Disable NetVips warnings for tiff.
            NetVipsHelper.DisableLog();

            Data = Image.NewFromBuffer(inputBytes, access: NetVips.Enums.Access.Random);

            //Get border coordinates и raster sizes.
            Size = new Size(Data.Width, Data.Height);

            FileInfo inputFileInfo = new FileInfo("tmp.tif");
            Data.WriteToFile(inputFileInfo.FullName);
            //TODO: get coordinates without fileinfo
            (MinCoordinate, MaxCoordinate) = Gdal.Gdal.GetImageBorders(inputFileInfo, Size, GeoCoordinateType);
            inputFileInfo.Delete();
        }

        public Raster(Stream inputStream)
        {
            //Disable NetVips warnings for tiff.
            NetVipsHelper.DisableLog();

            Data = Image.NewFromStream(inputStream, access: NetVips.Enums.Access.Random);

            //Get border coordinates и raster sizes.
            Size = new Size(Data.Width, Data.Height);

            FileInfo inputFileInfo = new FileInfo("tmp.tif");
            Data.WriteToFile(inputFileInfo.FullName);
            //TODO: get coordinates without fileinfo
            (MinCoordinate, MaxCoordinate) = Gdal.Gdal.GetImageBorders(inputFileInfo, Size, GeoCoordinateType);
            inputFileInfo.Delete();
        }

        /// <summary>
        /// Destructor.
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

        /// <summary>
        /// Actually disposes the data.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
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
        /// Writes one tile of current zoom.
        /// <para/>Crops zoom directly from input image.
        /// </summary>
        private Image CreateTileImage(Image tileCache, ITile tile, int bands)
        {
            //TODO: NetVips kernel
            const string kernel = NetVips.Enums.Kernel.Lanczos3;

            //Get postitions and sizes for current tile.
            (Area readArea, Area writeArea) = Area.GetAreas(this, tile);

            // Scaling calculations
            double xScale = (double)writeArea.Size.Width / readArea.Size.Width;
            double yScale = (double)writeArea.Size.Height / readArea.Size.Height;

            // Crop and resize tile
            Image tempTileImage = tileCache.Crop(readArea.X, readArea.Y, readArea.Size.Width, readArea.Size.Height)
                                           .Resize(xScale, kernel, yScale);

            // Add alpha channel if needed
            tempTileImage = tempTileImage.AddBands(bands);

            // Make transparent image and insert tile
            return Image.Black(tile.Size.Width, tile.Size.Height).NewFromImage(0, 0, 0, 0)
                          .Insert(tempTileImage, writeArea.X, writeArea.Y);
        }

        #endregion

        #region Create tile

        //private ITile CreateTile(int x, int y, int z, bool tmsCompatible, string tileExtension, int bands,
        //                         Size size)
        //{
        //    ITile tile = new Tiles.Tile(x, y, z, tmsCompatible: tmsCompatible, extension: tileExtension,
        //                                size: size);
        //    tile.D = WriteTileToEnumerable(tile, bands);

        //    return tile;
        //}

        //private async ValueTask<ITile> CreateTileAsync(int x, int y, int z, bool tmsCompatible, string tileExtension, int bands,
        //                                               Size size) =>
        //    await Task.Run(() => CreateTile(x, y, z, tmsCompatible, tileExtension, bands, size)).ConfigureAwait(false);

        #endregion

        #region WriteTile

        private void WriteTileToFile(Image tileCache, ITile tile, int bands)
        {
            using Image tileImage = CreateTileImage(tileCache, tile, bands);

            //TODO: Validate tileImage, not tile!
            //if (!tile.Validate(false)) return;

            tileImage.WriteToFile(tile.FileInfo.FullName);
        }

        private IEnumerable<byte> WriteTileToEnumerable(Image tileCache, ITile tile, int bands)
        {
            using Image tileImage = CreateTileImage(tileCache, tile, bands);
            //TODO: test this methods
            //return tileImage.WriteToBuffer(tile.Extension);
            return tileImage.WriteToMemory();
        }

        private bool WriteTileToChannel(ITile tile, ChannelWriter<ITile> channelWriter)
        {
            //tile.D = WriteTileToEnumerable(tileCache, tile, bands);

            return tile.Validate(false) && channelWriter.TryWrite(tile);
        }

        #endregion

        #region WriteTiles

        /// <inheritdoc />
        public async ValueTask WriteTilesToDirectoryAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                          Size tileSize,
                                                          bool tmsCompatible = false,
                                                          string tileExtension = Constants.FileExtensions.Png,
                                                          int bands = Constants.Image.Raster.Bands,
                                                          IProgress<double> progress = null, int threadsCount = 0,
                                                          bool isPrintEstimatedTime = true, int tileCacheCount = 1000)
        {
            //TODO: profile argument (geodetic/mercator)

            #region Parameters checking

            progress ??= new Progress<double>();

            #endregion

            ParallelOptions parallelOptions = new ParallelOptions();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);
            double counter = 0.0;

            if (tilesCount <= 0) return;

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            //For each zoom.
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                //Get tiles min/max numbers.
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                                                                                       zoom, tileSize.Width,
                                                                                tmsCompatible);

                //For each tile on given zoom calculate positions/sizes and save as file.
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    int y = tileY;
                    int z = zoom;

                    void MakeTile(int x)
                    {
                        //Create directories for the tile. The overall structure looks like: outputDirectory/zoom/x/y.png.
                        DirectoryInfo tileDirectoryInfo = new DirectoryInfo(Path.Combine(outputDirectoryInfo.FullName, $"{z}", $"{x}"));
                        CheckHelper.CheckDirectory(tileDirectoryInfo);

                        Number tileNumber = new Number(x, y, z);
                        ITile tile = new RasterTile(tileNumber, extension: tileExtension, tmsCompatible: tmsCompatible,
                                                          size: tileSize, bandsCount: bands,
                                                          coordinateType: GeoCoordinateType)
                        {
                            //Warning: OpenLayers requires replacement of tileY to tileY+1
                            FileInfo = new FileInfo(Path.Combine(tileDirectoryInfo.FullName, $"{y}{tileExtension}"))
                        };

                        // ReSharper disable once AccessToDisposedClosure
                        WriteTileToFile(tileCache, tile, bands);

                        //Report progress.
                        counter++;
                        double percentage = counter / tilesCount * 100.0;
                        progress.Report(percentage);

                        //Estimated time left calculation.
                        ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch);
                    }

                    await Task.Run(() => Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, MakeTile))
                              .ConfigureAwait(false);
                }
            }
        }

        //TODO: args order
        public async ValueTask WriteTilesToChannelAsync(ChannelWriter<ITile> channelWriter,
                                                        bool tmsCompatible, string tileExtension,
                                                        IProgress<double> progress, bool isPrintEstimatedTime,
                                                        int minZ, int maxZ, int bands,
                                                        Size tileSize, int threadsCount,
                                                        int tileCacheCount)
        {
            #region Progress stuff

            progress ??= new Progress<double>();

            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);
            double counter = 0.0;

            if (tilesCount <= 0) return;

            #endregion

            ParallelOptions parallelOptions = new ParallelOptions();
            if (threadsCount > 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            //For each zoom.
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                //Get tiles min/max numbers.
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                                                                                zoom, tileSize.Width,
                                                                                tmsCompatible);

                //For each tile on given zoom calculate positions/sizes and save as file.
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    int y = tileY;
                    int z = zoom;

                    void MakeTile(int x)
                    {
                        Number tileNumber = new Number(x, y, z);
                        ITile tile = new RasterTile(tileNumber, extension: tileExtension, tmsCompatible: tmsCompatible,
                                                    size: tileSize, bandsCount: bands,
                                                    coordinateType: GeoCoordinateType);

                        // ReSharper disable once AccessToDisposedClosure
                        tile.Bytes = WriteTileToEnumerable(tileCache, tile, bands);

                        if (!WriteTileToChannel(tile, channelWriter)) return;

                        //Report progress.
                        counter++;
                        double percentage = counter / tilesCount * 100.0;
                        progress.Report(percentage);

                        //Estimated time left calculation.
                        ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch);
                    }

                    await Task.Run(() => Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, MakeTile))
                              .ConfigureAwait(false);
                }
            }
        }

        //TODO: args order
        public async IAsyncEnumerable<ITile> WriteTilesToAsyncEnumerable(bool tmsCompatible, string tileExtension,
                                                                         IProgress<double> progress, bool isPrintEstimatedTime,
                                                                         int minZ, int maxZ, int bands,
                                                                         Size tileSize, int threadsCount,
                                                                         int tileCacheCount)
        {
            #region Progress stuff

            progress ??= new Progress<double>();

            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = Number.GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, tmsCompatible, tileSize);
            double counter = 0.0;

            //if (tilesCount <= 0) yield return null;

            #endregion

            using SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount);
            List<Task<ITile>> tasks = new List<Task<ITile>>();

            // Create tile cache to read data from it
            using Image tileCache = Data.Tilecache(tileSize.Width, tileSize.Height, tileCacheCount, threaded: true);

            //For each specified zoom.
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                //Get tiles min/max numbers.
                (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate,
                                                                                zoom, tileSize.Width,
                                                                                tmsCompatible);

                //For each tile on given zoom calculate positions/sizes and save as file.
                for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
                {
                    //TODO: use Parallel.For somehow
                    for (int tileX = minNumber.X; tileX <= maxNumber.X; tileX++)
                    {
                        await semaphoreSlim.WaitAsync();

                        int x = tileX;
                        int y = tileY;
                        int z = zoom;

                        ITile MakeTile()
                        {
                            Number tileNumber = new Number(x, y, z);
                            ITile tile = new RasterTile(tileNumber, extension: tileExtension,
                                                        tmsCompatible: tmsCompatible, size: tileSize, bandsCount: bands,
                                                        coordinateType: GeoCoordinateType);

                            try
                            {
                                // ReSharper disable once AccessToDisposedClosure
                                tile.Bytes = WriteTileToEnumerable(tileCache, tile, bands);
                                //if (!tile.Validate(false)) return null;
                            }
                            finally
                            {
                                //Report progress.
                                counter++;
                                double percentage = counter / tilesCount * 100.0;
                                progress.Report(percentage);

                                //Estimated time left calculation.
                                ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch);

                                // ReSharper disable once AccessToDisposedClosure
                                semaphoreSlim.Release();
                            }

                            return tile;
                        }

                        tasks.Add(Task.Run(MakeTile));
                    }

                    while (tasks.Count != 0)
                    {
                        Task<ITile> task = await Task.WhenAny(tasks);
                        tasks.Remove(task);
                        ITile tile = await task;

                        if (!Tile.Validate(tile, false)) continue;

                        yield return tile;
                    }

                    tasks.Clear();
                }
            }
        }

        #endregion

        #endregion
    }
}
