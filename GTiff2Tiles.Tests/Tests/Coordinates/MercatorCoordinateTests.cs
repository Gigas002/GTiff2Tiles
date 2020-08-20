using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
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
            MercatorCoordinate coord =
                new MercatorCoordinate(Locations.TokyoMercatorLongitude, Locations.TokyoMercatorLatitude);
        });

        [Test]
        public void CreateMercatorCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(-20046377.0, Locations.TokyoMercatorLatitude);
        });

        [Test]
        public void CreateMercatorCoordinateBigLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(20046377.0, Locations.TokyoMercatorLatitude);
        });

        [Test]
        public void CreateMercatorCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(Locations.TokyoMercatorLongitude, -20048967.0);
        });

        [Test]
        public void CreateMercatorCoordinateBigLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            MercatorCoordinate coord = new MercatorCoordinate(Locations.TokyoMercatorLongitude, 20048967.0);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrow(() =>
        {
            MercatorCoordinate coord =
                new MercatorCoordinate(Locations.TokyoMercatorLongitude, Locations.TokyoMercatorLatitude);
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
            PixelCoordinate pCoord = null;

            Assert.DoesNotThrow(() => pCoord = Locations.TokyoMercatorCoordinate
                                                        .ToPixelCoordinate(10, Tile.DefaultSize));
            Assert.True(pCoord == Locations.TokyoMercatorPixelCoordinate);
        }

        [Test]
        public void ToGeodeticCoordinateTest()
        {
            Coordinate gCoord = null;

            Assert.DoesNotThrow(() => gCoord = Coordinate.Round(Locations.TokyoMercatorCoordinate
                                                                    .ToGeodeticCoordinate(), 6));
            Assert.True(gCoord == Locations.TokyoGeodeticCoordinate);
        }

        [Test]
        public void ToNumber()
        {
            Number number = null;

            Assert.DoesNotThrow(() => number = Locations.TokyoMercatorCoordinate
                                                        .ToNumber(10, Tile.DefaultSize, false));
            Assert.True(number == Locations.TokyoMercatorNtmsNumber);
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
