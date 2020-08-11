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
        #region Constructors

        [Test]
        public void CreateGeodeticCoordinateNormal() => Assert.DoesNotThrow(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(0.0, 0.0);
        });

        [Test]
        public void CreateGeodeticCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(-181.0, 0.0);
        });

        [Test]
        public void CreateGeodeticCoordinateBigLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(181.0, 0.0);
        });

        [Test]
        public void CreateGeodeticCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(0.0, -91.0);
        });

        [Test]
        public void CreateGeodeticCoordinateBigLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(0.0, 91.0);
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
            GeodeticCoordinate coord = new GeodeticCoordinate(0.0, 0.0);
            Assert.DoesNotThrow(() =>
            {
                PixelCoordinate pCoord = coord.ToPixelCoordinate(10, Tile.DefaultSize);
            });
        }

        [Test]
        public void ToMercatorCoordinateTest()
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(0.0, 0.0);
            Assert.DoesNotThrow(() =>
            {
                MercatorCoordinate mCoord = coord.ToMercatorCoordinate();
            });
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
