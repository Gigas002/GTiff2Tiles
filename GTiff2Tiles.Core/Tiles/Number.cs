using System;
using System.Linq;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;

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
        /// <param name="x"><see cref="X"/>
        /// <remarks><para/>Should be >= 0</remarks></param>
        /// <param name="y"><see cref="Y"/>
        /// <remarks><para/>Should be >= 0</remarks></param>
        /// <param name="z">Zoom
        /// <remarks><para/>Should be >= 0</remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public Number(int x, int y, int z)
        {
            #region Preconditions checks

            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y));
            if (z < 0) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            (X, Y, Z) = (x, y, z);
        }

        #endregion

        #region Methods

        #region Flip

        /// <summary>
        /// Flips value of passed <see cref="Y"/> on specified zoom
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
        /// <exception cref="ArgumentNullException"/>
        public static Number Flip(Number number)
        {
            #region Preconditions checks

            if (number == null) throw new ArgumentNullException(nameof(number));

            #endregion

            return new Number(number.X, FlipY(number.Y, number.Z), number.Z);
        }

        #endregion

        #region ToGeoCoordinates

        /// <summary>
        /// Convert <see cref="Number"/> to <see cref="GeodeticCoordinate"/>s
        /// </summary>
        /// <param name="tileSize"><see cref="Tile"/>'s <see cref="Size"/></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <returns><see cref="ValueTuple{T1,T2}"/> of <see cref="GeodeticCoordinate"/>s</returns>
        /// <exception cref="ArgumentNullException"/>
        public (GeodeticCoordinate minCoordinate, GeodeticCoordinate maxCoordinate) ToGeodeticCoordinates(
            Size tileSize, bool tmsCompatible) => ToGeodeticCoordinates(this, tileSize, tmsCompatible);

        /// <inheritdoc cref="ToGeodeticCoordinates(Size, bool)"/>
        /// <param name="number"><see cref="Number"/> to convert</param>
        /// <param name="tileSize"></param>
        /// <param name="tmsCompatible"></param>
        public static (GeodeticCoordinate minCoordinate, GeodeticCoordinate maxCoordinate) ToGeodeticCoordinates(
            Number number, Size tileSize, bool tmsCompatible)
        {
            #region Preconditions checks

            if (number == null) throw new ArgumentNullException(nameof(number));
            if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));

            #endregion

            if (!tmsCompatible) number = Flip(number);

            double resolution = GeodeticCoordinate.Resolution(number.Z, tileSize);

            GeodeticCoordinate minCoordinate = new GeodeticCoordinate(number.X * tileSize.Width * resolution - 180.0,
                                                                      number.Y * tileSize.Height * resolution - 90.0);
            GeodeticCoordinate maxCoordinate = new GeodeticCoordinate((number.X + 1) * tileSize.Width * resolution - 180.0,
                                                                      (number.Y + 1) * tileSize.Height * resolution - 90.0);

            return (minCoordinate, maxCoordinate);
        }

        /// <summary>
        /// Convert <see cref="Number"/> to <see cref="MercatorCoordinate"/>s
        /// </summary>
        /// <param name="tileSize"><see cref="Tile"/>'s <see cref="Size"/></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <returns><see cref="ValueTuple{T1, T2}"/> of <see cref="MercatorCoordinate"/>s</returns>
        /// <exception cref="ArgumentNullException"/>
        public (MercatorCoordinate minCoordinate, MercatorCoordinate maxCoordinate) ToMercatorCoordinates(Size tileSize, bool tmsCompatible)
            => ToMercatorCoordinates(this, tileSize, tmsCompatible);

        /// <inheritdoc cref="ToMercatorCoordinates(Size, bool)"/>
        /// <param name="number"><see cref="Number"/> to convert</param>
        /// <param name="tileSize"></param>
        /// <param name="tmsCompatible"></param>
        public static (MercatorCoordinate minCoordinate, MercatorCoordinate maxCoordinate) ToMercatorCoordinates(
            Number number, Size tileSize, bool tmsCompatible)
        {
            #region Preconditions checks

            if (number == null) throw new ArgumentNullException(nameof(number));
            if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));

            #endregion

            if (!tmsCompatible) number = Flip(number);

            PixelCoordinate minPixelCoordinate = new PixelCoordinate(number.X * tileSize.Width,
                                                                     number.Y * tileSize.Height);
            PixelCoordinate maxPixelCoordinate = new PixelCoordinate((number.X + 1) * tileSize.Width,
                                                                     (number.Y + 1) * tileSize.Height);
            MercatorCoordinate minCoordinate = minPixelCoordinate.ToMercatorCoordinate(CoordinateSystem.Epsg3857,
                number.Z, tileSize);
            MercatorCoordinate maxCoordinate = maxPixelCoordinate.ToMercatorCoordinate(CoordinateSystem.Epsg3857,
                number.Z, tileSize);

            return (minCoordinate, maxCoordinate);
        }

        /// <summary>
        /// Convert <see cref="Number"/> to <see cref="GeoCoordinate"/>s
        /// </summary>
        /// <param name="coordinateSystem">Desired number's coordinate system</param>
        /// <param name="tileSize"><see cref="Tile"/>'s <see cref="Size"/></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <returns><see cref="ValueTuple{T1, T2}"/> of <see cref="GeoCoordinate"/>s</returns>
        /// <exception cref="ArgumentNullException"/>
        public (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) ToGeoCoordinates(
            CoordinateSystem coordinateSystem, Size tileSize, bool tmsCompatible) =>
            ToGeoCoordinates(this, coordinateSystem, tileSize, tmsCompatible);

        /// <inheritdoc cref="ToGeoCoordinates(CoordinateSystem,Size,bool)"/>
        /// <param name="number"><see cref="Number"/> to convert</param>
        /// <param name="coordinateSystem"></param>
        /// <param name="tileSize"></param>
        /// <param name="tmsCompatible"></param>
        /// <exception cref="NotSupportedException"/>
        public static (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) ToGeoCoordinates(
            Number number, CoordinateSystem coordinateSystem, Size tileSize, bool tmsCompatible)
        {
            #region Preconditions checks

            if (number == null) throw new ArgumentNullException(nameof(number));

            #endregion

            switch (coordinateSystem)
            {
                case CoordinateSystem.Epsg4326:
                {
                    (GeodeticCoordinate minCoordinate, GeodeticCoordinate maxCoordinate) =
                        number.ToGeodeticCoordinates(tileSize, tmsCompatible);

                    return (minCoordinate, maxCoordinate);
                }
                case CoordinateSystem.Epsg3857:
                {
                    (MercatorCoordinate minCoordinate, MercatorCoordinate maxCoordinate) =
                        number.ToMercatorCoordinates(tileSize, tmsCompatible);

                    return (minCoordinate, maxCoordinate);
                }
                default:
                {
                    string err = string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem);

                    throw new NotSupportedException(err);
                }
            }
        }

        #endregion

        #region GetLowerNumbers

        /// <summary>
        /// Get lower <see cref="Number"/>s for specified <see cref="Number"/> and zoom
        /// </summary>
        /// <param name="z">Zoom;
        /// <remarks><para/>Must be >= 10</remarks></param>
        /// <returns><see cref="ValueTuple{T1, T2}"/> of lower <see cref="Number"/>s</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public (Number minNumber, Number maxNumber) GetLowerNumbers(int z) =>
            GetLowerNumbers(this, z);

        /// <inheritdoc cref="GetLowerNumbers(int)"/>
        /// <param name="number">Base <see cref="Number"/></param>
        /// <param name="z"></param>
        public static (Number minNumber, Number maxNumber) GetLowerNumbers(Number number, int z)
        {
            #region Preconditions checks

            if (number == null) throw new ArgumentNullException(nameof(number));
            if (z < 10) throw new ArgumentOutOfRangeException(nameof(z));

            #endregion

            int resolution = Convert.ToInt32(Math.Pow(2.0, z - 10.0));

            int[] tilesXs = { number.X * resolution, (number.X + 1) * resolution - 1 };
            int[] tilesYs = { number.Y * resolution, (number.Y + 1) * resolution - 1 };

            Number minNumber = new Number(tilesXs.Min(), tilesYs.Min(), z);
            Number maxNumber = new Number(tilesXs.Max(), tilesYs.Max(), z);

            return (minNumber, maxNumber);
        }

        /// <inheritdoc cref="GetLowerNumbers(Number)"/>
        public Number[] GetLowerNumbers() => GetLowerNumbers(this);

        /// <summary>
        /// Gets 4 one zoom lower <see cref="Number"/>s
        /// </summary>
        /// <param name="number">Input <see cref="Number"/></param>
        /// <returns>4 lower <see cref="Number"/>s</returns>
        /// <exception cref="ArgumentNullException"/>
        public static Number[] GetLowerNumbers(Number number)
        {
            #region Preconditions checks

            if (number == null) throw new ArgumentNullException(nameof(number));

            #endregion

            Number[] numbers = new Number[4];

            numbers[0] = new Number(number.X * 2, number.Y * 2, number.Z + 1);
            numbers[1] = new Number(numbers[0].X + 1, numbers[0].Y, numbers[0].Z);
            numbers[2] = new Number(numbers[0].X, numbers[0].Y + 1, numbers[0].Z);
            numbers[3] = new Number(numbers[0].X + 1, numbers[0].Y + 1, numbers[0].Z);

            return numbers;
        }

        #endregion

        #region GetCount

        /// <summary>
        /// Get count of <see cref="Tile"/>s in specified region
        /// </summary>
        /// <param name="minCoordinate">Minimal <see cref="GeoCoordinate"/></param>
        /// <param name="maxCoordinate">Maximal <see cref="GeoCoordinate"/></param>
        /// <param name="minZ">Minimal zoom</param>
        /// <param name="maxZ">Maximal zoom</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="tileSize"><see cref="Tile"/>'s <see cref="Size"/></param>
        /// <returns><see cref="Tile"/>s count</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static int GetCount(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate,
                                   int minZ, int maxZ, bool tmsCompatible, Size tileSize)
        {
            #region Preconditions checks

            // Coordinates are checked inside GeoCoordinate.GetNumbers, no need to check it here
            // Size is checked inside GeoCoordinate.GetNumbers

            if (minZ < 0) throw new ArgumentOutOfRangeException(nameof(minZ));
            if (maxZ < minZ) throw new ArgumentOutOfRangeException(nameof(maxZ));

            #endregion

            int tilesCount = 0;

            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                (Number minNumber, Number maxNumber) =
                    GeoCoordinate.GetNumbers(minCoordinate, maxCoordinate, zoom, tileSize, tmsCompatible);

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
        /// <see langword="false"/> otherwise</returns>
        public static bool operator ==(Number number1, Number number2) => number1?.Equals(number2) ?? number2 is null;

        /// <summary>
        /// Check two <see cref="Number"/>s for non-equality
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Number"/>s are not equal;
        /// <see langword="false"/> otherwise</returns>
        public static bool operator !=(Number number1, Number number2) => !(number1 == number2);

        #endregion

        #region Math operations

        /// <summary>
        /// Sum <see cref="Number"/>s
        /// <remarks><para/>Sums <see cref="X"/> and <see cref="Y"/>
        /// only if <see cref="Z"/>'s are the same;
        /// returns <see langword="null"/> otherwise</remarks>
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/>, if <see cref="Z"/>s are the same</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Number operator +(Number number1, Number number2)
        {
            #region Preconditions checks

            if (number1 == null) throw new ArgumentNullException(nameof(number1));
            if (number2 == null) throw new ArgumentNullException(nameof(number2));

            string err = string.Format(Strings.Culture, Strings.NotEqual, nameof(number1.Z), nameof(number2.Z));

            if (number1.Z != number2.Z) throw new ArgumentException(err);

            #endregion

            return new Number(number1.X + number2.X, number1.Y + number2.Y, number1.Z);
        }

        /// <inheritdoc cref="op_Addition"/>
        /// <param name="other"><see cref="Number"/> to add</param>
        public Number Add(Number other) => this + other;

        /// <summary>
        /// Subtruct <see cref="Number"/>s
        /// <remarks><para/>Subtruct <see cref="X"/> and <see cref="Y"/>
        /// only if <see cref="Z"/>'s are the same;
        /// returns <see langword="null"/> otherwise</remarks>
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/>, if <see cref="Z"/>s are the same</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Number operator -(Number number1, Number number2)
        {
            #region Preconditions checks

            if (number1 == null) throw new ArgumentNullException(nameof(number1));
            if (number2 == null) throw new ArgumentNullException(nameof(number2));

            string err = string.Format(Strings.Culture, Strings.NotEqual, nameof(number1.Z), nameof(number2.Z));

            if (number1.Z != number2.Z) throw new ArgumentException(err);

            #endregion

            return new Number(number1.X - number2.X, number1.Y - number2.Y, number1.Z);
        }

        /// <inheritdoc cref="op_Subtraction"/>
        /// <param name="other"><see cref="Number"/> to subtract</param>
        public Number Subtract(Number other) => this - other;

        /// <summary>
        /// Multiply <see cref="Number"/>s
        /// <remarks><para/>Multiply <see cref="X"/> and <see cref="Y"/>
        /// only if <see cref="Z"/>'s are the same;
        /// returns <see langword="null"/> otherwise</remarks>
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/>, if <see cref="Z"/>s are the same</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Number operator *(Number number1, Number number2)
        {
            #region Preconditions checks

            if (number1 == null) throw new ArgumentNullException(nameof(number1));
            if (number2 == null) throw new ArgumentNullException(nameof(number2));

            string err = string.Format(Strings.Culture, Strings.NotEqual, nameof(number1.Z), nameof(number2.Z));

            if (number1.Z != number2.Z) throw new ArgumentException(err);

            #endregion

            return new Number(number1.X * number2.X, number1.Y * number2.Y, number1.Z);
        }

        /// <inheritdoc cref="op_Multiply"/>
        /// <param name="other"><see cref="Number"/> to multiply</param>
        public Number Multiply(Number other) => this * other;

        /// <summary>
        /// Divide <see cref="Number"/>s
        /// <remarks><para/>Divide <see cref="X"/> and <see cref="Y"/>
        /// only if <see cref="Z"/>'s are the same;
        /// returns <see langword="null"/> otherwise</remarks>
        /// </summary>
        /// <param name="number1"><see cref="Number"/> 1</param>
        /// <param name="number2"><see cref="Number"/> 2</param>
        /// <returns>New <see cref="Number"/>, if <see cref="Z"/>s are the same</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static Number operator /(Number number1, Number number2)
        {
            #region Preconditions checks

            if (number1 == null) throw new ArgumentNullException(nameof(number1));
            if (number2 == null) throw new ArgumentNullException(nameof(number2));

            string err = string.Format(Strings.Culture, Strings.NotEqual, nameof(number1.Z), nameof(number2.Z));

            if (number1.Z != number2.Z) throw new ArgumentException(err);

            #endregion

            return new Number(number1.X / number2.X, number1.Y / number2.Y, number1.Z);
        }

        /// <inheritdoc cref="op_Division"/>
        /// <param name="other"><see cref="Number"/> to divide on</param>
        public Number Divide(Number other) => this / other;

        #endregion

        #endregion
    }
}
