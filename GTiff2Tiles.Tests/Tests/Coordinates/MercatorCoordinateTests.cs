using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

// ReSharper disable RedundantAssignment
// ReSharper disable NotAccessedVariable
// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates
{
    [TestFixture]
    public sealed class MercatorCoordinateTests
    {
        private const double TokyoLongitude = 15566858.37;

        private const double TokyoLatitude = 4252956.14;

        #region Constructors

        [Test]
        public void CreateMercatorCoordinateNormal() => Assert.DoesNotThrow(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(0.0, 0.0);
        });

        [Test]
        public void CreateMercatorCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(-20046377.0, 0.0);
        });

        [Test]
        public void CreateMercatorCoordinateBigLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(20046377.0, 0.0);
        });

        [Test]
        public void CreateMercatorCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(0.0, -20048967.0);
        });

        [Test]
        public void CreateMercatorCoordinateBigLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(0.0, 20048967.0);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrow(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(0.0, 0.0);
            double val = MercatorCoordinate.MinPossibleLonValue;
            val = MercatorCoordinate.MaxPossibleLonValue;
            val = MercatorCoordinate.MinPossibleLatValue;
            val = MercatorCoordinate.MaxPossibleLatValue;
            val = coord.X;
            val = coord.Y;
        });

        #endregion

        #region Methods

        [Test]
        public void ToPixelCoordinateTest()
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, TokyoLatitude);

            PixelCoordinate realCoord = new PixelCoordinate(232899.99305018864, 158891.99925568007);
            PixelCoordinate pCoord = null;

            Assert.DoesNotThrow(() => pCoord = coord.ToPixelCoordinate(10, Tile.DefaultSize));
            Assert.True(pCoord == realCoord);
        }

        [Test]
        public void ToGeodeticCoordinateTest()
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, TokyoLatitude);

            GeodeticCoordinate realCoord = new GeodeticCoordinate(139.839468, 35.652832);
            Coordinate gCoord = null;

            Assert.DoesNotThrow(() => gCoord = (Coordinate)coord.ToGeodeticCoordinate().Round(6));
            Assert.True(gCoord == realCoord);
        }

        [Test]
        public void ToNumber()
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, TokyoLatitude);

            Number realNumber = new Number(909, 403, 10);
            Number number = null;

            Assert.DoesNotThrow(() => number = coord.ToNumber(10, Tile.DefaultSize, false));
            Assert.True(number == realNumber);
        }

        #region Resolution

        [Test]
        public void ResolutionNormal() => Assert.DoesNotThrow(() =>
        {
            double res = MercatorCoordinate.Resolution(10, Tile.DefaultSize);
        });

        [Test]
        public void ResolutionSmallZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            double res = MercatorCoordinate.Resolution(-1, Tile.DefaultSize);
        });

        [Test]
        public void ResolutionNullTileSize() => Assert.Throws<ArgumentNullException>(() =>
        {
            double res = MercatorCoordinate.Resolution(10, null);
        });

        [Test]
        public void ResolutionNotSquareTileSize() => Assert.Throws<ArgumentException>(() =>
        {
            Size size = new Size(10, 20);
            double res = MercatorCoordinate.Resolution(10, size);
        });

        #endregion

        #endregion
    }
}
