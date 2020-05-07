/*******************************************************************************
 Copyright (c) 2008, Klokan Petr Pridal
 Copyright (c) 2010-2013, Even Rouault <even dot rouault at spatialys.com>

 Permission is hereby granted, free of charge, to any person obtaining a
 copy of this software and associated documentation files (the "Software"),
 to deal in the Software without restriction, including without limitation
 the rights to use, copy, modify, merge, publish, distribute, sublicense,
 and/or sell copies of the Software, and to permit persons to whom the
 Software is furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included
 in all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 DEALINGS IN THE SOFTWARE.
*******************************************************************************/

/*******************************************************************************
 I don't really know about that licensing stuff, but I used some code with this license in files above, so I included it here just in case.
 All the C# code (except for GdalHelper) in this project is written by me (https://github.com/Gigas002).
*******************************************************************************/

using System;
using System.Linq;
using GTiff2Tiles.Core.Exceptions.Tile;
using GTiff2Tiles.Core.Localization;

// ReSharper disable MemberCanBeInternal

namespace GTiff2Tiles.Core.Tile
{
    /// <summary>
    /// Class with static methods for calculating some tile stuff.
    /// </summary>
    public static class Tile
    {
        #region Private

        /// <summary>
        /// Resolution for tiles.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>Resoultion value.</returns>
        private static double Resolution(int zoom) => 180.0 / Constants.Image.Image.TileSize / Math.Pow(2.0, zoom);

        #endregion

        #region Public

        /// <summary>
        /// Calculates the tile numbers for zoom which covers given lon/lat coordinates.
        /// </summary>
        /// <param name="minX">Minimum longitude.</param>
        /// <param name="minY">Minimum latitude.</param>
        /// <param name="maxX">Maximum longitude.</param>
        /// <param name="maxY">Maximum latitude.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <param name="tmsCompatible">Do you want tms tiles on output?</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> of tiles numbers.</returns>
        public static (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetTileNumbersFromCoords(double minX,
                                                                                                        double minY,
                                                                                                        double maxX,
                                                                                                        double maxY,
                                                                                                        int zoom,
                                                                                                        bool tmsCompatible)
        {
            #region Parameters checking

            if (zoom < 0) throw new TileException(string.Format(Strings.LesserThan, nameof(zoom), 0));

            #endregion

            int[] tilesXs = new int[2];
            int[] tilesYs = new int[2];

            try
            {
                tilesXs[0] =
                    Convert.ToInt32(Math.Ceiling((180.0 + minX) / Resolution(zoom) / Constants.Image.Image.TileSize) - 1.0);
                tilesXs[1] =
                    Convert.ToInt32(Math.Ceiling((180.0 + maxX) / Resolution(zoom) / Constants.Image.Image.TileSize) - 1.0);
                tilesYs[0] =
                    Convert.ToInt32(Math.Ceiling((90.0 + minY) / Resolution(zoom) / Constants.Image.Image.TileSize) - 1.0);
                tilesYs[1] =
                    Convert.ToInt32(Math.Ceiling((90.0 + maxY) / Resolution(zoom) / Constants.Image.Image.TileSize) - 1.0);
            }
            catch (Exception exception)
            {
                throw new TileException(string.Format(Strings.UnableToConvertCoordinatesToTiles), exception);
            }

            //Flip y's
            // ReSharper disable once InvertIf
            if (!tmsCompatible)
            {
                tilesYs[0] = Convert.ToInt32(Math.Pow(2.0, zoom) - tilesYs[0] - 1);
                tilesYs[1] = Convert.ToInt32(Math.Pow(2.0, zoom) - tilesYs[1] - 1);
            }

            return (tilesXs.Min(), tilesYs.Min(), tilesXs.Max(), tilesYs.Max());
        }

        /// <summary>
        /// Calculates tile's coordinate borders for passed tiles numbers and zoom.
        /// </summary>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <param name="tmsCompatible">Do you want tms tiles on output?</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> of WGS84 coordinates.</returns>
        public static (double minX, double minY, double maxX, double maxY) TileBounds(int tileX, int tileY, int zoom, bool tmsCompatible)
        {
            try
            {
                //Flip the y number for non-tms
                if (!tmsCompatible) tileY = Convert.ToInt32(Math.Pow(2.0, zoom) - tileY - 1);

                double minX = tileX * Constants.Image.Image.TileSize * Resolution(zoom) - 180.0;
                double minY = tileY * Constants.Image.Image.TileSize * Resolution(zoom) - 90.0;
                double maxX = (tileX + 1) * Constants.Image.Image.TileSize * Resolution(zoom) - 180.0;
                double maxY = (tileY + 1) * Constants.Image.Image.TileSize * Resolution(zoom) - 90.0;

                return (minX, minY, maxX, maxY);
            }
            catch (Exception exception)
            {
                throw new TileException(string.Format(Strings.UnableToCalculateCoordinates), exception);
            }
        }

        #endregion
    }
}
