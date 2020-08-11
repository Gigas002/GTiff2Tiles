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
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using NetVips;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests
{
    [TestFixture]
    public sealed class GdalWorkerTests
    {
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

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg3857);
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress).ConfigureAwait(false);

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

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg4326);
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress).ConfigureAwait(false);

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

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg4326);
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress).ConfigureAwait(false);

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
        public void WarpNullInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg3857);
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.WarpAsync(null, outPath, options.ToArray(), progress).ConfigureAwait(false));
        }

        [Test]
        public void WarpNonExistingInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg3857);
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.WarpAsync("ShouldFail", outPath, options.ToArray(), progress).ConfigureAwait(false));
        }

        [Test]
        public void WarpNullOutput()
        {
            string inPath = Constants.FileSystemEntries.Input4326FilePath;

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg3857);
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.WarpAsync(inPath, null, options.ToArray(), progress).ConfigureAwait(false));
        }

        [Test]
        public async Task WarpExistingOutput()
        {
            // Only because File.Create can fail
            Constants.FileSystemEntries.OutputDirectoryInfo.Create();

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            FileStream fs = File.Create(outPath);
            // Must dispose explicitly to delete correctly
            await fs.DisposeAsync().ConfigureAwait(false);

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg3857);
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray(), progress).ConfigureAwait(false);
            }
            catch (FileException)
            {
                Assert.Pass();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Fail();
        }

        [Test]
        public void WarpNullOptions()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.WarpAsync(inPath, outPath, null, progress).ConfigureAwait(false));
        }

        [Test]
        public async Task WarpNullProgress()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");

            List<string> options = GdalWorker.ConvertCoordinateSystemOptions.ToList();
            options.AddRange(GdalWorker.SrsEpsg3857);

            try
            {
                await GdalWorker.WarpAsync(inPath, outPath, options.ToArray()).ConfigureAwait(false);

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
            string gdalInfo = await GdalWorker.InfoAsync(Constants.FileSystemEntries.Input4326FilePath).ConfigureAwait(false);

            Assert.False(string.IsNullOrWhiteSpace(gdalInfo));
        }

        [Test]
        public void InfoNullPath() => Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.InfoAsync(null).ConfigureAwait(false));

        [Test]
        public void InfoNonExistantPath() => Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.InfoAsync("ShouldFail").ConfigureAwait(false));

        #endregion

        #region GetProjString tests

        [Test]
        public async Task GetProjStringNormal()
        {
            string proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input4326FilePath).ConfigureAwait(false);

            Assert.False(string.IsNullOrWhiteSpace(proj));
        }

        [Test]
        public void GetProjStringNullPath() => Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.GetProjStringAsync(null).ConfigureAwait(false));

        [Test]
        public void GetProjStringNonExistantPath() => Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.GetProjStringAsync("ShouldFail").ConfigureAwait(false));

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
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress).ConfigureAwait(false);

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
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress).ConfigureAwait(false);

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
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress).ConfigureAwait(false);

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
        public void ConvertGeoTiffToTargetSystemNullInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync(null, outPath, cs, progress).ConfigureAwait(false));
        }

        [Test]
        public void ConvertGeoTiffToTargetSystemNonExistingInput()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<FileNotFoundException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync("ShouldFail", outPath, cs, progress).ConfigureAwait(false));
        }

        [Test]
        public void ConvertGeoTiffToTargetSystemNullOutput()
        {
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, null, cs, progress).ConfigureAwait(false));
        }

        [Test]
        public async Task ConvertGeoTiffToTargetSystemExistingOutput()
        {
            // Only because File.Create can fail
            Constants.FileSystemEntries.OutputDirectoryInfo.Create();

            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            FileStream fs = File.Create(outPath);
            // Must dispose explicitly to delete correctly
            await fs.DisposeAsync().ConfigureAwait(false);

            try
            {
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress).ConfigureAwait(false);
            }
            catch (FileException)
            {
                Assert.Pass();
            }
            finally
            {
                File.Delete(outPath);
            }

            Assert.Fail();
        }

        [Test]
        public void ConvertGeoTiffToTargetSystemOtherCs()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string inPath = Constants.FileSystemEntries.Input4326FilePath;
            string outPath = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath,
                                          $"{timestamp}{GdalWorker.TempFileName}");
            const CoordinateSystem cs = CoordinateSystem.Other;
            Progress<double> progress = new Progress<double>(Console.WriteLine);

            Assert.ThrowsAsync<NotSupportedException>(async () =>
               await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs, progress).ConfigureAwait(false));
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
                await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inPath, outPath, cs).ConfigureAwait(false);

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
            string proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input4326FilePath).ConfigureAwait(false);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);
            Assert.True(cs == CoordinateSystem.Epsg4326);

            proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input3785FilePath).ConfigureAwait(false);
            cs = GdalWorker.GetCoordinateSystem(proj);
            Assert.True(cs == CoordinateSystem.Epsg3857);

            proj = await GdalWorker.GetProjStringAsync(Constants.FileSystemEntries.Input3395FilePath).ConfigureAwait(false);
            cs = GdalWorker.GetCoordinateSystem(proj);
            Assert.True(cs == CoordinateSystem.Other);

            Assert.Pass();
        }

        #endregion

        #region GetGeoTransform tests

        [Test]
        public void GetGeoTransformNormal()
        {
            double[] gt = GdalWorker.GetGeoTransform(Constants.FileSystemEntries.Input4326FilePath);
            Assert.True(gt?.Any());
        }

        [Test]
        public void GetGeoTransformNullPath() => Assert.Throws<ArgumentNullException>(() =>
               GdalWorker.GetGeoTransform(null));

        [Test]
        public void GetGeoTransformNonExistantPath() => Assert.Throws<FileNotFoundException>(() =>
               GdalWorker.GetGeoTransform("ShouldFail"));

        #endregion

        #region GetImageBorders tests

        [Test]
        public async Task GetImageBordersNormal()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            string proj = await GdalWorker.GetProjStringAsync(path).ConfigureAwait(false);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) = GdalWorker.GetImageBorders(path, size, cs);

            Assert.True(minCoordinate is GeodeticCoordinate && maxCoordinate is GeodeticCoordinate);
        }

        [Test]
        public async Task GetImageBordersNullPath()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            string proj = await GdalWorker.GetProjStringAsync(path).ConfigureAwait(false);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            Assert.Throws<ArgumentNullException>(() => GdalWorker.GetImageBorders(null, size, cs));
        }

        [Test]
        public async Task GetImageBordersNonExistingPath()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            string proj = await GdalWorker.GetProjStringAsync(path).ConfigureAwait(false);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            Assert.Throws<FileNotFoundException>(() => GdalWorker.GetImageBorders("ShouldFail", size, cs));
        }

        [Test]
        public async Task GetImageBordersNullSize()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            string proj = await GdalWorker.GetProjStringAsync(path).ConfigureAwait(false);
            CoordinateSystem cs = GdalWorker.GetCoordinateSystem(proj);

            Assert.Throws<ArgumentNullException>(() => GdalWorker.GetImageBorders(path, null, cs));
        }

        [Test]
        public void GetImageBordersOtherCs()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;

            Image image = Image.NewFromFile(path);
            Size size = new Size(image.Width, image.Height);
            const CoordinateSystem cs = CoordinateSystem.Other;

            Assert.Throws<NotSupportedException>(() => GdalWorker.GetImageBorders(path, size, cs));
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
