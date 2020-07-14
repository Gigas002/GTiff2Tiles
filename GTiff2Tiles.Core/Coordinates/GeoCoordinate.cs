using System;
using System.Collections.Generic;
using System.Linq;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Abstract class for geographical coordinates
    /// </summary>
    public abstract class GeoCoordinate : Coordinate
    {
        #region Methods

        /// <inheritdoc />
        public override Number ToNumber(int zoom, int tileSize)
        {
            PixelCoordinate pixelCoordinate = ToPixelCoordinate(zoom, tileSize);

            return pixelCoordinate.ToNumber(zoom, tileSize);
        }

        /// <summary>
        /// Convert current <see cref="GeoCoordinate"/> to <see cref="PixelCoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns>Converted <see cref="PixelCoordinate"/></returns>
        public abstract PixelCoordinate ToPixelCoordinate(int zoom, int tileSize);

        /// <summary>
        /// Resolution for given zoom level (measured at Equator)
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns>Resolution value</returns>
        public abstract double Resolution(int zoom, int tileSize);

        /// <summary>
        /// Calculate zoom from known pixel size
        /// </summary>
        /// <param name="pixelSize">Pixel size</param>
        /// <param name="tileSize">Tile's size</param>
        /// <param name="minZoom">Minimal zoom
        /// <para/>0 by default</param>
        /// <param name="maxZoom">Maximal zoom
        /// <para/>32 by default</param>
        /// <returns>Approximate zoom value</returns>
        public int ZoomForPixelSize(int pixelSize, int tileSize, int minZoom = 0, int maxZoom = 32)
        {
            //"Maximal scaledown zoom of the pyramid closest to the pixelSize."
            IEnumerable<int> range = Enumerable.Range(minZoom, maxZoom + 1);

            foreach (int i in range)
            {
                if (pixelSize > Resolution(i, tileSize)) return Math.Max(0, i - 1);
            }

            return maxZoom - 1;
        }

        #endregion
    }
}
