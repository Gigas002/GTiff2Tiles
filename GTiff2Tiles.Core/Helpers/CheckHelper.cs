using System;
using System.IO;
using System.Linq;
using GTiff2Tiles.Core.Localization;

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
            if (string.IsNullOrWhiteSpace(gdalInfoString))
                throw new Exception(string.Format(Strings.StringIsEmpty, "GdalInfo", $"{nameof(CheckHelper)}.{nameof(CheckTifInfo)}"));

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
            if (!inputFileInfo.Exists)
                throw new Exception(string.Format(Strings.IsntExist, nameof(inputFileInfo), inputFileInfo.FullName,
                                                  $"{nameof(CheckHelper)}.{nameof(CheckInputFile)}"));

            //Check if input file is not .tif.
            if (inputFileInfo.Extension != Enums.Extensions.Tif)
                throw new Exception(string.Format(Strings.ExtensionIsnt, nameof(inputFileInfo), Enums.Extensions.Tif,
                                                  inputFileInfo.FullName, $"{nameof(CheckHelper)}.{nameof(CheckInputFile)}"));

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
                throw new Exception(string.Format(Strings.UnableToCreate, nameof(outputDirectoryInfo),
                                                  outputDirectoryInfo.FullName, $"{nameof(CheckHelper)}.{nameof(CheckOutputDirectory)}"),
                                    exception);
            }

            if (outputDirectoryInfo.EnumerateFileSystemInfos().Any())
                throw new Exception(string.Format(Strings.DirectoryIsntEmpty, nameof(outputDirectoryInfo),
                                                  outputDirectoryInfo.FullName, $"{nameof(CheckHelper)}.{nameof(CheckOutputDirectory)}"));
        }

        #endregion

        #endregion
    }
}
