using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Localization;

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Class with static methods to check for errors
    /// </summary>
    public static class CheckHelper
    {
        #region Methods

        #region Internal

        /// <summary>
        /// Checks, if file's path is not empty string and file exists, if it should
        /// </summary>
        /// <param name="fileInfo">File to check</param>
        /// <param name="shouldExist">Should it exist?</param>
        /// <param name="fileExtension">Checks file extension</param>
        internal static void CheckFile(FileInfo fileInfo, bool shouldExist, string fileExtension = null)
        {
            // Update file state 
            fileInfo.Refresh();

            // Check file's path 
            if (string.IsNullOrWhiteSpace(fileInfo.FullName))
                throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                                  Strings.StringIsEmpty, nameof(fileInfo.FullName)));

            // Check file's extension
            if (!string.IsNullOrWhiteSpace(fileExtension))
                if (fileInfo.Extension != fileExtension)
                    throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                                      Strings.WrongExtension, nameof(fileInfo), fileExtension,
                                                      fileInfo.FullName));

            // Check file's existance
            if (shouldExist)
            {
                if (!fileInfo.Exists)
                    throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                                      Strings.DoesntExist, nameof(fileInfo), fileInfo.FullName));
            }
            else if (fileInfo.Exists)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                                  Strings.AlreadyExist, nameof(fileInfo), fileInfo.FullName));
            }
        }

        #endregion

        #region Public

        /// <summary>
        /// Checks, if directory's path is not empty, creates directory if it doesn't exist
        /// and checks if it's empty or not
        /// </summary>
        /// <param name="directoryInfo">Directory to check</param>
        /// <param name="shouldBeEmpty">Should directory be empty?
        /// <para/>If set <see keyword="null"/>, emptyness doesn't check</param>
        public static void CheckDirectory(DirectoryInfo directoryInfo, bool? shouldBeEmpty = null)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));

            // Check directory's path
            if (string.IsNullOrWhiteSpace(directoryInfo.FullName))
                throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                                  Strings.StringIsEmpty, nameof(directoryInfo.FullName)));

            // Try to create directory
            try
            {
                directoryInfo.Create();
                directoryInfo.Refresh();
            }
            catch (Exception exception)
            {
                throw new
                    Exception(string.Format(CultureInfo.InvariantCulture,
                                            Strings.UnableToCreate, nameof(directoryInfo), directoryInfo.FullName),
                              exception);
            }

            // Check directory's emptyness
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (shouldBeEmpty == true)
            {
                if (directoryInfo.EnumerateFileSystemInfos().Any())
                    throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                                      Strings.DirectoryIsntEmpty, directoryInfo.FullName));
            }
            else if (shouldBeEmpty == false)
            {
                if (!directoryInfo.EnumerateFileSystemInfos().Any())
                    throw new Exception(string.Format(CultureInfo.InvariantCulture,
                                                      Strings.DirectoryIsEmpty, directoryInfo.FullName));
            }
        }

        /// <summary>
        /// Checks the existance, projection and type
        /// </summary>
        /// <param name="inputFileInfo">Input geotiff</param>
        /// <param name="targetSystem">Target coordinate system</param>
        /// <returns><see langword="true"/> if file needs to be converted;
        /// <para/><see langword="false"/> otherwise</returns>
        public static async ValueTask<bool> CheckInputFileAsync(FileInfo inputFileInfo, CoordinateSystems targetSystem)
        {
            CheckFile(inputFileInfo, true, FileExtensions.Tif);

            // Get proj and gdalInfo strings
            string projString = await GdalWorker.GetProjStringAsync(inputFileInfo).ConfigureAwait(false);
            string gdalInfoString = await GdalWorker.InfoAsync(inputFileInfo).ConfigureAwait(false);
            CoordinateSystems inputSystem = GdalWorker.GetCoordinateSystem(projString);

            // Check if input image is ready for cropping
            return inputSystem == targetSystem && gdalInfoString.Contains(GdalWorker.Byte,
                                                                          StringComparison.InvariantCulture);
        }

        #endregion

        #endregion
    }
}
