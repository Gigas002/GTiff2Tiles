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

        /// <param name="bandsCount"><see cref="BandsCount"/></param>
        /// <inheritdoc cref="Tile(Number,Size,IEnumerable{byte},string,bool,CoordinateType)"/>
        /// <param name="number"></param>
        /// <param name="size"></param>
        /// <param name="bytes"></param>
        /// <param name="extension"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="coordinateType"></param>
        public RasterTile(Number number, Size size = null, IEnumerable<byte> bytes = null,
                          string extension = FileExtensions.Png, bool tmsCompatible = false,
                          CoordinateType coordinateType = CoordinateType.Mercator,
                          int bandsCount = DefaultBandsCount)
            : base(number, size, bytes, extension, tmsCompatible, coordinateType) => BandsCount = bandsCount;

        /// <param name="bandsCount"><see cref="BandsCount"/></param>
        /// <inheritdoc cref="Tile(GeoCoordinate,GeoCoordinate,int,Size,IEnumerable{byte},string,bool)"/>
        /// <param name="minCoordinate"></param>
        /// <param name="maxCoordinate"></param>
        /// <param name="zoom"></param>
        /// <param name="size"></param>
        /// <param name="bytes"></param>
        /// <param name="extension"></param>
        /// <param name="tmsCompatible"></param>
        public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom,
                          Size size = null, IEnumerable<byte> bytes = null,
                          string extension = FileExtensions.Png,
                          bool tmsCompatible = false, int bandsCount = DefaultBandsCount)
            : base(minCoordinate, maxCoordinate, zoom, size, bytes, extension, tmsCompatible) => BandsCount = bandsCount;

        #endregion
    }
}
