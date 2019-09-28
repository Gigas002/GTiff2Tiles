# CHANGELOG

## WIP

Changes since 1.3.1:

### Overall

- Updated **CommandLineParser** package to **2.6.0**;
- Updated **NUnit3TestAdapter** package to **3.15.1**;
- Updated **NetVips.Native.win-x64** package to **8.8.2**;
- Update target framework to **.NET Framework 4.8**;

### GUI

- Updated **MaterialDesignExtensions** package to **2.8.0-a1**;
- Removed **MahApps.Metro** package;
- Replaced **MahApps.Metro.MetroWindow** with **MaterialDesignExtensions.MaterialWindow**;

## 30.07.2019 – Released 1.3.1

Changes since 1.3.0:

- Updated **NetVips** to stable **1.1.0**;
- Updated **NetVips.Native.win-x64** to **8.8.1**;

## 28.07.2019 – Released 1.3.0

Changes since 1.2.0:

### Overall

- Solution rebuilt in **Visual Studio 2019**;
- Removed explicitly unused packages;
- Added **NetVips.Native.win-x64 8.8.0** package with **Windows x64** binaries;
- Updated **NetVips** package to **1.1.0-rc3**;
- Updated **GDAL** and **GDAL.Native** to **2.4.2**;
- Updated **System.Threading.Tasks.Extension** to **4.5.3**;
- Moved all strings to **Strings.resx** file in **Localization** directory;
- Added support for Russian language;
- Added **GTiff2Tiles.Benchmarks** project and **README** section;
- *New feature:* Option to create **non-tms tiles** (see [#9](https://github.com/Gigas002/GTiff2Tiles/issues/9) and [162e3a2](https://github.com/Gigas002/GTiff2Tiles/commit/162e3a28043006af71e9eac150ed150a7596ee8a));
- *New feature:* Now **GTiff2Tiles** is able to process non-8 bit images (see [#9](https://github.com/Gigas002/GTiff2Tiles/issues/9) and [f08f690](https://github.com/Gigas002/GTiff2Tiles/commit/f08f690f5d08cd604dc0ffa46171fd98d0c4a8ee));

### Core

- Fix **NetVips** issue ([#3](https://github.com/Gigas002/GTiff2Tiles/issues/3) and [#4](https://github.com/Gigas002/GTiff2Tiles/issues/4)), when trying to shrink image, lesser than 1px (see [e086568](https://github.com/Gigas002/GTiff2Tiles/commit/e086568ab1fcc528d9a49ee9fead2ade476815e7) and [e42e5ae](https://github.com/Gigas002/GTiff2Tiles/commit/e42e5aecc9bf42e2329dade4a9bc1575f006a4fc));
- Fix additional band creation issue (see [#5](https://github.com/Gigas002/GTiff2Tiles/issues/5) and [514ecd9](https://github.com/Gigas002/GTiff2Tiles/commit/514ecd912b1e9f3c7a1eb5db8c3fe1770a365a6b));
- `Image.Gdal.Warp` method is now `async` and returns `ValueTask`;
- `Image.Gdal.Info` method is now `async` and returns `ValueTask<string>`;
- `Helpers.CheckHelper.CheckInputFile` method is now `async` and returns `ValueTask<bool>`;
- `Helpers.CheckHelper.CheckDirectory` method now also takes optional nullable parameter `shouldBeEmpty`. Read more about it’s using in project [wiki page](https://github.com/Gigas002/GTiff2Tiles/wiki/Helpers.CheckHelper);
- Cleaned up some code;

**Breaking changes:**

- `Image.Image.GenerateTilesByJoining` and `Image.Image.GenerateTilesByCropping` now takes `bool tmsCompatible` as 4th argument;
- `Tile.Tile.GetTileNumbersFromCoords` method now also takes `bool tmsCompatible` parameter;
- `Tile.Tile.TileBounds` method now takes `bool tmsCompatible` parameter, instead of `bool isFlipY = true)`, which was working the opposite. So, set the `tmsCompatible` value to `true` if you want to have the previous behaviour (creation of `tms` tiles);
- `Enums.Image.Gdal.Block` property has been removed;
- `Enums.Image.Gdal.Byte` property value has been changed to `Type=Byte` for more correct checks from  `Image.Gdal.Info`;
- `Enums.Image.Gdal.RepairTifOptions` array now also has options `“-of”, “GTiff”` and `“-ot”, “Byte”` for processing non-8 bit images;
- Option `“-co”, “TILED=YES”` was removed from `Enums.Image.Gdal.RepairTifOptions` array. You should explicitly add this option to your array if you want to convert input image to `TILED` image;

### GUI

- Added screenshot;
- Added [icon](https://material.io/tools/icons/?icon=image&style=baseline);
- Added **material design**, including dialogs and message boxes;
- Added `TmsCompatible` check box;
- Removed **Ookii.Dialogs.Wpf** package (replaced by **MaterialDesignExtensions**);
- Updated **MaterialDesignThemes** package to **2.6.0**;
- Updated **MaterialDesignColors** package to **1.2.0**;

### Console

- Added [icon](https://material.io/tools/icons/?icon=image&style=baseline);
- Added `--tms` console option. Read more about using it in updated **README**;
- Updated **CommandLineParser** to **2.5.0**;

### Test

- Added more zoom levels to test (from 0 to 18);
- Added tests for tms-compatible/non tms-compatible tiles creation;
- Updated **NUnit** to **3.12.0**;
- Added **NUnit3TestAdapter** package;
- Slightly improved code coverage;

## 27.02.2019 – Released 1.2.0

Changes since 1.1.0:

- `GenerateTiles` methods are now `async`;
- Use static classes instead of structs;
- Fixed black borders issue;
- Multithreading performance improvements;
- Replaced `GeoTransform` property with more obvious `MinX`, `MinY`, `MaxX` and `MaxY`;
- *Temp* directory is now *timestamp* and not deleted;
- Exceptions (`TileException`, `GdalException`, `ImageException`);
- A lot of error checking;
- Lots of improvements in `Image.Gdal` class
- Disabled NetVips console warnings;
- Added conversion of bad tiffs to **4326**;
- Rewritten `Image.Image` constructor;
- Fixed some typos;
- Fixed minor issues;

## 03.02.2019 – Released 1.1.0

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
- Updated **README**;
- Fixed some minor issues;

## 30.12.2018 – Released 1.0.0

Changes since beta:

- Changed solution architecture: previous project split upon GTiff2Tiles library and GTiff2Tiles. Test console application for running tests;
- Fixed typo;
- Updated **GDAL** and **GDAL.Native** from **2.3.2** to **2.3.3**;
- Moved from `packages.config` to `PackageReference`;
- Updated **README**;
- `GTiff2Tiles` class is no longer static and implements `IDisposable`;
- FIxed some minor issues.
