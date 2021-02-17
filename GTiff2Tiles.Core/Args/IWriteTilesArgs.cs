using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Images;

namespace GTiff2Tiles.Core.Args
{
    /// <summary>
    /// Interface with args for creating different kind of tiles
    /// </summary>
    public interface IWriteTilesArgs : IDisposable
    {
        /// <summary>
        /// Minimal zoom
        /// </summary>
        public int MinZ { get; }

        /// <summary>
        /// Maximal zoom
        /// </summary>
        public int MaxZ { get; }

        /// <summary>
        /// Minimal coordinate
        /// </summary>
        public GeoCoordinate MinCoordinate { get; }

        /// <summary>
        /// Maximal coordinate
        /// </summary>
        public GeoCoordinate MaxCoordinate { get; }

        /// <summary>
        /// Is tms compatible?
        /// </summary>
        public bool TmsCompatible { get; }

        /// <summary>
        /// Size of tiles
        /// </summary>
        public Size TileSize { get; }

        /// <summary>
        /// Progress-reporter
        /// </summary>
        public IProgress<double> Progress { get; }

        /// <summary>
        /// Full path to output directory
        /// </summary>
        public string OutputDirectoryPath { get; }

        /// <summary>
        /// Count of threads
        /// </summary>
        public int ThreadsCount { get; }

        /// <summary>
        /// Shows if this object is already disposed
        /// </summary>
        public bool IsDisposed { get; }
    }
}
