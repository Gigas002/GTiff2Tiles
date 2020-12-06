using System;

// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// <see cref="Size"/> of any image
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
        /// Image's resolution
        /// </summary>
        public int Resolution => Width * Height;

        /// <summary>
        /// Shows if this tile is square (width == height)
        /// </summary>
        public bool IsSquare => Width == Height;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <see cref="Size"/>
        /// </summary>
        /// <param name="width"><see cref="Width"/>
        /// <remarks><para/>Should be > 0</remarks></param>
        /// <param name="height"><see cref="Height"/>
        /// <remarks><para/>Should be > 0</remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Size(int width, int height)
        {
            #region Preconditions checks

            if (width <= 0) throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0) throw new ArgumentOutOfRangeException(nameof(height));

            #endregion

            (Width, Height) = (width, height);
        }

        #endregion

        #region Methods

        #region Bool compare overrides

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as Size);

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
        /// <see langword="false"/> otherwise</returns>
        public static bool operator ==(Size size1, Size size2) => size1?.Equals(size2) ?? size2 is null;

        /// <summary>
        /// Check two <see cref="Size"/>s for non-equality
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>s are not equal;
        /// <see langword="false"/> otherwise</returns>
        public static bool operator !=(Size size1, Size size2) => !(size1 == size2);

        #endregion

        #region Math operations

        /// <summary>
        /// Sum <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        /// <exception cref="ArgumentNullException"/>
        public static Size operator +(Size size1, Size size2)
        {
            #region Preconditions checks

            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            #endregion

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
        /// <exception cref="ArgumentNullException"/>
        public static Size operator -(Size size1, Size size2)
        {
            #region Preconditions checks

            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            #endregion

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
        /// <exception cref="ArgumentNullException"/>
        public static Size operator *(Size size1, Size size2)
        {
            #region Preconditions checks

            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            #endregion

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
        /// <exception cref="ArgumentNullException"/>
        public static Size operator /(Size size1, Size size2)
        {
            #region Preconditions checks

            if (size1 == null) throw new ArgumentNullException(nameof(size1));
            if (size2 == null) throw new ArgumentNullException(nameof(size2));

            #endregion

            return new Size(size1.Width / size2.Width, size1.Height / size2.Height);
        }

        /// <inheritdoc cref="op_Division"/>
        /// <param name="other"><see cref="Size"/> to divide on</param>
        public Size Divide(Size other) => this / other;

        #endregion

        #endregion
    }
}
