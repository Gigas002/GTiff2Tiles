using System;
using System.Collections.Generic;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Images;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// Interface for all tiles
    /// </summary>
    public interface ITile : IDisposable, IAsyncDisposable
    {
        #region Properties

        /// <summary>
        /// Shows if <see cref="ITile"/>'s already disposed
        /// </summary>
        public bool IsDisposed { get; set; }

        /// <summary>
        /// Minimum <see cref="GeoCoordinate"/> of this <see cref="ITile"/>
        /// </summary>
        public GeoCoordinate MinCoordinate { get; }

        /// <summary>
        /// Maximum <see cref="GeoCoordinate"/> of this <see cref="ITile"/>
        /// </summary>
        public GeoCoordinate MaxCoordinate { get; }

        /// <summary>
        /// <see cref="Tiles.Number"/> of this <see cref="ITile"/>
        /// </summary>
        public Number Number { get; }

        /// <summary>
        /// <see cref="ITile"/> bytes
        /// </summary>
        public IEnumerable<byte> Bytes { get; set; }

        /// <summary>
        /// <see cref="Images.Size"/> (width, height) of this <see cref="ITile"/>
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// Path on disk of this <see cref="ITile"/>
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Extension of <see cref="ITile"/>
        /// </summary>
        public string Extension { get; }

        /// <summary>
        /// Is <see cref="ITile"/> tms compatible?
        /// </summary>
        public bool TmsCompatible { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this <see cref="ITile"/> is not empty or too small
        /// </summary>
        /// <param name="isCheckPath">Do you want to check <see cref="Path"/>?</param>
        /// <param name="minimalBytesCount">Minimal accepted count of <see cref="byte"/>s in this tile</param>
        /// <returns><see langword="true"/> if <see cref="ITile"/>'s valid;
        /// <para/><see langword="false"/> otherwise</returns>
        public bool Validate(bool isCheckPath, int minimalBytesCount);

        /// <summary>
        /// Calculates this <see cref="ITile"/> position in upper <see cref="ITile"/>
        /// </summary>
        /// <returns>Value in range from 0 to 3</returns>
        public int CalculatePosition();

        #endregion
    }
}
