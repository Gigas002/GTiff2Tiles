using System;
using System.IO;
using System.Threading.Tasks;

// ReSharper disable InheritdocConsiderUsage
// ReSharper disable MemberCanBeInternal
// ReSharper disable UnusedMemberInSuper.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Image
{
    //TODO: write xml-doc for entire interface
    /// <summary>
    /// Main interface for cropping different tiles.
    /// </summary>
    public interface IImage : IAsyncDisposable, IDisposable
    {
        #region Properties

        /// <summary>
        /// Image's width.
        /// </summary>
        public int RasterXSize { get; }

        /// <summary>
        /// Image's height.
        /// </summary>
        public int RasterYSize { get; }

        /// <summary>
        /// Upper left X coordinate.
        /// </summary>
        public double MinX { get; }

        /// <summary>
        /// Lower right Y coordinate.
        /// </summary>
        public double MinY { get; }

        /// <summary>
        /// Lower right X coordinate.
        /// </summary>
        public double MaxX { get; }

        /// <summary>
        /// Upper left Y coordinate.
        /// </summary>
        public double MaxY { get; }

        /// <summary>
        /// Shows if resources have already been disposed.
        /// </summary>
        public bool IsDisposed { get; }

        /// <summary>
        /// Image's data.
        /// </summary>
        public NetVips.Image Data { get; }

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
        public ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, bool tmsCompatible,
                                            string tileExtension, IProgress<double> progress, int threadsCount);

        /// <summary>
        /// Crops input tiff for each zoom.
        /// <para>Experimental version, don't use in release app!</para>
        /// <para>There's an issue with NetVips performance on Windows, so when it's fixed -- this version
        /// will replace the current method for cropping tiles.</para>
        /// </summary>
        /// <param name="outputDirectoryInfo">Output directory.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="tmsCompatible">Do you want to create tms-compatible tiles?</param>
        /// <param name="tileExtension">Extension of ready tiles.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <param name="isExperimental">Set to <see langword="true"/> to use this method.</param>
        /// <returns></returns>
        public ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, bool tmsCompatible,
                                            string tileExtension, IProgress<double> progress, int threadsCount,
                                            bool isExperimental);

        #endregion
    }
}
