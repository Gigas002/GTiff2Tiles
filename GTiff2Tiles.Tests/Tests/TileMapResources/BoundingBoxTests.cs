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

        private readonly ICoordinate _tokyoGeodeticMin = Locations.TokyoGeodeticMin;

        private readonly ICoordinate _tokyoGeodeticMax = Locations.TokyoGeodeticMax;

        #endregion

        #region Constructors

        [Test]
        public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new BoundingBox());

        [Test]
        public void FromDouble()
        {
            BoundingBox boundingBox = null;

            Assert.DoesNotThrow(() => boundingBox = new(_tokyoGeodeticMin.X, _tokyoGeodeticMin.Y, _tokyoGeodeticMax.X,
                                                        _tokyoGeodeticMax.Y));

            Assert.True(Math.Abs(boundingBox.MinX - _tokyoGeodeticMin.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MinY - _tokyoGeodeticMin.Y) < double.Epsilon);

            Assert.True(Math.Abs(boundingBox.MaxX - _tokyoGeodeticMax.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MaxY - _tokyoGeodeticMax.Y) < double.Epsilon);
        }

        [Test]
        public void FromCoordinatesNormal()
        {
            BoundingBox boundingBox = null;

            Assert.DoesNotThrow(() => boundingBox = new(_tokyoGeodeticMin, _tokyoGeodeticMax));

            Assert.True(Math.Abs(boundingBox.MinX - _tokyoGeodeticMin.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MinY - _tokyoGeodeticMin.Y) < double.Epsilon);

            Assert.True(Math.Abs(boundingBox.MaxX - _tokyoGeodeticMax.X) < double.Epsilon &&
                        Math.Abs(boundingBox.MaxY - _tokyoGeodeticMax.Y) < double.Epsilon);
        }

        [Test]
        public void FromCoordinatesBad1() => Assert.Throws<ArgumentNullException>(() => _ = new BoundingBox(null, _tokyoGeodeticMax));

        [Test]
        public void FromCoordinatesBad2() => Assert.Throws<ArgumentNullException>(() => _ = new BoundingBox(_tokyoGeodeticMin, null));

        #endregion
    }
}
