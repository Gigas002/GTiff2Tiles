using GTiff2Tiles.Core.Exceptions;
using NUnit.Framework;

// ReSharper disable NotAccessedVariable
// ReSharper disable RedundantAssignment

namespace GTiff2Tiles.Tests.Tests
{
    [TestFixture]
    public sealed class ExceptionsTests
    {
        [Test]
        public void RasterExceptionConstructors() => Assert.DoesNotThrow(() =>
        {
            RasterException e = new();
            e = new RasterException(string.Empty);
            e = new RasterException(string.Empty, null);
        });

        [Test]
        public void FileExceptionConstructors() => Assert.DoesNotThrow(() =>
        {
            FileException e = new();
            e = new FileException(string.Empty);
            e = new FileException(string.Empty, null);
        });

        [Test]
        public void DirectoryExceptionConstructors() => Assert.DoesNotThrow(() =>
        {
            DirectoryException e = new();
            e = new DirectoryException(string.Empty);
            e = new DirectoryException(string.Empty, null);
        });
    }
}
