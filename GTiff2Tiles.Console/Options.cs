using CommandLine;

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
        /// Full path to temp directory.
        /// </summary>
        [Option('t', "temp", Required = true, HelpText = "Full path to temp directory.")]
        public string TempDirectoryPath { get; set; }

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

        /// <summary>
        /// Algorithm to create tiles. Can be "join" or "crop".
        /// </summary>
        [Option('a', "algorithm", Required = true, HelpText = "Algorithm to create tiles. Can be \"join\" or \"crop\".")]
        public string Algorithm { get; set; }

        #endregion

        #region Optional

        /// <summary>
        /// Threads count.
        /// </summary>
        [Option("threads", Required = false, HelpText = "Threads count.")]
        public int ThreadsCount { get; set; } = 1;

        #endregion
    }
}
