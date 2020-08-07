using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Core.GeoTiffs
{
    /// <summary>
    /// Example of class to use <see cref="IGeoTiff"/> interface.
    /// </summary>
    public static class Img
    {
        /// <summary>
        /// Example of method to use <see cref="IGeoTiff"/> interface for cropping the tiles.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTIFF.</param>
        /// <param name="outputDirectoryInfo">Directory for cropped tiles.</param>
        /// <param name="minZ">Minimum cropped zoom.</param>
        /// <param name="maxZ">Maximum cropped zoom.</param>
        /// <param name="tileType">Type of tiles to create.</param>
        /// <param name="targetSystem"></param>
        /// <param name="tmsCompatible">Are tiles compatible with tms?</param>
        /// <param name="tileExtension">Extension of ready tiles.</param>
        /// <param name="progress">Progress.</param>
        /// <param name="threadsCount">Threads count.</param>
        /// <returns></returns>
        [Obsolete("Don't use in production, will be removed in stable 2.0.0 release")]
        public static async ValueTask GenerateTilesAsync(FileInfo inputFileInfo, DirectoryInfo outputDirectoryInfo,
                                                         int minZ, int maxZ, TileType tileType,
                                                         CoordinateSystem targetSystem,
                                                         bool tmsCompatible = true,
                                                         TileExtension tileExtension = TileExtension.Png,
                                                         IProgress<double> progress = null,
                                                         int threadsCount = 5)
        {
            await using Raster image = new Raster(inputFileInfo?.FullName, targetSystem);

            //Generate tiles.
            await image.WriteTilesToDirectoryAsync(outputDirectoryInfo?.FullName, minZ, maxZ,
                                                   tileSize: null, tmsCompatible: tmsCompatible,
                                                   tileExtension: tileExtension,
                                                   bandsCount: 4, progress: progress,
                                                   threadsCount: threadsCount, isPrintEstimatedTime: false)
                       .ConfigureAwait(false);
        }
    }
}
