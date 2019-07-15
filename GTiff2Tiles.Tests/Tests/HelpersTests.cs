using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Helpers;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests
{
    public class HelpersTests
    {
        [SetUp]
        public void SetUp() { }

        [Test]
        public async Task CheckHelperTests()
        {
            try
            {
                DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
                string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                    Enums.FileSystemEntries.InputDirectoryName,
                                                    $"{Enums.FileSystemEntries.Input4326}{Core.Enums.Extensions.Tif}");
                FileInfo inputFileInfo = new FileInfo(inputFilePath);
                CheckHelper.CheckDirectory(examplesDirectoryInfo, false);
                await CheckHelper.CheckInputFile(inputFileInfo);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
