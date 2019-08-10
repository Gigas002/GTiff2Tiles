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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using GTiff2Tiles.Core.Localization;
using OSGeo.GDAL;
using OSGeo.OGR;

// ReSharper disable LocalizableElement

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Class for initializing gdal. Analogue of <see cref="GdalConfiguration"/> class.
    /// </summary>
    internal static class GdalHelper
    {
        #region Fields and properties

        /// <summary>
        /// Shows if Ogr drivers are configured.
        /// </summary>
        internal static bool IsOgrConfigured { get; private set; }

        /// <summary>
        /// Shows if Gdal drivers are configured.
        /// </summary>
        internal static bool IsGdalConfigured { get; private set; }

        /// <summary>
        /// Shows, if Gdal's binaries are found and have system paths.
        /// </summary>
        internal static bool Usable { get; private set; }

        #region P/Invoke

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultDllDirectories(uint directoryFlags);

        //LOAD_LIBRARY_SEARCH_USER_DIRS | LOAD_LIBRARY_SEARCH_SYSTEM32
        private const uint DllSearchFlags = 0x00000400 | 0x00000800;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddDllDirectory(string lpPathName);

        #endregion

        #endregion

        #region Methods

        #region Private

        /// <summary>
        /// Gets current platform.
        /// </summary>
        /// <returns>x64 or x86.</returns>
        private static string GetPlatform() => Environment.Is64BitProcess ? "x64" : "x86";

        /// <summary>
        /// Shows, if we're running on windows.
        /// </summary>
        /// <returns><see langword="true"/> if Windows, <see langword="false"/> otherwise.</returns>
        private static bool IsWindows => !(Environment.OSVersion.Platform == PlatformID.Unix ||
                                           Environment.OSVersion.Platform == PlatformID.MacOSX);

        /// <summary>
        /// Prints initialized Ogr drivers.
        /// </summary>
        private static void PrintDriversOgr()
        {
            if (!Usable) throw new Exception(string.Format(Strings.UnableToPrintDrivers, "OGR"));

            int driverCount = Ogr.GetDriverCount();

            for (int index = 0; index < driverCount; index++)
                try
                {
                    using (OSGeo.OGR.Driver driver = Ogr.GetDriver(index))
                    {
                        Console.WriteLine($"OGR {index}: {driver.GetName()}", "Debug");
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format(Strings.UnableToPrintDrivers, "OGR"), exception);
                }
        }

        /// <summary>
        /// Prints initialized Gdal drivers.
        /// </summary>
        private static void PrintDriversGdal()
        {
            if (!Usable) throw new Exception(string.Format(Strings.UnableToPrintDrivers, "GDAL"));

            int driverCount = Gdal.GetDriverCount();

            for (int index = 0; index < driverCount; index++)
                try
                {
                    using (OSGeo.GDAL.Driver driver = Gdal.GetDriver(index))
                    {
                        Console.WriteLine($"GDAL {index}: {driver.ShortName}-{driver.LongName}");
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception(string.Format(Strings.UnableToPrintDrivers), exception);
                }
        }

        #endregion

        #region Internal

        /// <summary>
        /// Looks for Gdal's binaries and sets PATHs if needed.
        /// </summary>
        internal static void Initialize()
        {
            try
            {
                if (!IsWindows)
                {
                    const string notSet = "_Not_set_";
                    string temp = Gdal.GetConfigOption("GDAL_DATA", notSet);
                    Usable = temp != notSet;

                    throw new Exception(string.Format(Strings.DoesntSupportPlatform, "GDAL"));
                }

                string executingAssemblyFile = Assembly.GetExecutingAssembly().Location;
                string executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

                if (string.IsNullOrWhiteSpace(executingDirectory))
                    throw new Exception(string.Format(Strings.StringIsEmpty, nameof(executingDirectory)));

                // modify search place and order
                SetDefaultDllDirectories(DllSearchFlags);

                // ReSharper disable once AssignNullToNotNullAttribute
                string gdalPath = Path.Combine(executingDirectory, "gdal");
                string nativePath = Path.Combine(gdalPath, GetPlatform());

                if (!Directory.Exists(nativePath))
                    throw new Exception(string.Format(Strings.DoesntExist, "GDAL", nativePath));
                if (!File.Exists(Path.Combine(nativePath, "gdal_wrap.dll")))
                    throw new Exception(string.Format(Strings.DoesntExist, "gdal_warp.dll", nativePath));

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

                //TODO: Fix bug with cyrillic paths
                //string gdalFileNameIsUtf8 = "NO";
                //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", gdalFileNameIsUtf8);
            }
            catch (Exception exception)
            {
                Usable = false;

                throw new Exception(string.Format(Strings.UnableToConfigure, "GDAL"), exception);
            }

            Usable = true;
        }

        /// <summary>
        /// Configures Ogr.
        /// </summary>
        /// <remarks>Make sure to call this method before using Ogr.</remarks>
        internal static void ConfigureOgr()
        {
            if (!Usable) throw new Exception(string.Format(Strings.UnableToConfigure, "OGR"));

            if (IsOgrConfigured) return;

            // Register drivers
            try { Ogr.RegisterAll(); }
            catch (Exception exception)
            {
                throw new Exception(string.Format(Strings.UnableToConfigure, "OGR"), exception);
            }

            IsOgrConfigured = true;

            #if DEBUG
            PrintDriversOgr();
            #endif
        }

        /// <summary>
        /// Configures Gdal.
        /// </summary>
        /// <remarks>Make sure to call this method before using Gdal.</remarks>
        internal static void ConfigureGdal()
        {
            if (!Usable) throw new Exception(string.Format(Strings.UnableToConfigure, "GDAL"));

            if (IsGdalConfigured) return;

            // Register drivers
            try { Gdal.AllRegister(); }
            catch (Exception exception)
            {
                throw new Exception(string.Format(Strings.UnableToConfigure, "GDAL"), exception);
            }

            IsGdalConfigured = true;

            #if DEBUG
            PrintDriversGdal();
            #endif
        }

        #endregion

        #endregion
    }
}
