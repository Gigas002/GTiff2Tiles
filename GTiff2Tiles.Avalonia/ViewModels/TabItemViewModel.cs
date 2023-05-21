using ReactiveUI;

namespace GTiff2Tiles.Avalonia.ViewModels;

/// <inheritdoc/>
public class TabItemViewModel : ViewModelBase
{
    #region Properties

    private string _header;

    /// <summary>
    /// Header
    /// </summary>
    public string Header
    {
        get => _header;
        set => this.RaiseAndSetIfChanged(ref _header, value);
    }

    private ViewModelBase _viewModel;

    /// <summary>
    /// ViewModel
    /// </summary>
    public ViewModelBase ViewModel
    {
        get => _viewModel;
        set => this.RaiseAndSetIfChanged(ref _viewModel, value);
    }

    #endregion

    #region Constructors

    /// <inheritdoc/>
    public TabItemViewModel(string header, ViewModelBase viewModel)
    {
        Header = header;
        ViewModel = viewModel;
    }

    #endregion
}