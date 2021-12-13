using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NetVips;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace GTiff2Tiles.Core.Args
{
    /// <summary>
    /// Class with args for creating raster tiles
    /// </summary>
    public class WriteRasterTilesArgs : IWriteTilesArgs
    {
        #region Properties

        #region Implementation of IWriteArgs

        /// <inheritdoc />
        public int MinZ { get; init; }

        /// <inheritdoc />
        public int MaxZ { get; init; }

        /// <inheritdoc />
        public GeoCoordinate MinCoordinate { get; set; }

        /// <inheritdoc />
        public GeoCoordinate MaxCoordinate { get; set; }

        /// <inheritdoc />
        /// <remarks><para/><see langword="false"/> by default</remarks>
        public bool TmsCompatible { get; set; }

        /// <inheritdoc />
        /// <remarks><para/>256x256 by default</remarks>
        public Size TileSize { get; set; } = Tile.DefaultSize;

        /// <inheritdoc />
        public IProgress<double> Progress { get; set; }

        /// <inheritdoc />
        public string OutputDirectoryPath { get; set; }

        /// <inheritdoc />
        public bool IsDisposed { get; protected set; }

        /// <inheritdoc />
        public int ThreadsCount { get; set; }

        /// <inheritdoc />
        /// <remarks>EPSG:4326 by default</remarks>
        public CoordinateSystem GeoCoordinateSystem { get; set; } = CoordinateSystem.Epsg4326;

        #endregion

        /// <summary>
        /// Interpolation of ready tiles
        /// <remarks><para/><see cref="NetVips.Enums.Kernel.Lanczos3"/> by default</remarks>
        /// </summary>
        public NetVips.Enums.Kernel TileInterpolation { get; set; } = NetVips.Enums.Kernel.Lanczos3;

        /// <summary>
        /// Count of <see cref="Band"/>s in ready <see cref="ITile"/>s
        /// <remarks><para/>4 by default</remarks>
        /// </summary>
        public int BandsCount { get; set; } = RasterTile.DefaultBandsCount;

        /// <summary>
        /// Count of <see cref="ITile"/> to be in cache
        /// <remarks><para/>1000 by default</remarks>
        /// </summary>
        public int TileCacheCount { get; set; } = 1000;

        /// <summary>
        /// Extension of ready tiles
        /// </summary>
        public TileExtension TileExtension { get; set; } = TileExtension.Png;

        /// <summary>
        /// Prints time left
        /// </summary>
        public PrintTime TimePrinter { get; set; }

        /// <summary>
        /// Raster image source cache
        /// </summary>
        public Image TileCache { get; set; }

        #endregion

        #region Delegates

        /// <summary>
        /// Prints estimated time left
        /// </summary>
        /// <param name="timeString">String to print</param>
        public delegate void PrintTime(string timeString);

        #endregion

        #region Constructors/Destructor

        /// <summary>
        /// Create new object woth default args
        /// </summary>
        /// <param name="minZ">Minimal zoom
        /// <remarks><para/>Should be >= 0 and lesser or equal, than <paramref name="maxZ"/>
        /// </remarks></param>
        /// <param name="maxZ">Maximal zoom
        /// <remarks><para/>Should be >= 0 and bigger or equal, than <paramref name="minZ"/>
        /// </remarks></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public WriteRasterTilesArgs(int minZ, int maxZ)
        {
            #region Preconditions checks

            if (minZ < 0) throw new ArgumentOutOfRangeException(nameof(minZ));
            if (maxZ < minZ) throw new ArgumentOutOfRangeException(nameof(maxZ));

            #endregion

            (MinZ, MaxZ) = (minZ, maxZ);
        }

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> on this object
        /// </summary>
        ~WriteRasterTilesArgs() => Dispose(false);

        #endregion

        #region Methods

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="Dispose()"/>
        /// <param name="disposing">Dispose static fields?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                // Occurs only if called by programmer. Dispose static things here
            }

            TileCache?.Dispose();

            IsDisposed = true;
        }

        #endregion

        #endregion
    }

}
