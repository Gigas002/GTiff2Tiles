using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using GTiff2Tiles.Console.Localization;

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

        /// <summary>
        /// Do you want to create tms-compatible tiles?
        /// </summary>
        private static bool TmsCompatible { get; set; }

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
                Helpers.ErrorHelper.PrintError(Strings.ParsingError);
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
                Core.Helpers.CheckHelper.CheckDirectory(OutputDirectoryInfo, true);
                if (!Core.Helpers.CheckHelper.CheckInputFile(InputFileInfo))
                {
                    string tempFilePath = Path.Combine(TempDirectoryInfo.FullName, $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.Warp(InputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    InputFileInfo = tempFileInfo;
                }
                //Create image object.
                Core.Image.Image inputImage = new Core.Image.Image(InputFileInfo);

                //Switch on algorithm.
                switch (Algorithm)
                {
                    case Core.Enums.Algorithms.Join:
                        await inputImage.GenerateTilesByJoining(OutputDirectoryInfo, MinZ, MaxZ, TmsCompatible, consoleProgress, ThreadsCount);
                        break;
                    case Core.Enums.Algorithms.Crop:
                        await inputImage.GenerateTilesByCropping(OutputDirectoryInfo, MinZ, MaxZ, TmsCompatible, consoleProgress, ThreadsCount);
                        break;
                    default:
                        Helpers.ErrorHelper.PrintError(Strings.AlgorithmNotSupported);
                        return;
                }
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.PrintException(exception);
                return;
            }

            stopwatch.Stop();
            System.Console.WriteLine(Strings.Done, Environment.NewLine, stopwatch.Elapsed.Days,
                                     stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes,
                                     stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds);
            #if DEBUG
            System.Console.WriteLine(Strings.PressAnyKey);
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
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "-i/--input"));
                IsParsingErrors = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(options.OutputDirectoryPath))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "-o/--output"));
                IsParsingErrors = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(options.TempDirectoryPath))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "-t/--temp"));
                IsParsingErrors = true;
                return;
            }
            if (string.IsNullOrWhiteSpace(options.Algorithm))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "-a/--algorithm"));
                IsParsingErrors = true;
                return;
            }

            //Check zooms.
            if (options.MinZ < 0)
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.LesserThan, "--minz", 0));
                IsParsingErrors = true;
                return;
            }
            if (options.MaxZ < 0)
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.LesserThan, "--maxz", 0));
                IsParsingErrors = true;
                return;
            }
            if (options.MinZ > options.MaxZ)
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.LesserThan, "--maxz", "--minz"));
                IsParsingErrors = true;
                return;
            }

            //Threads check.
            if (options.ThreadsCount <= 0)
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.LesserOrEqual, "--threads", 0));
                IsParsingErrors = true;
                return;
            }

            //Set properties values.
            InputFileInfo = new FileInfo(options.InputFilePath);
            OutputDirectoryInfo = new DirectoryInfo(options.OutputDirectoryPath);
            TempDirectoryInfo = new DirectoryInfo(options.TempDirectoryPath);
            MinZ = options.MinZ;
            MaxZ = options.MaxZ;
            Algorithm = options.Algorithm;
            ThreadsCount = options.ThreadsCount;
            if (!bool.TryParse(options.TmsCompatible, out bool tmsComptaible))
            {
                Helpers.ErrorHelper.PrintError(string.Format(Strings.OptionIsEmpty, "--tms"));
                IsParsingErrors = true;
                return;
            }
            TmsCompatible = tmsComptaible;
        }

        #endregion
    }
}
