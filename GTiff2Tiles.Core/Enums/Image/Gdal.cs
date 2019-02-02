namespace GTiff2Tiles.Core.Enums.Image
{
    public static class Gdal
    {
        public const string Block = "Block";
        public const string Byte = "Byte";
        public const string Projection = "AUTHORITY[\"EPSG\",\"4326\"]]";
        public static readonly string[] RepairTifOptions =
        {
            "-overwrite", "-t_srs", "EPSG:4326", "-co", "TILED=YES", "-multi", "-s_srs", "EPSG:4326", "-srcnodata", "0",
            "-nosrcalpha", "-dstalpha", "0"
        };
        public const string TempFileName = "Temp";
    }
}
