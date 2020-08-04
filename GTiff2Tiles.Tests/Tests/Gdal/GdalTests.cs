#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Gdal
{
    public sealed class GdalTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public async Task CheckAndRepair3785Async()
        {
            // TODO: coordinate system
            var coordinateSystem = CoordinateSystem.Epsg4326;

            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                FileSystemEntries.InputDirectoryName,
                                                $"{FileSystemEntries.Input3785}{FileExtensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                //Check and repair.
                if (!await CheckHelper.CheckInputFileAsync(inputFileInfo.FullName, coordinateSystem))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{GdalWorker.TempFileName}{FileExtensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputFileInfo.FullName, tempFileInfo.FullName, coordinateSystem).ConfigureAwait(false);
                }

                //Check if temp file was created successfuly.
                if (!tempDirectoryInfo.EnumerateFiles().Any()) Assert.Fail("Temp file wasn't created.");

                //Delete temp directory.
                tempDirectoryInfo.Delete(true);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public async Task CheckAndRepair3395Async()
        {
            // TODO: coordinate system
            var coordinateSystem = CoordinateSystem.Epsg4326;

            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                FileSystemEntries.InputDirectoryName,
                                                $"{FileSystemEntries.Input3395}{FileExtensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                //Check and repair.
                if (!await CheckHelper.CheckInputFileAsync(inputFileInfo.FullName, coordinateSystem))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{GdalWorker.TempFileName}{FileExtensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await GdalWorker.ConvertGeoTiffToTargetSystemAsync(inputFileInfo.FullName, tempFileInfo.FullName, coordinateSystem).ConfigureAwait(false);
                }

                //Check if temp file was created successfuly.
                if (!tempDirectoryInfo.EnumerateFiles().Any()) Assert.Fail("Temp file wasn't created.");

                //Delete temp directory.
                tempDirectoryInfo.Delete(true);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
