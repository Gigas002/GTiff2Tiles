using System.Collections.ObjectModel;

namespace GTiff2Tiles.Avalonia.ViewModels;

/// <inheritdoc/>
public class MainWindowViewModel : ViewModelBase
{
    #region Properties

    /// <summary>
    /// Main DialogHost id;
    /// Has to be a property, not const field
    /// </summary>
    public static string MainDialogHostId => "MainDialogHost";

    /// <summary>
    /// Collection of tabs
    /// </summary>
    public ObservableCollection<TabItemViewModel> Tabs { get; } = new();

    #region Localizable

    /// <summary>
    /// Application title
    /// </summary>
    public static string AppTitle => "GTiff2Tiles";

    /// <summary>
    /// AQS
    /// </summary>
    public static string MainTabName => "Main";

    /// <summary>
    /// Settings
    /// </summary>
    public static string SettingsTabName => "Settings";

    /// <summary>
    /// About
    /// </summary>
    public static string AboutTabName => "About";

    #endregion

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public MainWindowViewModel()
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        var dataRunnerViewModel = new DataRunnerViewModel();
#pragma warning restore CA2000 // Dispose objects before losing scope

        var settingsViewModel = new SettingsViewModel();
        // var aboutViewModel = new AboutViewModel();

        Tabs.Add(new TabItemViewModel(MainTabName, dataRunnerViewModel));
        Tabs.Add(new TabItemViewModel(SettingsTabName, settingsViewModel));
        // Tabs.Add(new TabItemViewModel(AboutTabName, new AboutViewModel()));
    }

    #endregion
}
