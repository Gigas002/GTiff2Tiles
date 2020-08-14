using NetVips;

namespace GTiff2Tiles.Core.Helpers
{
    // TODO public

    /// <summary>
    /// Some additional methods for NetVips
    /// </summary>
    internal static class NetVipsHelper
    {
        /// <summary>
        /// Disables NetVips log warnings
        /// </summary>
        internal static void DisableLog() => Log.SetLogHandler("VIPS", NetVips.Enums.LogLevelFlags.Warning, null);
    }
}
