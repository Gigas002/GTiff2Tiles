using System;
using System.Collections.Generic;
using System.Linq;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Abstract class for geographical coordinates
    /// </summary>
    public class GeoCoordinate : Coordinate
    {
        #region Constructors

        /// <inheritdoc />
        protected GeoCoordinate(double x, double y) : base(x, y) { }

        #endregion

        #region Methods

        /// <inheritdoc />
        public override Number ToNumber(int zoom, int tileSize, bool tmsCompatible)
        {
            PixelCoordinate pixelCoordinate = ToPixelCoordinate(zoom, tileSize);

            return pixelCoordinate.ToNumber(zoom, tileSize, tmsCompatible);
        }

        /// <summary>
        /// Gets <see cref="Number"/>s for specified <see cref="GeoCoordinate"/>s
        /// </summary>
        /// <param name="minCoordinate">Minimal <see cref="GeoCoordinate"/></param>
        /// <param name="maxCoordinate">Maximal <see cref="GeoCoordinate"/></param>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <param name="tmsCompatible">Is <see cref="ITile"/> tme compatible?</param>
        /// <returns><see cref="ValueTuple"/> of <see cref="Number"/>s</returns>
        public static (Number minNumber, Number maxNumber) GetNumbers(GeoCoordinate minCoordinate,
               GeoCoordinate maxCoordinate, int zoom, int tileSize, bool tmsCompatible)
        {
            if (minCoordinate == null) throw new ArgumentNullException(nameof(minCoordinate));
            if (maxCoordinate == null) throw new ArgumentNullException(nameof(maxCoordinate));

            Number minCoordNumber = minCoordinate.ToNumber(zoom, tileSize, tmsCompatible);
            Number maxCoordNumber = maxCoordinate.ToNumber(zoom, tileSize, tmsCompatible);

            Number minNumber = new Number(Math.Min(minCoordNumber.X, maxCoordNumber.X),
                                          Math.Min(minCoordNumber.Y, maxCoordNumber.Y),
                                          minCoordNumber.Z);
            Number maxNumber = new Number(Math.Max(minCoordNumber.X, maxCoordNumber.X),
                                          Math.Max(minCoordNumber.Y, maxCoordNumber.Y),
                                          maxCoordNumber.Z);

            return (minNumber, maxNumber);
        }

        /// <summary>
        /// Convert current <see cref="GeoCoordinate"/> to <see cref="PixelCoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <returns>Converted <see cref="PixelCoordinate"/></returns>
        public virtual PixelCoordinate ToPixelCoordinate(int zoom, int tileSize) => null;

        /// <summary>
        /// Resolution for given zoom level (measured at Equator)
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <param name="coordinateSystem">Desired coordinate system</param>
        /// <returns>Resolution value</returns>
        protected static double Resolution(int zoom, int tileSize, CoordinateSystem coordinateSystem) =>
            coordinateSystem switch
            {
                CoordinateSystem.Epsg4326 => GeodeticCoordinate.Resolution(zoom, tileSize),
                CoordinateSystem.Epsg3857 => MercatorCoordinate.Resolution(zoom, tileSize),
                _ => -1.0
            };

        /// <summary>
        /// Calculate zoom from known pixel size
        /// </summary>
        /// <param name="pixelSize">Pixel size</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <param name="coordinateSystem">Desired coordinate system</param>
        /// <param name="minZoom">Minimal zoom
        /// <para/>0 by default</param>
        /// <param name="maxZoom">Maximal zoom
        /// <para/>32 by default</param>
        /// <returns>Approximate zoom value</returns>
        public static int ZoomForPixelSize(int pixelSize, int tileSize, CoordinateSystem coordinateSystem,
                                           int minZoom = 0, int maxZoom = 32)
        {
            IEnumerable<int> range = Enumerable.Range(minZoom, maxZoom + 1);

            foreach (int i in range.Where(i => pixelSize > Resolution(i, tileSize, coordinateSystem)))
                return Math.Max(0, i - 1);

            return maxZoom - 1;
        }

        #endregion
    }
}
