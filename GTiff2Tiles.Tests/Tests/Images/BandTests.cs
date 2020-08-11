#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0219 // The variable is assigned but it's value is never used

using System;
using System.Collections.Generic;
using GTiff2Tiles.Core.Images;
using NetVips;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Images
{
    [TestFixture]
    public sealed class BandTests
    {
        #region Constructors

        [Test]
        public void CreateBandDefault() => Assert.DoesNotThrow(() =>
        {
            Band band = new Band();
        });

        [Test]
        public void CreateBandWithValue() => Assert.DoesNotThrow(() =>
        {
            Band band = new Band(128);
        });

        [Test]
        public void CreateBandSmall() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Band band = new Band(-1);
        });

        [Test]
        public void CreateBandBig() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Band band = new Band(256);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties() => Assert.DoesNotThrow(() =>
        {
            Band band = new Band();
            int val = band.Value;
            int def = Band.DefaultValue;
        });

        #endregion

        #region Methods

        #region AddBands

        [Test]
        public void AddBandsNormal()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            Image image = Image.NewFromFile(inputPath, true);
            IEnumerable<Band> bands = new[] { new Band() };

            int countOfBands = image.Bands;
            Assert.DoesNotThrow(() => Band.AddBands(ref image, bands));
            Assert.True(image.Bands > countOfBands);
        }

        [Test]
        public void AddBandsNullImage()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            Image image = null;
            IEnumerable<Band> bands = new[] { new Band() };

            Assert.Throws<ArgumentNullException>(() => Band.AddBands(ref image, bands));
        }

        [Test]
        public void AddBandsNullBands()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            Image image = Image.NewFromFile(inputPath, true);

            Assert.Throws<ArgumentNullException>(() => Band.AddBands(ref image, null));
        }

        #endregion

        #region AddDefaultBands

        [Test]
        [Combinatorial]
        public void AddDefaultBandsBadValuesDoesntThrow([Values(-1, 0, 1, 2, 3)] int bandsCount)
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            Image image = Image.NewFromFile(inputPath, true);

            int countOfBands = image.Bands;
            Assert.DoesNotThrow(() => Band.AddDefaultBands(ref image, bandsCount));
            Assert.True(image.Bands == countOfBands);
        }

        [Test]
        [Combinatorial]
        public void AddDefaultBandsBadValuesNormal([Values(4, 5)] int bandsCount)
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            Image image = Image.NewFromFile(inputPath, true);

            int countOfBands = image.Bands;
            Assert.DoesNotThrow(() => Band.AddDefaultBands(ref image, bandsCount));
            Assert.True(image.Bands > countOfBands);
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

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0219 // The variable is assigned but it's value is never used
