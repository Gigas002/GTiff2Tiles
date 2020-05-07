#pragma warning disable CA1031 // Do not catch general exception types

using System;
using GTiff2Tiles.Core.Exceptions.Gdal;
using GTiff2Tiles.Core.Exceptions.Image;
using GTiff2Tiles.Core.Exceptions.Tile;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class ExceptionsTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void ImageGdalException()
        {
            try
            {
                GdalException gdalException = new GdalException(string.Empty);
                GdalException gdalException2 = new GdalException(string.Empty, null);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void ImageImageException()
        {
            try
            {
                RasterException imageException = new RasterException(string.Empty);
                RasterException imageException2 = new RasterException(string.Empty, null);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void TileTileException()
        {
            try
            {
                TileException tileException = new TileException(string.Empty);
                TileException tileException2 = new TileException(string.Empty, null);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
