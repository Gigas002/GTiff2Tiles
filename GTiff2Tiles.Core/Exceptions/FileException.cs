using System;

namespace GTiff2Tiles.Core.Exceptions
{
    /// <inheritdoc />
    public sealed class FileException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        public FileException(string message) : base(message) { }

        /// <inheritdoc />
        public FileException(string message, Exception innerException) : base(message, innerException) { }

        /// <inheritdoc />
        public FileException() { }

        #endregion
    }
}
