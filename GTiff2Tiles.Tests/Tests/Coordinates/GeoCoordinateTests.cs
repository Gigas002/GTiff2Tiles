using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates
{
    [TestFixture]
    public sealed class GeoCoordinateTests
    {
        #region Consts

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

        private const CoordinateSystem CsOther = CoordinateSystem.Other;

        #endregion

        // Only static things tests here. Everything else is tested on derived classes

        #region ToNumber

        [Test]
        public void ToNumberNormal()
        {
            GeoCoordinate coord = new GeodeticCoordinate(Locations.TokyoGeodeticLongitude,
                                                         Locations.TokyoGeodeticLatitude);

            Assert.DoesNotThrow(() =>
            {
                Number num = coord.ToNumber(10, Tile.DefaultSize, false);
            });
        }

        [Test]
        public void ToNumberSmallZ()
        {
            GeoCoordinate coord = new GeodeticCoordinate(Locations.TokyoGeodeticLongitude,
                                                         Locations.TokyoGeodeticLatitude);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Number num = coord.ToNumber(-1, Tile.DefaultSize, false);
            });
        }

        #endregion

        #region GetNumbers

        [Test]
        public void GetNumbersNormal() => Assert.DoesNotThrow(() => GeoCoordinate.GetNumbers(Locations.TokyoGeodeticMin, Locations.TokyoGeodeticMax, 10, Tile.DefaultSize, false));

        [Test]
        public void GetNumbersNullMinCoordinate() => Assert.Throws<ArgumentNullException>(() => GeoCoordinate.GetNumbers(null, Locations.TokyoGeodeticMax, 10, Tile.DefaultSize, false));

        [Test]
        public void GetNumbersNullMaxCoordinate() => Assert.Throws<ArgumentNullException>(() => GeoCoordinate.GetNumbers(Locations.TokyoGeodeticMin, null, 10, Tile.DefaultSize, false));

        #endregion

        #region Resolution

        [Test]
        public void ResolutionTests()
        {
            Assert.DoesNotThrow(() => GeoCoordinate.Resolution(10, Tile.DefaultSize, Cs4326));
            Assert.DoesNotThrow(() => GeoCoordinate.Resolution(10, Tile.DefaultSize, Cs3857));
            Assert.Throws<NotSupportedException>(() => GeoCoordinate.Resolution(10, Tile.DefaultSize, CsOther));
        }

        #endregion

        #region ZoomForPixelSize

        [Test]
        public void ZoomForPixelSizeNormal() => Assert.DoesNotThrow(() =>
        {
            int z = GeoCoordinate.ZoomForPixelSize(1, Tile.DefaultSize, Cs4326, 0, 10);
        });

        [Test]
        public void ZoomForPixelSizeBadPixelSize() => Assert.Throws<ArgumentOutOfRangeException>(() => GeoCoordinate.ZoomForPixelSize(0, Tile.DefaultSize, Cs4326, 0, 10));

        [Test]
        public void ZoomForPixelSizeBadMinZ() => Assert.Throws<ArgumentOutOfRangeException>(() => GeoCoordinate.ZoomForPixelSize(1, Tile.DefaultSize, Cs4326, -1, 10));

        [Test]
        public void ZoomForPixelSizeBadMaxZ() => Assert.Throws<ArgumentOutOfRangeException>(() => GeoCoordinate.ZoomForPixelSize(1, Tile.DefaultSize, Cs4326, 10, 0));

        #endregion
    }
}
