#pragma warning disable CA1031 // Do not catch general exception types

using System;
using GTiff2Tiles.Core.Enums;
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
        public void TileExtensions()
        {
            try
            {
                TileExtension png = TileExtension.Png;
                TileExtension jpg = TileExtension.Jpg;
                TileExtension webp = TileExtension.Webp;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public void TileType()
        {
            try
            {
                TileType raster = Core.Enums.TileType.Raster;
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
