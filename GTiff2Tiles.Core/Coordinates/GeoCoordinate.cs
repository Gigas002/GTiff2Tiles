using System;
using System.Collections.Generic;
using System.Linq;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Class for geographical coordinates
    /// </summary>
    public class GeoCoordinate : Coordinate
    {
        #region Constructors

        /// <inheritdoc />
        protected GeoCoordinate(double x, double y) : base(x, y) { }

        #endregion

        #region Methods

        /// <inheritdoc />
        /// <exception cref="ArgumentOutOfRangeException"/>
        public override Number ToNumber(int z, Size tileSize, bool tmsCompatible)
        {
            #region Preconditions checks

            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            PixelCoordinate pixelCoordinate = ToPixelCoordinate(z, tileSize);

            return pixelCoordinate.ToNumber(z, tileSize, tmsCompatible);
        }

        /// <summary>
        /// Gets <see cref="Number"/>s for specified <see cref="GeoCoordinate"/>s
        /// </summary>
        /// <param name="minCoordinate">Minimal <see cref="GeoCoordinate"/></param>
        /// <param name="maxCoordinate">Maximal <see cref="GeoCoordinate"/></param>
        /// <param name="z">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s size
        /// <remarks><para/>Must be square</remarks></param>
        /// <param name="tmsCompatible">Is <see cref="ITile"/> tms compatible?</param>
        /// <returns><see cref="ValueTuple{T1, T2}"/> of <see cref="Number"/>s</returns>
        public static (Number minNumber, Number maxNumber) GetNumbers(GeoCoordinate minCoordinate,
               GeoCoordinate maxCoordinate, int z, Size tileSize, bool tmsCompatible)
        {
            #region Preconditions checks

            if (minCoordinate == null) throw new ArgumentNullException(nameof(minCoordinate));
            if (maxCoordinate == null) throw new ArgumentNullException(nameof(maxCoordinate));
            // zoom and size are checked on lower levels

            #endregion

            Number minCoordNumber = minCoordinate.ToNumber(z, tileSize, tmsCompatible);
            Number maxCoordNumber = maxCoordinate.ToNumber(z, tileSize, tmsCompatible);

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
        /// <param name="z">Zoom
        /// <remarks><para/>Must be >= 0</remarks></param>
        /// <param name="tileSize"><see cref="ITile"/>'s size
        /// <remarks><para/>Must be square</remarks></param>
        /// <returns>Converted <see cref="PixelCoordinate"/></returns>
        public virtual PixelCoordinate ToPixelCoordinate(int z, Size tileSize) => null;

        /// <summary>
        /// Resolution for given zoom level (measured at Equator)
        /// </summary>
        /// <param name="z">Zoom
        /// <remarks><para/>Must be >= 0</remarks></param>
        /// <param name="tileSize"><see cref="ITile"/>'s size
        /// <remarks><para/>Must be square</remarks></param>
        /// <param name="coordinateSystem">Coordinate system</param>
        /// <returns>Resolution value or -1.0 if something goes wrong</returns>
        /// <exception cref="NotSupportedException"/>
        public static double Resolution(int z, Size tileSize, CoordinateSystem coordinateSystem) =>
            coordinateSystem switch
            {
                CoordinateSystem.Epsg4326 => GeodeticCoordinate.Resolution(z, tileSize),
                CoordinateSystem.Epsg3857 => MercatorCoordinate.Resolution(z, tileSize),
                _ => throw new NotSupportedException(string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem))
            };

        /// <summary>
        /// Calculate zoom from known pixel size
        /// </summary>
        /// <param name="pixelSize">Pixel size</param>
        /// <param name="tileSize"><see cref="ITile"/>'s size
        /// <remarks><para/>Must be square</remarks></param>
        /// <param name="coordinateSystem">Coordinate system</param>
        /// <param name="minZ">Minimal zoom
        /// <para/>Must be >= 0 and lesser or equal, than <paramref name="maxZ"/>
        /// <para/>0 by default</param>
        /// <param name="maxZ">Maximal zoom
        /// <para/>Must be >= 0 and bigger or equal, than <paramref name="minZ"/>
        /// <para/>32 by default</param>
        /// <returns>Approximate zoom value</returns>
        public static int ZoomForPixelSize(int pixelSize, Size tileSize, CoordinateSystem coordinateSystem,
                                           int minZ = 0, int maxZ = 32)
        {
            #region Preconditions checks

            if (pixelSize <= 0) throw new ArgumentOutOfRangeException(nameof(pixelSize));
            if (minZ < 0) throw new ArgumentOutOfRangeException(nameof(minZ));
            if (maxZ < minZ) throw new ArgumentOutOfRangeException(nameof(maxZ));

            #endregion

            IEnumerable<int> range = Enumerable.Range(minZ, maxZ + 1);

            foreach (int i in range.Where(i => pixelSize > Resolution(i, tileSize, coordinateSystem)))
                return Math.Max(0, i - 1);

            return maxZ - 1;
        }

        #endregion
    }
}
