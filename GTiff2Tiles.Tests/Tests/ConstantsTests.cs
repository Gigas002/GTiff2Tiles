#pragma warning disable CA1031 // Do not catch general exception types

using System;
using NUnit.Framework;

// ReSharper disable UnusedVariable
#pragma warning disable 219

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class ConstantsTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void ImageGdal()
        {
            try
            {
                string lonLat = Core.Constants.Proj.LongLat;
                string wgs84 = Core.Constants.Proj.Wgs84;
                string typeByte = Core.Gdal.Gdal.Byte;
                string[] repairTifOptions = Core.Gdal.Gdal.RepairTifOptions;
                string tempFileName = Core.Gdal.Gdal.TempFileName;
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
                string png = Core.Constants.FileExtensions.Png;
                string tif = Core.Constants.FileExtensions.Tif;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
