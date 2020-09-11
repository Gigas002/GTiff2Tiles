using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GTiff2Tiles.GUI.Models
{
    /// <summary>
    /// Model of Settings.json
    /// </summary>
    public sealed class SettingsModel
    {
        /// <summary>
        /// Current theme
        /// </summary>
        [JsonPropertyName(nameof(Theme))]
        public string Theme { get; set; }

        /// <summary>
        /// Size of tile's side
        /// </summary>
        [JsonPropertyName(nameof(TileSideSize))]
        public int TileSideSize { get; set; }

        /// <summary>
        /// Do you want to calculate threads count automatically?
        /// </summary>
        [JsonPropertyName(nameof(IsAutoThreads))]
        public bool IsAutoThreads { get; set; }

        /// <summary>
        /// Manual input of number of threads
        /// </summary>
        [JsonPropertyName(nameof(ThreadsCount))]
        public int ThreadsCount { get; set; }

        /// <summary>
        /// Number of tiles to store in cache
        /// </summary>
        [JsonPropertyName(nameof(TileCache))]
        public int TileCache { get; set; }

        /// <summary>
        /// Max size of input tiff to store in memory
        /// </summary>
        [JsonPropertyName(nameof(Memory))]
        public long Memory { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="theme">Theme</param>
        /// <param name="tileSideSize">Tile's side size</param>
        /// <param name="isAutoThreads">Is auto threads?</param>
        /// <param name="threadsCount">Threads count</param>
        /// <param name="tileCache">Tile cache</param>
        /// <param name="memory">Memory</param>
        public SettingsModel(string theme, int tileSideSize, bool isAutoThreads,
                             int threadsCount, int tileCache, long memory) =>
            (Theme, TileSideSize, IsAutoThreads, ThreadsCount, TileCache, Memory) =
            (theme, tileSideSize, isAutoThreads, threadsCount, tileCache, memory);

        /// <summary>
        /// Get the full path of Settings.json file
        /// </summary>
        internal static readonly string Location = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, "settings.json");

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

        /// <summary>
        /// Get the default <see cref="SettingsModel"/>
        /// in case something gone very wrong
        /// <remarks><para/>
        /// Default values:<para/>
        /// <see cref="Theme"/>="Dark"<para/>
        /// <see cref="TileSideSize"/>=256<para/>
        /// <see cref="IsAutoThreads"/>=true<para/>
        /// <see cref="ThreadsCount"/>=8<para/>
        /// <see cref="TileCache"/>=1000<para/>
        /// <see cref="Memory"/>=2Gb<para/>
        /// </remarks>
        /// </summary>
        /// <returns>Default <see cref="SettingsModel"/></returns>
        internal static SettingsModel Default => new SettingsModel("Dark", 256, true, 8, 1000, 2147483649);
    }
}
