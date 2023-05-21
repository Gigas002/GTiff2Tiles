using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GTiff2Tiles.Avalonia.Views;

/// <inheritdoc/>
public partial class SettingsView : UserControl
{
    /// <inheritdoc/>
    public SettingsView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}