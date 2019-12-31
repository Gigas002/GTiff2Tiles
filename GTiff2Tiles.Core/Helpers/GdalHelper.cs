using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MaxRev.Gdal.Core;

/****************************************************************
 * Original class created by MaxRev and located in this repo: https://github.com/MaxRev-Dev/gdal.netcore
 ***************************************************************/

// ReSharper disable PossibleNullReferenceException

namespace GTiff2Tiles.Core.Helpers
{
    /// <summary>
    /// Configures all variables and options for GDAL including plugins and Proj6.db path
    /// </summary>
    public static class GdalHelper
    {
        /// <summary>
        /// Shows if everything has been configured.
        /// </summary>
        private static bool IsConfigured { get; set; }

        /// <summary>
        /// Setups gdalplugins and calls Gdal.AllRegister(), Ogr.RegisterAll(), Proj6.Configure().
        /// </summary>
        public static void ConfigureAll()
        {
            if (IsConfigured) return;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assembly winRtAssembly = Assembly.Load(new AssemblyName("MaxRev.Gdal.WindowsRuntime.Minimal"));
                DirectoryInfo assemblyDirectoryInfo = new FileInfo(winRtAssembly.Location).Directory;
                DirectoryInfo executingDirectoryInfo = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;

                if (assemblyDirectoryInfo?.EnumerateFiles("gdal_*.dll").Any() == false)
                {
                    static string Sources(string path) => Path.Combine(path, "runtimes", "win-x64", "native");

                    DirectoryInfo nativeDirectoryInfo = new DirectoryInfo(Sources(assemblyDirectoryInfo.FullName));

                    if (!nativeDirectoryInfo.Exists)
                    {
                        string primarySource = assemblyDirectoryInfo.Parent?.Parent?.FullName;
                        nativeDirectoryInfo = new DirectoryInfo(Sources(primarySource));

                        if (!nativeDirectoryInfo.Exists) nativeDirectoryInfo = new DirectoryInfo(Sources(executingDirectoryInfo?.FullName));
                    }

                    if (nativeDirectoryInfo.Exists)
                    {
                        string driversPath = Path.Combine(nativeDirectoryInfo.FullName, "gdalplugins");
                        // here hdf4 driver requires jpeg library to be loaded
                        // and I won't copy all libraries on each startup
                        string jpegPath = Path.Combine(executingDirectoryInfo.FullName, "jpeg.dll");

                        if (!File.Exists(jpegPath))
                            File.Copy(Path.Combine(nativeDirectoryInfo.FullName, "jpeg.dll"),
                                      Path.Combine(executingDirectoryInfo.FullName, "jpeg.dll"));

                        OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DRIVER_PATH", driversPath);
                    }
                    else
                    {
                        throw new Exception("Can't find runtime libraries");
                    }
                }
                else
                {
                    IEnumerable<FileInfo> dlls = executingDirectoryInfo?.EnumerateFiles("gdal_*.dll").Where(file => !file.Name.Contains("wrap"));
                    string driversPath = Path.Combine(executingDirectoryInfo.FullName, "gdalplugins");
                    Directory.CreateDirectory(driversPath);

                    foreach (FileInfo dll in dlls)
                    {
                        string destPath = Path.Combine(driversPath, dll.Name);
                        File.Move(dll.FullName, destPath); //, true);
                    }

                    OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DRIVER_PATH", driversPath);
                }
            }

            //TODO: Cyrillic paths for GdalBuildVrt
            //string gdalFileNameIsUtf8 = "NO";
            //Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", gdalFileNameIsUtf8);

            OSGeo.GDAL.Gdal.AllRegister();
            OSGeo.OGR.Ogr.RegisterAll();
            Proj6.Configure();

            IsConfigured = true;
        }
    }
}
