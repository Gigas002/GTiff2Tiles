using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
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
            GeodeticCoordinate coord =
                new GeodeticCoordinate(Locations.TokyoGeodeticLongitude, Locations.TokyoGeodeticLatitude);
        });

        [Test]
        public void CreateGeodeticCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(-181.0, Locations.TokyoGeodeticLatitude);
        });

        [Test]
        public void CreateGeodeticCoordinateBigLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(181.0, Locations.TokyoGeodeticLatitude);
        });

        [Test]
        public void CreateGeodeticCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(Locations.TokyoGeodeticLongitude, -91.0);
        });

        [Test]
        public void CreateGeodeticCoordinateBigLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            GeodeticCoordinate coord = new GeodeticCoordinate(Locations.TokyoGeodeticLongitude, 91.0);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrow(() =>
        {
            GeodeticCoordinate coord =
                new GeodeticCoordinate(Locations.TokyoGeodeticLongitude, Locations.TokyoGeodeticLatitude);
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
            PixelCoordinate pCoord = null;

            Assert.DoesNotThrow(() => pCoord = Locations.TokyoGeodeticCoordinate
                                                        .ToPixelCoordinate(10, Tile.DefaultSize));
            Assert.True(pCoord == Locations.TokyoGeodeticPixelCoordinate);
        }

        [Test]
        public void ToMercatorCoordinateTest()
        {
            Coordinate mCoord = null;

            Assert.DoesNotThrow(() => mCoord = Coordinate.Round(Locations.TokyoGeodeticCoordinate
                                                                    .ToMercatorCoordinate(), 2));
            Assert.True(mCoord == Locations.TokyoMercatorCoordinate);
        }

        [Test]
        public void ToNumber()
        {
            Number number = null;

            Assert.DoesNotThrow(() => number = Locations.TokyoGeodeticCoordinate
                                                        .ToNumber(10, Tile.DefaultSize, false));
            Assert.True(number == Locations.TokyoGeodeticNtmsNumber);
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
