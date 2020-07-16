using System;

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Class for EPSG:3857 coordinates
    /// </summary>
    public class MercatorCoordinate : GeoCoordinate
    {
        #region Constructors

        /// <inheritdoc />
        /// <param name="longitude"><see cref="Coordinate.X"/> or Longitude</param>
        /// <param name="latitude"><see cref="Coordinate.Y"/> or Latitude</param>
        public MercatorCoordinate(double longitude, double latitude) : base(longitude, latitude) { }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override PixelCoordinate ToPixelCoordinate(int zoom, int tileSize)
        {
            double resolution = Resolution(zoom, tileSize);

            double px = (X + Constants.Geodesic.OriginShift) / resolution;
            double py = (Y + Constants.Geodesic.OriginShift) / resolution;

            return new PixelCoordinate(px, py);
        }

        /// <summary>
        /// Convert current coordinate to <see cref="GeodeticCoordinate"/>
        /// </summary>
        /// <returns>Converted <see cref="GeodeticCoordinate"/></returns>
        public GeodeticCoordinate ToGeodeticCoordinate()
        {
            double geodeticLongitude = X / Constants.Geodesic.OriginShift * 180.0;
            double geodeticLatitude = Y / Constants.Geodesic.OriginShift * 180.0;
            geodeticLatitude = 180.0 / Math.PI
                             * (2.0 * Math.Atan(Math.Exp(geodeticLatitude * Math.PI / 180.0)) - Math.PI / 2.0);

            return new GeodeticCoordinate(geodeticLongitude, geodeticLatitude);
        }

        /// <inheritdoc />
        public override double Resolution(int zoom, int tileSize) => Resolution(this, zoom, tileSize);

        /// <inheritdoc cref="Resolution(int, int)"/>
        /// <param name="coordinate">Safe to set to <see langword="null"/></param>
        /// <param name="zoom"></param>
        /// <param name="tileSize"></param>
        /// <returns></returns>
        public static double Resolution(GeoCoordinate coordinate, int zoom, int tileSize)
        {
            //156543.03392804062 for tileSize = 256
            double initialResolution = 2.0 * Constants.Geodesic.OriginShift / tileSize;

            return initialResolution / Math.Pow(2.0, zoom);
        }

        #endregion
    }
}
