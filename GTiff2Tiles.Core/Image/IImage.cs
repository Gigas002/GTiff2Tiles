using System;
using System.IO;
using System.Threading.Tasks;

namespace GTiff2Tiles.Core.Image
{
    //TODO: write xml-doc for entire interface
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

        public ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, bool tmsCompatible,
                                            string tileExtension, IProgress<double> progress, int threadsCount);

        public ValueTask GenerateTilesAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, bool tmsCompatible,
                                            string tileExtension, IProgress<double> progress, int threadsCount, bool isExperimental);

        #endregion
    }
}
