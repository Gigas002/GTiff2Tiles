using System;
using System.IO;
using System.Threading.Tasks;
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
        public async Task GenerateTilesByJoining()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input4326}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.GenerateTilesByJoiningOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            try
            {
                IProgress<double> progress = new Progress<double>(Console.WriteLine);
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ);
                await image.GenerateTilesByJoining(tempDirectoryInfo, progress, Enums.Multithreading.ThreadsCount);
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
        public async Task GenerateTilesByCropping()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName, Enums.FileSystemEntries.Temp));
            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input4326}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName, Enums.FileSystemEntries.GenerateTilesByCroppingOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            try
            {
                IProgress<double> progress = new Progress<double>(Console.WriteLine);
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ);
                await image.GenerateTilesByCropping(tempDirectoryInfo, progress, Enums.Multithreading.ThreadsCount);
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
