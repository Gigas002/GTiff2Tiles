# GTIFF2TILES

Analogue of [gdal2tiles.py](https://github.com/OSGeo/gdal/blob/master/gdal/swig/python/scripts/gdal2tiles.py) on C#. I've rewritten it for my university project, so now it only works like gdal2tiles.py with the following arguments (zoom values and paths are here for example):

```con
input.tif outputDirectory -s EPSG:4326 -p geodetic -r cubicspline --tmscompatible -z 10-14
```

It also doesn't make any output/progress reporting at the moment and doesn't write openlayers.html/.kml/etc, only .png tiles in tms structure. Later I plan to develop this to support all original's gdal2tiles functionality.

Project is build in VS2017, .NET Framework 4.7.2, targeting x64 systems.

- [GTIFF2TILES](#gtiff2tiles)
  * [Table of Contents](#table-of-contents)
  * [Dependencies](#dependencies)
  * [Current version](#current-version)
  * [Usage](#usage)
    + [GTiff2Tiles class](#gtiff2tiles-class)
    + [Gdal2Tiles class](#gdal2tiles-class)
  * [TODO](#todo)
  * [Contributing](#contributing)

Table of contents generated with [markdown-toc](http://ecotrust-canada.github.io/markdown-toc/ ).


## Dependencies

- [GDAL](https://www.nuget.org/packages/GDAL/) - 2.3.3;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) - 2.3.3;

## Current version

Current stable version is 1.0.0. Information about changes since previous release can be found in [changelog](https://github.com/Gigas002/GTiff2Tiles/blob/master/CHANGELOG.md).

## Usage

Reference the GTiff2Tiles.dll and then use one of the following classes:

### GTiff2Tiles class

1. Reference GDAL and GDAL.Native (if you need gdal’s binaries as well) packages. It will automatically add `GdalConfiguration` class to your project, if you didn’t create it already;

2. Call `GdalConfiguration.ConfigureGdal()` or `Gdal.AllRegister()` (but in that case you should add `using OSGeo.Gdal` directive and specify environment variables before by yourself) method:
3. Create `GTiff2Tiles` object with constructor, which takes the following parameters:
   - `string inputFile` - full path to input GeoTIFF in EPSG:4326 projection;
   - `string outputDirectory` - full path to output directory, in which zoom directories with .png tiles will be created;
   - `int minZ` - minimum cropped zoom;
   - `int maxZ` - maximum cropped zoom;
4. Call `GenerateTiles()` method to create tiles.

Don't forget about `using`, when making `GTiff2Tiles` object, or manually call `Dispose()` on object, when tiles are done.

### Gdal2Tiles class

**Warning!** Prefer using GTiff2Tiles class. It works much faster I’ve fixed coordinates error there.

1. Reference GDAL and GDAL.Native (if you need gdal’s binaries as well) packages. It will automatically add `GdalConfiguration` class to your project, if you didn’t create it already;
2. Add `using OSGeo.GDAL` directive to your project;
3. Call `GdalConfiguration.ConfigureGdal()` or `Gdal.AllRegister()` (but in that case you should specify environment variables before by yourself) method:
4. Call `Gdal2Tiles.CropTifToTiles` method, which takes following parameters:

   - `string inputFile` - full path to input GeoTIFF in EPSG:4326 (current version, plan to expand functionality later);
   - `string outputDirectory` - full path to output directory, in which zoom directories with data will be created;
   - `int minZ` - minimum cropped zoom, which you want for your data;
   - `int maxZ` - maximum cropped zoom;
   - `OSGeo.Gdal.ResampleAlg resampling` - resampling algorithm (currently CubicSpline/Cubic only supported);

Also, it’s worth mentioning, that `CreateBaseTile()` and `CreateOverviewTiles()` methods use `Parallel.For`/`Parallel.ForEach`, so if you don’t like it you’d probably will need to rewrite these three lines with usual `foreach`/`for` loop.

## TODO

- Target .Net standard 2.1 as soon as possible;
- Replace `System.Drawing.Image` class with something better, like `NetVips.Image`, because original library is not capable of working with bigtiffs;
- In ideal, fully replace gdal (GeoTiff’s metadata probably can be read with help of [libtiff.net](https://github.com/BitMiracle/libtiff.net), need tests);
- Support all functional of original script;
- Add multithreading to `GTiff2Tiles` class;
- Improve exception handling in `GTiff2Tiles` class;
- Progress reporting.

## Contributing

Feel free to contribute, make forks, change some code, add issues etc.
