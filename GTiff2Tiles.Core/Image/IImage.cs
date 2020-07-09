using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Geodesic;

// ReSharper disable InheritdocConsiderUsage
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Image
{
    /// <summary>
    /// Main interface for cropping different tiles.
    /// </summary>
    public interface IImage : IAsyncDisposable, IDisposable
    {
        #region Properties

        public Size Size { get; }

        public Coordinate MinCoordinate { get; }

        public Coordinate MaxCoordinate { get; }

        // <summary>
        // Upper left X coordinate.
        // </summary>
        //public double MinX { get; }
        // <summary>
        // Lower right Y coordinate.
        // </summary>
        //public double MinY { get; }
        // <summary>
        // Lower right X coordinate.
        // </summary>
        //public double MaxX { get; }
        // <summay>
        // Upper left Y coordinate.
        // </summary>
        //public double MaxY { get; }

        /// <summary>
        /// Shows if resources have already been disposed.
        /// </summary>
        public bool IsDisposed { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Crops input tiff for each zoom.
        /// </summary>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="tmsCompatible">Do you want to create tms-compatible tiles?</param>
        /// <param name="tileExtension">Extension of ready tiles.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        public ValueTask WriteTilesToDirectoryAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ,
                                                    Size tileSize, bool tmsCompatible = false,
                                                    string tileExtension = Constants.Extensions.Png,
                                                    int bands = Constants.Image.Raster.Bands,
                                                    IProgress<double> progress = null, int threadsCount = 0,
                                                    bool isPrintEstimatedTime = true, int tileCacheCount = 1000);

        #endregion
    }
}
