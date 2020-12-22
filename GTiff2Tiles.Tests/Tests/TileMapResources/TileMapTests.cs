using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
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

        private string _timestamp;

        private const string Version = "1.0.1";

        private const string TmsLink = "http://tms.osgeo.org/1.0.1";

        private const string Srs = "EPSG:4326";

        private BoundingBox _boundingBox;

        private Origin _origin;

        private TileFormat _tileFormat;

        private IEnumerable<TileSet> _tileSetCollection;

        private TileSets _tileSets;

        private readonly ICoordinate _tokyoGeodeticMin = Locations.TokyoGeodeticMin;

        private readonly ICoordinate _tokyoGeodeticMax = Locations.TokyoGeodeticMax;

        private readonly ICoordinate _originCoordinate = new GeodeticCoordinate(-180.0, -90.0);

        private readonly Size _tileSize = new(256, 256);

        private const TileExtension Extension = TileExtension.Png;

        private const int MinZ = 0;

        private const int MaxZ = 18;

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        private const string XmlName = "tilemapresource.xml";

        private readonly string _inXmlPath = FileSystemEntries.TileMapResourceXmlPath;

        private string _outXmlPath;

        [SetUp]
        public void SetUp()
        {
            _timestamp = DateTime.Now.ToString(Core.Constants.DateTimePatterns.LongWithMs,
                                               CultureInfo.InvariantCulture);

            _boundingBox = new(_tokyoGeodeticMin, _tokyoGeodeticMax);
            _origin = new(_originCoordinate);
            _tileFormat = new(_tileSize, Extension);

            _tileSetCollection = TileSets.GenerateTileSetCollection(MinZ, MaxZ, _tileSize, Cs4326);
            _tileSets = new TileSets(_tileSetCollection, Cs4326);

            _outXmlPath = Path.Combine(FileSystemEntries.OutputDirectoryPath, $"{_timestamp}_{XmlName}");

            FileSystemEntries.OutputDirectoryInfo.Create();
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

            Assert.True(tileMap.Version.Equals(Version, StringComparison.Ordinal));
            Assert.True(tileMap.TileMapServiceLink.Equals(TmsLink, StringComparison.Ordinal));
            Assert.True(tileMap.Srs.Equals(Srs, StringComparison.Ordinal));
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

        #region GetSrs

        [Test]
        public void GetSrs4326() => Assert.True(TileMap.GetSrs(Cs4326).Equals(Srs, StringComparison.Ordinal));

        [Test]
        public void GetSrs3857() => Assert.True(TileMap.GetSrs(CoordinateSystem.Epsg3857).Equals("EPSG:3857", StringComparison.Ordinal));

        [Test]
        public void GetSrsOther() => Assert.True(string.IsNullOrWhiteSpace(TileMap.GetSrs(CoordinateSystem.Other)));

        #endregion

        #region Serialize

        [Test]
        public void SerializeNormal()
        {
            TileMap tileMap = new(_tokyoGeodeticMin, _tokyoGeodeticMax, _tileSize, Extension, _tileSetCollection,
                                  Cs4326, Version, TmsLink, _originCoordinate);

            using FileStream fileStream = File.OpenWrite(_outXmlPath);

            Assert.DoesNotThrow(() => tileMap.Serialize(fileStream));
        }

        [Test]
        public void SerializeNullStream()
        {
            TileMap tileMap = new(_tokyoGeodeticMin, _tokyoGeodeticMax, _tileSize, Extension, _tileSetCollection,
                                  Cs4326, Version, TmsLink, _originCoordinate);

            Assert.Throws<ArgumentNullException>(() => tileMap.Serialize(null));
        }

        #endregion

        #region Deserialize

        [Test]
        public void DeserializeNormal()
        {
            using FileStream fileStream = File.OpenRead(_inXmlPath);

            TileMap tileMap = TileMap.Deserialize(fileStream);

            Assert.True(tileMap.Version.Equals(Version, StringComparison.Ordinal));
            Assert.True(tileMap.TileMapServiceLink.Equals(TmsLink, StringComparison.Ordinal));
            Assert.True(tileMap.Srs.Equals(Srs, StringComparison.Ordinal));
            Assert.True(tileMap.BoundingBox == _boundingBox);
            Assert.True(tileMap.Origin == _origin);
            Assert.True(tileMap.TileFormat == _tileFormat);

            foreach (TileSet baseTs in _tileSetCollection) Assert.True(tileMap.TileSets.TileSetCollection.Contains(baseTs));
        }

        [Test]
        public void DeserializeNullStream() =>
            Assert.Throws<ArgumentNullException>(() => _ = TileMap.Deserialize(null));

        #endregion

        #endregion
    }
}
