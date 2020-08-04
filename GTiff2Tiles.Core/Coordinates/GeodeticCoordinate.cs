using System;
using GTiff2Tiles.Core.Enums;

// ReSharper disable UnusedMember.Global

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
        public double Longitude => X;

        /// <summary>
        /// Analogue of <see cref="Coordinate.Y"/>
        /// </summary>
        public double Latitude => Y;

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <param name="longitude"><see cref="Coordinate.X"/> or Longitude</param>
        /// <param name="latitude"><see cref="Coordinate.Y"/> or Latitude</param>
        public GeodeticCoordinate(double longitude, double latitude) : base(longitude, latitude) { }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override PixelCoordinate ToPixelCoordinate(int zoom, int tileSize)
        {
            double resolution = Resolution(zoom, tileSize);

            double pixelX = (180.0 + X) / resolution;
            double pixelY = (90.0 + Y) / resolution;

            return new PixelCoordinate(pixelX, pixelY);
        }

        /// <summary>
        /// Convert current coordinate to <see cref="MercatorCoordinate"/>
        /// </summary>
        /// <returns>Converted <see cref="MercatorCoordinate"/></returns>
        public MercatorCoordinate ToMercatorCoordinate()
        {
            double mercatorX = X * Constants.Geodesic.OriginShift / 180.0;
            double mercatorY =
                Math.Log(Math.Tan((90.0 + Y) * Math.PI / 360.0)) / (Math.PI / 180.0);
            mercatorY *= Constants.Geodesic.OriginShift / 180.0;

            return new MercatorCoordinate(mercatorX, mercatorY);
        }

        /// <inheritdoc cref="GeoCoordinate.Resolution(int, int, CoordinateType)"/>
        public static double Resolution(int zoom, int tileSize)
        {
            double resFactor = 180.0 / tileSize;

            return resFactor / Math.Pow(2, zoom);
        }

        #endregion
    }
}
