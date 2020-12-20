using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.TileMapResource;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources
{
    [TestFixture]
    public sealed class Origintests
    {
        #region Consts

        private static readonly ICoordinate OriginCoordinate = new GeodeticCoordinate(-180.0, -90.0);

        #endregion

        #region Constructors

        [Test]
        public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new Origin());

        [Test]
        public void FromDouble()
        {
            Origin origin = null;

            Assert.DoesNotThrow(() => origin = new(OriginCoordinate.X, OriginCoordinate.Y));

            Assert.True(Math.Abs(origin.X - OriginCoordinate.X) < double.Epsilon &&
                        Math.Abs(origin.Y - OriginCoordinate.Y) < double.Epsilon);
        }

        [Test]
        public void FromCoordinateNormal()
        {
            Origin origin = null;

            Assert.DoesNotThrow(() => origin = new(OriginCoordinate));

            Assert.True(Math.Abs(origin.X - OriginCoordinate.X) < double.Epsilon &&
                        Math.Abs(origin.Y - OriginCoordinate.Y) < double.Epsilon);
        }

        [Test]
        public void FromCoordinateBad() => Assert.Throws<ArgumentNullException>(() => _ = new Origin(null));

        #endregion
    }
}
