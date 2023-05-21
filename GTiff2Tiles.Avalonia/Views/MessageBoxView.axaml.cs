using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GTiff2Tiles.Avalonia.Views;

/// <inheritdoc/>
public partial class MessageBoxView : UserControl
{
    /// <inheritdoc/>
    public MessageBoxView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
