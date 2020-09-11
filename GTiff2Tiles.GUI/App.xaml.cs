using Prism.Ioc;
using GTiff2Tiles.GUI.Views;
using System.Windows;

namespace GTiff2Tiles.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell() => Container.Resolve<MainView>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry) { }
    }
}
