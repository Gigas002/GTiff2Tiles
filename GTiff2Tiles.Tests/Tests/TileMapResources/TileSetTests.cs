using GTiff2Tiles.Core.TileMapResource;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources;

[TestFixture]
public sealed class TileSetTests
{
    #region Consts

    private const string Href = "10";

    private const double Units = 1.0;

    private const int Order = 10;
        
    #endregion

    #region Constructors

    [Test]
    public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new TileSet());

    [Test]
    public void TileSetNormal()
    {
        TileSet tileSet = null;

        Assert.DoesNotThrow(() => tileSet = new(Href, Units, Order));

        Assert.That(tileSet.Href.Equals(Href, StringComparison.Ordinal) &&
                    Math.Abs(tileSet.UnitsPerPixel - Units) < double.Epsilon &&
                    tileSet.Order == Order, Is.True);
    }

    #endregion
}