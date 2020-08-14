using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using GTiff2Tiles.Core;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
using NetVips;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.GeoTiffs
{
    [TestFixture]
    public sealed class GeoTiffTests
    {
        #region Constructors

        #region Create from file wo cs

        [Test]
        public void CreateRasterFromFileWoCsNormal()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const int memCache = 13000000;

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new Raster(inputPath, memCache);
            });
        }

        [Test]
        public void CreateRasterFromFileWoCsOtherCs()
        {
            string inputPath = FileSystemEntries.Input3395FilePath;

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new Raster(inputPath);
            });
        }

        [Test]
        public void CreateRasterFromFileWoCsSmallMemChache()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const int memCache = -1;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Raster raster = new Raster(inputPath, memCache);
            });
        }

        [Test]
        public void CreateRasterFromFileWoCsDontUseMemCache()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const int memCache = 1;

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new Raster(inputPath, memCache);
            });
        }

        #endregion

        #region Create from file with cs

        [Test]
        public void CreateRasterFromFileWCsNormal()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            const int memCache = 13000000;

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new Raster(inputPath, cs, memCache);
            });
        }

        [Test]
        public void CreateRasterFromFileWCsOtherCs()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Other;

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new Raster(inputPath, cs);
            });
        }

        [Test]
        public void CreateRasterFromFileWCsSmallMemChache()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            const int memCache = -1;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Raster raster = new Raster(inputPath, cs, memCache);
            });
        }

        [Test]
        public void CreateRasterFromFileWCsDontUseMemCache()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            const int memCache = 1;

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new Raster(inputPath, cs, memCache);
            });
        }

        #endregion

        #region Create from stream

        [Test]
        public void CreateRasterFromStreamNormal()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            using FileStream fs = File.OpenRead(inputPath);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new Raster(fs, cs);
            });
        }

        [Test]
        public void CreateRasterFromStreamNullStream()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.Throws<ArgumentNullException>(() =>
            {
                Raster raster = new Raster(null, cs);
            });
        }

        [Test]
        public void CreateRasterFromStreamOtherCs()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            using FileStream fs = File.OpenRead(inputPath);
            const CoordinateSystem cs = CoordinateSystem.Other;

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new Raster(fs, cs);
            });
        }

        #endregion

        #endregion

        #region Properties

        [Test]
        public void GetProperties()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.DoesNotThrowAsync(async () =>
            {
                await using IGeoTiff tiff = new Raster(inputPath, cs);
                bool d = tiff.IsDisposed;
                Size s = tiff.Size;
                GeoCoordinate mic = tiff.MinCoordinate;
                GeoCoordinate mac = tiff.MaxCoordinate;
                CoordinateSystem g = tiff.GeoCoordinateSystem;

                Raster raster = (Raster)tiff;
                Image data = raster.Data;
            });
        }

        #endregion

        #region Methods

        #region Destructor/Dispose

        [Test]
        public void DisposeTest()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);
            raster.Dispose();

            Assert.True(raster.IsDisposed);
        }

        [Test]
        public async Task DisposeAsyncTest()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);
            await raster.DisposeAsync();

            Assert.True(raster.IsDisposed);
        }

        #endregion

        #region CreateTile

        [Test]
        public void CreateTileImageNormal()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            using Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, cs);

            Assert.DoesNotThrow(() =>
            {
                Image image = raster.CreateTileImage(raster.Data, tile);
            });
        }

        [Test]
        public void CreateTileImageNullImage()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            using Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            using RasterTile tile = new RasterTile(number, cs);

            Assert.Throws<ArgumentNullException>(() =>
            {
                using Image image = raster.CreateTileImage(null, tile);
            });
        }

        [Test]
        public void CreateTileImageNullTile()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            using Raster raster = new Raster(inputPath, cs);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Image image = raster.CreateTileImage(raster.Data, null);
            });
        }

        [Test]
        public void CreateTileImageDifferentInterpolations()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile t1 = new RasterTile(number, cs, interpolation: Interpolation.Nearest);
            RasterTile t2 = new RasterTile(number, cs, interpolation: Interpolation.Linear);
            RasterTile t3 = new RasterTile(number, cs, interpolation: Interpolation.Cubic);
            RasterTile t4 = new RasterTile(number, cs, interpolation: Interpolation.Mitchell);
            RasterTile t5 = new RasterTile(number, cs, interpolation: Interpolation.Lanczos2);
            RasterTile t6 = new RasterTile(number, cs, interpolation: Interpolation.Lanczos3);

            Assert.DoesNotThrow(() => raster.CreateTileImage(raster.Data, t1));
            Assert.DoesNotThrow(() => raster.CreateTileImage(raster.Data, t2));
            Assert.DoesNotThrow(() => raster.CreateTileImage(raster.Data, t3));
            Assert.DoesNotThrow(() => raster.CreateTileImage(raster.Data, t4));
            Assert.DoesNotThrow(() => raster.CreateTileImage(raster.Data, t5));
            Assert.DoesNotThrow(() => raster.CreateTileImage(raster.Data, t6));
        }

        #endregion

        #region WriteTile

        #region WriteTileToFile

        [Test]
        public void WriteTileToFileNormal()
        {
            FileSystemEntries.OutputDirectoryInfo.Create();

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}_tile.png");

            Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, cs) { Path = outPath };

            Assert.DoesNotThrowAsync(async () => await raster.WriteTileToFileAsync(raster.Data, tile));

            File.Delete(outPath);
        }

        [Test]
        public void WriteTileToFileNullTile()
        {
            FileSystemEntries.OutputDirectoryInfo.Create();

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await raster.WriteTileToFileAsync(raster.Data, null));
        }

        #endregion

        [Test]
        public void WriteTileToEnumberable()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, cs);

            IEnumerable<byte> tileBytes = null;

            Assert.DoesNotThrow(() => tileBytes = raster.WriteTileToEnumerable(raster.Data, tile));
            Assert.True(tileBytes?.Any() == true);
        }

        #region WriteToChannel

        [Test]
        public void WriteToChannelNormal()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, cs);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.True(raster.WriteTileToChannel(raster.Data, tile, channel.Writer));
        }

        [Test]
        public void WriteToChannelNullTile()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.Throws<ArgumentNullException>(() => raster.WriteTileToChannel(raster.Data, null, channel.Writer));
        }

        [Test]
        public void WriteToChannelNullWriter()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, cs);

            Assert.Throws<ArgumentNullException>(() => raster.WriteTileToChannel(raster.Data, tile, null));
        }

        [Test]
        public void WriteToChannelAsyncNormal()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, cs);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTileToChannelAsync(raster.Data, tile, channel.Writer));
        }

        [Test]
        public void WriteToChannelAsyncNullTile()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await raster.WriteTileToChannelAsync(raster.Data, null, channel.Writer));
        }

        [Test]
        public void WriteToChannelAsyncNullWriter()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, cs);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await raster.WriteTileToChannelAsync(raster.Data, tile, null));
        }

        #endregion

        #endregion

        #region WriteTiles

        #region WriteTilesToDirectory

        [Test]
        public void WriteTilesToDirectoryNormal()
        {
            // With default args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, timestamp);

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));

            Directory.Delete(outPath, true);
        }

        [Test]
        public void WriteTilesToDirectoryOverrideArgs()
        {
            // With overriden args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, timestamp);

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
              true, new Size(64, 64), TileExtension.Jpg, Interpolation.Cubic, 3, 999, 10,
              new Progress<double>(), true));

            Directory.Delete(outPath, true);
        }

        [Test]
        public void WriteTilesToDirectoryLowTileCacheCount()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, timestamp);

            Raster raster = new Raster(inputPath, cs);

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await raster.WriteTilesToDirectoryAsync(
                outPath, 0, 11, tileCacheCount: 0));

            Directory.Delete(outPath, true);
        }

        #endregion

        #region Data-related WriteToDirectory

        [Test]
        public void Proj4326Ntms256PngLanczos3Bands4Directory()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_4326_ntms_256_png_lanczos3_4");

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public void Proj4326Tms256PngLanczos3Bands4Directory()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_4326_tms_256_png_lanczos3_4");

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   true));
        }

        [Test]
        public void Proj4326Ntms128PngLanczos3Bands4Directory()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_4326_ntms_128_png_lanczos3_4");

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileSize: new Size(128, 128)));
        }

        [Test]
        public void Proj4326Tms256WebpLanczos3Bands4Directory()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_4326_tms_256_webp_lanczos3_4");

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileExtension: TileExtension.Webp));
        }

        [Test]
        public void Proj4326Ntms256JpgLanczos3Bands3Directory()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_4326_ntms_256_jpg_lanczos3_3");

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileExtension: TileExtension.Jpg, bandsCount: 3));
        }

        [Test]
        public void Proj4326Ntms256PngCubicBands4Directory()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_4326_ntms_256_png_cubic_4");

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   interpolation: Interpolation.Cubic));
        }

        [Test]
        public void Proj3857Ntms256PngLanczos3Bands4Directory()
        {
            string inputPath = FileSystemEntries.Input3785FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_3857_ntms_256_png_lanczos3_4");

            Raster raster = new Raster(inputPath, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj4326To3857Directory()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_4326_to_3857_tiles");

            string tmp = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                                                                     + $"_4326_to_3857{GdalWorker.TempFileName}");
            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputPath, tmp, cs);

            Raster raster = new Raster(tmp, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj3857To4326Directory()
        {
            string inputPath = FileSystemEntries.Input3785FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_3857_to_4326_tiles");

            string tmp = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                                                                     + $"_3857_to_4326{GdalWorker.TempFileName}");
            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputPath, tmp, cs);

            Raster raster = new Raster(tmp, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj3395To4326Directory()
        {
            string inputPath = FileSystemEntries.Input3395FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                          + "_3395_to_4326_tiles");

            string tmp = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{timestamp}"
                                        + $"_3395_to_4326{GdalWorker.TempFileName}");

            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputPath, tmp, cs);

            Raster raster = new Raster(tmp, cs);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        #endregion

        #region WriteTilesToChannel

        [Test]
        public void WriteTilesToChannelNormal()
        {
            // With default args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToChannelAsync(channel.Writer, 0, 11));
        }

        [Test]
        public void WriteTilesToChannelOverrideArgs()
        {
            // With overriden args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToChannelAsync(channel.Writer, 0, 11,
                   true, new Size(128, 128), Interpolation.Cubic, 3, 999, 10, new Progress<double>(), true));
        }

        [Test]
        public void WriteTilesToChannelLowTileCacheCount()
        {
            // With default args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await
                    raster.WriteTilesToChannelAsync(channel.Writer, 0, 11, tileCacheCount: 0));
        }

        #endregion

        #region WriteTilesToEnumerable

        [Test]
        public void WriteTilesToEnumerableNormal()
        {
            // With default args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            IEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToEnumerable(0, 11));

            Assert.True(tiles?.Any());
        }

        [Test]
        public void WriteTilesToEnumerableOverrideArgs()
        {
            // With overriden args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            IEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToEnumerable(0, 11,
                   true, new Size(128, 128), Interpolation.Mitchell, 3, 999, new Progress<double>(), true));

            Assert.True(tiles?.Any());
        }

        [Test]
        public void WriteTilesToEnumerableLowTileCache()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                ITile[] tiles = raster.WriteTilesToEnumerable(0, 11, tileCacheCount: 0).ToArray();
            });
        }

        #endregion

        #region WriteTilesToAsyncEnumerable

        [Test]
        public void WriteTilesToAsyncEnumerableNormal()
        {
            // With default args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            IAsyncEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToAsyncEnumerable(0, 11));

            Assert.DoesNotThrowAsync(async () =>
            {
                await foreach (ITile tile in tiles) tile.Validate(false);
            });
        }

        [Test]
        public void WriteTilesToAsyncEnumerableOverrideArgs()
        {
            // With override args

            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Raster raster = new Raster(inputPath, cs);

            IAsyncEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToAsyncEnumerable(0, 11,
                   true, new Size(128, 128), Interpolation.Nearest, 3, 999, 10, new Progress<double>(), true));

            Assert.DoesNotThrowAsync(async () =>
            {
                await foreach (ITile tile in tiles) tile.Validate(false);
            });
        }

        #endregion

        #endregion

        #region GetBorders

        [Test]
        public void GetBordersNormal()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            FileStream fs = File.OpenRead(inputPath);

            GeodeticCoordinate expectedMin = new GeodeticCoordinate(13.367990255355835, 52.501827478408813);
            GeodeticCoordinate expectedMax = new GeodeticCoordinate(13.438467979431152, 52.534797191619873);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(fs, cs));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersMercator()
        {
            string inputPath = FileSystemEntries.Input3785FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            FileStream fs = File.OpenRead(inputPath);

            MercatorCoordinate expectedMin = new MercatorCoordinate(15556898.732197443, 4247491.006264816);
            MercatorCoordinate expectedMax = new MercatorCoordinate(15567583.19555743, 4257812.3937404491);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(fs, cs));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersNullStream()
        {
            {
                const CoordinateSystem cs = CoordinateSystem.Epsg4326;

                Assert.Throws<ArgumentNullException>(() => Raster.GetBorders(null, cs));
            }
        }

        [Test]
        public void GetBordersClosedStream()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            FileStream fs = File.OpenRead(inputPath);
            fs.Dispose();

            Assert.Throws<ArgumentException>(() => Raster.GetBorders(fs, cs));
        }

        [Test]
        public void GetBordersOtherCs()
        {
            string inputPath = FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Other;
            FileStream fs = File.OpenRead(inputPath);

            Assert.Throws<NotSupportedException>(() => Raster.GetBorders(fs, cs));
        }

        #endregion

        #endregion
    }
}
