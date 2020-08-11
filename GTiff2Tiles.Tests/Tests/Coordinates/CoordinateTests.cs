using System;
using GTiff2Tiles.Core.Coordinates;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Coordinates
{
    [TestFixture]
    public sealed class CoordinateTests
    {
        // Only static things tests here. Everything else is tested on derived classes

        [Test]
        public void DegreesToRadiansTest()
        {
            const double expected = 1.5708;
            double res = Math.Round(Coordinate.DegreesToRadians(90.0), 4);

            Assert.True(Math.Abs(expected - res) < double.Epsilon);
        }

        [Test]
        public void RadiansToDegreesTest()
        {
            const double expected = 57.2958;
            double res = Math.Round(Coordinate.RadiansToDegrees(1.0), 4);

            Assert.True(Math.Abs(expected - res) < double.Epsilon);
        }
    }
}
