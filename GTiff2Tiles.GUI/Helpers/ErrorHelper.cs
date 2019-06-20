using System;
using System.Windows;

namespace GTiff2Tiles.GUI.Helpers
{
    public static class ErrorHelper
    {
        //TODO: replace message boxes

        /// <summary>
        /// Shows current exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        public static void ShowException(Exception exception)
        {
            MessageBox.Show(exception.Message);

            #if DEBUG
            if (exception.InnerException != null) MessageBox.Show(exception.InnerException.Message);
            #endif
        }

        /// <summary>
        /// Shows current error and exception, if it was thrown.
        /// </summary>
        /// <param name="errorMessage">Error message.</param>
        /// <param name="exception">Exception.</param>
        /// <returns><see langword="false"/>.</returns>
        public static bool ShowError(string errorMessage, Exception exception)
        {
            MessageBox.Show(errorMessage);

            #if DEBUG
            if (exception != null) MessageBox.Show(exception.Message);
            #endif

            return false;
        }
    }
}
