using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable UnusedMemberInSuper.Global

namespace GTiff2Tiles.Core.Coordinates;

/// <summary>
/// Interface for any coordinate
/// </summary>
public interface ICoordinate : IEquatable<ICoordinate>
{
    #region Properties

    /// <summary>
    /// X coordinate value or Longitude
    /// </summary>
    public double X { get; }

    /// <summary>
    /// Y coordinate value or Latitude
    /// </summary>
    public double Y { get; }

    #endregion

    #region Methods

    /// <summary>
    /// Calculate <see cref="Number"/> for current <see cref="ICoordinate"/>
    /// </summary>
    /// <param name="z">Zoom
    /// <remarks><para/>Must be >= 0</remarks></param>
    /// <param name="tileSize"><see cref="ITile"/>'s size</param>
    /// <param name="tmsCompatible">Is <see cref="ITile"/> tms compatible?</param>
    /// <returns><see cref="Number"/> in which this <see cref="ICoordinate"/> belongs</returns>
    public Number ToNumber(int z, Size tileSize, bool tmsCompatible);

    /// <summary>
    /// Round coordinate's <see cref="X"/> and <see cref="Y"/>
    /// </summary>
    /// <typeparam name="T">Child of <see cref="ICoordinate"/></typeparam>
    /// <param name="digits">Number of digits after zero in return falue
    /// <remarks><para/>Must be bigger or equal, than 0</remarks></param>
    /// <returns>Rounded coordinate</returns>
    public T Round<T>(int digits) where T : ICoordinate;

    #endregion
}