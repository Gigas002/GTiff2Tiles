using System.Collections.Generic;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Images;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// <see cref="Raster"/> <see cref="Tile"/>
    /// </summary>
    public class RasterTile : Tile
    {
        #region Properties/Constants

        /// <summary>
        /// Default count of bands
        /// </summary>
        public const int DefaultBandsCount = 4;

        /// <summary>
        /// Count of bands in <see cref="RasterTile"/>
        /// </summary>
        public int BandsCount { get; }

        #endregion

        #region Constructors

        /// <param name="bandsCount"><see cref="BandsCount"/></param>
        /// <inheritdoc cref="Tile(Number,CoordinateSystem,Size,IEnumerable{byte},TileExtension,bool)"/>
        /// <param name="number"></param>
        /// <param name="coordinateSystem"></param>
        /// <param name="size"></param>
        /// <param name="bytes"></param>
        /// <param name="extension"></param>
        /// <param name="tmsCompatible"></param>
        public RasterTile(Number number, CoordinateSystem coordinateSystem,
                          Size size = null, IEnumerable<byte> bytes = null,
                          TileExtension extension = TileExtension.Png, bool tmsCompatible = false,
                          int bandsCount = DefaultBandsCount)
            : base(number, coordinateSystem, size, bytes, extension, tmsCompatible) => BandsCount = bandsCount;

        /// <param name="bandsCount"><see cref="BandsCount"/></param>
        /// <inheritdoc cref="Tile(GeoCoordinate,GeoCoordinate,int,Size,IEnumerable{byte},TileExtension,bool)"/>
        /// <param name="minCoordinate"></param>
        /// <param name="maxCoordinate"></param>
        /// <param name="zoom"></param>
        /// <param name="size"></param>
        /// <param name="bytes"></param>
        /// <param name="extension"></param>
        /// <param name="tmsCompatible"></param>
        public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom,
                          Size size = null, IEnumerable<byte> bytes = null,
                          TileExtension extension = TileExtension.Png,
                          bool tmsCompatible = false, int bandsCount = DefaultBandsCount)
            : base(minCoordinate, maxCoordinate, zoom, size, bytes, extension, tmsCompatible) => BandsCount = bandsCount;

        #endregion
    }
}
