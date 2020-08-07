using System;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Coordinates
{
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

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException"/>
        public override Number ToNumber(int z, int tileSize, bool tmsCompatible)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            int tileX = Convert.ToInt32(Math.Ceiling(X / tileSize) - 1.0);
            int tileY = Convert.ToInt32(Math.Ceiling(Y / tileSize) - 1.0);

            Number result = new Number(tileX, tileY, z);
            if (!tmsCompatible) result = result.Flip();

            return result;
        }

        /// <summary>
        /// Convert current coordinate to child of <see cref="GeoCoordinate"/>
        /// </summary>
        /// <param name="coordinateSystem">Coordinate system</param>
        /// <param name="z">Zoom
        /// <remarks><para/>Must be >= 0</remarks></param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted to <see cref="GeoCoordinate"/> value
        /// or <see langword="null"/> if something goes wrong</returns>
        public GeoCoordinate ToGeoCoordinate(CoordinateSystem coordinateSystem, int z, int tileSize) =>
            coordinateSystem switch
            {
                CoordinateSystem.Epsg4326 => ToGeodeticCoordinate(z, tileSize),
                CoordinateSystem.Epsg3857 => ToMercatorCoordinate(z, tileSize),
                _ => null
                //todo throw notsupported
            };

        /// <summary>
        /// Convert current coordinate to <see cref="GeodeticCoordinate"/>
        /// </summary>
        /// <param name="z">Zoom
        /// <remarks><para/>Must be >= 0</remarks></param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted <see cref="GeodeticCoordinate"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public GeodeticCoordinate ToGeodeticCoordinate(int z, int tileSize)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            MercatorCoordinate mercatorCoordinate = ToMercatorCoordinate(z, tileSize);

            return mercatorCoordinate.ToGeodeticCoordinate();
        }

        /// <summary>
        /// Convert current coordinate to <see cref="MercatorCoordinate"/>
        /// </summary>
        /// <param name="z">Zoom
        /// <remarks><para/>Must be >= 0</remarks></param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted <see cref="MercatorCoordinate"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public MercatorCoordinate ToMercatorCoordinate(int z, int tileSize)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            double resolution = MercatorCoordinate.Resolution(z, tileSize);
            double mx = X * resolution - Constants.Geodesic.OriginShift;
            double my = Y * resolution - Constants.Geodesic.OriginShift;

            return new MercatorCoordinate(mx, my);
        }

        /// <summary>
        /// Move the origin of pixel coordinates to top-left corner
        /// </summary>
        /// <param name="z">Zoom
        /// <remarks><para/>Must be >= 0</remarks></param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted <see cref="PixelCoordinate"/></returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public PixelCoordinate ToRasterPixelCoordinate(int z, int tileSize)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            // TODO: Test

            int mapSize = tileSize << z;

            return new PixelCoordinate(X, mapSize - Y);
        }

        #endregion
    }
}
