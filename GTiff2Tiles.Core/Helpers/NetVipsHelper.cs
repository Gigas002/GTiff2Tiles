using NetVips;

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Some additional methods for NetVips.
    /// </summary>
    internal static class NetVipsHelper
    {
        /// <summary>
        /// Disables NetVips log warnings.
        /// </summary>
        internal static void DisableLog() =>
            Log.SetLogHandler("VIPS", NetVips.Enums.LogLevelFlags.Warning, (domain, level, message) => { });
    }
}
