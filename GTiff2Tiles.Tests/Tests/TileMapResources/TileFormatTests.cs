using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.TileMapResource;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources;

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

        Assert.Multiple(() =>
        {
            Assert.That(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height, Is.True);
            Assert.That(tileFormat.MimeType.Equals(PngMimeType, StringComparison.Ordinal), Is.True);
            Assert.That(tileFormat.Extension.Equals(PngExtensionString, StringComparison.Ordinal), Is.True);
        });
    }

    [Test]
    public void TileFormatPngNormal2()
    {
        TileFormat tileFormat = null;

        Assert.DoesNotThrow(() => tileFormat = new TileFormat(_tileSize, PngExtension));

        Assert.Multiple(() =>
        {
            Assert.That(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height, Is.True);
            Assert.That(tileFormat.MimeType.Equals(PngMimeType, StringComparison.Ordinal), Is.True);
            Assert.That(tileFormat.Extension.Equals(PngExtensionString, StringComparison.Ordinal), Is.True);
        });

    }

    [Test]
    public void TileFormatWebpNormal()
    {
        TileFormat tileFormat = null;

        Assert.DoesNotThrow(() => tileFormat = new TileFormat(_tileSize, WebpExtension));

        Assert.Multiple(() =>
        {
            Assert.That(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height, Is.True);
            Assert.That(tileFormat.MimeType.Equals(WebpMimeType, StringComparison.Ordinal), Is.True);
            Assert.That(tileFormat.Extension.Equals(WebpExtensionString, StringComparison.Ordinal), Is.True);
        });

    }

    [Test]
    public void TileFormatJpgNormal()
    {
        TileFormat tileFormat = null;

        Assert.DoesNotThrow(() => tileFormat = new TileFormat(_tileSize, JpgExtension));

        Assert.Multiple(() =>
        {
            Assert.That(tileFormat.Width == _tileSize.Width && tileFormat.Height == _tileSize.Height, Is.True);
            Assert.That(tileFormat.MimeType.Equals(JpegMimeType, StringComparison.Ordinal), Is.True);
            Assert.That(tileFormat.Extension.Equals(JpgExtensionString, StringComparison.Ordinal), Is.True);
        });

    }

    [Test]
    public void TileFormatBadSize() => Assert.Throws<ArgumentNullException>(() => _ = new TileFormat(null, PngExtension));

    #endregion
}