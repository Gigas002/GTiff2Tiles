namespace GTiff2Tiles.Core.Enums.Image
{
    /// <summary>
    /// Static class with Gdal constants.
    /// </summary>
    public static class Gdal
    {
        #region Proj4

        /// <summary>
        /// +proj=longlat
        /// </summary>
        public const string LongLat = "+proj=longlat";

        /// <summary>
        /// +datum=WGS84
        /// </summary>
        public const string Wgs84 = "+datum=WGS84";

        #endregion

        #region GdalInfo

        /// <summary>
        /// Type=Byte
        /// </summary>
        public const string Byte = "Type=Byte";

        #endregion

        #region GdalWarp

        /// <summary>
        /// Options for GdalWarp.
        /// </summary>
        public static readonly string[] RepairTifOptions =
        {
            "-overwrite", "-t_srs", "EPSG:4326", "-multi", "-srcnodata", "0",
            "-of", "GTiff", "-ot", "Byte"
            //"-co", "TILED=YES",
        };

        /// <summary>
        /// Name for temporary (converted) file.
        /// </summary>
        public const string TempFileName = "Temp";

        #endregion
    }
}
