using System;

namespace GTiff2Tiles.Core.Exceptions
{
    /// <inheritdoc />
    public sealed class GdalException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        public GdalException(string message) : base(message) { }

        /// <inheritdoc />
        public GdalException(string message, Exception innerException) : base(message, innerException) { }

        /// <inheritdoc />
        public GdalException() { }

        #endregion
    }
}
