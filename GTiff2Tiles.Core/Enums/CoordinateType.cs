namespace GTiff2Tiles.Core.Enums
{
    //TODO: Consider replacing with CoordinateSystem
    /// <summary>
    /// Type of coordinates
    /// </summary>
    public enum CoordinateType
    {
        /// <summary>
        /// For EPSG:4326
        /// </summary>
        Geodetic,

        /// <summary>
        /// For EPSG:3857
        /// </summary>
        Mercator
    }
}
