using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GTiff2Tiles.Avalonia.ViewModels;
using GTiff2Tiles.Avalonia.Views;

namespace GTiff2Tiles.Avalonia;

/// <inheritdoc/>
public class App : Application
{
    /// <summary>
    /// Main Vjik.GUI window
    /// </summary>
    public static Window AppMainWindow { get; private set; }

    /// <summary>
    /// Main Vjik.GUI ViewModel
    /// </summary>
    public static MainWindowViewModel AppMainViewModel { get; private set; } = new();

    /// <inheritdoc/>
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    /// <inheritdoc/>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = AppMainViewModel
            };

            AppMainWindow = desktop.MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}