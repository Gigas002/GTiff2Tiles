using System.Reflection;

namespace GTiff2Tiles.Tests.Constants;

internal static class FileSystemEntries
{
    #region Examples

    private const string ExamplesDirectoryName = "Examples";

    private static string ExamplesDirectoryPath
    {
        get
        {
            DirectoryInfo di = new DirectoryInfo(Assembly.GetExecutingAssembly().Location)
                              .Parent?.Parent?.Parent?.Parent?.Parent?.Parent;

            return Path.Combine(di?.FullName ?? throw new InvalidOperationException(), ExamplesDirectoryName);
        }
    }

    #endregion

    #region Input

    private const string InputDirectoryName = "Input";

    private static string InputDirectoryPath => Path.Combine(ExamplesDirectoryPath, InputDirectoryName);

    #endregion

    #region Input files

    private const string Input4326FileName = "Input4326.tif";

    internal static string Input4326FilePath => Path.Combine(InputDirectoryPath, Input4326FileName);

    private const string Input3785FileName = "Input3785.tif";

    internal static string Input3785FilePath => Path.Combine(InputDirectoryPath, Input3785FileName);

    private const string Input3395FileName = "Input3395.tif";

    internal static string Input3395FilePath => Path.Combine(InputDirectoryPath, Input3395FileName);

    private const string TileMapResourceXmlName = "tilemapresource.xml";

    internal static string TileMapResourceXmlPath => Path.Combine(InputDirectoryPath, TileMapResourceXmlName);

    #endregion

    #region Output

    private const string OutputDirectoryName = "Output";

    internal static string OutputDirectoryPath => Path.Combine(ExamplesDirectoryPath, OutputDirectoryName);

    internal static DirectoryInfo OutputDirectoryInfo => new(OutputDirectoryPath);

    #endregion
}