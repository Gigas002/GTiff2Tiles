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
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input3785}{Core.Enums.Extensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo, tempDirectoryInfo);
                if (!tempDirectoryInfo.EnumerateFiles().Any())
                    Assert.Fail("Temp file wasn't created.");
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
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input3395}{Core.Enums.Extensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo, tempDirectoryInfo);
                if (!tempDirectoryInfo.EnumerateFiles().Any())
                    Assert.Fail("Temp file wasn't created.");
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
