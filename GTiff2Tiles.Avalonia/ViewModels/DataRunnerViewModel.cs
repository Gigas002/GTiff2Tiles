using ReactiveUI;
using GTiff2Tiles.Avalonia.Enums;
using GTiff2Tiles.Avalonia.Models;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Core;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.TileMapResource;
using System.Globalization;

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
        set
        {
            value = value < 0 ? 0 : value;

            this.RaiseAndSetIfChanged(ref _minZoom, value);
        }
    }

    private int _maxZoom = 11;

    /// <summary>
    /// Max zoom
    /// </summary>
    public int MaxZoom
    {
        get => _maxZoom;
        set
        {
            value = value < 0 ? 0 : value;

            this.RaiseAndSetIfChanged(ref _maxZoom, value);
        }
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
            await Task.Delay(1).ConfigureAwait(true);
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

        PreExecutionTask("Task in process");

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

        PostExecutionTask("Task complete");

        await MessageBoxViewModel.ShowAsync("Task complete", false).ConfigureAwait(true);
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

    
    
    // Copy-paste from GTiff2Tiles.GUI
     
    public async ValueTask StartButton2()
    {
        // Freeze settings for current run
        var runSettings = Settings.Clone();

        PreExecutionTask("Generating tiles");

        try
        {
            await GenerateTiles(runSettings);
        }
        catch (Exception exception)
        {
            PostExecutionTask(OperationCancelledOrErrorOccuredMessage);

            await MessageBoxViewModel.ShowAsync(exception).ConfigureAwait(true);

            return;
        }

        PostExecutionTask("Complete");

        await MessageBoxViewModel.ShowAsync("Task is done", false).ConfigureAwait(true);
    }

    public Task GenerateTiles(SettingsModel runSettings) => Task.Run(async () =>
    {
            ValidateData(runSettings);

            // Create temp directory object
            string tempDirectoryPath = Path.Combine(runSettings.TempPath, DateTime.Now.ToString(DateTimePatterns.LongWithMs, CultureInfo.InvariantCulture));
            CheckHelper.CheckDirectory(tempDirectoryPath, true);

            // Because we need to check input file it's better to use temprorary value
            string inputFilePath = InputDataSelectorText;

            if (!await CheckHelper.CheckInputFileAsync(inputFilePath, runSettings.CoordinateSystem).ConfigureAwait(true))
            {
                string tempFilePath = Path.Combine(tempDirectoryPath, GdalWorker.TempFileName);

                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputFilePath, tempFilePath,
                                                                   runSettings.CoordinateSystem, Progress)
                                .ConfigureAwait(true);
                inputFilePath = tempFilePath;
            }

            using Raster image = new(inputFilePath, runSettings.CoordinateSystem);

            var tileSize = new Core.Images.Size(runSettings.RasterTileSize, runSettings.RasterTileSize);

            // Generate tiles
            await image.WriteTilesToDirectoryAsync(OutputDataSelectorText, MinZoom, MaxZoom,
                runSettings.TmsCompatible, tileSize, runSettings.RasterTileExtension,
                runSettings.RasterTileInterpolation, runSettings.BandsCount, runSettings.TileCacheCount,
                runSettings.ThreadsCount, Progress).ConfigureAwait(true);

            // Generate tilemapresource if needed
            if (runSettings.Tmr)
            {
                IEnumerable<TileSet> tileSets = TileSets.GenerateTileSetCollection(MinZoom, MaxZoom,
                    tileSize, runSettings.CoordinateSystem);
                TileMap tileMap = new(image.MinCoordinate, image.MaxCoordinate, tileSize,
                    runSettings.RasterTileExtension, tileSets, runSettings.CoordinateSystem);

                string xmlPath = $"{OutputDataSelectorText}/tilemapresource.xml";
                using FileStream fs = File.OpenWrite(xmlPath);
                tileMap.Serialize(fs);
            }

    }, CancellationToken);
     
    private void ValidateData(SettingsModel settings)
    {
        // Check paths
        CheckHelper.CheckFile(InputDataSelectorText, true, FileExtensions.Tif);
        CheckHelper.CheckDirectory(OutputDataSelectorText);
        CheckHelper.CheckDirectory(settings.TempPath);

        // Required params
        if (MaxZoom < MinZoom) throw new ArgumentOutOfRangeException(nameof(MaxZoom));

        // Optional params
        if (settings.IsAutoThreads) settings.ThreadsCount = Environment.ProcessorCount;
    }

    #endregion
}
