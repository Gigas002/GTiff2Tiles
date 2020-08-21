using System;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;

// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Class for EPSG:3857 coordinates
    /// </summary>
    public class MercatorCoordinate : GeoCoordinate
    {
        #region Properties/Constants

        /// <summary>
        /// Maximal possible value of longitude for EPSG:3857
        /// </summary>
        public const double MaxPossibleLonValue = 20037508.35; // 20026376.39;

        /// <summary>
        /// Maximal possible value of latitude for EPSG:3857
        /// </summary>
        public const double MaxPossibleLatValue = 20048966.10;

        /// <summary>
        /// Minimal possible value of longitude for EPSG:3857
        /// </summary>
        public const double MinPossibleLonValue = -MaxPossibleLonValue;

        /// <summary>
        /// Minimal possible value of latitude for EPSG:3857
        /// </summary>
        public const double MinPossibleLatValue = -MaxPossibleLatValue;

        #endregion

        #region Constructors

        /// <inheritdoc />
        /// <param name="longitude"><see cref="Coordinate.X"/> or Longitude
        /// <remarks><para/>Must be in range [-20026376.39, 20026376.39]</remarks></param>
        /// <param name="latitude"><see cref="Coordinate.Y"/> or Latitude
        /// <remarks><para/>Must be in range [-20048966.10, 20048966.10]</remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public MercatorCoordinate(double longitude, double latitude) : base(longitude, latitude)
        {
            if (longitude < MinPossibleLonValue || longitude > MaxPossibleLonValue)
                throw new ArgumentOutOfRangeException(nameof(longitude));
            if (latitude < MinPossibleLatValue || latitude > MaxPossibleLatValue)
                throw new ArgumentOutOfRangeException(nameof(latitude));
        }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override PixelCoordinate ToPixelCoordinate(int z, Size tileSize)
        {
            double resolution = Resolution(z, tileSize);

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

        /// <inheritdoc cref="GeoCoordinate.Resolution(int, Size, CoordinateSystem)"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static double Resolution(int z, Size tileSize)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));
            if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));
            if (!tileSize.IsSquare) throw new ArgumentException(Strings.NotSqare);

            #endregion

            //156543.03392804062 for tileSize = 256
            double initialResolution = 2.0 * Constants.Geodesic.OriginShift / tileSize.Width;

            return initialResolution / Math.Pow(2.0, z);
        }

        #endregion
    }
}
