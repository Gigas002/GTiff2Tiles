using System;
using GTiff2Tiles.Core.Enums;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Class for EPSG:4326 coordinates
    /// </summary>
    public class GeodeticCoordinate : GeoCoordinate
    {
        #region Properties/Constants

        /// <summary>
        /// Maximal possible value of longitude for EPSG:4326
        /// </summary>
        public const double MaxPossibleLonValue = 180.0;

        /// <summary>
        /// Maximal possible value of latitude for EPSG:4326
        /// </summary>
        public const double MaxPossibleLatValue = 90.0;

        /// <summary>
        /// Minimal possible value of longitude for EPSG:4326
        /// </summary>
        public const double MinPossibleLonValue = -MaxPossibleLonValue;

        /// <summary>
        /// Minimal possible value of latitude for EPSG:4326
        /// </summary>
        public const double MinPossibleLatValue = -MaxPossibleLatValue;

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
        /// <param name="longitude"><see cref="Coordinate.X"/> or Longitude
        /// <remarks><para/>Must be in range [-180.0, 180.0]</remarks></param>
        /// <param name="latitude"><see cref="Coordinate.Y"/> or Latitude
        /// <remarks><para/>Must be in range [-90.0, 90.0]</remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public GeodeticCoordinate(double longitude, double latitude) : base(longitude, latitude)
        {
            if (longitude < MinPossibleLonValue || longitude > MaxPossibleLonValue)
                throw new ArgumentOutOfRangeException(nameof(longitude));
            if (latitude < MinPossibleLatValue || latitude > MaxPossibleLatValue)
                throw new ArgumentOutOfRangeException(nameof(latitude));
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException"/>
        public override PixelCoordinate ToPixelCoordinate(int z, int tileSize)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            double resolution = Resolution(z, tileSize);

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
            double mercatorY = Math.Log(Math.Tan((90.0 + Y) * Math.PI / 360.0)) / (Math.PI / 180.0);
            mercatorY *= Constants.Geodesic.OriginShift / 180.0;

            return new MercatorCoordinate(mercatorX, mercatorY);
        }

        /// <inheritdoc cref="GeoCoordinate.Resolution(int, int, CoordinateSystem)"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static double Resolution(int z, int tileSize)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            double resFactor = 180.0 / tileSize;

            return resFactor / Math.Pow(2, z);
        }

        #endregion
    }
}
