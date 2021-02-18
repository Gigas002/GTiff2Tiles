using System;
using System.Collections.Generic;
using System.Threading.Channels;
using GTiff2Tiles.Core.Args;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
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
        /// Shows if this <see cref="ITile"/>'s already disposed
        /// </summary>
        public bool IsDisposed { get; }

        /// <summary>
        /// Coordinate system of this <see cref="ITile"/>
        /// </summary>
        public CoordinateSystem CoordinateSystem { get; }

        /// <summary>
        /// Minimal <see cref="GeoCoordinate"/> of this <see cref="ITile"/>
        /// </summary>
        public GeoCoordinate MinCoordinate { get; }

        /// <summary>
        /// Maximal <see cref="GeoCoordinate"/> of this <see cref="ITile"/>
        /// </summary>
        public GeoCoordinate MaxCoordinate { get; }

        /// <summary>
        /// <see cref="Tiles.Number"/> of this <see cref="ITile"/>
        /// </summary>
        public Number Number { get; }

        /// <summary>
        /// Collection of <see cref="ITile"/>'s bytes
        /// </summary>
        public IEnumerable<byte> Bytes { get; set; }

        /// <summary>
        /// <see cref="Images.Size"/> (width and height) of this <see cref="ITile"/>
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Path on disk of this <see cref="ITile"/>
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Extension of <see cref="ITile"/>
        /// </summary>
        public TileExtension Extension { get; set; }

        /// <summary>
        /// Is <see cref="ITile"/> tms compatible?
        /// </summary>
        public bool TmsCompatible { get; set; }

        /// <summary>
        /// <see cref="ITile"/>s with <see cref="Bytes"/> count lesser
        /// than this value won't pass <see cref="Validate(bool)"/> check
        /// </summary>
        public int MinimalBytesCount { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if this <see cref="ITile"/> is not empty or too small
        /// <remarks><para/>See <see cref="MinimalBytesCount"/> property for more info</remarks>
        /// </summary>
        /// <param name="isCheckPath">Do you want to check <see cref="Path"/>?</param>
        /// <returns><see langword="true"/> if <see cref="ITile"/>'s valid;
        /// <para/><see langword="false"/> otherwise</returns>
        public bool Validate(bool isCheckPath);

        /// <summary>
        /// Calculates this <see cref="ITile"/>'s position in upper <see cref="ITile"/>
        /// </summary>
        /// <returns>Value in range from 0 to 3
        /// <remarks><para/>Starts always from upper-left corner and goes to
        /// lower-right, but maths depends on <see cref="TmsCompatible"/> value</remarks></returns>
        public int CalculatePosition();

        /// <summary>
        /// Get <see cref="string"/> from <see cref="TileExtension"/>
        /// </summary>
        /// <returns>Converted <see cref="string"/></returns>
        public string GetExtensionString();

        /// <summary>
        /// Writes <see cref="ITile"/>'s <see cref="Bytes"/> to file
        /// </summary>
        /// <param name="path">Full path to write <see cref="ITile"/>
        /// <remarks><para/>if not set, <see cref="Path"/> property will be used instead</remarks></param>
        public void WriteToFile(string path = null);

        /// <summary>
        /// Write tile to file, using source geotiff and corresponding args
        /// </summary>
        /// <param name="sourceGeoTiff">Source <see cref="IGeoTiff"/></param>
        /// <param name="args">Additional arguments</param>
        public void WriteToFile(IGeoTiff sourceGeoTiff, IWriteTilesArgs args);

        /// <summary>
        /// Write tile to enumerable of bytes, using source geotiff and corresponding args
        /// </summary>
        /// <param name="sourceGeoTiff">Source <see cref="IGeoTiff"/></param>
        /// <param name="args">Additional arguments</param>
        public IEnumerable<byte> WriteToEnumerable(IGeoTiff sourceGeoTiff, IWriteTilesArgs args);

        /// <summary>
        /// Write tile to channel, using source geotiff and corresponding args
        /// </summary>
        /// <typeparam name="T">Inheritors of <see cref="ITile"/></typeparam>
        /// <param name="sourceGeoTiff">Source <see cref="IGeoTiff"/></param>
        /// <param name="tileWriter">Target <see cref="ChannelWriter{T}"/></param>
        /// <param name="args">Additional arguments</param>
        /// <returns><see langword="true"/> if no errors happened; <see langword="false"/> otherwse</returns>
        public bool WriteToChannel<T>(IGeoTiff sourceGeoTiff, ChannelWriter<T> tileWriter, IWriteTilesArgs args) where T : class, ITile;

        /// <summary>
        /// Create overview bytes for current <see cref="ITile"/> from source <see cref="ITile"/>s
        /// </summary>
        /// <typeparam name="T">Inheritors of <see cref="ITile"/></typeparam>
        /// <param name="allBaseTiles">Array of all tiles to search 4 correlating</param>
        /// <returns>Bytes of overview tile</returns>
        public IEnumerable<byte> WriteOverviewTileBytes<T>(T[] allBaseTiles) where T : class, ITile;

        #endregion
    }
}
