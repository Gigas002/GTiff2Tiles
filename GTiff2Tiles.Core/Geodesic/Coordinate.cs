using System;

namespace GTiff2Tiles.Core.Geodesic
{
    /// <summary>
    /// Coordinate
    /// </summary>
    public class Coordinate : IEquatable<Coordinate>
    {
        //TODO: coordinate system?
        //public const string System

        #region Properties/Constants

        /// <summary>
        /// Longitude
        /// </summary>
        public readonly double Longitude;

        /// <summary>
        /// Latitude
        /// </summary>
        public readonly double Latitude;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates coordinate instance
        /// </summary>
        /// <param name="longitude">Longitude</param>
        /// <param name="latitude">Latitude</param>
        public Coordinate(double longitude, double latitude) => (Longitude, Latitude) = (longitude, latitude);

        #endregion

        #region Methods

        #region Equals

        /// <inheritdoc />
        public override bool Equals(object coordinate) => Equals(coordinate as Coordinate);

        /// <inheritdoc />
        public bool Equals(Coordinate other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Math.Abs(Longitude - other.Longitude) < double.Epsilon &&
                   Math.Abs(Latitude - other.Latitude) < double.Epsilon;
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Longitude, Latitude);

        /// <summary>
        /// Check two sizes for equality
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns><see langword="true"/> if coordinates are equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool? operator ==(Coordinate coordinate1, Coordinate coordinate2) =>
            coordinate1?.Equals(coordinate2);

        /// <summary>
        /// Check two sizes for non-equality
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns><see langword="true"/> if coordinates are not equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool? operator !=(Coordinate coordinate1, Coordinate coordinate2) =>
            !(coordinate1 == coordinate2);

        #endregion

        #region Math operations

        /// <summary>
        /// Sum coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator +(Coordinate coordinate1, Coordinate coordinate2) =>
            new Coordinate(coordinate1.Longitude + coordinate2.Longitude, coordinate1.Latitude + coordinate2.Latitude);

        /// <summary>
        /// Subtruct coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator -(Coordinate coordinate1, Coordinate coordinate2) =>
            new Coordinate(coordinate1.Longitude - coordinate2.Longitude, coordinate1.Latitude - coordinate2.Latitude);

        /// <summary>
        /// Multiply coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator *(Coordinate coordinate1, Coordinate coordinate2) =>
            new Coordinate(coordinate1.Longitude * coordinate2.Longitude, coordinate1.Latitude * coordinate2.Latitude);

        /// <summary>
        /// Divide coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static Coordinate operator /(Coordinate coordinate1, Coordinate coordinate2) =>
            new Coordinate(coordinate1.Longitude / coordinate2.Longitude, coordinate1.Latitude / coordinate2.Latitude);

        #endregion

        #endregion
    }
}
