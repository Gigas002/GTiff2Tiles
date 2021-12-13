// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Constants;

/// <summary>
/// Proj constants
/// </summary>
public static class Proj
{
    /// <summary>
    /// Full PROJ string for Spherical Mercator (EPSG:3857) projection
    /// </summary>
    public static readonly string MercFull = $"{ProjMerc} +a=6378137 +b=6378137 +lat_ts=0.0 +lon_0=0.0 "
                                           + $"+x_0=0.0 +y_0=0 +k=1.0 +units=m +nadgrids=@null +wktext {NoDefs}";

    /// <summary>
    /// Full PROJ string for LongLat (EPSG:4326) projection
    /// </summary>
    public static readonly string LongLatFull = $"{ProjLongLat} {DatumWgs84} {NoDefs}";

    /// <summary>
    /// +no_defs
    /// </summary>
    public const string NoDefs = "+no_defs";

    /// <summary>
    /// +proj=merc
    /// </summary>
    public const string ProjMerc = "+proj=merc";

    /// <summary>
    /// +proj=longlat
    /// </summary>
    public const string ProjLongLat = "+proj=longlat";

    /// <summary>
    /// For EPSG:4326 AND World Mercator, BUT not Spherical Mercator (EPSG:3857)
    /// </summary>
    public const string DatumWgs84 = "+datum=WGS84";
}