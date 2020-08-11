using System;
using System.Diagnostics;
using GTiff2Tiles.Core.Localization;

// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Class with methods to simplify progress-reporting
    /// </summary>
    public static class ProgressHelper
    {
        /// <summary>
        /// Calculate estimated time left, based on your current progress and time from start
        /// </summary>
        /// <param name="percentage">Current progress;
        /// <remarks><para/>Should be in range (0.0, 100.0]</remarks></param>
        /// <param name="stopwatch">Time passed from the start</param>
        /// <returns>Estimated <see cref="TimeSpan"/> left</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static TimeSpan GetEstimatedTimeLeft(double percentage, Stopwatch stopwatch)
        {
            #region Preconditions checks

            if (stopwatch == null) throw new ArgumentNullException(nameof(stopwatch));
            if (percentage <= 0.0 || percentage > 100.0) throw new ArgumentOutOfRangeException(nameof(percentage));

            #endregion

            double timePassed = stopwatch.ElapsedMilliseconds;
            double estimatedAllTime = 100.0 * timePassed / percentage;
            double estimatedTimeLeft = estimatedAllTime - timePassed;

            return TimeSpan.FromMilliseconds(estimatedTimeLeft);
        }

        /// <summary>
        /// Prints estimated time left
        /// </summary>
        /// <param name="percentage">Current progress;
        /// <remarks><para/>Should be in range (0.0, 100.0]</remarks></param>
        /// <param name="stopwatch">Time passed from the start;
        /// <remarks><para/>If set to <see langword="null"/> no time printed</remarks></param>
        public static void PrintEstimatedTimeLeft(double percentage, Stopwatch stopwatch = null)
        {
            #region Preconditions checks

            // Don't print anything, no need to throw exception
            if (stopwatch == null) return;

            #endregion

            TimeSpan timeSpan = GetEstimatedTimeLeft(percentage, stopwatch);
            Console.WriteLine(Strings.EstimatedTime, Environment.NewLine, timeSpan.Days, timeSpan.Hours,
                              timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}
