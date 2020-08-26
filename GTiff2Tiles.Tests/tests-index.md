# GTiff2Tiles.Tests

**GTiff2Tiles.Tests** is a unit test project for **GTiff2Tiles.Core**.

[![Build status](https://ci.appveyor.com/api/projects/status/wp5bbi08sgd4i9bh/branch/master?svg=true)](https://ci.appveyor.com/project/Gigas002/gtiff2tiles/branch/master)
[![Actions Status](https://github.com/Gigas002/GTiff2Tiles/workflows/.NET%20Core%20CI/badge.svg)](https://github.com/Gigas002/GTiff2Tiles/actions)
[![codecov](https://codecov.io/gh/Gigas002/GTiff2Tiles/branch/master/graph/badge.svg)](https://codecov.io/gh/Gigas002/GTiff2Tiles)

You can run tests and analyze code coverage by running the [codecov-local.ps1](https://github.com/Gigas002/GTiff2Tiles/blob/master/codecov-local.ps1) script. Be sure to check the script out, since *coverlet.msbuild* works **only under Win, and for linux you shold run it a bit differently**. Tests will create the files inside `GTiff2Tiles/Examples/Output` directory. You can browse the results for different tile systems, sizes, bands count, extensions and geo systems out there after the tests are done.

## Build dependencies

- GTiff2Tiles.Core;
- [Microsoft.NET.Test.Sdk](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk) – 16.7.1;
- [NUnit](https://www.nuget.org/packages/NUnit) – 3.12.0;
- [NUnit3TestAdapter](https://www.nuget.org/packages/NUnit3TestAdapter/) – 3.17.0;
- [coverlet.collector](https://www.nuget.org/packages/coverlet.collector) – 1.3.0;
- [coverlet.msbuild](https://www.nuget.org/packages/coverlet.msbuild) – 2.9.0;
