using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.Helpers;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Helpers
{
    [TestFixture]
    public sealed class CheckHelperTests
    {
        #region CheckFile

        [Test]
        public void CheckFileNullPath() => Assert.Throws<ArgumentNullException>(() => CheckHelper.CheckFile(null));

        [Test]
        public void CheckFileTrueExist()
        {
            string filePath = Constants.FileSystemEntries.Input4326FilePath;
            string ext = Path.GetExtension(filePath);

            Assert.DoesNotThrow(() => CheckHelper.CheckFile(filePath, true, ext));
            Assert.Throws<FileException>(() => CheckHelper.CheckFile(filePath, false, ext));
        }

        [Test]
        public void CheckFileFalseExist()
        {
            const string filePath = "Shouldn't exist";

            Assert.DoesNotThrow(() => CheckHelper.CheckFile(filePath, false));
            Assert.Throws<FileNotFoundException>(() => CheckHelper.CheckFile(filePath));
        }

        [Test]
        public void CheckFileNullExist()
        {
            const string filePath = "Shouldn't exist";

            Assert.DoesNotThrow(() => CheckHelper.CheckFile(filePath, null));
        }

        [Test]
        public void CheckFileNullExtension()
        {
            string filePath = Constants.FileSystemEntries.Input4326FilePath;

            Assert.DoesNotThrow(() => CheckHelper.CheckFile(filePath));
            Assert.Throws<FileException>(() => CheckHelper.CheckFile(filePath, false));
        }

        [Test]
        public void CheckFileWrongExtension()
        {
            string filePath = Constants.FileSystemEntries.Input4326FilePath;
            const string ext = ".notexistentextension";

            Assert.Throws<ArgumentException>(() => CheckHelper.CheckFile(filePath, null, ext));
        }

        #endregion

        #region CheckDirectory

        [Test]
        public void CheckDirectoryNullPath() => Assert.Throws<ArgumentNullException>(() => CheckHelper.CheckDirectory(null));

        [Test]
        public void CheckDirectoryNormal()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string path = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath, $"{timestamp}");

            Assert.DoesNotThrow(() => CheckHelper.CheckDirectory(path));

            Directory.Delete(path);
        }

        [Test]
        public void CheckDirectoryTrueEmpty()
        {
            string timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                                     CultureInfo.InvariantCulture);
            string path = Path.Combine(Constants.FileSystemEntries.OutputDirectoryPath, $"{timestamp}");

            Assert.DoesNotThrow(() => CheckHelper.CheckDirectory(path, true));
            Assert.Throws<DirectoryException>(() => CheckHelper.CheckDirectory(path, false));

            Directory.Delete(path);
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
        public async Task CheckInputFileAsyncTrue()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.True(await CheckHelper.CheckInputFileAsync(path, cs));
        }

        [Test]
        public async Task CheckInputFileAsyncFalse()
        {
            string path = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;

            Assert.False(await CheckHelper.CheckInputFileAsync(path, cs));
        }

        #endregion
    }
}
