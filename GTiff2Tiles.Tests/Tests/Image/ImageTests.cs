using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Constants.Image;
using GTiff2Tiles.Core.Helpers;
using NUnit.Framework;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace GTiff2Tiles.Tests.Tests.Image
{
    public sealed class ImageTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public async Task GenerateTmsTilesAsync()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input4326}{Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName,
                                                      Enums.FileSystemEntries.TmsCompatible,
                                                      Enums.FileSystemEntries
                                                           .GenerateTilesByCroppingOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);
            IProgress<double> progress = new Progress<double>(Console.WriteLine);
            const bool tmsCompatible = true;

            try
            {
                //Check for errors.
                CheckHelper.CheckDirectory(outputDirectoryInfo, true);

                if (!await CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Gdal.TempFileName}{Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                await using Core.Image.Raster image = new Core.Image.Raster(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                const string tileExtension = Extensions.Png;
                await image.GenerateTilesAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension,
                                               progress, threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public async Task GenerateNonTmsTilesAsync()
        {
            DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();

            DirectoryInfo tempDirectoryInfo =
                new DirectoryInfo(Path.Combine(Helpers.TestHelper.GetExamplesDirectoryInfo().FullName,
                                               Enums.FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                Enums.FileSystemEntries.InputDirectoryName,
                                                $"{Enums.FileSystemEntries.Input4326}{Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName,
                                                      Enums.FileSystemEntries.NonTmsCompatible,
                                                      Enums.FileSystemEntries
                                                           .GenerateTilesByCroppingOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);
            IProgress<double> progress = new Progress<double>(Console.WriteLine);
            const bool tmsCompatible = false;

            try
            {
                //Check for errors.
                CheckHelper.CheckDirectory(outputDirectoryInfo, true);

                if (!await CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Gdal.TempFileName}{Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                await using Core.Image.Raster image = new Core.Image.Raster(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                const string tileExtension = Extensions.Png;
                await image.GenerateTilesAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension,
                                               progress, threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
