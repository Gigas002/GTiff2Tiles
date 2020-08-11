using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

// ReSharper disable NotAccessedVariable
// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates
{
    [TestFixture]
    public sealed class PixelCoordinateTests
    {
        #region Constructors

        [Test]
        public void CreatePixelCoordinateNormal() => Assert.DoesNotThrow(() =>
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);
        });

        [Test]
        public void CreatePixelCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            PixelCoordinate coord = new PixelCoordinate(-1.0, 0.0);
        });

        [Test]
        public void CreatePixelCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, -1.0);
        });

        #endregion

        #region Methods

        #region ToNumber

        [Test]
        public void ToNumberNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(1234 * Tile.DefaultSize.Width, 123 * Tile.DefaultSize.Height);

            Assert.DoesNotThrow(() => coord.ToNumber(11, Tile.DefaultSize, false));
        }

        [Test]
        public void ToNumberSmallZ()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentOutOfRangeException>(() => coord.ToNumber(-1, Tile.DefaultSize, false));
        }

        [Test]
        public void ToNumberNullTileSize()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentNullException>(() => coord.ToNumber(10, null, false));
        }

        [Test]
        public void ToNumberNotSqureTileSize()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);
            Size size = new Size(10, 20);

            Assert.Throws<ArgumentException>(() => coord.ToNumber(10, size, false));
        }

        #endregion

        [Test]
        public void ToGeoCoordinateTest()
        {
            PixelCoordinate coord = new PixelCoordinate(1234 * Tile.DefaultSize.Width, 123 * Tile.DefaultSize.Height);
            GeoCoordinate geo = null;

            Assert.DoesNotThrow(() => geo = coord.ToGeoCoordinate(CoordinateSystem.Epsg4326, 11, Tile.DefaultSize));
            Assert.DoesNotThrow(() => geo = coord.ToGeoCoordinate(CoordinateSystem.Epsg3857, 11, Tile.DefaultSize));
            Assert.Throws<NotSupportedException>(() => coord.ToGeoCoordinate(CoordinateSystem.Other, 11, Tile.DefaultSize));
        }

        #region ToGeodeticCoordinate

        [Test]
        public void ToGeodeticCoordinateNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(1234 * Tile.DefaultSize.Width, 123 * Tile.DefaultSize.Height);

            Assert.DoesNotThrow(() => coord.ToGeodeticCoordinate(11, Tile.DefaultSize));
        }

        [Test]
        public void ToGeodeticCoordinateSmallZ()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentOutOfRangeException>(() => coord.ToGeodeticCoordinate(-1, Tile.DefaultSize));
        }

        #endregion

        #region ToMercatorCoordinate

        [Test]
        public void ToMercatorCoordinateNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(1234 * Tile.DefaultSize.Width, 123 * Tile.DefaultSize.Height);

            Assert.DoesNotThrow(() => coord.ToMercatorCoordinate(11, Tile.DefaultSize));
        }

        [Test]
        public void ToMercatorCoordinateSmallZ()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentOutOfRangeException>(() => coord.ToMercatorCoordinate(-1, Tile.DefaultSize));
        }

        #endregion

        #region ToRasterPixelCoordinate

        [Test]
        public void ToRasterPixelCoordinateNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.DoesNotThrow(() =>
            {
                PixelCoordinate pCoord = coord.ToRasterPixelCoordinate(10, Tile.DefaultSize);
            });
        }

        [Test]
        public void ToRasterPixelCoordinateSmallZ()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                PixelCoordinate pCoord = coord.ToRasterPixelCoordinate(-1, Tile.DefaultSize);
            });
        }

        [Test]
        public void ToRasterPixelCoordinateNullTileSize()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                PixelCoordinate pCoord = coord.ToRasterPixelCoordinate(10, null);
            });
        }

        [Test]
        public void ToRasterPixelCoordinateNotSquareTileSize()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);
            Size size = new Size(10, 20);

            Assert.Throws<ArgumentException>(() =>
            {
                PixelCoordinate pCoord = coord.ToRasterPixelCoordinate(10, size);
            });
        }

        #endregion

        #endregion
    }
}
