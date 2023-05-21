using System.Text.Json;
using System.Text.Json.Serialization;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Avalonia.JsonConverters;
using static NetVips.Enums;

#pragma warning disable CA1031 // Do not catch general exception types

namespace GTiff2Tiles.Avalonia.Models;

// TODO: TMR

/// <summary>
/// Basic implementation of ISettings
/// </summary>
public class SettingsModel
{
    #region Properties

    #region Data settings

    [JsonPropertyName("raster_tile_size")]
    public int RasterTileSize { get; set; } = 256;

    [JsonPropertyName("raster_tile_extension")]
    [JsonConverter(typeof(StringToEnumJsonConverter<TileExtension>))]
    public TileExtension RasterTileExtension { get; set; } = TileExtension.Webp;

    [JsonPropertyName("raster_tile_interpolation")]
    [JsonConverter(typeof(StringToEnumJsonConverter<Kernel>))]
    public Kernel RasterTileInterpolation { get; set; } = Kernel.Lanczos3;

    [JsonPropertyName("bands_count")]
    public int BandsCount { get; set; } = RasterTile.DefaultBandsCount;

    [JsonPropertyName("tms_compatible")]
    public bool TmsCompatible { get; set; }

    [JsonPropertyName("coordinate_system")]
    [JsonConverter(typeof(StringToEnumJsonConverter<CoordinateSystem>))]
    public CoordinateSystem CoordinateSystem { get; set; } = CoordinateSystem.Epsg4326;

    #endregion

    #region Performance settings

    [JsonPropertyName("max_tiff_memory_cache")]
    public long MaxTiffMemoryCache { get; set; } = 2147483648;

    [JsonPropertyName("tile_cache_count")]
    public int TileCacheCount { get; set; } = 1000;

    [JsonPropertyName("minimal_bytes_count")]
    public int MinimalBytesCount { get; set; } = 356;

    [JsonPropertyName("throw_on_override")]
    public bool ThrowOnOverride { get; set; } = true;

    [JsonPropertyName("threads_count")]
    public int ThreadsCount { get; set; }

    #endregion

    #region Additional non-json

    /// <summary>
    /// Is auto bands?
    /// </summary>
    [JsonIgnore]
    public bool IsAutoBands => BandsCount <= 0;

    /// <summary>
    /// Is auto threads?
    /// </summary>
    [JsonIgnore]
    public bool IsAutoThreads => ThreadsCount <= 0;

    /// <summary>
    /// Default path
    /// </summary>
    [JsonIgnore]
    public const string Path = "settings.json";

    /// <summary>
    /// Serializer options
    /// </summary>
    [JsonIgnore]
    public static JsonSerializerOptions SerializerOptions { get; set; } = new()
    {
        //DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        WriteIndented = true
    };

    #endregion

    #endregion

    #region Methods

    /// <summary>
    /// Load settings from file or load default
    /// </summary>
    /// <param name="path">Path to settings file</param>
    /// <returns>Settings object</returns>
    public static SettingsModel Load(string path = null)
    {
        path = string.IsNullOrWhiteSpace(path) ? Path : path;

        try
        {
            var json = File.ReadAllText(path);

            return JsonSerializer.Deserialize<SettingsModel>(json);
        }
        catch (Exception)
        {
            return new SettingsModel();
        }
    }

    /// <summary>
    /// Save or update settings file
    /// </summary>
    /// <param name="path">Path to settings file</param>
    public void Save(string path = null)
    {
        path = string.IsNullOrWhiteSpace(path) ? Path : path;

        var json = JsonSerializer.Serialize(this, SerializerOptions);
        File.WriteAllText(path, json);
    }

    public object Clone() => new SettingsModel
    {
        RasterTileSize = RasterTileSize,
        RasterTileExtension = RasterTileExtension,
        RasterTileInterpolation = RasterTileInterpolation,
        BandsCount = BandsCount,
        TmsCompatible = TmsCompatible,
        CoordinateSystem = CoordinateSystem,
        
        MaxTiffMemoryCache = MaxTiffMemoryCache,
        TileCacheCount = TileCacheCount,
        MinimalBytesCount = MinimalBytesCount,
        ThrowOnOverride = ThrowOnOverride,
        ThreadsCount = ThreadsCount,
    };
    
    #endregion
}
