using System;

// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// Number of tile
    /// </summary>
    public class Number : IEquatable<Number>
    {
        // TODO: GeodeticNumber, etc?

        #region Properties

        /// <summary>
        /// X number
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y number
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Zoom
        /// </summary>
        public int Z { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes number
        /// </summary>
        /// <param name="x">X number</param>
        /// <param name="y">Y number</param>
        /// <param name="z">Zoom</param>
        public Number(int x, int y, int z) => (X, Y, Z) = (x, y, z);

        #endregion

        #region Methods

        #region Equals

        /// <inheritdoc />
        public override bool Equals(object number) => Equals(number as Number);

        /// <inheritdoc />
        public bool Equals(Number other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        /// <summary>
        /// Check two numbers for equality
        /// </summary>
        /// <param name="num1">Number 1</param>
        /// <param name="num2">Number 2</param>
        /// <returns><see langword="true"/> if numbers are equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool? operator ==(Number num1, Number num2) => num1?.Equals(num2);

        /// <summary>
        /// Check two numbers for non-equality
        /// </summary>
        /// <param name="num1">Number 1</param>
        /// <param name="num2">Number 2</param>
        /// <returns><see langword="true"/> if numbers are not equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool? operator !=(Number num1, Number num2) => !(num1 == num2);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        #endregion

        #region Flip

        /// <summary>
        /// Converts y number
        /// </summary>
        /// <param name="y">Number.Y</param>
        /// <param name="z">Zoom</param>
        /// <returns>Converted Number.Y</returns>
        internal static int FlipY(int y, int z) => Convert.ToInt32(Math.Pow(2.0, z) - y - 1);

        /// <summary>
        /// Flips this number
        /// </summary>
        /// <returns>Converted number</returns>
        public Number Flip() => Flip(this);

        /// <summary>
        /// Flips number
        /// </summary>
        /// <param name="number">Number to flip</param>
        /// <returns>Converted number</returns>
        public static Number Flip(Number number) => new Number(number.X, FlipY(number.Y, number.Z), number.Z);

        #endregion

        #endregion
    }
}
