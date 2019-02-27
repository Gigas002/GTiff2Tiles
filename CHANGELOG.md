# 27.02.2019 - Released 1.2.0

Changes since 1.1.0:

* GenerateTiles methods are now async;
* Use static classes instead of structs;
* Fixed black borders issue;
* Multithreading performance improvements;
* Replaced GeoTransform property with more obvious MinX, MinY, MaxX and MaxY;
* Temp directory is now timestamp and not deleted;
* Exceptions (TileException, GdalException, ImageException);
* A lot of error checking;
* Lots of improvements in Image.Gdal class
* Disabled NetVips console warnings;
* Added convertion of bad tiffs to 4326;
* Rewritten Image.Image constructor;
* Fixed some typos;
* Fixed minor issues;

# 03.02.2019 - Released 1.1.0

Changes since 1.0.0:

- Changed solution archeticture (again). Think now it'll last longer, since everything is organized. Now there's **Core** library, **Console** and **GUI** applications and unit **Tests** project;
- Fixed lots of typos;
- Removed **Gdal2Tiles.cs**;
- Use `NetVips.Image` instead of `Drawing.Imaging.Image`;
- Now can work with big tiffs and tiffs in another projections (will be converted);
- Improved error handling;
- Added `Examples` directory with `Input.tif` for you to test;
- Added multithreading and `ThreadsCount` properties here and there;
- Added progress reporting;
- Updated README;
- Fixed some minor issues;


# 30.12.2018 - Released 1.0.0

Changes since beta:

- Changed solution architecture: previous project split upon GTiff2Tiles library and GTiff2Tiles. Test console application for running tests;
- Fixed typo;
- Updated GDAL and GDAL.Native from 2.3.2 to 2.3.3;
- Moved from packages.config to PackageReference;
- Updated README;
- `GTiff2Tiles` class is no longer static and implements `IDisposable`;
- FIxed some minor issues.

