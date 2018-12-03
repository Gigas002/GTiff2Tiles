using System;
using System.Diagnostics;
using OSGeo.GDAL;

namespace Gdal2Tiles
{
    internal static class Program
    {
        #region Properties

        private static string InputFile { get; set; }

        private static string OutputDirectory { get; set; }

        private static int MinZ { get; set; }

        private static int MaxZ { get; set; }

        //todo: progress reporting
        //public static int Progress { get; set; }

        #endregion

        private static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //Write your parameters here, if you want to debug it without arguments.
            InputFile = args.Length > 0 ? args[0] : "C:/test.tif";
            OutputDirectory = args.Length > 1 ? args[1] : "C:/result";
            MinZ = args.Length > 2 ? int.Parse(args[2]) : 10;
            MaxZ = args.Length > 3 ? int.Parse(args[3]) : 14;

            //Configure gdal's location.
            GdalConfiguration.ConfigureGdal();

            //Now only CubicSpline and Cubic are supported for resampling algorythm.
            //Gdal2Tiles.CropTifToTiles(InputFile, OutputDirectory, MinZ, MaxZ, ResampleAlg.GRA_CubicSpline);

            //Works much faster (in one thread like gdal2tiles in multithreaded mode), uses less memory/cpu, better tiles borders.
            GTiff2Tiles.GenerateTiles(InputFile, OutputDirectory, MinZ, MaxZ);

            Console.WriteLine("Done!");
            Console.WriteLine($"Days:{stopwatch.Elapsed.Days} hours:{stopwatch.Elapsed.Hours} minutes:{stopwatch.Elapsed.Minutes} "
                            + $"seconds:{stopwatch.Elapsed.Seconds} ms:{stopwatch.Elapsed.Milliseconds}");
            #if DEBUG
            Console.ReadKey();
            #endif
        }
    }
}
