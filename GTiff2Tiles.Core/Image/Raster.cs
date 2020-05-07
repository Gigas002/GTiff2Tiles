#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants.Image;
using GTiff2Tiles.Core.Exceptions.Image;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Localization;
using NetVips;

namespace GTiff2Tiles.Core.Image
{
    public sealed class Raster : IImage
    {
        #region Release

        #region Properties

        #region Private

        /// <summary>
        /// Output directory.
        /// </summary>
        private DirectoryInfo OutputDirectoryInfo { get; set; }

        /// <summary>
        /// Minimum cropped zoom.
        /// </summary>
        private int MinZ { get; set; }

        /// <summary>
        /// Maximum cropped zoom.
        /// </summary>
        private int MaxZ { get; set; }

        /// <summary>
        /// Shows if tms tiles on output are created.
        /// </summary>
        private bool TmsCompatible { get; set; }

        /// <summary>
        /// Extension of ready tiles.
        /// </summary>
        private string TileExtension { get; set; }

        #endregion

        #region Public

        /// <inheritdoc />
        public int RasterXSize { get; }

        /// <inheritdoc />
        public int RasterYSize { get; }

        /// <inheritdoc />
        public double MinX { get; }

        /// <inheritdoc />
        public double MinY { get; }

        /// <inheritdoc />
        public double MaxX { get; }

        /// <inheritdoc />
        public double MaxY { get; }

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public NetVips.Image Data { get; }

        #endregion

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Creates new <see cref="Raster"/> object.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff image.</param>
        public Raster(FileInfo inputFileInfo)
        {
            //Disable NetVips warnings for tiff.
            NetVipsHelper.DisableLog();

            #region Check parameters

            CheckHelper.CheckFile(inputFileInfo, true);

            #endregion

            Data = NetVips.Image.NewFromFile(inputFileInfo.FullName, access: NetVips.Enums.Access.Random);

            //Get border coordinates и raster sizes.
            try
            {
                RasterXSize = Data.Width;
                RasterYSize = Data.Height;
                (MinX, MinY, MaxX, MaxY) = Gdal.Gdal.GetImageBorders(inputFileInfo, RasterXSize, RasterYSize);
            }
            catch (Exception exception)
            {
                throw new RasterException(string.Format(Strings.UnableToGetCoordinates, nameof(inputFileInfo),
                                                       exception));
            }
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

        #region Private

        /// <summary>
        /// Resizes tile before creating it
        /// </summary>
        /// <param name="tileImage">Basic image to resize</param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="kernel"></param>
        /// <param name="interpolation"></param>
        /// <param name="isCentre"></param>
        /// <returns></returns>
        private static NetVips.Image Resize(NetVips.Image tileImage, double xScale, double yScale,
                                    string kernel = NetVips.Enums.Kernel.Lanczos3,
                                    string interpolation = Interpolations.Bicubic,
                                    bool isCentre = false)
        {
            // We could just use vips_resize if we use centre sampling convention
            if (isCentre) return tileImage.Resize(xScale, kernel, yScale);

            // Otherwise, we need to implement vips_resize for ourselves

            // Calculate integral box shrink
            // We will get the best quality (but be the slowest) if we let reduce
            // do all the work. Leave it the final 200 - 300% to do as a compromise
            // for efficiency.
            int xShirnk = Math.Max(1, (int)Math.Floor(1.0 / (xScale * 2.0)));
            int yShrink = Math.Max(1, (int)Math.Floor(1.0 / (yScale * 2.0)));

            // Fast, integral box-shrink
            if (yShrink > 1)
            {
                tileImage = tileImage.Shrinkv(yShrink);
                yScale *= yShrink;
            }

            if (xShirnk > 1)
            {
                tileImage = tileImage.Shrinkh(xShirnk);
                xScale *= xShirnk;
            }

            // Any residual downsizing
            if (yScale < 1.0) tileImage = tileImage.Reducev(1.0 / yScale, kernel, false);
            if (xScale < 1.0) tileImage = tileImage.Reduceh(1.0 / xScale, kernel, false);

            // Any upsizing
            if (!(xScale > 1.0) && !(yScale > 1.0)) return tileImage;
            // Floating point affine transformation
            //double id = isCentre ? 0.5 : 0.0;
            const double id = 0.0;

            // Floating point affine transformation
            using Interpolate interpolate = Interpolate.NewFromName(interpolation);
            if (xScale > 1.0 && yScale > 1.0)
                tileImage = tileImage.Affine(new[] { xScale, 0.0, 0.0, yScale }, interpolate, idx: id, idy: id,
                                             extend: NetVips.Enums.Extend.Copy);
            else if (xScale > 1.0)
                tileImage = tileImage.Affine(new[] { xScale, 0.0, 0.0, 1.0 }, interpolate, idx: id, idy: id,
                                             extend: NetVips.Enums.Extend.Copy);
            else
                tileImage = tileImage.Affine(new[] { 1.0, 0.0, 0.0, yScale }, interpolate, idx: id, idy: id,
                                             extend: NetVips.Enums.Extend.Copy);

            return tileImage;
        }

        /// <summary>
        /// Gets number of tiles for current raster
        /// </summary>
        /// <param name="zoom">Current zoom</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <returns>Tuple of tile numbers</returns>
        private (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetTileNumbers(int zoom, bool tmsCompatible)
        {
            //Convert coordinates to tile numbers.
            (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) =
                Tile.TileTools.GetTileNumbersFromCoords(MinX, MinY, MaxX, MaxY, zoom, tmsCompatible);

            return (Math.Max(0, tileMinX), Math.Max(0, tileMinY),
                    Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom + 1)) - 1, tileMaxX),
                    Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom)) - 1, tileMaxY));
        }

        /// <summary>
        /// Calculate size and positions to read/write.
        /// </summary>
        /// <param name="upperLeftX">Tile's upper left x coordinate.</param>
        /// <param name="upperLeftY">Tile's upper left y coordinate.</param>
        /// <param name="lowerRightX">Tile's lower right x coordinate.</param>
        /// <param name="lowerRightY">Tile's lower right y coordinate.</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4, T5, T6, T7, T8}"/> of x/y positions and sizes to read and write tiles.</returns>
        private (int readPosX, int readPosY, int readXSize, int readYSize, int writePosX, int writePosY, int writeXSize,
            int writeYSize) GeoQuery(double upperLeftX, double upperLeftY, double lowerRightX, double lowerRightY)
        {
            //Read from input geotiff in pixels.
            double readPosMinX = RasterXSize * (upperLeftX - MinX) / (MaxX - MinX);
            double readPosMinY = RasterYSize - RasterYSize * (upperLeftY - MinY) / (MaxY - MinY);
            double readPosMaxX = RasterXSize * (lowerRightX - MinX) / (MaxX - MinX);
            double readPosMaxY = RasterYSize - RasterYSize * (lowerRightY - MinY) / (MaxY - MinY);

            //If outside of tiff.
            readPosMinX = readPosMinX < 0.0 ? 0.0 :
                          readPosMinX > RasterXSize ? RasterXSize : readPosMinX;
            readPosMinY = readPosMinY < 0.0 ? 0.0 :
                          readPosMinY > RasterYSize ? RasterYSize : readPosMinY;
            readPosMaxX = readPosMaxX < 0.0 ? 0.0 :
                          readPosMaxX > RasterXSize ? RasterXSize : readPosMaxX;
            readPosMaxY = readPosMaxY < 0.0 ? 0.0 :
                          readPosMaxY > RasterYSize ? RasterYSize : readPosMaxY;

            //Output tile's borders in pixels.
            double tilePixMinX = readPosMinX.Equals(0.0) ? MinX :
                                 readPosMinX.Equals(RasterXSize) ? MaxX : upperLeftX;
            double tilePixMinY = readPosMaxY.Equals(0.0) ? MaxY :
                                 readPosMaxY.Equals(RasterYSize) ? MinY : lowerRightY;
            double tilePixMaxX = readPosMaxX.Equals(0.0) ? MinX :
                                 readPosMaxX.Equals(RasterXSize) ? MaxX : lowerRightX;
            double tilePixMaxY = readPosMinY.Equals(0.0) ? MaxY :
                                 readPosMinY.Equals(RasterYSize) ? MinY : upperLeftY;

            //Positions of dataset to write in tile.
            double writePosMinX = Constants.Image.Raster.TileSize -
                                  Constants.Image.Raster.TileSize * (lowerRightX - tilePixMinX) / (lowerRightX - upperLeftX);
            double writePosMinY = Constants.Image.Raster.TileSize * (upperLeftY - tilePixMaxY) / (upperLeftY - lowerRightY);
            double writePosMaxX = Constants.Image.Raster.TileSize -
                                  Constants.Image.Raster.TileSize * (lowerRightX - tilePixMaxX) / (lowerRightX - upperLeftX);
            double writePosMaxY = Constants.Image.Raster.TileSize * (upperLeftY - tilePixMinY) / (upperLeftY - lowerRightY);

            //Sizes to read and write.
            double readXSize = readPosMaxX - readPosMinX;
            double readYSize = readPosMaxY - readPosMinY;
            double writeXSize = writePosMaxX - writePosMinX;
            double writeYSize = writePosMaxY - writePosMinY;

            //Shifts.
            double readXShift = readPosMinX - (int)readPosMinX;
            readXSize += readXShift;
            double readYShift = readPosMinY - (int)readPosMinY;
            readYSize += readYShift;
            double writeXShift = writePosMinX - (int)writePosMinX;
            writeXSize += writeXShift;
            double writeYShift = writePosMinY - (int)writePosMinY;
            writeYSize += writeYShift;

            //If output image sides are lesser then 1 - make image 1x1 pixels to prevent division by 0.
            writeXSize = writeXSize > 1.0 ? writeXSize : 1.0;
            writeYSize = writeYSize > 1.0 ? writeYSize : 1.0;

            return ((int)readPosMinX, (int)readPosMinY, (int)readXSize, (int)readYSize, (int)writePosMinX,
                    (int)writePosMinY, (int)writeXSize, (int)writeYSize);
        }

        /// <summary>
        /// Writes one tile of current zoom.
        /// <para/>Crops zoom directly from input image.
        /// </summary>
        /// <param name="tileX">Tile x.</param>
        /// <param name="tileY">Tile y.</param>
        /// <param name="zoom">Zoom level.</param>
        private void WriteTile(int tileX, int tileY, int zoom)
        {
            #region Parameters checking

            if (zoom < 0) throw new RasterException(string.Format(Strings.LesserThan, nameof(zoom), 0));
            if (tileX < 0) throw new RasterException(string.Format(Strings.LesserThan, nameof(tileX), 0));
            if (tileY < 0) throw new RasterException(string.Format(Strings.LesserThan, nameof(tileY), 0));

            #endregion

            //Create directories for the tile. The overall structure looks like: outputDirectory/zoom/x/y.png.
            DirectoryInfo tileDirectoryInfo =
                new DirectoryInfo(Path.Combine(OutputDirectoryInfo.FullName, $"{zoom}", $"{tileX}"));
            CheckHelper.CheckDirectory(tileDirectoryInfo);

            //Get the coordinate borders for current tile from tile numbers.
            (double minX, double minY, double maxX, double maxY) =
                Tile.TileTools.TileBounds(tileX, tileY, zoom, TmsCompatible);

            //Get postitions and sizes for current tile.
            (int readPosX, int readPosY, int readXSize, int readYSize, int writePosX, int writePosY, int writeXSize,
             int writeYSize) = GeoQuery(minX, maxY, maxX, minY);

            //Warning: OpenLayers requires replacement of tileY to tileY+1
            FileInfo outputTileFileInfo = new FileInfo(Path.Combine(tileDirectoryInfo.FullName,
                                                                    $"{tileY}{TileExtension}"));

            // Scaling calculations
            double xScale = (double)writeXSize / readXSize;
            double yScale = (double)writeYSize / readYSize;

            // Crop and resize tile
            NetVips.Image tileImage = Resize(Data.Crop(readPosX, readPosY, readXSize, readYSize), xScale, yScale);

            // Add alpha channel if needed
            for (; tileImage.Bands < Constants.Image.Raster.Bands;) tileImage = tileImage.Bandjoin(255);

            // Make transparent image and insert tile
            using NetVips.Image outputImage = NetVips
                                             .Image.Black(Constants.Image.Raster.TileSize,
                                                          Constants.Image.Raster.TileSize).NewFromImage(0, 0, 0, 0)
                                             .Insert(tileImage, writePosX, writePosY);
            outputImage.WriteToFile(outputTileFileInfo.FullName);

            //Dispose the tile.
            tileImage.Dispose();

            //Check if tile was created successfuly.
            CheckHelper.CheckFile(outputTileFileInfo, true);
        }

        /// <summary>
        /// Crops passed zoom to tiles.
        /// </summary>
        /// <param name="zoom">Current zoom to crop.</param>
        /// <param name="tmsCompatible">Are tiles tms compatible?</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        private async ValueTask WriteZoomAsync(int zoom, bool tmsCompatible, int threadsCount)
        {
            #region Parameters checking

            if (zoom < 0) throw new RasterException(string.Format(Strings.LesserThan, nameof(zoom), 0));
            if (threadsCount <= 0)
                throw new RasterException(string.Format(Strings.LesserOrEqual, nameof(threadsCount), 0));

            #endregion

            using SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount);

            List<Task> tasks = new List<Task>();

            //Get min/max tiles numbers
            (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) = GetTileNumbers(zoom, tmsCompatible);

            for (int tileY = tileMinY; tileY <= tileMaxY; tileY++)
            {
                for (int tileX = tileMinX; tileX <= tileMaxX; tileX++)
                {
                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);

                    int x = tileX;
                    int y = tileY;

                    tasks.Add(Task.Run(() =>
                    {
                        try { WriteTile(x, y, zoom); }
                        // ReSharper disable once AccessToDisposedClosure
                        finally { semaphoreSlim.Release(); }
                    }));
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            //Dispose tasks.
            foreach (Task task in tasks) task.Dispose();

            //Alternative way, a bit less effective.
            //ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = threadsCount };

            //for (int tileY = tileMinY; tileY <= tileMaxY; tileY++)
            //{
            //    int y = tileY;
            //    await Task.Run(() => Parallel.For(tileMinX, tileMaxX + 1, parallelOptions,
            //                                      x => WriteTile(x, y, zoom))).ConfigureAwait(false);
            //}
        }

        /// <summary>
        /// Sets properties, needed for cropping current <see cref="Raster"/>.
        /// </summary>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="tmsCompatible">Do you want tms tiles on output?</param>
        /// <param name="tileExtension">Extensions of ready tiles.</param>
        private void SetGenerateTilesProperties(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                bool tmsCompatible, string tileExtension)
        {
            #region Check parameters

            CheckHelper.CheckDirectory(outputDirectoryInfo, true);

            if (maxZ < minZ) throw new RasterException(string.Format(Strings.LesserThan, nameof(maxZ), nameof(minZ)));
            if (minZ < 0) throw new RasterException(string.Format(Strings.LesserThan, nameof(minZ), 0));
            if (maxZ < 0) throw new RasterException(string.Format(Strings.LesserThan, nameof(maxZ), 0));

            #endregion

            (OutputDirectoryInfo, MinZ, MaxZ, TmsCompatible, TileExtension) =
                (outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension);
        }

        #endregion

        #region Public

        /// <inheritdoc />
        public async ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                  bool tmsCompatible,
                                                  string tileExtension,
                                                  IProgress<double> progress,
                                                  int threadsCount)
        {
            //TODO: profile argument (geodetic/mercator)

            #region Parameters checking

            progress ??= new Progress<double>();

            if (threadsCount <= 0)
                throw new RasterException(string.Format(Strings.LesserOrEqual, nameof(threadsCount), 0));

            #endregion

            SetGenerateTilesProperties(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension);

            //Crop tiles for each zoom.
            for (int zoom = MinZ; zoom <= MaxZ; zoom++)
            {
                await WriteZoomAsync(zoom, tmsCompatible, threadsCount).ConfigureAwait(false);

                double percentage = (double)(zoom - MinZ + 1) / (MaxZ - MinZ + 1) * 100.0;
                progress.Report(percentage);
            }
        }

        #endregion

        #endregion

        #endregion

        #region Experimental

        /// <summary>
        /// Gets count of tiles to crop. Needed for progress calculations.
        /// </summary>
        /// <param name="tmsCompatible">Are tiles tims compatible?</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns>Number of tiles to crop.</returns>
        private async ValueTask<int> GetTilesCount(bool tmsCompatible, int threadsCount)
        {
            int tilesCount = 0;

            for (int zoom = MinZ; zoom <= MaxZ; zoom++)
            {
                ParallelOptions parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = threadsCount };
                int currentZoom = zoom;
                await Task.Run(() =>
                {
                    //Get tiles min/max numbers.
                    (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) =
                        GetTileNumbers(currentZoom, tmsCompatible);

                    for (int tileY = tileMinY; tileY <= tileMaxY; tileY++)
                        Parallel.For(tileMinX, tileMaxX + 1,
                                     parallelOptions,
                                     () => 0,
                                     (i, state, subtotal) => ++subtotal,
                                     value => Interlocked.Add(ref tilesCount, value));
                }).ConfigureAwait(false);
            }

            return tilesCount;
        }

        /// <inheritdoc />
        public async ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                  bool tmsCompatible, string tileExtension,
                                                  IProgress<double> progress,
                                                  int threadsCount,
                                                  bool isExperimental)
        {
            if (!isExperimental)
            {
                await GenerateTilesAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension, progress, threadsCount);

                return;
            }

            //TODO: profile argument (geodetic/mercator)

            //TODO: Temprorary for performance testing purpose
            Stopwatch stopwatch = Stopwatch.StartNew();

            #region Parameters checking

            progress ??= new Progress<double>();

            if (threadsCount <= 0)
                throw new RasterException(string.Format(Strings.LesserOrEqual, nameof(threadsCount), 0));

            #endregion

            SetGenerateTilesProperties(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension);

            const bool isPrintEstimatedTime = false;
            //Crop all tiles.
            // ReSharper disable once RedundantArgumentDefaultValue
            await RunTiling(tmsCompatible, progress, threadsCount, isPrintEstimatedTime).ConfigureAwait(false);

            stopwatch.Stop();
            // ReSharper disable once LocalizableElement
            Console.WriteLine($"Elapsed time:{stopwatch.ElapsedMilliseconds}");
        }

        /// <summary>
        /// Crops tiles one by one with more accurant progress report and slightly better performance (WIP).
        /// </summary>
        /// <param name="tmsCompatible">Are tiles tms compatible?</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <param name="isPrintEstimatedTime">Do you want to see estimated time left as progress changes?</param>
        /// <returns></returns>
        private async ValueTask RunTiling(bool tmsCompatible, IProgress<double> progress, int threadsCount,
                                          bool isPrintEstimatedTime = false)
        {
            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = await GetTilesCount(tmsCompatible, threadsCount).ConfigureAwait(false);
            double counter = 0.0;

            if (tilesCount <= 0) return;

            using SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount);
            List<Task> tasks = new List<Task>();

            //For each zoom.
            for (int zoom = MinZ; zoom <= MaxZ; zoom++)
            {
                //Get tiles min/max numbers.
                (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) = GetTileNumbers(zoom, tmsCompatible);

                //For each tile on given zoom calculate positions/sizes and save as file.
                for (int tileY = tileMinY; tileY <= tileMaxY; tileY++)
                {
                    for (int tileX = tileMinX; tileX <= tileMaxX; tileX++)
                    {
                        await semaphoreSlim.WaitAsync().ConfigureAwait(false);

                        int x = tileX;
                        int y = tileY;
                        int currentZoom = zoom;

                        tasks.Add(Task.Run(() =>
                        {
                            try { WriteTile(x, y, currentZoom); }
                            finally
                            {
                                // ReSharper disable once AccessToDisposedClosure
                                semaphoreSlim.Release();

                                //Report progress.
                                counter++;
                                double percentage = counter / tilesCount * 100.0;
                                progress.Report(percentage);

                                //Estimated time left calculation.
                                PrintEstimatedTimeLeft(percentage, stopwatch);
                            }
                        }));
                    }
                }
            }

            //Wait for all tasks for complete.
            await Task.WhenAll(tasks).ConfigureAwait(false);

            //Dispose tasks.
            foreach (Task task in tasks) task.Dispose();

            stopwatch?.Stop();
        }

        /// <summary>
        /// Prints estimated time left.
        /// </summary>
        /// <param name="percentage">Current progress.</param>
        /// <param name="stopwatch">Get elapsed time from this.</param>
        [SuppressMessage("ReSharper", "LocalizableElement")]
        private static void PrintEstimatedTimeLeft(double percentage, Stopwatch stopwatch = null)
        {
            if (stopwatch == null) return;

            double timePassed = stopwatch.ElapsedMilliseconds;
            double estimatedAllTime = 100.0 * timePassed / percentage;
            double estimatedTimeLeft = estimatedAllTime - timePassed;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(estimatedTimeLeft);
            //TODO: localize string
            Console.WriteLine($"Estimated time. Days:{timeSpan.Days}, "
                            + $"Hours:{timeSpan.Hours}, " + $"Minutes:{timeSpan.Minutes}, "
                            + $"Seconds:{timeSpan.Seconds}, " + $"Ms:{timeSpan.Milliseconds}.");
        }

        #endregion
    }
}
