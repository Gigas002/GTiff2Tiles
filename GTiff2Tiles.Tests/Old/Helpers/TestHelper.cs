﻿using System;
using System.IO;
using System.Reflection;
using GTiff2Tiles.Tests.Old.Constants;

namespace GTiff2Tiles.Tests.Old.Helpers
{
    internal static class TestHelper
    {
        internal static DirectoryInfo GetExamplesDirectoryInfo()
        {
            string examplesDirectoryPath = Environment.Is64BitProcess
                                               ? new DirectoryInfo(Assembly.GetExecutingAssembly().Location)
                                                .Parent?.Parent?.Parent?.Parent?.Parent?.Parent?.FullName
                                               : new DirectoryInfo(Assembly.GetExecutingAssembly().Location)
                                                .Parent?.Parent?.Parent?.Parent?.Parent?.FullName;

            if (string.IsNullOrWhiteSpace(examplesDirectoryPath))
                throw new Exception("Unable to locate Examples directory.");

            examplesDirectoryPath = Path.Combine(examplesDirectoryPath, FileSystemEntries.ExamplesDirectoryName);

            return new DirectoryInfo(examplesDirectoryPath);
        }
    }
}