// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Constants
{
    /// <summary>
    /// Represents the interpolation algorithms
    /// </summary>
    internal static class Interpolations
    {
        /// <summary>
        /// Nearest-neighbour interpolation
        /// </summary>
        internal const string Nearest = "nearest";

        /// <summary>
        /// Linear interpolation
        /// </summary>
        internal const string Linear = "linear";

        /// <summary>
        /// Cubic interpolation
        /// </summary>
        internal const string Cubic = "cubic";

        /// <summary>
        /// Mitchell
        /// </summary>
        internal const string Mitchell = "mitchell";

        /// <summary>
        /// Two-lobe Lanczos
        /// </summary>
        internal const string Lanczos2 = "lanczos2";

        /// <summary>
        /// Three-lobe Lanczos
        /// </summary>
        internal const string Lanczos3 = "lanczos3";
    }
}
