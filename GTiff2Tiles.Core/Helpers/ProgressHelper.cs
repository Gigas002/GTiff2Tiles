using System;
using System.Diagnostics;
using System.Globalization;
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

            if (percentage <= 0.0 || percentage > 100.0) throw new ArgumentOutOfRangeException(nameof(percentage));
            if (stopwatch == null) throw new ArgumentNullException(nameof(stopwatch));

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
        /// <param name="reporter">Delegate to work with reported string
        /// <remarks><para/>E.g. <see cref="Console.WriteLine(string)"/>; if set to <see langword="null"/> no time printed</remarks></param>
        public static void PrintEstimatedTimeLeft(double percentage, Stopwatch stopwatch = null, Action<string> reporter = null)
        {
            #region Preconditions checks

            // Don't print anything, no need to throw exception
            if (stopwatch == null) return;
            if (reporter == null) return;

            #endregion

            TimeSpan timeSpan = GetEstimatedTimeLeft(percentage, stopwatch);

            string reportString = string.Format(CultureInfo.InvariantCulture, Strings.EstimatedTime, Environment.NewLine,
                                       timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds,
                                       timeSpan.Milliseconds);
            reporter.Invoke(reportString);
        }
    }
}
