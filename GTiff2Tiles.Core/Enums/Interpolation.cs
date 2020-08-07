// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Enums
{
    /// <summary>
    /// Represents the interpolation algorithms
    /// </summary>
    public enum Interpolation
    {
        /// <summary>
        /// Nearest-neighbour interpolation
        /// </summary>
        Nearest,

        /// <summary>
        /// Linear interpolation
        /// </summary>
        Linear,

        /// <summary>
        /// Cubic interpolation
        /// </summary>
        Cubic,

        /// <summary>
        /// Mitchell
        /// </summary>
        Mitchell,

        /// <summary>
        /// Two-lobe Lanczos
        /// </summary>
        Lanczos2,

        /// <summary>
        /// Three-lobe Lanczos
        /// </summary>
        Lanczos3
    }
}
