using System;
using System.Diagnostics;
using GTiff2Tiles.Core.Localization;

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Class with methods to simplify progress-reporting
    /// </summary>
    public static class ProgressHelper
    {
        /// <summary>
        /// Prints estimated time left
        /// </summary>
        /// <param name="percentage">Current progress</param>
        /// <param name="stopwatch">Time passed from the start;
        /// <remarks>If set to <see langword="null"/> no time printed</remarks></param>
        public static void PrintEstimatedTimeLeft(double percentage, Stopwatch stopwatch = null)
        {
            if (stopwatch == null) return;

            double timePassed = stopwatch.ElapsedMilliseconds;
            double estimatedAllTime = 100.0 * timePassed / percentage;
            double estimatedTimeLeft = estimatedAllTime - timePassed;
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(estimatedTimeLeft);
            Console.WriteLine(Strings.EstimatedTime, Environment.NewLine, timeSpan.Days, timeSpan.Hours,
                              timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}
