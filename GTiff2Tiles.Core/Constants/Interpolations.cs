namespace GTiff2Tiles.Core.Constants
{
    /// <summary>
    /// Represents the interpolation algorithms
    /// </summary>
    public static class Interpolations
    {
        /// <summary>Nearest-neighbour interpolation.</summary>
        public const string Nearest = "nearest";
        /// <summary>Linear interpolation.</summary>
        public const string Linear = "linear";
        /// <summary>Cubic interpolation.</summary>
        public const string Cubic = "cubic";
        /// <summary>Mitchell</summary>
        public const string Mitchell = "mitchell";
        /// <summary>Two-lobe Lanczos.</summary>
        public const string Lanczos2 = "lanczos2";
        /// <summary>Three-lobe Lanczos.</summary>
        public const string Lanczos3 = "lanczos3";
    }
}
