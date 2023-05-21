using System.Globalization;
using Avalonia.Threading;
using ReactiveUI;

namespace GTiff2Tiles.Avalonia.ViewModels;

/// <inheritdoc/>
public class ProgressViewModel : ViewModelBase
{
    #region Properties

    #region Current stage

    private string _currentStageValue;

    /// <summary>
    /// Current stage text
    /// </summary>
    public string CurrentStageValue
    {
        get => _currentStageValue;
        set => this.RaiseAndSetIfChanged(ref _currentStageValue, value);
    }

    #endregion

    #region Progress

    private double _progressValue;

    /// <summary>
    /// Progress value
    /// </summary>
    public double ProgressValue
    {
        get => _progressValue;
        set => this.RaiseAndSetIfChanged(ref _progressValue, value);
    }

    #endregion

    #region IsIndeterminate

    private bool _isIndeterminate;

    /// <summary>
    /// Progress bar state
    /// </summary>
    public bool IsIndeterminate
    {
        get => _isIndeterminate;
        set => this.RaiseAndSetIfChanged(ref _isIndeterminate, value);
    }

    #endregion

    #region Timer

    private DispatcherTimer Timer { get; set; }

    private TimeSpan _timerValue = TimeSpan.Zero;

    /// <summary>
    /// Timer
    /// </summary>
    public string TimerValue
    {
        get => _timerValue.ToString("g", CultureInfo.InvariantCulture);
        set
        {
            var timer = TimeSpan.Parse(value, CultureInfo.InvariantCulture);

            this.RaiseAndSetIfChanged(ref _timerValue, timer);
        }
    }

    //public TimeSpan PublicTimerValue => _timerValue;

    #endregion

    #region Localizable

    /// <summary>
    /// Current stage
    /// </summary>
    public static string CurrentStageText => "Current stage:";

    /// <summary>
    /// Time passed
    /// </summary>
    public static string TimePassedText => "Time passed:";

    #endregion

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public ProgressViewModel() => InitializeTimer();

    #endregion

    #region Methods

    private void TimerTick(object sender, EventArgs args)
    {
        var timer = (DispatcherTimer)sender;
        TimerValue = _timerValue.Add(timer.Interval).ToString();
    }

    private void InitializeTimer() => Timer = new DispatcherTimer(TimeSpan.FromSeconds(1), default, TimerTick);

    /// <summary>
    /// Starts timer
    /// </summary>
    public void StartTimer()
    {
        InitializeTimer();
        TimerValue = TimeSpan.Zero.ToString();
        Timer.Start();
    }

    /// <summary>
    /// Stops timer
    /// </summary>
    public void StopTimer() => Timer?.Stop();

    #endregion
}