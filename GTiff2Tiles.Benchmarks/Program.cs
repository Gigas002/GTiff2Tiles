using System.IO;
using BenchmarkDotNet.Running;
using CommandLine;

namespace GTiff2Tiles.Benchmarks
{
    internal static class Program
    {
        private static string _inputPath = "../../../../../Examples/Input/Benchmark.tif";

        private static bool _isParsingErrors;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(ParseConsoleOptions)
                  .WithNotParsed(error => _isParsingErrors = true);

            if (_isParsingErrors) return;

            Directory.CreateDirectory($"{Benchmarks.Data}/{Benchmarks.In}");
            // Copy benchmark.tif to data/in/i.tif
            File.Copy(_inputPath, $"{Benchmarks.Data}/{Benchmarks.In}/{Benchmarks.Itif}", true);

            // Pull the latest images
            Docker.Pull(Benchmarks.GdalName);
            Docker.Pull(Benchmarks.MaptilerName);

            // Run the benchmarks
            BenchmarkRunner.Run<Benchmarks>();
        }

        private static void ParseConsoleOptions(Options options) => _inputPath = string.IsNullOrWhiteSpace(options.InputFilePath) ? _inputPath : options.InputFilePath;
    }
}
