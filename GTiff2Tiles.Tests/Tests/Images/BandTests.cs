#pragma warning disable CS0219 // The variable is assigned but it's value is never used

using System;
using System.Collections.Generic;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Tests.Constants;
using NetVips;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Images
{
    [TestFixture]
    public sealed class BandTests
    {
        #region SetUp and consts

        private readonly string _in4326 = FileSystemEntries.Input4326FilePath;

        [SetUp]
        public void SetUp() => NetVipsHelper.DisableLog();

        #endregion

        #region Constructors

        [Test]
        public void CreateBandDefault() => Assert.DoesNotThrow(() =>
        {
            Band band = new();
        });

        [Test]
        public void CreateBandWithValue() => Assert.DoesNotThrow(() =>
        {
            Band band = new(128);
        });

        [Test]
        [Combinatorial]
        public void CreateBandBad([Values(-1, 256)] int value) => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Band band = new(value);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrow(() =>
        {
            Band band = new();
            int val = band.Value;
            int def = Band.DefaultValue;
        });

        #endregion

        #region Methods

        #region AddBands

        [Test]
        public void AddBandsNormal()
        {
            Image image = Image.NewFromFile(_in4326, true);
            IEnumerable<Band> bands = new[] { new Band() };

            int countOfBands = image.Bands;
            Assert.DoesNotThrow(() => Band.AddBands(ref image, bands));
            Assert.True(image.Bands > countOfBands);

            image.Dispose();
        }

        [Test]
        public void AddBandsNullImage()
        {
            Image image = null;
            IEnumerable<Band> bands = new[] { new Band() };

            Assert.Throws<ArgumentNullException>(() => Band.AddBands(ref image, bands));
        }

        [Test]
        public void AddBandsNullBands()
        {
            Image image = Image.NewFromFile(_in4326, true);

            Assert.Throws<ArgumentNullException>(() => Band.AddBands(ref image, null));

            image.Dispose();
        }

        #endregion

        #region AddDefaultBands

        [Test]
        [Combinatorial]
        public void AddDefaultBandsBadValuesDoesntThrow([Values(-1, 0, 1, 2, 3)] int bandsCount)
        {
            Image image = Image.NewFromFile(_in4326, true);

            int countOfBands = image.Bands;
            Assert.DoesNotThrow(() => Band.AddDefaultBands(ref image, bandsCount));
            Assert.True(image.Bands == countOfBands);

            image.Dispose();
        }

        [Test]
        [Combinatorial]
        public void AddDefaultBandsBadValuesNormal([Values(4, 5)] int bandsCount)
        {
            Image image = Image.NewFromFile(_in4326, true);

            int countOfBands = image.Bands;
            Assert.DoesNotThrow(() => Band.AddDefaultBands(ref image, bandsCount));
            Assert.True(image.Bands > countOfBands);

            image.Dispose();
        }

        [Test]
        public void AddDefaultBandsNullImage()
        {
            Image image = null;
            const int bandsCount = 4;

            Assert.Throws<ArgumentNullException>(() => Band.AddDefaultBands(ref image, bandsCount));
        }

        #endregion

        #endregion
    }
}

#pragma warning restore CS0219 // The variable is assigned but it's value is never used
