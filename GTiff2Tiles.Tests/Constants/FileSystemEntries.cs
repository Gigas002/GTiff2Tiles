using System;
using System.IO;
using System.Reflection;

namespace GTiff2Tiles.Tests.Constants
{
    internal static class FileSystemEntries
    {
        #region Examples

        internal const string ExamplesDirectoryName = "Examples";

        internal static string ExamplesDirectoryPath
        {
            get
            {
                DirectoryInfo di = new DirectoryInfo(Assembly.GetExecutingAssembly().Location).Parent?.Parent?.Parent?.Parent
                                                                                   ?.Parent?.Parent;

                return Path.Combine(di?.FullName ?? throw new InvalidOperationException(), ExamplesDirectoryName);
            }
        }

        internal static DirectoryInfo ExamplesDirectoryInfo => new DirectoryInfo(ExamplesDirectoryPath);

        #endregion

        #region Input

        internal const string InputDirectoryName = "Input";

        internal static string InputDirectoryPath => Path.Combine(ExamplesDirectoryPath, InputDirectoryName);

        internal static DirectoryInfo InputDirectoryInfo => new DirectoryInfo(InputDirectoryPath);

        #endregion

        #region Input files

        internal const string Input4326FileName = "Input4326.tif";

        internal static string Input4326FilePath => Path.Combine(InputDirectoryPath, Input4326FileName);

        internal static FileInfo Input4326FileInfo => new FileInfo(Input4326FilePath);

        internal const string Input3785FileName = "Input3785.tif";

        internal static string Input3785FilePath => Path.Combine(InputDirectoryPath, Input3785FileName);

        internal static FileInfo Input3785FileInfo => new FileInfo(Input3785FilePath);

        internal const string Input3395FileName = "Input3395.tif";

        internal static string Input3395FilePath => Path.Combine(InputDirectoryPath, Input3395FileName);

        internal static FileInfo Input3395FileInfo => new FileInfo(Input3395FilePath);

        #endregion

        #region Temp

        internal const string TempDirectoryName = "Temp";

        internal static string TempDirectoryPath => Path.Combine(ExamplesDirectoryPath, TempDirectoryName);

        internal static DirectoryInfo TempDirectoryInfo => new DirectoryInfo(TempDirectoryPath);

        #endregion

        #region Output

        internal const string OutputDirectoryName = "Output";

        internal static string OutputDirectoryPath => Path.Combine(ExamplesDirectoryPath, OutputDirectoryName);

        internal static DirectoryInfo OutputDirectoryInfo => new DirectoryInfo(OutputDirectoryPath);

        #endregion
    }
}
