#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using GTiff2Tiles.Core;
using Prism.Mvvm;
using Prism.Commands;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.GUI.Localization;
using GTiff2Tiles.GUI.Models;
using GTiff2Tiles.GUI.Views;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;
using Theme = GTiff2Tiles.GUI.Enums.Theme;
using Size = GTiff2Tiles.Core.Images.Size;

// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.GUI.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="MainView"/>
    /// </summary>
    public class MainViewModel : BindableBase
    {
        #region Properties

        #region DialogHost / Main grid

        /// <summary>
        /// Identifier of DialogHost on <see cref="MainView"/>
        /// </summary>
        public static string DialogHostId => "DialogHost";

        private bool _isMainGridEnabled;

        /// <summary>
        /// Change main grid's state
        /// </summary>
        public bool IsMainGridEnabled
        {
            get => _isMainGridEnabled;
            set => SetProperty(ref _isMainGridEnabled, value);
        }

        #endregion

        #region Input file / Grid.Row=0

        private string _inputFilePath;

        /// <summary>
        /// Input file path
        /// </summary>
        public string InputFilePath
        {
            get => _inputFilePath;
            set => SetProperty(ref _inputFilePath, value);
        }

        /// <summary>
        /// Hint for InputFile TextBox
        /// </summary>
        public static string InputFileHint => Strings.InputFileHint;

        /// <summary>
        /// InputFileButton DelegateCommand
        /// </summary>
        public DelegateCommand InputFileButtonCommand { get; }

        #endregion

        #region Output directory / Grid.Row=2

        private string _outputDirectoryPath;

        /// <summary>
        /// Output directory path
        /// </summary>
        public string OutputDirectoryPath
        {
            get => _outputDirectoryPath;
            set => SetProperty(ref _outputDirectoryPath, value);
        }

        /// <summary>
        /// Hint for OutputDirectory TextBox
        /// </summary>
        public static string OutputDirectoryHint => Strings.OutputDirectoryHint;

        /// <summary>
        /// OutputDirectoryButton DelegateCommand
        /// </summary>
        public DelegateCommand OutputDirectoryButtonCommand { get; }

        #endregion

        #region Temp directory / Grid.Row=4

        private string _tempDirectoryPath;

        /// <summary>
        /// Temp directory path
        /// </summary>
        public string TempDirectoryPath
        {
            get => _tempDirectoryPath;
            set => SetProperty(ref _tempDirectoryPath, value);
        }

        /// <summary>
        /// Hint for TempDirectory TextBox
        /// </summary>
        public static string TempDirectoryHint => Strings.TempDirectoryHint;

        /// <summary>
        /// TempDirectoryButton DelegateCommand
        /// </summary>
        public DelegateCommand TempDirectoryButtonCommand { get; }

        #endregion

        #region Zooms / Grid.Row=6

        private int _minZ;

        /// <summary>
        /// Minimal zoom
        /// </summary>
        public int MinZ
        {
            get => _minZ;
            set => SetProperty(ref _minZ, value);
        }

        /// <summary>
        /// Hint for MinZ TextBox
        /// </summary>
        public static string MinZHint => Strings.MinZHint;

        private int _maxZ;

        /// <summary>
        /// Maximal zoom
        /// </summary>
        public int MaxZ
        {
            get => _maxZ;
            set => SetProperty(ref _maxZ, value);
        }

        /// <summary>
        /// Hint for MaxZ TextBox
        /// </summary>
        public static string MaxZHint => Strings.MaxZHint;

        #endregion

        #region Tile extension / Coordinate system / Grid.Row=8

        private TileExtension _targetTileExtension;

        /// <summary>
        /// Target tile's extension
        /// </summary>
        public TileExtension TargetTileExtension
        {
            get => _targetTileExtension;
            set => SetProperty(ref _targetTileExtension, value);
        }

        /// <summary>
        /// Collection of supported <see cref="TileExtension"/>s
        /// </summary>
        public ObservableCollection<TileExtension> TileExtensions { get; } = new ObservableCollection<TileExtension>();

        /// <summary>
        /// Hint for Extensions ComboBox
        /// </summary>
        public static string TileExtensionsHint => Strings.TileExtensionsHint;

        private CoordinateSystem _targetCoordinateSystem;

        /// <summary>
        /// Target tile's coordinate system
        /// </summary>
        public CoordinateSystem TargetCoordinateSystem
        {
            get => _targetCoordinateSystem;
            set => SetProperty(ref _targetCoordinateSystem, value);
        }

        /// <summary>
        /// Hint for CoordinateSystems ComboBox
        /// TODO: strings
        /// </summary>
        public static string CoordinateSystemsHint => "Target coordinate system";

        /// <summary>
        /// Collection of supprted <see cref="CoordinateSystem"/>s
        /// </summary>
        public ObservableCollection<CoordinateSystem> CoordinateSystems { get; } = new ObservableCollection<CoordinateSystem>();

        #endregion

        #region Interpolation / Bands count / Grid.Row=10

        private Interpolation _targetInterpolation;

        /// <summary>
        /// <see cref="Interpolation"/> of ready tiles
        /// </summary>
        public Interpolation TargetInterpolation
        {
            get => _targetInterpolation;
            set => SetProperty(ref _targetInterpolation, value);
        }

        /// <summary>
        /// Hint for Interpolation TextBox
        /// TODO: strings
        /// </summary>
        public static string InterpolationsHint => "Tile interpolation";

        /// <summary>
        /// Collection of supprted <see cref="Interpolation"/>s
        /// </summary>
        public ObservableCollection<Interpolation> Interpolations { get; } = new ObservableCollection<Interpolation>();

        private int _bandsCount;

        /// <summary>
        /// Number of bands in ready tiles
        /// </summary>
        public int BandsCount
        {
            get => _bandsCount;
            set => SetProperty(ref _bandsCount, value);
        }

        /// <summary>
        /// Hint for Bands TextBox
        /// TODO: strings
        /// </summary>
        public static string BandsHint => "Bands count";

        #endregion

        #region Tms compatible / Grid.Row=12

        private bool _tmsCompatible;

        /// <summary>
        /// Shows if you want to create tms-compatible tiles
        /// </summary>
        public bool TmsCompatible
        {
            get => _tmsCompatible;
            set => SetProperty(ref _tmsCompatible, value);
        }

        /// <summary>
        /// Text near tms check box
        /// </summary>
        public static string TmsCheckBoxContent => Strings.TmsCheckBoxContent;

        #endregion

        #region Expander with additional settings / Grid.Row=14

        /// <summary>
        /// Expander's name
        /// </summary>
        public static string ExpanderName => "Additional settings";

        #region Theme

        private Theme _theme;

        /// <summary>
        /// Theme for DialogHost
        /// <remarks><para/>Automatically changes on set</remarks>
        /// </summary>
        public Theme Theme
        {
            get => _theme;
            set
            {
                SetProperty(ref _theme, value);

                // Set theme
                BaseDialogTheme = ThemeModel.SetTheme(value);
            }
        }

        /// <summary>
        /// Collection of supported themes
        /// </summary>
        public ObservableCollection<Theme> Themes { get; } = new ObservableCollection<Theme>();

        private BaseTheme _baseDialogTheme;

        /// <summary>
        /// Value in DialogHost only
        /// </summary>
        public BaseTheme BaseDialogTheme
        {
            get => _baseDialogTheme;
            set => SetProperty(ref _baseDialogTheme, value);
        }

        /// <summary>
        /// Hint for Themes combobox
        /// TODO: strings
        /// </summary>
        public static string ThemesHint => "Theme";

        #endregion

        #region Tile side size

        private int _tileSideSize;

        /// <summary>
        /// Size of tile's side
        /// </summary>
        public int TileSideSize
        {
            get => _tileSideSize;
            set => SetProperty(ref _tileSideSize, value);
        }

        /// <summary>
        /// Hint for TileSideSize TextBox
        /// TODO: strings
        /// </summary>
        public static string TileSizeHint => "Tile side size";

        /// <summary>
        /// Ready tiles's size
        /// </summary>
        public Size TileSize => new Size(TileSideSize, TileSideSize);

        #endregion

        #region Threads

        private bool _isAutoThreads;

        /// <summary>
        /// Should threads be calculated automatically?
        /// </summary>
        public bool IsAutoThreads
        {
            get => _isAutoThreads;
            set
            {
                SetProperty(ref _isAutoThreads, value);
                ThreadsCountVisibility = value ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// Hint for AutoThreads CheckBox
        /// TODO: strings
        /// </summary>
        public static string IsAutoThreadsContent => "Auto threads";

        private int _threadsCount;

        /// <summary>
        /// Threads count
        /// <remarks><para/>Used only when <see cref="IsAutoThreads"/>
        /// equals <see langword="false"/></remarks>
        /// </summary>
        public int ThreadsCount
        {
            get => _threadsCount;
            set => SetProperty(ref _threadsCount, value);
        }

        /// <summary>
        /// Hint for ThreadsCount TextBox
        /// </summary>
        public static string ThreadsCountHint => Strings.ThreadsCountHint;

        private Visibility _threadsCountVisibility;

        /// <summary>
        /// Controls the ThreadsCount TextBox visibility
        /// </summary>
        public Visibility ThreadsCountVisibility
        {
            get => _threadsCountVisibility;
            set => SetProperty(ref _threadsCountVisibility, value);
        }

        #endregion

        #region Tile cache

        private int _tileCache;

        /// <summary>
        /// How much tiles would you like to store in cache?
        /// </summary>
        public int TileCache
        {
            get => _tileCache;
            set => SetProperty(ref _tileCache, value);
        }

        /// <summary>
        /// Hint for TileCache TextBox
        /// TODO: strings
        /// </summary>
        public static string TileCacheHint => "Tile cache";

        #endregion

        #region Memory

        private long _memory;

        /// <summary>
        /// Max size of input tiff to store in RAM
        /// </summary>
        public long Memory
        {
            get => _memory;
            set => SetProperty(ref _memory, value);
        }

        /// <summary>
        /// Hint for Memory TextBox
        /// TODO: strings
        /// </summary>
        public static string MemoryHint => "Memory";

        #endregion

        #region Settings

        private SettingsModel _settings;

        /// <summary>
        /// Parsed <see cref="SettingsModel"/> from .json
        /// </summary>
        public SettingsModel Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        /// <summary>
        /// Content of SaveSettings Button
        /// TODO: strings
        /// </summary>
        public static string SaveSettingsButtonContent => "Save settings";

        /// <summary>
        /// SaveSettings Button command delegate
        /// </summary>
        public DelegateCommand SaveSettingsButtonCommand { get; }

        #endregion

        #endregion

        #region Start button / Grid.Row=16

        /// <summary>
        /// Text inside Start button
        /// </summary>
        public static string StartButtonContent => Strings.StartButtonContent;

        /// <summary>
        /// StartButton DelegateCommand
        /// </summary>
        public DelegateCommand StartButtonCommand { get; }

        #endregion

        #region Progress bar / Grid.Row=18, 20

        private double _progressBarValue;

        /// <summary>
        /// Progress bar value
        /// </summary>
        public double ProgressBarValue
        {
            get => _progressBarValue;
            set => SetProperty(ref _progressBarValue, value);
        }

        /// <summary>
        /// Text in progress's TextBlock (e.g. "Progress:")
        /// </summary>
        public static string ProgressTextBlock => Strings.ProgressTextBlock;

        #endregion

        #region Meta info / Grid.Row=22

        /// <summary>
        /// Copyright string
        /// </summary>
        public static string Copyright => "© Gigas002 2020";

        /// <summary>
        /// Info about current version
        /// <remarks><para/>Pattern: {MAJOR}.{MINOR}.{PATCH}.{BUILD}</remarks>
        /// </summary>
        public static string Version => Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        #endregion

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize all needed properties
        /// </summary>
        public MainViewModel()
        {
            IsMainGridEnabled = true;

            InputFilePath = string.Empty;
            InputFileButtonCommand = new DelegateCommand(async () => await InputFileButtonAsync().ConfigureAwait(true));

            OutputDirectoryPath = string.Empty;
            OutputDirectoryButtonCommand = new DelegateCommand(async () => await OutputDirectoryButtonAsync().ConfigureAwait(true));

            TempDirectoryPath = string.Empty;
            TempDirectoryButtonCommand = new DelegateCommand(async () => await TempDirectoryButtonAsync().ConfigureAwait(true));

            MinZ = 0;
            MaxZ = 17;

            TileExtensions.Add(TileExtension.Png);
            TileExtensions.Add(TileExtension.Jpg);
            TileExtensions.Add(TileExtension.Webp);
            TargetTileExtension = TileExtension.Png;
            CoordinateSystems.Add(CoordinateSystem.Epsg3857);
            CoordinateSystems.Add(CoordinateSystem.Epsg4326);
            TargetCoordinateSystem = CoordinateSystem.Epsg4326;

            Interpolations.Add(Interpolation.Linear);
            Interpolations.Add(Interpolation.Nearest);
            Interpolations.Add(Interpolation.Cubic);
            Interpolations.Add(Interpolation.Lanczos2);
            Interpolations.Add(Interpolation.Lanczos3);
            Interpolations.Add(Interpolation.Mitchell);
            TargetInterpolation = Interpolation.Lanczos3;

            BandsCount = 4;

            TmsCompatible = false;

            Settings = JsonSerializer.Deserialize<SettingsModel>(File.ReadAllBytes(SettingsModel.Location))
                    ?? SettingsModel.Default;

            Themes.Add(Theme.Dark);
            Themes.Add(Theme.Light);
            Theme = ThemeModel.GetTheme(Settings.Theme);
            TileSideSize = Settings.TileSideSize;
            IsAutoThreads = Settings.IsAutoThreads;
            ThreadsCount = Settings.ThreadsCount;
            TileCache = Settings.TileCache;
            Memory = Settings.Memory;
            SaveSettingsButtonCommand = new DelegateCommand(async () => await SaveSettingsAsync().ConfigureAwait(true));

            StartButtonCommand = new DelegateCommand(async () => await StartButtonAsync().ConfigureAwait(true));

            ProgressBarValue = 0.0;
        }

        #endregion

        #region Methods

        #region Buttons

        /// <summary>
        /// Input directory button
        /// </summary>
        /// <returns></returns>
        public async ValueTask InputFileButtonAsync()
        {
            try
            {
                OpenFileDialogResult dialogResult = await OpenFileDialog.ShowDialogAsync(DialogHostId, new OpenFileDialogArguments())
                                                                        .ConfigureAwait(true);

                InputFilePath = dialogResult.Canceled ? InputFilePath : dialogResult.FileInfo.FullName;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowExceptionAsync(exception).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Output directory button
        /// </summary>
        /// <returns></returns>
        public async ValueTask OutputDirectoryButtonAsync()
        {
            try
            {
                OpenDirectoryDialogArguments args = new OpenDirectoryDialogArguments { CreateNewDirectoryEnabled = true };
                OpenDirectoryDialogResult dialogResult = await OpenDirectoryDialog.ShowDialogAsync(DialogHostId, args).ConfigureAwait(true);

                OutputDirectoryPath = dialogResult.Canceled ? OutputDirectoryPath : dialogResult.Directory;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowExceptionAsync(exception).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Temp directory button
        /// </summary>
        /// <returns></returns>
        public async ValueTask TempDirectoryButtonAsync()
        {
            try
            {
                OpenDirectoryDialogArguments args = new OpenDirectoryDialogArguments { CreateNewDirectoryEnabled = true };
                OpenDirectoryDialogResult dialogResult = await OpenDirectoryDialog.ShowDialogAsync(DialogHostId, args).ConfigureAwait(true);

                TempDirectoryPath = dialogResult.Canceled ? TempDirectoryPath : dialogResult.Directory;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowExceptionAsync(exception).ConfigureAwait(true);
            }
        }

        /// <summary>
        /// Start button
        /// </summary>
        /// <returns></returns>
        public async ValueTask StartButtonAsync()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Check properties for errors
            if (!await CheckPropertiesAsync().ConfigureAwait(true)) return;

            // Create temp directory object
            string tempDirectoryPath = Path.Combine(TempDirectoryPath, DateTime.Now.ToString(DateTimePatterns.LongWithMs));
            CheckHelper.CheckDirectory(tempDirectoryPath, true);

            // Create progress reporter
            IProgress<double> progress = new Progress<double>(value => ProgressBarValue = value);

            // Because we need to check input file it's better to use temprorary value
            string inputFilePath = InputFilePath;

            // Threads should be calculated automatically if checked
            int threadsCount = IsAutoThreads ? 0 : ThreadsCount;

            // Run tiling asynchroniously
            try
            {
                if (!await CheckHelper.CheckInputFileAsync(inputFilePath, TargetCoordinateSystem).ConfigureAwait(true))
                {
                    string tempFilePath = Path.Combine(tempDirectoryPath, $"{GdalWorker.TempFileName}");

                    await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputFilePath, tempFilePath,
                                                                       TargetCoordinateSystem, progress).ConfigureAwait(false);
                    inputFilePath = tempFilePath;
                }

                await using Raster image = new Raster(inputFilePath, TargetCoordinateSystem);

                // Generate tiles
                await image.WriteTilesToDirectoryAsync(OutputDirectoryPath, MinZ, MaxZ, TmsCompatible,
                                                       TileSize, TargetTileExtension, TargetInterpolation,
                                                       BandsCount, TileCache, threadsCount, progress)
                           .ConfigureAwait(true);
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowExceptionAsync(exception).ConfigureAwait(true);
                IsMainGridEnabled = true;

                return;
            }

            // Enable controls
            IsMainGridEnabled = true;

            stopwatch.Stop();
            string timePassed = string.Format(Strings.Done, Environment.NewLine, stopwatch.Elapsed.Days,
                                              stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes,
                                              stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds);
            await DialogHost.Show(new MessageBoxDialogViewModel(timePassed)).ConfigureAwait(true);
        }

        /// <summary>
        /// Update the Settings.json
        /// </summary>
        /// <returns></returns>
        public Task SaveSettingsAsync()
        {
            Settings = new SettingsModel(ThemeModel.GetTheme(Theme), TileSideSize, IsAutoThreads,
                                         ThreadsCount, TileCache, Memory);

            return SettingsModel.SaveAsync(Settings);
        }

        #endregion

        #region Check properties

        /// <summary>
        /// Checks properties for errors and set some before starting
        /// </summary>
        /// <returns><see langword="true"/> if no errors occured;
        /// <see langword="false"/> otherwise</returns>
        private ValueTask<bool> CheckPropertiesAsync()
        {
            try
            {
                // Check paths
                CheckHelper.CheckFile(InputFilePath, true, FileExtensions.Tif);
                CheckHelper.CheckDirectory(OutputDirectoryPath, true);
                CheckHelper.CheckDirectory(TempDirectoryPath);

                // Required params
                if (MinZ < 0) throw new ArgumentOutOfRangeException(nameof(MinZ));
                if (MaxZ < MinZ) throw new ArgumentOutOfRangeException(nameof(MaxZ));
                if (BandsCount <= 0 || BandsCount > 5) throw new ArgumentOutOfRangeException(nameof(BandsCount));

                // Optional params
                if (TileSideSize <= 0) TileSideSize = Tile.DefaultSize.Width;
                if (ThreadsCount <= 0) ThreadsCount = Environment.ProcessorCount;
                if (TileCache < 0) TileCache = 1000;

                // Need to set explicitly
                if (Memory <= 0) throw new ArgumentOutOfRangeException(nameof(Memory));
            }
            catch (Exception exception)
            {
                return Helpers.ErrorHelper.ShowExceptionAsync(exception);
            }

            // Disable grid while working if no errors in args
            IsMainGridEnabled = false;

            // Set default progress bar value for each run
            ProgressBarValue = 0.0;

            return ValueTask.FromResult(true);
        }

        #endregion

        #endregion
    }
}

#pragma warning restore CA1031 // Do not catch general exception types
