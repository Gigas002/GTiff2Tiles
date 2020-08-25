using CommandLine;

// ReSharper disable All

namespace GTiff2Tiles.Benchmarks
{
    /// <summary>
    /// Class for parsing command line arguments
    /// </summary>
    public class Options
    {
        #region Optional

        /// <summary>
        /// Path to input file
        /// </summary>
        [Option('i', "input", Required = false, HelpText = "Path to input file")]
        public string InputFilePath { get; set; }

        #endregion
    }
}
