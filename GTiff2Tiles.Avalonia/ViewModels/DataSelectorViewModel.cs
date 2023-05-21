using ReactiveUI;
using GTiff2Tiles.Avalonia.Enums;
using Avalonia.Platform.Storage;

namespace GTiff2Tiles.Avalonia.ViewModels;

/// <inheritdoc/>
public class DataSelectorViewModel : ViewModelBase
{
    #region Properties

    private string _selectorPath = string.Empty;

    /// <summary>
    /// Selected path
    /// </summary>
    public string SelectorPath
    {
        get => _selectorPath;
        set => this.RaiseAndSetIfChanged(ref _selectorPath, value);
    }

    private string _selectorTip = "Select the path";

    /// <summary>
    /// Selector tip
    /// </summary>
    public string SelectorTip
    {
        get => _selectorTip;
        set => this.RaiseAndSetIfChanged(ref _selectorTip, value);
    }

    /// <summary>
    /// Selector button text (...)
    /// </summary>
    public static string SelectorButtonText => "...";

    private bool _isSelectorEnabled = true;

    /// <summary>
    /// Is selector enabled?
    /// </summary>
    public bool IsSelectorEnabled
    {
        get => _isSelectorEnabled;
        set => this.RaiseAndSetIfChanged(ref _isSelectorEnabled, value);
    }

    /// <summary>
    /// Selector mode
    /// </summary>
    public DataSelectorMode SelectorMode { get; private set; } = DataSelectorMode.Undefined;

    private Func<DataSelectorMode> SelectorModeChecker { get; }

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public DataSelectorViewModel() { }

    /// <inheritdoc/>
    public DataSelectorViewModel(string tip, DataSelectorMode selectorMode) : this()
    {
        SelectorTip = tip;
        SelectorMode = selectorMode;
    }

    /// <inheritdoc/>
    public DataSelectorViewModel(string tip, Func<DataSelectorMode> selectorModeChecker)
    {
        SelectorTip = tip;
        SelectorModeChecker = selectorModeChecker;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Open dialog selector button
    /// </summary>
    /// <returns></returns>
    public async Task SelectorButton()
    {
        SelectorMode = SelectorModeChecker?.Invoke() ?? SelectorMode;

        switch (SelectorMode)
        {
            case DataSelectorMode.OpenDirectory:
            {
                var paths = await App.AppMainWindow.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()).ConfigureAwait(true);

                if (paths.Count <= 0) return;

                SelectorPath = paths[0].Path.AbsolutePath;

                return;
            }
            case DataSelectorMode.OpenFile:
            {
                var paths = await App.AppMainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()).ConfigureAwait(true);

                if (paths.Count <= 0) return;

                SelectorPath = paths[0].Path.AbsolutePath;

                return;
            }
        }
    }

    #endregion
}