using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Class with static methods to check for errors
    /// </summary>
    public static class CheckHelper
    {
        #region Methods

        #region Public

        /// <summary>
        /// Checks, if file's path is not empty string and file exists, if it should
        /// </summary>
        /// <param name="filePath">File's path to check</param>
        /// <param name="shouldExist">Should it exist?</param>
        /// <param name="fileExtension">Checks file extension</param>
        /// <returns><see langword="true"/> if everything is OK;
        /// <para/><see langword="false"/> otherwise</returns>
        public static bool CheckFile(string filePath, bool shouldExist, string fileExtension = null)
        {
            // Check file path
            if (string.IsNullOrWhiteSpace(filePath)) return false;

            // Check file extension
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                var actualExtension = Path.GetExtension(filePath);

                if (actualExtension != fileExtension) return false;
            }

            // Check file's existance
            bool existance = File.Exists(filePath);

            switch (existance)
            {
                case true when !shouldExist:
                case false when shouldExist:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks, if directory's path is not empty, creates directory if it doesn't exist
        /// and checks if it's empty or not
        /// </summary>
        /// <param name="directoryPath">Directory's path to check</param>
        /// <param name="shouldBeEmpty">Should directory be empty?
        /// <remarks><para/>If set <see keyword="null"/>, emptyness doesn't check</remarks></param>
        /// <returns><see langword="true"/> if everything is OK;
        /// <para/><see langword="false"/> otherwise</returns>
        public static bool CheckDirectory(string directoryPath, bool? shouldBeEmpty = null)
        {
            // Check directory's path
            if (string.IsNullOrWhiteSpace(directoryPath)) return false;

            // Try to create directory
            DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);

            // Check directory's emptyness
            bool containsAny = directoryInfo.EnumerateFileSystemInfos().Any();

            switch (shouldBeEmpty)
            {
                case true when !containsAny:
                case false when containsAny:
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks the existance, projection and type
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="targetSystem">Target coordinate system</param>
        /// <returns><see langword="true"/> if file needs to be converted;
        /// <para/><see langword="false"/> otherwise</returns>
        public static async ValueTask<bool> CheckInputFileAsync(string inputFilePath, CoordinateSystems targetSystem)
        {
            CheckFile(inputFilePath, true, FileExtensions.Tif);

            // Get proj and gdalInfo strings
            string projString = await GdalWorker.GetProjStringAsync(inputFilePath).ConfigureAwait(false);
            string gdalInfoString = await GdalWorker.InfoAsync(inputFilePath).ConfigureAwait(false);
            CoordinateSystems inputSystem = GdalWorker.GetCoordinateSystem(projString);

            // Check if input image is ready for cropping
            return inputSystem == targetSystem && gdalInfoString.Contains(GdalWorker.Byte,
                                                                          StringComparison.InvariantCulture);
        }

        #endregion

        #endregion
    }
}
