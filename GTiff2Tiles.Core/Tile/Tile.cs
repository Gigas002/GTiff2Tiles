using System;
using System.Linq;

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
        /// <param name="xMin">Minimum longitude.</param>
        /// <param name="yMin">Minimum latitude.</param>
        /// <param name="xMax">Maximum longitude.</param>
        /// <param name="yMax">Maximum latitude.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <returns>Tile numbers array.</returns>
        public static (int xmin, int ymin, int xmax, int ymax) GetTileNumbersFromCoords(double xMin,
                                                                                        double yMin,
                                                                                        double xMax,
                                                                                        double yMax,
                                                                                        int zoom)
        {
            int[] xs = new int[2];
            int[] ys = new int[2];
            xs[0] = Convert.ToInt32(Math.Ceiling((180.0 + xMin) * Math.Pow(2.0, zoom) / 180.0) - 1.0);
            xs[1] = Convert.ToInt32(Math.Ceiling((180.0 + xMax) * Math.Pow(2.0, zoom) / 180.0) - 1.0);
            ys[0] = Convert.ToInt32(Math.Ceiling((90.0 + yMin) * Math.Pow(2.0, zoom) / 180.0) - 1.0);
            ys[1] = Convert.ToInt32(Math.Ceiling((90.0 + yMax) * Math.Pow(2.0, zoom) / 180.0) - 1.0);

            return (xs.Min(), ys.Min(), xs.Max(), ys.Max());
        }

        /// <summary>
        /// Calculates tile's coordinate borders for passed tiles numbers and zoom.
        /// </summary>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <param name="isFlipY">Should flip y number?</param>
        /// <returns>4 WGS84 coordinates.</returns>
        public static (double xMin, double yMin, double xMax, double yMax) TileBounds(
            int tileX, int tileY, int zoom, bool isFlipY = true)
        {
            if (isFlipY)
                tileY = Convert.ToInt32(Math.Pow(2.0, zoom) - tileY - 1);
            return (tileX * 180.0 / Math.Pow(2.0, zoom) - 180.0,
                    tileY * 180.0 / Math.Pow(2.0, zoom) - 90.0,
                    (tileX + 1) * 180.0 / Math.Pow(2.0, zoom) - 180.0,
                    (tileY + 1) * 180.0 / Math.Pow(2.0, zoom) - 90.0);
        }
    }
}
