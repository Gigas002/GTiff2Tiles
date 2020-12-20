using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.TileMapResource;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources
{
    [TestFixture]
    public sealed class BoundingBoxTests
    {
        #region Consts

        private static readonly ICoordinate TokyoGeodeticMin = Locations.TokyoGeodeticMin;

        private static readonly ICoordinate TokyoGeodeticMax = Locations.TokyoGeodeticMax;

        #endregion

        #region Constructors

        [Test]
        public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new BoundingBox());

        [Test]
        public void FromDouble()
        {
            BoundingBox boundingBox = null;

            Assert.DoesNotThrow(() => boundingBox = new(TokyoGeodeticMin.X, TokyoGeodeticMin.Y, TokyoGeodeticMax.X,
                                                        TokyoGeodeticMax.Y));

            Assert.True(Math.Abs(boundingBox.MinX - TokyoGeodeticMin.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MinY - TokyoGeodeticMin.Y) < double.Epsilon);

            Assert.True(Math.Abs(boundingBox.MaxX - TokyoGeodeticMax.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MaxY - TokyoGeodeticMax.Y) < double.Epsilon);
        }

        [Test]
        public void FromCoordinatesNormal()
        {
            BoundingBox boundingBox = null;

            Assert.DoesNotThrow(() => boundingBox = new(TokyoGeodeticMin, TokyoGeodeticMax));

            Assert.True(Math.Abs(boundingBox.MinX - TokyoGeodeticMin.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MinY - TokyoGeodeticMin.Y) < double.Epsilon);

            Assert.True(Math.Abs(boundingBox.MaxX - TokyoGeodeticMax.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MaxY - TokyoGeodeticMax.Y) < double.Epsilon);
        }

        [Test]
        public void FromCoordinatesBad1() => Assert.Throws<ArgumentNullException>(() => _ = new BoundingBox(null, TokyoGeodeticMax));

        [Test]
        public void FromCoordinatesBad2() => Assert.Throws<ArgumentNullException>(() => _ = new BoundingBox(TokyoGeodeticMin, null));

        #endregion
    }
}
