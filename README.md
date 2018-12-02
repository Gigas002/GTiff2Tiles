# GDAL2TILES

Analogue of [gdal2tiles.py](https://github.com/OSGeo/gdal/blob/master/gdal/swig/python/scripts/gdal2tiles.py) on C#. I've rewritten it for my university project, so now it only works like gdal2tiles.py with the following arguments (zoom values and paths are here for example):

```con
input.tif outputDirectory -s EPSG:4326 -p geodetic -r cubicspline --tmscompatible -z 10-14
```

It also doesn't make any output/progress reporting at the moment and doesn't write openlayers.html/.kml/etc, only .png tiles in tms structure. Later I plan to develop this to support all original's gdal2tiles functionality.

Project is build in VS2017, .NET Framework 4.7.2, targeting x64 systems.

## Table of Contents

- [GDAL2TILES](#gdal2tiles)
    - [Table of Contents](#table-of-contents)
    - [Dependencies](#dependencies)
    - [Usage](#usage)
    - [TODO](#todo)
    - [Contributing](#contributing)

## Dependencies

- [GDAL](https://www.nuget.org/packages/GDAL/) - 2.3.2;
- [GDAL.Native](https://www.nuget.org/packages/GDAL.Native/) - 2.3.2;

## Usage

To use Gdal2Tiles class you should add this file to your project. It depends on Gdal's C# bindings, that you should reference in your project and configure before calling Gdal2Tiles.cs methods. I used GDAL and GDAL.Native in my example and I recommend you to do the same, yet it would work with other versions of [Gdal](https://www.gisinternals.com/query.html?content=filelist&file=release-1911-x64-gdal-mapserver.zip) bindings too. So, the complete algorithm looks like this:

1. Add Gdal's c# bindings, Gdal2Tiles.cs file and `using OSGeo.GDAL` directive to your project;
2. Configure Gdal in your code before calling Gdal2Tiles methods (for GDAL.NET nuget package call `GdalConfiguration.ConfigureGdal()`; and if you don't want to use GdalConfiguration class, you can call `Gdal.AllRegister()` directly, but you should specify environment variables before by yourself);

3. Call `Gdal2Tiles.CropTifToTiles` method, which takes following parameters:

   - `string inputFile` - full path to input GeoTIFF in EPSG:4326 (current version, plan to expand functionality later);
   - `string outputDirectory` - full path to output directory, in which zoom directories with data will be created;
   - `int minZ` - minimum cropped zoom, which you want for your data;
   - `int maxZ` - maximum cropped zoom;
   - `OSGeo.Gdal.ResampleAlg resampling` - resampling algorithm (currently CubicSpline/Cubic only supported);

Also, it’s worth mentioning, that `CreateBaseTile()` and `CreateOverviewTiles()` methods use `Parallel.For`/`Parallel.ForEach`, so if you don’t like it you’d probably will need to rewrite these three lines with usual `foreach`/`for` loop.

## TODO

- Fix tile borders error from original gdal2tiles script (error probably happens in GeoQuery method);
- Replace gdal’s reading and writing methods with something more performant (at least System.Drawing.Image, Bitmap or Graphics);
- In ideal, fully replace gdal (GeoTiff’s metadata probably can be read with help of [libtiff.net](https://github.com/BitMiracle/libtiff.net), need tests);
- Support all functional of original script;
- Progress reporting;

## Contributing

Feel free to contribute, make forks, change some code, add issues etc.
