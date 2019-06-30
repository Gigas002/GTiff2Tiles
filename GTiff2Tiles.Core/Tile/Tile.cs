using System;
using System.Linq;
using GTiff2Tiles.Core.Exceptions.Tile;
using GTiff2Tiles.Core.Localization;

namespace GTiff2Tiles.Core.Tile
{
    /// <summary>
    /// Class with static methods for calculating some tile stuff.
    /// </summary>
    public static class Tile
    {
        /// <summary>
        /// Calculates the tile numbers for zoom which covers given lon/lat coordinates.
        /// </summary>
        /// <param name="minX">Minimum longitude.</param>
        /// <param name="minY">Minimum latitude.</param>
        /// <param name="maxX">Maximum longitude.</param>
        /// <param name="maxY">Maximum latitude.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <remarks>Throws <see cref="TileException"/>.</remarks>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> of tiles numbers.</returns>
        public static (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetTileNumbersFromCoords(double minX,
                                                                                                        double minY,
                                                                                                        double maxX,
                                                                                                        double maxY,
                                                                                                        int zoom)
        {
            #region Parameters checking

            if (zoom < 0)
                throw new TileException(string.Format(Strings.LesserThan, nameof(zoom), 0));

            #endregion

            int[] tilesXs = new int[2];
            int[] tilesYs = new int[2];

            try
            {
                tilesXs[0] = Convert.ToInt32(Math.Ceiling((180.0 + minX) * Math.Pow(2.0, zoom) / 180.0) - 1.0);
                tilesXs[1] = Convert.ToInt32(Math.Ceiling((180.0 + maxX) * Math.Pow(2.0, zoom) / 180.0) - 1.0);
                tilesYs[0] = Convert.ToInt32(Math.Ceiling((90.0 + minY) * Math.Pow(2.0, zoom) / 180.0) - 1.0);
                tilesYs[1] = Convert.ToInt32(Math.Ceiling((90.0 + maxY) * Math.Pow(2.0, zoom) / 180.0) - 1.0);
            }
            catch (Exception exception)
            {
                throw new
                    TileException(string.Format(Strings.UnableToConvertCoordinatesToTiles),
                                  exception);
            }

            return (tilesXs.Min(), tilesYs.Min(), tilesXs.Max(), tilesYs.Max());
        }

        /// <summary>
        /// Calculates tile's coordinate borders for passed tiles numbers and zoom.
        /// </summary>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <param name="isFlipY">Should flip y number?</param>
        /// <remarks>Выбрасывает <see cref="TileException"/>.</remarks>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> of WGS84 coordinates.</returns>
        public static (double minX, double minY, double maxX, double maxY) TileBounds(int tileX,
                                                                                      int tileY,
                                                                                      int zoom,
                                                                                      bool isFlipY = true)
        {
            try
            {
                if (isFlipY) tileY = Convert.ToInt32(Math.Pow(2.0, zoom) - tileY - 1);

                double minX = tileX * 180.0 / Math.Pow(2.0, zoom) - 180.0;
                double minY = tileY * 180.0 / Math.Pow(2.0, zoom) - 90.0;
                double maxX = (tileX + 1) * 180.0 / Math.Pow(2.0, zoom) - 180.0;
                double maxY = (tileY + 1) * 180.0 / Math.Pow(2.0, zoom) - 90.0;

                return (minX, minY, maxX, maxY);
            }
            catch (Exception exception)
            {
                throw new TileException(string.Format(Strings.UnableToCalculateCoordinates),
                                        exception);
            }
        }
    }
}
