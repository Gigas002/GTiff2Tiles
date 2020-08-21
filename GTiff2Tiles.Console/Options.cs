using CommandLine;
using GTiff2Tiles.Core.Constants;

// ReSharper disable All

namespace GTiff2Tiles.Console
{
    /// <summary>
    /// Class for parsing command line arguments
    /// </summary>
    public class Options
    {
        #region Required

        /// <summary>
        /// Path to input file
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "Path to input file")]
        public string InputFilePath { get; set; }

        /// <summary>
        /// Path to output directory
        /// </summary>
        [Option('o', "output", Required = true, HelpText = "Path to output directory")]
        public string OutputDirectoryPath { get; set; }

        /// <summary>
        /// Minimum cropped zoom
        /// </summary>
        [Option("minz", Required = true, HelpText = "Minimum cropped zoom")]
        public int MinZ { get; set; }

        /// <summary>
        /// Maximum cropped zoom
        /// </summary>
        [Option("maxz", Required = true, HelpText = "Maximum cropped zoom")]
        public int MaxZ { get; set; }

        #endregion

        #region Optional

        /// <summary>
        /// Threads count
        /// <remarks><para/>Auto by default</remarks>
        /// </summary>
        [Option("threads", Required = false, HelpText = "Threads count, auto (<=0) by default")]
        public int ThreadsCount { get; set; } = 0;

        /// <summary>
        /// Extension of ready tiles
        /// <remarks><para/>.png by default</remarks>
        /// </summary>
        [Option("extension", Required = false, HelpText = "Extension of ready tiles; default is .png; supported extensions: .webp, .jpg, .png")]
        public string TileExtension { get; set; } = FileExtensions.Png;

        /// <summary>
        /// Path to temp directory
        /// <remarks><para/>Current directory by default</remarks>
        /// </summary>
        [Option('t', "temp", Required = false, HelpText = "Path to temp directory; current directory by default")]
        public string TempDirectoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Do you want to create tms-compatible tiles?
        /// <remarks><para/>True by default</remarks>
        /// </summary>
        [Option("tms", Required = false, HelpText = "Do you want to create tms-compatible tiles? True by default")]
        public string TmsCompatible { get; set; } = "true";

        /// <summary>
        /// Target tiles coordinate system
        /// <remarks><para/>geodetic by default</remarks>
        /// </summary>
        [Option('c', "coordinates", Required = false, HelpText = "Target tiles coordinate system; geodetic (4326) by default; supported values: geodetic, mercator")]
        public string CoordinateSystem { get; set; } = "geodetic";

        /// <summary>
        /// Interpolation of ready tiles
        /// <remarks><para/>lanczos3 by default</remarks>
        /// </summary>
        [Option("interpolation", Required = false, HelpText = "Interpolation of ready tiles; lanczos3 by default")]
        public string Interpolation { get; set; } = "lanczos3";

        /// <summary>
        /// Count of bands in ready tiles
        /// <remarks><para/>4 by default</remarks>
        /// </summary>
        [Option('b', "bands", Required = false, HelpText = "Count of bands in ready tiles; 4 by default")]
        public int BandsCount { get; set; } = 4;

        /// <summary>
        /// How much tiles would you like to store in memory cache?
        /// <remarks><para/>1000 by default</remarks>
        /// </summary>
        [Option("tilecache", Required = false, HelpText = "How much tiles would you like to store in memory cache? 1000 by default")]
        public int TileCacheCount { get; set; } = 1000;

        /// <summary>
        /// Maximum size of input files to store in RAM
        /// <remarks><para/>2Gb by default</remarks>
        /// </summary>
        [Option('m', "memcache", Required = false, HelpText = "Maximum size of input files to store in RAM; 2Gb by default")]
        public long MemCache { get; set; } = 2147483648;

        /// <summary>
        /// Do you want to see the progress?
        /// <remarks><para/>True by default</remarks>
        /// </summary>
        [Option("progress", Required = false, HelpText = "Do you want to see the progress? True by default")]
        public string IsProgress { get; set; } = "true";

        /// <summary>
        /// Do you want to see estimated time left?
        /// <remarks><para/>False by default</remarks>
        /// </summary>
        [Option("timeleft", Required = false, HelpText = "Do you want to see estimated time left? False by default")]
        public string IsTime { get; set; } = "false";

        /// <summary>
        /// Ready tile's size
        /// <remarks><para/>256x256 by default</remarks>
        /// </summary>
        [Option("tilesize", Required = false, HelpText = "Ready tile's size; 256x256 by default")]
        public int TileSize { get; set; } = 256;

        #endregion
    }
}
