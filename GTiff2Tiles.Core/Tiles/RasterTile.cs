using System.Collections.Generic;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
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

        /// <summary>
        /// Creates new <see cref="RasterTile"/>
        /// </summary>
        /// <param name="number"><see cref="Tile.Number"/></param>
        /// <param name="size"><see cref="Tile.Size"/>;
        /// <remarks>should be a square, e.g. 256x256</remarks></param>
        /// <param name="bytes"><see cref="Tile.Bytes"/></param>
        /// <param name="extension"><see cref="Tile.Extension"/></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="coordinateType">Type of <see cref="GeoCoordinate"/></param>
        /// <param name="bandsCount"><see cref="BandsCount"/></param>
        public RasterTile(Number number, Size size = null, IEnumerable<byte> bytes = null,
                          string extension = FileExtensions.Png, bool tmsCompatible = false,
                          CoordinateType coordinateType = CoordinateType.Mercator,
                          int bandsCount = DefaultBandsCount)
            : base(number, size, bytes, extension, tmsCompatible, coordinateType) => BandsCount = bandsCount;

        /// <summary>
        /// Creates new <see cref="RasterTile"/> from <see cref="GeoCoordinate"/> values
        /// </summary>
        /// <param name="minCoordinate">Minimum <see cref="GeoCoordinate"/></param>
        /// <param name="maxCoordinate">Maximum <see cref="GeoCoordinate"/></param>
        /// <param name="zoom">Zoom</param>
        /// <param name="size"><see cref="Tile.Size"/>;
        /// <remarks>should be a square, e.g. 256x256</remarks></param>
        /// <param name="bytes"><see cref="Tile.Bytes"/></param>
        /// <param name="extension"><see cref="Tile.Extension"/></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="bandsCount"><see cref="BandsCount"/></param>
        public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom,
                          Size size = null, IEnumerable<byte> bytes = null,
                          string extension = FileExtensions.Png,
                          bool tmsCompatible = false, int bandsCount = DefaultBandsCount)
            : base(minCoordinate, maxCoordinate, zoom, size, bytes, extension, tmsCompatible) => BandsCount = bandsCount;

        #endregion
    }
}
