using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace GTiff2Tiles.Test
{
    public class ImageTests
    {
        private const string ExamplesDirectoryName = "Examples";
        private const string Input = "Input";
        private const string GenerateTilesOutputDirectoryName = "GenerateTilesOutput";
        private const string GenerateTilesOldOutputDirectoryName = "GenerateTilesOldOutput";

        private const int MinZ = 10;
        private const int MaxZ = 17;

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GenerateTiles()
        {
            string executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(executingAssemblyPath)) Assert.Fail();

            string inputFilePath = Path.Combine(executingAssemblyPath, ExamplesDirectoryName, Input,
                                                $"{Input}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName =
                Path.Combine(executingAssemblyPath, ExamplesDirectoryName, GenerateTilesOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            int minZ = MinZ;
            int maxZ = MaxZ;

            try
            {
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, minZ, maxZ);
                image.GenerateTiles();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            Assert.Pass();
        }

        [Test]
        public void GenerateTilesOld()
        {
            string executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(executingAssemblyPath)) Assert.Fail();

            string inputFilePath = Path.Combine(executingAssemblyPath, ExamplesDirectoryName, Input,
                                                $"{Input}{Core.Enums.Extensions.Tif}");
            string outputDirectoryName =
                Path.Combine(executingAssemblyPath, ExamplesDirectoryName, GenerateTilesOldOutputDirectoryName);

            FileInfo inputFileInfo = new FileInfo(inputFilePath);
            DirectoryInfo outputDirectoryInfo = new DirectoryInfo(outputDirectoryName);

            int minZ = MinZ;
            int maxZ = MaxZ;

            int threadsCount = 5;

            try
            {
                Core.Image.Image image = new Core.Image.Image(inputFileInfo, outputDirectoryInfo, minZ, maxZ);
                image.GenerateTilesOld(threadsCount);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }

            Assert.Pass();
        }
    }
}
