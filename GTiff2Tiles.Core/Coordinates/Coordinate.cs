using System;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Basic implementation of <see cref="ICoordinate"/> interface
    /// </summary>
    public class Coordinate : ICoordinate
    {
        #region Properties

        /// <inheritdoc />
        public virtual double X { get; }

        /// <inheritdoc />
        public virtual double Y { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create instance of class
        /// </summary>
        /// <param name="x">X coordinate value</param>
        /// <param name="y">Y coordinate value</param>
        protected Coordinate(double x, double y) => (X, Y) = (x, y);

        #endregion

        #region Methods

        /// <inheritdoc />
        public virtual Number ToNumber(int z, Size tileSize, bool tmsCompatible) => null;

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        /// <param name="degrees">Value to convert</param>
        /// <returns>Converted radians</returns>
        public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        /// <param name="radians">Value to convert</param>
        /// <returns>Converted degrees</returns>
        public static double RadiansToDegrees(double radians) => radians * 180.0 / Math.PI;

        #region Bool compare overrides

        /// <inheritdoc />
        public override bool Equals(object coordinate) => Equals(coordinate as ICoordinate);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(X, Y);

        /// <inheritdoc />
        public bool Equals(ICoordinate other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Math.Abs(X - other.X) < double.Epsilon &&
                   Math.Abs(Y - other.Y) < double.Epsilon;
        }

        /// <summary>
        /// Check two coordinates for equality
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns><see langword="true"/> if coordinates are equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator ==(Coordinate coordinate1, Coordinate coordinate2) =>
            coordinate1?.Equals(coordinate2) ?? coordinate2 is null;

        /// <summary>
        /// Check two coordinates for non-equality
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns><see langword="true"/> if coordinates are not equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator !=(Coordinate coordinate1, Coordinate coordinate2) =>
            !(coordinate1 == coordinate2);

        #endregion

        #region Math operations

        /// <summary>
        /// Sum coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator +(Coordinate coordinate1, Coordinate coordinate2)
        {
            if (coordinate1 == null) throw new ArgumentNullException(nameof(coordinate1));
            if (coordinate2 == null) throw new ArgumentNullException(nameof(coordinate2));

            return new Coordinate(coordinate1.X + coordinate2.X, coordinate1.Y + coordinate2.Y);
        }

        /// <inheritdoc cref="op_Addition"/>
        /// <param name="other"><see cref="Coordinate"/> to add</param>
        public Coordinate Add(Coordinate other) => this + other;

        /// <summary>
        /// Subtruct coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator -(Coordinate coordinate1, Coordinate coordinate2)
        {
            if (coordinate1 == null) throw new ArgumentNullException(nameof(coordinate1));
            if (coordinate2 == null) throw new ArgumentNullException(nameof(coordinate2));

            return new Coordinate(coordinate1.X - coordinate2.X, coordinate1.Y - coordinate2.Y);
        }

        /// <inheritdoc cref="op_Subtraction"/>
        /// <param name="other"><see cref="Coordinate"/> to subtract</param>
        public Coordinate Subtract(Coordinate other) => this - other;

        /// <summary>
        /// Multiply coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator *(Coordinate coordinate1, Coordinate coordinate2)
        {
            if (coordinate1 == null) throw new ArgumentNullException(nameof(coordinate1));
            if (coordinate2 == null) throw new ArgumentNullException(nameof(coordinate2));

            return new Coordinate(coordinate1.X * coordinate2.X, coordinate1.Y * coordinate2.Y);
        }

        /// <inheritdoc cref="op_Multiply"/>
        /// <param name="other"><see cref="Coordinate"/> to multiply</param>
        public Coordinate Multiply(Coordinate other) => this * other;

        /// <summary>
        /// Divide coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator /(Coordinate coordinate1, Coordinate coordinate2)
        {
            if (coordinate1 == null) throw new ArgumentNullException(nameof(coordinate1));
            if (coordinate2 == null) throw new ArgumentNullException(nameof(coordinate2));

            return new Coordinate(coordinate1.X / coordinate2.X, coordinate1.Y / coordinate2.Y);
        }

        /// <inheritdoc cref="op_Division"/>
        /// <param name="other"><see cref="Coordinate"/> to divide on</param>
        public Coordinate Divide(Coordinate other) => this / other;

        #endregion

        #endregion
    }
}
