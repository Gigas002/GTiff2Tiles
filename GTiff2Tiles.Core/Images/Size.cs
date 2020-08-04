using System;

// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// <see cref="IImage"/>'s size
    /// </summary>
    public sealed class Size : IEquatable<Size>
    {
        #region Properties/Constants

        /// <summary>
        /// Image's width
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Image's height
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// <see cref="IImage"/>'s resolution
        /// </summary>
        public int Resolution => Width * Height;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <see cref="Size"/>
        /// </summary>
        /// <param name="width"><see cref="Width"/></param>
        /// <param name="height"><see cref="Height"/></param>
        public Size(int width, int height)
        {
            if (width <= 0) throw new ArgumentNullException(nameof(width));
            if (height <= 0) throw new ArgumentNullException(nameof(height));

            (Width, Height) = (width, height);
        }

        #endregion

        #region Methods

        #region Bool compare overrides

        /// <inheritdoc />
        public override bool Equals(object size) => Equals(size as Size);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Width, Height);

        /// <inheritdoc />
        public bool Equals(Size other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Width == other.Width && Height == other.Height;
        }

        /// <summary>
        /// Check two <see cref="Size"/>s for equality
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>s are equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator ==(Size size1, Size size2) => size1?.Equals(size2) == true;

        /// <summary>
        /// Check two <see cref="Size"/>s for non-equality
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>s are not equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator !=(Size size1, Size size2) => !(size1 == size2);

        #endregion

        #region Math operations

        /// <summary>
        /// Sum <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator +(Size size1, Size size2)
        {
            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            return new Size(size1.Width + size2.Width, size1.Height + size2.Height);
        }

        /// <inheritdoc cref="op_Addition"/>
        /// <param name="other"><see cref="Size"/> to add</param>
        public Size Add(Size other) => this + other;

        /// <summary>
        /// Subtruct <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator -(Size size1, Size size2)
        {
            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            return new Size(size1.Width - size2.Width, size1.Height - size2.Height);
        }

        /// <inheritdoc cref="op_Subtraction"/>
        /// <param name="other"><see cref="Size"/> to subtract</param>
        public Size Subtract(Size other) => this - other;

        /// <summary>
        /// Multiply <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator *(Size size1, Size size2)
        {
            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            return new Size(size1.Width * size2.Width, size1.Height * size2.Height);
        }

        /// <inheritdoc cref="op_Multiply"/>
        /// <param name="other"><see cref="Size"/> to multiply</param>
        public Size Multiply(Size other) => this * other;

        /// <summary>
        /// Divide <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator /(Size size1, Size size2)
        {
            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            return new Size(size1.Width / size2.Width, size1.Height / size2.Height);
        }

        /// <inheritdoc cref="op_Division"/>
        /// <param name="other"><see cref="Size"/> to divide on</param>
        public Size Divide(Size other) => this / other;

        #endregion

        #endregion
    }
}
