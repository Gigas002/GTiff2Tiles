#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable CA1308 // Normalize strings to uppercase

using System.Diagnostics;
using System.Globalization;
using CommandLine;
using GTiff2Tiles.Console.Localization;
using GTiff2Tiles.Core;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.TileMapResource;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Console;

internal static class Program
{
    #region Properties

    /// <summary>
    /// Input file path
    /// </summary>
    private static string InputFilePath { get; set; }

    /// <summary>
    /// Output directory path
    /// </summary>
    private static string OutputDirectoryPath { get; set; }

    /// <summary>
    /// Temp directory path
    /// </summary>
    private static string TempDirectoryPath { get; set; }

    /// <summary>
    /// Minimum cropped zoom
    /// </summary>
    private static int MinZ { get; set; }

    /// <summary>
    /// Maximum cropped zoom
    /// </summary>
    private static int MaxZ { get; set; }

    /// <summary>
    /// Shows if there were command line parsing errors
    /// </summary>
    private static bool IsParsingErrors { get; set; }

    /// <summary>
    /// Threads count
    /// </summary>
    private static int ThreadsCount { get; set; }

    /// <summary>
    /// Do you want to create tms-compatible tiles?
    /// </summary>
    private static bool TmsCompatible { get; set; }

    /// <summary>
    /// Ready tiles extension
    /// </summary>
    private static TileExtension TileExtension { get; set; } = TileExtension.Png;

    /// <summary>
    /// Coordinate system of ready tiles
    /// </summary>
    private static CoordinateSystem TargetCoordinateSystem { get; set; } = CoordinateSystem.Epsg4326;

    /// <summary>
    /// Interpolation of ready tiles
    /// </summary>
    private static NetVips.Enums.Kernel TargetInterpolation { get; set; } = NetVips.Enums.Kernel.Lanczos3;

    /// <summary>
    /// Count of bands in ready tiles
    /// </summary>
    private static int BandsCount { get; set; } = 4;

    /// <summary>
    /// How much tiles would you like to store in memory cache?
    /// </summary>
    private static int TileCacheCount { get; set; } = 1000;

    /// <summary>
    /// Maximum size of input files to store in RAM
    /// </summary>
    private static long MemCache { get; set; } = 2147483648;

    /// <summary>
    /// Do you want to see the progress?
    /// </summary>
    private static bool IsProgress { get; set; } = true;

    /// <summary>
    /// Do you want to see estimated time left?
    /// </summary>
    private static bool IsTime { get; set; }

    /// <summary>
    /// Size of ready tiles
    /// </summary>
    private static Size TileSize { get; set; } = Tile.DefaultSize;

    /// <summary>
    /// Do you want to create tilemapresource.xml?
    /// </summary>
    private static bool IsTmr { get; set; }

    /// <summary>
    /// tilemapresource.xml
    /// </summary>
    private const string TmrName = "tilemapresource.xml";

    /// <summary>
    /// Property for actual progress reporing
    /// </summary>
    private static int ProgressValue { get; set; }

    #endregion

    private static async Task Main(string[] args)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(ParseConsoleOptions)
                  .WithNotParsed(_ => IsParsingErrors = true);
        }
        catch (Exception exception)
        {
            // Catch some uncaught parsing errors
            Helpers.ErrorHelper.PrintException(exception);

            return;
        }

        if (IsParsingErrors)
        {
            Helpers.ErrorHelper.PrintError(Strings.ParsingError);

            return;
        }

        // Create progress-reporter
        IProgress<double> consoleProgress = IsProgress ? new Progress<double>(ProgressReporter) : null;
        Action<string> printTimeAction = IsTime ? new Action<string>(System.Console.WriteLine) : null;

        // Create temp directory object
        TempDirectoryPath = Path.Combine(TempDirectoryPath,
                                         DateTime.Now.ToString(DateTimePatterns.LongWithMs, CultureInfo.InvariantCulture));

        // Run tiling asynchroniously
        try
        {
            // Check for errors
            if (!await CheckHelper.CheckInputFileAsync(InputFilePath, TargetCoordinateSystem).ConfigureAwait(false))
            {
                string tempFilePath = Path.Combine(TempDirectoryPath, GdalWorker.TempFileName);

                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(InputFilePath, tempFilePath, TargetCoordinateSystem,
                                                                   consoleProgress).ConfigureAwait(false);
                InputFilePath = tempFilePath;
            }

            using Raster image = new(InputFilePath, TargetCoordinateSystem, MemCache);

            // Generate tiles
            await image.WriteTilesToDirectoryAsync(OutputDirectoryPath, MinZ, MaxZ, TmsCompatible,
                                                   TileSize, TileExtension, TargetInterpolation, BandsCount,
                                                   TileCacheCount, ThreadsCount, consoleProgress, printTimeAction)
                       .ConfigureAwait(false);

            // Generate tilemapresource if needed
            if (IsTmr)
            {
                IEnumerable<TileSet> tileSets = TileSets.GenerateTileSetCollection(MinZ, MaxZ, TileSize, TargetCoordinateSystem);
                TileMap tileMap = new(image.MinCoordinate, image.MaxCoordinate, TileSize, TileExtension, tileSets,
                                      TargetCoordinateSystem);

                string xmlPath = $"{OutputDirectoryPath}/{TmrName}";
                using FileStream fs = File.OpenWrite(xmlPath);
                tileMap.Serialize(fs);
            }
        }
        catch (Exception exception)
        {
            Helpers.ErrorHelper.PrintException(exception);

            return;
        }

        System.Console.WriteLine(Strings.Done, Environment.NewLine, stopwatch.Elapsed.Days, stopwatch.Elapsed.Hours,
                                 stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds,
                                 stopwatch.Elapsed.Milliseconds);
    }

    #region Methods

    /// <summary>
    /// Report int progress
    /// </summary>
    /// <param name="progressValue">Progress's double value</param>
    private static void ProgressReporter(double progressValue)
    {
        int progressToInt = (int)Math.Round(progressValue);

        if (progressToInt.Equals(ProgressValue)) return;

        ProgressValue = progressToInt;
        System.Console.WriteLine(ProgressValue);
    }

    /// <summary>
    /// Set properties values from command line options
    /// </summary>
    /// <param name="options">Command line options</param>
    private static void ParseConsoleOptions(Options options)
    {
        // Check options and set properties

        #region Required

        InputFilePath = options.InputFilePath;
        OutputDirectoryPath = options.OutputDirectoryPath;
        MinZ = options.MinZ;
        MaxZ = options.MaxZ;

        #endregion

        ThreadsCount = options.ThreadsCount;

        TileExtension = options.TileExtension.ToLowerInvariant() switch
        {
            FileExtensions.Jpg => TileExtension.Jpg,
            FileExtensions.Webp => TileExtension.Webp,
            _ => TileExtension.Png
        };

        TempDirectoryPath = string.IsNullOrWhiteSpace(options.TempDirectoryPath)
                                ? AppContext.BaseDirectory
                                : options.TempDirectoryPath;
        TmsCompatible = bool.Parse(options.TmsCompatible);

        TargetCoordinateSystem = options.CoordinateSystem.ToLowerInvariant() switch
        {
            "mercator" => CoordinateSystem.Epsg3857,
            _ => CoordinateSystem.Epsg4326
        };

        TargetInterpolation = Enum.Parse<NetVips.Enums.Kernel>(options.Interpolation, true);

        BandsCount = options.BandsCount;
        TileCacheCount = options.TileCacheCount;
        MemCache = options.MemCache;
        IsProgress = bool.Parse(options.IsProgress);
        IsTime = bool.Parse(options.IsTime);
        TileSize = new Size(options.TileSize, options.TileSize);
        IsTmr = bool.Parse(options.IsTmr);
    }

    #endregion
}
#pragma warning restore CA1308 // Normalize strings to uppercase
