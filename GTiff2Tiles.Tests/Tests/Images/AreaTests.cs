#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0219 // The variable is assigned but it's value is never used

using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Images
{
    [TestFixture]
    public sealed class AreaTests
    {
        #region Constructors

        [Test]
        public void CreateAreaNormal()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.DoesNotThrow(() =>
            {
                Area area = new Area(coord, Tile.DefaultSize);
            });
        }

        [Test]
        public void CreateAreaNullCoordinate() => Assert.Throws<ArgumentNullException>(() =>
        {
            Area area = new Area(null, Tile.DefaultSize);
        });

        [Test]
        public void CreateAreaNullSize()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Area area = new Area(coord, null);
            });
        }

        #endregion

        #region Properties

        [Test]
        public void GetProperties()
        {
            PixelCoordinate coord = new PixelCoordinate(0.0, 0.0);

            Area area = new Area(coord, Tile.DefaultSize);

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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(13.367990255355835, 52.501827478408813);
            GeodeticCoordinate maxImgCoord = new GeodeticCoordinate(13.438467979431152, 52.534797191619873);
            Size imgSize = new Size(6569, 3073);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(13.359375, 52.3828125);
            GeodeticCoordinate maxTileCoord = new GeodeticCoordinate(13.53515625, 52.55859375);
            Size tileSize = Tile.DefaultSize;

            PixelCoordinate expectedReadCoord = new PixelCoordinate(0.0, 0.0);
            Size expectedReadSize = new Size(6569, 3073);
            PixelCoordinate expectedWriteCoord = new PixelCoordinate(12.546875, 34.65625);
            Size expectedWriteSize = new Size(103, 48);

            Area calcReadArea = null;
            Area calcWriteArea = null;
            Assert.DoesNotThrow(() => (calcReadArea, calcWriteArea) = Area.GetAreas(minImgCoord, maxImgCoord, imgSize, minTileCoord, maxTileCoord, tileSize));

            Assert.True(calcReadArea.OriginCoordinate == expectedReadCoord && calcReadArea.Size == expectedReadSize);
            Assert.True(calcWriteArea.OriginCoordinate == expectedWriteCoord && calcWriteArea.Size == expectedWriteSize);
        }

        [Test]
        public void GetAreasCoordsNullMinImgCoord()
        {
            GeodeticCoordinate maxImgCoord = new GeodeticCoordinate(1.0, 1.0);
            Size imgSize = new Size(10, 10);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxTileCoord = new GeodeticCoordinate(0.0, 0.0);
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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(0.0, 0.0);
            Size imgSize = new Size(10, 10);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxTileCoord = new GeodeticCoordinate(1.0, 1.0);
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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxImgCoord = minImgCoord;
            Size imgSize = new Size(10, 10);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxTileCoord = new GeodeticCoordinate(1.0, 1.0);
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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxImgCoord = new GeodeticCoordinate(1.0, 1.0);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxTileCoord = new GeodeticCoordinate(0.0, 0.0);
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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxImgCoord = new GeodeticCoordinate(1.0, 1.0);
            Size imgSize = new Size(10, 10);

            GeodeticCoordinate maxTileCoord = new GeodeticCoordinate(1.0, 1.0);
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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxImgCoord = new GeodeticCoordinate(1.0, 1.0);
            Size imgSize = new Size(10, 10);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(0.0, 0.0);
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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxImgCoord = new GeodeticCoordinate(1.0, 1.0);
            Size imgSize = new Size(10, 10);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(0.0, 0.0);
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
            GeodeticCoordinate minImgCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxImgCoord = new GeodeticCoordinate(1.0, 1.0);
            Size imgSize = new Size(10, 10);

            GeodeticCoordinate minTileCoord = new GeodeticCoordinate(0.0, 0.0);
            GeodeticCoordinate maxTileCoord = new GeodeticCoordinate(1.0, 1.0);

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
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            IGeoTiff image = new Raster(inputPath, cs);

            Number number = new Number(1100, 810, 10);
            ITile tile = new RasterTile(number, cs, tmsCompatible: true);

            PixelCoordinate expectedReadCoord = new PixelCoordinate(0.0, 0.0);
            Size expectedReadSize = new Size(6569, 3073);
            PixelCoordinate expectedWriteCoord = new PixelCoordinate(12.546875, 34.65625);
            Size expectedWriteSize = new Size(103, 48);

            Area calcReadArea = null;
            Area calcWriteArea = null;
            Assert.DoesNotThrow(() => (calcReadArea, calcWriteArea) = Area.GetAreas(image, tile));

            Assert.True(calcReadArea.OriginCoordinate == expectedReadCoord && calcReadArea.Size == expectedReadSize);
            Assert.True(calcWriteArea.OriginCoordinate == expectedWriteCoord && calcWriteArea.Size == expectedWriteSize);
        }

        [Test]
        public void GetAreasGeoTiffNullImage()
        {
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            Number number = new Number(1100, 810, 10);
            ITile tile = new RasterTile(number, cs, tmsCompatible: true);

            Assert.Throws<ArgumentNullException>(() => Area.GetAreas(null, tile));
        }

        [Test]
        public void GetAreasGeoTiffNullTile()
        {
            string inputPath = Constants.FileSystemEntries.Input4326FilePath;
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;
            IGeoTiff image = new Raster(inputPath, cs);

            Assert.Throws<ArgumentNullException>(() => Area.GetAreas(image, null));
        }

        #endregion

        #endregion
    }
}

#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0219 // The variable is assigned but it's value is never used
