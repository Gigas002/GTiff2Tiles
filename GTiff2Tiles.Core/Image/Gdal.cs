using System;
using System.IO;
using System.Runtime.InteropServices;
using GTiff2Tiles.Core.Exceptions.Image;
using GTiff2Tiles.Core.Localization;
using OSGeo.GDAL;
using OSGeo.OSR;

namespace GTiff2Tiles.Core.Image
{
    /// <summary>
    /// Gdal's method to work with input files.
    /// </summary>
    public static class Gdal
    {
        #region GdalApps

        /// <summary>
        /// Runs GdalWarp with passed parameters.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <param name="outputFileInfo">Output file.</param>
        /// <param name="options">Array of string parameters.</param>
        /// <param name="callback">Delegate for progress reporting from Gdal.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        public static void Warp(FileInfo inputFileInfo, FileInfo outputFileInfo,
                                string[] options,
                                OSGeo.GDAL.Gdal.GDALProgressFuncDelegate callback = null)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(inputFileInfo),
                                                      $"{nameof(Gdal)}.{nameof(Warp)}"));
            if (string.IsNullOrWhiteSpace(outputFileInfo.FullName))
                throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(outputFileInfo),
                                                      $"{nameof(Gdal)}.{nameof(Warp)}"));
            if (!inputFileInfo.Exists)
                throw new GdalException(string.Format(Strings.IsntExist, nameof(inputFileInfo), inputFileInfo.FullName,
                                                      $"{nameof(Gdal)}.{nameof(Warp)}"));

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                outputFileInfo.Directory.Create();
            }
            catch (Exception exception)
            {
                throw new
                    GdalException(string.Format(Strings.UnableToCreate, nameof(outputFileInfo.Directory), outputFileInfo.DirectoryName,
                                                $"{nameof(Gdal)}.{nameof(Warp)}"), exception);
            }

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly))
                {
                    GCHandle gcHandle =
                        GCHandle.Alloc(new[] { Dataset.getCPtr(inputDataset).Handle }, GCHandleType.Pinned);
                    SWIGTYPE_p_p_GDALDatasetShadow gdalDatasetShadow =
                        new SWIGTYPE_p_p_GDALDatasetShadow(gcHandle.AddrOfPinnedObject(), false, null);
                    // ReSharper disable once UnusedVariable
                    using (Dataset resultDataset =
                        OSGeo.GDAL.Gdal.wrapper_GDALWarpDestName(outputFileInfo.FullName, 1,
                                                                 gdalDatasetShadow,
                                                                 new GDALWarpAppOptions(options),
                                                                 callback, string.Empty))
                    {
                        gcHandle.Free();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new GdalException(string.Format(Strings.UnableToComplete, $"{nameof(Gdal)}.{nameof(Warp)}"), exception);
            }

            //Was file created?
            if (!outputFileInfo.Exists)
                throw new
                    GdalException(string.Format(Strings.UnableToCreate, nameof(outputFileInfo), outputFileInfo.FullName,
                                                $"{nameof(Gdal)}.{nameof(Warp)}"));
        }

        /// <summary>
        /// Runs GdalInfo with passed parameters.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <param name="options">Array of string parameters for GdalInfo.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns><see cref="string"/> from GdalInfo.</returns>
        public static string Info(FileInfo inputFileInfo, string[] options = null)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(inputFileInfo),
                                                      $"{nameof(Gdal)}.{nameof(Info)}"));
            if (!inputFileInfo.Exists)
                throw new GdalException(string.Format(Strings.IsntExist, nameof(inputFileInfo), inputFileInfo.FullName,
                                                      $"{nameof(Gdal)}.{nameof(Info)}"));

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly))
                {
                    string gdalInfoString = OSGeo.GDAL.Gdal.GDALInfo(inputDataset, new GDALInfoOptions(options));

                    if (string.IsNullOrWhiteSpace(gdalInfoString))
                        throw new GdalException(string.Format(Strings.StringIsEmpty, "GdalInfo",
                                                              $"{nameof(Gdal)}.{nameof(Info)}"));

                    return gdalInfoString;
                }
            }
            catch (Exception exception)
            {
                throw new GdalException(string.Format(Strings.UnableToComplete, $"{nameof(Gdal)}.{nameof(Warp)}"), exception);
            }
        }

        #endregion

        #region Initialize Gdal/Ogr

        /// <summary>
        /// Initialize Gdal, if it hadn't been initialized yet.
        /// </summary>
        private static void ConfigureGdal()
        {
            if (!Helpers.GdalHelper.Usable)
                Helpers.GdalHelper.Initialize();
            if (Helpers.GdalHelper.IsGdalConfigured) return;
            Helpers.GdalHelper.ConfigureGdal();
        }

        /// <summary>
        /// Initialize Ogr, if it hadn't been initialized yet.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static void ConfigureOgr()
        {
            if (!Helpers.GdalHelper.Usable)
                Helpers.GdalHelper.Initialize();
            if (Helpers.GdalHelper.IsOgrConfigured) return;
            Helpers.GdalHelper.ConfigureOgr();
        }

        #endregion

        #region Image

        #region Private

        /// <summary>
        /// Gets the coordinates and pixel sizes of image.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns>Array of double coordinates and pixel sizes.</returns>
        private static double[] GetGeoTransform(FileInfo inputFileInfo)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(inputFileInfo),
                                                      $"{nameof(Gdal)}.{nameof(GetGeoTransform)}"));
            if (!inputFileInfo.Exists)
                throw new GdalException(string.Format(Strings.IsntExist, nameof(inputFileInfo), inputFileInfo.FullName,
                                                      $"{nameof(Gdal)}.{nameof(GetGeoTransform)}"));

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly))
                {
                    double[] geoTransform = new double[6];
                    inputDataset.GetGeoTransform(geoTransform);
                    return geoTransform;
                }
            }
            catch (Exception exception)
            {
                throw new GdalException(string.Format(Strings.UnableToComplete, $"{nameof(Gdal)}.{nameof(GetGeoTransform)}"), exception);
            }
        }

        #endregion

        #region Internal

        /// <summary>
        /// Gets proj4 string of input file.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns>Proj4 string.</returns>
        internal static string GetProj4String(FileInfo inputFileInfo)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(inputFileInfo),
                                                      $"{nameof(Gdal)}.{nameof(GetProj4String)}"));
            if (!inputFileInfo.Exists)
                throw new GdalException(string.Format(Strings.IsntExist, nameof(inputFileInfo), inputFileInfo.FullName,
                                                      $"{nameof(Gdal)}.{nameof(GetProj4String)}"));

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            try
            {
                using (Dataset dataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly))
                {
                    string wkt = dataset.GetProjection();
                    using (SpatialReference spatialReference = new SpatialReference(wkt))
                    {
                        spatialReference.ExportToProj4(out string proj4String);

                        if (string.IsNullOrWhiteSpace(proj4String))
                            throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(proj4String),
                                                                  $"{nameof(Gdal)}.{nameof(GetProj4String)}"));

                        return proj4String;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new GdalException(string.Format(Strings.UnableToComplete, $"{nameof(Gdal)}.{nameof(GetProj4String)}"), exception);
            }
        }

        /// <summary>
        /// Gets the coordinates borders of the input Geotiff file.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <param name="rasterXSize">Raster's width.</param>
        /// <param name="rasterYSize">Raster's height.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> with WGS84 coordinates.</returns>
        internal static (double minX, double minY, double maxX, double maxY) GetImageBorders(FileInfo inputFileInfo, int rasterXSize, int rasterYSize)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(inputFileInfo),
                                                      $"{nameof(Gdal)}.{nameof(GetImageBorders)}"));
            if (!inputFileInfo.Exists)
                throw new GdalException(string.Format(Strings.IsntExist, nameof(inputFileInfo), inputFileInfo.FullName,
                                                      $"{nameof(Gdal)}.{nameof(GetImageBorders)}"));
            if (rasterXSize < 0)
                throw new GdalException(string.Format(Strings.LesserThan, nameof(rasterXSize), 0,
                                                      $"{nameof(Gdal)}.{nameof(GetImageBorders)}"));
            if (rasterYSize < 0) throw new GdalException(string.Format(Strings.LesserThan, nameof(rasterYSize), 0,
                                                                       $"{nameof(Gdal)}.{nameof(GetImageBorders)}"));

            #endregion

            double[] geoTransform = GetGeoTransform(inputFileInfo);

            double minX = geoTransform[0];
            double minY = geoTransform[3] - rasterYSize * geoTransform[1];
            double maxX = geoTransform[0] + rasterXSize * geoTransform[1];
            double maxY = geoTransform[3];

            return (minX, minY, maxX, maxY);
        }

        /// <summary>
        /// Gets image sizes in pixels.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns><see cref="ValueTuple{T1, T2}"/> with image sizes in pixels.</returns>
        internal static (int rasterXSize, int rasterYSize) GetImageSizes(FileInfo inputFileInfo)
        {
            #region Parameters checking.

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(inputFileInfo),
                                                      $"{nameof(Gdal)}.{nameof(GetProj4String)}"));
            if (!inputFileInfo.Exists)
                throw new GdalException(string.Format(Strings.IsntExist, nameof(inputFileInfo), inputFileInfo.FullName,
                                                      $"{nameof(Gdal)}.{nameof(GetProj4String)}"));

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly))
                {
                    return (inputDataset.RasterXSize, inputDataset.RasterYSize);
                }
            }
            catch (Exception exception)
            {
                throw new GdalException(string.Format(Strings.UnableToComplete, $"{nameof(Gdal)}.{nameof(GetProj4String)}"), exception);
            }
        }

        #endregion

        #endregion
    }
}
