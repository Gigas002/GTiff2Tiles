using System;
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

        #endregion
    }
}
