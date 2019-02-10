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

        /// <summary>
        /// Algorithm to create tiles.
        /// </summary>
        private static string Algorithm { get; set; }

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
            ConsoleProgress<double> consoleProgress = new ConsoleProgress<double>(System.Console.WriteLine);

            //Create temp directory object.
            string tempDirectoryPath = Path.Combine(TempDirectoryInfo.FullName,
                                                    DateTime.Now.ToString(Core.Enums.DateTimePatterns.LongWithMs));
            TempDirectoryInfo = new DirectoryInfo(tempDirectoryPath);

            //Run tiling asynchroniously.
            try
            {
                //Check for errors.
                Core.Helpers.CheckHelper.CheckOutputDirectory(OutputDirectoryInfo);
                if (!Core.Helpers.CheckHelper.CheckInputFile(InputFileInfo))
                {
                    string tempFilePath = Path.Combine(TempDirectoryInfo.FullName, $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    Core.Image.Gdal.Warp(InputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    InputFileInfo = tempFileInfo;
                }
                //Create image object.
                Core.Image.Image inputImage = new Core.Image.Image(InputFileInfo, OutputDirectoryInfo, MinZ, MaxZ);

                //Switch on algorithm.
                switch (Algorithm)
                {
                    case Core.Enums.Algorithms.Join:
                        await inputImage.GenerateTilesByJoining(consoleProgress, ThreadsCount);
                        break;
                    case Core.Enums.Algorithms.Crop:
                        await inputImage.GenerateTilesByCropping(consoleProgress, ThreadsCount);
                        break;
                    default:
                        Helpers.ErrorHelper.PrintError("This algorithm is not supported.");
                        return;
                }
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.PrintException(exception);
                return;
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
        private static void ParseConsoleOptions(Options options)
        {
            //Check if string options are empty strings.
            if (string.IsNullOrWhiteSpace(options.InputFilePath))
            {
                Helpers.ErrorHelper.PrintError("-i option is empty.");
                IsParsingErrors = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(options.OutputDirectoryPath))
            {
                Helpers.ErrorHelper.PrintError("-o option is empty.");
                IsParsingErrors = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(options.TempDirectoryPath))
            {
                Helpers.ErrorHelper.PrintError("-t option is empty.");
                IsParsingErrors = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(options.Algorithm))
            {
                Helpers.ErrorHelper.PrintError("-a option is empty.");
                IsParsingErrors = true;
                return;
            }

            //Check zooms.
            if (options.MinZ < 0)
            {
                Helpers.ErrorHelper.PrintError("--minz is lesser, than 0.");
                IsParsingErrors = true;
                return;
            }
            if (options.MaxZ < 0)
            {
                Helpers.ErrorHelper.PrintError("--maxz is lesser, than 0.");
                IsParsingErrors = true;
                return;
            }
            if (options.MinZ > options.MaxZ)
            {
                Helpers.ErrorHelper.PrintError("--minz is bigger, than --maxz.");
                IsParsingErrors = true;
                return;
            }

            //Threads check.
            if (options.ThreadsCount <= 0)
            {
                Helpers.ErrorHelper.PrintError("--threads is lesser, than 0.");
                IsParsingErrors = true;
                return;
            }

            InputFileInfo = new FileInfo(options.InputFilePath);
            OutputDirectoryInfo = new DirectoryInfo(options.OutputDirectoryPath);
            TempDirectoryInfo = new DirectoryInfo(options.TempDirectoryPath);
            MinZ = options.MinZ;
            MaxZ = options.MaxZ;
            Algorithm = options.Algorithm;
            ThreadsCount = options.ThreadsCount;
        }

        #endregion
    }
}
