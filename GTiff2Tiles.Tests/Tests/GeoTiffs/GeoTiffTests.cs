using System;
using System.IO;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
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
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            const int memCache = 13000000;

            Assert.DoesNotThrow(() =>
            {
                Raster raster = new Raster(inputPath, memCache);
            });
        }

        [Test]
        public void CreateRasterFromFileWoCsOtherCs()
        {
            string inputPath = Constants.FileSystemEntries.Input3395FilePath;

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new Raster(inputPath);
            });
        }

        [Test]
        public void CreateRasterFromFileWoCsSmallMemChache()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            const int memCache = -1;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Raster raster = new Raster(inputPath, memCache);
            });
        }

        [Test]
        public void CreateRasterFromFileWoCsDontUseMemCache()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
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
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
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
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Other;

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new Raster(inputPath, cs);
            });
        }

        [Test]
        public void CreateRasterFromFileWCsSmallMemChache()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
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
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
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
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
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
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.Throws<ArgumentNullException>(() =>
            {
                Raster raster = new Raster(null, cs);
            });
        }

        [Test]
        public void CreateRasterFromStreamOtherCs()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            using FileStream fs = File.OpenRead(inputPath);
            const CoordinateSystem cs = CoordinateSystem.Other;

            Assert.Throws<NotSupportedException>(() =>
            {
                Raster raster = new Raster(fs, cs);
            });
        }

        #endregion

        #endregion
    }
}
