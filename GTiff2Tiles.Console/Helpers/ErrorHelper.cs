namespace GTiff2Tiles.Console.Helpers;

/// <summary>
/// Helps to print errors.
/// </summary>
internal static class ErrorHelper
{
    /// <summary>
    /// Prints current exception and waits for input (in DEBUG mode).
    /// </summary>
    /// <param name="exception">Exception.</param>
    internal static void PrintException(Exception exception)
    {
        System.Console.WriteLine(exception.Message);

        #if DEBUG
        if (exception.InnerException != null) System.Console.WriteLine(exception.InnerException.Message);
        #endif
    }

    /// <summary>
    /// Prints current error and waits for input (in DEBUG mode).
    /// </summary>
    /// <param name="errorMessage">Your error's message.</param>
    internal static void PrintError(string errorMessage) => System.Console.WriteLine(errorMessage);
}