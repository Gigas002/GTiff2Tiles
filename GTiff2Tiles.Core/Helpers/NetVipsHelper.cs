using NetVips;

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Some methods for initializing NetVips.
    /// </summary>
    internal static class NetVipsHelper
    {
        //todo initialize NetVips location.

        /// <summary>
        /// Disables NetVips log warnings.
        /// </summary>
        internal static void DisableLog() => Log.SetLogHandler("VIPS", NetVips.Enums.LogLevelFlags.Warning, (domain, level, message) => { });
    }
}
