/******************************************************************************
 *
 * Name:     GdalConfiguration.cs.pp
 * Project:  GDAL CSharp Interface
 * Purpose:  A static configuration utility class to enable GDAL/OGR.
 * Author:   Felix Obermaier
 *
 ******************************************************************************
 * Copyright (c) 2012, Felix Obermaier
 *
 * Permission is hereby granted, free of charge, to any person obtaining a
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included
 * in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

using System;
using System.IO;
using System.Reflection;
using OSGeo.GDAL;
using OSGeo.OGR;

namespace Gdal2Tiles
{
    /// <summary>
    /// Class for configurating gdal's environment variables.
    /// </summary>
    public static class GdalConfiguration
    {
        #region Properties

        /// <summary>
        /// Shows if gdal configured.
        /// </summary>
        public static bool IsGdalConfigured { get; private set; }

        /// <summary>
        /// Shows if ogr configured.
        /// </summary>
        public static bool IsOgrConfigured { get; private set; }

        /// <summary>
        /// Property to determine which platform we're on.
        /// </summary>
        private static string Platform => IntPtr.Size == 4 ? "x86" : "x64";

        #endregion

        #region Constructor

        /// <summary>
        /// Construction of Gdal/Ogr
        /// </summary>
        static GdalConfiguration()
        {
            string executingAssemblyFile = Assembly.GetExecutingAssembly().Location;
            string executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

            //if (string.IsNullOrWhiteSpace(executingDirectory))
            //    throw new InvalidOperationException("cannot get executing directory");

            string gdalPath = Path.Combine(executingDirectory, "gdal2");
            string nativePath = Path.Combine(gdalPath, Platform);

            // Prepend native path to environment path, to ensure the right libs are being used.
            string path = Environment.GetEnvironmentVariable("PATH");
            path = nativePath + ";" + Path.Combine(nativePath, "plugins") + ";" + path;
            Environment.SetEnvironmentVariable("PATH", path);

            // Set the additional GDAL environment variables.
            string gdalData = Path.Combine(gdalPath, "data");
            Environment.SetEnvironmentVariable("GDAL_DATA", gdalData);
            Gdal.SetConfigOption("GDAL_DATA", gdalData);

            string driverPath = Path.Combine(nativePath, "plugins");
            Environment.SetEnvironmentVariable("GDAL_DRIVER_PATH", driverPath);
            Gdal.SetConfigOption("GDAL_DRIVER_PATH", driverPath);

            Environment.SetEnvironmentVariable("GEOTIFF_CSV", gdalData);
            Gdal.SetConfigOption("GEOTIFF_CSV", gdalData);

            string projSharePath = Path.Combine(gdalPath, "share");
            Environment.SetEnvironmentVariable("PROJ_LIB", projSharePath);
            Gdal.SetConfigOption("PROJ_LIB", projSharePath);
        }

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Prints available OGR drivers in DEBUG mode.
        /// </summary>
        private static void PrintDriversOgr()
        {
            int count = Ogr.GetDriverCount();
            for (int i = 0; i < count; i++)
            {
                OSGeo.OGR.Driver driver = Ogr.GetDriver(i);
                Console.WriteLine($"OGR {i}: {driver.name}");
            }
        }

        /// <summary>
        /// Prints available GDAL drivers in DEBUG mode.
        /// </summary>
        private static void PrintDriversGdal()
        {
            int count = Gdal.GetDriverCount();
            for (int i = 0; i < count; i++)
            {
                OSGeo.GDAL.Driver driver = Gdal.GetDriver(i);
                Console.WriteLine($"GDAL {i}: {driver.ShortName}-{driver.LongName}");
            }
        }

        #endregion

        #region Public

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
        public static void ConfigureOgr()
        {
            if (IsOgrConfigured) return;

            // Register drivers
            Ogr.RegisterAll();
            IsOgrConfigured = true;

            #if DEBUG
            PrintDriversOgr();
            #endif
        }

        /// <summary>
        /// Method to ensure the static constructor is being called.
        /// </summary>
        /// <remarks>Be sure to call this function before using Gdal/Ogr/Osr</remarks>
        public static void ConfigureGdal()
        {
            if (IsGdalConfigured) return;

            // Register drivers
            Gdal.AllRegister();
            IsGdalConfigured = true;

            #if DEBUG
            PrintDriversGdal();
            #endif
        }

        #endregion

        #endregion
    }
}
