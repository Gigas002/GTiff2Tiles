using System;

namespace GTiff2Tiles.Core.Exceptions.Tile
{
    /// <inheritdoc />
    /// <summary>
    /// Exceptions, happened in Tile.TileTileTools.cs.
    /// </summary>
    public sealed class TileException : Exception
    {
        #region Constructors

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="TileException"/> object with passed error message.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        public TileException(string errorMessage) : base(errorMessage) { }

        /// <inheritdoc />
        /// <summary>
        /// Creates new <see cref="TileException"/> object with passed error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="innerException">Inner exception.</param>
        public TileException(string errorMessage, Exception innerException) : base(errorMessage, innerException) { }

        public TileException() { }

        #endregion
    }
}
