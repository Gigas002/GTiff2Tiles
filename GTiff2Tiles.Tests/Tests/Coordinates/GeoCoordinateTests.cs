using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates
{
    [TestFixture]
    public sealed class GeoCoordinateTests
    {
        // Only static things tests here. Everything else is tested on derived classes

        #region ToNumber

        [Test]
        public void ToNumberNormal()
        {
            GeoCoordinate coord = new GeodeticCoordinate(0.0, 0.0);

            Assert.DoesNotThrow(() =>
            {
                Number num = coord.ToNumber(10, Tile.DefaultSize, false);
            });
        }

        [Test]
        public void ToNumberSmallZ()
        {
            GeoCoordinate coord = new GeodeticCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Number num = coord.ToNumber(-1, Tile.DefaultSize, false);
            });
        }

        #endregion

        #region GetNumbers

        [Test]
        public void GetNumbersNormal()
        {
            GeodeticCoordinate minCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxCoord = new GeodeticCoordinate(0.0, 0.0);

            Assert.DoesNotThrow(() => GeoCoordinate.GetNumbers(minCoord, maxCoord, 10, Tile.DefaultSize, false));
        }

        [Test]
        public void GetNumbersNullMinCoordinate()
        {
            GeodeticCoordinate maxCoord = new GeodeticCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentNullException>(() => GeoCoordinate.GetNumbers(null, maxCoord, 10, Tile.DefaultSize, false));
        }

        [Test]
        public void GetNumbersNullMaxCoordinate()
        {
            GeodeticCoordinate minCoord = new GeodeticCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentNullException>(() => GeoCoordinate.GetNumbers(minCoord, null, 10, Tile.DefaultSize, false));
        }

        #endregion

        #region Resolution

        [Test]
        public void ResolutionTests()
        {
            Assert.DoesNotThrow(() => GeoCoordinate.Resolution(10, Tile.DefaultSize, CoordinateSystem.Epsg4326));
            Assert.DoesNotThrow(() => GeoCoordinate.Resolution(10, Tile.DefaultSize, CoordinateSystem.Epsg3857));
            Assert.Throws<NotSupportedException>(() => GeoCoordinate.Resolution(10, Tile.DefaultSize, CoordinateSystem.Other));
        }

        #endregion

        #region ZoomForPixelSize

        [Test]
        public void ZoomForPixelSizeNormal()
        {
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.DoesNotThrow(() =>
            {
                int z = GeoCoordinate.ZoomForPixelSize(1, Tile.DefaultSize, cs, 0, 10);
            });
        }

        [Test]
        public void ZoomForPixelSizeBadPixelSize()
        {
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.Throws<ArgumentOutOfRangeException>(() => GeoCoordinate.ZoomForPixelSize(0, Tile.DefaultSize, cs, 0, 10));
        }

        [Test]
        public void ZoomForPixelSizeBadMinZ()
        {
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.Throws<ArgumentOutOfRangeException>(() => GeoCoordinate.ZoomForPixelSize(1, Tile.DefaultSize, cs, -1, 10));
        }

        [Test]
        public void ZoomForPixelSizeBadMaxZ()
        {
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.Throws<ArgumentOutOfRangeException>(() => GeoCoordinate.ZoomForPixelSize(1, Tile.DefaultSize, cs, 10, 0));
        }

        #endregion
    }
}
