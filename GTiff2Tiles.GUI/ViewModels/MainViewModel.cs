using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Caliburn.Micro;
using GTiff2Tiles.GUI.Localization;
using GTiff2Tiles.GUI.Models;
using GTiff2Tiles.GUI.Properties;
using GTiff2Tiles.GUI.Enums;
using MaterialDesignExtensions.Controls;
using MaterialDesignThemes.Wpf;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.GUI.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel for <see cref="Views.MainView"/>.
    /// </summary>
    public class MainViewModel : PropertyChangedBase
    {
        #region Properties

        #region Settings

        /// <summary>
        /// Shows if dark theme selected.
        /// </summary>
        public bool IsDarkTheme { get; } = Settings.Default.IsDarkTheme;

        #endregion

        #region UI

        /// <summary>
        /// Hint for InputFile TextBox.
        /// </summary>
        public string InputFileHint { get; } = Strings.InputFileHint;

        /// <summary>
        /// Hint for OutputDirectory TextBox.
        /// </summary>
        public string OutputDirectoryHint { get; } = Strings.OutputDirectoryHint;

        /// <summary>
        /// Hint for TempDirectory TextBox.
        /// </summary>
        public string TempDirectoryHint { get; } = Strings.TempDirectoryHint;

        /// <summary>
        /// Hint for MinZ TextBox.
        /// </summary>
        public string MinZHint { get; } = Strings.MinZHint;

        /// <summary>
        /// Hint for MaxZ TextBox.
        /// </summary>
        public string MaxZHint { get; } = Strings.MaxZHint;

        /// <summary>
        /// Hint for Algorithms ComboBox.
        /// </summary>
        public string AlgorithmsHint { get; } = Strings.AlgorithmsHint;

        /// <summary>
        /// Hint for ThreadsCount TextBox.
        /// </summary>
        public string ThreadsCountHint { get; } = Strings.ThreadsCountHint;

        /// <summary>
        /// Text in progress's TextBlock (e.g. "Progress:").
        /// </summary>
        public string ProgressTextBlock { get; } = Strings.ProgressTextBlock;

        /// <summary>
        /// Text inside Start button.
        /// </summary>
        public string StartButtonContent { get; } = Strings.StartButtonContent;

        /// <summary>
        /// Hey, it's me!
        /// </summary>
        public string Copyright { get; } = Enums.MainViewModel.Copyright;

        /// <summary>
        /// Assembly version.
        /// </summary>
        public string Version { get; } = Enums.MainViewModel.Version;

        /// <summary>
        /// Text near tms check box.
        /// </summary>
        public string TmsCheckBoxContent { get; } = Strings.TmsCheckBoxContent;

        /// <summary>
        /// Theme string for DialogHosts.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
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
        /// Threads count.
        /// </summary>
        public int ThreadsCount
        {
            get => _threadsCount;
            set
            {
                _threadsCount = value;
                NotifyOfPropertyChange(() => ThreadsCount);
            }
        }

        /// <summary>
        /// Maximum zoom.
        /// </summary>
        public int MaxZ
        {
            get => _maxZ;
            set
            {
                _maxZ = value;
                NotifyOfPropertyChange(() => MaxZ);
            }
        }

        /// <summary>
        /// Minimum zoom.
        /// </summary>
        public int MinZ
        {
            get => _minZ;
            set
            {
                _minZ = value;
                NotifyOfPropertyChange(() => MinZ);
            }
        }

        /// <summary>
        /// Input file path.
        /// </summary>
        public string InputFilePath
        {
            get => _inputFilePath;
            set
            {
                _inputFilePath = value;
                NotifyOfPropertyChange(() => InputFilePath);
            }
        }

        /// <summary>
        /// Output directory path.
        /// </summary>
        public string OutputDirectoryPath
        {
            get => _outputDirectoryPath;
            set
            {
                _outputDirectoryPath = value;
                NotifyOfPropertyChange(() => OutputDirectoryPath);
            }
        }

        /// <summary>
        /// Temp directory path.
        /// </summary>
        public string TempDirectoryPath
        {
            get => _tempDirectoryPath;
            set
            {
                _tempDirectoryPath = value;
                NotifyOfPropertyChange(() => TempDirectoryPath);
            }
        }

        #endregion

        #region CheckBox

        #region Backing fields

        private bool _tmsCompatible;

        #endregion

        /// <summary>
        /// Shows if you want to create tms-compatible tiles.
        /// </summary>
        public bool TmsCompatible
        {
            get => _tmsCompatible;
            set
            {
                _tmsCompatible = value;
                NotifyOfPropertyChange(() => TmsCompatible);
            }
        }

        #endregion

        #region ComboBox

        private string _algorithm;

        /// <summary>
        /// Currently chosen algorythm.
        /// </summary>
        public string Algorithm
        {
            get => _algorithm;
            set
            {
                _algorithm = value;
                NotifyOfPropertyChange(() => Algorithm);
            }
        }

        /// <summary>
        /// Collection of supported algorythms.
        /// </summary>
        // ReSharper disable once CollectionNeverQueried.Global
        public ObservableCollection<string> Algorithms { get; } = new ObservableCollection<string>();

        #endregion

        #region ProgressBar

        private double _progressBarValue;

        public double ProgressBarValue
        {
            get => _progressBarValue;
            set
            {
                _progressBarValue = value;
                NotifyOfPropertyChange(() => ProgressBarValue);
            }
        }

        #endregion

        private bool _isEnabled;

        /// <summary>
        /// Sets grid's state.
        /// </summary>
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        /// <summary>
        /// Identifier of DialogHost on <see cref="Views.MainView"/>.
        /// </summary>
        public string DialogHostId { get; } = Enums.MainViewModel.DialogHostId;

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize all needed properties.
        /// </summary>
        public MainViewModel()
        {
            //Setting the theme.
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
            Algorithms.Add("crop");
            Algorithms.Add("join");
        }

        #endregion

        #region Methods

        #region Buttons

        /// <summary>
        /// Input directory button
        /// </summary>
        /// <returns></returns>
        public async ValueTask InputFileButton()
        {
            try
            {
                OpenFileDialogResult dialogResult =  await OpenFileDialog.ShowDialogAsync(Enums.MainViewModel.DialogHostId,
                                                                                          new OpenFileDialogArguments());
                InputFilePath = dialogResult.Canceled ? InputFilePath : dialogResult.FileInfo.FullName;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
            }
        }

        /// <summary>
        /// Output directory button.
        /// </summary>
        /// <returns></returns>
        public async ValueTask OutputDirectoryButton()
        {
            try
            {
                OpenDirectoryDialogResult dialogResult =
                    await OpenDirectoryDialog.ShowDialogAsync(Enums.MainViewModel.DialogHostId,
                                                              new OpenDirectoryDialogArguments
                                                                  {CreateNewDirectoryEnabled = true});
                OutputDirectoryPath = dialogResult.Canceled ? OutputDirectoryPath : dialogResult.Directory;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
            }
        }

        /// <summary>
        /// Temp directory button.
        /// </summary>
        /// <returns></returns>
        public async ValueTask TempDirectoryButton()
        {
            try
            {
                OpenDirectoryDialogResult dialogResult =
                    await OpenDirectoryDialog.ShowDialogAsync(Enums.MainViewModel.DialogHostId,
                                                              new OpenDirectoryDialogArguments
                                                                  {CreateNewDirectoryEnabled = true});
                TempDirectoryPath = dialogResult.Canceled ? TempDirectoryPath : dialogResult.Directory;
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
            }
        }

        /// <summary>
        /// Start button.
        /// </summary>
        /// <returns></returns>
        public async ValueTask StartButton()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //Check properties for errors.
            if (!await CheckProperties()) return;

            //Initialize FileSystemEntries from properties.
            FileInfo inputFileInfo = new FileInfo(InputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(OutputDirectoryPath);

            //Create temp directory object.
            string tempDirectoryPath =
                Path.Combine(TempDirectoryPath, DateTime.Now.ToString(Core.Enums.DateTimePatterns.LongWithMs));
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(tempDirectoryPath);

            //Create progress reporter.
            IProgress<double> progress = new Progress<double>(value => ProgressBarValue = value);

            //Run tiling asynchroniously.
            try
            {
                //Check for errors.
                Core.Helpers.CheckHelper.CheckDirectory(outputDirectoryInfo, true);
                if (!await Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName, $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.Warp(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create image object.
                Core.Image.Image inputImage = new Core.Image.Image(inputFileInfo);

                //Switch on algorithm.
                switch (Algorithm)
                {
                    case Core.Enums.Algorithms.Join:
                        await inputImage.GenerateTilesByJoining(outputDirectoryInfo, MinZ, MaxZ, TmsCompatible, progress, ThreadsCount);
                        break;
                    case Core.Enums.Algorithms.Crop:
                        await inputImage.GenerateTilesByCropping(outputDirectoryInfo, MinZ, MaxZ, TmsCompatible, progress, ThreadsCount);
                        break;
                    default:
                        await Helpers.ErrorHelper.ShowError(Strings.AlgorithmNotSupported);
                        IsEnabled = true;
                        return;
                }
            }
            catch (Exception exception)
            {
                await Helpers.ErrorHelper.ShowException(exception);
                IsEnabled = true;
                return;
            }

            //Enable controls.
            IsEnabled = true;

            stopwatch.Stop();
            await DialogHost.Show(new MessageBoxDialogViewModel(string.Format(Strings.Done, Environment.NewLine, stopwatch.Elapsed.Days,
                                                                              stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes,
                                                                              stopwatch.Elapsed.Seconds, stopwatch.Elapsed.Milliseconds)));
        }

        #endregion

        #region Other

        /// <summary>
        /// Checks properties for errors and set some before starting.
        /// </summary>
        /// <returns><see langword="true"/> if no errors occured, <see langword="false"/> otherwise.</returns>
        private async ValueTask<bool> CheckProperties()
        {
            if (string.IsNullOrWhiteSpace(InputFilePath))
                return await Helpers.ErrorHelper.ShowError(string.Format(Strings.PathIsEmpty, nameof(InputFilePath)));

            if (string.IsNullOrWhiteSpace(OutputDirectoryPath))
                return await Helpers.ErrorHelper.ShowError(string.Format(Strings.PathIsEmpty, nameof(OutputDirectoryPath)));

            if (string.IsNullOrWhiteSpace(TempDirectoryPath))
                return await Helpers.ErrorHelper.ShowError(string.Format(Strings.PathIsEmpty, nameof(TempDirectoryPath)));

            if (MinZ < 0)
                return await Helpers.ErrorHelper.ShowError(string.Format(Strings.LesserThan, nameof(MinZ), 0));

            if (MaxZ < 0)
                return await Helpers.ErrorHelper.ShowError(string.Format(Strings.LesserThan, nameof(MaxZ), 0));

            if (MaxZ < MinZ)
                return await Helpers.ErrorHelper.ShowError(string.Format(Strings.LesserThan, nameof(MaxZ), nameof(MinZ)));

            if (string.IsNullOrWhiteSpace(Algorithm))
                return await Helpers.ErrorHelper.ShowError(Strings.SelectAlgorithm);

            Algorithm = Algorithm.ToLowerInvariant();

            if (Algorithm != Core.Enums.Algorithms.Join && Algorithm != Core.Enums.Algorithms.Crop)
                return await Helpers.ErrorHelper.ShowError(Strings.AlgorithmNotSupported);

            if (ThreadsCount <= 0)
                return await Helpers.ErrorHelper.ShowError(string.Format(Strings.LesserOrEqual, nameof(ThreadsCount), 0));

            //Disable controls.
            IsEnabled = false;

            //Set default progress bar value for each run.
            ProgressBarValue = 0.0;

            return true;
        }

        #endregion

        #endregion
    }
}
