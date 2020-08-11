using GTiff2Tiles.Core.Exceptions;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Exceptions
{
    [TestFixture]
    public sealed class ExceptionsTests
    {
        [Test]
        public void RasterExceptionConstructors() => Assert.DoesNotThrow(() =>
        {
            RasterException e1 = new RasterException();
            RasterException e2 = new RasterException(string.Empty);
            RasterException e3 = new RasterException(string.Empty, null);
        });

        [Test]
        public void FileExceptionConstructors() => Assert.DoesNotThrow(() =>
        {
            FileException e1 = new FileException();
            FileException e2 = new FileException(string.Empty);
            FileException e3 = new FileException(string.Empty, null);
        });

        [Test]
        public void DirectoryExceptionConstructors() => Assert.DoesNotThrow(() =>
        {
            DirectoryException e1 = new DirectoryException();
            DirectoryException e2 = new DirectoryException(string.Empty);
            DirectoryException e3 = new DirectoryException(string.Empty, null);
        });
    }
}
