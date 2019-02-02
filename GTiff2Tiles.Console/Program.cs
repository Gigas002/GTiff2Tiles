using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommandLine;

namespace GTiff2Tiles.Console
{
    internal static class Program
    {
        #region Properties

        /// <summary>
        /// Input file.
        /// </summary>
        private static FileInfo InputFIleInfo { get; set; }

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

        private static async Task Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                Parser.Default.ParseArguments<Options>(args)
                      .WithParsed(ParseConsoleOptions)
                      .WithNotParsed(error => IsParsingErrors = true);
            }
            catch (Exception exception)
            {
                //Catch some uncaught parsing errors.
                Helpers.ErrorHelper.PrintException(exception);
                return;
            }

            if (IsParsingErrors)
            {
                Helpers.ErrorHelper.PrintError("Unable to parse console options.");
                return;
            }

            //Create progress-reporter.
            ConsoleProgress<int> consoleProgress = new ConsoleProgress<int>(System.Console.WriteLine);

            //Run crop asynchroniously.
            await GenerateTiles(InputFIleInfo, OutputDirectoryInfo, TempDirectoryInfo, MinZ, MaxZ, consoleProgress);

            //Try to delete temp directory.
            try
            {
                TempDirectoryInfo.Delete(true);
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to delete temp directory.", exception);
            }

            stopwatch.Stop();
            System.Console.WriteLine("Done!");
            System.Console
                  .WriteLine($"Days:{stopwatch.Elapsed.Days} hours:{stopwatch.Elapsed.Hours} minutes:{stopwatch.Elapsed.Minutes} "
                           + $"seconds:{stopwatch.Elapsed.Seconds} ms:{stopwatch.Elapsed.Milliseconds}");
            #if DEBUG
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
            #endif
        }

        #region Methods

        /// <summary>
        /// Set properties values from console options.
        /// </summary>
        /// <param name="options">Console options.</param>
        private static void ParseConsoleOptions(Options options) =>
            (InputFIleInfo, OutputDirectoryInfo, TempDirectoryInfo, MinZ, MaxZ, ThreadsCount) =
            (new FileInfo(options.InputFilePath), new DirectoryInfo(options.OutputDirectoryPath),
             new DirectoryInfo(options.TempDirectoryPath), options.MinZ,
             options.MaxZ, options.ThreadsCount);

        /// <summary>
        /// Crops tiles.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff.</param>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="tempDirectoryInfo">Temp directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="progress">Progress.</param>
        private static async ValueTask GenerateTiles(FileInfo inputFileInfo, DirectoryInfo outputDirectoryInfo,
                                                     DirectoryInfo tempDirectoryInfo, int minZ, int maxZ,
                                                     IProgress<int> progress)
        {
            try
            {
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, minZ, maxZ);

                //todo progress
                await Task.Factory.StartNew(() => image.GenerateTiles(tempDirectoryInfo),
                                            TaskCreationOptions.LongRunning);
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.PrintException(exception);
            }
        }

        /// <summary>
        /// Crops tiles.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff.</param>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="tempDirectoryInfo">Temp directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        private static async ValueTask GenerateTilesOld(FileInfo inputFileInfo, DirectoryInfo outputDirectoryInfo,
                                                        DirectoryInfo tempDirectoryInfo, int minZ, int maxZ,
                                                        IProgress<int> progress, int threadsCount)
        {
            try
            {
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, minZ, maxZ);

                //todo progress
                await Task.Factory.StartNew(() => image.GenerateTilesOld(tempDirectoryInfo, threadsCount),
                                            TaskCreationOptions.LongRunning);
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.PrintException(exception);
            }
        }

        #endregion
    }
}
