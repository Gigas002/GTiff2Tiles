using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Exceptions.Image;
using NetVips;
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

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new <see cref="Image"/> object.
        /// </summary>
        /// <param name="inputFileInfo">Object of <see cref="FileInfo"/> class, representing input file.</param>
        public Image(FileInfo inputFileInfo)
        {
            //Disable NetVips warnings for tiff.
            Helpers.NetVipsHelper.DisableLog();

            #region Check parameters

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName)) throw new ImageException("Input file's path is empty.");
            if (!inputFileInfo.Exists) throw new ImageException("Input file isn't exist.");

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
                throw new ImageException("Unable to get input image's coordinate borders or  sizes.", exception);
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
            int writeYSize) GeoQuery(double upperLeftX,
                                     double upperLeftY,
                                     double lowerRightX,
                                     double lowerRightY)
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

            return ((int) readPosMinX, (int) readPosMinY,
                    (int) readXSize, (int) readYSize,
                    (int) writePosMinX, (int) writePosMinY,
                    (int) writeXSize, (int) writeYSize);
        }

        /// <summary>
        /// Writes one tile of current zoom.
        /// Crops zoom directly from input image.
        /// </summary>
        /// <param name="zoom">Zoom level.</param>
        /// <param name="tileX">Tile x.</param>
        /// <param name="tileY">Tile y.</param>
        /// <param name="inputImage">Input image.</param>
        private void WriteTile(int zoom, int tileX, int tileY, NetVips.Image inputImage)
        {
            #region Parameters checking

            if (zoom < 0) throw new ImageException("Zoom is lesser than 0.");
            if (tileX < 0) throw new ImageException("Tile X number is lesser than 0.");
            if (tileY < 0) throw new ImageException("Tile Y number is lesser than 0.");

            #endregion

            //Create directories for the tile. The overall structure looks like: outputDirectory/zoom/x/y.png.
            try
            {
                string tileDirectoryPath = Path.Combine(OutputDirectoryInfo.FullName, $"{zoom}", $"{tileX}");
                Directory.CreateDirectory(tileDirectoryPath);
            }
            catch (Exception exception)
            {
                throw new ImageException("Unable to create tile's directory.", exception);
            }

            const bool centreConvention = false;

            //Get the coordinate borders for current tile from tile numbers.
            (double minX, double minY, double maxX, double maxY) = Tile.Tile.TileBounds(tileX, tileY, zoom, false);

            //Get postitions and sizes for current tile.
            (int readPosX, int readPosY, int readXSize, int readYSize, int writePosX, int writePosY,
             int writeXSize, int writeYSize) = GeoQuery(minX, maxY, maxX, minY);

            string outputFilePath = Path.Combine(OutputDirectoryInfo.FullName, $"{zoom}",
                                                 $"{tileX}",
                                                 $"{tileY}{Enums.Extensions.Png}");

            try
            {
                Directory.CreateDirectory(Path.Combine(OutputDirectoryInfo.FullName, $"{zoom}", $"{tileX}"));
            }
            catch (Exception exception)
            {
                throw new ImageException("Inable to create directory for current tile.", exception);
            }

            // Scaling calculations
            double xFactor = (double) readXSize / writeXSize;
            double yFactor = (double) readYSize / writeYSize;

            // Calculate integral box shrink
            // We will get the best quality (but be the slowest) if we let reduce
            // do all the work. Leave it the final 200 - 300% to do as a compromise
            // for efficiency.
            int xShrink = Math.Max(1, (int) Math.Floor(1.0 / (xFactor * 2.0)));
            int yShrink = Math.Max(1, (int) Math.Floor(1.0 / (yFactor * 2.0)));

            // Calculate residual float affine transformation
            double xResidual = xShrink / xFactor;
            double yResidual = yShrink / yFactor;

            //Try open input image and crop tile
            NetVips.Image tileImage;
            try
            {
                tileImage = inputImage.Crop(readPosX, readPosY, readXSize, readYSize);
            }
            catch (Exception exception)
            {
                throw new ImageException("Unable to create current tile.", exception);
            }

            // Fast, integral box-shrink
            if (yShrink > 1)
            {
                tileImage = tileImage.Shrinkv(yShrink);

                //Recalculate residual float.
                yResidual = (double) writeYSize / tileImage.Height;
            }
            if (xShrink > 1)
            {
                tileImage = tileImage.Shrinkh(xShrink);

                //Recalculate residual float.
                xResidual = (double) writeXSize / tileImage.Width;
            }

            //TODO: Fix bug with too small images to shrink.
            // Perform kernel-based reduction
            if (yResidual < 1.0)
                tileImage = tileImage.Reducev(1.0 / yResidual, NetVips.Enums.Kernel.Lanczos3, centreConvention);
            if (xResidual < 1.0)
                tileImage = tileImage.Reduceh(1.0 / xResidual, NetVips.Enums.Kernel.Lanczos3, centreConvention);

            //Perform enlargement
            if (yResidual > 1.0 || xResidual > 1.0)
            {
                // Input displacement. For centre sampling, shift by 0.5 down and right.
                const double id = 0.0; //centreConvention ? 0.5 : 0.0;

                // Floating point affine transformation
                using (Interpolate interpolate = Interpolate.NewFromName(Enums.Image.Interpolations.Bicubic))
                {
                    if (yResidual > 1.0 && xResidual > 1.0)
                        tileImage = tileImage.Affine(new[] {xResidual, 0.0, 0.0, yResidual}, interpolate,
                                           idx: id, idy: id,
                                           extend: NetVips.Enums.Extend.Copy);
                    else if (yResidual > 1.0)
                        tileImage = tileImage.Affine(new[] {1.0, 0.0, 0.0, yResidual}, interpolate, idx: id,
                                           idy: id,
                                           extend: NetVips.Enums.Extend.Copy);
                    else if (xResidual > 1.0)
                        tileImage = tileImage.Affine(new[] {xResidual, 0.0, 0.0, 1.0}, interpolate, idx: id,
                                           idy: id,
                                           extend: NetVips.Enums.Extend.Copy);
                }
            }

            // Add alpha channel if needed
            for (; tileImage.Bands < Enums.Image.Image.Bands;)
                tileImage = tileImage.Bandjoin(255);

            // Make a transparent image
            NetVips.Image outputImage;
            try
            {
                outputImage = NetVips.Image.Black(Enums.Image.Image.TileSize, Enums.Image.Image.TileSize)
                                     .NewFromImage(0, 0, 0, 0);
                // Insert tile into output image
                outputImage = outputImage.Insert(tileImage, writePosX, writePosY);
                outputImage.Pngsave(outputFilePath);
            }
            catch (Exception exception)
            {
                throw new ImageException("Unable to write tile.", exception);
            }

            if (!File.Exists(outputFilePath)) throw new ImageException($"Method {nameof(WriteTile)} was unable to create tile. Path: {outputFilePath}.");

            outputImage.Dispose();
            tileImage.Dispose();
        }

        /// <summary>
        /// Writes new tile by joining 4 lower ones.
        /// </summary>
        /// <param name="zoom">Current zoom level.</param>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        private void WriteTile(int zoom, int tileX, int tileY)
        {
            #region Parameters checking

            if (zoom < 0) throw new ImageException("Zoom is lesser than 0.");
            if (tileX < 0) throw new ImageException("Tile X number is lesser than 0.");
            if (tileY < 0) throw new ImageException("Tile Y number is lesser than 0.");

            #endregion

            //Create directories for the tile. The overall structure looks like: outputDirectory/zoom/x/y.png.
            string tileDirectoryPath = Path.Combine(OutputDirectoryInfo.FullName, $"{zoom}", $"{tileX}");
            try
            {
                Directory.CreateDirectory(tileDirectoryPath);
            }
            catch (Exception exception)
            {
                throw new ImageException($"Unable to create tile's directory here: {tileDirectoryPath}.",
                                    exception);
            }

            //Calculate upper tiles's positions.
            int upperTileX1 = tileX * 2;
            int upperTileY1 = tileY * 2;
            int upperTileX2 = upperTileX1 + 1;
            int upperTileY2 = upperTileY1;
            int upperTileX3 = upperTileX1;
            int upperTileY3 = upperTileY1 + 1;
            int upperTileX4 = upperTileX1 + 1;
            int upperTileY4 = upperTileY1 + 1;

            bool tilesExists = false;

            const int upperTileSize = Enums.Image.Image.TileSize / 2;

            #region Create 4 inner tiles

            NetVips.Image upperTileImage1;
            string tile1Path = Path.Combine(OutputDirectoryInfo.FullName, $"{zoom + 1}", $"{upperTileX1}",
                                            $"{upperTileY1}{Enums.Extensions.Png}");

            try
            {
                if (File.Exists(tile1Path))
                {
                    upperTileImage1 = NetVips.Image.Pngload(tile1Path);
                    upperTileImage1 = upperTileImage1.ThumbnailImage(upperTileSize, upperTileSize);
                    tilesExists = true;
                }
                else
                    upperTileImage1 = NetVips.Image.Black(upperTileSize, upperTileSize);
            }
            catch (Exception exception)
            {
                throw new
                    Exception($"Unable to create tile1. {nameof(tileX)}:{tileX}, {nameof(tileY)}:{tileY}.",
                              exception);
            }

            NetVips.Image upperTileImage2;
            string tile2Path = Path.Combine(OutputDirectoryInfo.FullName, $"{zoom + 1}", $"{upperTileX2}",
                                            $"{upperTileY2}{Enums.Extensions.Png}");

            try
            {
                if (File.Exists(tile2Path))
                {
                    upperTileImage2 = NetVips.Image.Pngload(tile2Path);
                    upperTileImage2 = upperTileImage2.ThumbnailImage(upperTileSize, upperTileSize);
                    tilesExists = true;
                }
                else
                    upperTileImage2 = NetVips.Image.Black(upperTileSize, upperTileSize);
            }
            catch (Exception exception)
            {
                throw new
                    Exception($"Unable to create tile2. {nameof(tileX)}:{tileX}, {nameof(tileY)}:{tileY}.",
                              exception);
            }

            NetVips.Image upperTileImage3;
            string tile3Path = Path.Combine(OutputDirectoryInfo.FullName, $"{zoom + 1}", $"{upperTileX3}",
                                            $"{upperTileY3}{Enums.Extensions.Png}");

            try
            {
                if (File.Exists(tile3Path))
                {
                    upperTileImage3 = NetVips.Image.Pngload(tile3Path);
                    upperTileImage3 = upperTileImage3.ThumbnailImage(upperTileSize, upperTileSize);
                    tilesExists = true;
                }
                else
                    upperTileImage3 = NetVips.Image.Black(upperTileSize, upperTileSize);
            }
            catch (Exception exception)
            {
                throw new
                    Exception($"Unable to create tile3. {nameof(tileX)}:{tileX}, {nameof(tileY)}:{tileY}.",
                              exception);
            }

            NetVips.Image upperTileImage4;
            string tile4Path = Path.Combine(OutputDirectoryInfo.FullName, $"{zoom + 1}", $"{upperTileX4}",
                                            $"{upperTileY4}{Enums.Extensions.Png}");

            try
            {
                if (File.Exists(tile4Path))
                {
                    upperTileImage4 = NetVips.Image.Pngload(tile4Path);
                    upperTileImage4 = upperTileImage4.ThumbnailImage(upperTileSize, upperTileSize);
                    tilesExists = true;
                }
                else
                    upperTileImage4 = NetVips.Image.Black(upperTileSize, upperTileSize);
            }
            catch (Exception exception)
            {
                throw new
                    Exception($"Unable to create tile4. {nameof(tileX)}:{tileX}, {nameof(tileY)}:{tileY}.",
                              exception);
            }

            #endregion

            //We shouldn't create tiles, if they're not exist.
            if (!tilesExists) return;

            NetVips.Image[] images = {upperTileImage3, upperTileImage4, upperTileImage1, upperTileImage2};

            //Check and write bands if needed.
            for (int i = 0; i < images.Length; i++)
            {
                int bands = images[i].Bands;
                switch (bands)
                {
                    case Enums.Image.Image.Bands:
                        continue;
                    case 1:
                    {
                        for (int j = bands; j < Enums.Image.Image.Bands; j++)
                        {
                            try
                            {
                                images[i] = images[i].Bandjoin(0);
                            }
                            catch (Exception exception)
                            {
                                throw new
                                    Exception($"Unable to join band {i}. {nameof(tileX)}:{tileX}, {nameof(tileY)}:{tileY}.",
                                              exception);
                            }
                        }
                        break;
                    }
                    default:
                    {
                        for (int j = bands; j < Enums.Image.Image.Bands; j++)
                        {
                            try
                            {
                                images[i] = images[i].Bandjoin(255);
                            }
                            catch (Exception exception)
                            {
                                throw new
                                    Exception($"Unable to join band {i}. {nameof(tileX)}:{tileX}, {nameof(tileY)}:{tileY}.",
                                              exception);
                            }
                        }
                        break;
                    }
                }
            }

            //Join 4 tiles.
            string outputTilePath = Path.Combine(tileDirectoryPath, $"{tileY}{Enums.Extensions.Png}");
            try
            {
                using (NetVips.Image resultImage = NetVips.Image.Arrayjoin(images, 2))
                {
                    resultImage.Pngsave(outputTilePath);
                }
            }
            catch (Exception exception)
            {
                throw new
                    Exception($"Unable to join tiles. {nameof(tileX)}:{tileX}, {nameof(tileY)}:{tileY}.",
                              exception);
            }

            if (!File.Exists(outputTilePath)) throw new ImageException($"Method {nameof(WriteTile)} was unable to create tile. Path: {outputTilePath}.");

            //Dispose images.
            upperTileImage1.Dispose();
            upperTileImage2.Dispose();
            upperTileImage3.Dispose();
            upperTileImage4.Dispose();
            foreach (NetVips.Image image in images)
                image.Dispose();
        }

        /// <summary>
        /// Crops passed zoom to tiles.
        /// </summary>
        /// <param name="zoom">Current zoom to crop.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        private async ValueTask WriteZoom(int zoom, int threadsCount)
        {
            #region Parameters checking

            if (zoom < 0) throw new ImageException("Zoom is lesser than 0.");
            if (threadsCount <= 0) throw new ImageException("Threads couns is ledder or equal 0.");

            #endregion

            //Try to open input image.
            NetVips.Image inputImage;
            try
            {
                inputImage = NetVips.Image.Tiffload(InputFileInfo.FullName, access: NetVips.Enums.Access.Random);
            }
            catch (Exception exception)
            {
                throw new ImageException("NetVips is unable to open input image.", exception);
            }

            using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount))
            {
                List<Task> tasks = new List<Task>();

                //For each tile on given zoom calculate positions/sizes and save as file.
                for (int tileY = TilesMinMax[zoom][1]; tileY <= TilesMinMax[zoom][3]; tileY++)
                {
                    for (int tileX = TilesMinMax[zoom][0]; tileX <= TilesMinMax[zoom][2]; tileX++)
                    {
                        await semaphoreSlim.WaitAsync();

                        int x = tileX;
                        int y = tileY;

                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                WriteTile(zoom, x, y, inputImage);
                            }
                            finally
                            {
                                semaphoreSlim.Release();
                            }
                        }));
                    }
                }

                await Task.WhenAll(tasks);

                //Dispose tasks.
                foreach (Task task in tasks) task.Dispose();
            }

            inputImage.Dispose();
        }

        /// <summary>
        /// Make upper tiles from the lowest zoom.
        /// </summary>
        /// <param name="zoom">Zoom, for which we're cropping tiles atm.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        private async ValueTask MakeUpperTiles(int zoom, int threadsCount)
        {
            #region Parameters checking

            if (zoom < 0) throw new ImageException("Zoom is lesser than 0.");
            if (threadsCount <= 0) throw new ImageException("Threads couns is ledder or equal 0.");

            #endregion

            using (SemaphoreSlim semaphoreSlim = new SemaphoreSlim(threadsCount))
            {
                List<Task> tasks = new List<Task>();

                //For each tile on current zoom.
                for (int tileY = TilesMinMax[zoom][1]; tileY <= TilesMinMax[zoom][3]; tileY++)
                {
                    for (int tileX = TilesMinMax[zoom][0]; tileX <= TilesMinMax[zoom][2]; tileX++)
                    {
                        await semaphoreSlim.WaitAsync();

                        int x = tileX;
                        int y = tileY;

                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                WriteTile(zoom, x, y);
                            }
                            finally
                            {
                                semaphoreSlim.Release();
                            }
                        }));
                    }
                }

                await Task.WhenAll(tasks);

                //Dispose tasks.
                foreach (Task task in tasks) task.Dispose();
            }
        }

        /// <summary>
        /// Sets properties, needed for cropping current <see cref="Image"/>.
        /// </summary>
        /// <param name="outputDirectoryInfo">Object of <see cref="DirectoryInfo"/> class, representing output directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        private void SetCropProperties(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ)
        {
            #region Check parameters

            if (string.IsNullOrWhiteSpace(outputDirectoryInfo.FullName))
                throw new ImageException("Output directory path is empty.");

            Helpers.CheckHelper.CheckOutputDirectory(outputDirectoryInfo);

            if (maxZ < minZ) throw new ImageException("Maximum zoom is lesser than minimum zoom.");
            if (minZ < 0) throw new ImageException("Minimum zoom is lesser than 0.");
            if (maxZ < 0) throw new ImageException("Maximum zoom is lesser, than 0.");

            #endregion

            (OutputDirectoryInfo, MinZ, MaxZ) = (outputDirectoryInfo, minZ, maxZ);

            //Create dictionary with tiles for each cropped zoom.
            for (int zoom = MinZ; zoom <= MaxZ; zoom++)
            {
                //Convert coordinates to tile numbers.
                (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) =
                    Tile.Tile.GetTileNumbersFromCoords(MinX, MinY, MaxX, MaxY, zoom);

                //Crop tiles extending world limits (+-180,+-90).
                tileMinX = Math.Max(0, tileMinX);
                tileMinY = Math.Max(0, tileMinY);
                tileMaxX = Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom + 1)) - 1, tileMaxX);
                tileMaxY = Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom)) - 1, tileMaxY);

                try
                {
                    TilesMinMax.Add(zoom, new[] { tileMinX, tileMinY, tileMaxX, tileMaxY });
                }
                catch (Exception exception)
                {
                    throw new ImageException($"Unable to add value to {nameof(TilesMinMax)} dictionary.", exception);
                }
            }
        }

        #endregion

        #region Public

        /// <summary>
        /// Create tiles. Crops input tiff only for lowest zoom and then join the higher ones from it.
        /// </summary>
        /// <param name="outputDirectoryInfo">Object of <see cref="DirectoryInfo"/> class, representing output directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        public async ValueTask GenerateTilesByJoining(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, IProgress<double> progress, int threadsCount)
        {
            #region Parameters checking

            if (progress == null) throw new ImageException("Progress == null.");
            if (threadsCount <= 0) throw new ImageException("Threads count is lesser or equal 0.");

            #endregion

            SetCropProperties(outputDirectoryInfo, minZ, maxZ);

            //Crop lowest zoom level.
            await WriteZoom(MaxZ, threadsCount);
            double percentage = 1.0 / (MaxZ - MinZ + 1) * 100.0;
            progress.Report(percentage);

            //Crop upper tiles.
            for (int zoom = MaxZ - 1; zoom >= MinZ; zoom--)
            {
                await MakeUpperTiles(zoom, threadsCount);

                percentage = (double) (MaxZ - zoom + 1) / (MaxZ - MinZ + 1) * 100.0;
                progress.Report(percentage);
            }
        }

        /// <summary>
        /// Crops input tiff for each zoom.
        /// </summary>
        /// <param name="outputDirectoryInfo">Object of <see cref="DirectoryInfo"/> class, representing output directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        public async ValueTask GenerateTilesByCropping(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, IProgress<double> progress, int threadsCount)
        {
            #region Parameters checking

            if (progress == null) throw new ImageException("Progress == null.");
            if (threadsCount <= 0) throw new ImageException("Threads count is lesser or equal 0.");

            #endregion

            SetCropProperties(outputDirectoryInfo, minZ, maxZ);

            //Crop tiles for each zoom.
            for (int zoom = MinZ; zoom <= MaxZ; zoom++)
            {
                await WriteZoom(zoom, threadsCount);

                double percentage = (double) (zoom - MinZ + 1) / (MaxZ - MinZ + 1) * 100.0;
                progress.Report(percentage);
            }
        }

        #endregion

        #endregion
    }
}
