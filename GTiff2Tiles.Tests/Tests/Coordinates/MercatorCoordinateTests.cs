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
        #region Consts

        private const double TokyoLongitude = 15566859.48;

        private const double TokyoLatitude = 4252956.14;

        private readonly Number _tokyoMercatorNumber = new Number(909, 403, 10);

        private readonly MercatorCoordinate _tokyoGeodeticCoordinate = new MercatorCoordinate(139.839478, 35.652832);

        private readonly PixelCoordinate _tokyoMercatorPixelCoordinate = new PixelCoordinate(232900.00031106747, 158891.99925568007);

        #endregion

        #region Constructors

        [Test]
        public void CreateMercatorCoordinateNormal() => Assert.DoesNotThrow(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, TokyoLatitude);
        });

        [Test]
        public void CreateMercatorCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(-20046377.0, TokyoLatitude);
        });

        [Test]
        public void CreateMercatorCoordinateBigLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(20046377.0, TokyoLatitude);
        });

        [Test]
        public void CreateMercatorCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, -20048967.0);
        });

        [Test]
        public void CreateMercatorCoordinateBigLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, 20048967.0);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrow(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, TokyoLatitude);
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
            PixelCoordinate pCoord = null;

            Assert.DoesNotThrow(() => pCoord = coord.ToPixelCoordinate(10, Tile.DefaultSize));
            Assert.True(pCoord == _tokyoMercatorPixelCoordinate);
        }

        [Test]
        public void ToGeodeticCoordinateTest()
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, TokyoLatitude);
            Coordinate gCoord = null;

            Assert.DoesNotThrow(() => gCoord = (Coordinate)coord.ToGeodeticCoordinate().Round(6));
            Assert.True(gCoord == _tokyoGeodeticCoordinate);
        }

        [Test]
        public void ToNumber()
        {
            MercatorCoordinate coord = new MercatorCoordinate(TokyoLongitude, TokyoLatitude);
            Number number = null;

            Assert.DoesNotThrow(() => number = coord.ToNumber(10, Tile.DefaultSize, false));
            Assert.True(number == _tokyoMercatorNumber);
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
