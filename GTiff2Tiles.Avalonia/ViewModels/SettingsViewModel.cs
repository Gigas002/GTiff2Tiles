using System.Collections.ObjectModel;
using Avalonia.Platform.Storage;
using ReactiveUI;
using GTiff2Tiles.Avalonia.Models;
using GTiff2Tiles.Core.Enums;
using static NetVips.Enums;

#pragma warning disable CA1031 // Do not catch general exception types

namespace GTiff2Tiles.Avalonia.ViewModels;

/// <inheritdoc/>
public class SettingsViewModel : ViewModelBase
{
    #region Properties

    #region Data settings
    
    private int _rasterTileSize;

    /// <summary>
    /// Raster tile size
    /// </summary>
    public int RasterTileSize
    {
        get => _rasterTileSize;
        set
        {
            value = value <= 0 ? 256 : value;

            this.RaiseAndSetIfChanged(ref _rasterTileSize, value);
        }
    }

    #region RasterTileExtension

    /// <summary>
    /// Possible raster tile extension
    /// </summary>
    public ObservableCollection<ComboBoxItemModel> RasterTileExtensions { get; } = new()
    {
        new ComboBoxItemModel(TileExtension.Png),
        new ComboBoxItemModel(TileExtension.Jpg),
        new ComboBoxItemModel(TileExtension.Webp)
    };

    private ComboBoxItemModel _rasterTileExtension;

    /// <summary>
    /// Selected raster tile extension
    /// </summary>
    public ComboBoxItemModel RasterTileExtension
    {
        get => _rasterTileExtension;
        set => this.RaiseAndSetIfChanged(ref _rasterTileExtension, value);
    }

    #endregion

    #region RasterTileInterpolation

    /// <summary>
    /// Possible raster tile interpolations
    /// </summary>
    public ObservableCollection<ComboBoxItemModel> RasterTileInterpolations { get; } = new()
    {
        new ComboBoxItemModel(Kernel.Linear),
        new ComboBoxItemModel(Kernel.Nearest),
        new ComboBoxItemModel(Kernel.Mitchell),
        new ComboBoxItemModel(Kernel.Cubic),
        new ComboBoxItemModel(Kernel.Lanczos2),
        new ComboBoxItemModel(Kernel.Lanczos3)
    };

    private ComboBoxItemModel _rasterTileInterpolation;

    /// <summary>
    /// Selected raster tile interpolation
    /// </summary>
    public ComboBoxItemModel RasterTileInterpolation
    {
        get => _rasterTileInterpolation;
        set => this.RaiseAndSetIfChanged(ref _rasterTileInterpolation, value);
    }

    #endregion

    #region BandsCount

    private int _bandsCount;

    /// <summary>
    /// Bands count
    /// </summary>
    public int BandsCount
    {
        get => _bandsCount;
        set => this.RaiseAndSetIfChanged(ref _bandsCount, value);
    }

    #endregion

    #region TmsCompatible

    private bool _tmsCompatible;

    /// <summary>
    /// Tms compatible
    /// </summary>
    public bool TmsCompatible
    {
        get => _tmsCompatible;
        set => this.RaiseAndSetIfChanged(ref _tmsCompatible, value);
    }

    #endregion

    #region CoordinateSystem

    /// <summary>
    /// Possible coorinate systems
    /// </summary>
    public ObservableCollection<ComboBoxItemModel> CoordinateSystems { get; } = new()
    {
        new ComboBoxItemModel(CoordinateSystem.Epsg4326),
        new ComboBoxItemModel(CoordinateSystem.Epsg3857)
    };

    private ComboBoxItemModel _selectedCoordinateSystem;

    /// <summary>
    /// Selected coordinate system
    /// </summary>
    public ComboBoxItemModel SelectedCoordinateSystem
    {
        get => _selectedCoordinateSystem;
        set => this.RaiseAndSetIfChanged(ref _selectedCoordinateSystem, value);
    }

    #endregion

    #endregion

    #region Performance settings

    #region TileCachePath

    private string _tileCachePath;

    /// <summary>
    /// Tile cache path
    /// </summary>
    public string TileCachePath
    {
        get => _tileCachePath;
        set => this.RaiseAndSetIfChanged(ref _tileCachePath, value);
    }

    #endregion

    #region MaxTiffMemoryCache

    private long _maxTiffMemoryCache;

    /// <summary>
    /// Max tiff memory cache
    /// </summary>
    public long MaxTiffMemoryCache
    {
        get => _maxTiffMemoryCache;
        set => this.RaiseAndSetIfChanged(ref _maxTiffMemoryCache, value);
    }

    #endregion

    #region TileCacheCount

    private int _tileCacheCount;

    /// <summary>
    /// Tile cache count
    /// </summary>
    public int TileCacheCount
    {
        get => _tileCacheCount;
        set
        {
            value = value <= 0 ? 1000 : value;

            this.RaiseAndSetIfChanged(ref _tileCacheCount, value);
        }
    }

    #endregion

    #region MinimalBytesCount

    private int _minimalBytesCount;

    /// <summary>
    /// Minimal bytes count
    /// </summary>
    public int MinimalBytesCount
    {
        get => _minimalBytesCount;
        set
        {
            value = value <= 0 ? 355 : value;

            this.RaiseAndSetIfChanged(ref _minimalBytesCount, value);
        }
    }

    #endregion

    #region ThrowOnOverride

    private bool _throwOnOverride;

    /// <summary>
    /// Throw on override?
    /// </summary>
    public bool ThrowOnOverride
    {
        get => _throwOnOverride;
        set => this.RaiseAndSetIfChanged(ref _throwOnOverride, value);
    }

    #endregion

    #region ThreadsCount

    private int _threadsCount;

    /// <summary>
    /// Threads count
    /// </summary>
    public int ThreadsCount
    {
        get => _threadsCount;
        set => this.RaiseAndSetIfChanged(ref _threadsCount, value);
    }

    #endregion

    #endregion

    #region Localizable

    #region Data settings

    /// <summary>
    /// DATA SETTINGS
    /// </summary>
    public static string DataSettingsText => "DATA SETTINGS";

    /// <summary>
    /// Raster tile size
    /// </summary>
    public static string RasterTileSizeTip => "Raster tile size";

    /// <summary>
    /// Raster tile extension
    /// </summary>
    public static string RasterTileExtensionTip => "Raster tile extension";

    /// <summary>
    /// Raster tile interpolation
    /// </summary>
    public static string RasterTileInterpolationTip => "Raster tile interpolation";

    /// <summary>
    /// Bands count
    /// </summary>
    public static string BandsCountTextBlock => "Bands count";

    /// <summary>
    /// Tms compatible
    /// </summary>
    public static string TmsCompatibleText => "Tms compatible";

    /// <summary>
    /// Coordinate systems
    /// </summary>
    public static string CoordinateSystemTip => "Coordinate system";

    #endregion

    #region Performance settings

    /// <summary>
    /// PERFORMANCE SETTINGS
    /// </summary>
    public static string PerformanceSettingsText => "PERFORMANCE SETTINGS";

    /// <summary>
    /// Tile cache path
    /// </summary>
    public static string TileCachePathTip => "Tile cache path";

    /// <summary>
    /// Max tiff memory cache
    /// </summary>
    public static string MaxTiffMemoryCacheTip => "Max tiff memory cache";

    /// <summary>
    /// Tile cache count
    /// </summary>
    public static string TileCacheCountTip => "Tile cache count";

    /// <summary>
    /// Minimal bytes count
    /// </summary>
    public static string MinimalBytesCountTip => "Minimal bytes count";

    /// <summary>
    /// Throw on override
    /// </summary>
    public static string ThrowOnOverrideText => "Throw on override";

    /// <summary>
    /// Thread count
    /// </summary>
    public static string ThreadsCountText => "Threads count";

    #endregion

    /// <summary>
    /// Set to 0 or lesser for auto
    /// </summary>
    public static string ZeroOrLesserTip => "Set to 0 or lesser for auto";

    /// <summary>
    /// Save settings
    /// </summary>
    public static string SaveSettingsText => "Save settings";

    /// <summary>
    /// Load settings
    /// </summary>
    public static string LoadSettingsText => "Load settings";

    private string _currentSettingsName;

    /// <summary>
    /// Current settings name
    /// </summary>
    public string CurrentSettingsName
    {
        get => _currentSettingsName;
        set
        {
            var str = $"Current settings file: {value}";
            this.RaiseAndSetIfChanged(ref _currentSettingsName, str);
        }
    }

    #endregion

    /// <summary>
    /// Vjik.GUI settings;
    /// init on SettingsViewModel ctor or LoadSettings method
    /// </summary>
    public static SettingsModel Settings { get; private set; }

    private static string SettingsPath { get; set; } = "settings.json";

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public SettingsViewModel() => LoadSettings();

    #endregion

    #region Methods

    private static ComboBoxItemModel FindEnumInCollection<T>(IEnumerable<ComboBoxItemModel> inputCollection, T value)
        where T : struct, Enum => inputCollection.FirstOrDefault(t => t.GetRealContent<T>().Equals(value));

    /// <summary>
    /// Save settings
    /// </summary>
    public async Task SaveSettings()
    {
        Settings.RasterTileSize = RasterTileSize;
        Settings.RasterTileExtension = RasterTileExtension.GetRealContent<TileExtension>();
        Settings.RasterTileInterpolation = RasterTileInterpolation.GetRealContent<Kernel>();
        Settings.BandsCount = BandsCount;
        Settings.TmsCompatible = TmsCompatible;
        Settings.CoordinateSystem = SelectedCoordinateSystem.GetRealContent<CoordinateSystem>();

        Settings.MaxTiffMemoryCache = MaxTiffMemoryCache;
        Settings.TileCacheCount = TileCacheCount;
        Settings.MinimalBytesCount = MinimalBytesCount;
        Settings.ThrowOnOverride = ThrowOnOverride;
        Settings.ThreadsCount = ThreadsCount;

        try
        {
            Settings.Save(SettingsPath);
        }
        catch (Exception exception)
        {
            await MessageBoxViewModel.ShowAsync(exception).ConfigureAwait(true);

            return;
        }

        await MessageBoxViewModel.ShowAsync("Saved successfully", false).ConfigureAwait(true);
    }

    /// <summary>
    /// Load settings file on selected path
    /// </summary>
    public async Task LoadSettingsButton()
    {
        var paths = await App.AppMainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()).ConfigureAwait(true);

        if (paths.Count <= 0) return;

        var path = paths[0].Path.AbsolutePath;

        try
        {
            if (Path.GetExtension(path) != ".json") throw new ArgumentOutOfRangeException("Wrong settings file extension");

            SettingsPath = path;

            LoadSettings();
        }
        catch (Exception exception)
        {
            await MessageBoxViewModel.ShowAsync(exception).ConfigureAwait(true);

            return;
        }

        await MessageBoxViewModel.ShowAsync("Loaded successfully", false).ConfigureAwait(true);
    }

    /// <summary>
    /// Load settings
    /// </summary>
    public void LoadSettings()
    {
        Settings = SettingsModel.Load(SettingsPath);

        RasterTileSize = Settings.RasterTileSize;
        RasterTileExtension = FindEnumInCollection(RasterTileExtensions, Settings.RasterTileExtension);
        RasterTileInterpolation = FindEnumInCollection(RasterTileInterpolations, Settings.RasterTileInterpolation);
        BandsCount = Settings.BandsCount;
        TmsCompatible = Settings.TmsCompatible;
        SelectedCoordinateSystem = FindEnumInCollection(CoordinateSystems, Settings.CoordinateSystem);

        MaxTiffMemoryCache = Settings.MaxTiffMemoryCache;
        TileCacheCount = Settings.TileCacheCount;
        MinimalBytesCount = Settings.MinimalBytesCount;
        ThrowOnOverride = Settings.ThrowOnOverride;
        ThreadsCount = Settings.ThreadsCount;

        CurrentSettingsName = Path.GetFileName(SettingsPath);
    }

    #endregion
}
