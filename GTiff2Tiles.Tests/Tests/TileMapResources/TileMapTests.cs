using System;
using System.Collections.Generic;
using System.Linq;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.TileMapResource;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources
{
    [TestFixture]
    public sealed class TileMapTests
    {
        #region SetUp and consts

        private const string Version = "1.0.1";

        private const string TmsLink = "http://tms.osgeo.org/1.0.1";

        private const string Srs = "EPSG:4326";

        private BoundingBox _boundingBox;

        private Origin _origin;

        private TileFormat _tileFormat;

        private HashSet<TileSet> _tileSetCollection;

        private TileSets _tileSets;

        private readonly ICoordinate _tokyoGeodeticMin = Locations.TokyoGeodeticMin;

        private readonly ICoordinate _tokyoGeodeticMax = Locations.TokyoGeodeticMax;

        private readonly ICoordinate _originCoordinate = new GeodeticCoordinate(-180.0, -90.0);

        private readonly Size _tileSize = new(256, 256);

        private const TileExtension Extension = TileExtension.Png;

        private const int MinZ = 0;

        private const int MaxZ = 18;

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        [SetUp]
        public void SetUp()
        {
            _boundingBox = new(_tokyoGeodeticMin, _tokyoGeodeticMax);
            _origin = new(_originCoordinate);
            _tileFormat = new(_tileSize, Extension);

            _tileSetCollection = TileSets.GenerateTileSetCollection(MinZ, MaxZ, _tileSize, Cs4326).ToHashSet();
            _tileSets = new TileSets(_tileSetCollection, Cs4326);

            NetVipsHelper.DisableLog();
        }

        #endregion

        #region Constructors

        [Test]
        public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new TileMap());

        [Test]
        public void TileMapNormal()
        {
            TileMap tileMap = null;

            Assert.DoesNotThrow(() => tileMap = new TileMap(Srs, _boundingBox, _origin, _tileFormat, _tileSets, Version,
                                                            TmsLink));

            Assert.True(string.Equals(tileMap.Version, Version, StringComparison.Ordinal));
            Assert.True(string.Equals(tileMap.TileMapServiceLink, TmsLink, StringComparison.Ordinal));
            Assert.True(string.Equals(tileMap.Srs, Srs, StringComparison.Ordinal));
            Assert.True(tileMap.BoundingBox == _boundingBox);
            Assert.True(tileMap.Origin == _origin);
            Assert.True(tileMap.TileFormat == _tileFormat);
            Assert.True(tileMap.TileSets == _tileSets);
        }

        [Test]
        public void TileMapNormal2() =>
            Assert.DoesNotThrow(() => _ = new TileMap(_tokyoGeodeticMin, _tokyoGeodeticMax, _tileSize, Extension,
                                                      _tileSetCollection, Cs4326, Version, TmsLink, _originCoordinate));

        #endregion

        #region Methods



        #endregion
    }
}
