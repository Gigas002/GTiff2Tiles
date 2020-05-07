using System;
using System.IO;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums.Image;

namespace GTiff2Tiles.Core.Image
{
    public static class Img
    {
        public static async ValueTask GenerateTilesAsync(FileInfo inputFileInfo, DirectoryInfo outputDirectoryInfo,
                                                         int minZ, int maxZ, TileType tileType,
                                                         bool tmsCompatible = true,
                                                         TileExtension tileExtension = TileExtension.Png,
                                                         IProgress<double> progress = null,
                                                         int threadsCount = 5)
        {
            //TODO: This is +-example
            await using IImage image = tileType switch
            {
                TileType.Raster => new Raster(inputFileInfo),
                //CropTypes.Terrain => new Image(inputFileInfo),
                _ => throw new Exception()
            };

            string tileExtensionString = tileExtension switch
            {
                TileExtension.Png => Extensions.Png,
                TileExtension.Jpg => Extensions.Jpg,
                TileExtension.Webp => Extensions.Webp,
                _ => throw new ArgumentOutOfRangeException(nameof(tileExtension), tileExtension, null)
            };

            await image.GenerateTilesAsync(outputDirectoryInfo, minZ, maxZ, tmsCompatible, tileExtensionString, progress,
                                           threadsCount);
        }
    }
}
