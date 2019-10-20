using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using GTiff2Tiles.Benchmarks;
using GTiff2Tiles.NetCore.Core.Image;

// ReSharper disable LocalizableElement
// ReSharper disable UnusedParameter.Local

namespace GTiff2Tiles.NetCore.Benchmarks
{
    internal static class Program
    {
        #region Properties

        /// <summary>
        /// Input file.
        /// </summary>
        private static FileInfo InputFileInfo { get; set; }

        /// <summary>
        /// Output directory.
        /// </summary>
        private static DirectoryInfo OutputDirectoryInfo { get; set; }

        /// <summary>
        /// Temp directory.
        /// </summary>
        private static DirectoryInfo TempDirectoryInfo { get; set; }

        /// <summary>
        /// Minimum cropped zoom.
        /// </summary>
        private static int MinZ { get; set; }

        /// <summary>
        /// Maximum cropped zoom.
        /// </summary>
        private static int MaxZ { get; set; }

        /// <summary>
        /// Shows if there were console line parsing errors.
        /// </summary>
        private static bool IsParsingErrors { get; set; }

        /// <summary>
        /// Threads count.
        /// </summary>
        private static int ThreadsCount { get; set; }

        #endregion

        internal static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Parser.Default.ParseArguments<Options>(args).WithParsed(ParseConsoleOptions)
                  .WithNotParsed(error => IsParsingErrors = true);

            if (IsParsingErrors) throw new Exception("Error while parsing occured");

            await RunTilingAsync().ConfigureAwait(false);

            Console.WriteLine("Tests ended. Press any button to close the application.");
            Console.ReadKey();
        }

        private static void ParseConsoleOptions(Options options)
        {
            //Check if string options are empty strings.
            if (string.IsNullOrWhiteSpace(options.InputFilePath))
            {
                IsParsingErrors = true;

                return;
            }

            if (string.IsNullOrWhiteSpace(options.OutputDirectoryPath))
            {
                IsParsingErrors = true;

                return;
            }

            if (string.IsNullOrWhiteSpace(options.TempDirectoryPath))
            {
                IsParsingErrors = true;

                return;
            }

            //Check zooms.
            if (options.MinZ < 0)
            {
                IsParsingErrors = true;

                return;
            }

            if (options.MaxZ < 0)
            {
                IsParsingErrors = true;

                return;
            }

            if (options.MinZ > options.MaxZ)
            {
                IsParsingErrors = true;

                return;
            }

            //Threads check.
            if (options.ThreadsCount <= 0)
            {
                IsParsingErrors = true;

                return;
            }

            //Set properties values.
            InputFileInfo = new FileInfo(options.InputFilePath);
            OutputDirectoryInfo = new DirectoryInfo(options.OutputDirectoryPath);
            TempDirectoryInfo = new DirectoryInfo(options.TempDirectoryPath);
            MinZ = options.MinZ;
            MaxZ = options.MaxZ;
            ThreadsCount = options.ThreadsCount;
        }

        private static async ValueTask RunTilingAsync()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            #region GTiff2Tiles

            string gtiff2TilesOutputDirectoryPath = Path.Combine(OutputDirectoryInfo.FullName, "gtiff2tiles");
            string gtiff2TilesTempDirectoryPath = Path.Combine(TempDirectoryInfo.FullName, "gtiff2tiles");
            Directory.CreateDirectory(gtiff2TilesOutputDirectoryPath);
            Directory.CreateDirectory(gtiff2TilesTempDirectoryPath);
            Image image = new Image(InputFileInfo);
            await image.GenerateTilesByCroppingAsync(new DirectoryInfo(gtiff2TilesOutputDirectoryPath), MinZ, MaxZ, true,
                                                new Progress<double>(), ThreadsCount).ConfigureAwait(false);
            stopwatch.Stop();
            Console.WriteLine("GTiff2Tiles process ended.");
            Console.WriteLine("Time passed:");
            Console.WriteLine($"Hours:{stopwatch.Elapsed.Hours}, Minutes:{stopwatch.Elapsed.Minutes}, " +
                              $"Seconds:{stopwatch.Elapsed.Seconds}, Ms:{stopwatch.Elapsed.Milliseconds}.");
            stopwatch.Restart();

            #endregion

            #region MapTiler

            string maptilerOutputDirectoryPath = Path.Combine(OutputDirectoryInfo.FullName, "maptiler");
            string maptilerTempDirectoryPath = Path.Combine(TempDirectoryInfo.FullName, "maptiler");
            Directory.CreateDirectory(maptilerOutputDirectoryPath);
            Directory.CreateDirectory(maptilerTempDirectoryPath);
            await Task.Run(() => RunMapTiler(InputFileInfo.FullName, maptilerOutputDirectoryPath,
                                             maptilerTempDirectoryPath, MinZ, MaxZ, ThreadsCount))
                      .ConfigureAwait(false);

            stopwatch.Stop();
            Console.WriteLine("MapTiler process ended.");
            Console.WriteLine("Time passed:");
            Console.WriteLine($"Hours:{stopwatch.Elapsed.Hours}, Minutes:{stopwatch.Elapsed.Minutes}, " +
                              $"Seconds:{stopwatch.Elapsed.Seconds}, Ms:{stopwatch.Elapsed.Milliseconds}.");
            stopwatch.Restart();

            #endregion

            #region Gdal2Tiles

            string gdal2TilesOutputDirectoryPath = Path.Combine(OutputDirectoryInfo.FullName, "gdal2tiles");
            Directory.CreateDirectory(gdal2TilesOutputDirectoryPath);
            await Task.Run(() => RunGdal2Tiles(InputFileInfo.FullName, gdal2TilesOutputDirectoryPath, MinZ, MaxZ,
                                               ThreadsCount)).ConfigureAwait(false);

            stopwatch.Stop();
            Console.WriteLine("Gdal2Tiles process ended.");
            Console.WriteLine("Time passed:");
            Console.WriteLine($"Hours:{stopwatch.Elapsed.Hours}, Minutes:{stopwatch.Elapsed.Minutes}, " +
                              $"Seconds:{stopwatch.Elapsed.Seconds}, Ms:{stopwatch.Elapsed.Milliseconds}.");

            #endregion
        }

        private static void RunMapTiler(string inputFilePath, string outputDirectoryPath, string tempDirectoryPath,
                                        int minZ, int maxZ, int threadsCount)
        {
            using (Process maptilerProcess = new Process
            {
                StartInfo = new ProcessStartInfo("maptiler")
                {
                    Arguments = $"-geodetic -tms -resampling cubic_spline -f png32 -P {threadsCount} " +
                                $"-o \"{outputDirectoryPath}\" -work_dir \"{tempDirectoryPath}\" " +
                                $"-srs EPSG:4326 -zoom {minZ} {maxZ} \"{inputFilePath}\"",
                    CreateNoWindow = true, RedirectStandardInput = true, RedirectStandardOutput = true, UseShellExecute = false
                }
            })
            {
                maptilerProcess.Start();
                maptilerProcess.WaitForExit();
            }
        }

        private static void RunGdal2Tiles(string inputFilePath, string outputDirectoryPath, int minZ, int maxZ,
                                          int threadsCount)
        {
            using (Process gdal2TilesProcess = new Process
            {
                //Should be placed in Gdal2Tiles directory near Benchmarks binaries.
                StartInfo = new ProcessStartInfo("Gdal2Tiles/Gdal2Tiles")
                {
                    Arguments = $"-s EPSG:4326 -p geodetic -r cubicspline --tmscompatible -z {minZ}-{maxZ} " +
                                //$"--processes={threadsCount} " + //TODO: doesn't work
                                $"\"{inputFilePath}\" \"{outputDirectoryPath}\"",
                    CreateNoWindow = true, RedirectStandardInput = true, RedirectStandardOutput = true, UseShellExecute = false
                }
            })
            {
                gdal2TilesProcess.Start();
                gdal2TilesProcess.WaitForExit();
            }
        }
    }
}
