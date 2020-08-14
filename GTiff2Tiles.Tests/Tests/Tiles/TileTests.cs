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

namespace GTiff2Tiles.Tests.Tests.Tiles
{
    [TestFixture]
    public sealed class TileTests
    {
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
        public void FromNumberSmallBands()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            const int bandsCount = -1;

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await using ITile tile = new RasterTile(number, cs, bandsCount: bandsCount);
            });
        }

        [Test]
        public void FromNumberMuchBands()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            const int bandsCount = 5;

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await using ITile tile = new RasterTile(number, cs, bandsCount: bandsCount);
            });
        }

        [Test]
        public void FromNumberNotSquare()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            Size size = new Size(1, 256);

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await using ITile tile = new RasterTile(number, cs, size);
            });
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
        public void FromCoordinatesSmallBands()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;
            const int bandsCount = -1;

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await using ITile tile = new RasterTile(coordinate, coordinate, zoom, bandsCount: bandsCount);
            });
        }

        [Test]
        public void FromCoordinatesMuchBands()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;
            const int bandsCount = 5;

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
            {
                await using ITile tile = new RasterTile(coordinate, coordinate, zoom, bandsCount: bandsCount);
            });
        }

        [Test]
        public void FromCoordinatesNotSquare()
        {
            GeoCoordinate coordinate = new GeodeticCoordinate(0.0, 0.0);
            const int zoom = 10;
            Size size = new Size(1, 256);

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await using ITile tile = new RasterTile(coordinate, coordinate, zoom, size);
            });
        }

        [Test]
        public void FromCoordinatesMinNotEqualsMax()
        {
            GeoCoordinate min = new GeodeticCoordinate(0.0, 0.0);
            GeoCoordinate max = new GeodeticCoordinate(180.0, 90.0);
            const int zoom = 10;

            Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await using ITile tile = new RasterTile(min, max, zoom);
            });
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

        [Test]
        public async Task SetProperties()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs)
            {
                Bytes = null, Path = string.Empty, MinimalBytesCount = int.MinValue
            };

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
            Number number = new Number(1230, 120, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            ITile tile = new RasterTile(number, cs);

            int pos0 = tile.CalculatePosition();

            tile = new RasterTile(new Number(number.X + 1, number.Y, number.Z), cs);
            int pos1 = tile.CalculatePosition();

            tile = new RasterTile(new Number(number.X, number.Y + 1, number.Z), cs);
            int pos2 = tile.CalculatePosition();

            tile = new RasterTile(new Number(number.X + 1, number.Y + 1, number.Z), cs);
            int pos3 = tile.CalculatePosition();

            Assert.True(pos0 == 0 && pos1 == 1 && pos2 == 2 && pos3 == 3);

            await tile.DisposeAsync();
        }

        [Test]
        public void CalculatePositionNullNumber() => Assert.Throws<ArgumentNullException>(() =>
              Tile.CalculatePosition(null, false));

        #endregion

        #region GetExtensionString

        [Test]
        public async Task GetExtensionString()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            await using ITile tile = new RasterTile(number, cs);

            tile.GetExtensionString();

            Assert.True(Tile.GetExtensionString(TileExtension.Png) == Core.Constants.FileExtensions.Png);
            Assert.True(Tile.GetExtensionString(TileExtension.Jpg) == Core.Constants.FileExtensions.Jpg);
            Assert.True(Tile.GetExtensionString(TileExtension.Webp) == Core.Constants.FileExtensions.Webp);
        }

        #endregion

        #endregion
    }
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0219 // The variable is assigned but it's value is never used
#pragma warning restore CA1031 // Do not catch general exception types
