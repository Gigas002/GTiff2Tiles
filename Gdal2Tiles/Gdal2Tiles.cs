using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSGeo.GDAL;

namespace Gdal2Tiles
{
    /// <summary>
    /// Contains static methods and properties for cropping GeoTIFFs into tiles.
    /// </summary>
    public static class Gdal2Tiles
    {
        #region Properties

        #region Private

        /// <summary>
        /// Dataset's x size.
        /// </summary>
        private static int RasterXSize { get; set; }

        /// <summary>
        /// Dataset's y size.
        /// </summary>
        private static int RasterYSize { get; set; }

        /// <summary>
        /// GeoTransform of output files.
        /// </summary>
        private static double[] OutGeoTransform { get; set; }

        /// <summary>
        /// Query size.
        /// </summary>
        private static int QuerySize { get; set; }

        /// <summary>
        /// Dictionary with min and max tiles numbers for each zoom.
        /// </summary>
        private static Dictionary<int, int[]> MinMax { get; } = new Dictionary<int, int[]>();

        /// <summary>
        /// FullName of input file.
        /// </summary>
        private static string InputFile { get; set; }

        /// <summary>
        /// FullName of output directory.
        /// </summary>
        private static string OutputDirectory { get; set; }

        /// <summary>
        /// Minimum zoom.
        /// </summary>
        private static int MinZ { get; set; }

        /// <summary>
        /// Maximum zoom.
        /// </summary>
        private static int MaxZ { get; set; }

        /// <summary>
        /// Number of bands in input dataset.
        /// </summary>
        private static int DataBandsCount { get; set; }

        /// <summary>
        /// File extension of output tiles.
        /// </summary>
        private static string TileExtension { get; } = "png";

        /// <summary>
        /// Size of output tiles.
        /// </summary>
        private static int TileSize { get; } = 256;

        /// <summary>
        /// Previously known as TileDetail class.
        /// </summary>
        private static List<Dictionary<string, int>> Metadata { get; } = new List<Dictionary<string, int>>();

        /// <summary>
        /// Defines resampling algorithm.
        /// </summary>
        private static ResampleAlg Resampling { get; set; }

        #endregion

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Initilizes default fields.
        /// </summary>
        /// <param name="inputFile">FullName of input GeoTIFF.</param>
        /// <param name="outputDirectory">FullName of output directory.</param>
        /// <param name="minZ">Minimum zoom.</param>
        /// <param name="maxZ">Maxmimum zoom.</param>
        /// <param name="resampling">Resampling method.</param>
        private static void Initialize(string inputFile,
            string outputDirectory,
            int minZ,
            int maxZ,
            ResampleAlg resampling)
        {
            InputFile = inputFile;
            OutputDirectory = outputDirectory;
            MinZ = minZ;
            MaxZ = maxZ;
            Resampling = resampling;
            QuerySize = 4 * TileSize;

            // Open the input file
            Dataset inputDataset = Gdal.Open(InputFile, Access.GA_ReadOnly);
            DataBandsCount = inputDataset.RasterCount;

            // Read the georeference
            double[] outGeoTransform = new double[6];
            inputDataset.GetGeoTransform(outGeoTransform);
            OutGeoTransform = outGeoTransform;

            // Set x/y sizes of dataset
            RasterXSize = inputDataset.RasterXSize;
            RasterYSize = inputDataset.RasterYSize;

            // Dispose InputDataset
            inputDataset.Dispose();

            // Generate dictionary with min max tile coordinates for all zoomlevels
            foreach (int zoom in Enumerable.Range(MinZ, MaxZ - MinZ + 1))
            {
                double xMin = OutGeoTransform[0];
                double yMin = OutGeoTransform[3] - RasterYSize * OutGeoTransform[1];
                double xMax = OutGeoTransform[0] + RasterXSize * OutGeoTransform[1];
                double yMax = OutGeoTransform[3];

                int[] lonLatToTile = GetTileNumbersFromCoords(xMin, yMin, xMax, yMax, TileSize, zoom);
                int tileMinX = lonLatToTile[0];
                int tileMinY = lonLatToTile[1];
                int tileMaxX = lonLatToTile[2];
                int tileMaxY = lonLatToTile[3];

                // crop tiles extending world limits (+-180,+-90)
                tileMinX = Math.Max(0, tileMinX);
                tileMinY = Math.Max(0, tileMinY);
                tileMaxX = Math.Min(Convert.ToInt32(Math.Pow(2, zoom + 1)) - 1, tileMaxX);
                tileMaxY = Math.Min(Convert.ToInt32(Math.Pow(2, zoom)) - 1, tileMaxY);
                MinMax.Add(zoom, new[] {tileMinX, tileMinY, tileMaxX, tileMaxY});
            }
        }

        /// <summary>
        /// Calculates parameters for ReadRaster() and WriteRaster().
        /// </summary>
        /// <param name="upperLeftX">Upper left x coordinate.</param>
        /// <param name="upperLeftY">Upper left y coordinate.</param>
        /// <param name="lowerRightX">Lower right x coordinate.</param>
        /// <param name="lowerRightY">Lower right y coordinate.</param>
        /// <param name="querySize">Query size,</param>
        /// <returns>Parameters in raster coordinates and x/y shifts(for border tiles).</returns>
        [SuppressMessage("ReSharper", "InvertIf")]
        private static int[][] GeoQuery(double upperLeftX,
            double upperLeftY,
            double lowerRightX,
            double lowerRightY,
            int querySize)
        {
            double readXPos = (upperLeftX - OutGeoTransform[0]) / OutGeoTransform[1] + 0.001;
            double readYPos = (upperLeftY - OutGeoTransform[3]) / OutGeoTransform[5] + 0.001;
            double readXSize = (lowerRightX - upperLeftX) / OutGeoTransform[1] + 0.5;
            double readYSize = (lowerRightY - upperLeftY) / OutGeoTransform[5] + 0.5;
            double writeXSize = querySize;
            double writeYSize = querySize;

            // Coordinates should not go out of the bounds of the raster
            double writeXPos = 0.0;
            if (readXPos < 0.0)
            {
                double readXShift = Math.Abs(readXPos);
                writeXPos = writeXSize * readXShift / readXSize;
                writeXSize = writeXSize - writeXPos;
                readXSize = readXSize - readXSize * readXShift / readXSize;
                readXPos = 0.0;
            }

            if (readXPos + readXSize > RasterXSize)
            {
                writeXSize = writeXSize * (RasterXSize - readXPos) / readXSize;
                readXSize = RasterXSize - readXPos;
            }

            double writeYPos = 0.0;
            if (readYPos < 0.0)
            {
                double readYShift = Math.Abs(readYPos);
                writeYPos = writeYSize * readYShift / readYSize;
                writeYSize = writeYSize - writeYPos;
                readYSize = readYSize - readYSize * readYShift / readYSize;
                readYPos = 0.0;
            }

            if (readYPos + readYSize > RasterYSize)
            {
                writeYSize = writeYSize * (RasterYSize - readYPos) / readYSize;
                readYSize = RasterYSize - readYPos;
            }

            return new[]
            {
                new[]
                {
                    Convert.ToInt32(readXPos), Convert.ToInt32(readYPos), Convert.ToInt32(readXSize),
                    Convert.ToInt32(readYSize)
                },
                new[]
                {
                    Convert.ToInt32(writeXPos), Convert.ToInt32(writeYPos), Convert.ToInt32(writeXSize),
                    Convert.ToInt32(writeYSize)
                }
            };
        }

        /// <summary>
        /// Calculates the tile numbers for zoom which covers given lon/lat coordinates.
        /// </summary>
        /// <param name="xMin">Minimum longitude.</param>
        /// <param name="yMin">Minimum latitude.</param>
        /// <param name="xMax">Maximum longitude.</param>
        /// <param name="yMax">Maximum latitude.</param>
        /// <param name="tileSize">Tile's size.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <returns>Tile numbers array.</returns>
        private static int[] GetTileNumbersFromCoords(double xMin,
            double yMin,
            double xMax,
            double yMax,
            double tileSize,
            int zoom)
        {
            double resolution = 180.0 / tileSize / Math.Pow(2, zoom);
            int[] xs = new int[2];
            int[] ys = new int[2];
            xs[0] = Convert.ToInt32(Math.Ceiling((180.0 + xMin) / resolution / tileSize) - 1.0);
            xs[1] = Convert.ToInt32(Math.Ceiling((180.0 + xMax) / resolution / tileSize) - 1.0);
            ys[0] = Convert.ToInt32(Math.Ceiling((90.0 + yMin) / resolution / tileSize) - 1.0);
            ys[1] = Convert.ToInt32(Math.Ceiling((90.0 + yMax) / resolution / tileSize) - 1.0);
            return new[] {xs.Min(), ys.Min(), xs.Max(), ys.Max()};
        }

        /// <summary>
        /// Calculates borders of given tile.
        /// </summary>
        /// <param name="tileX">Tile's x.</param>
        /// <param name="tileY">Tile's y.</param>
        /// <param name="tileSize">Tile's size.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <returns>Tile's angles in double array.</returns>
        private static double[] TileBounds(int tileX, int tileY, int tileSize, int zoom)
        {
            double resolution = 180.0 / tileSize / Math.Pow(2, zoom);
            return new[]
            {
                tileX * tileSize * resolution - 180.0, tileY * tileSize * resolution - 90.0,
                (tileX + 1) * tileSize * resolution - 180.0, (tileY + 1) * tileSize * resolution - 90.0
            };
        }

        /// <summary>
        /// Generation of the base tiles (the lowest in the pyramid) directly from the input raster.
        /// </summary>
        private static void GenerateBaseTiles()
        {
            for (int currentY = MinMax[MaxZ][1]; currentY <= MinMax[MaxZ][3]; currentY++)
            {
                for (int currentX = MinMax[MaxZ][0]; currentX <= MinMax[MaxZ][2]; currentX++)
                {
                    // Create directories for the tile
                    Directory.CreateDirectory(Path.Combine(OutputDirectory, $"{MaxZ}", $"{currentX}"));
                    double[] bounds = TileBounds(currentX, currentY, TileSize, MaxZ);

                    // Tile bounds in raster coordinates for ReadRaster query
                    int[][] geoQuery = GeoQuery(bounds[0], bounds[3], bounds[2], bounds[1], QuerySize);
                    Metadata.Add(new Dictionary<string, int>
                    {
                        {"TileX", currentX},
                        {"TileY", currentY},
                        {"TileZoom", MaxZ},
                        {"ReadPosX", geoQuery[0][0]},
                        {"ReadPosY", geoQuery[0][1]},
                        {"ReadXSize", geoQuery[0][2]},
                        {"ReadYSize", geoQuery[0][3]},
                        {"WritePosX", geoQuery[1][0]},
                        {"WritePosY", geoQuery[1][1]},
                        {"WriteXSize", geoQuery[1][2]},
                        {"WriteYSize", geoQuery[1][3]},
                        {"QuerySize", QuerySize}
                    });
                }
            }
        }

        /// <summary>
        /// Scales down query dataset to the tile dataset.
        /// </summary>
        /// <param name="queryDataset">Source dataset.</param>
        /// <param name="tileDataset">Destination dataset.</param>
        /// <param name="resampling">Resampling method.</param>
        private static void ScaleQueryToTile(Dataset queryDataset, Dataset tileDataset, ResampleAlg resampling)
        {
            queryDataset.SetGeoTransform(new[]
            {
                0.0,
                Convert.ToSingle(tileDataset.RasterXSize) / queryDataset.RasterXSize,
                0.0, 0.0, 0.0,
                Convert.ToSingle(tileDataset.RasterXSize) / queryDataset.RasterXSize
            });
            tileDataset.SetGeoTransform(new[] {0.0, 1.0, 0.0, 0.0, 0.0, 1.0});
            Gdal.ReprojectImage(queryDataset, tileDataset, null, null, resampling, 0.0, 0.0, null, null, null);
        }

        /// <summary>
        /// Creates and writes base pyramid tile.
        /// </summary>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        private static void CreateBaseTile()
        {
            using (Driver memDriver = Gdal.GetDriverByName("MEM"))
            {
                Parallel.ForEach(Metadata, metadata =>
                {
                    using (Dataset tileDataset = memDriver.Create("", TileSize,
                        TileSize,
                        DataBandsCount + 1,
                        DataType.GDT_Byte,
                        null))
                    {
                        byte[] data = new byte[metadata["WriteXSize"]
                                               * metadata["WriteYSize"]
                                               * DataBandsCount];
                        byte[] alpha = new byte[metadata["WriteXSize"] * metadata["WriteYSize"]];

                        // We scale down the query to the tilesize by supplied algorithm.
                        if (metadata["ReadXSize"] != 0
                            && metadata["ReadYSize"] != 0
                            && metadata["WriteXSize"] != 0
                            && metadata["WriteYSize"] != 0)
                        {
                            using (Dataset inputDataset = Gdal.Open(InputFile, Access.GA_ReadOnly))
                            {
                                inputDataset.ReadRaster(metadata["ReadPosX"], metadata["ReadPosY"],
                                    metadata["ReadXSize"],
                                    metadata["ReadYSize"], data, metadata["WriteXSize"],
                                    metadata["WriteYSize"],
                                    DataBandsCount,
                                    Enumerable.Range(1, DataBandsCount + 1)
                                              .ToArray(),
                                    0, 0,
                                    0);
                                using (Band alphaBand = inputDataset.GetRasterBand(1).GetMaskBand())
                                {
                                    alphaBand.ReadRaster(metadata["ReadPosX"], metadata["ReadPosY"],
                                        metadata["ReadXSize"],
                                        metadata["ReadYSize"], alpha, metadata["WriteXSize"],
                                        metadata["WriteYSize"], 0, 0);
                                }
                            }
                        }

                        if (data.All(bytes => bytes <= 0))
                            return;

                        using (Dataset queryDataset = memDriver.Create("", metadata["QuerySize"],
                            metadata["QuerySize"],
                            DataBandsCount + 1,
                            DataType.GDT_Byte,
                            null))
                        {
                            queryDataset.WriteRaster(metadata["WritePosX"], metadata["WritePosY"],
                                metadata["WriteXSize"],
                                metadata["WriteYSize"], data, metadata["WriteXSize"],
                                metadata["WriteYSize"],
                                DataBandsCount,
                                Enumerable.Range(1, DataBandsCount + 1).ToArray(), 0, 0,
                                0);
                            queryDataset.WriteRaster(metadata["WritePosX"], metadata["WritePosY"],
                                metadata["WriteXSize"],
                                metadata["WriteYSize"], alpha, metadata["WriteXSize"],
                                metadata["WriteYSize"],
                                1,
                                new[] {DataBandsCount + 1}, 0, 0,
                                0);
                            ScaleQueryToTile(queryDataset, tileDataset, Resampling);
                        }

                        // Write a copy of tile to png
                        using (Driver tileDriver = Gdal.GetDriverByName("PNG"))
                        {
                            tileDriver.CreateCopy(Path.Combine(OutputDirectory, $"{metadata["TileZoom"]}",
                                    $"{metadata["TileX"]}",
                                    $"{metadata["TileY"]}.{TileExtension}"),
                                tileDataset, 0, null, null, null);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// Generation of the overview tiles (higher in the pyramid) based on existing tiles.
        /// </summary>
        private static void CreateOverviewTiles()
        {
            // Usage of existing tiles: from 4 underlying tiles generate one as overview.
            for (int currentZoom = MaxZ - 1; currentZoom >= MinZ; currentZoom--)
            {
                int tileMinX = MinMax[currentZoom][0];
                int tileMinY = MinMax[currentZoom][1];
                int tileMaxX = MinMax[currentZoom][2];
                int tileMaxY = MinMax[currentZoom][3];
                int zoom = currentZoom;
                Parallel.For(tileMinY, tileMaxY + 1, currentY => Parallel.For(tileMinX, tileMaxX + 1, currentX =>
                {
                    string tileFileName = Path.Combine(OutputDirectory, $"{zoom}", $"{currentX}",
                        $"{currentY}.{TileExtension}");
                    Directory.CreateDirectory(Path.GetDirectoryName(tileFileName)
                                              ?? throw new InvalidOperationException());

                    // fill the null value
                    using (Driver memDriver = Gdal.GetDriverByName("MEM"))
                    {
                        using (Dataset tileDataset = memDriver.Create("", TileSize,
                            TileSize,
                            DataBandsCount + 1,
                            DataType.GDT_Byte, null))
                        {
                            using (Dataset queryDataset = memDriver.Create("", 2 * TileSize,
                                2 * TileSize,
                                DataBandsCount + 1,
                                DataType.GDT_Byte,
                                null))
                            {
                                // Read the tiles and write them to query window
                                for (int y = currentY * 2; y < currentY * 2 + 2; y++)
                                {
                                    for (int x = currentX * 2; x < 2 * currentX + 2; x++)
                                    {
                                        int minX = MinMax[zoom + 1][0];
                                        int minY = MinMax[zoom + 1][1];
                                        int maxX = MinMax[zoom + 1][2];
                                        int maxY = MinMax[zoom + 1][3];
                                        if (x < minX || x > maxX || y < minY || y > maxY) continue;
                                        if (!File.Exists(Path.Combine(OutputDirectory,
                                            $"{zoom + 1}",
                                            $"{x}",
                                            $"{y}.{TileExtension}")))
                                            continue;
                                        using (Dataset queryTileDataset = Gdal.Open(Path.Combine(OutputDirectory,
                                                $"{zoom + 1}",
                                                $"{x}",
                                                $"{y}.{TileExtension}"),
                                            Access.GA_ReadOnly))
                                        {
                                            int tilePosY;
                                            if (currentY == 0 && y == 1
                                                || currentY != 0 && y % (2 * currentY) != 0)
                                                tilePosY = 0;
                                            else
                                                tilePosY = TileSize;

                                            int tilePosX;
                                            if (currentX != 0)
                                                tilePosX = x % (2 * currentX) * TileSize;
                                            else if (currentX == 0 && x == 1)
                                                tilePosX = TileSize;
                                            else
                                                tilePosX = 0;

                                            byte[] readRaster =
                                                new byte[TileSize
                                                         * TileSize
                                                         * (DataBandsCount + 1)];
                                            queryTileDataset.ReadRaster(0, 0, TileSize,
                                                TileSize,
                                                readRaster, TileSize,
                                                TileSize,
                                                DataBandsCount + 1,
                                                Enumerable
                                                    .Range(1, DataBandsCount + 2)
                                                    .ToArray(),
                                                0,
                                                0, 0);

                                            if (readRaster.All(b => b <= 0))
                                                continue;

                                            queryDataset.WriteRaster(tilePosX, tilePosY, TileSize,
                                                TileSize,
                                                readRaster, TileSize,
                                                TileSize,
                                                DataBandsCount + 1,
                                                Enumerable
                                                    .Range(1, DataBandsCount + 2)
                                                    .ToArray(),
                                                0, 0,
                                                0);
                                        }
                                    }
                                }

                                ScaleQueryToTile(queryDataset, tileDataset, Resampling);
                            }

                            // Write a copy of tile to png/jpg
                            using (Driver tileDriver = Gdal.GetDriverByName("PNG"))
                            {
                                tileDriver.CreateCopy(tileFileName, tileDataset, 0, null, null, null);
                            }
                        }
                    }
                }));
            }
        }

        #endregion

        #region Public

        /// <summary>
        /// Crops input GeoTIFF to tiles.
        /// Important: You should call your <see cref="GdalConfiguration.ConfigureGdal"/> method from Gdal.NET package BEFORE using this method.
        /// </summary>
        /// <param name="inputFile">FullName of input GeoTIFF.</param>
        /// <param name="outputDirectory">FullName of output directory.</param>
        /// <param name="minZ">Minimum zoom.</param>
        /// <param name="maxZ">Maxmimum zoom.</param>
        /// <param name="resampling">Resampling method.</param>
        public static void CropTifToTiles(string inputFile,
            string outputDirectory,
            int minZ,
            int maxZ,
            ResampleAlg resampling)
        {
            Initialize(inputFile, outputDirectory, minZ, maxZ, resampling);
            GenerateBaseTiles();
            CreateBaseTile();
            CreateOverviewTiles();
            MinMax.Clear();
            foreach (Dictionary<string, int> d in Metadata) d.Clear();
            Metadata.Clear();
        }

        #endregion

        #endregion
    }
}
