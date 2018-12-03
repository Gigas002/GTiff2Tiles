using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using OSGeo.GDAL;
// ReSharper disable InvertIf

//todo build overview from lowest zoom, multithreading, progress-reporting, exception handling.

namespace Gdal2Tiles
{
    /// <summary>
    /// Alternative to <see cref="Gdal2Tiles"/> class. Uses c# methods to crop tile.
    /// </summary>
    public static class GTiff2Tiles
    {
        #region Properties

        /// <summary>
        /// Image's x size.
        /// </summary>
        private static int RasterXSize { get; set; }

        /// <summary>
        /// Image's y size.
        /// </summary>
        private static int RasterYSize { get; set; }

        /// <summary>
        /// Contains coordinates and pixel resolution of input image.
        /// </summary>
        private static double[] GeoTransform { get; set; }

        /// <summary>
        /// File extension of output tiles.
        /// </summary>
        private static string TileExtension { get; } = "png";

        /// <summary>
        /// Tile's size.
        /// </summary>
        private static int TileSize { get; } = 256;

        /// <summary>
        /// Dictionary with min and max tiles numbers for each zoom.
        /// </summary>
        private static Dictionary<int, int[]> MinMax { get; } = new Dictionary<int, int[]>();

        /// <summary>
        /// FullName of input file.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private static string InputFile { get; set; }

        /// <summary>
        /// FullName of output directory.
        /// </summary>
        private static string OutputDirectory { get; set; }

        /// <summary>
        /// Input geotiff in memory.
        /// </summary>
        private static Image Image { get; set; }

        /// <summary>
        /// Input dataset. Needed only to get coordinates and raster sizes. Opened and disposed in <see cref="Initialize"/>.
        /// </summary>
        private static Dataset InputDataset { get; set; }

        /// <summary>
        /// Contains some useful metadata of input GeoTiff.
        /// </summary>
        private static List<Dictionary<string, int>> Metadata { get; } = new List<Dictionary<string, int>>();

        /// <summary>
        /// Minimum zoom.
        /// </summary>
        private static int MinZ { get; set; }

        /// <summary>
        /// Maximum zoom.
        /// </summary>
        private static int MaxZ { get; set; }

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Initialize needed properties.
        /// </summary>
        /// <param name="inputFile">Fullname of input GeoTiff.</param>
        /// <param name="outputDirectory">Fullname of output directory, in which tiles will be cropped.</param>
        /// <param name="minZ">Minimum zoom.</param>
        /// <param name="maxZ">Maxmimum zoom.</param>
        private static void Initialize(string inputFile, string outputDirectory, int minZ, int maxZ)
        {
            InputFile = inputFile;
            OutputDirectory = outputDirectory;
            MinZ = minZ;
            MaxZ = maxZ;

            //todo read coordinates and rastersizes without gdal?
            InputDataset = Gdal.Open(InputFile, Access.GA_ReadOnly);
            double[] outGeoTransform = new double[6];
            InputDataset.GetGeoTransform(outGeoTransform);
            GeoTransform = outGeoTransform;
            RasterXSize = InputDataset.RasterXSize;
            RasterYSize = InputDataset.RasterYSize;
            InputDataset.Dispose();

            foreach (int zoom in Enumerable.Range(MinZ, MaxZ - MinZ + 1))
            {
                double xMin = GeoTransform[0];
                double yMin = GeoTransform[3] - RasterYSize * GeoTransform[1];
                double xMax = GeoTransform[0] + RasterXSize * GeoTransform[1];
                double yMax = GeoTransform[3];

                int[] lonLatToTile = GetTileNumbersFromCoords(xMin, yMin, xMax, yMax, zoom);
                int tileMinX = lonLatToTile[0];
                int tileMinY = lonLatToTile[1];
                int tileMaxX = lonLatToTile[2];
                int tileMaxY = lonLatToTile[3];

                // crop tiles extending world limits (+-180,+-90)
                tileMinX = Math.Max(0, tileMinX);
                tileMinY = Math.Max(0, tileMinY);
                tileMaxX = Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom + 1)) - 1, tileMaxX);
                tileMaxY = Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom)) - 1, tileMaxY);

                MinMax.Add(zoom, new[] {tileMinX, tileMinY, tileMaxX, tileMaxY});
            }

            Image = Image.FromFile(InputFile);
        }

        /// <summary>
        /// Calculates the tile numbers for zoom which covers given lon/lat coordinates.
        /// </summary>
        /// <param name="xMin">Minimum longitude.</param>
        /// <param name="yMin">Minimum latitude.</param>
        /// <param name="xMax">Maximum longitude.</param>
        /// <param name="yMax">Maximum latitude.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <returns>Tile numbers array.</returns>
        private static int[] GetTileNumbersFromCoords(double xMin,
                                                      double yMin,
                                                      double xMax,
                                                      double yMax,
                                                      int zoom)
        {
            double resolution = 180.0 / TileSize / Math.Pow(2, zoom);
            int[] xs = new int[2];
            int[] ys = new int[2];
            xs[0] = Convert.ToInt32(Math.Ceiling((180.0 + xMin) / resolution / TileSize) - 1.0);
            xs[1] = Convert.ToInt32(Math.Ceiling((180.0 + xMax) / resolution / TileSize) - 1.0);
            ys[0] = Convert.ToInt32(Math.Ceiling((90.0 + yMin) / resolution / TileSize) - 1.0);
            ys[1] = Convert.ToInt32(Math.Ceiling((90.0 + yMax) / resolution / TileSize) - 1.0);
            return new[] {xs.Min(), ys.Min(), xs.Max(), ys.Max()};
        }

        /// <summary>
        /// Calculates borders of given tile.
        /// </summary>
        /// <param name="tileX">Tile's x.</param>
        /// <param name="tileY">Tile's y.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <returns>Tile's angles in double array.</returns>
        private static double[] TileBounds(int tileX, int tileY, int zoom)
        {
            double resolution = 180.0 / TileSize / Math.Pow(2.0, zoom);
            return new[]
            {
                tileX * TileSize * resolution - 180.0, tileY * TileSize * resolution - 90.0,
                (tileX + 1) * TileSize * resolution - 180.0, (tileY + 1) * TileSize * resolution - 90.0
            };
        }

        /// <summary>
        /// Ca;culate size and positions to read/write.
        /// </summary>
        /// <param name="upperLeftX">Tile's upper left x coordinate.</param>
        /// <param name="upperLeftY">Tile's upper left y coordinate.</param>
        /// <param name="lowerRightX">Tile's lower right x coordinate.</param>
        /// <param name="lowerRightY">Tile's lower right y coordinate.</param>
        /// <returns>Array with x/y positions and sizes to read; array with x/y positions and sizes to write tiles.</returns>
        private static int[][] GeoQuery(double upperLeftX,
                                        double upperLeftY,
                                        double lowerRightX,
                                        double lowerRightY)
        {
            //Geotiff coordinate borders
            double tiffXMin = GeoTransform[0];
            double tiffYMin = GeoTransform[3] - RasterYSize * GeoTransform[1];
            double tiffXMax = GeoTransform[0] + RasterXSize * GeoTransform[1];
            double tiffYMax = GeoTransform[3];

            //Read from input geotiff in pixels
            double readXMin = RasterXSize * (upperLeftX - tiffXMin) / (tiffXMax - tiffXMin);
            double readYMin = RasterYSize - RasterYSize * (upperLeftY - tiffYMin) / (tiffYMax - tiffYMin);
            double readXMax = RasterXSize * (lowerRightX - tiffXMin) / (tiffXMax - tiffXMin);
            double readYMax = RasterYSize - RasterYSize * (lowerRightY - tiffYMin) / (tiffYMax - tiffYMin);

            //If outside tiff
            readXMin = readXMin < 0.0 ? 0.0 :
                       readXMin > RasterXSize ? RasterXSize : readXMin;
            readYMin = readYMin < 0.0 ? 0.0 :
                       readYMin > RasterYSize ? RasterYSize : readYMin;
            readXMax = readXMax < 0.0 ? 0.0 :
                       readXMax > RasterXSize ? RasterXSize : readXMax;
            readYMax = readYMax < 0.0 ? 0.0 :
                       readYMax > RasterYSize ? RasterYSize : readYMax;

            //Output tile's borders in pixels
            double tileXMin = readXMin.Equals(0.0) ? tiffXMin :
                              readXMin.Equals(RasterXSize) ? tiffXMax : upperLeftX;
            double tileYMin = readYMax.Equals(0.0) ? tiffYMax :
                              readYMax.Equals(RasterYSize) ? tiffYMin : lowerRightY;
            double tileXMax = readXMax.Equals(0.0) ? tiffXMin :
                              readXMax.Equals(RasterXSize) ? tiffXMax : lowerRightX;
            double tileYMax = readYMin.Equals(0.0) ? tiffYMax :
                              readYMin.Equals(RasterYSize) ? tiffYMin : upperLeftY;

            //Positions of dataset to write in tile
            double writeXMin = TileSize - TileSize * (lowerRightX - tileXMin) / (lowerRightX - upperLeftX);
            double writeYMin = TileSize * (upperLeftY - tileYMax) / (upperLeftY - lowerRightY);
            double writeXMax = TileSize - TileSize * (lowerRightX - tileXMax) / (lowerRightX - upperLeftX);
            double writeYMax = TileSize * (upperLeftY - tileYMin) / (upperLeftY - lowerRightY);

            //Sizes to read and write
            double readXSize = readXMax - readXMin;
            double readYSize = readYMax - readYMin;
            double writeXSize = writeXMax - writeXMin;
            double writeYSize = writeYMax - writeYMin;

            //Need more tests with Convert.ToInt32 and return value. Now borders seems better with (int).
            //Shifts
            double readXShift = readXMin - (int) readXMin;
            readXSize += readXShift;
            double readYShift = readYMin - (int) readYMin;
            readYSize += readYShift;
            double writeXShift = writeXMin - (int) writeXMin;
            writeXSize += writeXShift;
            double writeYShift = writeYMin - (int) writeYMin;
            writeYSize += writeYShift;

            return new[]
            {
                new[] {(int) readXMin, (int) readYMin, (int) readXSize, (int) readYSize},
                new[] {(int) writeXMin, (int) writeYMin, (int) writeXSize, (int) writeYSize}
            };
        }

        /// <summary>
        /// Crops passed zoom to tiles.
        /// </summary>
        /// <param name="zoom">Current zoom to crop.</param>
        private static void WriteOneZoom(int zoom)
        {
            for (int currentY = MinMax[zoom][1]; currentY <= MinMax[zoom][3]; currentY++)
            {
                for (int currentX = MinMax[zoom][0]; currentX <= MinMax[zoom][2]; currentX++)
                {
                    // Create directories for the tile
                    Directory.CreateDirectory(Path.Combine(OutputDirectory, $"{zoom}", $"{currentX}"));
                    double[] bounds = TileBounds(currentX, currentY, zoom);

                    // Tile bounds in raster coordinates for ReadRaster query
                    int[][] geoQuery = GeoQuery(bounds[0], bounds[3], bounds[2], bounds[1]);
                    Metadata.Add(new Dictionary<string, int>
                    {
                        {"TileX", currentX},
                        {"TileY", currentY},
                        {"TileZoom", zoom},
                        {"ReadXPos", geoQuery[0][0]},
                        {"ReadYPos", geoQuery[0][1]},
                        {"ReadXSize", geoQuery[0][2]},
                        {"ReadYSize", geoQuery[0][3]},
                        {"WriteXPos", geoQuery[1][0]},
                        {"WriteYPos", geoQuery[1][1]},
                        {"WriteXSize", geoQuery[1][2]},
                        {"WriteYSize", geoQuery[1][3]}
                    });
                }
            }

            foreach (Dictionary<string, int> metadata in Metadata)
            {
                string outputFileName = Path.Combine(OutputDirectory, $"{metadata["TileZoom"]}",
                                                     $"{metadata["TileX"]}",
                                                     $"{metadata["TileY"]}.{TileExtension}");
                using (Bitmap target = new Bitmap(TileSize, TileSize))
                {
                    using (Graphics graphics = Graphics.FromImage(target))
                    {
                        graphics.DrawImage(Image,
                                           new Rectangle(metadata["WriteXPos"],
                                                         metadata["WriteYPos"],
                                                         metadata["WriteXSize"],
                                                         metadata["WriteYSize"]),
                                           new Rectangle(metadata["ReadXPos"],
                                                         metadata["ReadYPos"],
                                                         metadata["ReadXSize"],
                                                         metadata["ReadYSize"]),
                                           GraphicsUnit.Pixel);
                    }

                    target.Save(outputFileName, ImageFormat.Png);
                }
            }

            Metadata.Clear();
        }

        #endregion

        #region Public

        /// <summary>
        /// Create tiles.
        /// </summary>
        /// <param name="inputFile">FullName of input GeoTIFF.</param>
        /// <param name="outputDirectory">FullName of output directory.</param>
        /// <param name="minZ">Minimum zoom.</param>
        /// <param name="maxZ">Maxmimum zoom.</param>
        public static void GenerateTiles(string inputFile, string outputDirectory, int minZ, int maxZ)
        {
            //Initialize properties.
            Initialize(inputFile, outputDirectory, minZ, maxZ);

            //Crop tiles for each zoom.
            for (int zoom = MaxZ; zoom >= MinZ; zoom--) WriteOneZoom(zoom);

            //Disposing used values.
            Image.Dispose();
            MinMax.Clear();
        }

        #endregion

        #endregion
    }
}
