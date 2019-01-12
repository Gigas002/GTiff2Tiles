using System;
using System.IO;
using System.Reflection;
using NetVips;

//todo write about initializing procedures in readme

namespace GTiff2Tiles.Test
{
    public static class NetVipsConfiguration
    {
        public static bool ConfigureVips(string vipsDirectoryName)
        {
            if (ModuleInitializer.VipsInitialized) return true;

            // Get the directory for the executing assembly in which the current code resides.
            string currentDirectory;
            try
            {
                currentDirectory =
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            }
            catch (Exception)
            {
                return false;
            }

            // <LibvipsOutputBase>vips</LibvipsOutputBase>
            if (currentDirectory != null)
            {
                string vipsPath = Path.Combine(currentDirectory, vipsDirectoryName);

                // Prepend the vips path to PATH environment variable, to ensure the right libs are being used.
                string path = Environment.GetEnvironmentVariable("PATH");
                path = $"{vipsPath};{path}";
                Environment.SetEnvironmentVariable("PATH", path);
                Console.WriteLine(vipsPath);
            }
            else
                return false;

            // Try to reinitialize libvips
            try
            {
                return Base.VipsInit();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
