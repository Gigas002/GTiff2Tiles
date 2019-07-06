using System;
using System.IO;
using GTiff2Tiles.Core.Helpers;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests
{
    public class HelpersTests
    {
        [SetUp]
        public void SetUp() { }

        [Test]
        public void CheckHelperTests()
        {
            try
            {
                DirectoryInfo examplesDirectoryInfo = Helpers.TestHelper.GetExamplesDirectoryInfo();
                string inputFilePath = Path.Combine(examplesDirectoryInfo.FullName,
                                                    Enums.FileSystemEntries.InputDirectoryName,
                                                    $"{Enums.FileSystemEntries.Input4326}{Core.Enums.Extensions.Tif}");
                FileInfo inputFileInfo = new FileInfo(inputFilePath);
                CheckHelper.CheckDirectory(examplesDirectoryInfo, false);
                CheckHelper.CheckInputFile(inputFileInfo);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
