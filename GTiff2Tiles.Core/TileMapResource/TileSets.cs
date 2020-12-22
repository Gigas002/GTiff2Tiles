using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;

namespace GTiff2Tiles.Core.TileMapResource
{
    /// <summary>
    /// TileMap's TileSets
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "TileSets")]
    public record TileSets
    {
        #region Properties

        /// <summary>
        /// Collection of <see cref="TileSet"/>s
        /// </summary>
        [XmlElement(ElementName = "TileSet")]
        public HashSet<TileSet> TileSetCollection { get; }

        /// <summary>
        /// Tile's profile
        /// </summary>
        [XmlAttribute(AttributeName = "profile")]
        public string Profile { get; init; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TileSets() => TileSetCollection = new HashSet<TileSet>();

        /// <summary>
        /// Initialize a new tile sets
        /// </summary>
        /// <param name="tileSetCollection">Collection of <see cref="TileSet"/>s</param>
        /// <param name="profile">Tile's profle</param>
        public TileSets(IEnumerable<TileSet> tileSetCollection, string profile) => (TileSetCollection, Profile) = (tileSetCollection.ToHashSet(), profile);

        /// <summary>
        /// Initialize a new tile sets
        /// </summary>
        /// <param name="tileSetCollection">Collection of <see cref="TileSet"/>s</param>
        /// <param name="coordinateSystem">Tile's <see cref="CoordinateSystem"/></param>
        /// <exception cref="NotSupportedException"/>
        public TileSets(IEnumerable<TileSet> tileSetCollection, CoordinateSystem coordinateSystem)
        {
            TileSetCollection = tileSetCollection.ToHashSet();

            Profile = coordinateSystem switch
            {
                CoordinateSystem.Epsg4326 => "geodetic",
                CoordinateSystem.Epsg3857 => "mercator",
                _ => throw new NotSupportedException(string.Format(Strings.Culture, Strings.NotSupported,
                                                                   coordinateSystem))
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates collection of <see cref="TileSet"/>s
        /// </summary>
        /// <param name="minZ">Minimal zoom in collection</param>
        /// <param name="maxZ">Maximal zoom in collection</param>
        /// <param name="tileSize">Tile's size</param>
        /// <param name="coordinateSystem">Tile's coordinate system</param>
        /// <returns>Collection of <see cref="TileSet"/>s</returns>
        public static IEnumerable<TileSet> GenerateTileSetCollection(int minZ, int maxZ, Size tileSize, CoordinateSystem coordinateSystem)
        {
            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                double resoultion = GeoCoordinate.Resolution(zoom, tileSize, coordinateSystem);

                yield return new($"{zoom}", resoultion, zoom);
            }
        }

        #endregion
    }
}
