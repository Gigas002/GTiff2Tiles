using System;
using System.IO;

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Class with static methods for check the errors.
    /// </summary>
    public static class CheckHelper
    {
        /// <summary>
        /// Check GdalInfo's string.
        /// Block - if image is tiled;
        /// Byte - colors;
        /// EPSG - projection;
        /// </summary>
        /// <param name="gdalInfoString">String from <see cref="Image.Gdal.Info"/> method.</param>
        /// <returns><see langword="true"/>, if file is OK, <see langword="false"/> otherwise.</returns>
        private static bool CheckGdalInfo(string gdalInfoString) =>
            gdalInfoString?.Contains("Block") == true
         && gdalInfoString.Contains("Byte")
         && gdalInfoString.Contains("AUTHORITY[\"EPSG\",\"4326\"]]");

        /// <summary>
        /// Checks the existance, projection, block and byte.
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        public static void CheckInputFile(FileInfo inputFileInfo)
        {
            //Check if file exists.
            if (!inputFileInfo.Exists) throw new Exception("Input file isn't exists.");

            //Check if input file is not .tif.
            if (inputFileInfo.Extension != Enums.Extensions.Tif) throw new Exception("Input file extension isn't .tif");

            //Configure Gdal.
            Image.Gdal.ConfigureGdal();

            //Check if input image is ready for cropping.
            if (!CheckGdalInfo(Image.Gdal.GetInfo(inputFileInfo.FullName, null))) throw new Exception("This image has wrong projecting/isn't tiled/has wrong colors.");
        }
    }
}
