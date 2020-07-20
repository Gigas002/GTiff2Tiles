using System;
using System.Linq;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// <see cref="Number"/> of <see cref="ITile"/>
    /// </summary>
    public class Number : IEquatable<Number>
    {
        #region Properties

        /// <summary>
        /// X <see cref="Number"/> value
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y <see cref="Number"/> value
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Zoom
        /// </summary>
        public int Z { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <see cref="Number"/>
        /// </summary>
        /// <param name="x"><see cref="X"/></param>
        /// <param name="y"><see cref="Y"/></param>
        /// <param name="z">Zoom</param>
        public Number(int x, int y, int z) => (X, Y, Z) = (x, y, z);

        #endregion

        #region Methods

        #region Flip

        /// <summary>
        /// Flips value of passed <see cref="Y"/>
        /// </summary>
        /// <param name="y"><see cref="Y"/> to flip</param>
        /// <param name="z">Zoom</param>
        /// <returns>Converted <see cref="Y"/></returns>
        private static int FlipY(int y, int z) => Convert.ToInt32(Math.Pow(2.0, z) - y - 1.0);

        /// <summary>
        /// Flips <see cref="Number"/>
        /// </summary>
        /// <returns>Converted <see cref="Number"/></returns>
        public Number Flip() => Flip(this);

        /// <inheritdoc cref="Flip()"/>
        /// <param name="number"><see cref="Number"/> to flip</param>
        public static Number Flip(Number number) => new Number(number.X, FlipY(number.Y, number.Z), number.Z);

        #endregion

        #region ToGeoCoordinates

        /// <summary>
        /// Convert <see cref="Number"/> to <see cref="GeodeticCoordinate"/>s
        /// </summary>
        /// <param name="tileSize"><see cref="Tile"/> <see cref="Size"/></param>
        /// <returns><see cref="ValueTuple"/> of <see cref="GeodeticCoordinate"/>s</returns>
        public (GeodeticCoordinate minCoordinate, GeodeticCoordinate maxCoordinate) ToGeodeticCoordinates(
            Size tileSize) => ToGeodeticCoordinates(this, tileSize);

        /// <inheritdoc cref="ToGeodeticCoordinates(Size)"/>
        /// <param name="number"><see cref="Number"/> to convert</param>
        /// <param name="tileSize"></param>
        public static (GeodeticCoordinate minCoordinate, GeodeticCoordinate maxCoordinate) ToGeodeticCoordinates(
            Number number, Size tileSize)
        {
            double resolution = GeodeticCoordinate.Resolution(null, number.Z, tileSize.Width);

            GeodeticCoordinate minCoordinate = new GeodeticCoordinate(number.X * tileSize.Width * resolution - 180.0,
                                                                      number.Y * tileSize.Height * resolution - 90.0);
            GeodeticCoordinate maxCoordinate = new GeodeticCoordinate((number.X + 1) * tileSize.Width * resolution - 180.0,
                                                                      (number.Y + 1) * tileSize.Height * resolution - 90.0);

            return (minCoordinate, maxCoordinate);
        }

        /// <summary>
        /// Convert <see cref="Number"/> to <see cref="MercatorCoordinate"/>s
        /// </summary>
        /// <param name="tileSize"><see cref="Tile"/> <see cref="Size"/></param>
        /// <returns><see cref="ValueTuple"/> of <see cref="MercatorCoordinate"/>s</returns>
        public (MercatorCoordinate minCoordinate, MercatorCoordinate maxCoordinate) ToMercatorCoordinates(Size tileSize)
            => ToMercatorCoordinates(this, tileSize);

        /// <inheritdoc cref="ToMercatorCoordinates(Size)"/>
        /// <param name="number"><see cref="Number"/> to convert</param>
        /// <param name="tileSize"></param>
        public static (MercatorCoordinate minCoordinate, MercatorCoordinate maxCoordinate) ToMercatorCoordinates(
            Number number, Size tileSize)
        {
            PixelCoordinate minPixelCoordinate = new PixelCoordinate(number.X * tileSize.Width,
                                                                     number.Y * tileSize.Height);
            PixelCoordinate maxPixelCoordinate = new PixelCoordinate((number.X + 1) * tileSize.Width,
                                                                     (number.Y + 1) * tileSize.Height);
            MercatorCoordinate minCoordinate = minPixelCoordinate.ToMercatorCoordinate(number.Z, tileSize.Width);
            MercatorCoordinate maxCoordinate = maxPixelCoordinate.ToMercatorCoordinate(number.Z, tileSize.Width);

            return (minCoordinate, maxCoordinate);
        }

        /// <summary>
        /// Convert <see cref="Number"/> to <see cref="GeoCoordinate"/>s
        /// </summary>
        /// <param name="coordinateType">Type of <see cref="GeoCoordinate"/></param>
        /// <param name="tileSize"><see cref="Tile"/> <see cref="Size"/></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <returns><see cref="ValueTuple"/> of <see cref="GeoCoordinate"/>s</returns>
        public (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) ToGeoCoordinates(
            CoordinateType coordinateType, Size tileSize, bool tmsCompatible) =>
            ToGeoCoordinates(this, coordinateType, tileSize, tmsCompatible);

        /// <inheritdoc cref="ToGeoCoordinates(CoordinateType,Size,bool)"/>
        /// <param name="number"><see cref="Number"/> to convert</param>
        /// <param name="coordinateType"></param>
        /// <param name="tileSize"></param>
        /// <param name="tmsCompatible"></param>
        public static (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) ToGeoCoordinates(
            Number number, CoordinateType coordinateType, Size tileSize, bool tmsCompatible)
        {
            if (!tmsCompatible) number = Flip(number);

            switch (coordinateType)
            {
                case CoordinateType.Geodetic:
                    {
                        (GeodeticCoordinate minCoordinate, GeodeticCoordinate maxCoordinate) =
                            ToGeodeticCoordinates(number, tileSize);

                        return (minCoordinate, maxCoordinate);
                    }
                case CoordinateType.Mercator:
                    {
                        (MercatorCoordinate minCoordinate, MercatorCoordinate maxCoordinate) =
                            ToMercatorCoordinates(number, tileSize);

                        return (minCoordinate, maxCoordinate);
                    }
                default: return (null, null);
            }
        }

        #endregion

        #region GetLowerNumbers

        /// <summary>
        /// Get lower <see cref="Number"/>s for specified <see cref="Number"/> and zoom (>=10)
        /// </summary>
        /// <param name="z">Zoom;
        /// <remarks>must be >=10</remarks></param>
        /// <returns><see cref="ValueTuple"/> of lower <see cref="Number"/>s</returns>
        public (Number minNumber, Number maxNumber) GetLowerNumbers(int z) =>
            GetLowerNumbers(this, z);

        /// <inheritdoc cref="GetLowerNumbers(int)"/>
        /// <param name="number">Base <see cref="Number"/></param>
        /// <param name="z"></param>
        public static (Number minNumber, Number maxNumber) GetLowerNumbers(Number number, int z)
        {
            if (z < 10) return (null, null);

            int resolution = Convert.ToInt32(Math.Pow(2.0, z - 10.0));

            int[] tilesXs = { number.X * resolution, (number.X + 1) * resolution - 1 };
            int[] tilesYs = { number.Y * resolution, (number.Y + 1) * resolution - 1 };

            Number minNumber = new Number(tilesXs.Min(), tilesYs.Min(), z);
            Number maxNumber = new Number(tilesXs.Max(), tilesYs.Max(), z);

            return (minNumber, maxNumber);
        }

        #endregion

        #region GetCount

        /// <summary>
        /// Get count of <see cref="Tile"/>s in specified region
        /// </summary>
        /// <param name="minCoordinate">Minimum <see cref="GeoCoordinate"/></param>
        /// <param name="maxCoordinate">Maximum <see cref="GeoCoordinate"/></param>
        /// <param name="minZ">Minimum zoom</param>
        /// <param name="maxZ">Maximum zoom</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="size"><see cref="Tile"/>'s <see cref="Size"/></param>
        /// <returns><see cref="Tile"/>s count</returns>
        public static int GetCount(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate,
                                   int minZ, int maxZ, bool tmsCompatible, Size size)
        {
            int tilesCount = 0;

            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                (Number minNumber, Number maxNumber) =
                    GeoCoordinate.GetNumbers(minCoordinate, maxCoordinate, zoom, size.Width, tmsCompatible);

                int xsCount = Enumerable.Range(minNumber.X, maxNumber.X - minNumber.X + 1).Count();
                int ysCount = Enumerable.Range(minNumber.Y, maxNumber.Y - minNumber.Y + 1).Count();

                tilesCount += xsCount * ysCount;
            }

            return tilesCount;
        }

        #endregion

        #region Bool compare overrides

        /// <inheritdoc />
        public override bool Equals(object number) => Equals(number as Number);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        /// <inheritdoc />
        public bool Equals(Number other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Check two <see cref="Number"/>s for equality
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Number"/>s are equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator ==(Number number1, Number number2) =>
            number1?.Equals(number2) == true;

        /// <summary>
        /// Check two <see cref="Number"/>s for non-equality
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Number"/>s are not equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator !=(Number number1, Number number2) =>
            !(number1 == number2);

        /// <summary>
        /// Check if <see cref="Number"/>1 is lesser, then <see cref="Number"/>2
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Number"/>1 is lesser;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator <(Number number1, Number number2) =>
            number1.X < number2.X && number1.Y < number2.Y && number1.Z < number2.Z;

        /// <summary>
        /// Check if <see cref="Number"/>1 is bigger, then <see cref="Number"/>2
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Number"/>1 is bigger;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator >(Number number1, Number number2) =>
            number1.X > number2.X && number1.Y > number2.Y && number1.Z > number2.Z;

        /// <summary>
        /// Check if <see cref="Number"/>1 is lesser or equal, then <see cref="Number"/>2
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Number"/>1 is lesser or equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator <=(Number number1, Number number2) =>
            number1.X <= number2.X && number1.Y <= number2.Y && number1.Z <= number2.Z;

        /// <summary>
        /// Check if <see cref="Number"/>1 is bigger or equal, then <see cref="Number"/>2
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Number"/>1 is bigger or equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator >=(Number number1, Number number2) =>
            number1.X >= number2.X && number1.Y >= number2.Y && number1.Z >= number2.Z;

        #endregion

        #region Math operations

        /// <summary>
        /// Sum <see cref="Number"/>s
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/></returns>
        public static Number operator +(Number number1, Number number2) => number1.Z != number2.Z ? null : new Number(number1.X + number2.X, number1.Y + number2.Y, number1.Z);

        /// <summary>
        /// Subtruct <see cref="Number"/>s
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/></returns>
        public static Number operator -(Number number1, Number number2) => number1.Z != number2.Z ? null : new Number(number1.X - number2.X, number1.Y - number2.Y, number1.Z);

        /// <summary>
        /// Multiply <see cref="Number"/>s
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/></returns>
        public static Number operator *(Number number1, Number number2) => number1.Z != number2.Z ? null : new Number(number1.X * number2.X, number1.Y * number2.Y, number1.Z);

        /// <summary>
        /// Divide <see cref="Number"/>s
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/></returns>
        public static Number operator /(Number number1, Number number2) => number1.Z != number2.Z ? null : new Number(number1.X / number2.X, number1.Y / number2.Y, number1.Z);

        #endregion

        #endregion
    }
}
