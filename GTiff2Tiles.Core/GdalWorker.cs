using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;
using MaxRev.Gdal.Core;
using OSGeo.GDAL;
using OSGeo.OSR;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeInternal

namespace GTiff2Tiles.Core
{
    /// <summary>
    /// Gdal's methods to work with input files
    /// </summary>
    public static class GdalWorker
    {
        #region Properties/Constants

        /// <summary>
        /// Progress to report inside <see cref="GdalProgress"/>
        /// </summary>
        private static IProgress<double> _gdalProgress;

        #region GdalInfo

        /// <summary>
        /// Type of bands
        /// </summary>
        internal const string Byte = "Type=Byte";

        #endregion

        #region GdalWarp

        /// <summary>
        /// -t_srs EPSG:4326
        /// </summary>
        public static readonly string[] SrsEpsg4326 = { "-t_srs", "EPSG:4326" };

        /// <summary>
        /// -t_srs EPSG:3857
        /// </summary>
        public static readonly string[] SrsEpsg3857 = { "-t_srs", "EPSG:3857" };

        /// <summary>
        /// Options for GdalWarp to convert GeoTiff's coordinate system;
        /// <remarks><para/>Requires you to add target system param (-t_srs).
        /// Included default args: <code>-overwrite -multi -srcnodata 0
        /// -of GTiff -ot Byte</code></remarks>
        /// </summary>
        public static readonly string[] ConvertCoordinateSystemOptions =
        {
            "-overwrite", "-multi", "-srcnodata", "0", "-of", "GTiff", "-ot", "Byte"
            // TODO: test performance with TILED
            //"-co", "TILED=YES",
        };

        /// <summary>
        /// Name for temporary (converted) GeoTiff
        /// <remarks><para/>Includes .tif extension,
        /// looks like: _tmp_converted.tif</remarks>
        /// </summary>
        public static readonly string TempFileName = $"_tmp_converted{FileExtensions.Tif}";

        #endregion

        #endregion

        #region GdalApps

        /// <summary>
        /// Runs GdalWarp with passed parameters
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="outputFilePath">Output file's path</param>
        /// <param name="options">Array of string parameters
        /// <remarks><para/>See <see cref="ConvertCoordinateSystemOptions"/> field for
        /// more info</remarks></param>
        /// <param name="progress">GdalWarp's progress
        /// <remarks><para/><see langword="null"/> by default</remarks></param>
        /// <exception cref="ArgumentNullException"/>
        public static Task WarpAsync(string inputFilePath, string outputFilePath,
                                     string[] options, IProgress<double> progress = null)
        {
            #region Preconditions checks

            CheckHelper.CheckFile(inputFilePath);
            CheckHelper.CheckFile(outputFilePath, false);
            CheckHelper.CheckDirectory(Path.GetDirectoryName(outputFilePath));

            if (options == null || options.Length <= 0) throw new ArgumentNullException(nameof(options));

            #endregion

            Gdal.GDALProgressFuncDelegate callback = GdalProgress;
            _gdalProgress = progress;

            // Initialize Gdal, if needed
            ConfigureGdal();

            return Task.Run(() =>
            {
                using Dataset inputDataset = Gdal.Open(inputFilePath, Access.GA_ReadOnly);

                using Dataset resultDataset = Gdal.Warp(outputFilePath, new[] { inputDataset },
                                                        new GDALWarpAppOptions(options), callback, string.Empty);
            });
        }

        /// <summary>
        /// Runs GdalInfo with passed parameters
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="options">Array of string parameters for GdalInfo
        /// <remarks><para/><see langword="null"/> by default</remarks></param>
        /// <returns><see cref="string"/> from GdalInfo if everything OK;
        /// <para/><see langword="null"/> otherwise</returns>
        public static Task<string> InfoAsync(string inputFilePath, string[] options = null)
        {
            #region Preconditions checks

            CheckHelper.CheckFile(inputFilePath);

            #endregion

            // Initialize Gdal, if needed
            ConfigureGdal();

            return Task.Run(() =>
            {
                using Dataset inputDataset = Gdal.Open(inputFilePath, Access.GA_ReadOnly);

                return Gdal.GDALInfo(inputDataset, new GDALInfoOptions(options));
            });
        }

        #endregion

        #region Initialize Gdal/Ogr

        /// <summary>
        /// Initialize Gdal, if it hadn't been initialized yet
        /// </summary>
        public static void ConfigureGdal() => GdalBase.ConfigureAll();

        #endregion

        #region Other methods

        #region Private

        /// <summary>
        /// Implementation of <see cref="Gdal.GDALProgressFuncDelegate"/>
        /// </summary>
        /// <param name="complete">Current progress;
        /// <remarks><para/>values in range from 0.0 to 1.0</remarks></param>
        /// <param name="message">I actually don't know what that pointer means</param>
        /// <param name="data">I actually don't know what that pointer means</param>
        /// <returns>Always 1</returns>
        /// <exception cref="ArgumentOutOfRangeException"/>
        private static int GdalProgress(double complete, IntPtr message, IntPtr data)
        {
            #region Preconditions checks

            if (complete < 0.0 || complete > 1.0) throw new ArgumentOutOfRangeException(nameof(complete));

            #endregion

            // It's safe not to use progress-reporter for gdal
            _gdalProgress?.Report(complete * 100.0);

            return 1;
        }

        #endregion

        #region Public

        /// <inheritdoc cref="GetProjStringAsync"/>
        public static string GetProjString(string inputFilePath)
        {
            #region Preconditions checks

            CheckHelper.CheckFile(inputFilePath);

            #endregion

            // Initialize Gdal, if needed
            ConfigureGdal();

            using Dataset dataset = Gdal.Open(inputFilePath, Access.GA_ReadOnly);

            string wkt = dataset.GetProjection();

            using SpatialReference spatialReference = new(wkt);

            spatialReference.ExportToProj4(out string projString);

            // Alternative way -- needs using System.Text.Json to deserialize
            //spatialReference.ExportToPROJJSON(out string argout, null);

            return projString;
        }

        /// <summary>
        /// Gets proj <see cref="string"/> of input file
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <returns>Proj <see cref="string"/> if everything OK;
        /// <para/><see langword="null"/> otherwise</returns>
        public static Task<string> GetProjStringAsync(string inputFilePath) => Task.Run(() => GetProjString(inputFilePath));

        /// <summary>
        /// Converts current GeoTiff to a new GeoTiff with target <see cref="CoordinateSystem"/>
        /// through GdalWarp
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="outputFilePath">Output GeoTiff's path</param>
        /// <param name="targetSystem">Target <see cref="CoordinateSystem"/></param>
        /// <param name="progress">GdalWarp's progress
        /// <remarks><para/><see langword="null"/> by default</remarks></param>
        /// <exception cref="NotSupportedException"/>
        public static Task ConvertGeoTiffToTargetSystemAsync(string inputFilePath,
            string outputFilePath, CoordinateSystem targetSystem, IProgress<double> progress = null)
        {
            // Preconditions checks are done in WarpAsync method, no need to add them here too

            // Don't use hashset
            List<string> gdalWarpOptions = ConvertCoordinateSystemOptions.ToList();

            switch (targetSystem)
            {
                case CoordinateSystem.Epsg4326:
                {
                    gdalWarpOptions.AddRange(SrsEpsg4326);

                    break;
                }
                case CoordinateSystem.Epsg3857:
                {
                    gdalWarpOptions.AddRange(SrsEpsg3857);

                    break;
                }
                default:
                {
                    string err = string.Format(Strings.Culture, Strings.NotSupported, targetSystem);

                    throw new NotSupportedException(err);
                }
            }

            return WarpAsync(inputFilePath, outputFilePath, gdalWarpOptions.ToArray(), progress);
        }

        /// <summary>
        /// Gets supported coordinate system from proj string of GeoTiff
        /// </summary>
        /// <param name="projString">Proj string of input GeoTiff</param>
        /// <returns>Input file's <see cref="CoordinateSystem"/></returns>
        /// <exception cref="ArgumentNullException"/>
        public static CoordinateSystem GetCoordinateSystem(string projString)
        {
            #region Preconditions checks

            if (string.IsNullOrWhiteSpace(projString)) throw new ArgumentNullException(nameof(projString));

            #endregion

            bool isWgs84 = projString.Contains(Proj.DatumWgs84, StringComparison.InvariantCulture);
            bool isLongLat = projString.Contains(Proj.ProjLongLat, StringComparison.InvariantCulture);
            bool isMerc = projString.Contains(Proj.ProjMerc, StringComparison.InvariantCulture);

            return isWgs84 switch
            {
                true when isLongLat => CoordinateSystem.Epsg4326,
                false when isMerc => CoordinateSystem.Epsg3857,
                _ => CoordinateSystem.Other
            };
        }

        /// <summary>
        /// Gets the coordinates and pixel sizes of image
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <returns>Array of double coordinates and pixel sizes if everything is OK;
        /// <para/><see langword="null"/> otherwise</returns>
        public static double[] GetGeoTransform(string inputFilePath)
        {
            #region Preconditions checks

            CheckHelper.CheckFile(inputFilePath);

            #endregion

            // Initialize Gdal, if needed
            ConfigureGdal();

            using Dataset inputDataset = Gdal.Open(inputFilePath, Access.GA_ReadOnly);

            double[] geoTransform = new double[6];
            inputDataset.GetGeoTransform(geoTransform);

            return geoTransform;
        }

        /// <summary>
        /// Gets the coordinates borders of the input Geotiff file
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="size">Image's <see cref="Size"/>s</param>
        /// <param name="coordinateSystem">Image's coordinate system</param>
        /// <returns><see cref="ValueTuple{T1,T2}"/> of
        /// <see cref="GeoCoordinate"/>s of image's borders</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"/>
        public static (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) GetImageBorders(
            string inputFilePath, Size size, CoordinateSystem coordinateSystem)
        {
            #region Preconditions checks

            // File is checked inside GeteGeoTransform method, no need to check it here

            if (size == null) throw new ArgumentNullException(nameof(size));

            #endregion

            double[] geoTransform = GetGeoTransform(inputFilePath);

            double minX = geoTransform[0];
            double minY = geoTransform[3] - size.Height * geoTransform[1];
            double maxX = geoTransform[0] + size.Width * geoTransform[1];
            double maxY = geoTransform[3];

            switch (coordinateSystem)
            {
                case CoordinateSystem.Epsg4326:
                {
                    GeodeticCoordinate minCoordinate = new(minX, minY);
                    GeodeticCoordinate maxCoordinate = new(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
                case CoordinateSystem.Epsg3857:
                {
                    MercatorCoordinate minCoordinate = new(minX, minY);
                    MercatorCoordinate maxCoordinate = new(maxX, maxY);

                    return (minCoordinate, maxCoordinate);
                }
                default:
                {
                    string err = string.Format(Strings.Culture, Strings.NotSupported, coordinateSystem);

                    throw new NotSupportedException(err);
                }
            }
        }

        #endregion

        #endregion
    }
}
