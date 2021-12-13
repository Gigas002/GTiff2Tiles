using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace GTiff2Tiles.Core.GeoTiffs;

/// <summary>
/// Main interface for different type of GeoTiffs and <see cref="ITile"/>
/// </summary>
public interface IGeoTiff : IAsyncDisposable, IDisposable
{
    #region Properties

    /// <summary>
    /// Shows if resources have already been disposed
    /// </summary>
    public bool IsDisposed { get; }

    /// <summary>
    /// Image's <see cref="Images.Size"/> (width and height)
    /// </summary>
    public Size Size { get; }

    /// <summary>
    /// Minimal <see cref="GeoCoordinate"/> of this <see cref="IGeoTiff"/>
    /// </summary>
    public GeoCoordinate MinCoordinate { get; }

    /// <summary>
    /// Maximal <see cref="GeoCoordinate"/> of this <see cref="IGeoTiff"/>
    /// </summary>
    public GeoCoordinate MaxCoordinate { get; }

    /// <summary>
    /// Type of desired <see cref="CoordinateSystem"/>
    /// </summary>
    public CoordinateSystem GeoCoordinateSystem { get; }

    #endregion
}
