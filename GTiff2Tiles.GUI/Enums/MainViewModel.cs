using System.Reflection;

namespace GTiff2Tiles.GUI.Enums
{
    public static class MainViewModel
    {
        public const string Copyright = "© Gigas002 2019";

        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
