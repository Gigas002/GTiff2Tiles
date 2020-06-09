using System;

namespace GTiff2Tiles.Core.Exceptions.Image
{
    /// <inheritdoc />
    /// <summary>
    /// Exceptions, happened in Image.Raster.cs.
    /// </summary>
    public sealed class RasterException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="RasterException"/> object with passed error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public RasterException(string errorMessage) : base(errorMessage) { }

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="RasterException"/> object with passed error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public RasterException(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="RasterException"/> object without error message.
        /// </summary>
        public RasterException() { }

        #endregion
    }
}
