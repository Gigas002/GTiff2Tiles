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
        /// <returns></returns>
        internal static Task<object> ShowExceptionAsync(Exception exception)
        {
            #if DEBUG
            if (exception.InnerException != null) DialogHost.Show(new MessageBoxDialogViewModel(exception.InnerException.Message));
            #endif

            return DialogHost.Show(new MessageBoxDialogViewModel(exception.Message));
        }

        /// <summary>
        /// Shows current error and exception, if it was thrown
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <param name="exception">Exception</param>
        /// <returns><see langword="false"/></returns>
        internal static ValueTask<bool> ShowErrorAsync(string errorMessage, Exception exception = null)
        {
            #if DEBUG
            if (exception != null) DialogHost.Show(new MessageBoxDialogViewModel(exception.Message)).ConfigureAwait(true);
            #endif

            DialogHost.Show(new MessageBoxDialogViewModel(errorMessage));

            return ValueTask.FromResult(false);
        }
    }
}
