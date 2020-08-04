#pragma warning disable CA1062 // Check args

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Helpers;
using OSGeo.GDAL;
using OSGeo.OSR;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeInternal

namespace GTiff2Tiles.Core
{
    /// <summary>
    /// Gdal's method to work with input files
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
        private static readonly string[] SrsEpsg4326 = { "-t_srs", "EPSG:4326" };

        /// <summary>
        /// -t_srs EPSG:3857
        /// </summary>
        private static readonly string[] SrsEpsg3857 = { "-t_srs", "EPSG:3857" };

        /// <summary>
        /// Options for GdalWarp to convert GeoTIFF's coordinate system
        /// <remarks><para/>Requires you to add target system param (-t_srs)</remarks>
        /// </summary>
        public static readonly string[] ConvertCoordinateSystemOptions =
        {
            "-overwrite", "-multi", "-srcnodata", "0", "-of", "GTiff", "-ot", "Byte"
            //"-co", "TILED=YES",
        };

        /// <summary>
        /// Name for temporary (converted) GeoTIFF
        /// </summary>
        public const string TempFileName = "tmp_converted";

        #endregion

        #endregion

        #region GdalApps

        /// <summary>
        /// Runs GdalWarp with passed parameters
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="outputFilePath">Output file's path</param>
        /// <param name="options">Array of string parameters</param>
        /// <param name="progress">GdalWarp's progress</param>
        /// <returns><see langword="true"/> if operation was sucessful;
        /// <para/><see langword="false"/> otherwise</returns>
        public static async ValueTask<bool> WarpAsync(string inputFilePath, string outputFilePath,
               string[] options, IProgress<double> progress = null)
        {
            #region Parameters checking

            if (!CheckHelper.CheckFile(inputFilePath, true)) return false;
            if (!CheckHelper.CheckFile(outputFilePath, false)) return false;
            if (!CheckHelper.CheckDirectory(Path.GetDirectoryName(outputFilePath))) return false;
            if (options == null || options.Length <= 0) return false;

            #endregion

            Gdal.GDALProgressFuncDelegate callback = GdalProgress;
            _gdalProgress = progress;

            // Initialize Gdal, if needed
            ConfigureGdal();

            await Task.Run(() =>
            {
                using Dataset inputDataset = Gdal.Open(inputFilePath, Access.GA_ReadOnly);

                GCHandle gcHandle = GCHandle.Alloc(new[] { Dataset.getCPtr(inputDataset).Handle },
                                                   GCHandleType.Pinned);
                SWIGTYPE_p_p_GDALDatasetShadow gdalDatasetShadow =
                    new SWIGTYPE_p_p_GDALDatasetShadow(gcHandle.AddrOfPinnedObject(), false, null);

                // ReSharper disable once UnusedVariable
                using Dataset resultDataset =
                    Gdal.wrapper_GDALWarpDestName(outputFilePath, 1, gdalDatasetShadow,
                                                  new GDALWarpAppOptions(options), callback, string.Empty);

                gcHandle.Free();
            }).ConfigureAwait(false);

            // Was file created?
            return CheckHelper.CheckFile(outputFilePath, true);
        }

        /// <summary>
        /// Runs GdalInfo with passed parameters
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="options">Array of string parameters for GdalInfo</param>
        /// <returns><see cref="string"/> from GdalInfo if everything OK;
        /// <para/><see langword="null"/> otherwise</returns>
        public static Task<string> InfoAsync(string inputFilePath, string[] options = null)
        {
            #region Parameters checking

            if (!CheckHelper.CheckFile(inputFilePath, true)) return null;

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
        public static void ConfigureGdal() => GdalHelper.ConfigureAll();

        #endregion

        #region Other methods

        #region Private

        /// <summary>
        /// Realistaion of <see cref="Gdal.GDALProgressFuncDelegate"/>
        /// </summary>
        /// <param name="complete">Current progress;
        /// <remarks><para/>values from 0.0 to 1.0</remarks></param>
        /// <param name="message"></param>
        /// <param name="data"></param>
        /// <returns>Always 1</returns>
        private static int GdalProgress(double complete, IntPtr message, IntPtr data)
        {
            _gdalProgress?.Report(complete * 100.0);

            return 1;
        }

        #endregion

        #region Public

        /// <summary>
        /// Gets proj string of input file
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <returns>Proj <see cref="string"/> if everything OK;
        /// <para/><see langword="null"/> otherwise</returns>
        public static Task<string> GetProjStringAsync(string inputFilePath)
        {
            #region Parameters checking

            if (!CheckHelper.CheckFile(inputFilePath, true)) return null;

            #endregion

            // Initialize Gdal, if needed
            ConfigureGdal();

            return Task.Run(() =>
            {
                using Dataset dataset = Gdal.Open(inputFilePath, Access.GA_ReadOnly);

                string wkt = dataset.GetProjection();

                using SpatialReference spatialReference = new SpatialReference(wkt);

                spatialReference.ExportToProj4(out string projString);

                return projString;

                // TODO: requires PROJ 6.2+
                //spatialReference.ExportToPROJJSON(out string argout, null);
            });
        }

        /// <summary>
        /// Converts current GeoTiff to a new file with target <see cref="CoordinateSystem"/>
        /// through GdalWarp
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <param name="outputFilePath">Output file's path</param>
        /// <param name="targetSystem">Target <see cref="CoordinateSystem"/></param>
        /// <param name="progress">GdalWarp's progress</param>
        /// <returns><see langword="true"/> if operation was sucessful;
        /// <para/><see langword="false"/> otherwise</returns>
        public static ValueTask<bool> ConvertGeoTiffToTargetSystemAsync(string inputFilePath,
            string outputFilePath, CoordinateSystem targetSystem, IProgress<double> progress = null)
        {
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
                        return ValueTask.FromResult(false);
                    }
            }

            return WarpAsync(inputFilePath, outputFilePath, gdalWarpOptions.ToArray(), progress);
        }

        /// <summary>
        /// Gets supported coordinate system from proj string of GeoTIFF
        /// </summary>
        /// <param name="projString">Proj string of input GeoTIFF</param>
        /// <returns>Input file's <see cref="CoordinateSystem"/></returns>
        public static CoordinateSystem GetCoordinateSystem(string projString)
        {
            if (string.IsNullOrWhiteSpace(projString)) throw new ArgumentNullException(nameof(projString));

            if (projString.Contains(Proj.ProjLongLat, StringComparison.InvariantCulture)
             && projString.Contains(Proj.DatumWgs84, StringComparison.InvariantCulture))
                return CoordinateSystem.Epsg4326;
            if (projString.Contains(Proj.ProjMerc, StringComparison.InvariantCulture)
             && !projString.Contains(Proj.DatumWgs84, StringComparison.InvariantCulture))
                return CoordinateSystem.Epsg3857;

            throw new GdalException();
        }

        /// <summary>
        /// Gets the coordinates and pixel sizes of image
        /// </summary>
        /// <param name="inputFilePath">Input GeoTiff's path</param>
        /// <returns>Array of double coordinates and pixel sizes if everything is OK;
        /// <para/><see langword="null"/> otherwise</returns>
        public static double[] GetGeoTransform(string inputFilePath)
        {
            #region Parameters checking

            if (!CheckHelper.CheckFile(inputFilePath, true)) return null;

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
        /// <param name="coordinateSystem">Desired coordinate system</param>
        /// <returns><see cref="GeoCoordinate"/>s of this image borders if everything is OK;
        /// <para/>(<see langword="null"/>, <see langword="null"/>) otherwise</returns>
        public static (GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate) GetImageBorders(
            string inputFilePath, Size size, CoordinateSystem coordinateSystem)
        {
            #region Parameters checking

            if (!CheckHelper.CheckFile(inputFilePath, true)) return (null, null);
            if (size == null) return (null, null);

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
                        GeodeticCoordinate minCoordinate = new GeodeticCoordinate(minX, minY);
                        GeodeticCoordinate maxCoordinate = new GeodeticCoordinate(maxX, maxY);

                        return (minCoordinate, maxCoordinate);
                    }
                case CoordinateSystem.Epsg3857:
                    {
                        MercatorCoordinate minCoordinate = new MercatorCoordinate(minX, minY);
                        MercatorCoordinate maxCoordinate = new MercatorCoordinate(maxX, maxY);

                        return (minCoordinate, maxCoordinate);
                    }
                default: return (null, null);
            }
        }

        #endregion

        #endregion
    }
}

#pragma warning restore CA1062 // Check args
