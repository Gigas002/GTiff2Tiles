using System;
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

        /// <summary>
        /// Interpolation of this <see cref="RasterTile"/>
        /// </summary>
        public Interpolation Interpolation { get; }

        #endregion

        #region Constructors

        /// <inheritdoc cref="Tile(Number,CoordinateSystem,Size,IEnumerable{byte},TileExtension,bool)"/>
        /// <param name="number"></param>
        /// <param name="coordinateSystem"></param>
        /// <param name="size"></param>
        /// <param name="bytes"></param>
        /// <param name="extension"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="bandsCount"><see cref="BandsCount"/>
        /// <remarks><para/>Must be in range (0, 4];
        /// <para/><see cref="DefaultBandsCount"/> by default</remarks></param>
        /// <param name="interpolation"><see cref="Interpolation"/>
        /// <remarks><para/>Lanczos3 by default</remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public RasterTile(Number number, CoordinateSystem coordinateSystem,
                          Size size = null, IEnumerable<byte> bytes = null,
                          TileExtension extension = TileExtension.Png, bool tmsCompatible = false,
                          int bandsCount = DefaultBandsCount,
                          Interpolation interpolation = Interpolation.Lanczos3)
            : base(number, coordinateSystem, size, bytes, extension, tmsCompatible)
        {
            #region Preconditions checks

            if (bandsCount <= 0 || bandsCount > 4) throw new ArgumentOutOfRangeException(nameof(bandsCount));

            #endregion

            (BandsCount, Interpolation) = (bandsCount, interpolation);
        }

        /// <inheritdoc cref="Tile(GeoCoordinate,GeoCoordinate,int,Size,IEnumerable{byte},TileExtension,bool)"/>
        /// <param name="minCoordinate"></param>
        /// <param name="maxCoordinate"></param>
        /// <param name="zoom"></param>
        /// <param name="size"></param>
        /// <param name="bytes"></param>
        /// <param name="extension"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="bandsCount"><see cref="BandsCount"/>
        /// <remarks><para/>Must be in range (0, 4];
        /// <para/><see cref="DefaultBandsCount"/> by default</remarks></param>
        /// <param name="interpolation"><see cref="Interpolation"/>
        /// <remarks><para/>Lanczos3 by default</remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom,
                          Size size = null, IEnumerable<byte> bytes = null,
                          TileExtension extension = TileExtension.Png,
                          bool tmsCompatible = false, int bandsCount = DefaultBandsCount,
                          Interpolation interpolation = Interpolation.Lanczos3)
            : base(minCoordinate, maxCoordinate, zoom, size, bytes, extension, tmsCompatible)
        {
            #region Preconditions checks

            if (bandsCount <= 0 || bandsCount > 4) throw new ArgumentOutOfRangeException(nameof(bandsCount));

            #endregion

            (BandsCount, Interpolation) = (bandsCount, interpolation);
        }

        #endregion
    }
}
