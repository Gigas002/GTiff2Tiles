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

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(Core.Enums.DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input4326}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName,
                                                      Enums.FileSystemEntries
                                                           .GenerateTilesByJoiningOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);
            IProgress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                //Check for errors.
                Core.Helpers.CheckHelper.CheckOutputDirectory(outputDirectoryInfo);
                if (!Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName, $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    Core.Image.Gdal.Warp(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }
                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                await image.GenerateTilesByJoining(outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ, progress, Enums.Multithreading.ThreadsCount);
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

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(Core.Enums.DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input4326}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName,
                                                      Enums.FileSystemEntries
                                                           .GenerateTilesByCroppingOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);
            IProgress<double> progress = new Progress<double>(Console.WriteLine);

            try
            {
                //Check for errors.
                Core.Helpers.CheckHelper.CheckOutputDirectory(outputDirectoryInfo);
                if (!Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName, $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    Core.Image.Gdal.Warp(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }
                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                await image.GenerateTilesByCropping(outputDirectoryInfo, Enums.Zooms.MinZ, Enums.Zooms.MaxZ, progress, Enums.Multithreading.ThreadsCount);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            Assert.Pass();
        }
    }
}
