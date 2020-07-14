using System;
using System.Collections.Generic;
using System.IO;
using GTiff2Tiles.Core.Geodesic;
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
        public Coordinate MinCoordinate { get; set; }

        /// <summary>
        /// Maximum coordinate of this tile
        /// </summary>
        public Coordinate MaxCoordinate { get; set; }

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

        /// <summary>
        /// Get minimum/maximum numbers from this tile's coordinates
        /// </summary>
        /// <returns><see cref="ValueTuple"/> of numbers</returns>
        public (Number minNumber, Number maxNumber) GetNumbersFromCoords();

        /// <summary>
        /// Get lower tiles on specified zoom for current tile
        /// </summary>
        /// <param name="zoom">Zoom on which you want to know tile's numbers</param>
        /// <returns><see cref="ValueTuple"/> of numbers</returns>
        public (Number minNumber, Number maxNumber) GetLowerNumbers(int zoom);

        /// <summary>
        /// Get number of tiles in current region from minimum zoom to maximum
        /// </summary>
        /// <param name="minZ">Minimum zoom</param>
        /// <param name="maxZ">Maximum zoom</param>
        /// <returns>Tiles count</returns>
        public int GetCount(int minZ, int maxZ);

        /// <summary>
        /// Get coordinates of current tile
        /// </summary>
        /// <returns><see cref="ValueTuple"/> of WGS84 coordinates</returns>
        public (Coordinate minCoordinate, Coordinate maxCoordinate) GetCoordinates();
    }
}
