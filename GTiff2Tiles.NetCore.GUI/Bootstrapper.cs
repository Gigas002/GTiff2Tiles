using System.Windows;
using Caliburn.Micro;
using GTiff2Tiles.NetCore.GUI.ViewModels;

namespace GTiff2Tiles.NetCore.GUI
{
    internal sealed class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() => Initialize();

        protected override void OnStartup(object sender, StartupEventArgs e) => DisplayRootViewFor<MainViewModel>();
    }
}
