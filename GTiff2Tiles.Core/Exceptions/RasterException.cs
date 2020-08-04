using System;

namespace GTiff2Tiles.Core.Exceptions
{
    /// <inheritdoc />
    public sealed class RasterException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        public RasterException(string message) : base(message) { }

        /// <inheritdoc />
        public RasterException(string message, Exception innerException) : base(message, innerException) { }

        /// <inheritdoc />
        public RasterException() { }

        #endregion
    }
}
