using System;
using System.Threading.Tasks;
using GTiff2Tiles.GUI.ViewModels;
using MaterialDesignThemes.Wpf;

namespace GTiff2Tiles.GUI.Helpers
{
    /// <summary>
    /// That class helps to print exceptions and custom errors
    /// </summary>
    internal static class ErrorHelper
    {
        /// <summary>
        /// Shows current exception
        /// </summary>
        /// <param name="exception">Exception</param>
        /// <returns>Always <see langword="false"/></returns>
        internal static async ValueTask<bool> ShowExceptionAsync(Exception exception)
        {
            #if DEBUG
            if (exception.InnerException != null) await DialogHost.Show(new MessageBoxDialogViewModel(exception.InnerException.Message)).ConfigureAwait(true);
            #endif

            await DialogHost.Show(new MessageBoxDialogViewModel(exception.Message)).ConfigureAwait(true);

            return false;
        }
    }
}
