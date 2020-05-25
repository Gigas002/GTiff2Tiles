using CommandLine;
using GTiff2Tiles.Core.Constants;

// ReSharper disable All

namespace GTiff2Tiles.Console
{
    /// <summary>
    /// Class for parsing console arguments.
    /// </summary>
    public class Options
    {
        #region Required

        /// <summary>
        /// Full path to input file.
        /// </summary>
        [Option('i', "input", Required = true, HelpText = "Full path to input file.")]
        public string InputFilePath { get; set; }

        /// <summary>
        /// Full path to output directory.
        /// </summary>
        [Option('o', "output", Required = true, HelpText = "Full path to output directory.")]
        public string OutputDirectoryPath { get; set; }

        /// <summary>
        /// Minimum cropped zoom.
        /// </summary>
        [Option("minz", Required = true, HelpText = "Minimum cropped zoom.")]
        public int MinZ { get; set; }

        /// <summary>
        /// Maximum cropped zoom.
        /// </summary>
        [Option("maxz", Required = true, HelpText = "Maximum cropped zoom.")]
        public int MaxZ { get; set; }

        #endregion

        #region Optional

        /// <summary>
        /// Threads count. 5 by default.
        /// </summary>
        [Option("threads", Required = false, HelpText = "Threads count.")]
        public int ThreadsCount { get; set; } = 5;

        /// <summary>
        /// Extension of ready tiles. Png by default.
        /// </summary>
        [Option("extension", Required = false, HelpText = "Extension of ready tiles. Default is .png. Supported extensions: .webp, .jpg, .png.")]
        public string TileExtension { get; set; } = Extensions.Png;

        /// <summary>
        /// Full path to temp directory. Empty string by default.
        /// </summary>
        [Option('t', "temp", Required = false, HelpText = "Full path to temp directory.")]
        public string TempDirectoryPath { get; set; } = string.Empty;

        /// <summary>
        /// Do you want to create tms-compatible tiles? True by default.
        /// </summary>
        [Option("tms", Required = false, HelpText = "Do you want to create tms-compatible tiles?")]
        public string TmsCompatible { get; set; } = "true";

        #endregion
    }
}
