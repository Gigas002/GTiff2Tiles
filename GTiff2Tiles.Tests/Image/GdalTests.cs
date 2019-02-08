using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Image
{
    public class GdalTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void CheckAndRepair3785()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(Core.Enums.DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input3785}{Core.Enums.Extensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                //Check and repair.
                if (!Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                    Core.Image.Gdal.RepairTif(inputFileInfo, tempDirectoryInfo);

                //Check if temp file was created successfuly.
                if (!tempDirectoryInfo.EnumerateFiles().Any())
                    Assert.Fail("Temp file wasn't created.");

                //Delete temp directory.
                tempDirectoryInfo.Delete(true);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            Assert.Pass();
        }

        [Test]
        public void CheckAndRepair3395()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(Core.Enums.DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input3395}{Core.Enums.Extensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                //Check and repair.
                if (!Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                    Core.Image.Gdal.RepairTif(inputFileInfo, tempDirectoryInfo);

                //Check if temp file was created successfuly.
                if (!tempDirectoryInfo.EnumerateFiles().Any())
                    Assert.Fail("Temp file wasn't created.");

                //Delete temp directory.
                tempDirectoryInfo.Delete(true);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            Assert.Pass();
        }
    }
}
