using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

// ReSharper disable NotAccessedVariable
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates
{
    [TestFixture]
    public sealed class GeodeticCoordinateTests
    {
        #region Consts

        private const double TokyoLongitude = 139.839478;

        private const double TokyoLatitude = 35.652832;

        private readonly Number _tokyoGeodeticNumber = new Number(1819, 309, 10);

        private readonly MercatorCoordinate _tokyoMercatorCoordinate = new MercatorCoordinate(15566859.48, 4252956.14);

        private readonly PixelCoordinate _tokyoGeodeticPixelCoordinate = new PixelCoordinate(465800.00067128887, 182995.19995448887);

        #endregion

        #region Constructors

        [Test]
        public void CreateGeodeticCoordinateNormal() => Assert.DoesNotThrow(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(TokyoLongitude, TokyoLatitude);
        });

        [Test]
        public void CreateGeodeticCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(-181.0, TokyoLatitude);
        });

        [Test]
        public void CreateGeodeticCoordinateBigLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(181.0, TokyoLatitude);
        });

        [Test]
        public void CreateGeodeticCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(TokyoLongitude, -91.0);
        });

        [Test]
        public void CreateGeodeticCoordinateBigLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(TokyoLongitude, 91.0);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrow(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(0.0, 0.0);
            double val = GeodeticCoordinate.MinPossibleLonValue;
            val = GeodeticCoordinate.MaxPossibleLonValue;
            val = GeodeticCoordinate.MinPossibleLatValue;
            val = GeodeticCoordinate.MaxPossibleLatValue;
            val = coord.Latitude;
            val = coord.Longitude;
            val = coord.X;
            val = coord.Y;
        });

        #endregion

        #region Methods

        [Test]
        public void ToPixelCoordinateTest()
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(TokyoLongitude, TokyoLatitude);
            PixelCoordinate pCoord = null;

            Assert.DoesNotThrow(() => pCoord = coord.ToPixelCoordinate(10, Tile.DefaultSize));
            Assert.True(pCoord == _tokyoGeodeticPixelCoordinate);
        }

        [Test]
        public void ToMercatorCoordinateTest()
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(TokyoLongitude, TokyoLatitude);
            Coordinate mCoord = null;

            Assert.DoesNotThrow(() => mCoord = (Coordinate)coord.ToMercatorCoordinate().Round(2));
            Assert.True(mCoord == _tokyoMercatorCoordinate);
        }

        [Test]
        public void ToNumber()
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(TokyoLongitude, TokyoLatitude);
            Number number = null;

            Assert.DoesNotThrow(() => number = coord.ToNumber(10, Tile.DefaultSize, false));
            Assert.True(number == _tokyoGeodeticNumber);
        }

        #region Resolution

        [Test]
        public void ResolutionNormal() => Assert.DoesNotThrow(() =>
        {
            double res = GeodeticCoordinate.Resolution(10, Tile.DefaultSize);
        });

        [Test]
        public void ResolutionSmallZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            double res = GeodeticCoordinate.Resolution(-1, Tile.DefaultSize);
        });

        [Test]
        public void ResolutionNullTileSize() => Assert.Throws<ArgumentNullException>(() =>
        {
            double res = GeodeticCoordinate.Resolution(10, null);
        });

        [Test]
        public void ResolutionNotSquareTileSize() => Assert.Throws<ArgumentException>(() =>
        {
            Size size = new Size(10, 20);
            double res = GeodeticCoordinate.Resolution(10, size);
        });

        #endregion

        #endregion
    }
}
