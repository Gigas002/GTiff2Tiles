# WIP – Released 1.3.0

Changes since 1.2.0:

## Overall

- Solution rebuilt in **VS2019**;
- Removed explicitly unused packages;
- Added **NetVips.Native 8.8.0** package with **Windows x64** binaries;
- Updated **NetVips** package to **1.1.0-rc3**;
- Updated **GDAL** and **GDAL.Native** to **2.4.1**;
- Moved all strings to **Strings.resx** file in **Localization** directory;
- Added support for Russian language;
- *New feature:* Option to create non-tms tiles (*WIP*);

## Core

- Fix **NetVips** issue (#3, #4), when trying to shrink image, lesser than 1px (see https://github.com/Gigas002/GTiff2Tiles/commit/e086568ab1fcc528d9a49ee9fead2ade476815e7 and https://github.com/Gigas002/GTiff2Tiles/commit/e42e5aecc9bf42e2329dade4a9bc1575f006a4fc);
- Fix additional band creation issue (#5) (see https://github.com/Gigas002/GTiff2Tiles/commit/514ecd912b1e9f3c7a1eb5db8c3fe1770a365a6b);
- Cleaned up some code;

**Breaking changes:**

- `Image.Tile.GetTileNumbersFromCoords` method now also takes `bool tmsCompatible` parameter;
- `Image.Tile.TileBounds` method now takes `bool tmsCompatible` parameter, instead of `bool isFlipY = true)`, which was working the opposite. So, set the `tmsCompatible` value to `true` if you want to have the previous behaviour (creation of `tms` tiles).

## GUI

- Added screenshot;
- Added [icon](https://material.io/tools/icons/?icon=image&style=baseline);
- Added **material design**, including dialogs and message boxes;
- Removed **Ookii.Dialogs.Wpf** package (replaced by **MaterialDesignExtensions**);

## Console

- Added [icon](https://material.io/tools/icons/?icon=image&style=baseline);

## Test

- Added more zoom levels to test (from 0 to 18);
- Improved code coverage (*WIP*);

# 27.02.2019 – Released 1.2.0

Changes since 1.1.0:

* `GenerateTiles` methods are now `async`;
* Use static classes instead of structs;
* Fixed black borders issue;
* Multithreading performance improvements;
* Replaced `GeoTransform` property with more obvious `MinX`, `MinY`, `MaxX` and `MaxY`;
* *Temp* directory is now *timestamp* and not deleted;
* Exceptions (`TileException`, `GdalException`, `ImageException`);
* A lot of error checking;
* Lots of improvements in `Image.Gdal` class
* Disabled NetVips console warnings;
* Added conversion of bad tiffs to **4326**;
* Rewritten `Image.Image` constructor;
* Fixed some typos;
* Fixed minor issues;

# 03.02.2019 – Released 1.1.0

Changes since 1.0.0:

- Changed solution architecture (again). Think now it'll last longer, since everything is organized. Now there's **Core** library, **Console** and **GUI** applications and unit **Tests** project;
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


# 30.12.2018 – Released 1.0.0

Changes since beta:

- Changed solution architecture: previous project split upon GTiff2Tiles library and GTiff2Tiles. Test console application for running tests;
- Fixed typo;
- Updated GDAL and GDAL.Native from 2.3.2 to 2.3.3;
- Moved from packages.config to PackageReference;
- Updated README;
- `GTiff2Tiles` class is no longer static and implements `IDisposable`;
- FIxed some minor issues.

