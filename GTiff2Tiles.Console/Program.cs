#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using CommandLine;
using GTiff2Tiles.Console.Localization;
using GTiff2Tiles.Core;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;

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
        /// Do you want to create tms-compatible tiles?
        /// </summary>
        private static bool TmsCompatible { get; set; }

        /// <summary>
        /// Ready tiles extension.
        /// </summary>
        private static TileExtension TileExtension { get; set; } = TileExtension.Png;

        #endregion

        private static async Task Main(string[] args)
        {
            // TODO: coordinate system option
            var targetCoordinateSystem = CoordinateSystems.Epsg4326;

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                Parser.Default.ParseArguments<Options>(args).WithParsed(ParseConsoleOptions)
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
                                                    DateTime.Now.ToString(DateTimePatterns.LongWithMs));
            TempDirectoryInfo = new DirectoryInfo(tempDirectoryPath);

            //Run tiling asynchroniously.
            try
            {
                //Check for errors.
                CheckHelper.CheckDirectory(OutputDirectoryInfo.FullName, true);

                if (!await CheckHelper.CheckInputFileAsync(InputFileInfo.FullName, targetCoordinateSystem).ConfigureAwait(false))
                {
                    string tempFilePath = Path.Combine(TempDirectoryInfo.FullName,
                                                       $"{GdalWorker.TempFileName}{FileExtensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await GdalWorker.ConvertGeoTiffToTargetSystemAsync(InputFileInfo.FullName, tempFileInfo.FullName, targetCoordinateSystem,
                                                                       consoleProgress).ConfigureAwait(false);
                    InputFileInfo = tempFileInfo;
                }

                //Run tiling.
                await Img.GenerateTilesAsync(InputFileInfo, OutputDirectoryInfo, MinZ, MaxZ, TileType.Raster,
                                             targetCoordinateSystem,
                                             TmsCompatible, TileExtension,
                                             consoleProgress, ThreadsCount).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.PrintException(exception);

                return;
            }

            stopwatch.Stop();
            System.Console.WriteLine(Strings.Done, Environment.NewLine, stopwatch.Elapsed.Days, stopwatch.Elapsed.Hours,
                                     stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds,
                                     stopwatch.Elapsed.Milliseconds);
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

            //Set tile extension. Png by default or unknown input
            TileExtension = options.TileExtension switch
            {
                FileExtensions.Jpg => TileExtension.Jpg,
                FileExtensions.Webp => TileExtension.Webp,
                _ => TileExtension.Png
            };

            //Set properties values.
            InputFileInfo = new FileInfo(options.InputFilePath);
            OutputDirectoryInfo = new DirectoryInfo(options.OutputDirectoryPath);
            TempDirectoryInfo = string.IsNullOrWhiteSpace(options.TempDirectoryPath)
                                    ? new FileInfo(Assembly.GetExecutingAssembly().Location).Directory
                                    : new DirectoryInfo(options.TempDirectoryPath);
            MinZ = options.MinZ;
            MaxZ = options.MaxZ;
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
