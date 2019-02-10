# GTIFF2TILES

Analogue of [gdal2tiles.py](https://github.com/OSGeo/gdal/blob/master/gdal/swig/python/scripts/gdal2tiles.py) on C#. Currently support any GEOTIFF, but creates **EPSG:4325** **geodetic** tiles on output in [**tms**](https://wiki.osgeo.org/wiki/Tile_Map_Service_Specification) structure.

Solution is build in VS2017, .NET Framework 4.7.2, targeting Windows x64 systems.

[![Build status](https://ci.appveyor.com/api/projects/status/wp5bbi08sgd4i9bh?svg=true)](https://ci.appveyor.com/project/Gigas002/gtiff2tiles)

## Table of contents

- [GTIFF2TILES](#gtiff2tiles)
  * [Table of contents](#table-of-contents)
  * [Current version](#current-version)
  * [Examples](#examples)
  * [GTiff2Tiles.Core](#gtiff2tilescore)
    + [Dependencies](#dependencies)
  * [GTiff2Tiles.Console](#gtiff2tilesconsole)
    + [Usage](#usage)
    + [Dependencies](#dependencies-1)
  * [GTiff2Tiles.GUI](#gtiff2tilesgui)
    + [Dependencies](#dependencies-2)
  * [GTiff2Tiles.Tests](#gtiff2tilestests)
    + [Dependencies](#dependencies-3)
  * [TODO](#todo)
  * [Contributing](#contributing)

Table of contents generated with [markdown-toc](http://ecotrust-canada.github.io/markdown-toc/ ).

## Current version

Current stable can be found here: [![Release](https://img.shields.io/github/release/Gigas002/GTiff2Tiles.svg)](https://github.com/Gigas002/GTiff2Tiles/releases/latest). Information about changes since previous release can be found in [changelog](https://github.com/Gigas002/GTiff2Tiles/blob/master/CHANGELOG.md).

## Examples

In [Examples](https://github.com/Gigas002/GTiff2Tiles/tree/master/Examples/Input) directory you can find GEOTIFF for some tests.

## GTiff2Tiles.Core 

**GTiff2Tiles.Core** is a core library. [Here’s]() (will be some day later) the API. 

Library uses 2 different algorithms to create tiles:

- **Crop** - crops all the zooms from input file;
- **Join** - crops the lowest zoom from input file and then join the upper images from built tiles.

Also I should mention, that if your input .tif is not **EPSG:4326**, it will be converted by **GdalWarp** to that projection, and saved in **temp** directory before cropping tiles.

### Dependencies

- [GDAL](https://www.nuget.org/packages/GDAL/) - 2.3.3;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) - 2.3.3;
- [NetVips](https://www.nuget.org/packages/NetVips/) - 1.0.7;
- [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/) - 4.5.2;
- [System.Runtime.CompilerServices.Unsafe](https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/) - 4.5.2;

## GTiff2Tiles.Console

**GTiff2Tiles.Console** is a console application, that uses methods from core to create tiles. 

### Usage

| Short |    Long     |                      Description                       | Required? |
| :---: | :---------: | :----------------------------------------------------: | :-------: |
|  -i   |   --input   |                Full path to input file                 |    Yes    |
|  -o   |  --output   |             Full path to output directory              |    Yes    |
|  -t   |   --temp    |              Full path to temp directory               |    Yes    |
|       |   --minz    |                  Minimum cropped zoom                  |    Yes    |
|       |   --maxz    |                  Maximum cropped zoom                  |    Yes    |
|  -a   | --algorythm | Algorithm to create tiles. Can be \"join\" or \"crop\" |    Yes    |
|       |  --threads  |                     Threads count                      |    No     |

**Please, be aware of temp directory parameter, because this directory will be deleted after successful crop of tiles!**


### Dependencies

- [GDAL](https://www.nuget.org/packages/GDAL/) - 2.3.3;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) - 2.3.3;
- [NetVips](https://www.nuget.org/packages/NetVips/) - 1.0.7;
- [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/) - 4.5.2;
- [System.Runtime.CompilerServices.Unsafe](https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/) - 4.5.2;
- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) - 2.4.3;

## GTiff2Tiles.GUI

**GTiff2Tiles.GUI** is a very simple (and ugly!) GUI, that has the same functions, as **GTiff2Tiles.Console**.

**Please, be aware that temp directory will be deleted after successful crop of tiles!**

### Dependencies

- [GDAL](https://www.nuget.org/packages/GDAL/) - 2.3.3;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) - 2.3.3;
- [NetVips](https://www.nuget.org/packages/NetVips/) - 1.0.7;
- [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions/) - 4.5.2;
- [System.Runtime.CompilerServices.Unsafe](https://www.nuget.org/packages/System.Runtime.CompilerServices.Unsafe/) - 4.5.2;
- [Caliburn.Micro](https://www.nuget.org/packages/Caliburn.Micro) - 3.2.0;
- [Ookii.Dialogs.Wpf](https://www.nuget.org/packages/Ookii.Dialogs.Wpf/) - 1.0.0;

Later I’ll probably make this look better, but first I should write docs for **GTiff2Tiles.Core**…

## GTiff2Tiles.Tests

**GTiff2Tiles.Tests** is a unit test project for **GTiff2Tiles.Core**. I’ll add support of CI later.

### Dependencies

- [GDAL](https://www.nuget.org/packages/GDAL/) - 2.3.3;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) - 2.3.3;
- [NetVips](https://www.nuget.org/packages/NetVips/) - 1.0.7;

## TODO

- Target .Net standard 2.1 as soon as possible;
- Support all functional of original Gdal2Tiles.py script;
- Write docs;
- Add CI;

## Contributing

Feel free to contribute, make forks, change some code, add issues etc.
