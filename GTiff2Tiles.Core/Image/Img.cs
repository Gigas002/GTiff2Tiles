using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;

namespace GTiff2Tiles.Core.Image
{
    public abstract class Img
    {
        public static async ValueTask GenerateTilesAsync(FileInfo inputFileInfo, DirectoryInfo outputDirectoryInfo,
                                                         int minZ, int maxZ, CropTypes cropType,
                                                         bool tmsCompatible = true,
                                                         string tileExtension = Extensions.Png,
                                                         IProgress<double> progress = null,
                                                         int threadsCount = 5)
        {
            //TODO: This is example
            await using IImage image = cropType switch
            {
                CropTypes.Raster => new Raster(inputFileInfo),
                //CropTypes.Terrain => new Image(inputFileInfo),
                _ => throw new Exception()
            };

            await image.GenerateTilesAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtension, progress,
                                           threadsCount);
        }

        public enum CropTypes
        {
            Raster
        }
    }
}
