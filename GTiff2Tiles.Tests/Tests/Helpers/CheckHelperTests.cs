using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Helpers
{
    [TestFixture]
    public sealed class CheckHelperTests
    {
        #region SetUp and consts

        private string _timestamp;

        private string _outPath;

        private readonly string _in4326 = FileSystemEntries.Input4326FilePath;

        private const string ShouldFail = "ShouldFail";

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

        [SetUp]
        public void SetUp()
        {
            _timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                               CultureInfo.InvariantCulture);
            _outPath = Path.Combine(FileSystemEntries.OutputDirectoryPath,
                                    $"{_timestamp}");

            FileSystemEntries.OutputDirectoryInfo.Create();
        }

        #endregion

        #region CheckFile

        [Test]
        public void CheckFileNullPath() => Assert.Throws<ArgumentNullException>(() => CheckHelper.CheckFile(null));

        [Test]
        public void CheckFileTrueExist()
        {
            string ext = Path.GetExtension(_in4326);

            Assert.DoesNotThrow(() => CheckHelper.CheckFile(_in4326, true, ext));
            Assert.Throws<FileException>(() => CheckHelper.CheckFile(_in4326, false, ext));
        }

        [Test]
        public void CheckFileFalseExist()
        {
            Assert.DoesNotThrow(() => CheckHelper.CheckFile(ShouldFail, false));
            Assert.Throws<FileNotFoundException>(() => CheckHelper.CheckFile(ShouldFail));
        }

        [Test]
        public void CheckFileNullExist() => Assert.DoesNotThrow(() => CheckHelper.CheckFile(ShouldFail, null));

        [Test]
        public void CheckFileNullExtension()
        {
            Assert.DoesNotThrow(() => CheckHelper.CheckFile(_in4326));
            Assert.Throws<FileException>(() => CheckHelper.CheckFile(_in4326, false));
        }

        [Test]
        public void CheckFileWrongExtension()
        {
            const string ext = ".notexistentextension";

            Assert.Throws<ArgumentException>(() => CheckHelper.CheckFile(_in4326, null, ext));
        }

        #endregion

        #region CheckDirectory

        [Test]
        public void CheckDirectoryNullPath() => Assert.Throws<ArgumentNullException>(() => CheckHelper.CheckDirectory(null));

        [Test]
        public void CheckDirectoryNormal()
        {
            Assert.DoesNotThrow(() => CheckHelper.CheckDirectory(_outPath));

            Directory.Delete(_outPath);
        }

        [Test]
        public void CheckDirectoryTrueEmpty()
        {
            Assert.DoesNotThrow(() => CheckHelper.CheckDirectory(_outPath, true));
            Assert.Throws<DirectoryException>(() => CheckHelper.CheckDirectory(_outPath, false));

            Directory.Delete(_outPath);
        }

        [Test]
        public void CheckDirectoryFalseEmpty()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Assert.DoesNotThrow(() => CheckHelper.CheckDirectory(path, false));
            Assert.Throws<DirectoryException>(() => CheckHelper.CheckDirectory(path, true));
        }

        #endregion

        #region CheckInputFileAsync

        [Test]
        public async Task CheckInputFileAsyncTrue() => Assert.True(await CheckHelper.CheckInputFileAsync(_in4326, Cs4326));

        [Test]
        public async Task CheckInputFileAsyncFalse() => Assert.False(await CheckHelper.CheckInputFileAsync(_in4326, Cs3857));

        #endregion
    }
}
