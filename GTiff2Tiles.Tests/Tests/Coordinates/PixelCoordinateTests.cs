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
        private const double GeodeticPixelLongitude = 465800.00067128887;

        private const double GeodeticPixelLatitude = 182995.19995448887;

        private const double MercatorPixelLongitude = 232899.99305018864;

        private const double MercatorPixelLatitude = 158891.99925568007;

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
            PixelCoordinate coord = new PixelCoordinate(GeodeticPixelLongitude, GeodeticPixelLatitude);

            Number expected = new Number(1819, 309, 10);
            Number res = null;

            Assert.DoesNotThrow(() => res = coord.ToNumber(10, Tile.DefaultSize, false));
            Assert.True(res == expected);
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

        #region ToGeoCoordinate

        [Test]
        public void ToGeoCoordinateFromGeodeticPixels()
        {
            PixelCoordinate coord = new PixelCoordinate(GeodeticPixelLongitude, GeodeticPixelLatitude);

            GeodeticCoordinate realGCoordinate = new GeodeticCoordinate(139.839478, 35.652832);
            MercatorCoordinate realMCoordinate = new MercatorCoordinate(15566859.48, 4252956.14);

            Coordinate geodeticCoordinate = null;
            Coordinate mercatorCoordinate = null;

            Assert.DoesNotThrow(() => geodeticCoordinate = (Coordinate)coord.ToGeoCoordinate(CoordinateSystem.Epsg4326,
                                                                  CoordinateSystem.Epsg4326, 10, Tile.DefaultSize).Round(6));
            Assert.DoesNotThrow(() => mercatorCoordinate = (Coordinate)coord.ToGeoCoordinate(CoordinateSystem.Epsg4326,
                                                                  CoordinateSystem.Epsg3857, 10, Tile.DefaultSize).Round(2));

            Assert.True(geodeticCoordinate == realGCoordinate);
            Assert.True(mercatorCoordinate == realMCoordinate);

            Assert.Throws<NotSupportedException>(() => coord.ToGeoCoordinate(CoordinateSystem.Epsg4326,
                                                                             CoordinateSystem.Other, 10, Tile.DefaultSize));
        }

        [Test]
        public void ToGeoCoordinateFromMercatorPixels()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);

            GeodeticCoordinate realGCoordinate = new GeodeticCoordinate(139.839468, 35.652832);
            MercatorCoordinate realMCoordinate = new MercatorCoordinate(15566858.37, 4252956.14);

            Coordinate geodeticCoordinate = null;
            Coordinate mercatorCoordinate = null;

            Assert.DoesNotThrow(() => geodeticCoordinate = (Coordinate)coord.ToGeoCoordinate(CoordinateSystem.Epsg3857,
                                                                                 CoordinateSystem.Epsg4326, 10, Tile.DefaultSize).Round(6));
            Assert.DoesNotThrow(() => mercatorCoordinate = (Coordinate)coord.ToGeoCoordinate(CoordinateSystem.Epsg3857,
                                                                                 CoordinateSystem.Epsg3857, 10, Tile.DefaultSize).Round(2));

            Assert.True(geodeticCoordinate == realGCoordinate);
            Assert.True(mercatorCoordinate == realMCoordinate);

            Assert.Throws<NotSupportedException>(() => coord.ToGeoCoordinate(CoordinateSystem.Epsg3857,
                                                                             CoordinateSystem.Other, 10, Tile.DefaultSize));
        }

        #endregion

        #region ToGeodeticCoordinate

        [Test]
        public void ToGeodeticCoordinateNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(GeodeticPixelLongitude, GeodeticPixelLatitude);

            GeodeticCoordinate realGCoordinate = new GeodeticCoordinate(139.839478, 35.652832);
            Coordinate res = null;

            Assert.DoesNotThrow(() => res = (Coordinate)coord.ToGeodeticCoordinate(CoordinateSystem.Epsg4326, 10, Tile.DefaultSize).Round(6));
            Assert.True(res == realGCoordinate);
        }

        [Test]
        public void ToGeodeticCoordinateOtherCs()
        {
            PixelCoordinate coord = new PixelCoordinate(GeodeticPixelLongitude, GeodeticPixelLatitude);

            Assert.Throws<NotSupportedException>(() => coord.ToGeodeticCoordinate(CoordinateSystem.Other, 10,
                                                                                  Tile.DefaultSize));
        }

        [Test]
        public void ToGeodeticCoordinateSmallZ()
        {
            PixelCoordinate coord = new PixelCoordinate(GeodeticPixelLongitude, GeodeticPixelLatitude);

            Assert.Throws<ArgumentOutOfRangeException>(() => coord.ToGeodeticCoordinate(CoordinateSystem.Epsg4326,
                                                                                        -1, Tile.DefaultSize));
        }

        #endregion

        #region ToMercatorCoordinate

        [Test]
        public void ToMercatorCoordinateNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);
            MercatorCoordinate realMCoordinate = new MercatorCoordinate(15566858.37, 4252956.14);

            Coordinate res = null;
            Assert.DoesNotThrow(() => res = (Coordinate)coord.ToMercatorCoordinate(CoordinateSystem.Epsg3857, 10, Tile.DefaultSize).Round(2));
            Assert.True(res == realMCoordinate);
        }

        [Test]
        public void ToMercatorCoordinateOtherCs()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);

            Assert.Throws<NotSupportedException>(() => coord.ToMercatorCoordinate(CoordinateSystem.Other, 10,
                                                                                  Tile.DefaultSize));
        }

        [Test]
        public void ToMercatorCoordinateSmallZ()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);

            Assert.Throws<ArgumentOutOfRangeException>(() => coord.ToMercatorCoordinate(CoordinateSystem.Epsg3857,
                                                                                        -1, Tile.DefaultSize));
        }

        #endregion

        #region ToRasterPixelCoordinate

        [Test]
        public void ToRasterPixelCoordinateNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);

            Assert.DoesNotThrow(() =>
            {
                PixelCoordinate pCoord = coord.ToRasterPixelCoordinate(10, Tile.DefaultSize);
            });
        }

        [Test]
        public void ToRasterPixelCoordinateSmallZ()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                PixelCoordinate pCoord = coord.ToRasterPixelCoordinate(-1, Tile.DefaultSize);
            });
        }

        [Test]
        public void ToRasterPixelCoordinateNullTileSize()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);

            Assert.Throws<ArgumentNullException>(() =>
            {
                PixelCoordinate pCoord = coord.ToRasterPixelCoordinate(10, null);
            });
        }

        [Test]
        public void ToRasterPixelCoordinateNotSquareTileSize()
        {
            PixelCoordinate coord = new PixelCoordinate(MercatorPixelLongitude, MercatorPixelLatitude);
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
