using NetVips;

namespace GTiff2Tiles.Core.Helpers;

/// <summary>
/// Some additional methods for NetVips
/// </summary>
public static class NetVipsHelper
{
    /// <summary>
    /// Disables NetVips log warnings
    /// </summary>
    public static void DisableLog() => Log.SetLogHandler("VIPS", NetVips.Enums.LogLevelFlags.Warning, null);
}