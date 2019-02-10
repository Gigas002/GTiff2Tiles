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
        /// Byte - type;
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
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <returns><see langword="true"/> if no errors in input file, <see langword="false"/> otherwise.</returns>
        public static bool CheckInputFile(FileInfo inputFileInfo)
        {
            //Check if file exists.
            if (!inputFileInfo.Exists) throw new Exception($"Input file isn't exists. Path:{inputFileInfo.FullName}");

            //Check if input file is not .tif.
            if (inputFileInfo.Extension != Enums.Extensions.Tif) throw new Exception($"Input file extension isn't .tif. Path:{inputFileInfo.FullName}");

            //Get proj4 string.
            string proj4String = Image.Gdal.GetProj4String(inputFileInfo);

            //Check if input image is ready for cropping.
            return CheckTifInfo(Image.Gdal.Info(inputFileInfo), proj4String);
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
                throw new Exception($"Output directory isn't empty. Please, select another directory. Current path:{outputDirectoryInfo.FullName}.");
        }

        #endregion

        #endregion
    }
}
