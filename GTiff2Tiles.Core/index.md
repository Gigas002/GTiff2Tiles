# GTiff2Tiles.Core

[![NuGet](https://img.shields.io/nuget/v/GTiff2Tiles.svg)](https://www.nuget.org/packages/GTiff2Tiles/)

**GTiff2Tiles.Core** is a core library of **GTiff2Tiles** projects. It's available on [Nuget](https://www.nuget.org/packages/GTiff2Tiles/) and [GitHub Packages Feed](https://github.com/Gigas002/GTiff2Tiles/packages). It provides the ability to generate different kind of tiles and crop them to directory, `IEnumerable`, `IAsyncEnumerable` and `Channel`.

Main namespaces are the following:

- `Constants` -- contains often used constants;
- `Coordinates` -- contains interface and classes with maths for different coordinate calculations, transfromations and convertations;
- `Enums` -- constains all used enums;
- `Exceptions` -- contains self-implemented exception classes;
- `GeoTiffs` -- constains interface and classes for generating tiles from different kind of GeoTiff on input;
- `Helpers` -- contains some static helpers methods;
- `Images` -- contains classes, that defines some of images's metadata, like band;
- `Localization` -- contains strings and localization files;
- `Tiles` -- contains interface and classes, related to representation of tiles as objects;

## Build dependencies

- [MaxRev.Gdal.Core](https://www.nuget.org/packages/MaxRev.Gdal.Core/) – 3.2.0.100;
- [MaxRev.Gdal.LinuxRuntime.Minimal](https://www.nuget.org/packages/MaxRev.Gdal.LinuxRuntime.Minimal/) – 3.1.2.110;
- [MaxRev.Gdal.WindowsRuntime.Minimal](https://www.nuget.org/packages/MaxRev.Gdal.WindowsRuntime.Minimal/) – 3.2.0.110;
- [NetVips](https://www.nuget.org/packages/NetVips/) – 1.2.4;
- [NetVips.Native](https://www.nuget.org/packages/NetVips.Native/) – 8.10.1;
- [BitMiracle.LibTiff.NET](https://www.nuget.org/packages/BitMiracle.LibTiff.NET) – 2.4.639

When building from source, there are more packages needed:

- [Microsoft.CodeAnalysis.NetAnalyzers](https://www.nuget.org/packages/Microsoft.CodeAnalysis.NetAnalyzers) – 5.0.1;

## Offline documentation

Offline docs are available as [pdf](https://github.com/Gigas002/GTiff2Tiles/blob/master/docs/pdf/GTiff2Tiles.pdf). The website and pdf docs both are created by [create-docs.ps1](https://github.com/Gigas002/GTiff2Tiles/blob/master/create-docs.ps1) script.
