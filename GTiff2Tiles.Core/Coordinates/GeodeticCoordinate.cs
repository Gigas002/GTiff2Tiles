using System;

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Class for EPSG:4326 coordinates
    /// </summary>
    public class GeodeticCoordinate : GeoCoordinate
    {
        #region Properties

        /// <summary>
        /// Analogue of <see cref="Coordinate.X"/>
        /// </summary>
        public double Longitude
        {
            get => X;
            set => X = value;
        }

        /// <summary>
        /// Analogue of <see cref="Coordinate.Y"/>
        /// </summary>
        public double Latitude
        {
            get => Y;
            set => Y = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Create instance of class
        /// </summary>
        /// <param name="longitude">Longitude or X</param>
        /// <param name="latitude">Latitude or Y</param>
        public GeodeticCoordinate(double longitude, double latitude) => (Longitude, Latitude) = (longitude, latitude);

        #endregion

        #region Methods

        /// <inheritdoc />
        public override PixelCoordinate ToPixelCoordinate(int zoom, int tileSize)
        {
            double resolution = Resolution(zoom, tileSize);

            double pixelX = (180.0 + Longitude) / resolution;
            double pixelY = (90.0 + Latitude) / resolution;

            return new PixelCoordinate(pixelX, pixelY);
        }

        /// <summary>
        /// Convert current coordinate to <see cref="MercatorCoordinate"/>
        /// </summary>
        /// <returns>Converted <see cref="MercatorCoordinate"/></returns>
        public MercatorCoordinate ToMercatorCoordinate()
        {
            double mercatorLongitude = Longitude * Constants.Geodesic.OriginShift / 180.0;
            double mercatorLatitude =
                Math.Log(Math.Tan((90.0 + Latitude) * Math.PI / 360.0)) / (Math.PI / 180.0);
            mercatorLatitude *= Constants.Geodesic.OriginShift / 180.0;

            return new MercatorCoordinate(mercatorLongitude, mercatorLatitude);
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
            double resFactor = 180.0 / tileSize;
            return resFactor / Math.Pow(2, zoom);
        }

        #region Math operations

        /// <summary>
        /// Sum coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static GeodeticCoordinate operator +(GeodeticCoordinate coordinate1, GeodeticCoordinate coordinate2) =>
            new GeodeticCoordinate(coordinate1.X + coordinate2.X, coordinate1.Y + coordinate2.Y);

        /// <summary>
        /// Subtruct coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static GeodeticCoordinate operator -(GeodeticCoordinate coordinate1, GeodeticCoordinate coordinate2) =>
            new GeodeticCoordinate(coordinate1.X - coordinate2.X, coordinate1.Y - coordinate2.Y);

        /// <summary>
        /// Multiply coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static GeodeticCoordinate operator *(GeodeticCoordinate coordinate1, GeodeticCoordinate coordinate2) =>
            new GeodeticCoordinate(coordinate1.X * coordinate2.X, coordinate1.Y * coordinate2.Y);

        /// <summary>
        /// Divide coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static GeodeticCoordinate operator /(GeodeticCoordinate coordinate1, GeodeticCoordinate coordinate2) =>
            new GeodeticCoordinate(coordinate1.X / coordinate2.X, coordinate1.Y / coordinate2.Y);

        #endregion

        #endregion
    }
}
