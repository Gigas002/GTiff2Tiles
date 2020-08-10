#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0219 // The variable is assigned but it's value is never used
#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class TileTests
    {
        #region Setup

        [SetUp]
        public void Setup() { }

        #endregion

        #region Constructors

        #region FromNumber

        [Test]
        public async Task FromNumberDefaultArgs()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            Assert.Pass();
        }

        [Test]
        public async Task FromNumberOverrideDefaultArgs()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            Size size = new Size(256, 256);
            IEnumerable<byte> bytes = new byte[10];
            const TileExtension extension = TileExtension.Webp;
            const bool tmsCompatible = true;
            const int bandsCount = 3;
            const Interpolation interpolation = Interpolation.Cubic;

            await using ITile tile = new RasterTile(number, cs, size, bytes, extension, tmsCompatible,
                                                    bandsCount, interpolation);

            Assert.Pass();
        }

        [Test]
        public async Task FromNumberSmallBands()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            const int bandsCount = -1;

            try
            {
                await using ITile tile = new RasterTile(number, cs, bandsCount: bandsCount);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task FromNumberMuchBands()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            const int bandsCount = 5;

            try
            {
                await using ITile tile = new RasterTile(number, cs, bandsCount: bandsCount);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task FromNumberNotSquare()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            Size size = new Size(1, 256);

            try
            {
                await using ITile tile = new RasterTile(number, cs, size);
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        #endregion

        #region FromCoordinates

        [Test]
        public async Task FromCoordinatesDefaultArgs()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;

            await using ITile tile = new RasterTile(coordinate, coordinate, zoom);

            Assert.Pass();
        }

        [Test]
        public async Task FromCoordinatesOverrideDefaultArgs()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;
            Size size = new Size(64, 64);
            IEnumerable<byte> bytes = new byte[10];
            const TileExtension extension = TileExtension.Webp;
            const bool tmsCompatible = true;
            const int bandsCount = 3;
            const Interpolation interpolation = Interpolation.Cubic;

            await using ITile tile = new RasterTile(coordinate, coordinate, zoom, size, bytes,
                                                    extension, tmsCompatible, bandsCount, interpolation);

            Assert.Pass();
        }

        [Test]
        public async Task FromCoordinatesSmallBands()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;
            const int bandsCount = -1;

            try
            {
                await using ITile tile = new RasterTile(coordinate, coordinate, zoom, bandsCount: bandsCount);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task FromCoordinatesMuchBands()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;
            const int bandsCount = 5;

            try
            {
                await using ITile tile = new RasterTile(coordinate, coordinate, zoom, bandsCount: bandsCount);
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task FromCoordinatesNotSquare()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;
            Size size = new Size(1, 256);

            try
            {
                await using ITile tile = new RasterTile(coordinate, coordinate, zoom, size);
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task FromCoordinatesMinNotEqualsMax()
        {
            GeoCoordinate min = new GeodeticCoordinate(0.0, 0.0);
            GeoCoordinate max = new GeodeticCoordinate(180.0, 90.0);
            const int zoom = 10;

            try
            {
                await using ITile tile = new RasterTile(min, max, zoom);
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        #endregion

        #endregion

        #region Destructor/Dispose

        [Test]
        public void DisposeTest()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            ITile tile = new RasterTile(number, cs);
            tile.Dispose();

            Assert.True(tile.IsDisposed);
        }

        [Test]
        public async Task DisposeAsyncTest()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            ITile tile = new RasterTile(number, cs);
            await tile.DisposeAsync().ConfigureAwait(false);

            Assert.True(tile.IsDisposed);
        }

        #endregion

        #region Properties/Constants

        [Test]
        public void GetConstants()
        {
            Size size = Tile.DefaultSize;
            int bandsCount = RasterTile.DefaultBandsCount;

            Assert.Pass();
        }

        [Test]
        public async Task GetProperties()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            bool isDisposed = tile.IsDisposed;
            GeoCoordinate minC = tile.MinCoordinate;
            GeoCoordinate maxC = tile.MaxCoordinate;
            Number num = tile.Number;
            IEnumerable<byte> bytes = tile.Bytes;
            Size size = tile.Size;
            string path = tile.Path;
            TileExtension ext = tile.Extension;
            int minBytesC = tile.MinimalBytesCount;

            await using RasterTile rasterTile = (RasterTile)tile;
            int bandsCount = rasterTile.BandsCount;
            Interpolation interpolation = rasterTile.Interpolation;

            Assert.Pass();
        }

        #endregion

        #region Methods

        #region Validate

        [Test]
        public async Task ValidateWoPath()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            tile.Bytes = new byte[tile.MinimalBytesCount + 1];

            Assert.True(tile.Validate(false));
        }

        [Test]
        public async Task ValidateWPath()
        {
            // Only because File.Create can fail
            Constants.FileSystemEntries.OutputDirectoryInfo.Create();

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string tilePath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                           $"{timestamp}_validate.png");

            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            FileStream fs = File.Create(tilePath);
            await fs.DisposeAsync().ConfigureAwait(false);

            tile.Bytes = new byte[tile.MinimalBytesCount + 1];
            tile.Path = tilePath;

            try
            {
                Assert.True(tile.Validate(true));
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(tilePath);
            }
        }

        [Test]
        public void ValidateNullTile() => Assert.False(Tile.Validate(null, false));

        [Test]
        public async Task ValidateNullBytes()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            Assert.False(tile.Validate(false));
        }

        [Test]
        public async Task ValidateSmallBytes()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            tile.Bytes = new byte[tile.MinimalBytesCount - 1];

            Assert.False(tile.Validate(false));
        }

        [Test]
        public async Task ValidateNullPath()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            tile.Bytes = new byte[tile.MinimalBytesCount + 1];

            Assert.False(tile.Validate(true));
        }

        #endregion

        #region CalculatePosition

        [Test]
        public async Task CalculatePosition()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            int pos = tile.CalculatePosition();

            if (pos >= 0 && pos <= 3) Assert.Pass();

            Assert.Fail();
        }

        [Test]
        public void CalculatePositionNullNumber()
        {
            try
            {
                Tile.CalculatePosition(null, false);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        #endregion

        #region GetExtensionString

        [Test]
        public async Task GetExtensionString()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            tile.GetExtensionString();

            Tile.GetExtensionString(TileExtension.Png);
            Tile.GetExtensionString(TileExtension.Jpg);
            Tile.GetExtensionString(TileExtension.Webp);

            Assert.Pass();
        }

        #endregion

        #endregion
    }
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0219 // The variable is assigned but it's value is never used
#pragma warning restore CA1031 // Do not catch general exception types
