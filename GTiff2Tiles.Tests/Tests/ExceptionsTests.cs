using System;
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
                ImageException imageException = new ImageException(string.Empty);
                ImageException imageException2 = new ImageException(string.Empty, null);
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
