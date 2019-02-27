using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using OSGeo.GDAL;
using OSGeo.OGR;

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
        private static bool IsWindows => !(Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX);

        /// <summary>
        /// Prints initialized Ogr direvers.
        /// </summary>
        private static void PrintDriversOgr()
        {
            if (!Usable)
                throw new
                    Exception("Unable to print Ogr drivers because binary files wasn't found.");

            int driverCount = Ogr.GetDriverCount();
            for (int index = 0; index < driverCount; index++)
            {
                try
                {
                    using (OSGeo.OGR.Driver driver = Ogr.GetDriver(index))
                    {
                        Console.WriteLine($"OGR {index}: {driver.GetName()}", "Debug");
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("Unable to print current Ogr driver.", exception);
                }
            }
        }

        /// <summary>
        /// Prints initialized Gdal direvers.
        /// </summary>
        private static void PrintDriversGdal()
        {
            if (!Usable)
                throw new
                    Exception("Unable to print Gdal drivers because binary files wasn't found.");

            int driverCount = Gdal.GetDriverCount();
            for (int index = 0; index < driverCount; index++)
            {
                try
                {
                    using (OSGeo.GDAL.Driver driver = Gdal.GetDriver(index))
                    {
                        Console.WriteLine($"GDAL {index}: {driver.ShortName}-{driver.LongName}");
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("Unable to print current Gdal driver.", exception);
                }
            }
        }

        #endregion

        #region internal

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
                    throw new Exception("Gdal's binaries doesn't support current platform.");
                }

                string executingAssemblyFile = Assembly.GetExecutingAssembly().Location;
                string executingDirectory = Path.GetDirectoryName(executingAssemblyFile);

                if (string.IsNullOrWhiteSpace(executingDirectory))
                    throw new Exception("Unable to get executing directory.");

                // modify search place and order
                SetDefaultDllDirectories(DllSearchFlags);

                // ReSharper disable once AssignNullToNotNullAttribute
                string gdalPath = Path.Combine(executingDirectory, "gdal");
                string nativePath = Path.Combine(gdalPath, GetPlatform());
                if (!Directory.Exists(nativePath))
                    throw new Exception($"Gdal's binaries weren't found on: {nativePath}.");
                if (!File.Exists(Path.Combine(nativePath, "gdal_wrap.dll")))
                    throw new Exception($"gdal_wrap.dll wasn't found on: {nativePath}.");

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

                //todo
                //string gdalFileNameIsUtf8 = "NO";
                //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", gdalFileNameIsUtf8);
            }
            catch (Exception exception)
            {
                Usable = false;
                throw new Exception("Unable to initialize Gdal.", exception);
            }

            Usable = true;
        }

        /// <summary>
        /// Configures Ogr.
        /// </summary>
        /// <remarks>Make sure to call this method before using Ogr.</remarks>
        internal static void ConfigureOgr()
        {
            if (!Usable)
                throw new Exception("Can't find Gdal binaries, unable to configure Ogr.");
            if (IsOgrConfigured) return;

            // Register drivers
            try
            {
                Ogr.RegisterAll();
            }
            catch (Exception exception)
            {
                throw new Exception("Unabel to configure Ogr.", exception);
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
            if (!Usable)
                throw new Exception("Can't find Gdal binaries, unable to configure Gdal.");
            if (IsGdalConfigured) return;

            // Register drivers
            try
            {
                Gdal.AllRegister();
            }
            catch (Exception exception)
            {
                throw new Exception("Unable to configure Gdal.", exception);
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
