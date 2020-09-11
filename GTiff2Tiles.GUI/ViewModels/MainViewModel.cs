#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using GTiff2Tiles.Core;
using Prism.Mvvm;
using Prism.Commands;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.GUI.Constants;
using GTiff2Tiles.GUI.Localization;
using GTiff2Tiles.GUI.Models;
using GTiff2Tiles.GUI.Properties;
using GTiff2Tiles.GUI.Views;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedType.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.GUI.ViewModels
{
    /// <summary>
    /// ViewModel for <see cref="MainView"/>
    /// </summary>
    // ReSharper disable once MemberCanBeInternal
    public class MainViewModel : BindableBase
    {
        #region Properties

        #region Constants

        /// <summary>
        /// Copyright string
        /// </summary>
        public static string Copyright { get; } = "© Gigas002 2020";

        /// <summary>
        /// Info about current version
        /// <remarks><para/>Pattern: {MAJOR}.{MINOR}.{PATCH}.{BUILD}</remarks>
        /// </summary>
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();

        /// <summary>
        /// Identifier of DialogHost on <see cref="MainView"/>
        /// </summary>
        public static string DialogHostId { get; } = "DialogHost";

        #endregion

        #region Settings

        /// <summary>
        /// Shows if dark theme selected
        /// </summary>
        public bool IsDarkTheme
        {
            get
            {
#if DEBUG
                // That's for designer
                return false;
#endif

                return Settings.Default.IsDarkTheme;
            }
        }

        #endregion

        #region UI

        /// <summary>
        /// Hint for InputFile TextBox
        /// </summary>
        public string InputFileHint { get; } = Strings.InputFileHint;

        /// <summary>
        /// Hint for OutputDirectory TextBox
        /// </summary>
        public string OutputDirectoryHint { get; } = Strings.OutputDirectoryHint;

        /// <summary>
        /// Hint for TempDirectory TextBox
        /// </summary>
        public string TempDirectoryHint { get; } = Strings.TempDirectoryHint;

        /// <summary>
        /// Hint for MinZ TextBox
        /// </summary>
        public string MinZHint { get; } = Strings.MinZHint;

        /// <summary>
        /// Hint for MaxZ TextBox
        /// </summary>
        public string MaxZHint { get; } = Strings.MaxZHint;

        /// <summary>
        /// Hint for Extensions ComboBox
        /// </summary>
        public string TileExtensionsHint { get; } = Strings.TileExtensionsHint;

        /// <summary>
        /// Hint for ThreadsCount TextBox
        /// </summary>
        public string ThreadsCountHint { get; } = Strings.ThreadsCountHint;

        /// <summary>
        /// Text in progress's TextBlock (e.g. "Progress:")
        /// </summary>
        public string ProgressTextBlock { get; } = Strings.ProgressTextBlock;

        /// <summary>
        /// Text inside Start button
        /// </summary>
        public string StartButtonContent { get; } = Strings.StartButtonContent;

        /// <summary>
        /// Text near tms check box
        /// </summary>
        public string TmsCheckBoxContent { get; } = Strings.TmsCheckBoxContent;

        /// <summary>
        /// Theme string for DialogHosts
        /// </summary>
        public string Theme { get; }

        #endregion

        #region TextBoxes/Blocks

        #region Private backing fields

        private int _threadsCount;

        private int _maxZ;

        private int _minZ;

        private string _inputFilePath;

        private string _outputDirectoryPath;

        private string _tempDirectoryPath;

        #endregion

        /// <summary>
        /// Threads count
        /// </summary>
        public int ThreadsCount
        {
            get => _threadsCount;
            set => SetProperty(ref _threadsCount, value);
        }

        /// <summary>
        /// Maximum zoom
        /// </summary>
        public int MaxZ
        {
            get => _maxZ;
            set => SetProperty(ref _maxZ, value);
        }

        /// <summary>
        /// Minimum zoom
        /// </summary>
        public int MinZ
        {
            get => _minZ;
            set => SetProperty(ref _minZ, value);
        }

        /// <summary>
        /// Input file path
        /// </summary>
        public string InputFilePath
        {
            get => _inputFilePath;
            set => SetProperty(ref _inputFilePath, value);
        }

        /// <summary>
        /// Output directory path
        /// </summary>
        public string OutputDirectoryPath
        {
            get => _outputDirectoryPath;
            set => SetProperty(ref _outputDirectoryPath, value);
        }

        /// <summary>
        /// Temp directory path
        /// </summary>
        public string TempDirectoryPath
        {
            get => _tempDirectoryPath;
            set => SetProperty(ref _tempDirectoryPath, value);
        }

        #endregion

        #region CheckBox

        #region Backing fields

        private bool _tmsCompatible;

        #endregion

        /// <summary>
        /// Shows if you want to create tms-compatible tiles
        /// </summary>
        public bool TmsCompatible
        {
            get => _tmsCompatible;
            set => SetProperty(ref _tmsCompatible, value);
        }

        #endregion

        #region ComboBox

        private string _tileExtension;

        /// <summary>
        /// Currently chosen tile extension
        /// </summary>
        public string TileExtension
        {
            get => _tileExtension;
            set => SetProperty(ref _tileExtension, value);
        }

        /// <summary>
        /// Enum for tile extension
        /// </summary>
        private TileExtension RealTileExtension { get; set; } = Core.Enums.TileExtension.Png;

        /// <summary>
        /// Collection of supported tile extensions
        /// </summary>
        // ReSharper disable once CollectionNeverQueried.Global
        public ObservableCollection<string> TileExtensions { get; } = new ObservableCollection<string>();

        #endregion

        #region ProgressBar

        private double _progressBarValue;

        /// <summary>
        /// Progress bar value
        /// </summary>
        public double ProgressBarValue
        {
            get => _progressBarValue;
            set => SetProperty(ref _progressBarValue, value);
        }

        #endregion

        private bool _isEnabled;

        /// <summary>
        /// Sets grid's state
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        #endregion

        #region Constructor

        /// <inheritdoc />
        /// <summary>
        /// Initialize all needed properties
        /// </summary>
        public MainViewModel()
        {
            // Setting the theme
            SetThemeModel.SetTheme(IsDarkTheme);
            Theme = IsDarkTheme ? Themes.Dark : Themes.Light;

            InputFilePath = string.Empty;
            OutputDirectoryPath = string.Empty;
            TempDirectoryPath = string.Empty;
            MinZ = 0;
            MaxZ = 17;
            ThreadsCount = 5;
            ProgressBarValue = 0.0;
            IsEnabled = true;
            TileExtensions.Add(FileExtensions.Png);
            TileExtensions.Add(FileExtensions.Jpg);
            TileExtensions.Add(FileExtensions.Webp);

            // Bind delegates with methods
            InputFileButtonCommand = new DelegateCommand(async () => await InputFileButtonAsync().ConfigureAwait(true));
            OutputDirectoryButtonCommand = new DelegateCommand(async () => await OutputDirectoryButtonAsync().ConfigureAwait(true));
            TempDirectoryButtonCommand = new DelegateCommand(async () => await TempDirectoryButtonAsync().ConfigureAwait(true));
            StartButtonCommand = new DelegateCommand(async () => await StartButtonAsync().ConfigureAwait(true));
        }

        #endregion

        #region DelegateCommands

        /// <summary>
        /// InputFileButton DelegateCommand
        /// </summary>
        public DelegateCommand InputFileButtonCommand { get; }

        /// <summary>
        /// OutputDirectoryButton DelegateCommand
        /// </summary>
        public DelegateCommand OutputDirectoryButtonCommand { get; }

        /// <summary>
        /// TempDirectoryButton DelegateCommand
        /// </summary>
        public DelegateCommand TempDirectoryButtonCommand { get; }

        /// <summary>
        /// StartButton DelegateCommand
        /// </summary>
        public DelegateCommand StartButtonCommand { get; }

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
                OpenFileDialogResult dialogResult =
                    await OpenFileDialog.ShowDialogAsync(DialogHostId, new OpenFileDialogArguments())
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
                OpenDirectoryDialogResult dialogResult =
                    await OpenDirectoryDialog.ShowDialogAsync(DialogHostId, args).ConfigureAwait(true);

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
                OpenDirectoryDialogArguments args =
                    new OpenDirectoryDialogArguments { CreateNewDirectoryEnabled = true };
                OpenDirectoryDialogResult dialogResult =
                    await OpenDirectoryDialog.ShowDialogAsync(DialogHostId, args).ConfigureAwait(true);

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
            // TODO: coordinate system
            var coordinateSystem = CoordinateSystem.Epsg4326;

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Check properties for errors
            if (!await CheckPropertiesAsync().ConfigureAwait(true)) return;

            // Initialize FileSystemEntries from properties
            FileInfo inputFileInfo = new FileInfo(InputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(OutputDirectoryPath);

            // Create temp directory object
            string tempDirectoryPath =
                Path.Combine(TempDirectoryPath, DateTime.Now.ToString(DateTimePatterns.LongWithMs));
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(tempDirectoryPath);

            // Create progress reporter
            IProgress<double> progress = new Progress<double>(value => ProgressBarValue = value);

            // Run tiling asynchroniously
            try
            {
                // Check for errors
                CheckHelper.CheckDirectory(outputDirectoryInfo.FullName, true);

                if (!await CheckHelper.CheckInputFileAsync(inputFileInfo.FullName, coordinateSystem).ConfigureAwait(true))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{GdalWorker.TempFileName}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputFileInfo.FullName, tempFileInfo.FullName, coordinateSystem,
                                                                       progress).ConfigureAwait(false);
                    inputFileInfo = tempFileInfo;
                }

                await using Raster image = new Raster(inputFileInfo.FullName, coordinateSystem);

                // Generate tiles
                await image.WriteTilesToDirectoryAsync(outputDirectoryInfo.FullName, MinZ, MaxZ, TmsCompatible,
                                                       tileExtension: RealTileExtension,
                                                       bandsCount: 4, progress: progress,
                                                       threadsCount: ThreadsCount)
                           .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowExceptionAsync(exception).ConfigureAwait(true);
                IsEnabled = true;

                return;
            }

            // Enable controls
            IsEnabled = true;

            stopwatch.Stop();
            await DialogHost
                 .Show(new MessageBoxDialogViewModel(string.Format(Strings.Done, Environment.NewLine,
                                                                   stopwatch.Elapsed.Days, stopwatch.Elapsed.Hours,
                                                                   stopwatch.Elapsed.Minutes, stopwatch.Elapsed.Seconds,
                                                                   stopwatch.Elapsed.Milliseconds)))
                 .ConfigureAwait(true);
        }

        #endregion

        #region Other

        /// <summary>
        /// Checks properties for errors and set some before starting
        /// </summary>
        /// <returns><see langword="true"/> if no errors occured;
        /// <see langword="false"/> otherwise</returns>
        private ValueTask<bool> CheckPropertiesAsync()
        {
            if (string.IsNullOrWhiteSpace(InputFilePath))
                return Helpers.ErrorHelper.ShowErrorAsync(string.Format(Strings.PathIsEmpty, nameof(InputFilePath)));

            if (string.IsNullOrWhiteSpace(OutputDirectoryPath))
                return Helpers.ErrorHelper.ShowErrorAsync(string.Format(Strings.PathIsEmpty, nameof(OutputDirectoryPath)));

            if (string.IsNullOrWhiteSpace(TempDirectoryPath))
                return Helpers.ErrorHelper.ShowErrorAsync(string.Format(Strings.PathIsEmpty, nameof(TempDirectoryPath)));

            if (MinZ < 0) return Helpers.ErrorHelper.ShowErrorAsync(string.Format(Strings.LesserThan, nameof(MinZ), 0));

            if (MaxZ < 0) return Helpers.ErrorHelper.ShowErrorAsync(string.Format(Strings.LesserThan, nameof(MaxZ), 0));

            if (MaxZ < MinZ)
                return Helpers.ErrorHelper.ShowErrorAsync(string.Format(Strings.LesserThan, nameof(MaxZ), nameof(MinZ)));

            if (ThreadsCount <= 0)
                return Helpers.ErrorHelper.ShowErrorAsync(string.Format(Strings.LesserOrEqual, nameof(ThreadsCount), 0));

            if (string.IsNullOrWhiteSpace(TileExtension))
                return Helpers.ErrorHelper.ShowErrorAsync(Strings.SelectExtension);

            // Set tile extension; .png by default or unknown input
            RealTileExtension = TileExtension switch
            {
                FileExtensions.Jpg => Core.Enums.TileExtension.Jpg,
                FileExtensions.Webp => Core.Enums.TileExtension.Webp,
                _ => Core.Enums.TileExtension.Png
            };

            // Disable controls
            IsEnabled = false;

            // Set default progress bar value for each run
            ProgressBarValue = 0.0;

            return ValueTask.FromResult(true);
        }

        #endregion

        #endregion
    }
}
