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
        #region Constructors

        [Test]
        public void CreateMercatorCoordinateNormal() => Assert.DoesNotThrow(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(0.0, 0.0);
        });

        [Test]
        public void CreateMercatorCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(-20026377.0, 0.0);
        });

        [Test]
        public void CreateMercatorCoordinateBigLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(20026377.0, 0.0);
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
            MercatorCoordinate coord = new MercatorCoordinate(0.0, 0.0);

            Assert.DoesNotThrow(() =>
            {
                PixelCoordinate pCoord = coord.ToPixelCoordinate(10, Tile.DefaultSize);
            });
        }

        [Test]
        public void ToGeodeticCoordinateTest()
        {
            MercatorCoordinate coord = new MercatorCoordinate(0.0, 0.0);

            Assert.DoesNotThrow(() =>
            {
                GeodeticCoordinate gCoord = coord.ToGeodeticCoordinate();
            });
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
