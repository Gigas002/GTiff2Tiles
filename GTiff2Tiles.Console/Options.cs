using CommandLine;

namespace GTiff2Tiles.Console
{
    /// <summary>
    /// Class for parsing console arguments.
    /// </summary>
    public class Options
    {
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
        /// Full path to output directory.
        /// </summary>
        [Option("minz", Required = true, HelpText = "Minimum cropped zoom.")]
        public int MinZ { get; set; }

        /// <summary>
        /// Full path to output directory.
        /// </summary>
        [Option("maxz", Required = true, HelpText = "Maximum cropped zoom.")]
        public int MaxZ { get; set; }
    }
}
