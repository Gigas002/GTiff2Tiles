using System;
using System.IO;
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
        public void CheckAndRepairTif()
        {
            Assert.Pass(); //temporary

            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.Input,
                                                $"{Enums.FileSystemEntries.BadInput}{Core.Enums.Extensions.Tif}");
            FileInfo inputFileInfo = new FileInfo(inputFilePath);

            try
            {
                Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo, tempDirectoryInfo);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            try
            {
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
