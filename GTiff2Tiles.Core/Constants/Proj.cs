namespace GTiff2Tiles.Core.Constants
{
    /// <summary>
    /// Proj constants
    /// </summary>
    public static class Proj
    {
        /// <summary>
        /// +proj=longlat
        /// </summary>
        public const string LongLat = "+proj=longlat";

        /// <summary>
        /// For EPSG:4326 AND World Mercator, BUT not Spherical Mercator (EPSG:3857)
        /// </summary>
        public const string Wgs84 = "+datum=WGS84";
    }
}
