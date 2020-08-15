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
using GTiff2Tiles.Core.Helpers;
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
        #region SetUp and consts

        private string _timestamp;

        private string _outPath;

        private readonly string _in4326 = FileSystemEntries.Input4326FilePath;

        private readonly string _in3785 = FileSystemEntries.Input3785FilePath;

        private readonly string _in3395 = FileSystemEntries.Input3395FilePath;

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

        private const CoordinateSystem CsOther = CoordinateSystem.Other;

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

        #region Create from file wo cs

        [Test]
        public void CreateRasterFromFileWoCsNormal() => Assert.DoesNotThrow(() =>
        {
            Raster raster = new Raster(_in4326, 13000000);
        });

        [Test]
        public void CreateRasterFromFileWoCsOtherCs() => Assert.Throws<NotSupportedException>(() =>
        {
            Raster raster = new Raster(_in3395);
        });

        [Test]
        public void CreateRasterFromFileWoCsSmallMemChache() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Raster raster = new Raster(_in4326, -1);
        });

        [Test]
        public void CreateRasterFromFileWoCsDontUseMemCache() => Assert.DoesNotThrow(() =>
        {
            Raster raster = new Raster(_in4326, 1);
        });

        #endregion

        #region Create from file with cs

        [Test]
        public void CreateRasterFromFileWCsNormal() => Assert.DoesNotThrow(() =>
        {
            Raster raster = new Raster(_in4326, Cs4326, 13000000);
        });

        [Test]
        public void CreateRasterFromFileWCsOtherCs() => Assert.Throws<NotSupportedException>(() =>
        {
            Raster raster = new Raster(_in4326, CsOther);
        });

        [Test]
        public void CreateRasterFromFileWCsSmallMemChache() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Raster raster = new Raster(_in4326, Cs4326, -1);
        });

        [Test]
        public void CreateRasterFromFileWCsDontUseMemCache() => Assert.DoesNotThrow(() =>
        {
            Raster raster = new Raster(_in4326, Cs4326, 1);
        });

        #endregion

        #region Create from stream

        [Test]
        public void CreateRasterFromStreamNormal()
        {
            using FileStream fs = File.OpenRead(_in4326);

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new Raster(fs, Cs4326);
            });
        }

        [Test]
        public void CreateRasterFromStreamNullStream() => Assert.Throws<ArgumentNullException>(() =>
        {
            Raster raster = new Raster(null, Cs4326);
        });

        [Test]
        public void CreateRasterFromStreamOtherCs()
        {
            using FileStream fs = File.OpenRead(_in4326);

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new Raster(fs, CsOther);
            });
        }

        #endregion

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrowAsync(async () =>
        {
            await using IGeoTiff tiff = new Raster(_in4326, Cs4326);
            bool d = tiff.IsDisposed;
            Size s = tiff.Size;
            GeoCoordinate mic = tiff.MinCoordinate;
            GeoCoordinate mac = tiff.MaxCoordinate;
            CoordinateSystem g = tiff.GeoCoordinateSystem;

            await using Raster raster = (Raster)tiff;
            using Image data = raster.Data;
        });

        #endregion

        #region Methods

        #region Destructor/Dispose

        [Test]
        public void DisposeTest()
        {
            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrow(() => raster.Dispose());

            Assert.True(raster.IsDisposed);
        }

        [Test]
        public void DisposeAsyncTest()
        {
            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.DisposeAsync());

            Assert.True(raster.IsDisposed);
        }

        #endregion

        #region CreateTile

        [Test]
        public void CreateTileImageNormal()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            using RasterTile tile = new RasterTile(number, raster.GeoCoordinateSystem);

            Assert.DoesNotThrow(() =>
            {
                using Image image = raster.CreateTileImage(raster.Data, tile);
            });
        }

        [Test]
        public void CreateTileImageNullImage()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            using RasterTile tile = new RasterTile(number, raster.GeoCoordinateSystem);

            Assert.Throws<ArgumentNullException>(() =>
            {
                using Image image = raster.CreateTileImage(null, tile);
            });
        }

        [Test]
        public void CreateTileImageNullTile()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Assert.Throws<ArgumentNullException>(() =>
            {
                using Image image = raster.CreateTileImage(raster.Data, null);
            });
        }

        [Test]
        public void CreateTileImageDifferentInterpolations()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            using RasterTile t1 = new RasterTile(number, raster.GeoCoordinateSystem, interpolation: Interpolation.Nearest);
            using RasterTile t2 = new RasterTile(number, raster.GeoCoordinateSystem, interpolation: Interpolation.Linear);
            using RasterTile t3 = new RasterTile(number, raster.GeoCoordinateSystem, interpolation: Interpolation.Cubic);
            using RasterTile t4 = new RasterTile(number, raster.GeoCoordinateSystem, interpolation: Interpolation.Mitchell);
            using RasterTile t5 = new RasterTile(number, raster.GeoCoordinateSystem, interpolation: Interpolation.Lanczos2);
            using RasterTile t6 = new RasterTile(number, raster.GeoCoordinateSystem, interpolation: Interpolation.Lanczos3);

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
            string outPath = Path.Combine(_outPath, $"{_timestamp}_tile.png");

            Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, Cs4326) { Path = outPath };

            Assert.DoesNotThrowAsync(async () => await raster.WriteTileToFileAsync(raster.Data, tile));

            File.Delete(outPath);
        }

        [Test]
        public void WriteTileToFileNullTile()
        {
            Raster raster = new Raster(_in4326, Cs4326);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await raster.WriteTileToFileAsync(raster.Data, null));
        }

        #endregion

        [Test]
        public void WriteTileToEnumberable()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            using RasterTile tile = new RasterTile(number, Cs4326);

            IEnumerable<byte> tileBytes = null;

            Assert.DoesNotThrow(() => tileBytes = raster.WriteTileToEnumerable(raster.Data, tile));
            Assert.True(tileBytes?.Any() == true);
        }

        #region WriteToChannel

        [Test]
        public void WriteToChannelNormal()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            using RasterTile tile = new RasterTile(number, Cs4326);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.True(raster.WriteTileToChannel(raster.Data, tile, channel.Writer));
        }

        [Test]
        public void WriteToChannelNullTile()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.Throws<ArgumentNullException>(() => raster.WriteTileToChannel(raster.Data, null, channel.Writer));
        }

        [Test]
        public void WriteToChannelNullWriter()
        {
            using Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            using RasterTile tile = new RasterTile(number, Cs4326);

            Assert.Throws<ArgumentNullException>(() => raster.WriteTileToChannel(raster.Data, tile, null));
        }

        [Test]
        public void WriteToChannelAsyncNormal()
        {
            Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, raster.GeoCoordinateSystem);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTileToChannelAsync(raster.Data, tile, channel.Writer));
        }

        [Test]
        public void WriteToChannelAsyncNullTile()
        {
            Raster raster = new Raster(_in4326, Cs4326);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await raster.WriteTileToChannelAsync(raster.Data, null, channel.Writer));
        }

        [Test]
        public void WriteToChannelAsyncNullWriter()
        {
            Raster raster = new Raster(_in4326, Cs4326);

            Number number = new Number(1100, 213, 10);
            RasterTile tile = new RasterTile(number, raster.GeoCoordinateSystem);

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

            string outPath = Path.Combine(_outPath, _timestamp);

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));

            Directory.Delete(outPath, true);
        }

        [Test]
        public void WriteTilesToDirectoryOverrideArgs()
        {
            // With overriden args

            string outPath = Path.Combine(_outPath, _timestamp);

            static void Reporter(string s) { }

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
              true, new Size(64, 64), TileExtension.Jpg, Interpolation.Cubic, 3, 999, 10,
              new Progress<double>(), Reporter));

            Directory.Delete(outPath, true);
        }

        [Test]
        public void WriteTilesToDirectoryLowTileCacheCount()
        {
            string outPath = Path.Combine(_outPath, _timestamp);

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await raster.WriteTilesToDirectoryAsync(
                outPath, 0, 11, tileCacheCount: 0));

            Directory.Delete(outPath, true);
        }

        #endregion

        #region Data-related WriteToDirectory

        [Test]
        public void Proj4326Ntms256PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_256_png_lanczos3_4");

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public void Proj4326Tms256PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_tms_256_png_lanczos3_4");

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   true));
        }

        [Test]
        public void Proj4326Ntms128PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_128_png_lanczos3_4");

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileSize: new Size(128, 128)));
        }

        [Test]
        public void Proj4326Tms256WebpLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_tms_256_webp_lanczos3_4");

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileExtension: TileExtension.Webp));
        }

        [Test]
        public void Proj4326Ntms256JpgLanczos3Bands3Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_256_jpg_lanczos3_3");

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileExtension: TileExtension.Jpg, bandsCount: 3));
        }

        [Test]
        public void Proj4326Ntms256PngCubicBands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_256_png_cubic_4");

            Raster raster = new Raster(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   interpolation: Interpolation.Cubic));
        }

        [Test]
        public void Proj3857Ntms256PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_3857_ntms_256_png_lanczos3_4");

            Raster raster = new Raster(_in3785, Cs3857);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj4326To3857Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_to_3857_tiles");
            string tmp = Path.Combine(_outPath, $"{_timestamp}_4326_to_3857{GdalWorker.TempFileName}");

            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in4326, tmp, Cs3857);

            Raster raster = new Raster(tmp, Cs3857);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj3857To4326Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_3857_to_4326_tiles");
            string tmp = Path.Combine(_outPath, $"{_timestamp}_3857_to_4326{GdalWorker.TempFileName}");

            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in3785, tmp, Cs4326);

            Raster raster = new Raster(tmp, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj3395To4326Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_3395_to_4326_tiles");
            string tmp = Path.Combine(_outPath, $"{_timestamp}_3395_to_4326{GdalWorker.TempFileName}");

            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in3395, tmp, Cs4326);

            Raster raster = new Raster(tmp, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        #endregion

        #region WriteTilesToChannel

        [Test]
        public void WriteTilesToChannelNormal()
        {
            // With default args

            Raster raster = new Raster(_in4326, Cs4326);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToChannelAsync(channel.Writer, 0, 11));
        }

        [Test]
        public void WriteTilesToChannelOverrideArgs()
        {
            // With overriden args

            static void Reporter(string s) { }

            Raster raster = new Raster(_in4326, Cs4326);

            Channel<ITile> channel = Channel.CreateUnbounded<ITile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToChannelAsync(channel.Writer, 0, 11,
                   true, new Size(128, 128), Interpolation.Cubic, 3, 999, 10, new Progress<double>(), Reporter));
        }

        [Test]
        public void WriteTilesToChannelLowTileCacheCount()
        {
            // With default args

            Raster raster = new Raster(_in4326, Cs4326);

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

            Raster raster = new Raster(_in4326, Cs4326);

            IEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToEnumerable(0, 11));

            Assert.True(tiles?.Any());
        }

        [Test]
        public void WriteTilesToEnumerableOverrideArgs()
        {
            // With overriden args

            static void Reporter(string s) { }

            Raster raster = new Raster(_in4326, Cs4326);

            IEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToEnumerable(0, 11,
                   true, new Size(128, 128), Interpolation.Mitchell, 3, 999, new Progress<double>(), Reporter));

            Assert.True(tiles?.Any());
        }

        [Test]
        public void WriteTilesToEnumerableLowTileCache()
        {
            Raster raster = new Raster(_in4326, Cs4326);

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

            Raster raster = new Raster(_in4326, Cs4326);

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

            static void Reporter(string s) { }

            Raster raster = new Raster(_in4326, Cs4326);

            IAsyncEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToAsyncEnumerable(0, 11,
                   true, new Size(128, 128), Interpolation.Nearest, 3, 999, 10, new Progress<double>(), Reporter));

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
            FileStream fs = File.OpenRead(_in4326);

            GeodeticCoordinate expectedMin = new GeodeticCoordinate(13.367990255355835, 52.501827478408813);
            GeodeticCoordinate expectedMax = new GeodeticCoordinate(13.438467979431152, 52.534797191619873);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(fs, Cs4326));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersMercator()
        {
            FileStream fs = File.OpenRead(_in3785);

            MercatorCoordinate expectedMin = new MercatorCoordinate(15556898.732197443, 4247491.006264816);
            MercatorCoordinate expectedMax = new MercatorCoordinate(15567583.19555743, 4257812.3937404491);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(fs, Cs3857));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersNullStream() => Assert.Throws<ArgumentNullException>(() => Raster.GetBorders(null, Cs4326));

        [Test]
        public void GetBordersClosedStream()
        {
            FileStream fs = File.OpenRead(_in4326);
            fs.Dispose();

            Assert.Throws<ArgumentException>(() => Raster.GetBorders(fs, Cs4326));
        }

        [Test]
        public void GetBordersOtherCs()
        {
            FileStream fs = File.OpenRead(_in4326);

            Assert.Throws<NotSupportedException>(() => Raster.GetBorders(fs, CsOther));
        }

        #endregion

        #endregion
    }
}
