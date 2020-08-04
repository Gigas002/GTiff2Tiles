using System;

namespace GTiff2Tiles.Core.Exceptions
{
    /// <inheritdoc />
    public sealed class TileException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        public TileException(string message) : base(message) { }

        /// <inheritdoc />
        public TileException(string message, Exception innerException) : base(message, innerException) { }

        /// <inheritdoc />
        public TileException() { }

        #endregion
    }
}
