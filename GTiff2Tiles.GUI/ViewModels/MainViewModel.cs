using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using Ookii.Dialogs.Wpf;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.GUI.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// I wanted to write there something useful, but later I decided not to do it.
    /// </summary>
    public class MainViewModel : PropertyChangedBase
    {
        #region Properties

        #region Const

        /// <summary>
        /// Hey, it's me!
        /// </summary>
        public string Copyright { get; } = Enums.MainViewModel.Copyright;

        /// <summary>
        /// Assembly version.
        /// </summary>
        public string Version { get; } = Enums.MainViewModel.Version;

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
        /// Sets control's state.
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

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize all needed properties.
        /// </summary>
        public MainViewModel()
        {
            InputFilePath = string.Empty;
            OutputDirectoryPath = string.Empty;
            TempDirectoryPath = string.Empty;
            MinZ = 0;
            MaxZ = 1;
            ThreadsCount = 1;
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
        public void InputFileButton()
        {
            VistaOpenFileDialog openFileDialog = new VistaOpenFileDialog();
            InputFilePath = openFileDialog.ShowDialog() != true ? InputFilePath : openFileDialog.FileName;
        }

        /// <summary>
        /// Output directory button.
        /// </summary>
        public void OutputDirectoryButton()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            OutputDirectoryPath = folderBrowserDialog.ShowDialog() != true
                                      ? OutputDirectoryPath
                                      : folderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// Temp directory button.
        /// </summary>
        public void TempDirectoryButton()
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
            TempDirectoryPath = folderBrowserDialog.ShowDialog() != true
                                    ? TempDirectoryPath
                                    : folderBrowserDialog.SelectedPath;
        }

        /// <summary>
        /// Start button.
        /// </summary>
        public async void StartButton()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            //Check properties for errors.
            if (!CheckProperties()) return;

            //Initialize FileSystemEntries from properties.
            FileInfo inputFileInfo = new FileInfo(InputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(OutputDirectoryPath);
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(TempDirectoryPath);

            //Create progress reporter.
            IProgress<double> progress = new Progress<double>(value => ProgressBarValue = value);

            //Run tiling asynchroniously.
            try
            {
                switch (Algorithm)
                {
                    case Core.Enums.Algorithms.Join:
                        await GenerateTilesByJoining(inputFileInfo, outputDirectoryInfo, tempDirectoryInfo, MinZ, MaxZ,
                                                     progress, ThreadsCount);
                        break;
                    case Core.Enums.Algorithms.Crop:
                        await GenerateTilesByCropping(inputFileInfo, outputDirectoryInfo, tempDirectoryInfo, MinZ, MaxZ,
                                                      progress, ThreadsCount);
                        break;
                    default:
                        Helpers.ErrorHelper.ShowError("This algorithm is not supported.", null);
                        IsEnabled = true;
                        return;
                }
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.ShowException(exception);
                IsEnabled = true;
                return;
            }

            //Try to delete temp directory.
            try
            {
                tempDirectoryInfo.Delete(true);
            }
            catch (Exception exception)
            {
                Helpers.ErrorHelper.ShowException(exception);
                IsEnabled = true;
                return;
            }

            //Enable controls.
            IsEnabled = true;

            stopwatch.Stop();
            MessageBox.Show($"Done by: days:{stopwatch.Elapsed.Days} hours:{stopwatch.Elapsed.Hours} minutes:{stopwatch.Elapsed.Minutes} "
                          + $"seconds:{stopwatch.Elapsed.Seconds} ms:{stopwatch.Elapsed.Milliseconds}");
        }

        #endregion

        #region Other

        /// <summary>
        /// Checks properties for errors and set some before starting.
        /// </summary>
        /// <returns><see langword="true"/> if no errors occured, <see langword="false"/> otherwise.</returns>
        private bool CheckProperties()
        {
            if (string.IsNullOrWhiteSpace(InputFilePath))
                return Helpers.ErrorHelper.ShowError("Input file path is empty.", null);

            if (string.IsNullOrWhiteSpace(OutputDirectoryPath))
                return Helpers.ErrorHelper.ShowError("Output directory path is empty.", null);

            if (string.IsNullOrWhiteSpace(TempDirectoryPath))
                return Helpers.ErrorHelper.ShowError("Temp directory path is empty.", null);

            if (MinZ < 0)
                return Helpers.ErrorHelper.ShowError("Minimum zoom is lesser, than 0.", null);

            if (MaxZ < 0)
                return Helpers.ErrorHelper.ShowError("Maximum zoom is lesser, than 0.", null);

            if (MaxZ < MinZ)
                return Helpers.ErrorHelper.ShowError("Minimum zoom is bigger, than maximum.", null);

            Algorithm = Algorithm.ToLowerInvariant();

            if (string.IsNullOrWhiteSpace(Algorithm))
                return Helpers.ErrorHelper.ShowError("Please, choose the algorithm.", null);

            if (Algorithm != Core.Enums.Algorithms.Join && Algorithm != Core.Enums.Algorithms.Crop)
                return Helpers.ErrorHelper.ShowError("This algorithm is not supported.", null);

            if (ThreadsCount <= 0)
                return Helpers.ErrorHelper.ShowError("Threads count is lesser or equal 0.", null);

            //Disable controls.
            IsEnabled = false;

            //Set default progress bar value for each run.
            ProgressBarValue = 0.0;

            return true;
        }

        /// <summary>
        /// Crops input tiff for each zoom.
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="tempDirectoryInfo">Temp directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        private static async ValueTask GenerateTilesByCropping(FileInfo inputFileInfo,
                                                               DirectoryInfo outputDirectoryInfo,
                                                               DirectoryInfo tempDirectoryInfo, int minZ, int maxZ,
                                                               IProgress<double> progress, int threadsCount)
        {
            Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, minZ, maxZ);

            await Task.Factory.StartNew(() => image.GenerateTilesByCropping(tempDirectoryInfo, progress, threadsCount),
                                        TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Create tiles. Crops input tiff only for lowest zoom and then join the higher ones from it.
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="tempDirectoryInfo">Temp directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        private static async ValueTask GenerateTilesByJoining(FileInfo inputFileInfo, DirectoryInfo outputDirectoryInfo,
                                                              DirectoryInfo tempDirectoryInfo, int minZ, int maxZ,
                                                              IProgress<double> progress, int threadsCount)
        {
            Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, minZ, maxZ);

            await Task.Factory.StartNew(() => image.GenerateTilesByJoining(tempDirectoryInfo, progress, threadsCount),
                                        TaskCreationOptions.LongRunning);
        }

        #endregion

        #endregion
    }
}
