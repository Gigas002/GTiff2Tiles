#pragma warning disable CS0219 // The variable is assigned but it's value is never used

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Tiles
{
    [TestFixture]
    public sealed class TileTests
    {
        #region SetUp and consts

        private string _timestamp;

        private string _outPath;

        private readonly string _in4326 = FileSystemEntries.Input4326FilePath;

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        [SetUp]
        public void SetUp()
        {
            _timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                               CultureInfo.InvariantCulture);

            _outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath);

            FileSystemEntries.OutputDirectoryInfo.Create();
            NetVipsHelper.DisableLog();
        }

        #endregion

        #region Constructors

        #region FromNumber

        [Test]
        public void FromNumberDefaultArgs() => Assert.DoesNotThrowAsync(async () =>
        {
            await using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);
        });

        [Test]
        public void FromNumberOverrideDefaultArgs()
        {
            Size size = new(128, 128);
            IEnumerable<byte> bytes = new byte[10];
            const TileExtension extension = TileExtension.Webp;
            const bool tmsCompatible = true;
            const int bandsCount = 3;
            const Interpolation interpolation = Interpolation.Cubic;

            Assert.DoesNotThrowAsync(async () =>
            {
                await using ITile tile = new RasterTile(Locations.TokyoGeodeticTmsNumber, Cs4326, size, tmsCompatible);
            });
        }

        [Test]
        public void FromNumberNotSquare() => Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326, new Size(1, 256));
        });

        #endregion

        #region FromCoordinates

        [Test]
        public void FromCoordinatesDefaultArgs() => Assert.DoesNotThrowAsync(async () =>
        {
            await using ITile tile = new RasterTile(Locations.TokyoGeodeticCoordinate,
                                                    Locations.TokyoGeodeticCoordinate, 10);
        });

        [Test]
        public void FromCoordinatesOverrideDefaultArgs()
        {
            Size size = new(64, 64);
            IEnumerable<byte> bytes = new byte[10];
            const TileExtension extension = TileExtension.Webp;
            const bool tmsCompatible = true;
            const int bandsCount = 3;
            const Interpolation interpolation = Interpolation.Cubic;

            Assert.DoesNotThrowAsync(async () =>
            {
                await using ITile tile = new RasterTile(Locations.TokyoGeodeticCoordinate,
                                                        Locations.TokyoGeodeticCoordinate,
                                                        10, size, tmsCompatible);
            });
        }

        [Test]
        public void FromCoordinatesNotSquare() => Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await using ITile tile = new RasterTile(Locations.TokyoGeodeticCoordinate,
                                                    Locations.TokyoGeodeticCoordinate,
                                                    10, new Size(1, 256));
        });

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
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            Assert.DoesNotThrow(() => tile.Dispose());
            Assert.True(tile.IsDisposed);
        }

        [Test]
        public void DisposeAsyncTest()
        {
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            Assert.DoesNotThrowAsync(async () => await tile.DisposeAsync().ConfigureAwait(false));
            Assert.True(tile.IsDisposed);
        }

        #endregion

        #region Properties/Constants

        [Test]
        public void GetConstants() => Assert.DoesNotThrow(() =>
        {
            Size size = Tile.DefaultSize;
            int bandsCount = RasterTile.DefaultBandsCount;
        });

        [Test]
        public void GetProperties()
        {
            ITile tile = null;
            Assert.DoesNotThrow(() =>
            {
                tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

                bool isDisposed = tile.IsDisposed;
                CoordinateSystem coordinateSystem = tile.CoordinateSystem;
                GeoCoordinate minC = tile.MinCoordinate;
                GeoCoordinate maxC = tile.MaxCoordinate;
                Number num = tile.Number;
                IEnumerable<byte> bytes = tile.Bytes;
                Size size = tile.Size;
                string path = tile.Path;
                TileExtension ext = tile.Extension;
                int minBytesC = tile.MinimalBytesCount;
            });

            Assert.DoesNotThrowAsync(async () =>
            {
                await using RasterTile rasterTile = (RasterTile)tile;
                int bandsCount = rasterTile.BandsCount;
                Interpolation interpolation = rasterTile.Interpolation;
            });
        }

        [Test]
        public void SetProperties()
        {
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            Assert.DoesNotThrow(() =>
            {
                tile.Bytes = null;
                tile.Path = string.Empty;
                tile.MinimalBytesCount = int.MinValue;
                tile.Extension = TileExtension.Webp;
                tile.Size = Tile.DefaultSize;
                tile.TmsCompatible = true;
            });

            using RasterTile rasterTile = (RasterTile)tile;
            Assert.DoesNotThrow(() =>
            {
                rasterTile.BandsCount = 3;
                rasterTile.Interpolation = Interpolation.Cubic;
            });

            Assert.Throws<ArgumentOutOfRangeException>(() => rasterTile.BandsCount = -1);
            Assert.Throws<ArgumentOutOfRangeException>(() => rasterTile.BandsCount = 5);
        }

        #endregion

        #region Methods

        #region Validate

        [Test]
        public void ValidateWoPath()
        {
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            tile.Bytes = new byte[tile.MinimalBytesCount + 1];

            Assert.True(tile.Validate(false));
        }

        [Test]
        public void ValidateWPath()
        {
            string tilePath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{_timestamp}_validate.png");

            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            FileStream fs = File.Create(tilePath);
            fs.Dispose();

            tile.Bytes = new byte[tile.MinimalBytesCount + 1];
            tile.Path = tilePath;

            Assert.True(tile.Validate(true));

            File.Delete(tilePath);
        }

        [Test]
        public void ValidateNullTile() => Assert.False(Tile.Validate(null, false));

        [Test]
        public void ValidateNullBytes()
        {
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            Assert.False(tile.Validate(false));
        }

        [Test]
        public void ValidateSmallBytes()
        {
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            tile.Bytes = new byte[tile.MinimalBytesCount - 1];

            Assert.False(tile.Validate(false));
        }

        [Test]
        public void ValidateNullPath()
        {
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            tile.Bytes = new byte[tile.MinimalBytesCount + 1];

            Assert.False(tile.Validate(true));
        }

        #endregion

        #region CalculatePosition

        [Test]
        public void CalculatePosition()
        {
            Number number = Locations.TokyoGeodeticNtmsNumber;

            using ITile tile0 = new RasterTile(number, Cs4326);
            int pos3 = tile0.CalculatePosition();

            using ITile tile1 = new RasterTile(new Number(number.X + 1, number.Y, number.Z), Cs4326);
            int pos2 = tile1.CalculatePosition();

            using ITile tile2 = new RasterTile(new Number(number.X, number.Y + 1, number.Z), Cs4326);
            int pos1 = tile2.CalculatePosition();

            using ITile tile3 = new RasterTile(new Number(number.X + 1, number.Y + 1, number.Z), Cs4326);
            int pos0 = tile3.CalculatePosition();

            Assert.True(pos0 == 0 && pos1 == 1 && pos2 == 2 && pos3 == 3);
        }

        [Test]
        public void CalculatePositionNullNumber() => Assert.Throws<ArgumentNullException>(() =>
              Tile.CalculatePosition(null, false));

        #endregion

        #region GetExtensionString

        [Test]
        public void GetExtensionString()
        {
            using ITile tile = new RasterTile(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            tile.GetExtensionString();

            Assert.True(Tile.GetExtensionString(TileExtension.Png) == Core.Constants.FileExtensions.Png);
            Assert.True(Tile.GetExtensionString(TileExtension.Jpg) == Core.Constants.FileExtensions.Jpg);
            Assert.True(Tile.GetExtensionString(TileExtension.Webp) == Core.Constants.FileExtensions.Webp);
        }

        #endregion

        #region WriteToFile

        [Test]
        public void WriteTileToFileNormal()
        {
            // We need to create some tiles first:
            string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            Size tileSize = Tile.DefaultSize;
            RasterTile[] baseTiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToArray();

            CheckHelper.CheckDirectory(path);
            string filePath = Path.Combine(path, "tile.png");
            Assert.DoesNotThrow(() => baseTiles[0].WriteToFile(filePath));

            Directory.Delete(path, true);
        }

        [Test]
        public void WriteTileToFileNullTile() => Assert.Throws<ArgumentNullException>(() => Tile.WriteToFile(null));

        [Test]
        public void WriteTileToFileNullTileBytes()
        {
            using RasterTile tile = new(new Number(1, 1, 1), Cs4326);
            Assert.Throws<ArgumentNullException>(() => Tile.WriteToFile(tile));
        }

        [Test]
        public void WriteTileToFilePathProp()
        {
            // We need to create some tiles first:
            string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            Size tileSize = Tile.DefaultSize;
            RasterTile[] baseTiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToArray();

            RasterTile tile = baseTiles[0];
            CheckHelper.CheckDirectory(path);
            tile.Path = Path.Combine(path, "tile.png");
            Assert.DoesNotThrow(() => tile.WriteToFile());

            Directory.Delete(path, true);
        }

        #endregion

        #endregion
    }
}

#pragma warning restore CS0219 // The variable is assigned but it's value is never used
