using System;

namespace GTiff2Tiles.Console.Helpers
{
    /// <summary>
    /// Helps to print errors.
    /// </summary>
    public static class ErrorHelper
    {
        /// <summary>
        /// Prints current exception.
        /// </summary>
        /// <param name="exception">Exception.</param>
        public static void PrintException(Exception exception)
        {
            System.Console.WriteLine(exception.Message);

            #if DEBUG
            if (exception.InnerException != null)
                System.Console.WriteLine(exception.InnerException.Message);
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
            #endif
        }

        /// <summary>
        /// Prints current error.
        /// </summary>
        /// <param name="errorMessage"></param>
        public static void PrintError(string errorMessage)
        {
            System.Console.WriteLine(errorMessage);

            #if DEBUG
            System.Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();
            #endif
        }
    }
}
