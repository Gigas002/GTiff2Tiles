#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums.Image;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Tests.Constants;
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
                                               FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                FileSystemEntries.InputDirectoryName,
                                                $"{FileSystemEntries.Input4326}{Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName,
                                                      FileSystemEntries.TmsCompatible,
                                                      FileSystemEntries
                                                           .GenerateTilesOutputDirectoryName);

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
                                                       $"{Core.Constants.Gdal.Gdal.TempFileName}{Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Gdal.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Core.Constants.Gdal.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                const int minZ = Zooms.MinZ;
                const int maxZ = Zooms.MaxZ;
                const int threadsCount = Multithreading.ThreadsCount;
                const string tileExtension = Extensions.Png;

                //TODO: tileExtension
                await Core.Image.Img.GenerateTilesAsync(inputFileInfo, outputDirectoryInfo, minZ, maxZ, TileType.Raster,
                                                        tmsCompatible, TileExtension.Webp, progress, threadsCount)
                          .ConfigureAwait(false);
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
                                               FileSystemEntries.Temp,
                                               DateTime.Now.ToString(DateTimePatterns.LongWithMs)));

            string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                FileSystemEntries.InputDirectoryName,
                                                $"{FileSystemEntries.Input4326}{Extensions.Tif}");
            string outputDirectoryName = Path.Combine(examplesDirectoryInfo.FullName,
                                                      FileSystemEntries.NonTmsCompatible,
                                                      FileSystemEntries
                                                           .GenerateTilesOutputDirectoryName);

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
                                                       $"{Core.Constants.Gdal.Gdal.TempFileName}{Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Gdal.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Core.Constants.Gdal.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                const int minZ = Zooms.MinZ;
                const int maxZ = Zooms.MaxZ;
                const int threadsCount = Multithreading.ThreadsCount;
                const string tileExtension = Extensions.Png;

                //TODO: tileExtension
                await Core.Image.Img.GenerateTilesAsync(inputFileInfo, outputDirectoryInfo, minZ, maxZ, TileType.Raster,
                                                        tmsCompatible, TileExtension.Webp, progress, threadsCount)
                          .ConfigureAwait(false);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
