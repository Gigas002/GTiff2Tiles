#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions;

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
        /// <param name="shouldExist">Should the file exist?
        /// <remarks><para/><see langword="true"/> by default;
        /// <para/>set this to <see langword="null"/> if you don't know or care if file's already exists</remarks></param>
        /// <param name="fileExtension">Checks file extension
        /// <remarks><para/>If set to <see keyword="null"/>, extension doesn't check</remarks></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="FileNotFoundException"/>
        /// <exception cref="FileException"/>
        public static void CheckFile(string? filePath, bool? shouldExist = true, string? fileExtension = null)
        {
            // Check file path
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));

            // Check file extension
            if (!string.IsNullOrWhiteSpace(fileExtension))
            {
                string? actualExtension = Path.GetExtension(filePath);

                if (actualExtension != fileExtension) throw new ArgumentException($"Expected {fileExtension}, was {actualExtension}", nameof(fileExtension));
            }

            // Check file's existance
            bool existance = File.Exists(filePath);

            switch (shouldExist)
            {
                case true when !existance:
                    throw new FileNotFoundException("File doesn't exist", filePath);
                case false when existance:
                    throw new FileException("File already exists");
            }
        }

        /// <summary>
        /// Checks, if directory's path is not empty, creates directory if it doesn't exist
        /// and checks if it's empty or not
        /// </summary>
        /// <param name="directoryPath">Directory's path to check</param>
        /// <param name="shouldBeEmpty">Should directory be empty?
        /// <remarks><para/>If set <see keyword="null"/>, emptyness doesn't check</remarks></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="DirectoryException"/>
        public static void CheckDirectory(string? directoryPath, bool? shouldBeEmpty = null)
        {
            // Check directory's path
            if (string.IsNullOrWhiteSpace(directoryPath)) throw new ArgumentNullException(nameof(directoryPath));

            // Try to create directory
            DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);

            // Check directory's emptyness
            bool containsAny = directoryInfo.EnumerateFileSystemInfos().Any();

            switch (shouldBeEmpty)
            {
                case true when containsAny:
                    throw new DirectoryException($"Directory should be empty: {shouldBeEmpty}, actual status: {containsAny}");
                case false when !containsAny:
                    throw new DirectoryException($"Directory should be empty: {shouldBeEmpty}, actual status: {containsAny}");
            }
        }

        /// <summary>
        /// Checks the existance, projection and type
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="targetSystem">Target coordinate system</param>
        /// <returns><see langword="true"/> if file needs to be converted;
        /// <para/><see langword="false"/> otherwise</returns>
        public static async ValueTask<bool> CheckInputFileAsync(string inputFilePath, CoordinateSystem targetSystem)
        {
            // File's path checked in other methods, so checking it here is not necessary

            // Get proj and gdalInfo strings
            string projString = await GdalWorker.GetProjStringAsync(inputFilePath).ConfigureAwait(false);
            CoordinateSystem inputSystem = GdalWorker.GetCoordinateSystem(projString);
            string gdalInfoString = await GdalWorker.InfoAsync(inputFilePath).ConfigureAwait(false);

            // Check if input image is ready for cropping
            return inputSystem == targetSystem &&
                   gdalInfoString.Contains(GdalWorker.Byte, StringComparison.InvariantCulture);
        }

        #endregion

        #endregion
    }
}
