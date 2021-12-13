using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Coordinates;

/// <summary>
/// Coordinates in pixels
/// </summary>
public class PixelCoordinate : Coordinate
{
    #region Constructors

    /// <param name="x">X coordinate value
    /// <remarks><para/>Must be >= 0</remarks></param>
    /// <param name="y">Y coordinate value
    /// <remarks><para/>Must be >= 0</remarks></param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <inheritdoc />
    public PixelCoordinate(double x, double y) : base(x, y)
    {
        if (x < 0) throw new ArgumentOutOfRangeException(nameof(x));
        if (y < 0) throw new ArgumentOutOfRangeException(nameof(y));
    }

    #endregion

    #region Methods

    /// <param name="tileSize"><see cref="ITile"/>'s size
    /// <remarks><para/>Must be square</remarks></param>
    /// <inheritdoc />
    /// <param name="z"></param>
    /// <param name="tmsCompatible"></param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    public override Number ToNumber(int z, Size tileSize, bool tmsCompatible)
    {
        #region Preconditions checks

        if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));
        if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));
        if (!tileSize.IsSquare) throw new ArgumentException(Strings.NotSqare);

        #endregion

        int tileX = X == 0 ? 0 : Convert.ToInt32(Math.Ceiling(X / tileSize.Width) - 1.0);
        int tileY = Y == 0 ? 0 : Convert.ToInt32(Math.Ceiling(Y / tileSize.Height) - 1.0);

        Number result = new(tileX, tileY, z);
        if (!tmsCompatible) result = result.Flip();

        return result;
    }

    /// <summary>
    /// Convert current coordinate to child of <see cref="GeoCoordinate"/>
    /// </summary>
    /// <param name="inputCoordinateSystem"><see cref="CoordinateSystem"/> from which pixel coordinates were maid</param>
    /// <param name="targetCoordinateSystem">Coordinate system</param>
    /// <param name="z">Zoom
    /// <remarks><para/>Must be >= 0</remarks></param>
    /// <param name="tileSize"><see cref="ITile"/>'s size
    /// <remarks><para/>Must be square</remarks></param>
    /// <returns>Converted to <see cref="GeoCoordinate"/> value
    /// or <see langword="null"/> if something goes wrong</returns>
    /// <exception cref="NotSupportedException"/>
    public GeoCoordinate ToGeoCoordinate(CoordinateSystem inputCoordinateSystem,
                                         CoordinateSystem targetCoordinateSystem, int z, Size tileSize) =>
        targetCoordinateSystem switch
        {
            CoordinateSystem.Epsg4326 => ToGeodeticCoordinate(inputCoordinateSystem, z, tileSize),
            CoordinateSystem.Epsg3857 => ToMercatorCoordinate(inputCoordinateSystem, z, tileSize),
            _ => throw new NotSupportedException(string.Format(Strings.Culture, Strings.NotSupported, targetCoordinateSystem))
        };

    /// <summary>
    /// Convert current coordinate to <see cref="GeodeticCoordinate"/>
    /// </summary>
    /// <param name="inputCoordinateSystem"><see cref="CoordinateSystem"/> from which pixel coordinates were maid</param>
    /// <param name="z">Zoom
    /// <remarks><para/>Must be >= 0</remarks></param>
    /// <param name="tileSize"><see cref="ITile"/>'s size
    /// <remarks><para/>Must be square</remarks></param>
    /// <returns>Converted <see cref="GeodeticCoordinate"/></returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="NotSupportedException"/>
    public GeodeticCoordinate ToGeodeticCoordinate(CoordinateSystem inputCoordinateSystem, int z, Size tileSize)
    {
        #region Preconditions checks

        if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

        #endregion

        switch (inputCoordinateSystem)
        {
            case CoordinateSystem.Epsg4326:
            {
                double resolution = GeodeticCoordinate.Resolution(z, tileSize);

                double x = X * resolution - GeodeticCoordinate.MaxPossibleLonValue;
                double y = Y * resolution - GeodeticCoordinate.MaxPossibleLatValue;

                return new GeodeticCoordinate(x, y);
            }
            case CoordinateSystem.Epsg3857:
            {
                MercatorCoordinate mercatorCoordinate = ToMercatorCoordinate(inputCoordinateSystem, z, tileSize);

                return mercatorCoordinate.ToGeodeticCoordinate();
            }
            default:
            {
                throw new NotSupportedException(string.Format(Strings.Culture, Strings.NotSupported,
                                                              inputCoordinateSystem));
            }
        }
    }

    /// <summary>
    /// Convert current coordinate to <see cref="MercatorCoordinate"/>
    /// </summary>
    /// <param name="inputCoordinateSystem"><see cref="CoordinateSystem"/> from which pixel coordinates were maid</param>
    /// <param name="z">Zoom
    /// <remarks><para/>Must be >= 0</remarks></param>
    /// <param name="tileSize"><see cref="ITile"/>'s size
    /// <remarks><para/>Must be square</remarks></param>
    /// <returns>Converted <see cref="MercatorCoordinate"/></returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="NotSupportedException"/>
    public MercatorCoordinate ToMercatorCoordinate(CoordinateSystem inputCoordinateSystem, int z, Size tileSize)
    {
        #region Preconditions checks

        if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

        #endregion

        switch (inputCoordinateSystem)
        {
            case CoordinateSystem.Epsg3857:
            {
                double resolution = MercatorCoordinate.Resolution(z, tileSize);
                double mx = X * resolution - Constants.Geodesic.OriginShift;
                double my = Y * resolution - Constants.Geodesic.OriginShift;

                return new MercatorCoordinate(mx, my);
            }
            case CoordinateSystem.Epsg4326:
            {
                GeodeticCoordinate geodeticCoordinate = ToGeodeticCoordinate(inputCoordinateSystem, z, tileSize);

                return geodeticCoordinate.ToMercatorCoordinate();
            }
            default:
            {
                throw new NotSupportedException(string.Format(Strings.Culture, Strings.NotSupported,
                                                              inputCoordinateSystem));
            }
        }
    }

    /// <summary>
    /// Move the origin of pixel coordinates to top-left corner
    /// </summary>
    /// <param name="z">Zoom
    /// <remarks><para/>Must be >= 0</remarks></param>
    /// <param name="tileSize"><see cref="ITile"/>'s size
    /// <remarks><para/>Must be square</remarks></param>
    /// <returns>Converted <see cref="PixelCoordinate"/></returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    public PixelCoordinate ToRasterPixelCoordinate(int z, Size tileSize)
    {
        #region Preconditions checks

        if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));
        if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));
        if (!tileSize.IsSquare) throw new ArgumentException(Strings.NotSqare);

        #endregion

        // If it's square -- it's safe to choose any side
        int mapSize = tileSize.Height << z;

        return new PixelCoordinate(X, mapSize - Y);
    }

    #endregion
}