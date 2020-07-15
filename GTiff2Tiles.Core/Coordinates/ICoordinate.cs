using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Interface for any coordinate
    /// </summary>
    public interface ICoordinate
    {
        #region Properties

        /// <summary>
        /// X coordinate value or Longitude
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate value or Latitude
        /// </summary>
        public double Y { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Calculate <see cref="Number"/> for current <see cref="ICoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns><see cref="Number"/> in which this <see cref="ICoordinate"/> belongs</returns>
        public Number ToNumber(int zoom, int tileSize, bool tmsCompatible);

        #endregion
    }
}
