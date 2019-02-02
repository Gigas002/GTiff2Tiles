using System;
using System.IO;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Image
{
    public class ImageTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GenerateTiles()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.Input,
                                                $"{Enums.FileSystemEntries.Input}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.GenerateTilesOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            try
            {
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ);
                image.GenerateTiles(tempDirectoryInfo);
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

        [Test]
        public void GenerateTilesOld()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.Input,
                                                $"{Enums.FileSystemEntries.Input}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.GenerateTilesOldOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            try
            {
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ);
                image.GenerateTilesOld(tempDirectoryInfo, Enums.Multithreading.ThreadsCount);
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
