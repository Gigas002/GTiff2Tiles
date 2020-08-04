using System;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Coordinates in pixels
    /// </summary>
    public class PixelCoordinate : Coordinate
    {
        #region Constructors

        /// <inheritdoc />
        public PixelCoordinate(double x, double y) : base(x, y) { }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override Number ToNumber(int zoom, int tileSize, bool tmsCompatible)
        {
            int tileX = Convert.ToInt32(Math.Ceiling(X / tileSize) - 1.0);
            int tileY = Convert.ToInt32(Math.Ceiling(Y / tileSize) - 1.0);

            Number result = new Number(tileX, tileY, zoom);
            if (!tmsCompatible) result = result.Flip();

            return result;
        }

        /// <summary>
        /// Convert current coordinate to child of <see cref="GeoCoordinate"/>
        /// </summary>
        /// <param name="coordinateType">Type of coordinates</param>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted to <see cref="GeoCoordinate"/> value
        /// or null if something goes wrong</returns>
        public GeoCoordinate ToGeoCoordinate(CoordinateType coordinateType, int zoom, int tileSize) =>
            coordinateType switch
            {
                CoordinateType.Geodetic => ToGeodeticCoordinate(zoom, tileSize),
                CoordinateType.Mercator => ToMercatorCoordinate(zoom, tileSize),
                _ => null
            };

        /// <summary>
        /// Convert current coordinate to <see cref="GeodeticCoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted <see cref="GeodeticCoordinate"/></returns>
        public GeodeticCoordinate ToGeodeticCoordinate(int zoom, int tileSize)
        {
            MercatorCoordinate mercatorCoordinate = ToMercatorCoordinate(zoom, tileSize);
            return mercatorCoordinate.ToGeodeticCoordinate();
        }

        /// <summary>
        /// Convert current coordinate to <see cref="MercatorCoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted <see cref="MercatorCoordinate"/></returns>
        public MercatorCoordinate ToMercatorCoordinate(int zoom, int tileSize)
        {
            double resolution = MercatorCoordinate.Resolution(zoom, tileSize);
            double mx = X * resolution - Constants.Geodesic.OriginShift;
            double my = Y * resolution - Constants.Geodesic.OriginShift;

            return new MercatorCoordinate(mx, my);
        }

        /// <summary>
        /// Move the origin of pixel coordinates to top-left corner
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted <see cref="PixelCoordinate"/></returns>
        public PixelCoordinate ToRasterPixelCoordinate(int zoom, int tileSize)
        {
            // TODO: Test

            int mapSize = tileSize << zoom;

            return new PixelCoordinate(X, mapSize - Y);
        }

        #endregion
    }
}
