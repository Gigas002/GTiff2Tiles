using System.Collections.Generic;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Geodesic;
using GTiff2Tiles.Core.Images;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// Raster tile
    /// </summary>
    public class RasterTile : Tile
    {
        // TODO: Bands class?

        #region Properties/Constants

        /// <summary>
        /// Default number of bands
        /// </summary>
        public const int DefaultBandsCount = 4;

        /// <summary>
        /// Number of bands in tile
        /// </summary>
        public int BandsCount { get; }

        #endregion

        #region Constructors

        /// <inheritdoc />
        public RasterTile(Number number, Size size = null, IEnumerable<byte> d = null,
                          string extension = FileExtensions.Png, bool tmsCompatible = false,
                          int bandsCount = DefaultBandsCount) :
            base(number, size, d, extension, tmsCompatible) => BandsCount = bandsCount;

        /// <inheritdoc />
        public RasterTile(Coordinate minCoordinate, Coordinate maxCoordinate, int z, Size size = null,
                          IEnumerable<byte> d = null, string extension = FileExtensions.Png,
                          bool tmsCompatible = false, int bandsCount = DefaultBandsCount) :
            base(minCoordinate, maxCoordinate, z, size, d, extension, tmsCompatible) => BandsCount = bandsCount;

        #endregion
    }
}
