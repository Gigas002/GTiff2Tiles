using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;

// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace GTiff2Tiles.NetCore.Tests.Tests.Image
{
    public sealed class ImageTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public async Task GenerateTmsTilesByJoiningAsync()
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
                                                      Enums.FileSystemEntries.TmsCompatible,
                                                      Enums.FileSystemEntries
                                                           .GenerateTilesByJoiningOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);
            IProgress<double> progress = new Progress<double>(Console.WriteLine);
            const bool tmsCompatible = true;

            try
            {
                //Check for errors.
                Core.Helpers.CheckHelper.CheckDirectory(outputDirectoryInfo, true);

                if (!await Core.Helpers.CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                await image.GenerateTilesByJoiningAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, progress,
                                                   threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public async Task GenerateTmsTilesByCroppingAsync()
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
                Core.Helpers.CheckHelper.CheckDirectory(outputDirectoryInfo, true);

                if (!await Core.Helpers.CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                await image.GenerateTilesByCroppingAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, progress,
                                                    threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public async Task GenerateNonTmsTilesByJoiningAsync()
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
                                                      Enums.FileSystemEntries.NonTmsCompatible,
                                                      Enums.FileSystemEntries
                                                           .GenerateTilesByJoiningOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);
            IProgress<double> progress = new Progress<double>(Console.WriteLine);
            const bool tmsCompatible = false;

            try
            {
                //Check for errors.
                Core.Helpers.CheckHelper.CheckDirectory(outputDirectoryInfo, true);

                if (!await Core.Helpers.CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                await image.GenerateTilesByJoiningAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, progress,
                                                   threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }

        [Test]
        public async Task GenerateNonTmsTilesByCroppingAsync()
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
                Core.Helpers.CheckHelper.CheckDirectory(outputDirectoryInfo, true);

                if (!await Core.Helpers.CheckHelper.CheckInputFileAsync(inputFileInfo))
                {
                    string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                                       $"{Core.Enums.Image.Gdal.TempFileName}{Core.Enums.Extensions.Tif}");
                    FileInfo tempFileInfo = new FileInfo(tempFilePath);

                    await Core.Image.Gdal.WarpAsync(inputFileInfo, tempFileInfo, Core.Enums.Image.Gdal.RepairTifOptions);
                    inputFileInfo = tempFileInfo;
                }

                //Create Image object and crop tiles.
                Core.Image.Image image = new Core.Image.Image(inputFileInfo);
                const int minZ = Enums.Zooms.MinZ;
                const int maxZ = Enums.Zooms.MaxZ;
                const int threadsCount = Enums.Multithreading.ThreadsCount;
                await image.GenerateTilesByCroppingAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, progress,
                                                    threadsCount);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
