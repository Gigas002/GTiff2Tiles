using System;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable UnusedMemberInSuper.Global

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Interface for any coordinate
    /// </summary>
    public interface ICoordinate : IEquatable<ICoordinate>
    {
        #region Properties

        /// <summary>
        /// X coordinate value or Longitude
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y coordinate value or Latitude
        /// </summary>
        public double Y { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Calculate <see cref="Number"/> for current <see cref="ICoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize"><see cref="ITile"/>'s side size</param>
        /// <param name="tmsCompatible">Is <see cref="ITile"/> tms compatible?</param>
        /// <returns><see cref="Number"/> in which this <see cref="ICoordinate"/> belongs</returns>
        public Number ToNumber(int zoom, int tileSize, bool tmsCompatible);

        #endregion
    }
}
