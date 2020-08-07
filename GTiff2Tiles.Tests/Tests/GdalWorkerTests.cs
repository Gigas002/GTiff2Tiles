#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using NetVips;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class GdalWorkerTests
    {
        #region Setup

        [SetUp]
        public void Setup() { }

        #endregion

        #region Properties tests

        [Test]
        public void Properties()
        {
            // Test get only, don't test contents or I'll have to rewrite this on every change

            string[] srsEpsg4326 = GdalWorker.SrsEpsg4326;
            string[] srsEpsg3857 = GdalWorker.SrsEpsg3857;
            string[] convertCoordinateSystemOptions = GdalWorker.ConvertCoordinateSystemOptions;
            string tempFileName = GdalWorker.TempFileName;

            Assert.Pass();
        }

        #endregion

        #region ConfigureGdal tests

        [Test]
        public void ConfigureGdal()
        {
            GdalWorker.ConfigureGdal();

            Assert.Pass();
        }

        #endregion

        #region Warp tests

        [Test]
        public async Task Warp4326To3857()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg3857);
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress);

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        [Test]
        public async Task Warp3785To4326()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input3785FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg4326);
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress);

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        [Test]
        public async Task Warp3395To4326()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input3395FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg4326);
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress);

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        [Test]
        public async Task WarpNullInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg3857);
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                await GdalWorker.WarpAsync(null, outPath, options.ToArray(), progress);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task WarpNonExistingInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg3857);
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                await GdalWorker.WarpAsync("ShouldFail", outPath, options.ToArray(), progress);
            }
            catch (FileNotFoundException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task WarpNullOutput()
        {
            string inPath = Constants.FileSystemEntries.Input4326FilePath;

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg3857);
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                await GdalWorker.WarpAsync(inPath, null, options.ToArray(), progress);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task WarpExistingOutput()
        {
            // Probably should throw-catch another exception?

            // Only because File.Create can fail
            Constants.FileSystemEntries.OutputDirectoryInfo.Create();

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg3857);
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                File.Create(outPath);
                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress);
            }
            catch (Exception)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task WarpNullOptions()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                Progress<double> progress = new Progress<double>(Console.WriteLine);

                await GdalWorker.WarpAsync(inPath, outPath, null, progress);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task WarpNullProgress()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            try
            {
                List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
                options.AddRange(GdalWorker.SrsEpsg3857);

                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray());

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        #endregion

        #region Info tests

        [Test]
        public async Task InfoNormal()
        {
            string gdalInfo = await GdalWorker.InfoAsync(Constants.FileSystemEntries.Input4326FilePath);
            if (string.IsNullOrWhiteSpace(gdalInfo)) Assert.Fail();

            Assert.Pass();
        }

        [Test]
        public async Task InfoNullPath()
        {
            try
            {
                await GdalWorker.InfoAsync(null);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task InfoNonExistantPath()
        {
            try
            {
                await GdalWorker.InfoAsync("ShouldFail");
            }
            catch (FileNotFoundException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        #endregion

        #region GetProjString tests

        [Test]
        public async Task GetProjStringNormal()
        {
            string proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input4326FilePath);
            if (string.IsNullOrWhiteSpace(proj)) Assert.Fail();

            Assert.Pass();
        }

        [Test]
        public async Task GetProjStringNullPath()
        {
            try
            {
                await GdalWorker.GetProjStringAsync(null);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task GetProjStringNonExistantPath()
        {
            try
            {
                await GdalWorker.GetProjStringAsync("ShouldFail");
            }
            catch (FileNotFoundException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        #endregion

        #region ConvertGeoTiffToTargetSystem tests

        [Test]
        public async Task ConvertGeoTiffToTargetSystem4326To3857()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress);

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystem3785To4326()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input3785FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress);

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystem3395To4326()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input3395FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress);

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystemNullInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(null, outPath, cs, progress);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystemNonExistingInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync("ShouldFail", outPath, cs, progress);
            }
            catch (FileNotFoundException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystemNullOutput()
        {
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, null, cs, progress);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystemExistingOutput()
        {
            // Probably should throw-catch another exception?

            Constants.FileSystemEntries.OutputDirectoryInfo.Create();

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                File.Create(outPath);
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress);
            }
            catch (Exception)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystemOtherCs()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Other;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress);
            }
            catch (NotSupportedException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystemNullProgress()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs);

                CheckHelper.CheckFile(outPath);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Pass();
        }

        #endregion

        #region GetCoordinateSystem tests

        [Test]
        public async Task GetCoordinateSystem()
        {
            string proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input4326FilePath);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);
            if (cs != CoordinateSystem.Epsg4326) Assert.Fail();

            proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input3785FilePath);
            cs = GdalWorker.GetCoordinateSystem(proj);
            if (cs != CoordinateSystem.Epsg3857) Assert.Fail();

            proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input3395FilePath);
            cs = GdalWorker.GetCoordinateSystem(proj);
            if (cs != CoordinateSystem.Other) Assert.Fail();

            Assert.Pass();
        }

        #endregion

        #region GetGeoTransform tests

        [Test]
        public void GetGeoTransformNormal()
        {
            double[] gt = GdalWorker.GetGeoTransform(Constants.FileSystemEntries.Input4326FilePath);
            if (gt?.Any() != true) Assert.Fail();

            Assert.Pass();
        }

        [Test]
        public void GetGeoTransformNullPath()
        {
            try
            {
                GdalWorker.GetGeoTransform(null);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void GetGeoTransformNonExistantPath()
        {
            try
            {
                GdalWorker.GetGeoTransform("ShouldFail");
            }
            catch (FileNotFoundException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        #endregion

        #region GetImageBorders tests

        [Test]
        public async Task GetImageBordersNormal()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            string proj = await GdalWorker.GetProjStringAsync(path);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) = GdalWorker.GetImageBorders(path, size, cs);

            if (!(minCoordinate is GeodeticCoordinate && maxCoordinate is GeodeticCoordinate)) Assert.Fail();

            Assert.Pass();
        }

        [Test]
        public async Task GetImageBordersNullPath()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            string proj = await GdalWorker.GetProjStringAsync(path);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            try
            {
                GdalWorker.GetImageBorders(null, size, cs);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task GetImageBordersNonExistingPath()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            string proj = await GdalWorker.GetProjStringAsync(path);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            try
            {
                GdalWorker.GetImageBorders("ShouldFail", size, cs);
            }
            catch (FileNotFoundException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public async Task GetImageBordersNullSize()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            string proj = await GdalWorker.GetProjStringAsync(path);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            try
            {
                GdalWorker.GetImageBorders(path, null, cs);
            }
            catch (ArgumentNullException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void GetImageBordersOtherCs()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            const CoordinateSystem cs = CoordinateSystem.Other;

            try
            {
                GdalWorker.GetImageBorders(path, size, cs);
            }
            catch (NotSupportedException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void GetImageBordersWrongCs1()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;

            try
            {
                (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) = GdalWorker.GetImageBorders(path, size, cs);

                if (!(minCoordinate is GeodeticCoordinate && maxCoordinate is GeodeticCoordinate)) Assert.Pass();
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        [Test]
        public void GetImageBordersWrongCs2()
        {
            string path = Constants.FileSystemEntries.Input3785FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            try
            {
                (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) = GdalWorker.GetImageBorders(path, size, cs);

                if (!(minCoordinate is MercatorCoordinate && maxCoordinate is MercatorCoordinate)) Assert.Pass();
            }
            catch (ArgumentOutOfRangeException)
            {
                Assert.Pass();
            }

            Assert.Fail();
        }

        #endregion
    }
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CA1031 // Do not catch general exception types
