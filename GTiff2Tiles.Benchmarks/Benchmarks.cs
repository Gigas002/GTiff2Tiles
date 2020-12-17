using System;
using System.Globalization;
using System.IO;
using BenchmarkDotNet.Attributes;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Benchmarks
{
    [SimpleJob(10, 3, 5)]
    public class Benchmark
    {
        /*
         * We're creating GEODETIC png tiles from EPSG:4326 input file
         * resampling is cubic, zooms 0-15, process counter 8
         */

        #region Properties and consts

        private static string GetOutDirectory(string dirName)
        {
            string ts = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs, CultureInfo.InvariantCulture);

            return $"{dirName}_{ts}";
        }

        private const string GTiff2TilesOut = "gtiff2tiles";

        private const string Gdal2TilesOut = "gdal2tiles";

        private const string MaptilerOut = "maptiler";

        internal const string GdalName = "osgeo/gdal";

        internal const string MaptilerName = "maptiler/engine";

        private readonly int _tc = Environment.ProcessorCount;

        private const int MinZ = 0;

        private const int MaxZ = 15;

        internal const string Data = "data";

        internal const string In = "in";

        internal const string Itif = "i.tif";

        private static readonly string MountDir = AppContext.BaseDirectory;

        private readonly string _runArgs = $"--rm -v \"{MountDir}/data:/data\"";

        #endregion

        #region Benchmarks

        [Benchmark]
        public void RunGTiff2Tiles()
        {
            string path = $"{Data}/{In}/{Itif}";

            Directory.CreateDirectory($"{Data}/{In}");
            File.Copy($"../../../../{Data}/{In}/{Itif}", path, true);

            using Raster raster = new(path, CoordinateSystem.Epsg4326);
            raster.WriteTilesToDirectory($"{Data}/{GetOutDirectory(GTiff2TilesOut)}", MinZ, MaxZ,
                                         false, Tile.DefaultSize, TileExtension.Png, Interpolation.Cubic, 4, threadsCount: _tc);
        }

        [Benchmark]
        public void RunGdal2Tiles()
        {
            string path = $"{Data}/{In}/{Itif}";

            Directory.CreateDirectory($"{Data}/{In}");
            File.Copy($"../../../../{Data}/{In}/{Itif}", path, true);

            string gdalArgs = $"gdal2tiles.py -s EPSG:4326 -p geodetic -r cubic -z {MinZ}-{MaxZ} " +
                              $"--processes {_tc} {path} {Data}/{GetOutDirectory(Gdal2TilesOut)}";

            Docker.Run(_runArgs, GdalName, gdalArgs);
        }

        [Benchmark]
        public void RunMaptiler()
        {
            string path = $"{Data}/{In}/{Itif}";

            Directory.CreateDirectory($"{Data}/{In}");
            File.Copy($"../../../../{Data}/{In}/{Itif}", path, true);

            string maptilerArgs =
                $"maptiler -srs EPSG:4326 -preset geodetic -resampling cubic -zoom {MinZ} {MaxZ} " +
                $"-P {_tc} -f png32 -o {GetOutDirectory(MaptilerOut)} {In}/{Itif}";

            Docker.Run(_runArgs, MaptilerName, maptilerArgs);
        }

        #endregion
    }
}
