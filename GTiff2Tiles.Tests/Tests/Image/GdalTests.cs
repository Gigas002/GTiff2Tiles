using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Constants.Gdal;
using GTiff2Tiles.Core.Helpers;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Image
{
    public sealed class GdalTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public async Task CheckAndRepair3785Async()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input3785}{Extensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                //Check and repair.
                if (!await CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Gdal.TempFileName}{Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Gdal.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Gdal.RepairTifOptions);
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
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input3395}{Extensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                //Check and repair.
                if (!await CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Gdal.TempFileName}{Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Gdal.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Gdal.RepairTifOptions);
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
