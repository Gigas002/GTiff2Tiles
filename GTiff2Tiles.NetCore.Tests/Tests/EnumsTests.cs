using System;
using NUnit.Framework;

// ReSharper disable UnusedVariable
#pragma warning disable 219

namespace GTiff2Tiles.NetCore.Tests.Tests
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
                string lonLat = Core.Enums.Image.Gdal.LongLat;
                string wgs84 = Core.Enums.Image.Gdal.Wgs84;
                string typeByte = Core.Enums.Image.Gdal.Byte;
                string[] repairTifOptions = Core.Enums.Image.Gdal.RepairTifOptions;
                string tempFileName = Core.Enums.Image.Gdal.TempFileName;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void Algorithms()
        {
            try
            {
                string crop = Core.Enums.Algorithms.Crop;
                string join = Core.Enums.Algorithms.Join;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void DateTimePatterns()
        {
            try
            {
                string longWithMs = Core.Enums.DateTimePatterns.LongWithMs;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void Extensions()
        {
            try
            {
                string png = Core.Enums.Extensions.Png;
                string tif = Core.Enums.Extensions.Tif;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
