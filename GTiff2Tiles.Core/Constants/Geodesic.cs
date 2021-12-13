// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Constants;

/// <summary>
/// Some geo-related constants
/// </summary>
public static class Geodesic
{
    /// <summary>
    /// Radius of Earth, measured at equator
    /// </summary>
    public const double EquatorRadius = 6378137.0;

    /// <summary>
    /// Redius of Earth, measured at pole
    /// </summary>
    public const double PolarRadius = 6356752.314245;

    /// <summary>
    /// Approximately 20037508.342789244
    /// </summary>
    public const double OriginShift = Math.PI * EquatorRadius;
}