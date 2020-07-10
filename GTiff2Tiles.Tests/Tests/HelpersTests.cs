#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Tests.Constants;
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
                                                    FileSystemEntries.InputDirectoryName,
                                                    $"{FileSystemEntries.Input4326}{FileExtensions.Tif}");
                FileInfo inputFileInfo = new FileInfo(inputFilePath);
                CheckHelper.CheckDirectory(examplesDirectoryInfo, false);
                await CheckHelper.CheckInputFileAsync(inputFileInfo);
            }
            catch (Exception exception) { Assert.Fail(exception.Message); }

            Assert.Pass();
        }
    }
}
