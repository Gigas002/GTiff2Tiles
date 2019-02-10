using System;
using System.IO;
using System.Runtime.InteropServices;
using GTiff2Tiles.Core.Exceptions.Image;
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
        /// <param name="inputFileInfo">Object of <see cref="FileInfo"/> class, representing input file.</param>
        /// <param name="outputFileInfo">Object of <see cref="FileInfo"/> class, representing output file.</param>
        /// <param name="options">Array of <see cref="string"/> parameters.</param>
        /// <param name="callback"><see langword="delegate"/> for progress reporting from Gdal.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        public static void Warp(FileInfo inputFileInfo, FileInfo outputFileInfo,
                                string[] options,
                                OSGeo.GDAL.Gdal.GDALProgressFuncDelegate callback = null)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException($"Input file's path string is empty. Method: {nameof(Warp)}.");
            if (string.IsNullOrWhiteSpace(outputFileInfo.FullName))
                throw new GdalException($"Output file's path string is empty. Method: {nameof(Warp)}.");
            if (!inputFileInfo.Exists)
                throw new GdalException($"Input file isn't exist. Method: {nameof(Warp)}.");

            try
            {
                // ReSharper disable once PossibleNullReferenceException
                outputFileInfo.Directory.Create();
            }
            catch (Exception exception)
            {
                throw new
                    GdalException($"Unable to create output directory. Method: {nameof(Warp)}.", exception);
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
                    using (Dataset resultDataset = OSGeo.GDAL.Gdal.wrapper_GDALWarpDestName(outputFileInfo.FullName, 1,
                                                                                            gdalDatasetShadow,
                                                                                            new
                                                                                                GDALWarpAppOptions(options),
                                                                                            callback, string.Empty))
                    {
                        gcHandle.Free();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new GdalException($"Unable to complete {nameof(Warp)} method.", exception);
            }

            //Was file created?
            if (!outputFileInfo.Exists)
                throw new
                    GdalException($"{nameof(Warp)} method couldn't create ready file with this path: {outputFileInfo.FullName}.");
        }

        /// <summary>
        /// Runs GdalInfo with passed parameters.
        /// </summary>
        /// <param name="inputFileInfo">Object of <see cref="FileInfo"/> class, representing input file.</param>
        /// <param name="options">Array of <see cref="string"/> parameters for GdalInfo.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns><see cref="string"/> from GdalInfo.</returns>
        public static string Info(FileInfo inputFileInfo, string[] options = null)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException($"Input file's path string is empty. Method: {nameof(Info)}.");
            if (!inputFileInfo.Exists)
                throw new GdalException($"Input file isn't exist. Method: {nameof(Info)}.");

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly))
                {
                    string gdalInfoString = OSGeo.GDAL.Gdal.GDALInfo(inputDataset, new GDALInfoOptions(options));

                    if (string.IsNullOrWhiteSpace(gdalInfoString))
                        throw new GdalException("GdalInfo returned an empty string.");

                    return gdalInfoString;
                }
            }
            catch (Exception exception)
            {
                throw new GdalException($"Unable to complete {nameof(Info)} method.", exception);
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
        /// <param name="inputFileInfo">Object of <see cref="FileInfo"/> class, representing input file.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns>Array of <see cref="double"/> coordinates and pixel sizes.</returns>
        private static double[] GetGeoTransform(FileInfo inputFileInfo)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException($"Input file's path string is empty. Method: {nameof(GetGeoTransform)}.");
            if (!inputFileInfo.Exists)
                throw new GdalException($"Input file isn't exist. Method: {nameof(GetGeoTransform)}.");

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
                throw new GdalException($"Unable to complete {nameof(GetGeoTransform)} method.", exception);
            }
        }

        #endregion

        #region Public

        /// <summary>
        /// Gets proj4 string of input file.
        /// </summary>
        /// <param name="inputFileInfo">Object of <see cref="FileInfo"/> class, representing input file.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns><see cref="string"/> proj4.</returns>
        public static string GetProj4String(FileInfo inputFileInfo)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException($"Input file's path string is empty. Method: {nameof(GetProj4String)}.");
            if (!inputFileInfo.Exists)
                throw new GdalException($"Input file isn't exist. Method: {nameof(GetProj4String)}.");

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
                            throw new GdalException($"Proj4 string is empty. Method: {nameof(GetProj4String)}.");

                        return proj4String;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new GdalException($"Unable to complete {nameof(GetProj4String)} method.", exception);
            }
        }

        /// <summary>
        /// Gets the coordinates borders of the input Geotiff file.
        /// </summary>
        /// <param name="inputFileInfo">Object of <see cref="FileInfo"/> class, representing input file.</param>
        /// <param name="rasterXSize">Raster's width.</param>
        /// <param name="rasterYSize">Raster's height.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> with WGS84 coordinates.</returns>
        public static (double minX, double minY, double maxX, double maxY) GetFileBorders(FileInfo inputFileInfo, int rasterXSize, int rasterYSize)
        {
            #region Parameters checking

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException($"Input file's path string is empty. Method: {nameof(GetFileBorders)}.");
            if (!inputFileInfo.Exists)
                throw new GdalException($"Input file isn't exist. Method: {nameof(GetFileBorders)}.");
            if (rasterXSize < 0) throw new GdalException($"Raster x size is lesser than 0. Method: {nameof(GetFileBorders)}.");
            if (rasterYSize < 0) throw new GdalException($"Raster y size is lesser than 0. Method: {nameof(GetFileBorders)}.");

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
        /// <param name="inputFileInfo">Object of <see cref="FileInfo"/> class, representing input file.</param>
        /// <remarks>Throws <see cref="GdalException"/>.</remarks>
        /// <returns><see cref="ValueTuple{T1, T2}"/> with image sizes in pixels.</returns>
        public static (int rasterXSize, int rasterYSize) GetImageSizes(FileInfo inputFileInfo)
        {
            #region Parameters checking.

            if (string.IsNullOrWhiteSpace(inputFileInfo.FullName))
                throw new GdalException($"Input file's path string is empty. Method: {nameof(GetProj4String)}.");
            if (!inputFileInfo.Exists)
                throw new GdalException($"Input file isn't exist. Method: {nameof(GetProj4String)}.");

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
                throw new GdalException($"Unable to complete {nameof(GetImageSizes)} method.", exception);
            }
        }

        #endregion

        #endregion
    }
}
