using System;
using System.Diagnostics;

namespace GTiff2Tiles.Test
{
    internal static class Program
    {
        #region Properties

        private static string InputFile { get; set; }

        private static string OutputDirectory { get; set; }

        private static int MinZ { get; set; }

        private static int MaxZ { get; set; }

        //todo: progress reporting

        private static int _progress;

        public static int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        #endregion

        private static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //Write your parameters here, if you want to debug it without arguments.
            InputFile = args.Length > 0 ? args[0] : "D:/Examples/test.tif";
            OutputDirectory = args.Length > 1 ? args[1] : "D:/Result";
            MinZ = args.Length > 2 ? int.Parse(args[2]) : 10;
            MaxZ = args.Length > 3 ? int.Parse(args[3]) : 17;

            //Configure gdal's location.
            GdalConfiguration.ConfigureGdal();

            //Now only CubicSpline and Cubic are supported for resampling algorythm.
            //Gdal2Tiles.CropTifToTiles(InputFile, OutputDirectory, MinZ, MaxZ, ResampleAlg.GRA_CubicSpline);

            //Works much faster (in one thread like gdal2tiles in multithreaded mode), uses less memory/cpu, better tiles borders.
            using (GTiff2Tiles gTiff2Tiles = new GTiff2Tiles(InputFile, OutputDirectory, MinZ, MaxZ))
            {
                if (!gTiff2Tiles.GenerateTiles())
                {
                    Console.WriteLine("Error occured while cropping tiles.");
                    #if DEBUG
                    Console.WriteLine("Press any key to exit.");
                    Console.ReadKey();
                    #endif
                }
            }

            Console.WriteLine("Done!");
            Console.WriteLine($"Days:{stopwatch.Elapsed.Days} hours:{stopwatch.Elapsed.Hours} minutes:{stopwatch.Elapsed.Minutes} "
                            + $"seconds:{stopwatch.Elapsed.Seconds} ms:{stopwatch.Elapsed.Milliseconds}");
            #if DEBUG
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            #endif
        }
    }
}
