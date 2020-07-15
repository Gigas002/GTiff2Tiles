using System;
using System.Collections.Generic;
using System.IO;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Images;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// Interface for all tiles
    /// </summary>
    public interface ITile : IDisposable, IAsyncDisposable//, IEquatable<ITile>
    {
        #region Properties

        /// <summary>
        /// Shows if tile's already disposed
        /// </summary>
        public bool IsDisposed { get; set; }

        /// <summary>
        /// Minimum coordinate of this tile
        /// </summary>
        public GeoCoordinate MinCoordinate { get; set; }

        /// <summary>
        /// Maximum coordinate of this tile
        /// </summary>
        public GeoCoordinate MaxCoordinate { get; set; }

        /// <summary>
        /// Number of this tile
        /// </summary>
        public Number Number { get; set; }

        /// <summary>
        /// Tile bytes
        /// </summary>
        public IEnumerable<byte> Bytes { get; set; }

        /// <summary>
        /// Size (width, height) of this tile
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// FileInfo that represents this tile
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// Extension of tile
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Is tile tms compatible?
        /// </summary>
        public bool TmsCompatible { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this tile is not empty or too small
        /// </summary>
        /// <param name="isCheckFileInfo">Do you want to check <see cref="FileInfo"/>?</param>
        /// <returns><see langword="true"/> if tile's valid, <see langword="false"/> otherwise</returns>
        public bool Validate(bool isCheckFileInfo);

        /// <summary>
        /// Calculates this tile position in upper tile
        /// </summary>
        /// <returns>Value in range from 0 to 3</returns>
        public int CalculatePosition();

        #endregion
    }
}
