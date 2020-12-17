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
                  .WithNotParsed(_ => _isParsingErrors = true);

            if (_isParsingErrors) return;

            Directory.CreateDirectory($"{Benchmark.Data}/{Benchmark.In}");
            // Copy benchmark.tif to data/in/i.tif
            File.Copy(_inputPath, $"{Benchmark.Data}/{Benchmark.In}/{Benchmark.Itif}", true);

            // Pull the latest images
            Docker.Pull(Benchmark.GdalName);
            Docker.Pull(Benchmark.MaptilerName);

            // Run the benchmarks
            BenchmarkRunner.Run<Benchmark>();
        }

        private static void ParseConsoleOptions(Options options) => _inputPath = string.IsNullOrWhiteSpace(options.InputFilePath) ? _inputPath : options.InputFilePath;
    }
}
