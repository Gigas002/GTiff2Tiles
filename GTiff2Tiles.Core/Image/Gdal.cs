using System;
using System.IO;
using System.Runtime.InteropServices;
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

        #region Private

        /// <summary>
        /// Gets the coordinates and pixel sizes of image.
        /// </summary>
        /// <param name="inputFilePath">Full path to image.</param>
        /// <returns>Array of coordinates and pixel sizes.</returns>
        private static double[] GetGeoTransform(string inputFilePath)
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
        /// Converts file using GdalWarp.
        /// Run only on concrete file (like .vrt or .tif).
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <param name="outputFileInfo">Output file.</param>
        /// <param name="callback">Progress reporting delegate.</param>
        private static void RepairTif(FileInfo inputFileInfo, FileInfo outputFileInfo,
                                      OSGeo.GDAL.Gdal.GDALProgressFuncDelegate callback = null) =>
            Warp(inputFileInfo.FullName, outputFileInfo.FullName, Enums.Image.Gdal.RepairTifOptions, callback);

        #endregion

        #region Public

        /// <summary>
        /// Gets the information about image.
        /// </summary>
        /// <param name="inputFilePath">Full path to image.</param>
        /// <param name="options">Options array.</param>
        /// <returns>String from GdalInfo.</returns>
        public static string GetInfo(string inputFilePath, string[] options) => Info(inputFilePath, options);

        /// <summary>
        /// Gets proj4 string of input file.
        /// </summary>
        /// <param name="inputFilePath">Full path to input file.</param>
        /// <returns>Proj4 string.</returns>
        public static string GetProj4String(string inputFilePath)
        {
            try
            {
                using (Dataset dataset = OSGeo.GDAL.Gdal.Open(inputFilePath, Access.GA_ReadOnly))
                {
                    string wkt = dataset.GetProjection();
                    using (SpatialReference spatialReference = new SpatialReference(wkt))
                    {
                        spatialReference.ExportToProj4(out string proj4String);
                        return proj4String;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to read input file's projection.", exception);
            }
        }

        /// <summary>
        /// Gets the coordinates borders of the input Geotiff file.
        /// </summary>
        /// <param name="inputFilePath">Full path to image.</param>
        /// <param name="rasterXSize">Raster's width.</param>
        /// <param name="rasterYSize">Raster's height.</param>
        /// <returns>Tuple with coordinates.</returns>
        public static (double xMin, double yMin, double xMax, double yMax) GetFileBorders(
            string inputFilePath, int rasterXSize, int rasterYSize)
        {
            double[] geoTransform = GetGeoTransform(inputFilePath);
            return (geoTransform[0], geoTransform[3] - rasterYSize * geoTransform[1],
                    geoTransform[0] + rasterXSize * geoTransform[1], geoTransform[3]);
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
        /// Changes the input file, so it can be used by <see cref="Image"/> methods.
        /// </summary>
        /// <param name="inputFileInfo">Input file.</param>
        /// <param name="tempDirectoryInfo">Temp directory for fixed tif.</param>
        /// <returns>Fixed <see cref="FileInfo"/> object.</returns>
        public static FileInfo RepairTif(FileInfo inputFileInfo, DirectoryInfo tempDirectoryInfo)
        {
            //Try to create directory for temp file.
            try
            {
                tempDirectoryInfo.Create();
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to create temp directory. Path:{tempDirectoryInfo.FullName}.", exception);
            }

            string tempFilePath = Path.Combine(tempDirectoryInfo.FullName,
                                               $"{Enums.Image.Gdal.TempFileName}{Enums.Extensions.Tif}");
            FileInfo tempFileInfo = new FileInfo(tempFilePath);

            try
            {
                RepairTif(inputFileInfo, tempFileInfo);
            }
            catch (Exception exception)
            {
                throw new Exception($"Unable to repair input tif. Path:{inputFileInfo.FullName}.", exception);
            }

            return tempFileInfo;
        }

        #endregion

        #endregion
    }
}
