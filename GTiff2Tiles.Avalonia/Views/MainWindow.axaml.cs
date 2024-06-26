using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GTiff2Tiles.Avalonia.Views;

/// <inheritdoc/>
public partial class MainWindow : Window
{
    /// <inheritdoc/>
    public MainWindow() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
