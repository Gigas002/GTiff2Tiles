using System;
using System.Threading.Tasks;
using GTiff2Tiles.GUI.ViewModels;
using MaterialDesignThemes.Wpf;

namespace GTiff2Tiles.GUI.Helpers
{
    /// <summary>
    /// That class helps to print exceptions and custom errors.
    /// </summary>
    public static class ErrorHelper
    {
        /// <summary>
        /// Shows current exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns></returns>
        public static async ValueTask ShowException(Exception exception)
        {
            await DialogHost.Show(new MessageBoxDialogViewModel(exception.Message));

            #if DEBUG
            if (exception.InnerException != null) await DialogHost.Show(new MessageBoxDialogViewModel(exception.InnerException.Message));
            #endif
        }

        /// <summary>
        /// Shows current error and exception, if it was thrown.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="exception">Exception.</param>
        /// <returns><see langword="false"/>.</returns>
        public static async ValueTask<bool> ShowError(string errorMessage, Exception exception)
        {
            await DialogHost.Show(new MessageBoxDialogViewModel(errorMessage));

            #if DEBUG
            if (exception != null) await DialogHost.Show(new MessageBoxDialogViewModel(exception.Message));
            #endif

            return false;
        }
    }
}
