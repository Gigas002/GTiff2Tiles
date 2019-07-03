# GTiff2Tiles

![Icon](GTiff2Tiles.GUI/Resources/Icon.ico)

Analogue of [gdal2tiles.py](https://github.com/OSGeo/gdal/blob/master/gdal/swig/python/scripts/gdal2tiles.py) on **C#**. Currently support **only GeoTIFF** as input data and creates **only EPSG:4326 geodetic tiles** on output in [**tms**](https://wiki.osgeo.org/wiki/Tile_Map_Service_Specification) structure.

Solution is build in **VS2019 (16.1.5)**, **.NET Framework 4.7.2**, targeting **Windows x64** systems.

Icon is taken from [here](https://material.io/tools/icons/?icon=image&style=baseline) and is used in **GTiff2Tiles.GUI** and **GTiff2Tiles.Console**.

[![Build status](https://ci.appveyor.com/api/projects/status/wp5bbi08sgd4i9bh?svg=true)](https://ci.appveyor.com/project/Gigas002/gtiff2tiles)

## Table of contents

- [GTiff2Tiles](#gtiff2tiles)
  - [Table of contents](#table-of-contents)
  - [Current version](#current-version)
  - [Examples](#examples)
  - [GTiff2Tiles.Core](#gtiff2tilescore)
    - [Dependencies](#dependencies)
    - [Localization](#localization)
  - [GTiff2Tiles.Console](#gtiff2tilesconsole)
    - [Requirements](#requirements)
    - [Usage](#usage)
    - [Detailed options description](#detailed-options-description)
    - [Dependencies](#dependencies-1)
    - [Localization](#localization-1)
  - [GTiff2Tiles.GUI](#gtiff2tilesgui)
    - [Requirements](#requirements-1)
    - [Dependencies](#dependencies-2)
    - [Localization](#localization-2)
  - [GTiff2Tiles.Tests](#gtiff2tilestests)
    - [Dependencies](#dependencies-3)
  - [TODO](#todo)
  - [Contributing](#contributing)

Table of contents generated with [markdown-toc](http://ecotrust-canada.github.io/markdown-toc/ ).

## Current version

Current stable can be found here: [![Release](https://img.shields.io/github/release/Gigas002/GTiff2Tiles.svg)](https://github.com/Gigas002/GTiff2Tiles/releases/latest), or on NuGet: [![NuGet](https://img.shields.io/nuget/v/GTiff2Tiles.svg)](https://www.nuget.org/packages/GTiff2Tiles/).

Information about changes since previous releases can be found in [changelog](https://github.com/Gigas002/GTiff2Tiles/blob/master/CHANGELOG.md). This project supports [SemVer 2.0.0](https://semver.org/) (template is `{MAJOR}.{MINOR}.{PATCH}.{BUILD}`).

Previous versions can be found on [releases](https://github.com/Gigas002/GTiff2Tiles/releases) and [branches](https://github.com/Gigas002/GTiff2Tiles/branches) pages.

## Examples

In [Examples](https://github.com/Gigas002/GTiff2Tiles/tree/master/Examples/Input) directory you can find **GeoTIFFs** for some tests.

## GTiff2Tiles.Core

**GTiff2Tiles.Core** is a core library. [Here’s](https://github.com/Gigas002/GTiff2Tiles/wiki) the API documentation.

Library uses 2 different algorithms to create tiles:

- **Crop** – crops all the zooms from input file;
- **Join** – crops the lowest zoom from input file and then joins the upper images from built tiles.

Also I should mention, that if your input **GeoTIFF** is not **EPSG:4326**, it will be converted by **GdalWarp** to that projection, and saved in **temp** directory before cropping tiles.

### Dependencies

- [GDAL](https://www.nuget.org/packages/GDAL/) – 2.4.1;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) – 2.4.1;
- [NetVips](https://www.nuget.org/packages/NetVips/) – 1.1.0-rc3;
- [NetVips.Native.win-x64](https://www.nuget.org/packages/NetVips.Native.win-x64/) – 8.8.0;
- [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/) – 4.5.2;

### Localization

Localizable strings are located in `Localization/Strings.resx` file. You can add your translation (e.g. added `Strings.Ru.resx` file) and create pull request.

Currently, application is available on **English** and **Russian** languages.

## GTiff2Tiles.Console

**GTiff2Tiles.Console** is a console application, that uses methods from core to create tiles.

### Requirements

- Windows 7 SP1 x64 and newer;
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472);

If you’re using Windows 7 SP1, you can experience weird error with **GDAL** package. It’s recommended to install [KB2533623](<https://www.microsoft.com/en-us/download/details.aspx?id=26764>) to fix it. You can read about this Windows update on [MSDN](<https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot>).

### Usage

| Short |    Long     |          Description          | Required? |
| :---: | :---------: | :---------------------------: | :-------: |
|  -i   |   --input   |    Full path to input file    |    Yes    |
|  -o   |  --output   | Full path to output directory |    Yes    |
|  -t   |   --temp    |  Full path to temp directory  |    Yes    |
|       |   --minz    |     Minimum cropped zoom      |    Yes    |
|       |   --maxz    |     Maximum cropped zoom      |    Yes    |
|  -a   | --algorithm |   Algorithm to create tiles   |    Yes    |
|       |  --threads  |         Threads count         |    No     |
|       |  --version  |        Current version        |           |
|       |   --help    | Message about console options |           |

### Detailed options description

**input** is `string`, representing full path to input **GeoTIFF** file.

**output** is `string`, representing full path to directory, where tiles in **tms** structure will be created. **Should be empty.**

**temp** is `string`, representing full path to temporary directory. Inside will be created directory, which name is a **timestamp** in format `yyyyMMddHHmmssfff`.

**minz** is `int` parameter, representing minimum zoom, which you want to crop.

**maxz** is `int` parameter, representing maximum zoom, which you want to crop.

**algorithm** is `string`, representing cropping algorithm. Can be **crop** or **join**. When using **crop**, the input image will be cropped for each zoom. When using **join**, the input image will be cropped for the lowest zoom, and the upper tiles created by joining lowest ones.

**threads** is `int` parameter, representing threads count. By default (if not set) uses **5 threads**.

### Dependencies

- GTiff2Tiles.Core;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) – 2.4.1;
- [NetVips.Native.win-x64](https://www.nuget.org/packages/NetVips.Native.win-x64/) – 8.8.0;
- [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/) – 4.5.2;
- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) – 2.5.0;

### Localization

Localizable strings are located in `Localization/Strings.resx` file. You can add your translation (e.g. added `Strings.Ru.resx` file) and create pull request.

Currently, application is available on **English** and **Russian** languages.

## GTiff2Tiles.GUI

**GTiff2Tiles.GUI** is a very simple GUI, that has the same methods and parameters, as **GTiff2Tiles.Console**:

![Main page](GTiff2Tiles.GUI/Screenshots/MainPage.png)

### Requirements

- Windows 7 SP1 x64 and newer;
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472);

If you’re using Windows 7 SP1, you can experience weird error with **GDAL** package. It’s recommended to install [KB2533623](<https://www.microsoft.com/en-us/download/details.aspx?id=26764>) to fix it. You can read about this Windows update on [MSDN](<https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot>).

### Dependencies

- GTiff2Tiles.Core;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) – 2.4.1;
- [NetVips.Native.win-x64](https://www.nuget.org/packages/NetVips.Native.win-x64/) – 8.8.0;
- [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/) – 4.5.2;
- [Caliburn.Micro](https://www.nuget.org/packages/Caliburn.Micro) – 3.2.0;
- [MahApps.Metro](<https://www.nuget.org/packages/MahApps.Metro>) – 1.6.5;
- [MaterialDesignColors](<https://www.nuget.org/packages/MaterialDesignColors>) – 1.1.3;
- [MaterialDesignThemes](<https://www.nuget.org/packages/MaterialDesignThemes>) – 2.5.1;
- [MaterialDesignExtensions](https://www.nuget.org/packages/MaterialDesignExtensions) – 2.6.0;

### Localization

Localizable strings are located in `Localization/Strings.resx` file. You can add your translation (e.g. added `Strings.Ru.resx` file) and create pull request.

Currently, application is available on **English** and **Russian** languages.

## GTiff2Tiles.Tests

**GTiff2Tiles.Tests** is a unit test project for **GTiff2Tiles.Core**.

### Dependencies

- GTiff2Tiles.Core;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) – 2.4.1;
- [NetVips.Native.win-x64](https://www.nuget.org/packages/NetVips.Native.win-x64/) – 8.8.0;
- [NUnit](https://www.nuget.org/packages/NUnit/3.12.0) – 3.12.0;
- [NUnit3TestAdapter](https://www.nuget.org/packages/NUnit3TestAdapter/) – 3.13.0;

## TODO

You can track, what’s planned to do in future releases on [projects](https://github.com/Gigas002/GTiff2Tiles/projects) page.

## Contributing

Feel free to contribute, make forks, change some code, add [issues](https://github.com/Gigas002/GTiff2Tiles/issues), etc.
