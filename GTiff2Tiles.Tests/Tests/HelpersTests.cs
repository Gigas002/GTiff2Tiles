using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Helpers;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class HelpersTests
    {
        [SetUp]
        public void SetUp() { }

        [Test]
        public async Task CheckHelperTestsAsync()
        {
            try
            {
                DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
                string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                    Enums.FileSystemEntries.InputDirectoryName,
                                                    $"{Enums.FileSystemEntries.Input4326}{Extensions.Tif}");
                FileInfo inputFileInfo = new FileInfo(inputFilePath);
                CheckHelper.CheckDirectory(examplesDirectoryInfo, false);
                await CheckHelper.CheckInputFileAsync(inputFileInfo);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
