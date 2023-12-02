using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

// ReSharper disable NotAccessedVariable
// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates;

[TestFixture]
public sealed class PixelCoordinateTests
{
    #region Consts

    private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

    private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

    private const CoordinateSystem CsOther = CoordinateSystem.Other;

    #endregion

    #region Constructors

    [Test]
    public void CreatePixelCoordinateNormal() => Assert.DoesNotThrow(() =>
    {
        PixelCoordinate coord =
            new(Locations.TokyoMercatorPixelLongitude, Locations.TokyoMercatorPixelLatitude);
    });

    [Test]
    public void CreatePixelCoordinateSmallLon() => Assert.Throws<ArgumentOutOfRangeException>(() =>
    {
        PixelCoordinate coord = new(-1.0, Locations.TokyoMercatorPixelLatitude);
    });

    [Test]
    public void CreatePixelCoordinateSmallLat() => Assert.Throws<ArgumentOutOfRangeException>(() =>
    {
        PixelCoordinate coord = new(Locations.TokyoMercatorPixelLongitude, -1.0);
    });

    #endregion

    #region Methods

    #region ToNumber

    [Test]
    public void ToNumberNormal()
    {
        Number res = null;

        Assert.DoesNotThrow(() => res = Locations.TokyoGeodeticPixelCoordinate
                                                 .ToNumber(10, Tile.DefaultSize, false));
        Assert.That(res, Is.EqualTo(Locations.TokyoGeodeticNtmsNumber));
    }

    [Test]
    public void ToNumberSmallZ() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Locations.TokyoGeodeticPixelCoordinate
                                                                  .ToNumber(-1, Tile.DefaultSize, false));

    [Test]
    public void ToNumberNullTileSize() =>
        Assert.Throws<ArgumentNullException>(() => Locations.TokyoGeodeticPixelCoordinate
                                                            .ToNumber(10, null, false));

    [Test]
    public void ToNumberNotSqureTileSize()
    {
        Size size = new(10, 20);

        Assert.Throws<ArgumentException>(() => Locations.TokyoGeodeticPixelCoordinate
                                                        .ToNumber(10, size, false));
    }

    #endregion

    #region ToGeoCoordinate

    [Test]
    public void ToGeoCoordinateFromGeodeticPixels()
    {
        Coordinate geodeticCoordinate = null;
        Coordinate mercatorCoordinate = null;

        Assert.DoesNotThrow(() => geodeticCoordinate = Locations.TokyoGeodeticPixelCoordinate
                                                                .ToGeoCoordinate(Cs4326, Cs4326, 10, Tile.DefaultSize).Round<GeodeticCoordinate>(6));
        Assert.DoesNotThrow(() => mercatorCoordinate = Locations.TokyoGeodeticPixelCoordinate
                                                                .ToGeoCoordinate(Cs4326, Cs3857, 10, Tile.DefaultSize).Round<MercatorCoordinate>(2));


        Assert.That(geodeticCoordinate, Is.EqualTo(Locations.TokyoGeodeticCoordinate));
        Assert.That(mercatorCoordinate, Is.EqualTo(Locations.TokyoMercatorCoordinate));

        Assert.Throws<NotSupportedException>(() => Locations.TokyoGeodeticPixelCoordinate
                                                            .ToGeoCoordinate(Cs4326, CsOther, 10, Tile.DefaultSize));
    }

    [Test]
    public void ToGeoCoordinateFromMercatorPixels()
    {
        Coordinate geodeticCoordinate = null;
        Coordinate mercatorCoordinate = null;

        Assert.DoesNotThrow(() => geodeticCoordinate = Coordinate.Round((GeodeticCoordinate)Locations.TokyoMercatorPixelCoordinate
                                                                           .ToGeoCoordinate(Cs3857, Cs4326, 10, Tile.DefaultSize), 6));
        Assert.DoesNotThrow(() => mercatorCoordinate = Coordinate.Round((MercatorCoordinate)Locations.TokyoMercatorPixelCoordinate
                                                                           .ToGeoCoordinate(Cs3857, Cs3857, 10, Tile.DefaultSize), 2));

        Assert.That(geodeticCoordinate, Is.EqualTo(Locations.TokyoGeodeticCoordinate));
        Assert.That(mercatorCoordinate, Is.EqualTo(Locations.TokyoMercatorCoordinate));

        Assert.Throws<NotSupportedException>(() => Locations.TokyoMercatorPixelCoordinate
                                                            .ToGeoCoordinate(Cs3857, CsOther, 10, Tile.DefaultSize));
    }

    #endregion

    #region ToGeodeticCoordinate

    [Test]
    public void ToGeodeticCoordinateNormal()
    {
        Coordinate res = null;

        Assert.DoesNotThrow(() => res = Coordinate.Round(Locations.TokyoGeodeticPixelCoordinate
                                                                  .ToGeodeticCoordinate(Cs4326, 10, Tile.DefaultSize), 6));
        Assert.That(res, Is.EqualTo(Locations.TokyoGeodeticCoordinate));
    }

    [Test]
    public void ToGeodeticCoordinateOtherCs() => Assert.Throws<NotSupportedException>(() => Locations.TokyoGeodeticPixelCoordinate
       .ToGeodeticCoordinate(CsOther, 10, Tile.DefaultSize));

    [Test]
    public void ToGeodeticCoordinateSmallZ() => Assert.Throws<ArgumentOutOfRangeException>(() => Locations.TokyoGeodeticPixelCoordinate
       .ToGeodeticCoordinate(Cs4326, -1, Tile.DefaultSize));

    #endregion

    #region ToMercatorCoordinate

    [Test]
    public void ToMercatorCoordinateNormal()
    {
        Coordinate res = null;

        Assert.DoesNotThrow(() => res = Coordinate.Round(Locations.TokyoMercatorPixelCoordinate
                                                                  .ToMercatorCoordinate(Cs3857, 10, Tile.DefaultSize), 2));
        Assert.That(res, Is.EqualTo(Locations.TokyoMercatorCoordinate));
    }

    [Test]
    public void ToMercatorCoordinateOtherCs() => Assert.Throws<NotSupportedException>(() => Locations.TokyoMercatorPixelCoordinate
       .ToMercatorCoordinate(CsOther, 10, Tile.DefaultSize));

    [Test]
    public void ToMercatorCoordinateSmallZ() => Assert.Throws<ArgumentOutOfRangeException>(() => Locations.TokyoMercatorPixelCoordinate
       .ToMercatorCoordinate(Cs3857, -1, Tile.DefaultSize));

    #endregion

    #region ToRasterPixelCoordinate

    [Test]
    public void ToRasterPixelCoordinateNormal() => Assert.DoesNotThrow(() =>
    {
        PixelCoordinate pCoord = Locations.TokyoMercatorPixelCoordinate
                                          .ToRasterPixelCoordinate(10, Tile.DefaultSize);
    });

    [Test]
    public void ToRasterPixelCoordinateSmallZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
    {
        PixelCoordinate pCoord = Locations.TokyoMercatorPixelCoordinate
                                          .ToRasterPixelCoordinate(-1, Tile.DefaultSize);
    });

    [Test]
    public void ToRasterPixelCoordinateNullTileSize() => Assert.Throws<ArgumentNullException>(() =>
    {
        PixelCoordinate pCoord = Locations.TokyoMercatorPixelCoordinate
                                          .ToRasterPixelCoordinate(10, null);
    });

    [Test]
    public void ToRasterPixelCoordinateNotSquareTileSize()
    {
        Size size = new(10, 20);

        Assert.Throws<ArgumentException>(() =>
        {
            PixelCoordinate pCoord = Locations.TokyoMercatorPixelCoordinate
                                              .ToRasterPixelCoordinate(10, size);
        });
    }

    #endregion

    #endregion
}