#pragma warning disable CA1031 // Do not catch general exception types

using System;
using NUnit.Framework;

// ReSharper disable UnusedVariable
#pragma warning disable 219

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class EnumsTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void ImageGdal()
        {
            try
            {
                string lonLat = Core.Constants.Gdal.Gdal.LongLat;
                string wgs84 = Core.Constants.Gdal.Gdal.Wgs84;
                string typeByte = Core.Constants.Gdal.Gdal.Byte;
                string[] repairTifOptions = Core.Constants.Gdal.Gdal.RepairTifOptions;
                string tempFileName = Core.Constants.Gdal.Gdal.TempFileName;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void DateTimePatterns()
        {
            try
            {
                string longWithMs = Core.Constants.DateTimePatterns.LongWithMs;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void Extensions()
        {
            try
            {
                string png = Core.Constants.Extensions.Png;
                string tif = Core.Constants.Extensions.Tif;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
