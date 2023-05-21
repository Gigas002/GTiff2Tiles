using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GTiff2Tiles.Avalonia.Views;

/// <inheritdoc/>
public partial class ProgressView : UserControl
{
    /// <inheritdoc/>
    public ProgressView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}