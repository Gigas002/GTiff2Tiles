using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Image
{
    public class ImageTests
    {
        [SetUp]
        public void Setup() { }

        //todo tms and non-tms tests

        [Test]
        public async Task GenerateTilesByJoining()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(
                Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
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
                Core.Helpers.CheckHelper.CheckDirectory(outputDirectoryInfo, true);
                if (!Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.Warp(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                await image.GenerateTilesByJoining(outputDirectoryInfo, minZ, maxZ, progress,
                                                   threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public async Task GenerateTilesByCropping()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo = new DirectoryInfo(
                Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
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
                Core.Helpers.CheckHelper.CheckDirectory(outputDirectoryInfo, true);
                if (!Core.Helpers.CheckHelper.CheckInputFile(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.Warp(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                await image.GenerateTilesByCropping(outputDirectoryInfo, minZ, maxZ, progress,
                                                    threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
