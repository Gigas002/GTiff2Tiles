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

        private const string PngExtension = "png";

        private const string WebpExtension = "webp";

        private const string JpgExtension = "jpg";

        private static readonly Size TileSize = new(256, 256);

        #endregion

        #region Constructors

        [Test]
        public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new TileFormat());

        [Test]
        public void TileFormatPngNormal()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(TileSize.Width, TileSize.Height, PngMimeType, PngExtension));

            Assert.True(tileFormat.Width == TileSize.Width && tileFormat.Height == TileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(PngMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(PngExtension, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatPngNormal2()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(TileSize, TileExtension.Png));

            Assert.True(tileFormat.Width == TileSize.Width && tileFormat.Height == TileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(PngMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(PngExtension, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatWebpNormal()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(TileSize, TileExtension.Webp));

            Assert.True(tileFormat.Width == TileSize.Width && tileFormat.Height == TileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(WebpMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(WebpExtension, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatJpgNormal()
        {
            TileFormat tileFormat = null;

            Assert.DoesNotThrow(() => tileFormat = new TileFormat(TileSize, TileExtension.Jpg));

            Assert.True(tileFormat.Width == TileSize.Width && tileFormat.Height == TileSize.Height);
            Assert.True(tileFormat.MimeType.Equals(JpegMimeType, StringComparison.Ordinal));
            Assert.True(tileFormat.Extension.Equals(JpgExtension, StringComparison.Ordinal));
        }

        [Test]
        public void TileFormatBadSize() => Assert.Throws<ArgumentNullException>(() => _ = new TileFormat(null, TileExtension.Png));

        #endregion
    }
}
