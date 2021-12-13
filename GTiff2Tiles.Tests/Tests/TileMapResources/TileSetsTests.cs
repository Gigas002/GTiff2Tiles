using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.TileMapResource;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources;

[TestFixture]
public sealed class TileSetsTests
{
    #region SetUp and consts

    private const string GeodeticProfile = "geodetic";

    private const string MercatorProfile = "mercator";

    private const int MinZ = 0;

    private const int MaxZ = 18;

    private const double Units10 = 1.0;

    private const double Units11 = 1.1;

    private HashSet<TileSet> _tileSetCollection;

    private readonly Size _tileSize = new(256, 256);

    private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

    private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

    private const CoordinateSystem CsOther = CoordinateSystem.Other;

    [SetUp]
    public void SetUp()
    {
        string href1 = $"{MinZ}";
        string href2 = $"{MaxZ}";

        TileSet ts1 = new(href1, Units10, MinZ);
        TileSet ts2 = new(href2, Units11, MaxZ);

        _tileSetCollection = new HashSet<TileSet> { ts1, ts2 };
    }

    #endregion

    #region Constructors

    [Test]
    public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new TileSets());

    [Test]
    public void TileSetsNormal1()
    {
        TileSets tileSets = null;

        Assert.DoesNotThrow(() => tileSets = new(_tileSetCollection, GeodeticProfile));

        foreach (TileSet baseTs in _tileSetCollection) Assert.True(tileSets.TileSetCollection.Contains(baseTs));

        Assert.True(tileSets.Profile.Equals(GeodeticProfile, StringComparison.Ordinal));
    }

    [Test]
    public void TileSets4326()
    {
        TileSets tileSets = null;

        Assert.DoesNotThrow(() => tileSets = new(_tileSetCollection, Cs4326));

        foreach (TileSet baseTs in _tileSetCollection) Assert.True(tileSets.TileSetCollection.Contains(baseTs));

        Assert.True(tileSets.Profile.Equals(GeodeticProfile, StringComparison.Ordinal));
    }

    [Test]
    public void TileSets3857()
    {
        TileSets tileSets = null;

        Assert.DoesNotThrow(() => tileSets = new(_tileSetCollection, Cs3857));

        foreach (TileSet baseTs in _tileSetCollection) Assert.True(tileSets.TileSetCollection.Contains(baseTs));

        Assert.True(tileSets.Profile.Equals(MercatorProfile, StringComparison.Ordinal));
    }

    [Test]
    public void TileSetsBadCs() => Assert.Throws<NotSupportedException>(() => _ = new TileSets(_tileSetCollection, CsOther));

    #endregion

    #region Methods

    [Test]
    public void GenerateCollection() =>
        Assert.DoesNotThrow(() => _ = TileSets.GenerateTileSetCollection(MinZ, MaxZ, _tileSize, Cs4326));

    #endregion
}