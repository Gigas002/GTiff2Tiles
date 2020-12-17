#pragma warning disable CA1508 // Avoid dead conditional code

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GTiff2Tiles.Core;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Tests.Constants;
using NetVips;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests
{
    [TestFixture]
    public sealed class GdalWorkerTests
    {
        #region SetUp and consts

        private string _timestamp;

        private string _outPath;

        private readonly IProgress<double> _progress = new Progress<double>();

        private List<string> _gdalWarpOptions;

        private readonly string _in4326 = FileSystemEntries.Input4326FilePath;

        private readonly string _in3785 = FileSystemEntries.Input3785FilePath;

        private readonly string _in3395 = FileSystemEntries.Input3395FilePath;

        private const string ShouldFail = "ShouldFail";

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

        private const CoordinateSystem CsOther = CoordinateSystem.Other;

        [SetUp]
        public void SetUp()
        {
            _timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                               CultureInfo.InvariantCulture);
            _outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath,
                                    $"{_timestamp}{GdalWorker.TempFileName}");
            _gdalWarpOptions = GdalWorker.ConvertCoordinateSystemOptions.ToList();

            FileSystemEntries.OutputDirectoryInfo.Create();
            NetVipsHelper.DisableLog();
        }

        #endregion

        #region Properties tests

        [Test]
        public void Properties() => Assert.DoesNotThrow(() =>
        {
            string[] srsEpsg4326 = GdalWorker.SrsEpsg4326;
            string[] srsEpsg3857 = GdalWorker.SrsEpsg3857;
            string[] convertCoordinateSystemOptions = GdalWorker.ConvertCoordinateSystemOptions;
            string tempFileName = GdalWorker.TempFileName;
        });

        #endregion

        #region ConfigureGdal tests

        [Test]
        public void ConfigureGdal() => Assert.DoesNotThrow(GdalWorker.ConfigureGdal);

        #endregion

        #region Warp tests

        [Test]
        public void Warp4326To3857()
        {
            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg3857);

            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.WarpAsync(_in4326, _outPath, _gdalWarpOptions.ToArray(), _progress).ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        [Test]
        public void Warp3785To4326()
        {
            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg4326);

            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.WarpAsync(_in3785, _outPath, _gdalWarpOptions.ToArray(), _progress).ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        [Test]
        public void Warp3395To4326()
        {
            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg4326);

            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.WarpAsync(_in3395, _outPath, _gdalWarpOptions.ToArray(), _progress).ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        [Test]
        public void WarpNullInput()
        {
            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg3857);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.WarpAsync(null, _outPath, _gdalWarpOptions.ToArray(), _progress).ConfigureAwait(false));
        }

        [Test]
        public void WarpNonExistingInput()
        {
            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg3857);

            Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.WarpAsync(ShouldFail, _outPath, _gdalWarpOptions.ToArray(), _progress).ConfigureAwait(false));
        }

        [Test]
        public void WarpNullOutput()
        {
            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg3857);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.WarpAsync(_in4326, null, _gdalWarpOptions.ToArray(), _progress).ConfigureAwait(false));
        }

        [Test]
        public void WarpExistingOutput()
        {
            FileStream fs = File.Create(_outPath);

            // Must dispose explicitly to delete correctly
            fs.Dispose();

            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg3857);

            Assert.ThrowsAsync<FileException>(async () =>
                   await GdalWorker.WarpAsync(_in4326, _outPath, _gdalWarpOptions.ToArray(), _progress).ConfigureAwait(false));

            File.Delete(_outPath);
        }

        [Test]
        public void WarpNullOptions() => Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.WarpAsync(_in4326, _outPath, null, _progress).ConfigureAwait(false));

        [Test]
        public void WarpNullProgress()
        {
            _gdalWarpOptions.AddRange(GdalWorker.SrsEpsg3857);

            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.WarpAsync(_in4326, _outPath, _gdalWarpOptions.ToArray()).ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        #endregion

        #region Info tests

        [Test]
        public void InfoNormal()
        {
            string gdalInfo = null;

            Assert.DoesNotThrowAsync(async () => gdalInfo = await GdalWorker.InfoAsync(_in4326).ConfigureAwait(false));

            Assert.False(string.IsNullOrWhiteSpace(gdalInfo));
        }

        [Test]
        public void InfoNullPath() => Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.InfoAsync(null).ConfigureAwait(false));

        [Test]
        public void InfoNonExistantPath() => Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.InfoAsync(ShouldFail).ConfigureAwait(false));

        #endregion

        #region GetProjString tests

        [Test]
        public void GetProjStringNormal()
        {
            string proj = null;

            Assert.DoesNotThrowAsync(async () => proj = await GdalWorker.GetProjStringAsync(_in4326).ConfigureAwait(false));

            Assert.False(string.IsNullOrWhiteSpace(proj));
        }

        [Test]
        public void GetProjStringNullPath() => Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.GetProjStringAsync(null).ConfigureAwait(false));

        [Test]
        public void GetProjStringNonExistantPath() => Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.GetProjStringAsync(ShouldFail).ConfigureAwait(false));

        #endregion

        #region ConvertGeoTiffToTargetSystem tests

        [Test]
        public void ConvertGeoTiffToTargetSystem4326To3857()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in4326, _outPath, Cs3857, _progress).ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        [Test]
        public void ConvertGeoTiffToTargetSystem3785To4326()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in3785, _outPath, Cs4326, _progress)
                                .ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        [Test]
        public void ConvertGeoTiffToTargetSystem3395To4326()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in3395, _outPath, Cs4326, _progress)
                                .ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        [Test]
        public void ConvertGeoTiffToTargetSystemNullInput() => Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync(null, _outPath, Cs3857, _progress).ConfigureAwait(false));

        [Test]
        public void ConvertGeoTiffToTargetSystemNonExistingInput() => Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync(ShouldFail, _outPath, Cs3857, _progress).ConfigureAwait(false));

        [Test]
        public void ConvertGeoTiffToTargetSystemNullOutput() => Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in4326, null, Cs3857, _progress).ConfigureAwait(false));

        [Test]
        public void ConvertGeoTiffToTargetSystemExistingOutput()
        {
            FileStream fs = File.Create(_outPath);

            // Must dispose explicitly to delete correctly
            fs.Dispose();

            Assert.ThrowsAsync<FileException>(async () => await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in4326, _outPath, Cs3857, _progress).ConfigureAwait(false));

            File.Delete(_outPath);
        }

        [Test]
        public void ConvertGeoTiffToTargetSystemOtherCs() => Assert.ThrowsAsync<NotSupportedException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in4326, _outPath, CsOther, _progress).ConfigureAwait(false));

        [Test]
        public void ConvertGeoTiffToTargetSystemNullProgress()
        {
            Assert.DoesNotThrowAsync(async () =>
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(_in4326, _outPath, Cs3857).ConfigureAwait(false);

                CheckHelper.CheckFile(_outPath);
            });

            File.Delete(_outPath);
        }

        #endregion

        #region GetCoordinateSystem tests

        [Test]
        public void GetCoordinateSystem()
        {
            string proj;
            CoordinateSystem cs = CoordinateSystem.Other;

            Assert.DoesNotThrowAsync(async () =>
            {
                proj = await GdalWorker.GetProjStringAsync(_in4326).ConfigureAwait(false);
                cs = GdalWorker.GetCoordinateSystem(proj);
            });
            Assert.True(cs == Cs4326);

            Assert.DoesNotThrowAsync(async () =>
            {
                proj = await GdalWorker.GetProjStringAsync(_in3785).ConfigureAwait(false);
                cs = GdalWorker.GetCoordinateSystem(proj);
            });
            Assert.True(cs == Cs3857);

            Assert.DoesNotThrowAsync(async () =>
            {
                proj = await GdalWorker.GetProjStringAsync(_in3395).ConfigureAwait(false);
                cs = GdalWorker.GetCoordinateSystem(proj);
            });
            Assert.True(cs == CsOther);
        }

        #endregion

        #region GetGeoTransform tests

        [Test]
        public void GetGeoTransformNormal()
        {
            double[] gt = null;

            Assert.DoesNotThrow(() => gt = GdalWorker.GetGeoTransform(_in4326));
            Assert.True(gt?.Any());
        }

        [Test]
        public void GetGeoTransformNullPath() => Assert.Throws<ArgumentNullException>(() =>
               GdalWorker.GetGeoTransform(null));

        [Test]
        public void GetGeoTransformNonExistantPath() => Assert.Throws<FileNotFoundException>(() =>
               GdalWorker.GetGeoTransform(ShouldFail));

        #endregion

        #region GetImageBorders tests

        [Test]
        public void GetImageBordersNormal()
        {
            using Image image = Image.NewFromFile(_in4326);
            Size size = new(image.Width, image.Height);
            string proj = GdalWorker.GetProjString(_in4326);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            GeoCoordinate minCoordinate = null;
            GeoCoordinate maxCoordinate = null;

            Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = GdalWorker.GetImageBorders(_in4326, size, cs));
            Assert.True(minCoordinate is GeodeticCoordinate && maxCoordinate is GeodeticCoordinate);
        }

        [Test]
        public void GetImageBordersNullPath()
        {
            using Image image = Image.NewFromFile(_in4326);
            Size size = new(image.Width, image.Height);
            string proj = GdalWorker.GetProjString(_in4326);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            Assert.Throws<ArgumentNullException>(() => GdalWorker.GetImageBorders(null, size, cs));
        }

        [Test]
        public void GetImageBordersNonExistingPath()
        {
            using Image image = Image.NewFromFile(_in4326);
            Size size = new(image.Width, image.Height);
            string proj = GdalWorker.GetProjString(_in4326);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            Assert.Throws<FileNotFoundException>(() => GdalWorker.GetImageBorders(ShouldFail, size, cs));
        }

        [Test]
        public void GetImageBordersNullSize()
        {
            string proj = GdalWorker.GetProjString(_in4326);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            Assert.Throws<ArgumentNullException>(() => GdalWorker.GetImageBorders(_in4326, null, cs));
        }

        [Test]
        public void GetImageBordersOtherCs()
        {
            using Image image = Image.NewFromFile(_in4326);
            Size size = new(image.Width, image.Height);

            Assert.Throws<NotSupportedException>(() => GdalWorker.GetImageBorders(_in4326, size, CsOther));
        }

        [Test]
        public void GetImageBordersWrongCs1()
        {
            using Image image = Image.NewFromFile(_in4326);
            Size size = new(image.Width, image.Height);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) = GdalWorker.GetImageBorders(_in4326, size, Cs3857);

                // Pass the tests too
                if (!(minCoordinate is GeodeticCoordinate && maxCoordinate is GeodeticCoordinate))
                    throw new ArgumentOutOfRangeException();
            });
        }

        [Test]
        public void GetImageBordersWrongCs2()
        {
            using Image image = Image.NewFromFile(_in3785);
            Size size = new(image.Width, image.Height);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) = GdalWorker.GetImageBorders(_in3785, size, Cs4326);

                // Pass the tests too
                if (!(minCoordinate is MercatorCoordinate && maxCoordinate is MercatorCoordinate))
                    throw new ArgumentOutOfRangeException();
            });
        }

        #endregion
    }
}

#pragma warning restore CA1508 // Avoid dead conditional code
