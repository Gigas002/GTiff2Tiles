namespace GTiff2Tiles.Core.Exceptions;

/// <inheritdoc />
public sealed class DirectoryException : Exception
{
    #region Constructors

    /// <inheritdoc />
    public DirectoryException(string message) : base(message) { }

    /// <inheritdoc />
    public DirectoryException(string message, Exception innerException) : base(message, innerException) { }

    /// <inheritdoc />
    public DirectoryException() { }

    #endregion
}