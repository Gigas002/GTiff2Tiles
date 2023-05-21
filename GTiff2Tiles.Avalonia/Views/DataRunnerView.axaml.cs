using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GTiff2Tiles.Avalonia.Views;

/// <inheritdoc/>
public partial class DataRunnerView : UserControl
{
    /// <inheritdoc/>
    public DataRunnerView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
