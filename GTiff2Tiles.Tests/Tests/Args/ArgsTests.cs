using System;
using GTiff2Tiles.Core.Args;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NetVips;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Args
{
    [TestFixture]
    public sealed class ArgsTests
    {
        #region Constants

        private const int MinZ = 0;

        private const int MaxZ = 17;

        #endregion

        #region Constructors

        [Test]
        public void CreateArgsNormal() => Assert.DoesNotThrow(() =>
        {
            using WriteRasterTilesArgs args = new(MinZ, MaxZ);
        });

        [Test]
        public void CreateArgsNormalBadMinZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            using WriteRasterTilesArgs args = new(MinZ - 1, MaxZ);
        });

        [Test]
        public void CreateArgsNormalBadMaxZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            using WriteRasterTilesArgs args = new(MinZ, -MaxZ);
        });

        #endregion

        #region Properties

        [Test]
        public void SetProperties1() => Assert.DoesNotThrow(() =>
        {
            using IWriteTilesArgs args = new WriteRasterTilesArgs(MinZ, MaxZ)
            {
                MinCoordinate = new GeodeticCoordinate(0.0, 0.0),
                MaxCoordinate = new GeodeticCoordinate(1.0, 1.0),
                OutputDirectoryPath = string.Empty,
                Progress = null,
                ThreadsCount = 1000,
                TileSize = Tile.DefaultSize,
                TmsCompatible = true
            };
        });

        [Test]
        public void SetProperties2() => Assert.DoesNotThrow(() =>
        {
            using WriteRasterTilesArgs args = new(MinZ, MaxZ)
            {
                TileCache = Image.Black(10, 10, 4), BandsCount = 1, MinCoordinate = new GeodeticCoordinate(0.0, 0.0),
                MaxCoordinate = new GeodeticCoordinate(1.0, 1.0), OutputDirectoryPath = string.Empty,
                Progress = null, ThreadsCount = 1000, TileSize = Tile.DefaultSize, TmsCompatible = true,
                TileCacheCount = 4, TileExtension = TileExtension.Webp, TileInterpolation = Interpolation.Cubic,
                TimePrinter = Console.WriteLine
            };
        });


        [Test]
        public void GetProperties1() => Assert.DoesNotThrow(() =>
        {
            using IWriteTilesArgs args = new WriteRasterTilesArgs(MinZ, MaxZ);
            int minz = args.MinZ;
            int maxz = args.MaxZ;
            bool isdis = args.IsDisposed;
            GeoCoordinate minc = args.MinCoordinate;
            GeoCoordinate maxc = args.MaxCoordinate;
            string outd = args.OutputDirectoryPath;
            IProgress<double> prog = args.Progress;
            int thc = args.ThreadsCount;
            Size ts = args.TileSize;
            bool tic = args.TmsCompatible;
        });

        [Test]
        public void GetProperties2() => Assert.DoesNotThrow(() =>
        {
            using WriteRasterTilesArgs args = new(MinZ, MaxZ);
            int minz = args.MinZ;
            int maxz = args.MaxZ;
            Image tc = args.TileCache;
            int bc = args.BandsCount;
            bool isdis = args.IsDisposed;
            GeoCoordinate minc = args.MinCoordinate;
            GeoCoordinate maxc = args.MaxCoordinate;
            string outd = args.OutputDirectoryPath;
            IProgress<double> prog = args.Progress;
            int thc = args.ThreadsCount;
            int tcc = args.TileCacheCount;
            TileExtension te = args.TileExtension;
            Interpolation ti = args.TileInterpolation;
            Size ts = args.TileSize;
            WriteRasterTilesArgs.PrintTime tp = args.TimePrinter;
            bool tic = args.TmsCompatible;
        });

        #endregion

        #region Delegate

        [Test]
        public void TestPrintTimer()
        {
            using WriteRasterTilesArgs args = new(MinZ, MaxZ);

            Assert.DoesNotThrow(() =>
            {
                WriteRasterTilesArgs.PrintTime pt = Console.WriteLine;
                args.TimePrinter = pt;
                args.TimePrinter.Invoke("message");
            });
        }

        #endregion

        #region Dispose

        [Test]
        public void DisposeTest()
        {
            WriteRasterTilesArgs args = new(MinZ, MaxZ);

            Assert.DoesNotThrow(() => args.Dispose());

            Assert.True(args.IsDisposed);
        }

        #endregion
    }
}
