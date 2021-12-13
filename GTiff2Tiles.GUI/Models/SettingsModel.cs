#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA1308 // Normalize strings to uppercase

using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.GUI.Enums;

namespace GTiff2Tiles.GUI.Models
{
    /// <summary>
    /// Model of Settings.json
    /// </summary>
    public sealed class SettingsModel
    {
        #region Properties

        /// <summary>
        /// Path to input file
        /// </summary>
        [JsonPropertyName(nameof(InputFilePath))]
        public string InputFilePath { get; init; } = string.Empty;

        /// <summary>
        /// Path to output directory
        /// </summary>
        [JsonPropertyName(nameof(OutputDirectoryPath))]
        public string OutputDirectoryPath { get; init; } = string.Empty;

        /// <summary>
        /// Path to temp directory
        /// </summary>
        [JsonPropertyName(nameof(TempDirectoryPath))]
        public string TempDirectoryPath { get; init; } = string.Empty;

        /// <summary>
        /// Minimal zoom
        /// </summary>
        [JsonPropertyName(nameof(MinZ))]
        public int MinZ { get; init; }

        /// <summary>
        /// Maximal zoom
        /// </summary>
        [JsonPropertyName(nameof(MaxZ))]
        public int MaxZ { get; init; } = 17;

        /// <summary>
        /// Tile extension
        /// </summary>
        [JsonPropertyName(nameof(TileExtension))]
        public string TileExtension { get; init; } = "png";

        /// <summary>
        /// Tile extension
        /// </summary>
        internal TileExtension TargetTileExtension => TileExtension.ToLowerInvariant() switch
        {
            "webp" => Core.Enums.TileExtension.Webp,
            "jpg" => Core.Enums.TileExtension.Jpg,
            _ => Core.Enums.TileExtension.Png
        };

        /// <summary>
        /// Tile coordinate system
        /// </summary>
        [JsonPropertyName(nameof(CoordinateSystem))]
        public int CoordinateSystem { get; init; } = 4326;

        /// <summary>
        /// Tile coordinate system
        /// </summary>
        internal CoordinateSystem TargetCoordinateSystem => CoordinateSystem switch
        {
            3857 => Core.Enums.CoordinateSystem.Epsg3857,
            3587 => Core.Enums.CoordinateSystem.Epsg3857,
            3785 => Core.Enums.CoordinateSystem.Epsg3857,
            41001 => Core.Enums.CoordinateSystem.Epsg3857,
            54004 => Core.Enums.CoordinateSystem.Epsg3857,
            102113 => Core.Enums.CoordinateSystem.Epsg3857,
            102100 => Core.Enums.CoordinateSystem.Epsg3857,
            900913 => Core.Enums.CoordinateSystem.Epsg3857,
            _ => Core.Enums.CoordinateSystem.Epsg4326
        };

        /// <summary>
        /// Parse <see cref="Core.Enums.CoordinateSystem"/> enum into <see cref="int"/>
        /// </summary>
        /// <param name="coordinateSystem">Coordinate system to parse</param>
        /// <returns>Corresponding <see cref="int"/></returns>
        internal static int ParseCoordinateSystem(CoordinateSystem coordinateSystem) => coordinateSystem switch
        {
            Core.Enums.CoordinateSystem.Epsg4326 => 4326,
            Core.Enums.CoordinateSystem.Epsg3857 => 3857,
            _ => -1
        };

        /// <summary>
        /// Tile interpolation
        /// </summary>
        [JsonPropertyName(nameof(Interpolation))]
        public string Interpolation { get; init; } = "lanczos3";

        /// <summary>
        /// Tile interpolation
        /// </summary>
        internal NetVips.Enums.Kernel TargetInterpolation => Enum.Parse<NetVips.Enums.Kernel>(Interpolation, true);

        /// <summary>
        /// Number of bands in tile
        /// </summary>
        [JsonPropertyName(nameof(BandsCount))]
        public int BandsCount { get; init; } = 4;

        /// <summary>
        /// Is tiles tms compatible?
        /// </summary>
        [JsonPropertyName(nameof(TmsCompatible))]
        public bool TmsCompatible { get; init; }

        /// <summary>
        /// Do you want to create tilemapresource.xml?
        /// </summary>
        [JsonPropertyName(nameof(IsTmr))]
        public bool IsTmr { get; init; }

        /// <summary>
        /// Current theme
        /// </summary>
        [JsonPropertyName(nameof(Theme))]
        public string Theme { get; init; } = "dark";

        /// <summary>
        /// Current theme
        /// </summary>
        internal Theme TargetTheme => ThemeModel.GetTheme(Theme);

        /// <summary>
        /// Size of tile's side
        /// </summary>
        [JsonPropertyName(nameof(TileSideSize))]
        public int TileSideSize { get; init; } = 256;

        /// <summary>
        /// Do you want to calculate threads count automatically?
        /// </summary>
        [JsonPropertyName(nameof(IsAutoThreads))]
        public bool IsAutoThreads { get; init; } = true;

        /// <summary>
        /// Manual input of number of threads
        /// </summary>
        [JsonPropertyName(nameof(ThreadsCount))]
        public int ThreadsCount { get; init; } = Environment.ProcessorCount;

        /// <summary>
        /// Number of tiles to store in cache
        /// </summary>
        [JsonPropertyName(nameof(TileCache))]
        public int TileCache { get; init; } = 1000;

        /// <summary>
        /// Max size of input tiff to store in memory
        /// </summary>
        [JsonPropertyName(nameof(Memory))]
        public long Memory { get; init; } = 2147483649;

        /// <summary>
        /// Get the full path of Settings.json file
        /// </summary>
        internal static readonly string Location = Path.Combine(AppContext.BaseDirectory, "settings.json");

        #endregion

        /// <summary>
        /// Save the <see cref="SettingsModel"/> into file;
        /// <remarks><para/>Overwrites file if exists</remarks>
        /// </summary>
        /// <param name="settings"><see cref="SettingsModel"/> to write</param>
        /// <param name="path">Path to file, to save settings
        /// <remarks><para/>Uses <see cref="Location"/> by default</remarks></param>
        /// <returns></returns>
        internal static Task SaveAsync(SettingsModel settings, string path = null)
        {
            // SerializeAsync's stream doesn't overwrite file, so I don't use it
            string json = JsonSerializer.Serialize(settings);

            return File.WriteAllTextAsync(path ?? Location, json);
        }
    }
}

#pragma warning restore CA1308 // Normalize strings to uppercase
#pragma warning restore IDE0079 // Remove unnecessary suppression
