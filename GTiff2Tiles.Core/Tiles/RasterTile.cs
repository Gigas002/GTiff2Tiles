using System;
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
        /// <summary>
        /// <see cref="BandsCount"/> backing field
        /// </summary>
        private int _bandsCount = DefaultBandsCount;

        #region Properties/Constants

        /// <summary>
        /// Default count of bands
        /// </summary>
        public const int DefaultBandsCount = 4;

        /// <summary>
        /// Count of bands in <see cref="RasterTile"/>
        /// </summary>
        public int BandsCount
        {
            get => _bandsCount;
            set
            {
                if (value <= 0 || value > 4) throw new ArgumentOutOfRangeException(nameof(value));

                _bandsCount = value;
            }
        }

        /// <summary>
        /// Interpolation of this <see cref="RasterTile"/>
        /// </summary>
        public Interpolation Interpolation { get; set; } = Interpolation.Lanczos3;

        #endregion

        #region Constructors

        /// <inheritdoc cref="Tile(Number,CoordinateSystem,Size,bool)"/>
        /// <param name="number"></param>
        /// <param name="coordinateSystem"></param>
        /// <param name="size"></param>
        /// <param name="tmsCompatible"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public RasterTile(Number number, CoordinateSystem coordinateSystem, Size size = null, bool tmsCompatible = false) : base(number, coordinateSystem, size, tmsCompatible) { }

        /// <inheritdoc cref="Tile(GeoCoordinate,GeoCoordinate,int,Size,bool)"/>
        /// <param name="minCoordinate"></param>
        /// <param name="maxCoordinate"></param>
        /// <param name="zoom"></param>
        /// <param name="size"></param>
        /// <param name="tmsCompatible"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom, Size size = null, bool tmsCompatible = false) : base(minCoordinate, maxCoordinate, zoom, size, tmsCompatible) { }

        #endregion
    }
}
