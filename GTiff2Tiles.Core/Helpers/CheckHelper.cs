using System;
using System.IO;
using System.Linq;

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Class with static methods for check the errors.
    /// </summary>
    public static class CheckHelper
    {
        #region Methods

        #region Private

        /// <summary>
        /// Check GdalInfo's string.
        /// Block - if image is tiled;
        /// Byte - colors;
        /// EPSG - projection;
        /// </summary>
        /// <param name="gdalInfoString">String from <see cref="Image.Gdal.Info"/> method.</param>
        /// <returns><see langword="true"/>, if file is OK, <see langword="false"/> otherwise.</returns>
        private static bool CheckGdalInfo(string gdalInfoString)
        {
            if (string.IsNullOrWhiteSpace(gdalInfoString)) throw new Exception("Passed GdalInfo string is empty.");
            return gdalInfoString.Contains(Enums.Image.Gdal.Block)
                && gdalInfoString.Contains(Enums.Image.Gdal.Byte)
                && gdalInfoString.Contains(Enums.Image.Gdal.Projection);
        }

        #endregion

        #region Public

        /// <summary>
        /// Checks the existance, projection, block and byte.
        /// Changes the projection and write temp.tif if can't work with this image.
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <param name="tempDirectoryInfo">Temp directory.</param>
        /// <returns>Temp file.</returns>
        public static FileInfo CheckInputFile(FileInfo inputFileInfo, DirectoryInfo tempDirectoryInfo)
        {
            try
            {
                tempDirectoryInfo.Create();
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to create temp directory.", exception);
            }

            //Check if file exists.
            if (!inputFileInfo.Exists) throw new Exception("Input file isn't exists.");

            //Check if input file is not .tif.
            if (inputFileInfo.Extension != Enums.Extensions.Tif) throw new Exception("Input file extension isn't .tif");

            //Configure Gdal.
            Image.Gdal.ConfigureGdal();

            //Check if input image is ready for cropping.
            if (CheckGdalInfo(Image.Gdal.GetInfo(inputFileInfo.FullName, null)))
                return inputFileInfo;

            FileInfo tempFileInfo = new FileInfo(Path.Combine(tempDirectoryInfo.FullName,
                                                              $"{Enums.Image.Gdal.TempFileName}{Enums.Extensions.Tif}"));
            Image.Gdal.RepairTif(inputFileInfo, tempFileInfo, null);
            return tempFileInfo;
        }

        /// <summary>
        /// Checks, if can create the output directory and it's empty.
        /// </summary>
        /// <param name="outputDirectoryInfo"></param>
        public static void CheckOutputDirectory(DirectoryInfo outputDirectoryInfo)
        {
            try
            {
                outputDirectoryInfo.Create();
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to create output directory here:{outputDirectoryInfo.FullName}.",
                                    exception);
            }

            if (outputDirectoryInfo.EnumerateFileSystemInfos().Any())
                throw new Exception("Output directory isn't empty. Please, select another directory.");
        }

        #endregion

        #endregion
    }
}
