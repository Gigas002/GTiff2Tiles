using System;

namespace GTiff2Tiles.NetCore.Core.Exceptions.Image
{
    /// <inheritdoc />
    /// <summary>
    /// Exceptions, happened in Image.Gdal.cs.
    /// </summary>
    public sealed class ImageException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="ImageException"/> object with passed error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public ImageException(string errorMessage) : base(errorMessage) { }

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="ImageException"/> object with passed error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public ImageException(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }

        #endregion
    }
}
