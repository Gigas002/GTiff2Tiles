using System.Collections.Generic;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions.Tile;
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

        /// <summary>
        /// Creates new tile
        /// </summary>
        /// <param name="number">Tile number</param>
        /// <param name="size">Tile size</param>
        /// <param name="d">Tile bytes</param>
        /// <param name="extension">Tile extension</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="coordinateType">Type of coordinates</param>
        /// <param name="bandsCount">Bands count</param>
        public RasterTile(Number number, Size size = null, IEnumerable<byte> d = null,
                          string extension = FileExtensions.Png, bool tmsCompatible = false,
                          CoordinateType coordinateType = CoordinateType.Mercator,
                          int bandsCount = DefaultBandsCount)
        {
            (Number, Bytes, Extension, TmsCompatible, Size, BandsCount) = (number, d, extension, tmsCompatible, size ?? DefaultSize, bandsCount);
            (MinCoordinate, MaxCoordinate) = Number.ToGeoCoordinates(coordinateType, Size.Width, tmsCompatible);
        }

        /// <summary>
        /// Creates new tile from coordinate values
        /// </summary>
        /// <param name="minCoordinate">Minimum coordinate</param>
        /// <param name="maxCoordinate">Maximum coordinate</param>
        /// <param name="z">Zoom</param>
        /// <param name="size">Tile size</param>
        /// <param name="d">Tile bytes</param>
        /// <param name="extension">Tile extension</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="bandsCount">Bands count</param>
        public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int z,
                          Size size = null, IEnumerable<byte> d = null,
                          string extension = FileExtensions.Png,
                          bool tmsCompatible = false, int bandsCount = DefaultBandsCount)
        {
            Size = size ?? DefaultSize;
            (Number minNumber, Number maxNumber) =
                GeoCoordinate.GetNumbers(minCoordinate, maxCoordinate, z, Size.Width, tmsCompatible);

            if (!minNumber.Equals(maxNumber))
                throw new TileException();

            (Number, Bytes, Extension, TmsCompatible, BandsCount) = (minNumber, d, extension, tmsCompatible, bandsCount);
            (MinCoordinate, MaxCoordinate) = (minCoordinate, maxCoordinate);
        }

        #endregion
    }
}
