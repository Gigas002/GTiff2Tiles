using System;
using System.Diagnostics;
using GTiff2Tiles.Core.Helpers;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Helpers
{
    [TestFixture]
    public class ProgressHelperTests
    {
        #region GetEstimatedTimeLeft

        [Test]
        public void GetEstimatedTimeLeftNormal()
        {
            const double prc = 10.0;
            Stopwatch sw = Stopwatch.StartNew();

            Assert.DoesNotThrow(() => ProgressHelper.GetEstimatedTimeLeft(prc, sw));
        }

        [Test]
        public void GetEstimatedTimeLeftSmallProgress()
        {
            const double prc = 0.0;
            Stopwatch sw = Stopwatch.StartNew();

            Assert.Throws<ArgumentOutOfRangeException>(() => ProgressHelper.GetEstimatedTimeLeft(prc, sw));
        }

        [Test]
        public void GetEstimatedTimeLeftBigProgress()
        {
            const double prc = 101.0;
            Stopwatch sw = Stopwatch.StartNew();

            Assert.Throws<ArgumentOutOfRangeException>(() => ProgressHelper.GetEstimatedTimeLeft(prc, sw));
        }

        [Test]
        public void GetEstimatedTimeLeftNullStopwatch()
        {
            const double prc = 10.0;

            Assert.Throws<ArgumentNullException>(() => ProgressHelper.GetEstimatedTimeLeft(prc, null));
        }

        #endregion

        #region PrintExtimatedTimeLeft

        [Test]
        public void PrintEstimatedTimeLeftNormal()
        {
            const double prc = 10.0;
            Stopwatch sw = Stopwatch.StartNew();

            Assert.DoesNotThrow(() => ProgressHelper.PrintEstimatedTimeLeft(prc, sw));
        }

        [Test]
        public void PrintEstimatedTimeLeftNullStopwatch()
        {
            const double prc = 10.0;

            Assert.DoesNotThrow(() => ProgressHelper.PrintEstimatedTimeLeft(prc));
        }

        #endregion
    }
}
