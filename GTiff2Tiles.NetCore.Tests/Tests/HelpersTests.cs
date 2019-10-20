using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.NetCore.Core.Helpers;
using NUnit.Framework;

namespace GTiff2Tiles.NetCore.Tests.Tests
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
                                                    $"{Enums.FileSystemEntries.Input4326}{Core.Enums.Extensions.Tif}");
                FileInfo inputFileInfo = new FileInfo(inputFilePath);
                CheckHelper.CheckDirectory(examplesDirectoryInfo, false);
                await CheckHelper.CheckInputFileAsync(inputFileInfo);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
