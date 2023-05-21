using ReactiveUI;
using GTiff2Tiles.Avalonia.Enums;
using GTiff2Tiles.Avalonia.Models;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Tiles;

#pragma warning disable CA1031 // Do not catch general exception types

namespace GTiff2Tiles.Avalonia.ViewModels;

public class DataRunnerViewModel : ViewModelBase, IDisposable
{
    #region Properties

    public DataSelectorViewModel InputDataSelector { get; set; }

    public DataSelectorViewModel OutputDataSelector { get; set; }

    private int _minZoom;

    /// <summary>
    /// Min zoom
    /// </summary>
    public int MinZoom
    {
        get => _minZoom;
        set => this.RaiseAndSetIfChanged(ref _minZoom, value);
    }

    private int _maxZoom = 11;

    /// <summary>
    /// Max zoom
    /// </summary>
    public int MaxZoom
    {
        get => _maxZoom;
        set => this.RaiseAndSetIfChanged(ref _maxZoom, value);
    }

    public ProgressViewModel ProgressPresenter { get; set; } = new();

    /// <summary>
    /// Is disposed?
    /// </summary>
    public bool IsDisposed { get; protected set; }

    private bool _isCurrentGridEnabled = true;

    /// <summary>
    /// Is current ViewModel enabled?
    /// </summary>
    public bool IsCurrentGridEnabled
    {
        get => _isCurrentGridEnabled;
        set => this.RaiseAndSetIfChanged(ref _isCurrentGridEnabled, value);
    }

    /// <summary>
    /// Progress
    /// </summary>
    public IProgress<double> Progress { get; set; }

    /// <summary>
    /// CancellationTokenSource
    /// </summary>
    public CancellationTokenSource CancellationTokenSource { get; private set; }

    /// <summary>
    /// Cancellation token
    /// </summary>
    public CancellationToken CancellationToken { get; private set; }

    private bool _isStartButtonEnabled = true;

    /// <summary>
    /// Is start button enabled?
    /// </summary>
    public bool IsStartButtonEnabled
    {
        get => _isStartButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isStartButtonEnabled, value);
    }

    private bool _isCancelButtonEnabled;

    /// <summary>
    /// Is cancel button enabled?
    /// </summary>
    public bool IsCancelButtonEnabled
    {
        get => _isCancelButtonEnabled;
        set => this.RaiseAndSetIfChanged(ref _isCancelButtonEnabled, value);
    }

    #region Localizable

    /// <summary>
    /// Input data path
    /// </summary>
    public static string InputDataSelectorText => "Input data path";

    /// <summary>
    /// Output data path
    /// </summary>
    public static string OutputDataSelectorText => "Output data path";

    /// <summary>
    /// Start button text
    /// </summary>
    public static string StartButtonText => "Start";

    /// <summary>
    /// Cancel button text
    /// </summary>
    public static string CancelButtonText => "Cancel";

    /// <summary>
    /// Operation cancelled ot error occured
    /// </summary>
    public static string OperationCancelledOrErrorOccuredMessage => "Operation cancelled or error occured";

    /// <summary>
    /// Select zoom
    /// </summary>
    public static string SelectZoomTip => "Select min/max zoom values";

    #endregion

    /// <summary>
    /// Run settings
    /// </summary>
    public SettingsModel Settings { get; set; }

    #endregion

    #region Finalizer

    /// <summary>
    /// Disposing
    /// </summary>
    ~DataRunnerViewModel() => Dispose(false);

    #endregion

    #region Dispose

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="Dispose()"/>
    /// <param name="disposing">Dispose static fields?</param>
    protected virtual void Dispose(bool disposing)
    {
        if (IsDisposed) return;

        if (disposing)
        {
            // dispose managed objects
        }

        // dispose unmanaged objects
        CancellationTokenSource.Dispose();

        IsDisposed = true;
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Create new RunnerViewModel
    /// </summary>
    public DataRunnerViewModel()
    {
        Settings = SettingsViewModel.Settings;

        InitializeCancellationToken();

        InputDataSelector = new DataSelectorViewModel(InputDataSelectorText, DataSelectorMode.OpenFile);
        OutputDataSelector = new DataSelectorViewModel(OutputDataSelectorText, DataSelectorMode.OpenDirectory);

        Progress = new Progress<double>(ReportProgress);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Round progress to two numbers
    /// </summary>
    /// <param name="value">Progress real value</param>
    public void ReportProgress(double value)
    {
        var rounded = Math.Round(value, 2);

        if (Math.Abs(rounded - ProgressPresenter.ProgressValue) > double.Epsilon)
            ProgressPresenter.ProgressValue = rounded;
    }

    /// <summary>
    /// Initialize cancellation token
    /// </summary>
    public void InitializeCancellationToken()
    {
        CancellationTokenSource?.Dispose();

        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
    }

    /// <summary>
    /// Disable current grid,
    /// enable cancel button
    /// init cancellation token,
    /// start timer
    /// set current stage message
    /// </summary>
    /// <param name="message">Message to set</param>
    public void PreExecutionTask(string message)
    {
        IsCurrentGridEnabled = false;
        IsCancelButtonEnabled = true;
        InitializeCancellationToken();

        InputDataSelector.IsSelectorEnabled = false;
        ProgressPresenter.ProgressValue = 0.0;
        ProgressPresenter.StartTimer();
        ProgressPresenter.CurrentStageValue = message;
    }

    /// <summary>
    /// Enable current grid,
    /// disable cancel button
    /// stop timer
    /// set current stage message
    /// </summary>
    /// <param name="message">Message to set</param>
    public void PostExecutionTask(string message)
    {
        IsCurrentGridEnabled = true;
        IsCancelButtonEnabled = false;

        InputDataSelector.IsSelectorEnabled = true;
        ProgressPresenter.ProgressValue = 100.0;
        ProgressPresenter.StopTimer();
        ProgressPresenter.CurrentStageValue = message;
    }

    #region GUI testing actions

    /// <summary>
    /// Simulate work in loop
    /// </summary>
    private Task TestAction(double iterationCount) => Task.Run(async () =>
    {
        for (int i = 0; i <= iterationCount; i++)
        {
            CancellationToken.ThrowIfCancellationRequested();

            double percentage = i / iterationCount * 100.0;
            await Task.Delay(1, CancellationToken).ConfigureAwait(true);
            Progress.Report(percentage);
        }
    });

    /// <summary>
    /// Just freezes the interface, running progress bar, showing message box,
    /// cancelling task
    /// </summary>
    public async ValueTask TestAction()
    {
        // Freeze settings for current run
        var runSettings = Settings.Clone();

        PreExecutionTask("message");

        // Simulate work
        const double iterationCount = 10000.0;

        try
        {
            await TestAction(iterationCount).ConfigureAwait(true);
        }
        catch (Exception exception)
        {
            PostExecutionTask(OperationCancelledOrErrorOccuredMessage);

            await MessageBoxViewModel.ShowAsync(exception).ConfigureAwait(true);

            return;
        }

        PostExecutionTask("message");

        await MessageBoxViewModel.ShowAsync("message", false).ConfigureAwait(true);
    }

    #endregion

    public Task StartButton() => TestAction().AsTask();

    public async Task CancelButton()
    {
        var dialRes = await MessageBoxViewModel.ShowAsync("Cancel task execution?").ConfigureAwait(true);

        if (dialRes == DialogResult.Ok)
        {
            CancellationTokenSource.Cancel();
            ProgressPresenter.StopTimer();
        }
    }

    
    /*
     Copy-paste from GTiff2Tiles.GUI
     
     
    public async ValueTask StartButtonAsync()
    {
        #region Preconditions checks

        if (!await CheckPropertiesAsync().ConfigureAwait(true)) return;

        #endregion

        // Start timer after checks passed
        Stopwatch stopwatch = Stopwatch.StartNew();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1.0) };
        _timer.Tick += (_, _) =>
            TimePassedValue = string.Format(CultureInfo.InvariantCulture, Strings.TimePassedValue, stopwatch.Elapsed.Days,
                                            stopwatch.Elapsed.Hours, stopwatch.Elapsed.Minutes,
                                            stopwatch.Elapsed.Seconds);
        _timer.Start();

        // Create temp directory object
        string tempDirectoryPath = Path.Combine(TempDirectoryPath, DateTime.Now.ToString(DateTimePatterns.LongWithMs, CultureInfo.InvariantCulture));
        CheckHelper.CheckDirectory(tempDirectoryPath, true);

        // Create progress reporter
        IProgress<double> progress = new Progress<double>(value => ProgressBarValue = Math.Round(value, 4));

        // Because we need to check input file it's better to use temprorary value
        string inputFilePath = InputFilePath;

        // Threads should be calculated automatically if checked
        int threadsCount = IsAutoThreads ? 0 : ThreadsCount;

        // Run tiling asynchroniously
        try
        {
            if (!await CheckHelper.CheckInputFileAsync(inputFilePath, TargetCoordinateSystem).ConfigureAwait(true))
            {
                string tempFilePath = Path.Combine(tempDirectoryPath, GdalWorker.TempFileName);

                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputFilePath, tempFilePath,
                                                                   TargetCoordinateSystem, progress)
                                .ConfigureAwait(true);
                inputFilePath = tempFilePath;
            }

            using Raster image = new(inputFilePath, TargetCoordinateSystem);

            // Generate tiles
            await image.WriteTilesToDirectoryAsync(OutputDirectoryPath, MinZ, MaxZ, TmsCompatible, TileSize,
                                                   TargetTileExtension, TargetInterpolation, BandsCount, TileCache,
                                                   threadsCount, progress).ConfigureAwait(true);

            // Generate tilemapresource if needed
            if (IsTmr)
            {
                IEnumerable<TileSet> tileSets = TileSets.GenerateTileSetCollection(MinZ, MaxZ, TileSize, TargetCoordinateSystem);
                TileMap tileMap = new(image.MinCoordinate, image.MaxCoordinate, TileSize, TargetTileExtension, tileSets,
                                      TargetCoordinateSystem);

                string xmlPath = $"{OutputDirectoryPath}/{TmrName}";
                using FileStream fs = File.OpenWrite(xmlPath);
                tileMap.Serialize(fs);
            }
        }
        catch (Exception exception)
        {
            await Helpers.ErrorHelper.ShowExceptionAsync(exception).ConfigureAwait(true);

            return;
        }
        finally
        {
            // Enable controls and stop timer
            IsMainGridEnabled = true;
            stopwatch.Stop();
            _timer.Stop();
        }

        await DialogHost.Show(new MessageBoxDialogViewModel(Strings.Done)).ConfigureAwait(true);
    }
     
     
    /// <summary>
    /// Checks properties for errors and set some before starting
    /// </summary>
    /// <returns><see langword="true"/> if no errors occured;
    /// <see langword="false"/> otherwise</returns>
    private async ValueTask<bool> ValidateProperties()
    {
        try
        {
            // Check paths
            CheckHelper.CheckFile(InputFilePath, true, FileExtensions.Tif);
            CheckHelper.CheckDirectory(OutputDirectoryPath, true);
            CheckHelper.CheckDirectory(TempDirectoryPath);

            // Required params
            if (MinZoom < 0) throw new ArgumentOutOfRangeException(nameof(MinZoom));
            if (MaxZoom < MinZoom) throw new ArgumentOutOfRangeException(nameof(MaxZoom));
            if (BandsCount < 1 || BandsCount > 4) throw new ArgumentOutOfRangeException(nameof(BandsCount));

            // Optional params
            if (TileSideSize <= 0) TileSideSize = Tile.DefaultSize.Width;
            if (ThreadsCount <= 0) ThreadsCount = Environment.ProcessorCount;
            if (TileCache < 0) TileCache = 1000;

            // Need to set explicitly
            if (Memory <= 0) throw new ArgumentOutOfRangeException(nameof(Memory));
        }
        catch (Exception exception)
        {
            await MessageBoxViewModel.ShowAsync(exception);

            return false;
        }

        // Disable grid while working if no errors in args
        IsCurrentGridEnabled = false;

        // Set default progress bar value for each run
        ProgressBarValue = 0.0;

        return true;
    }
    */

    #endregion
}
