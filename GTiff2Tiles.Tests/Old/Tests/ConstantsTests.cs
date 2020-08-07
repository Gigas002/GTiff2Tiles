#pragma warning disable CA1031 // Do not catch general exception types

using System;
using GTiff2Tiles.Core;
using NUnit.Framework;

// ReSharper disable UnusedVariable
#pragma warning disable 219

namespace GTiff2Tiles.Tests.Old.Tests
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
                string lonLat = Core.Constants.Proj.ProjLongLat;
                string wgs84 = Core.Constants.Proj.DatumWgs84;
                string[] repairTifOptions = GdalWorker.ConvertCoordinateSystemOptions;
                string tempFileName = GdalWorker.TempFileName;
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
