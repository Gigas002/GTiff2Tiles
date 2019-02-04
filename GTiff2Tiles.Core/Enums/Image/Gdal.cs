namespace GTiff2Tiles.Core.Enums.Image
{
    /// <summary>
    /// Static class with Gdal enums.
    /// </summary>
    public struct Gdal
    {
        #region GdalInfo

        /// <summary>
        /// Input image is tiled.
        /// </summary>
        public const string Block = "Block";

        /// <summary>
        /// todo I don't really remember what that means.
        /// </summary>
        public const string Byte = "Byte";

        /// <summary>
        /// EPSG:4326 (WGS84) projection string.
        /// </summary>
        public const string Projection = "AUTHORITY[\"EPSG\",\"4326\"]]";

        #endregion

        #region GdalWarp

        /// <summary>
        /// Options for GdalWarp.
        /// </summary>
        public static readonly string[] RepairTifOptions =
        {
            "-overwrite", "-t_srs", "EPSG:4326", "-co", "TILED=YES", "-multi", "-s_srs", "EPSG:4326", "-srcnodata", "0",
            "-nosrcalpha", "-dstalpha", "0"
        };

        /// <summary>
        /// Name for temporary (converted) file.
        /// </summary>
        public const string TempFileName = "Temp";

        #endregion
    }
}
