using System;
using System.Collections.Generic;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.TileMapResource;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources
{
    [TestFixture]
    public sealed class TileSetsTests
    {
        #region SetUp and consts

        private const string GeodeticProfile = "geodetic";

        private const string MercatorProfile = "mercator";

        private HashSet<TileSet> TileSetCollection { get; set; }

        [SetUp]
        public void SetUp()
        {
            const string href1 = "10";
            const double units1 = 1.0;
            const int order1 = 10;

            const string href2 = "11";
            const double units2 = 1.1;
            const int order2 = 11;

            TileSet ts1 = new(href1, units1, order1);
            TileSet ts2 = new(href2, units2, order2);

            TileSetCollection = new HashSet<TileSet> { ts1, ts2 };
        }

        #endregion

        #region Constructors

        [Test]
        public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new TileSets());

        [Test]
        public void TileSetsNormal1()
        {
            TileSets tileSets = null;

            Assert.DoesNotThrow(() => tileSets = new(TileSetCollection, GeodeticProfile));

            Assert.True(tileSets.TileSetCollection == TileSetCollection &&
                        tileSets.Profile.Equals(GeodeticProfile, StringComparison.Ordinal));
        }

        [Test]
        public void TileSets4326()
        {
            TileSets tileSets = null;

            Assert.DoesNotThrow(() => tileSets = new(TileSetCollection, CoordinateSystem.Epsg4326));

            Assert.True(tileSets.TileSetCollection == TileSetCollection &&
                        tileSets.Profile.Equals(GeodeticProfile, StringComparison.Ordinal));
        }

        [Test]
        public void TileSets3857()
        {
            TileSets tileSets = null;

            Assert.DoesNotThrow(() => tileSets = new(TileSetCollection, CoordinateSystem.Epsg3857));

            Assert.True(tileSets.TileSetCollection == TileSetCollection &&
                        tileSets.Profile.Equals(MercatorProfile, StringComparison.Ordinal));
        }

        [Test]
        public void TileSetsBadCs() => Assert.Throws<NotSupportedException>(() => _ = new TileSets(TileSetCollection, CoordinateSystem.Other));

        #endregion

        #region Methods

        [Test]
        public void GenerateCollection()
        {
            const int minZ = 0;
            const int maxZ = 18;
            Size tileSize = new(256, 256);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.DoesNotThrow(() => _ = TileSets.GenerateTileSetCollection(minZ, maxZ, tileSize, cs));
        }

        #endregion
    }
}
