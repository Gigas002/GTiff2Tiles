/******************************************************************************
 *
 * Name:     GdalConfiguration.cs.pp
 * Project:  GDAL CSharp Interface
 * Purpose:  A static configuration utility class to enable GDAL/OGR.
 * Author:   Felix Obermaier
 *
 ******************************************************************************
 * Copyright (c) 2012-2018, Felix Obermaier
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using OSGeo.GDAL;
using OSGeo.OGR;

namespace GTiff2Tiles.Test
{
    /// <summary>
    /// Class for configurating gdal's environment variables.
    /// </summary>
    public static class GdalConfiguration
    {
        #region Properties and fields

        /// <summary>
        /// Shows if ogr configured.
        /// </summary>
        private static bool IsOgrConfigured { get; set; }

        /// <summary>
        /// Shows if gdal configured.
        /// </summary>
        private static bool IsGdalConfigured { get; set; }

        /// <summary>
        /// Gets a value indicating if the GDAL package is set up properly.
        /// </summary>
        public static bool Usable { get; private set; }

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDefaultDllDirectories(uint directoryFlags);
        //               LOAD_LIBRARY_SEARCH_USER_DIRS | LOAD_LIBRARY_SEARCH_SYSTEM32
        private const uint DllSearchFlags = 0x00000400 | 0x00000800;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddDllDirectory(string lpPathName);

        #endregion

        #region Constructor

        /// <summary>
        /// Construction of Gdal/Ogr
        /// </summary>
        static GdalConfiguration()
        {
            string executingDirectory = null, gdalPath = null, nativePath = null;
            try
            {
                if (!IsWindows)
                {
                    const string notSet = "_Not_set_";
                    string tmp = Gdal.GetConfigOption("GDAL_DATA", notSet);
                    Usable = tmp != notSet;
                    return;
                }

                string executingAssemblyFile = Assembly.GetExecutingAssembly().Location;
                executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

                if (string.IsNullOrEmpty(executingDirectory))
                    throw new InvalidOperationException("cannot get executing directory");

                // modify search place and order
                SetDefaultDllDirectories(DllSearchFlags);

                gdalPath = Path.Combine(executingDirectory, "gdal");
                nativePath = Path.Combine(gdalPath, GetPlatform());
                if (!Directory.Exists(nativePath))
                    throw new DirectoryNotFoundException($"GDAL native directory not found at '{nativePath}'");
                if (!File.Exists(Path.Combine(nativePath, "gdal_wrap.dll")))
                    throw new FileNotFoundException(
                                                    $"GDAL native wrapper file not found at '{Path.Combine(nativePath, "gdal_wrap.dll")}'");

                // Add directories
                AddDllDirectory(nativePath);
                AddDllDirectory(Path.Combine(nativePath, "plugins"));

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

                Usable = true;
            }
            catch (Exception e)
            {
                Usable = false;
                Trace.WriteLine(e, "error");
                Trace.WriteLine($"Executing directory: {executingDirectory}", "error");
                Trace.WriteLine($"gdal directory: {gdalPath}", "error");
                Trace.WriteLine($"native directory: {nativePath}", "error");

                //throw;
            }
        }

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Function to determine which platform we're on
        /// </summary>
        private static string GetPlatform() => Environment.Is64BitProcess ? "x64" : "x86";

        /// <summary>
        /// Gets a value indicating if we are on a windows platform
        /// </summary>
        private static bool IsWindows => !(Environment.OSVersion.Platform == PlatformID.Unix ||
                                           Environment.OSVersion.Platform == PlatformID.MacOSX);

        /// <summary>
        /// Prints available OGR drivers in DEBUG.
        /// </summary>
        private static void PrintDriversOgr()
        {
            if (!Usable) return;

            int num = Ogr.GetDriverCount();
            for (int i = 0; i < num; i++)
            {
                OSGeo.OGR.Driver driver = Ogr.GetDriver(i);
                Trace.WriteLine($"OGR {i}: {driver.GetName()}", "Debug");
            }
        }

        /// <summary>
        /// Prints available GDAL drivers in DEBUG.
        /// </summary>
        private static void PrintDriversGdal()
        {
            if (!Usable) return;
            int num = Gdal.GetDriverCount();
            for (int i = 0; i < num; i++)
            {
                OSGeo.GDAL.Driver driver = Gdal.GetDriver(i);
                Trace.WriteLine($"GDAL {i}: {driver.ShortName}-{driver.LongName}");
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
            if (!Usable) return;
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
            if (!Usable) return;
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
