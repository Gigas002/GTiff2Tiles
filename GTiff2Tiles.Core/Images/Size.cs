using System;

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// Image size
    /// </summary>
    public sealed class Size : IEquatable<Size>
    {
        #region Properties/Constants

        /// <summary>
        /// Image's width
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Image's height
        /// </summary>
        public readonly int Height;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates size instance
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        public Size(int width, int height) => (Width, Height) = (width, height);

        #endregion

        #region Methods

        /// <summary>
        /// Gets resolution
        /// </summary>
        /// <returns>Resolution of this size</returns>
        public long GetResoultion() => Width * Height;

        #region Equals

        /// <inheritdoc />
        public override bool Equals(object size) => Equals(size as Size);

        /// <inheritdoc />
        public bool Equals(Size other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Width == other.Width && Height == other.Height;
        }

        /// <summary>
        /// Check two sizes for equality
        /// </summary>
        /// <param name="size1">Size 1</param>
        /// <param name="size2">Size 2</param>
        /// <returns><see langword="true"/> if sizes are equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool? operator ==(Size size1, Size size2) => size1?.Equals(size2);

        /// <summary>
        /// Check two sizes for non-equality
        /// </summary>
        /// <param name="size1">Size 1</param>
        /// <param name="size2">Size 2</param>
        /// <returns><see langword="true"/> if sizes are not equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool? operator !=(Size size1, Size size2) => !(size1 == size2);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Width, Height);

        #endregion

        #endregion
    }
}
