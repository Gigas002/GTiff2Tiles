using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Images;

[TestFixture]
public sealed class AreaTests
{
    #region SetUp and consts

    private readonly string _in4326 = FileSystemEntries.Input4326FilePath;

    private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

    private readonly Size _in4326Size = new(4473, 3511);

    #endregion

    #region Constructors

    [Test]
    public void CreateAreaNormal()
    {
        PixelCoordinate coord = new(0.0, 0.0);

        Assert.DoesNotThrow(() =>
        {
            Area area = new(coord, Tile.DefaultSize);
        });
    }

    [Test]
    public void CreateAreaNullCoordinate() => Assert.Throws<ArgumentNullException>(() =>
    {
        Area area = new(null, Tile.DefaultSize);
    });

    [Test]
    public void CreateAreaNullSize()
    {
        PixelCoordinate coord = new(0.0, 0.0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Area area = new(coord, null);
        });
    }

    #endregion

    #region Properties

    [Test]
    public void GetProperties()
    {
        PixelCoordinate coord = new(0.0, 0.0);

        Area area = new(coord, Tile.DefaultSize);

        Assert.DoesNotThrow(() =>
        {
            Size size = area.Size;
            PixelCoordinate ori = area.OriginCoordinate;
        });
    }

    #endregion

    #region Methods

    #region GetAreas (coords overload)

    [Test]
    public void GetAreasCoordsNormal()
    {
        GeodeticCoordinate minImgCoord = new(139.74999904632568, 35.61293363571167);
        GeodeticCoordinate maxImgCoord = new(139.8459792137146, 35.688271522521973);

        GeodeticCoordinate minTileCoord = new(139.74609375, 35.68359375);
        GeodeticCoordinate maxTileCoord = new(139.921875, 35.859375);
        Size tileSize = Tile.DefaultSize;

        PixelCoordinate expectedReadCoord = new(0.0, 0.0);
        PixelCoordinate expectedWriteCoord = new(5.6875, 249.1875);
        Size expectedReadSize = new(4473, 218);
        Size expectedWriteSize = new(140, 7);

        Area calcReadArea = null;
        Area calcWriteArea = null;

        Assert.DoesNotThrow(() => (calcReadArea, calcWriteArea) = Area.GetAreas(minImgCoord, maxImgCoord, _in4326Size, minTileCoord, maxTileCoord, tileSize));
        Assert.True(calcReadArea.OriginCoordinate == expectedReadCoord && calcReadArea.Size == expectedReadSize);
        Assert.True(calcWriteArea.OriginCoordinate == expectedWriteCoord && calcWriteArea.Size == expectedWriteSize);
    }

    [Test]
    public void GetAreasCoordsNullMinImgCoord()
    {
        GeodeticCoordinate maxImgCoord = new(1.0, 1.0);
        Size imgSize = new(10, 10);

        GeodeticCoordinate minTileCoord = new(0.0, 0.0);
        GeodeticCoordinate maxTileCoord = new(0.0, 0.0);
        Size tileSize = Tile.DefaultSize;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(null, maxImgCoord, imgSize, minTileCoord, maxTileCoord, tileSize);
        });
    }

    [Test]
    public void GetAreasCoordsNullMaxImgCoord()
    {
        GeodeticCoordinate minImgCoord = new(0.0, 0.0);
        Size imgSize = new(10, 10);

        GeodeticCoordinate minTileCoord = new(0.0, 0.0);
        GeodeticCoordinate maxTileCoord = new(1.0, 1.0);
        Size tileSize = Tile.DefaultSize;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(minImgCoord, null, imgSize, minTileCoord, maxTileCoord, tileSize);
        });
    }

    [Test]
    public void GetAreasCoordsImgMinEqualsMax()
    {
        GeodeticCoordinate minImgCoord = new(0.0, 0.0);
        GeodeticCoordinate maxImgCoord = minImgCoord;
        Size imgSize = new(10, 10);

        GeodeticCoordinate minTileCoord = new(0.0, 0.0);
        GeodeticCoordinate maxTileCoord = new(1.0, 1.0);
        Size tileSize = Tile.DefaultSize;

        Assert.Throws<ArgumentException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(minImgCoord, maxImgCoord, imgSize, minTileCoord, maxTileCoord, tileSize);
        });
    }

    [Test]
    public void GetAreasCoordsNullImgSize()
    {
        GeodeticCoordinate minImgCoord = new(0.0, 0.0);
        GeodeticCoordinate maxImgCoord = new(1.0, 1.0);

        GeodeticCoordinate minTileCoord = new(0.0, 0.0);
        GeodeticCoordinate maxTileCoord = new(0.0, 0.0);
        Size tileSize = Tile.DefaultSize;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(minImgCoord, maxImgCoord, null, minTileCoord, maxTileCoord, tileSize);
        });
    }

    [Test]
    public void GetAreasCoordsNullMinTileCoord()
    {
        GeodeticCoordinate minImgCoord = new(0.0, 0.0);
        GeodeticCoordinate maxImgCoord = new(1.0, 1.0);
        Size imgSize = new(10, 10);

        GeodeticCoordinate maxTileCoord = new(1.0, 1.0);
        Size tileSize = Tile.DefaultSize;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(minImgCoord, maxImgCoord, imgSize, null, maxTileCoord, tileSize);
        });
    }

    [Test]
    public void GetAreasCoordsNullMaxTileCoord()
    {
        GeodeticCoordinate minImgCoord = new(0.0, 0.0);
        GeodeticCoordinate maxImgCoord = new(1.0, 1.0);
        Size imgSize = new(10, 10);

        GeodeticCoordinate minTileCoord = new(0.0, 0.0);
        Size tileSize = Tile.DefaultSize;

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(minImgCoord, maxImgCoord, imgSize, minTileCoord, null, tileSize);
        });
    }

    [Test]
    public void GetAreasCoordsTileMinEqualsMax()
    {
        GeodeticCoordinate minImgCoord = new(0.0, 0.0);
        GeodeticCoordinate maxImgCoord = new(1.0, 1.0);
        Size imgSize = new(10, 10);

        GeodeticCoordinate minTileCoord = new(0.0, 0.0);
        GeodeticCoordinate maxTileCoord = minTileCoord;
        Size tileSize = Tile.DefaultSize;

        Assert.Throws<ArgumentException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(minImgCoord, maxImgCoord, imgSize, minTileCoord, maxTileCoord, tileSize);
        });
    }

    [Test]
    public void GetAreasCoordsNullTileSize()
    {
        GeodeticCoordinate minImgCoord = new(0.0, 0.0);
        GeodeticCoordinate maxImgCoord = new(1.0, 1.0);
        Size imgSize = new(10, 10);

        GeodeticCoordinate minTileCoord = new(0.0, 0.0);
        GeodeticCoordinate maxTileCoord = new(1.0, 1.0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            // ReSharper disable once ConvertToLambdaExpressionWhenPossible
            (Area readArea, Area writeArea) = Area.GetAreas(minImgCoord, maxImgCoord, imgSize, minTileCoord, maxTileCoord, null);
        });
    }

    #endregion

    #region GetAreas (IGeoTiff overload)

    [Test]
    public void GetAreasGeoTiffNormal()
    {
        using IGeoTiff image = new Raster(_in4326, Cs4326);

        using ITile tile = new RasterTile(Locations.TokyoGeodeticTmsNumber,
                                          image.GeoCoordinateSystem, tmsCompatible: true);

        PixelCoordinate expectedReadCoord = new(0.0, 218.0);
        PixelCoordinate expectedWriteCoord = new(5.6875, 0.0);
        Size expectedReadSize = new(4473, 3293);
        Size expectedWriteSize = new(140, 102);

        Area calcReadArea = null;
        Area calcWriteArea = null;

        Assert.DoesNotThrow(() => (calcReadArea, calcWriteArea) = Area.GetAreas(image, tile));
        Assert.True(calcReadArea.OriginCoordinate == expectedReadCoord && calcReadArea.Size == expectedReadSize);
        Assert.True(calcWriteArea.OriginCoordinate == expectedWriteCoord && calcWriteArea.Size == expectedWriteSize);
    }

    [Test]
    public void GetAreasGeoTiffNullImage()
    {
        Number number = new(0, 0, 0);
        using ITile tile = new RasterTile(number, Cs4326, tmsCompatible: true);

        Assert.Throws<ArgumentNullException>(() => Area.GetAreas(null, tile));
    }

    [Test]
    public void GetAreasGeoTiffNullTile()
    {
        using IGeoTiff image = new Raster(_in4326, Cs4326);

        Assert.Throws<ArgumentNullException>(() => Area.GetAreas(image, null));
    }

    #endregion

    #endregion
}