#pragma warning disable CA1508 // Avoid dead conditional code

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
            Raster raster = new(_in4326, 13000000);
        });

        [Test]
        public void CreateRasterFromFileWoCsOtherCs() => Assert.Throws<NotSupportedException>(() =>
        {
            Raster raster = new(_in3395);
        });

        [Test]
        public void CreateRasterFromFileWoCsSmallMemChache() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Raster raster = new(_in4326, -1);
        });

        [Test]
        public void CreateRasterFromFileWoCsDontUseMemCache() => Assert.DoesNotThrow(() =>
        {
            Raster raster = new(_in4326, 1);
        });

        #endregion

        #region Create from file with cs

        [Test]
        public void CreateRasterFromFileWCsNormal() => Assert.DoesNotThrow(() =>
        {
            Raster raster = new(_in4326, Cs4326, 13000000);
        });

        [Test]
        public void CreateRasterFromFileWCsOtherCs() => Assert.Throws<NotSupportedException>(() =>
        {
            Raster raster = new(_in4326, CsOther);
        });

        [Test]
        public void CreateRasterFromFileWCsSmallMemChache() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Raster raster = new(_in4326, Cs4326, -1);
        });

        [Test]
        public void CreateRasterFromFileWCsDontUseMemCache() => Assert.DoesNotThrow(() =>
        {
            Raster raster = new(_in4326, Cs4326, 1);
        });

        #endregion

        #region Create from stream

        [Test]
        public void CreateRasterFromStreamNormal()
        {
            using FileStream fs = File.OpenRead(_in4326);

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new(fs, Cs4326);
            });
        }

        [Test]
        public void CreateRasterFromStreamNullStream() => Assert.Throws<ArgumentNullException>(() =>
        {
            Raster raster = new(null, Cs4326);
        });

        [Test]
        public void CreateRasterFromStreamOtherCs()
        {
            using FileStream fs = File.OpenRead(_in4326);

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new(fs, CsOther);
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
            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrow(() => raster.Dispose());

            Assert.True(raster.IsDisposed);
        }

        [Test]
        public void DisposeAsyncTest()
        {
            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(async () => await raster.DisposeAsync().ConfigureAwait(false));

            Assert.True(raster.IsDisposed);
        }

        #endregion

        #region CreateTile

        [Test]
        public void CreateTileImageNormal()
        {
            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, raster.GeoCoordinateSystem);

            Assert.DoesNotThrow(() =>
            {
                using Image image = raster.CreateTileImage(raster.Data, tile);
            });
        }

        [Test]
        public void CreateTileImageNullImage()
        {
            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, raster.GeoCoordinateSystem);

            Assert.Throws<ArgumentNullException>(() =>
            {
                using Image image = raster.CreateTileImage(null, tile);
            });
        }

        [Test]
        public void CreateTileImageNullTile()
        {
            using Raster raster = new(_in4326, Cs4326);

            Assert.Throws<ArgumentNullException>(() =>
            {
                using Image image = raster.CreateTileImage(raster.Data, null);
            });
        }

        [Test]
        public void CreateTileImageDifferentInterpolations()
        {
            using Raster raster = new(_in4326, Cs4326);
            Number number = Locations.TokyoGeodeticNtmsNumber;

            using RasterTile t1 = new(number, raster.GeoCoordinateSystem) { Interpolation = NetVips.Enums.Kernel.Nearest };
            using RasterTile t2 = new(number, raster.GeoCoordinateSystem) { Interpolation = NetVips.Enums.Kernel.Linear };
            using RasterTile t3 = new(number, raster.GeoCoordinateSystem) { Interpolation = NetVips.Enums.Kernel.Cubic };
            using RasterTile t4 = new(number, raster.GeoCoordinateSystem) { Interpolation = NetVips.Enums.Kernel.Mitchell };
            using RasterTile t5 = new(number, raster.GeoCoordinateSystem) { Interpolation = NetVips.Enums.Kernel.Lanczos2 };
            using RasterTile t6 = new(number, raster.GeoCoordinateSystem) { Interpolation = NetVips.Enums.Kernel.Lanczos3 };

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

            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, Cs4326) { Path = outPath };

            Assert.DoesNotThrowAsync(() => raster.WriteTileToFileAsync(raster.Data, tile));

            File.Delete(outPath);
        }

        [Test]
        public void WriteTileToFileNullTile()
        {
            using Raster raster = new(_in4326, Cs4326);

            Assert.ThrowsAsync<ArgumentNullException>(() => raster.WriteTileToFileAsync(raster.Data, null));
        }

        #endregion

        [Test]
        public void WriteTileToEnumberable()
        {
            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            IEnumerable<byte> tileBytes = null;

            Assert.DoesNotThrow(() => tileBytes = raster.WriteTileToEnumerable(raster.Data, tile));
            Assert.True(tileBytes?.Any() == true);
        }

        #region WriteToChannel

        [Test]
        public void WriteToChannelNormal()
        {
            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            Assert.True(raster.WriteTileToChannel(raster.Data, tile, channel.Writer));
        }

        [Test]
        public void WriteToChannelNullTile()
        {
            using Raster raster = new(_in4326, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            Assert.Throws<ArgumentNullException>(() => raster.WriteTileToChannel(raster.Data, null, channel.Writer));
        }

        [Test]
        public void WriteToChannelNullWriter()
        {
            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, Cs4326);

            Assert.Throws<ArgumentNullException>(() => raster.WriteTileToChannel(raster.Data, tile, null));
        }

        [Test]
        public void WriteToChannelAsyncNormal()
        {
            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, raster.GeoCoordinateSystem);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTileToChannelAsync(raster.Data, tile, channel.Writer).ConfigureAwait(false));
        }

        [Test]
        public void WriteToChannelAsyncNullTile()
        {
            using Raster raster = new(_in4326, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            Assert.ThrowsAsync<ArgumentNullException>(async () => await raster.WriteTileToChannelAsync(raster.Data, null, channel.Writer).ConfigureAwait(false));
        }

        [Test]
        public void WriteToChannelAsyncNullWriter()
        {
            using Raster raster = new(_in4326, Cs4326);
            using RasterTile tile = new(Locations.TokyoGeodeticNtmsNumber, raster.GeoCoordinateSystem);

            Assert.ThrowsAsync<ArgumentNullException>(async () => await raster.WriteTileToChannelAsync(raster.Data, tile, null).ConfigureAwait(false));
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

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11));

            Directory.Delete(outPath, true);
        }

        [Test]
        public void WriteTilesToDirectoryOverrideArgs()
        {
            // With overriden args

            string outPath = Path.Combine(_outPath, _timestamp);

            static void Reporter(string s) { }

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
              true, new Size(64, 64), TileExtension.Jpg, NetVips.Enums.Kernel.Cubic, 3, 999, 10,
              new Progress<double>(), Reporter));

            Directory.Delete(outPath, true);
        }

        [Test]
        public void WriteTilesToDirectoryLowTileCacheCount()
        {
            string outPath = Path.Combine(_outPath, _timestamp);

            using Raster raster = new(_in4326, Cs4326);

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => raster.WriteTilesToDirectoryAsync(
                outPath, 0, 11, tileCacheCount: 0));

            Directory.Delete(outPath, true);
        }

        #endregion

        #region Data-related WriteToDirectory

        [Test]
        public void Proj4326Ntms256PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_256_png_lanczos3_4");

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public void Proj4326Tms256PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_tms_256_png_lanczos3_4");

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   true));
        }

        [Test]
        public void Proj4326Ntms128PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_128_png_lanczos3_4");

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileSize: new Size(128, 128)));
        }

        [Test]
        public void Proj4326Tms256WebpLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_tms_256_webp_lanczos3_4");

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileExtension: TileExtension.Webp));
        }

        [Test]
        public void Proj4326Ntms256JpgLanczos3Bands3Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_256_jpg_lanczos3_3");

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   tileExtension: TileExtension.Jpg, bandsCount: 3));
        }

        [Test]
        public void Proj4326Ntms256PngCubicBands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_ntms_256_png_cubic_4");

            using Raster raster = new(_in4326, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11,
                   interpolation: NetVips.Enums.Kernel.Cubic));
        }

        [Test]
        public void Proj3857Ntms256PngLanczos3Bands4Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_3857_ntms_256_png_lanczos3_4");

            using Raster raster = new(_in3785, Cs3857);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj4326To3857Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_4326_to_3857_tiles");
            string tmp = Path.Combine(_outPath, $"{_timestamp}_4326_to_3857{GdalWorker.TempFileName}");

            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in4326, tmp, Cs3857).ConfigureAwait(false);

            await using Raster raster = new(tmp, Cs3857);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj3857To4326Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_3857_to_4326_tiles");
            string tmp = Path.Combine(_outPath, $"{_timestamp}_3857_to_4326{GdalWorker.TempFileName}");

            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in3785, tmp, Cs4326).ConfigureAwait(false);

            await using Raster raster = new(tmp, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        [Test]
        public async Task Proj3395To4326Directory()
        {
            string outPath = Path.Combine(_outPath, $"{_timestamp}_3395_to_4326_tiles");
            string tmp = Path.Combine(_outPath, $"{_timestamp}_3395_to_4326{GdalWorker.TempFileName}");

            await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in3395, tmp, Cs4326).ConfigureAwait(false);

            await using Raster raster = new(tmp, Cs4326);

            Assert.DoesNotThrowAsync(() => raster.WriteTilesToDirectoryAsync(outPath, 0, 11));
        }

        #endregion

        #region WriteTilesToChannel

        [Test]
        public void WriteTilesToChannelNormal()
        {
            // With default args

            using Raster raster = new(_in4326, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToChannelAsync(channel.Writer, 0, 11).ConfigureAwait(false));
        }

        [Test]
        public void WriteTilesToChannelOverrideArgs()
        {
            // With overriden args

            static void Reporter(string s) { }

            using Raster raster = new(_in4326, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            Assert.DoesNotThrowAsync(async () => await raster.WriteTilesToChannelAsync(channel.Writer, 0, 11,
                   true, new Size(128, 128), NetVips.Enums.Kernel.Cubic, 3, 999, 10, new Progress<double>(), Reporter).ConfigureAwait(false));
        }

        [Test]
        public void WriteTilesToChannelLowTileCacheCount()
        {
            // With default args

            using Raster raster = new(_in4326, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();

            Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await
                    raster.WriteTilesToChannelAsync(channel.Writer, 0, 11, tileCacheCount: 0).ConfigureAwait(false));
        }

        #endregion

        #region WriteTilesToEnumerable

        [Test]
        public void WriteTilesToEnumerableNormal()
        {
            // With default args

            using Raster raster = new(_in4326, Cs4326);

            IEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToEnumerable(0, 11));

            Assert.True(tiles?.Any());
        }

        [Test]
        public void WriteTilesToEnumerableOverrideArgs()
        {
            // With overriden args

            static void Reporter(string s) { }

            using Raster raster = new(_in4326, Cs4326);

            IEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToEnumerable(0, 11,
                   true, new Size(128, 128), NetVips.Enums.Kernel.Mitchell, 3, 999, new Progress<double>(), Reporter));

            Assert.True(tiles?.Any());
        }

        [Test]
        public void WriteTilesToEnumerableLowTileCache()
        {
            using Raster raster = new(_in4326, Cs4326);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                RasterTile[] tiles = raster.WriteTilesToEnumerable(0, 11, tileCacheCount: 0).ToArray();
            });
        }

        #endregion

        #region WriteTilesToAsyncEnumerable

        [Test]
        public void WriteTilesToAsyncEnumerableNormal()
        {
            // With default args

            using Raster raster = new(_in4326, Cs4326);

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

            using Raster raster = new(_in4326, Cs4326);

            IAsyncEnumerable<ITile> tiles = null;
            Assert.DoesNotThrow(() => tiles = raster.WriteTilesToAsyncEnumerable(0, 11,
                   true, new Size(128, 128), NetVips.Enums.Kernel.Nearest, 3, 999, 10, new Progress<double>(), Reporter));

            Assert.DoesNotThrowAsync(async () =>
            {
                await foreach (ITile tile in tiles) tile.Validate(false);
            });
        }

        #endregion

        #endregion

        #region GetBorders

        #region GetBorders using stream

        [Test]
        public void GetBordersStreamNormal()
        {
            using FileStream fs = File.OpenRead(_in4326);

            GeodeticCoordinate expectedMin = new(139.74999904632568, 35.61293363571167);
            GeodeticCoordinate expectedMax = new(139.8459792137146, 35.688271522521973);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(fs, Cs4326));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersStreamMercator()
        {
            using FileStream fs = File.OpenRead(_in3785);

            MercatorCoordinate expectedMin = new(15556898.732197443, 4247491.006264816);
            MercatorCoordinate expectedMax = new(15567583.19555743, 4257812.3937404491);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(fs, Cs3857));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersNullStream() => Assert.Throws<ArgumentNullException>(() => Raster.GetBorders(inputStream: null, Cs4326));

        [Test]
        public void GetBordersClosedStream()
        {
            FileStream fs = File.OpenRead(_in4326);
            fs.Dispose();

            Assert.Throws<ArgumentException>(() => Raster.GetBorders(fs, Cs4326));
        }

        [Test]
        public void GetBordersStreamOtherCs()
        {
            using FileStream fs = File.OpenRead(_in4326);

            Assert.Throws<NotSupportedException>(() => Raster.GetBorders(fs, CsOther));
        }

        #endregion

        #region GetBorders using file path

        [Test]
        public void GetBordersFilePathNormal()
        {
            GeodeticCoordinate expectedMin = new(139.74999904632568, 35.61293363571167);
            GeodeticCoordinate expectedMax = new(139.8459792137146, 35.688271522521973);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(_in4326, Cs4326));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersFilePathMercator()
        {
            MercatorCoordinate expectedMin = new(15556898.732197443, 4247491.006264816);
            MercatorCoordinate expectedMax = new(15567583.19555743, 4257812.3937404491);

            Coordinate min = null;
            Coordinate max = null;

            Assert.DoesNotThrow(() => (min, max) = Raster.GetBorders(_in3785, Cs3857));

            Assert.True(min == expectedMin && max == expectedMax);
        }

        [Test]
        public void GetBordersFilePathOtherCs() => Assert.Throws<NotSupportedException>(() => Raster.GetBorders(_in4326, CsOther));

        #endregion

        #endregion

        #region JoinTilesIntoImage

        #region Paths

        [Test]
        public void JoinTilesIntoImagePathsNormal()
        {
            // We need to create some tiles first:
            string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            Size tileSize = Tile.DefaultSize;
            const int bandsCount = 4;
            raster.WriteTilesToDirectory(path, sourceZ, sourceZ);

            // Now start the test
            string[] imgs = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToArray();
            Assert.DoesNotThrowAsync(() => Raster.JoinTilesIntoImageAsync(imgs[0], imgs[1], imgs[2], imgs[3], tileSize, bandsCount));

            // If you want to view the result on disk
            //string outPath = Path.Combine(path, "tile.png");
            //tileImage.WriteToFile(outPath);

            Directory.Delete(path, true);
        }

        [Test]
        public void JoinTilesIntoImagePathsNullPaths() => Assert.True(Raster.JoinTilesIntoImage(tile0Path: null, null, null, null, Tile.DefaultSize, 4) == null);

        [Test]
        public void JoinTilesIntoImagePathsNullSize() => Assert.Throws<ArgumentNullException>(() => Raster.JoinTilesIntoImage(tile0Path: null, null, null, null, null, 4));

        [Test]
        [Combinatorial]
        public void JoinTilesIntoImagePathsBadBands([Values(0, 5)] int b) => Assert.Throws<ArgumentOutOfRangeException>(() => Raster.JoinTilesIntoImage(tile0Path: null, null, null, null, Tile.DefaultSize, b));

        #endregion

        #region Bytes

        [Test]
        public void JoinTilesIntoImageBytesNormal()
        {
            // We need to create some tiles first:
            //string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            Size tileSize = Tile.DefaultSize;
            const int bandsCount = 4;
            RasterTile[] baseTiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToArray();

            // Now start the test
            Assert.DoesNotThrowAsync(() => Raster.JoinTilesIntoImageAsync(baseTiles[0].Bytes, baseTiles[1].Bytes,
                                                                baseTiles[2].Bytes, baseTiles[3].Bytes,
                                                                tileSize, bandsCount));

            // If you want to view the result on disk
            //Directory.CreateDirectory(path);
            //string outPath = Path.Combine(path, "tile.png");
            //tileImage.WriteToFile(outPath);
        }

        [Test]
        public void JoinTilesIntoImageBytesNullArrays() => Assert.True(Raster.JoinTilesIntoImage(tile0Bytes: null, null, null, null, Tile.DefaultSize, 4) == null);

        [Test]
        public void JoinTilesIntoImageBytesNullSize() => Assert.Throws<ArgumentNullException>(() => Raster.JoinTilesIntoImage(tile0Bytes: null, null, null, null, null, 4));

        [Test]
        [Combinatorial]
        public void JoinTilesIntoImageBytesBadBands([Values(0, 5)] int b) => Assert.Throws<ArgumentOutOfRangeException>(() => Raster.JoinTilesIntoImage(tile0Bytes: null, null, null, null, Tile.DefaultSize, b));

        #endregion

        #region RasterTiles

        [Test]
        public void JoinTilesIntoImageBuffered()
        {
            // We need to create some tiles first:
            //string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            const bool isBuffered = true;
            Size tileSize = Tile.DefaultSize;
            const int bandsCount = 4;
            RasterTile[] baseTiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToArray();

            // Now start the test
            Assert.DoesNotThrowAsync(() => Raster.JoinTilesIntoImageAsync(baseTiles[0], baseTiles[1], baseTiles[2],
                                                                          baseTiles[3], isBuffered, tileSize,
                                                                          bandsCount));

            // If you want to view the result on disk
            //Directory.CreateDirectory(path);
            //string outPath = Path.Combine(path, "tile.png");
            //tileImage.WriteToFile(outPath);
        }

        [Test]
        public void JoinTilesIntoImagePaths()
        {
            // We need to create some tiles first:
            string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const bool tmsCompatible = false;
            const int sourceZ = 12;
            const bool isBuffered = true;
            Size tileSize = Tile.DefaultSize;
            const int bandsCount = 4;
            const string tileExtension = ".png";
            raster.WriteTilesToDirectory(path, sourceZ, sourceZ);

            // Now start the test
            (Number minNumber, Number maxNumber) =
                GeoCoordinate.GetNumbers(raster.MinCoordinate, raster.MaxCoordinate, sourceZ, tileSize, tmsCompatible);

            Number n0 = minNumber;
            Number n1 = new(n0.X + 1, n0.Y, n0.Z);
            Number n2 = new(n0.X, n0.Y + 1, n0.Z);
            Number n3 = maxNumber;

            using RasterTile t0 = new(n0, Cs4326) { Path = Path.Combine(path, $"{sourceZ}", $"{n0.X}", $"{n0.Y}{tileExtension}") };
            using RasterTile t1 = new(n1, Cs4326) { Path = Path.Combine(path, $"{sourceZ}", $"{n1.X}", $"{n1.Y}{tileExtension}") };
            using RasterTile t2 = new(n2, Cs4326) { Path = Path.Combine(path, $"{sourceZ}", $"{n2.X}", $"{n2.Y}{tileExtension}") };
            using RasterTile t3 = new(n3, Cs4326) { Path = Path.Combine(path, $"{sourceZ}", $"{n3.X}", $"{n3.Y}{tileExtension}") };

            Assert.DoesNotThrow(() => Raster.JoinTilesIntoImage(t0, t1, t2, t3, isBuffered, tileSize, bandsCount));

            // If you want to view the result on disk
            //string outPath = Path.Combine(path, "tile.png");
            //tileImage.WriteToFile(outPath);

            Directory.Delete(path, true);
        }

        #endregion

        #endregion

        #region JoinTilesIntoBytes

        [Test]
        public void JoinTilesIntoBytesNormal()
        {
            // We need to create some tiles first:
            //string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            const bool isBuffered = true;
            Size tileSize = Tile.DefaultSize;
            const int bandsCount = 4;
            const TileExtension tileExtension = TileExtension.Png;
            RasterTile[] baseTiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToArray();

            // Now start the test
            IEnumerable<byte> bytes = null;
            Assert.DoesNotThrow(() => bytes = Raster.JoinTilesIntoBytes(baseTiles[0], baseTiles[1], baseTiles[2],
                                                                        baseTiles[3], isBuffered, tileSize, bandsCount,
                                                                        tileExtension));

            // If you want to view the result on disk
            //Directory.CreateDirectory(path);
            //string outPath = Path.Combine(path, "tile.png");
            //File.WriteAllBytes(outPath, bytes.ToArray());

            Assert.True(bytes?.Any() == true);
        }

        [Test]
        public void JoinTilesIntoBytesNullTiles()
        {
            const bool isBuffered = true;
            Size tileSize = Tile.DefaultSize;
            const int bandsCount = 4;
            const TileExtension tileExtension = TileExtension.Png;

            // Now start the test
            IEnumerable<byte> bytes = null;
            Assert.DoesNotThrow(() => bytes = Raster.JoinTilesIntoBytes(null, null, null, null,
                                                                        isBuffered, tileSize, bandsCount,
                                                                        tileExtension));

            Assert.True(bytes == null);
        }

        #endregion

        #region CreateOverviewTile

        [Test]
        public void CreateOverviewTileNormal()
        {
            // We need to create some tiles first:
            //string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            const bool isBuffered = true;
            RasterTile[] baseTiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToArray();

            // Now start the test

            // Any number passes, no calculations inside
            Number num = new(1, 1, 1);
            RasterTile target = new(num, Cs4326);
            Assert.DoesNotThrow(() => Raster.CreateOverviewTile(ref target, baseTiles[0], baseTiles[1], baseTiles[2],
                                                                baseTiles[3], isBuffered));

            // If you want to view the result on disk
            //Directory.CreateDirectory(path);
            //string outPath = Path.Combine(path, "tile.png");
            //File.WriteAllBytes(outPath, target.Bytes.ToArray());

            Assert.True(target.Bytes?.Any() == true);

            target.Dispose();
        }

        [Test]
        public void CreateOverviewTileNullTarget()
        {
            const bool isBuffered = true;
            RasterTile target = null;

            Assert.Throws<ArgumentNullException>(() => Raster.CreateOverviewTile(ref target, null, null,
                                                     null, null, isBuffered));
        }

        [Test]
        public void CreateOverviewTileFromArrayNormal()
        {
            // We need to create some tiles first:
            //string path = Path.Combine(_outPath, _timestamp);
            using Raster raster = new(_in4326, Cs4326);
            const int sourceZ = 12;
            const bool isBuffered = true;
            HashSet<RasterTile> baseTiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToHashSet();

            // Now start the test
            Number num = new(3638, 617, 11);
            RasterTile target = new(num, Cs4326);
            Assert.DoesNotThrow(() => Raster.CreateOverviewTile(ref target, baseTiles, isBuffered));

            // If you want to view the result on disk
            //Directory.CreateDirectory(path);
            //string outPath = Path.Combine(path, "tile.png");
            //File.WriteAllBytes(outPath, target.Bytes.ToArray());

            Assert.True(target.Bytes?.Any() == true);

            target.Dispose();
        }

        [Test]
        public void CreateOverviewTileFromArrayNullTarget()
        {
            const bool isBuffered = true;
            RasterTile target = null;

            Assert.Throws<ArgumentNullException>(() => Raster.CreateOverviewTile(ref target, null, isBuffered));
        }

        [Test]
        public void CreateOverviewTileFromArrayNullArray()
        {
            const bool isBuffered = true;
            RasterTile target = new(new Number(1, 1, 1), Cs4326);

            Assert.Throws<ArgumentNullException>(() => Raster.CreateOverviewTile(ref target, null, isBuffered));

            target.Dispose();
        }

        #endregion

        #region CreateOverviewTiles

        [Test]
        public async Task CreateOverviewTilesNormal()
        {
            //string outPath = Path.Combine(_outPath, _timestamp);

            Raster raster = new(_in4326, Cs4326);

            const int sourceZ = 12;

            HashSet<RasterTile> tiles = raster.WriteTilesToEnumerable(sourceZ, sourceZ).ToHashSet();

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();
            Assert.DoesNotThrowAsync(() => raster.CreateOverviewTilesAsync(channel.Writer, 11, 11, tiles, true, Cs4326)
                                                             .ContinueWith(_ => channel.Writer.Complete(), TaskScheduler.Default));

            //int ctr = 0;
            //await foreach (RasterTile tile in channel.Reader.ReadAllAsync())
            //{
            //    CheckHelper.CheckDirectory(outPath);
            //    await File.WriteAllBytesAsync(Path.Combine(outPath, $"{ctr}.png"), tile.Bytes.ToArray());
            //    ctr++;
            //}

            await raster.DisposeAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task CreateOverviewTilesNullChannel()
        {
            Raster raster = new(_in4326, Cs4326);

            Assert.ThrowsAsync<ArgumentNullException>(() => raster.CreateOverviewTilesAsync(null, 11, 11, null, true, Cs4326));

            await raster.DisposeAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task CreateOverviewTilesSmallMinZ()
        {
            Raster raster = new(_in4326, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => raster.CreateOverviewTilesAsync(channel.Writer, -1, 11, null, true, Cs4326));
            await raster.DisposeAsync().ConfigureAwait(false);
        }

        [Test]
        public async Task CreateOverviewTilesSmallMaxZ()
        {
            Raster raster = new(_in4326, Cs4326);

            Channel<RasterTile> channel = Channel.CreateUnbounded<RasterTile>();
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => raster.CreateOverviewTilesAsync(channel.Writer, 11, -1, null, true, Cs4326));
            await raster.DisposeAsync().ConfigureAwait(false);
        }

        #endregion

        #endregion
    }
}

#pragma warning restore CA1508 // Avoid dead conditional code
