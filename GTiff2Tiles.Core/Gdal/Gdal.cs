using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Exceptions.Gdal;
using GTiff2Tiles.Core.Geodesic;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Image;
using GTiff2Tiles.Core.Localization;
using OSGeo.GDAL;
using OSGeo.OSR;

// ReSharper disable MemberCanBeInternal

namespace GTiff2Tiles.Core.Gdal
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
        /// <returns></returns>
        public static async ValueTask WarpAsync(FileInfo inputFileInfo, FileInfo outputFileInfo, string[] options,
                                           OSGeo.GDAL.Gdal.GDALProgressFuncDelegate callback = null)
        {
            #region Parameters checking

            CheckHelper.CheckFile(inputFileInfo, true);
            CheckHelper.CheckFile(outputFileInfo, false);
            CheckHelper.CheckDirectory(outputFileInfo.Directory);

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            await Task.Run(() =>
            {
                using Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly);

                GCHandle gcHandle = GCHandle.Alloc(new[] { Dataset.getCPtr(inputDataset).Handle },
                                                   GCHandleType.Pinned);
                SWIGTYPE_p_p_GDALDatasetShadow gdalDatasetShadow =
                    new SWIGTYPE_p_p_GDALDatasetShadow(gcHandle.AddrOfPinnedObject(), false, null);

                // ReSharper disable once UnusedVariable
                using Dataset resultDataset =
                    OSGeo.GDAL.Gdal.wrapper_GDALWarpDestName(outputFileInfo.FullName, 1, gdalDatasetShadow,
                                                             new GDALWarpAppOptions(options), callback,
                                                             string.Empty);

                gcHandle.Free();
            }).ConfigureAwait(false);

            //Was file created?
            CheckHelper.CheckFile(outputFileInfo, true);
        }

        /// <summary>
        /// Runs GdalInfo with passed parameters.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <param name="options">Array of string parameters for GdalInfo.</param>
        /// <returns><see cref="string"/> from GdalInfo.</returns>
        public static async ValueTask<string> InfoAsync(FileInfo inputFileInfo, string[] options = null)
        {
            #region Parameters checking

            CheckHelper.CheckFile(inputFileInfo, true);

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            string gdalInfoString = string.Empty;

            await Task.Run(() =>
            {
                using Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly);

                gdalInfoString = OSGeo.GDAL.Gdal.GDALInfo(inputDataset, new GDALInfoOptions(options));

                if (string.IsNullOrWhiteSpace(gdalInfoString))
                    throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(gdalInfoString)));
            }).ConfigureAwait(false);

            return gdalInfoString;
        }

        #endregion

        #region Initialize Gdal/Ogr

        /// <summary>
        /// Initialize Gdal, if it hadn't been initialized yet.
        /// </summary>
        private static void ConfigureGdal() => GdalHelper.ConfigureAll();

        #endregion

        #region Image

        #region Private

        /// <summary>
        /// Gets the coordinates and pixel sizes of image.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <returns>Array of double coordinates and pixel sizes.</returns>
        private static double[] GetGeoTransform(FileInfo inputFileInfo)
        {
            #region Parameters checking

            CheckHelper.CheckFile(inputFileInfo, true);

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            using Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly);

            double[] geoTransform = new double[6];
            inputDataset.GetGeoTransform(geoTransform);

            return geoTransform;
        }

        #endregion

        #region Internal

        /// <summary>
        /// Gets proj4 string of input file.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <returns>Proj4 string.</returns>
        internal static async ValueTask<string> GetProj4StringAsync(FileInfo inputFileInfo)
        {
            #region Parameters checking

            CheckHelper.CheckFile(inputFileInfo, true);

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            string proj4String = string.Empty;

            await Task.Run(() =>
            {
                using Dataset dataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly);

                string wkt = dataset.GetProjection();

                using SpatialReference spatialReference = new SpatialReference(wkt);

                spatialReference.ExportToProj4(out proj4String);

                if (string.IsNullOrWhiteSpace(proj4String))
                    throw new GdalException(string.Format(Strings.StringIsEmpty, nameof(proj4String)));
            }).ConfigureAwait(false);

            return proj4String;
        }

        /// <summary>
        /// Gets the coordinates borders of the input Geotiff file.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <param name="rasterXSize">Raster's width.</param>
        /// <param name="rasterYSize">Raster's height.</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> with WGS84 coordinates.</returns>
        internal static (Coordinate minCoordinate, Coordinate maxCoordinate) GetImageBorders(FileInfo inputFileInfo, Size size)
        {
            #region Parameters checking

            CheckHelper.CheckFile(inputFileInfo, true);

            if (size.Width < 0) throw new GdalException(string.Format(Strings.LesserThan, nameof(size.Width), 0));
            if (size.Height < 0) throw new GdalException(string.Format(Strings.LesserThan, nameof(size.Height), 0));

            #endregion

            double[] geoTransform = GetGeoTransform(inputFileInfo);

            double minX = geoTransform[0];
            double minY = geoTransform[3] - size.Height * geoTransform[1];
            double maxX = geoTransform[0] + size.Width * geoTransform[1];
            double maxY = geoTransform[3];

            Coordinate minCoordinate = new Coordinate(minX, minY);
            Coordinate maxCoordinate = new Coordinate(maxX, maxY);

            return (minCoordinate, maxCoordinate);
        }

        /// <summary>
        /// Gets image sizes in pixels.
        /// </summary>
        /// <param name="inputFileInfo">Input GeoTiff file.</param>
        /// <returns><see cref="ValueTuple{T1, T2}"/> with image sizes in pixels.</returns>
        internal static (int rasterXSize, int rasterYSize) GetImageSizes(FileInfo inputFileInfo)
        {
            //TODO: Remove, better use NetVips

            #region Parameters checking.

            CheckHelper.CheckFile(inputFileInfo, true);

            #endregion

            //Initialize Gdal, if needed.
            ConfigureGdal();

            using Dataset inputDataset = OSGeo.GDAL.Gdal.Open(inputFileInfo.FullName, Access.GA_ReadOnly);

            return (inputDataset.RasterXSize, inputDataset.RasterYSize);
        }

        #endregion

        #endregion
    }
}
