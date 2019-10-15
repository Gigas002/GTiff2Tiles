namespace GTiff2Tiles.NetCore.Core.Enums
{
    /// <summary>
    /// Supported algorithms.
    /// </summary>
    public static class Algorithms
    {
        /// <summary>
        /// Crop input tif for each zoom.
        /// </summary>
        public const string Crop = "crop";

        /// <summary>
        /// Crop lowest zoom and join tiles from it.
        /// </summary>
        public const string Join = "join";
    }
}
