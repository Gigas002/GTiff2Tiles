using System;

namespace GTiff2Tiles.Core.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// Exceptions, happened in Image.Gdal.cs.
    /// </summary>
    public sealed class GdalException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="GdalException"/> object with passed error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public GdalException(string errorMessage) : base(errorMessage) { }

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="GdalException"/> object with passed error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public GdalException(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="GdalException"/> object without error message.
        /// </summary>
        public GdalException() { }

        #endregion
    }
}
