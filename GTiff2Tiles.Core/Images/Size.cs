using System;

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
        public Size(int width, int height) => (Width, Height) = (width, height);

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
        public static bool operator ==(Size size1, Size size2) => size1 != null && size1.Equals(size2);

        /// <summary>
        /// Check two <see cref="Size"/>s for non-equality
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>s are not equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator !=(Size size1, Size size2) => !(size1 == size2);

        /// <summary>
        /// Check if <see cref="Size"/>1 is lesser, then <see cref="Size"/>2
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>1 is lesser;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator <(Size size1, Size size2) =>
            size1.Width < size2.Width && size1.Height < size2.Height;

        /// <summary>
        /// Check if <see cref="Size"/>1 is bigger, then <see cref="Size"/>2
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>1 is bigger;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator >(Size size1, Size size2) =>
            size1.Width > size2.Width && size1.Height > size2.Height;

        /// <summary>
        /// Check if <see cref="Size"/>1 is lesser or equal, then <see cref="Size"/>2
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>1 is lesser or equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator <=(Size size1, Size size2) =>
            size1.Width <= size2.Width && size1.Height <= size2.Height;

        /// <summary>
        /// Check if <see cref="Size"/>1 is bigger or equal, then <see cref="Size"/>2
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns><see langword="true"/> if <see cref="Size"/>1 is bigger or equal;
        /// <see langword="false"/>otherwise</returns>
        public static bool operator >=(Size size1, Size size2) =>
            size1.Width >= size2.Width && size1.Height >= size2.Height;

        #endregion

        #region Math operations

        /// <summary>
        /// Sum <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator +(Size size1, Size size2) => new Size(size1.Width + size2.Width, size1.Height + size2.Height);

        /// <summary>
        /// Subtruct <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator -(Size size1, Size size2) => new Size(size1.Width - size2.Width, size1.Height - size2.Height);

        /// <summary>
        /// Multiply <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator *(Size size1, Size size2) => new Size(size1.Width * size2.Width, size1.Height * size2.Height);

        /// <summary>
        /// Divide <see cref="Size"/>s
        /// </summary>
        /// <param name="size1"><see cref="Size"/> 1</param>
        /// <param name="size2"><see cref="Size"/> 2</param>
        /// <returns>New <see cref="Size"/></returns>
        public static Size operator /(Size size1, Size size2) => new Size(size1.Width / size2.Width, size1.Height / size2.Height);

        #endregion

        #endregion
    }
}
