using System.Reflection;

namespace GTiff2Tiles.GUI.Enums
{
    /// <summary>
    /// Some const values for <see cref="GTiff2Tiles.GUI.ViewModels.MainViewModel"/> class.
    /// </summary>
    internal static class MainViewModel
    {
        #region UI

        /// <summary>
        /// Copyright string.
        /// </summary>
        internal const string Copyright = "© Gigas002 2019";

        /// <summary>
        /// Info about current version. Pattern: {MAJOR}.{MINOR}.{PATCH}.{BUILD}
        /// </summary>
        internal static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        /// <summary>
        /// Identifier of DialogHost on <see cref="GTiff2Tiles.GUI.Views.MainView"/>.
        /// </summary>
        internal const string DialogHostId = "DialogHost";

        #endregion
    }
}
