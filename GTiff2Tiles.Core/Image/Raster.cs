#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants.Image;
using GTiff2Tiles.Core.Exceptions.Image;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Localization;
using GTiff2Tiles.Core.Tiles;
using NetVips;

namespace GTiff2Tiles.Core.Image
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
        private NetVips.Image Data { get; }

        #endregion

        #region Public

        /// <inheritdoc />
        public int Width { get; }

        /// <inheritdoc />
        public int Height { get; }

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
                Width = Data.Width;
                Height = Data.Height;
                (MinX, MinY, MaxX, MaxY) = Gdal.Gdal.GetImageBorders(inputFileInfo, Width, Height);
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

        #region Other

        /// <summary>
        /// Gets count of tiles to crop. Needed for progress calculations.
        /// </summary>
        /// <param name="tmsCompatible">Are tiles tims compatible?</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns>Number of tiles to crop.</returns>
        private async ValueTask<int> GetTilesCount(int minZ, int maxZ, bool tmsCompatible)
        {
            int tilesCount = 0;
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                int currentZoom = zoom;
                await Task.Run(() =>
                {
                    //Get tiles min/max numbers.
                    (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) =
                        Tiles.Tile.GetNumbersFromCoords(MinX, MinY, MaxX, MaxY, currentZoom, tmsCompatible);

                    for (int tileY = tileMinY; tileY <= tileMaxY; tileY++)
                        Parallel.For(tileMinX, tileMaxX + 1,
                                     () => 0,
                                     (i, state, subtotal) => ++subtotal,
                                     value => Interlocked.Add(ref tilesCount, value));
                }).ConfigureAwait(false);
            }

            return tilesCount;
        }

        /// <summary>
        /// Prints estimated time left.
        /// </summary>
        /// <param name="percentage">Current progress.</param>
        /// <param name="stopwatch">Get elapsed time from this.</param>
        private static void PrintEstimatedTimeLeft(double percentage, Stopwatch stopwatch = null)
        {
            if (stopwatch == null) return;

            double timePassed = stopwatch.ElapsedMilliseconds;
            double estimatedAllTime = 100.0 * timePassed / percentage;
            double estimatedTimeLeft = estimatedAllTime - timePassed;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(estimatedTimeLeft);
            Console.WriteLine(Strings.EstimatedTime, Environment.NewLine, timeSpan.Days, timeSpan.Hours,
                              timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }

        #endregion

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
            double readPosMinX = Width * (upperLeftX - MinX) / (MaxX - MinX);
            double readPosMinY = Height - Height * (upperLeftY - MinY) / (MaxY - MinY);
            double readPosMaxX = Width * (lowerRightX - MinX) / (MaxX - MinX);
            double readPosMaxY = Height - Height * (lowerRightY - MinY) / (MaxY - MinY);

            //If outside of tiff.
            readPosMinX = readPosMinX < 0.0 ? 0.0 :
                          readPosMinX > Width ? Width : readPosMinX;
            readPosMinY = readPosMinY < 0.0 ? 0.0 :
                          readPosMinY > Height ? Height : readPosMinY;
            readPosMaxX = readPosMaxX < 0.0 ? 0.0 :
                          readPosMaxX > Width ? Width : readPosMaxX;
            readPosMaxY = readPosMaxY < 0.0 ? 0.0 :
                          readPosMaxY > Height ? Height : readPosMaxY;

            //Output tile's borders in pixels.
            double tilePixMinX = readPosMinX.Equals(0.0) ? MinX :
                                 readPosMinX.Equals(Width) ? MaxX : upperLeftX;
            double tilePixMinY = readPosMaxY.Equals(0.0) ? MaxY :
                                 readPosMaxY.Equals(Height) ? MinY : lowerRightY;
            double tilePixMaxX = readPosMaxX.Equals(0.0) ? MinX :
                                 readPosMaxX.Equals(Width) ? MaxX : lowerRightX;
            double tilePixMaxY = readPosMinY.Equals(0.0) ? MaxY :
                                 readPosMinY.Equals(Height) ? MinY : upperLeftY;

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

        private (int readPosX, int readPosY, int readXSize, int readYSize, int writePosX, int writePosY, int writeXSize,
            int writeYSize) GeoQuery(ITile tile) => GeoQuery(tile.MinLongtiude, tile.MinLatitude, tile.MaxLongitude, tile.MaxLatitude);

        private void AddBands(ref NetVips.Image image, int bands = Constants.Image.Raster.Bands)
        {
            for (; image.Bands < bands;) image = image.Bandjoin(255);
        }

        private void AddBands(NetVips.Image[] images, int bands = Constants.Image.Raster.Bands)
        {
            for (int index = 0; index < images.Length; index++) AddBands(ref images[index], bands);
        }

        /// <summary>
        /// Writes one tile of current zoom.
        /// <para/>Crops zoom directly from input image.
        /// </summary>
        /// <param name="outputDirectoryInfo">Directory for ready tiles.</param>
        /// <param name="tileX">Tile x.</param>
        /// <param name="tileY">Tile y.</param>
        /// <param name="zoom">Zoom level.</param>
        private void WriteTile(DirectoryInfo outputDirectoryInfo, int tileX, int tileY, int zoom,
                               bool tmsCompatible, string tileExtension)
        {
            #region Parameters checking

            if (zoom < 0) throw new RasterException(string.Format(CultureInfo.InvariantCulture, Strings.LesserThan, nameof(zoom), 0));
            if (tileX < 0) throw new RasterException(string.Format(CultureInfo.InvariantCulture, Strings.LesserThan, nameof(tileX), 0));
            if (tileY < 0) throw new RasterException(string.Format(CultureInfo.InvariantCulture, Strings.LesserThan, nameof(tileY), 0));

            #endregion

            //Create directories for the tile. The overall structure looks like: outputDirectory/zoom/x/y.png.
            DirectoryInfo tileDirectoryInfo =
                new DirectoryInfo(Path.Combine(outputDirectoryInfo.FullName, $"{zoom}", $"{tileX}"));
            CheckHelper.CheckDirectory(tileDirectoryInfo);

            //Get the coordinate borders for current tile from tile numbers.
            (double minX, double minY, double maxX, double maxY) =
                Tiles.Tile.GetBounds(tileX, tileY, zoom, tmsCompatible);

            //Get postitions and sizes for current tile.
            (int readPosX, int readPosY, int readXSize, int readYSize, int writePosX, int writePosY, int writeXSize,
             int writeYSize) = GeoQuery(minX, maxY, maxX, minY);

            //Warning: OpenLayers requires replacement of tileY to tileY+1
            FileInfo outputTileFileInfo = new FileInfo(Path.Combine(tileDirectoryInfo.FullName,
                                                                    $"{tileY}{tileExtension}"));

            // Scaling calculations
            double xScale = (double)writeXSize / readXSize;
            double yScale = (double)writeYSize / readYSize;

            // Crop and resize tile
            NetVips.Image tmpTileImage = Resize(Data.Crop(readPosX, readPosY, readXSize, readYSize), xScale, yScale);

            // Add alpha channel if needed
            //TODO: add bands param
            //for (; tileImage.Bands < Constants.Image.Raster.Bands;) tileImage = tileImage.Bandjoin(255);
            AddBands(ref tmpTileImage);

            // Make transparent image and insert tile
            using NetVips.Image outputImage = NetVips
                                             .Image.Black(256, 256).NewFromImage(0, 0, 0, 0)
                                             .Insert(tmpTileImage, writePosX, writePosY);
            outputImage.WriteToFile(outputTileFileInfo.FullName);

            //Dispose the tile.
            tmpTileImage.Dispose();

            //Check if tile was created successfuly.
            CheckHelper.CheckFile(outputTileFileInfo, true);
        }

        private NetVips.Image CreateTileImage(ITile tile)
        {
            //Get postitions and sizes for current tile.
            (int readPosX, int readPosY, int readXSize, int readYSize, int writePosX, int writePosY, int writeXSize,
             int writeYSize) = GeoQuery(tile.MinLongtiude, tile.MaxLatitude, tile.MaxLongitude, tile.MinLatitude);

            // Scaling calculations
            double xScale = (double)writeXSize / readXSize;
            double yScale = (double)writeYSize / readYSize;

            // Crop and resize tile
            NetVips.Image tmpTileImage = Resize(Data.Crop(readPosX, readPosY, readXSize, readYSize), xScale, yScale);

            // Add alpha channel if needed
            //TODO: add bands param
            //for (; tileImage.Bands < Constants.Image.Raster.Bands;) tileImage = tileImage.Bandjoin(255);
            AddBands(ref tmpTileImage);

            // Make transparent image and insert tile
            return NetVips.Image.Black(tile.Size, tile.Size).NewFromImage(0, 0, 0, 0)
                          .Insert(tmpTileImage, writePosX, writePosY);
        }

        private void WriteTileToFile(ITile tile)
        {
            //todo file path?
            using var tileImage = CreateTileImage(tile);
            tileImage.WriteToFile(tile.FileInfo.FullName);
        }

        private IEnumerable<byte> WriteTileToEnumerable(ITile tile)
        {
            using var tileImage = CreateTileImage(tile);
            //todo test
            //return tileImage.WriteToBuffer(tile.Extension);
            return tileImage.WriteToMemory();
        }

        #endregion

        #region Public

        /// <inheritdoc />
        public async ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                  bool tmsCompatible = false, string tileExtension = Constants.Extensions.Png,
                                                  IProgress<double> progress = null,
                                                  int threadsCount = 0, bool isPrintEstimatedTime = true)
        {
            //TODO: profile argument (geodetic/mercator)

            #region Parameters checking

            progress ??= new Progress<double>();

            #endregion

            ParallelOptions parallelOptions = new ParallelOptions();
            if (threadsCount <= 0) parallelOptions.MaxDegreeOfParallelism = threadsCount;

            //Crop all tiles.
            Stopwatch stopwatch = isPrintEstimatedTime ? Stopwatch.StartNew() : null;
            int tilesCount = await GetTilesCount(minZ, maxZ, tmsCompatible).ConfigureAwait(false);
            double counter = 0.0;

            if (tilesCount <= 0) return;

            //For each zoom.
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                //Get tiles min/max numbers.
                (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) =
                    Tiles.Tile.GetNumbersFromCoords(MinX, MinY, MaxX, MaxY, zoom, tmsCompatible);

                //For each tile on given zoom calculate positions/sizes and save as file.
                for (int tileY = tileMinY; tileY <= tileMaxY; tileY++)
                {
                    int y = tileY;
                    int z = zoom;

                    void MakeTile(int tileX)
                    {
                        WriteTile(outputDirectoryInfo, tileX, y, z, tmsCompatible,
                                  tileExtension);

                        //Report progress.
                        counter++;
                        double percentage = counter / tilesCount * 100.0;
                        progress.Report(percentage);

                        //Estimated time left calculation.
                        PrintEstimatedTimeLeft(percentage, stopwatch);
                    }

                    await Task.Run(() => Parallel.For(tileMinX, tileMaxX + 1, parallelOptions, MakeTile))
                              .ConfigureAwait(false);
                }
            }
        }

        #endregion

        #endregion
    }
}
