using System;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.TileMapResource;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources
{
    [TestFixture]
    public sealed class TileFormatTests
    {
        #region Consts

        private const string PngMimeType = "image/png";

        private const string WebpMimeType = "image/webp";

        private const string JpegMimeType = "image/jpeg";

        private const string PngExtensionString = "png";

        private const string WebpExtensionString = "webp";

        private const string JpgExtensionString = "jpg";

        private readonly Size _tileSize = new(256, 256);

        private const TileExtension PngExtension = TileExtension.Png;

        private const TileExtension JpgExtension = TileExtension.Jpg;

        private const TileExtension WebpExtension = TileExtension.Webp;

        #endregion

        #region Constructors

        [Test]
        public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new TileFormat());

        [Test]
        public void TileFormatPngNormal()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(_tileSize.Width, _tileSize.Height, PngMimeType, PngExtensionString));

            Assert.True(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(PngMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(PngExtensionString, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatPngNormal2()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(_tileSize, PngExtension));

            Assert.True(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(PngMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(PngExtensionString, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatWebpNormal()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(_tileSize, WebpExtension));

            Assert.True(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(WebpMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(WebpExtensionString, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatJpgNormal()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(_tileSize, JpgExtension));

            Assert.True(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(JpegMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(JpgExtensionString, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatBadSize() => Assert.Throws<ArgumentNullException>(() => _ = new TileFormat(null, PngExtension));

        #endregion
    }
}
