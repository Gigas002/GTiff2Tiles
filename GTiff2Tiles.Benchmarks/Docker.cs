using System.Diagnostics;

// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Benchmarks
{
    /// <summary>
    /// Simple wrapper around docker commands
    /// </summary>
    public static class Docker
    {
        // TODO: try Docker.DotNet when it'll be... less complex

        #region Properties and consts

        /// <summary>
        /// Docker's main process name
        /// </summary>
        private const string ProcessName = "docker";

        #endregion

        #region Methods

        /// <summary>
        /// Pull docker image from docker hub
        /// </summary>
        /// <param name="imageName">Name of image to pull</param>
        public static void Pull(string imageName)
        {
            using Process docker = new Process
            {
                StartInfo = new ProcessStartInfo(ProcessName)
                {
                    Arguments = $"pull {imageName}",
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            docker.Start();
            docker.WaitForExit();
        }

        /// <summary>
        /// Remove docker image
        /// </summary>
        /// <param name="imageName">Name of image or images to remove</param>
        public static void Rmi(string imageName)
        {
            using Process docker = new Process
            {
                StartInfo = new ProcessStartInfo(ProcessName)
                {
                    Arguments = $"rmi {imageName}",
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }
            };

            docker.Start();
            docker.WaitForExit();
        }

        /// <summary>
        /// Run docker image
        /// </summary>
        /// <param name="runArgs">Run command's args</param>
        /// <param name="imageName">Name of image (not the container) to run</param>
        /// <param name="imageArgs">Args of image to run</param>
        public static void Run(string runArgs, string imageName, string imageArgs)
        {
            using Process docker = new Process
            {
                StartInfo = new ProcessStartInfo(ProcessName)
                {
                    Arguments = $"run {runArgs} {imageName} {imageArgs}",

                    // Maptiler fails somehow if you set this to true
                    CreateNoWindow = false,
                    RedirectStandardInput = false,
                    RedirectStandardOutput = false,
                    UseShellExecute = false
                }
            };

            docker.Start();
            docker.WaitForExit();
        }

        #endregion
    }
}
