using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;
using NetVips;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMemberInSuper.Global

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// Main interface for cropping different <see cref="ITile"/>s
    /// </summary>
    public interface IImage : IAsyncDisposable, IDisposable
    {
        #region Properties

        /// <summary>
        /// <see cref="Images.Size"/> (width, height)
        /// </summary>
        public Size Size { get; }

        /// <summary>
        /// Minimal <see cref="GeoCoordinate"/> of this <see cref="IImage"/>
        /// </summary>
        public GeoCoordinate MinCoordinate { get; }

        /// <summary>
        /// Maximal <see cref="GeoCoordinate"/> of this <see cref="IImage"/>
        /// </summary>
        public GeoCoordinate MaxCoordinate { get; }

        /// <summary>
        /// Type of <see cref="GeoCoordinate"/>
        /// </summary>
        public CoordinateType GeoCoordinateType { get; }

        /// <summary>
        /// Shows if resources have already been disposed
        /// </summary>
        public bool IsDisposed { get; }

        #endregion

        #region Methods

        #region WriteTile

        /// <summary>
        /// Gets data from source <see cref="Image"/>
        /// or tile cache for specified <see cref="RasterTile"/>
        /// and writes it to ready file
        /// </summary>
        /// <param name="tileCache">Source <see cref="Image"/>
        /// or tile cache</param>
        /// <param name="tile">Target <see cref="RasterTile"/></param>
        /// <param name="interpolation">Interpolation of ready tiles</param>
        public void WriteTileToFile(Image tileCache, RasterTile tile, string interpolation);

        /// <inheritdoc cref="WriteTileToFile"/>
        public Task WriteTileToFileAsync(Image tileCache, RasterTile tile, string interpolation);

        /// <summary>
        /// Gets data from source <see cref="Image"/>
        /// or tile cache for specified <see cref="RasterTile"/>
        /// and writes it to <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="tileCache">Source <see cref="Image"/>
        /// or tile cache</param>
        /// <param name="tile">Target <see cref="RasterTile"/></param>
        /// <param name="interpolation">Interpolation of ready tiles</param>
        /// <returns><see cref="RasterTile"/>'s <see cref="byte"/>s</returns>
        public IEnumerable<byte> WriteTileToEnumerable(Image tileCache, RasterTile tile, string interpolation);

        /// <summary>
        /// Gets data from source <see cref="Image"/>
        /// or tile cache for specified <see cref="RasterTile"/>
        /// and writes it to <see cref="ChannelWriter{T}"/>
        /// </summary>
        /// <param name="tileCache">Source <see cref="Image"/>
        /// or tile cache</param>
        /// <param name="tile">Target <see cref="RasterTile"/></param>
        /// <param name="channelWriter">Target <see cref="ChannelWriter{T}"/></param>
        /// <param name="interpolation">Interpolation of ready tiles</param>
        /// <returns><see langword="true"/> if <see cref="ITile"/> was written;
        /// <see langword="false"/> otherwise</returns>
        public bool WriteTileToChannel(Image tileCache, RasterTile tile, ChannelWriter<ITile> channelWriter,
                                       string interpolation);

        /// <returns></returns>
        /// <inheritdoc cref="WriteTileToChannel"/>
        public ValueTask WriteTileToChannelAsync(Image tileCache, RasterTile tile, ChannelWriter<ITile> channelWriter,
                                                 string interpolation);

        #endregion

        #region WriteTiles

        /// <inheritdoc cref="WriteTilesToDirectoryAsync"/>
        public void WriteTilesToDirectory(string outputDirectoryPath, int minZ, int maxZ,
                                          bool tmsCompatible = false, Size tileSize = null,
                                          string tileExtension = FileExtensions.Png,
                                          string interpolation = Interpolations.Lanczos3,
                                          int bandsCount = RasterTile.DefaultBandsCount,
                                          int tileCacheCount = 1000, int threadsCount = 0,
                                          IProgress<double> progress = null,
                                          bool isPrintEstimatedTime = false);

        /// <summary>
        /// Crops current <see cref="IImage"/> on <see cref="ITile"/>s
        /// and writes them to <paramref name="outputDirectoryPath"/>
        /// </summary>
        /// <param name="outputDirectoryPath">Directory for output <see cref="ITile"/>s</param>
        /// <param name="tileExtension">Extension of ready <see cref="ITile"/>s
        /// <remarks><para/>.png by default</remarks></param>
        /// <returns></returns>
        /// <inheritdoc cref="WriteTilesToAsyncEnumerable"/>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="tileSize"></param>
        /// <param name="interpolation"></param>
        /// <param name="bandsCount"></param>
        /// <param name="tileCacheCount"></param>
        /// <param name="threadsCount">T</param>
        /// <param name="progress"></param>
        /// <param name="isPrintEstimatedTime"></param>
        public Task WriteTilesToDirectoryAsync(string outputDirectoryPath, int minZ, int maxZ,
                                                    bool tmsCompatible = false, Size tileSize = null,
                                                    string tileExtension = FileExtensions.Png,
                                                    string interpolation = Interpolations.Lanczos3,
                                                    int bandsCount = RasterTile.DefaultBandsCount,
                                                    int tileCacheCount = 1000, int threadsCount = 0,
                                                    IProgress<double> progress = null,
                                                    bool isPrintEstimatedTime = false);

        /// <inheritdoc cref="WriteTilesToChannelAsync"/>
        public void WriteTilesToChannel(ChannelWriter<ITile> channelWriter, int minZ, int maxZ,
                                        bool tmsCompatible = false, Size tileSize = null,
                                        string interpolation = Interpolations.Lanczos3,
                                        int bandsCount = RasterTile.DefaultBandsCount,
                                        int tileCacheCount = 1000, int threadsCount = 0,
                                        IProgress<double> progress = null,
                                        bool isPrintEstimatedTime = false);

        /// <summary>
        /// Crops current <see cref="IImage"/> on <see cref="ITile"/>s
        /// and writes them to <paramref name="channelWriter"/>
        /// </summary>
        /// <param name="channelWriter"><see cref="Channel"/> to write <see cref="ITile"/> to</param>
        /// <returns></returns>
        /// <inheritdoc cref="WriteTilesToAsyncEnumerable"/>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="tileSize"></param>
        /// <param name="interpolation"></param>
        /// <param name="bandsCount"></param>
        /// <param name="tileCacheCount"></param>
        /// <param name="threadsCount">T</param>
        /// <param name="progress"></param>
        /// <param name="isPrintEstimatedTime"></param>
        public Task WriteTilesToChannelAsync(ChannelWriter<ITile> channelWriter, int minZ, int maxZ,
                                                  bool tmsCompatible = false, Size tileSize = null,
                                                  string interpolation = Interpolations.Lanczos3,
                                                  int bandsCount = RasterTile.DefaultBandsCount,
                                                  int tileCacheCount = 1000, int threadsCount = 0,
                                                  IProgress<double> progress = null,
                                                  bool isPrintEstimatedTime = false);

        /// <summary>
        /// Crops current <see cref="IImage"/> on <see cref="ITile"/>s
        /// and writes them to <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <returns><see cref="IEnumerable{T}"/> of <see cref="ITile"/>s</returns>
        /// <inheritdoc cref="WriteTilesToAsyncEnumerable"/>
        /// <param name="minZ"></param>
        /// <param name="maxZ"></param>
        /// <param name="tmsCompatible"></param>
        /// <param name="tileSize"></param>
        /// <param name="interpolation"></param>
        /// <param name="bandsCount"></param>
        /// <param name="tileCacheCount"></param>
        /// <param name="progress"></param>
        /// <param name="isPrintEstimatedTime"></param>
        public IEnumerable<ITile> WriteTilesToEnumerable(int minZ, int maxZ,
                                                         bool tmsCompatible = false, Size tileSize = null,
                                                         string interpolation = Interpolations.Lanczos3,
                                                         int bandsCount = RasterTile.DefaultBandsCount,
                                                         int tileCacheCount = 1000,
                                                         IProgress<double> progress = null,
                                                         bool isPrintEstimatedTime = false);

        /// <summary>
        /// Crops current <see cref="IImage"/> on <see cref="ITile"/>s
        /// and writes them to <see cref="IAsyncEnumerable{T}"/>
        /// </summary>
        /// <param name="minZ">Minimum cropped zoom</param>
        /// <param name="maxZ">Maximum cropped zoom</param>
        /// <param name="tmsCompatible">Do you want to create tms-compatible <see cref="ITile"/>s?
        /// <remarks><para/><see langword="false"/> by default</remarks></param>
        /// <param name="tileSize"><see cref="Images.Size"/> of <see cref="ITile"/>s
        /// <remarks><para/><see langword="null"/> by default, be sure to set it
        /// for custom implementations of <see cref="IImage"/></remarks></param>
        /// <param name="interpolation">Interpolation of ready tiles
        /// <remarks><para/><see cref="Interpolations.Lanczos3"/> by default</remarks></param>
        /// <param name="bandsCount">Count of <see cref="Band"/>s in ready <see cref="ITile"/>s
        /// <remarks><para/>4 by default</remarks></param>
        /// <param name="tileCacheCount">Count of <see cref="ITile"/> to be in cache
        /// <remarks><para/>1000 by default</remarks></param>
        /// <param name="threadsCount">Threads count
        /// <remarks><para/>Calculates automatically by default</remarks></param>
        /// <param name="progress">Progress-reporter
        /// <remarks><para/><see langword="null"/> by default</remarks></param>
        /// <param name="isPrintEstimatedTime">Do you want to see estimated time left?
        /// <remarks><para/><see langword="false"/> by default</remarks></param>
        /// <returns><see cref="IAsyncEnumerable{T}"/> of <see cref="ITile"/>s</returns>
        public IAsyncEnumerable<ITile> WriteTilesToAsyncEnumerable(int minZ, int maxZ,
                                                                   bool tmsCompatible = false, Size tileSize = null,
                                                                   string interpolation = Interpolations.Lanczos3,
                                                                   int bandsCount = RasterTile.DefaultBandsCount,
                                                                   int tileCacheCount = 1000, int threadsCount = 0,
                                                                   IProgress<double> progress = null,
                                                                   bool isPrintEstimatedTime = false);

        #endregion

        #endregion
    }
}
