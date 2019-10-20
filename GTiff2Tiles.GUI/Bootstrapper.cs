using System.Windows;
using Caliburn.Micro;
using GTiff2Tiles.GUI.ViewModels;

namespace GTiff2Tiles.GUI
{
    internal sealed class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() => Initialize();

        protected override void OnStartup(object sender, StartupEventArgs e) => DisplayRootViewFor<MainViewModel>();
    }
}
