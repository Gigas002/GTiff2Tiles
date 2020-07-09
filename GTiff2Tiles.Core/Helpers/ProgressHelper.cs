using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Localization;

namespace GTiff2Tiles.Core.Helpers
{
    public class ProgressHelper
    {
        /// <summary>
        /// Prints estimated time left.
        /// </summary>
        /// <param name="percentage">Current progress.</param>
        /// <param name="stopwatch">Get elapsed time from this.</param>
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
