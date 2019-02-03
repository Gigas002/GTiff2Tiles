using System;
using System.IO;
using System.Runtime.InteropServices;
using OSGeo.GDAL;

namespace GTiff2Tiles.Core.Image
{
    /// <summary>
    /// Gdal's method to work with input files.
    /// </summary>
    public static class Gdal
    {
        #region GdalApps

        /// <summary>
        /// GdalInfo.
        /// </summary>
        /// <param name="inputFilePath">Full path to image.</param>
        /// <param name="options">Options array.</param>
        /// <returns>String from GdalInfo.</returns>
        private static string Info(string inputFilePath, string[] options)
        {
            string gdalInfoString;
            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFilePath, Access.GA_ReadOnly))
                {
                    gdalInfoString = OSGeo.GDAL.Gdal.GDALInfo(inputDataset, new GDALInfoOptions(options));
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get GdalInfo string.", exception);
            }

            return string.IsNullOrWhiteSpace(gdalInfoString)
                       ? throw new Exception("GdalInfo returned an empty string.")
                       : gdalInfoString;
        }

        /// <summary>
        /// Runs GdalWarp.
        /// </summary>
        /// <param name="inputFilePath">Full path to input file.</param>
        /// <param name="outputFilePath">Full path to output file.</param>
        /// <param name="options">Options.</param>
        /// <param name="callback">Progress reporting delegate.</param>
        private static void Warp(string inputFilePath,
                                 string outputFilePath,
                                 string[] options,
                                 OSGeo.GDAL.Gdal.GDALProgressFuncDelegate callback)
        {
            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFilePath, Access.GA_ReadOnly))
                {
                    GCHandle gcHandle =
                        GCHandle.Alloc(new[] {Dataset.getCPtr(inputDataset).Handle}, GCHandleType.Pinned);
                    SWIGTYPE_p_p_GDALDatasetShadow gdalDatasetShadow =
                        new SWIGTYPE_p_p_GDALDatasetShadow(gcHandle.AddrOfPinnedObject(), false, null);
                    // ReSharper disable once UnusedVariable
                    using (Dataset resultDataset = OSGeo.GDAL.Gdal.wrapper_GDALWarpDestName(outputFilePath, 1,
                                                                                            gdalDatasetShadow,
                                                                                            new GDALWarpAppOptions(
                                                                                                                   options),
                                                                                            callback, string.Empty))
                    {
                        gcHandle.Free();
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to run GdalWarp.", exception);
            }

            if (!File.Exists(outputFilePath))
                throw new Exception("GdalWarp couldn't create fixed file.");
        }

        #endregion

        #region Initialize Gdal/Ogr

        /// <summary>
        /// Initialize Gdal, if it hadn't been initialized yet.
        /// </summary>
        public static void ConfigureGdal()
        {
            if (!Helpers.GdalHelper.Usable)
                Helpers.GdalHelper.Initialize();
            if (Helpers.GdalHelper.IsGdalConfigured) return;
            Helpers.GdalHelper.ConfigureGdal();
        }

        /// <summary>
        /// Initialize Ogr, if it hadn't been initialized yet.
        /// </summary>
        public static void ConfigureOgr()
        {
            if (!Helpers.GdalHelper.Usable)
                Helpers.GdalHelper.Initialize();
            if (Helpers.GdalHelper.IsOgrConfigured) return;
            Helpers.GdalHelper.ConfigureOgr();
        }

        #endregion

        #region Image

        /// <summary>
        /// Gets the information about image.
        /// </summary>
        /// <param name="inputFilePath">Full path to image.</param>
        /// <param name="options">Options array.</param>
        /// <returns>String from GdalInfo.</returns>
        public static string GetInfo(string inputFilePath, string[] options) => Info(inputFilePath, options);

        /// <summary>
        /// Gets the coordinates and pixel sizes of image.
        /// </summary>
        /// <param name="inputFilePath">Full path to image.</param>
        /// <returns>Array of coordinates and pixel sizes.</returns>
        public static double[] GetGeoTransform(string inputFilePath)
        {
            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFilePath, Access.GA_ReadOnly))
                {
                    double[] geoTransform = new double[6];
                    inputDataset.GetGeoTransform(geoTransform);
                    return geoTransform;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get GeoTransform.", exception);
            }
        }

        /// <summary>
        /// Gets image sizes.
        /// </summary>
        /// <param name="inputFilePath">Full path to image.</param>
        /// <returns>Image sizes.</returns>
        public static (int rasterXSize, int rasterYSize) GetRasterSizes(string inputFilePath)
        {
            int rasterXSize, rasterYSize;

            try
            {
                using (Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFilePath, Access.GA_ReadOnly))
                {
                    rasterXSize = inputDataset.RasterXSize;
                    rasterYSize = inputDataset.RasterYSize;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to get image sizes.", exception);
            }

            return (rasterXSize, rasterYSize);
        }

        /// <summary>
        /// Converts file using GdalWarp.
        /// Run only on concrete file (like .vrt or .tif).
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <param name="outputFileInfo">Output file.</param>
        /// <param name="callback">Progress reporting delegate.</param>
        public static void RepairTif(FileInfo inputFileInfo, FileInfo outputFileInfo,
                                     OSGeo.GDAL.Gdal.GDALProgressFuncDelegate callback) =>
            Warp(inputFileInfo.FullName, outputFileInfo.FullName, Enums.Image.Gdal.RepairTifOptions, callback);

        #endregion
    }
}
