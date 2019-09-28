using System;
using System.Threading.Tasks;
using GTiff2Tiles.GUI.ViewModels;
using MaterialDesignThemes.Wpf;

namespace GTiff2Tiles.GUI.Helpers
{
    /// <summary>
    /// That class helps to print exceptions and custom errors.
    /// </summary>
    internal static class ErrorHelper
    {
        /// <summary>
        /// Shows current exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <returns></returns>
        internal static async ValueTask ShowExceptionAsync(Exception exception)
        {
            await DialogHost.Show(new MessageBoxDialogViewModel(exception.Message)).ConfigureAwait(false);

            #if DEBUG
            if (exception.InnerException != null)
                await DialogHost.Show(new MessageBoxDialogViewModel(exception.InnerException.Message))
                                .ConfigureAwait(false);
            #endif
        }

        /// <summary>
        /// Shows current error and exception, if it was thrown.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="exception">Exception.</param>
        /// <returns><see langword="false"/>.</returns>
        internal static async ValueTask<bool> ShowErrorAsync(string errorMessage, Exception exception = null)
        {
            await DialogHost.Show(new MessageBoxDialogViewModel(errorMessage)).ConfigureAwait(false);

            #if DEBUG
            if (exception != null)
                await DialogHost.Show(new MessageBoxDialogViewModel(exception.Message)).ConfigureAwait(false);
            #endif

            return false;
        }
    }
}
