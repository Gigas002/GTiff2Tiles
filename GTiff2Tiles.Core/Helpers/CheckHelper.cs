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
        /// </summary>
        /// <param name="gdalInfoString">String from <see cref="Image.Gdal.Info"/> method.</param>
        /// <param name="proj4String">Proj4 string.</param>
        /// <returns><see langword="true"/>, if file is OK, <see langword="false"/> otherwise.</returns>
        private static bool CheckTifInfo(string gdalInfoString, string proj4String)
        {
            if (string.IsNullOrWhiteSpace(gdalInfoString)) throw new Exception("Passed GdalInfo string is empty.");

            //Check projection.
            if (!proj4String.Contains(Enums.Image.Gdal.LongLat) || !proj4String.Contains(Enums.Image.Gdal.Wgs84))
                return false;

            //Other checks.
            return gdalInfoString.Contains(Enums.Image.Gdal.Block) && gdalInfoString.Contains(Enums.Image.Gdal.Byte);
        }

        #endregion

        #region Public

        /// <summary>
        /// Checks the existance, projection, block and byte.
        /// Changes the projection and write temp.tif if can't work with this image.
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <param name="tempDirectoryInfo">Temp directory for fixed tif.</param>
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

            //Get proj4 string.
            string proj4String = Image.Gdal.GetProj4String(inputFileInfo.FullName);

            //Check if input image is ready for cropping.
            if (CheckTifInfo(Image.Gdal.GetInfo(inputFileInfo.FullName, null), proj4String))
                return inputFileInfo;

            FileInfo tempFileInfo = new FileInfo(Path.Combine(tempDirectoryInfo.FullName,
                                                              $"{Enums.Image.Gdal.TempFileName}{Enums.Extensions.Tif}"));
            Image.Gdal.RepairTif(inputFileInfo, tempFileInfo);
            return tempFileInfo;
        }

        /// <summary>
        /// Checks, if can create the output directory and it's empty.
        /// </summary>
        /// <param name="outputDirectoryInfo">Output directory.</param>
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
