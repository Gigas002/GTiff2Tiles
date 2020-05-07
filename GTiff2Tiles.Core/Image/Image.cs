using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Exceptions.Image;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Localization;
using NetVips;

// ReSharper disable ClassCanBeSealed.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AccessToDisposedClosure

namespace GTiff2Tiles.Core.Image
{
    /// <summary>
    /// Class for creating raster tiles.
    /// </summary>
    public class Image
    {
        #region Properties

        #region Private

        /// <summary>
        /// Dictionary with min/max tile numbers for each zoom level.
        /// </summary>
        private Dictionary<int, int[]> TilesMinMax { get; } = new Dictionary<int, int[]>();

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

        #endregion

        #region Public

        /// <summary>
        /// Image's width.
        /// </summary>
        public int RasterXSize { get; }

        /// <summary>
        /// Image's height.
        /// </summary>
        public int RasterYSize { get; }

        /// <summary>
        /// Input GeoTiff.
        /// </summary>
        public FileInfo InputFileInfo { get; }

        /// <summary>
        /// Upper left X coordinate.
        /// </summary>
        public double MinX { get; }

        /// <summary>
        /// Lower right Y coordinate.
        /// </summary>
        public double MinY { get; }

        /// <summary>
        /// Lower right X coordinate.
        /// </summary>
        public double MaxX { get; }

        /// <summary>
        /// Upper left Y coordinate.
        /// </summary>
        public double MaxY { get; }

        /// <summary>
        /// Extension of ready tiles.
        /// </summary>
        public string TileExtension { get; private set; }

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new <see cref="Image"/> object.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff image.</param>
        public Image(FileInfo inputFileInfo)
        {
            //Disable NetVips warnings for tiff.
            NetVipsHelper.DisableLog();

            #region Check parameters

            CheckHelper.CheckFile(inputFileInfo, true);

            #endregion

            InputFileInfo = inputFileInfo;

            //Get border coordinates и raster sizes.
            try
            {
                (RasterXSize, RasterYSize) = Gdal.GetImageSizes(InputFileInfo);
                (MinX, MinY, MaxX, MaxY) = Gdal.GetImageBorders(InputFileInfo, RasterXSize, RasterYSize);
            }
            catch (Exception exception)
            {
                throw new ImageException(string.Format(Strings.UnableToGetCoordinates, nameof(inputFileInfo),
                                                       exception));
            }
        }

        #endregion

        #region Methods

        #region Private

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
            readPosMinX = readPosMinX < 0.0 ? 0.0 : readPosMinX > RasterXSize ? RasterXSize : readPosMinX;
            readPosMinY = readPosMinY < 0.0 ? 0.0 : readPosMinY > RasterYSize ? RasterYSize : readPosMinY;
            readPosMaxX = readPosMaxX < 0.0 ? 0.0 : readPosMaxX > RasterXSize ? RasterXSize : readPosMaxX;
            readPosMaxY = readPosMaxY < 0.0 ? 0.0 : readPosMaxY > RasterYSize ? RasterYSize : readPosMaxY;

            //Output tile's borders in pixels.
            double tilePixMinX = readPosMinX.Equals(0.0) ? MinX : readPosMinX.Equals(RasterXSize) ? MaxX : upperLeftX;
            double tilePixMinY = readPosMaxY.Equals(0.0) ? MaxY : readPosMaxY.Equals(RasterYSize) ? MinY : lowerRightY;
            double tilePixMaxX = readPosMaxX.Equals(0.0) ? MinX : readPosMaxX.Equals(RasterXSize) ? MaxX : lowerRightX;
            double tilePixMaxY = readPosMinY.Equals(0.0) ? MaxY : readPosMinY.Equals(RasterYSize) ? MinY : upperLeftY;

            //Positions of dataset to write in tile.
            double writePosMinX = Enums.Image.Image.TileSize -
                                  Enums.Image.Image.TileSize * (lowerRightX - tilePixMinX) / (lowerRightX - upperLeftX);
            double writePosMinY = Enums.Image.Image.TileSize * (upperLeftY - tilePixMaxY) / (upperLeftY - lowerRightY);
            double writePosMaxX = Enums.Image.Image.TileSize -
                                  Enums.Image.Image.TileSize * (lowerRightX - tilePixMaxX) / (lowerRightX - upperLeftX);
            double writePosMaxY = Enums.Image.Image.TileSize * (upperLeftY - tilePixMinY) / (upperLeftY - lowerRightY);

            //Sizes to read and write.
            double readXSize = readPosMaxX - readPosMinX;
            double readYSize = readPosMaxY - readPosMinY;
            double writeXSize = writePosMaxX - writePosMinX;
            double writeYSize = writePosMaxY - writePosMinY;

            //Shifts.
            double readXShift = readPosMinX - (int) readPosMinX;
            readXSize += readXShift;
            double readYShift = readPosMinY - (int) readPosMinY;
            readYSize += readYShift;
            double writeXShift = writePosMinX - (int) writePosMinX;
            writeXSize += writeXShift;
            double writeYShift = writePosMinY - (int) writePosMinY;
            writeYSize += writeYShift;

            //If output image sides are lesser then 1 - make image 1x1 pixels to prevent division by 0.
            writeXSize = writeXSize > 1.0 ? writeXSize : 1.0;
            writeYSize = writeYSize > 1.0 ? writeYSize : 1.0;

            return ((int) readPosMinX, (int) readPosMinY, (int) readXSize, (int) readYSize, (int) writePosMinX,
                    (int) writePosMinY, (int) writeXSize, (int) writeYSize);
        }

        /// <summary>
        /// Writes one tile of current zoom.
        /// <para/>Crops zoom directly from input image.
        /// </summary>
        /// <param name="inputImage">Input image.</param>
        /// <param name="tileX">Tile x.</param>
        /// <param name="tileY">Tile y.</param>
        /// <param name="zoom">Zoom level.</param>
        private void WriteTile(NetVips.Image inputImage, int tileX, int tileY, int zoom)
        {
            #region Parameters checking

            if (zoom < 0) throw new ImageException(string.Format(Strings.LesserThan, nameof(zoom), 0));
            if (tileX < 0) throw new ImageException(string.Format(Strings.LesserThan, nameof(tileX), 0));
            if (tileY < 0) throw new ImageException(string.Format(Strings.LesserThan, nameof(tileY), 0));

            #endregion

            //Create directories for the tile. The overall structure looks like: outputDirectory/zoom/x/y.png.
            DirectoryInfo tileDirectoryInfo =
                new DirectoryInfo(Path.Combine(OutputDirectoryInfo.FullName, $"{zoom}", $"{tileX}"));
            CheckHelper.CheckDirectory(tileDirectoryInfo);

            const bool centreConvention = false;

            //Get the coordinate borders for current tile from tile numbers.
            (double minX, double minY, double maxX, double maxY) =
                Tile.Tile.TileBounds(tileX, tileY, zoom, TmsCompatible);

            //Get postitions and sizes for current tile.
            (int readPosX, int readPosY, int readXSize, int readYSize, int writePosX, int writePosY, int writeXSize,
             int writeYSize) = GeoQuery(minX, maxY, maxX, minY);

            //Warning: OpenLayers requires replacement of tileY to tileY+1
            FileInfo outputTileFileInfo = new FileInfo(Path.Combine(tileDirectoryInfo.FullName,
                                                                    $"{tileY}{TileExtension}"));


            //Try open input image and crop tile
            NetVips.Image tileImage;

            try { tileImage = inputImage.Crop(readPosX, readPosY, readXSize, readYSize); }
            catch (Exception exception)
            {
                throw new ImageException(string.Format(Strings.UnableToCreateTile, tileX, tileY), exception);
            }

            // Scaling calculations
            double xScale = 1.0 / ((double) tileImage.Width / writeXSize);
            double yScale = 1.0 / ((double) tileImage.Height / writeYSize);

            // Calculate integral box shrink
            // We will get the best quality (but be the slowest) if we let reduce
            // do all the work. Leave it the final 200 - 300% to do as a compromise
            // for efficiency.
            int xShrink = Math.Max(1, (int) Math.Floor(1.0 / (xScale * 2.0)));
            int yShrink = Math.Max(1, (int) Math.Floor(1.0 / (yScale * 2.0)));

            // Fast, integral box-shrink
            if (yShrink > 1)
            {
                tileImage = tileImage.Shrinkv(yShrink);
                yScale *= yShrink;
            }

            if (xShrink > 1)
            {
                tileImage = tileImage.Shrinkh(xShrink);
                xScale *= xShrink;
            }

            // Any residual downsizing
            if (yScale < 1.0)
                tileImage = tileImage.Reducev(1.0 / yScale, NetVips.Enums.Kernel.Lanczos3, centreConvention);
            if (xScale < 1.0)
                tileImage = tileImage.Reduceh(1.0 / xScale, NetVips.Enums.Kernel.Lanczos3, centreConvention);

            // Any upsizing
            if (xScale > 1.0 || yScale > 1.0)
            {
                // Input displacement. For centre sampling, shift by 0.5 down and right.
                //double id = centreConvention ? 0.5 : 0.0;
                const double id = 0.0;

                // Floating point affine transformation
                using Interpolate interpolate = Interpolate.NewFromName(Enums.Image.Interpolations.Bicubic);
                if (xScale > 1.0 && yScale > 1.0)
                    tileImage = tileImage.Affine(new[] { xScale, 0.0, 0.0, yScale }, interpolate, idx: id, idy: id,
                                                 extend: NetVips.Enums.Extend.Copy);
                else if (xScale > 1.0)
                    tileImage = tileImage.Affine(new[] { xScale, 0.0, 0.0, 1.0 }, interpolate, idx: id, idy: id,
                                                 extend: NetVips.Enums.Extend.Copy);
                else
                    tileImage = tileImage.Affine(new[] { 1.0, 0.0, 0.0, yScale }, interpolate, idx: id, idy: id,
                                                 extend: NetVips.Enums.Extend.Copy);
            }

            // Add alpha channel if needed
            for (; tileImage.Bands < Enums.Image.Image.Bands;) tileImage = tileImage.Bandjoin(255);

            // Make a transparent image
            NetVips.Image outputImage;

            try
            {
                outputImage = NetVips.Image.Black(Enums.Image.Image.TileSize, Enums.Image.Image.TileSize)
                                     .NewFromImage(0, 0, 0, 0);

                // Insert tile into output image
                outputImage = outputImage.Insert(tileImage, writePosX, writePosY);
                outputImage.WriteToFile(outputTileFileInfo.FullName);
            }
            catch (Exception exception)
            {
                throw new ImageException(string.Format(Strings.UnableToCreateTile, tileX, tileY), exception);
            }

            //Check if tile was created successfuly.
            CheckHelper.CheckFile(outputTileFileInfo, true);

            tileImage.Dispose();
            outputImage.Dispose();
        }

        /// <summary>
        /// Crops passed zoom to tiles.
        /// </summary>
        /// <param name="zoom">Current zoom to crop.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        private async ValueTask WriteZoomAsync(int zoom, int threadsCount)
        {
            #region Parameters checking

            if (zoom < 0) throw new ImageException(string.Format(Strings.LesserThan, nameof(zoom), 0));
            if (threadsCount <= 0)
                throw new ImageException(string.Format(Strings.LesserOrEqual, nameof(threadsCount), 0));

            #endregion

            //Try to open input image.
            NetVips.Image inputImage;

            try { inputImage = NetVips.Image.Tiffload(InputFileInfo.FullName, access: NetVips.Enums.Access.Random); }
            catch (Exception exception)
            {
                throw new
                    ImageException(string.Format(Strings.UnableToOpen, nameof(inputImage), InputFileInfo.FullName),
                                   exception);
            }

            using SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount);

            List<Task> tasks = new List<Task>();

            //For each tile on given zoom calculate positions/sizes and save as file.
            for (int tileY = TilesMinMax[zoom][1]; tileY <= TilesMinMax[zoom][3]; tileY++)
            {
                for (int tileX = TilesMinMax[zoom][0]; tileX <= TilesMinMax[zoom][2]; tileX++)
                {
                    await semaphoreSlim.WaitAsync().ConfigureAwait(false);

                    int x = tileX;
                    int y = tileY;

                    tasks.Add(Task.Run(() =>
                    {
                        try { WriteTile(inputImage, x, y, zoom); }
                        finally { semaphoreSlim.Release(); }
                    }));
                }
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);

            //Dispose tasks.
            foreach (Task task in tasks) task.Dispose();

            inputImage.Dispose();
        }

        /// <summary>
        /// Sets properties, needed for cropping current <see cref="Image"/>.
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

            if (maxZ < minZ) throw new ImageException(string.Format(Strings.LesserThan, nameof(maxZ), nameof(minZ)));
            if (minZ < 0) throw new ImageException(string.Format(Strings.LesserThan, nameof(minZ), 0));
            if (maxZ < 0) throw new ImageException(string.Format(Strings.LesserThan, nameof(maxZ), 0));

            #endregion

            (OutputDirectoryInfo, MinZ, MaxZ) = (outputDirectoryInfo, minZ, maxZ);
            TmsCompatible = tmsCompatible;
            TileExtension = tileExtension;

            //Create dictionary with tiles for each cropped zoom.
            for (int zoom = MinZ; zoom <= MaxZ; zoom++)
            {
                //Convert coordinates to tile numbers.
                (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) =
                    Tile.Tile.GetTileNumbersFromCoords(MinX, MinY, MaxX, MaxY, zoom, tmsCompatible);

                //Crop tiles extending world limits (+-180,+-90).
                tileMinX = Math.Max(0, tileMinX);
                tileMinY = Math.Max(0, tileMinY);
                tileMaxX = Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom + 1)) - 1, tileMaxX);
                tileMaxY = Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom)) - 1, tileMaxY);

                try { TilesMinMax.Add(zoom, new[] { tileMinX, tileMinY, tileMaxX, tileMaxY }); }
                catch (Exception exception)
                {
                    throw new ImageException(string.Format(Strings.UnableToAddToCollection, nameof(TilesMinMax)),
                                             exception);
                }
            }
        }

        #endregion

        #region Public

        /// <summary>
        /// Crops input tiff for each zoom.
        /// </summary>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="tmsCompatible">Do you want to create tms-compatible tiles? <see langword="true"/> by default.</param>
        /// <param name="tileExtension">Extension of ready tiles. ".png" by default.</param>
        /// <param name="progress">Progress. <see langword="null"/> by default.</param>
        /// <param name="threadsCount">Threads count. 5 by default.</param>
        /// <returns></returns>
        public async ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                            bool tmsCompatible = true,
                                                            string tileExtension = Enums.Extensions.Png,
                                                            IProgress<double> progress = null,
                                                            int threadsCount = 5)
        {
            //TODO: profile argument (geodetic/mercator)

            #region Parameters checking

            if (progress == null) throw new ImageException(string.Format(Strings.IsNull, nameof(progress)));
            if (threadsCount <= 0)
                throw new ImageException(string.Format(Strings.LesserOrEqual, nameof(threadsCount), 0));

            #endregion

            SetGenerateTilesProperties(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension);

            //Crop tiles for each zoom.
            for (int zoom = MinZ; zoom <= MaxZ; zoom++)
            {
                await WriteZoomAsync(zoom, threadsCount).ConfigureAwait(false);

                double percentage = (double)(zoom - MinZ + 1) / (MaxZ - MinZ + 1) * 100.0;
                progress.Report(percentage);
            }
        }

        #endregion

        #endregion
    }
}
