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
        public void GenerateTilesByJoining()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.Input,
                                                $"{Enums.FileSystemEntries.Input}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.GenerateTilesByJoiningOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            try
            {
                IProgress<double> progress = new Progress<double>(Console.WriteLine);
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ);
                image.GenerateTilesByJoining(tempDirectoryInfo, progress, Enums.Multithreading.ThreadsCount);
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
        public void GenerateTilesByCropping()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.Input,
                                                $"{Enums.FileSystemEntries.Input}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.GenerateTilesByCroppingOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            try
            {
                IProgress<double> progress = new Progress<double>(Console.WriteLine);
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ);
                image.GenerateTilesByCropping(tempDirectoryInfo, progress, Enums.Multithreading.ThreadsCount);
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
