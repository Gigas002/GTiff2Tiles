#pragma warning disable CA1031 // Do not catch general exception types

using System;
using GTiff2Tiles.Core.Exceptions;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class ExceptionsTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void GdalException()
        {
            try
            {
                GdalException gdalException = new GdalException();
                GdalException gdalException1 = new GdalException(string.Empty);
                GdalException gdalException2 = new GdalException(string.Empty, null);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void RasterException()
        {
            try
            {
                RasterException rasterException = new RasterException();
                RasterException rasterException1 = new RasterException(string.Empty);
                RasterException rasterException2 = new RasterException(string.Empty, null);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void TileException()
        {
            try
            {
                TileException tileException = new TileException();
                TileException tileException1 = new TileException(string.Empty);
                TileException tileException2 = new TileException(string.Empty, null);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
