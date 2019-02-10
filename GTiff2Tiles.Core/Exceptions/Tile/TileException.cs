using System;

namespace GTiff2Tiles.Core.Exceptions.Tile
{
    /// <summary>
    /// Exceptions, happened in Tile.Tile.cs.
    /// </summary>
    public class TileException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="Exception"/> object with passed error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public TileException(string errorMessage) : base(errorMessage)
        { }

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="Exception"/> object with passed error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public TileException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
        { }

        #endregion
    }
}
