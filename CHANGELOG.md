# CHANGELOG

## WIP -- 2.0.0

This is a new major release of **GTiff2Tiles**. There are a lot of incompatible major and minor changes since **1.4.1** listed below.

### Repo organization

- Rewritten issue templates;
- Rewritten **GitHub Projects**;
- Added **GitHub Milestones**;
- More issues's labels;
- Add CI (*build & tests on Windows and Ubuntu2004*) on **GitHub**;
- Add **GitHub's CI** badge to readme;
- Improved Appveyor CI;
- Add CD through Appveyor;
- Add **CODE_OF_CONDUCT.md**;
- Add **CONTRIBUTING.md**;
- Add **Pull Request** template;
- Add **code coderage** badge;

### New packages

- [Docker Hub](https://hub.docker.com/r/gigas002/gtiff2tiles-console) (*GTiff2Tiles.Console*);
- [GitHub Packages: Docker](https://github.com/Gigas002/GTiff2Tiles/packages/145349) (*GTiff2Tiles.Console*);
- [GitHub Packages: nuget package](https://github.com/Gigas002/GTiff2Tiles/packages/146275) (*GTiff2Tiles.Core*);

### All solution

- Rewritten and added new publish scripts;
- Bump **.NET Core**/**.NET** version to **5.0.100**;
- Fixed localization files's names;
- Moved to **[DBAD](https://dbad-license.org/)** license;
- Drop support of **.NET Framework** and **.NET Standard**;
- Add **appveyor** configuration;
- Add a lot more metadata to `.csproj` files;
- Cleaner code thanks to new code analysis dependency;
- Removed `AnyCPU` builds (*now only x64 configuration available*);
- Fixed Linux build and apps;
- Fixed SemVer violations;

### Core

Core library was completely rewritten, a lot of new, moved and removed API. Please read [docs on GitHub Pages](https://gigas002.github.io/GTiff2Tiles/api/index.html) for new API, here's only a quick list of changes in previous API. Old docs are still available at [GitHub Wiki](https://github.com/Gigas002/GTiff2Tiles/wiki).

**Overall:**

- Better performance on all tasks;
- A lot of new/changed API;
- Completed and expanded all xml-docs;
- Improved publishing of package;
- Building .snupkg as well;
- Better error/exception-handling;
- Less useless localization strings;
- Reduced tile shifting in output tiles (resolved by updating **NetVips.Native to 8.10.0+**);

#### Changed API

Only changes in public API are listed.

**GTiff2Tiles.Core.Tile:**

- `GTiff2Tiles.Core.Tile` namespace renamed to `GTiff2Tiles.Core.Tiles`;
- `Tile` is no longer `static` class, methods moved to the corresponding classes in `GTiff2Tiles.Core.Tiles` namespace;
- `public static (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetTileNumbersFromCoords(double minX, double minY, double maxX, double maxY, int zoom, bool tmsCompatible)` method moved to `GTiff2Tiles.Core.Coordinates.GeoCoordinate` class and it's inherited classes as method `public static (Number minNumber, Number maxNumber) GetNumbers(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int z, Size tileSize, bool tmsCompatible)`;
- `public static (double minX, double minY, double maxX, double maxY) TileBounds(int tileX, int tileY, int zoom, bool tmsCompatible)` method moved to `GTiff2Tiles.Core.Tiles.Number` class as method `public static (GeodeticCoordinate minCoordinate, GeodeticCoordinate maxCoordinate) ToGeodeticCoordinates(Number number, Size tileSize, bool tmsCompatible)`

**GTiff2Tiles.Core.Image.Gdal:**

- Static class `GTiff2Tiles.Core.Image.Gdal` is moved to `GTiff2Tiles.Core` namespace and renamed to `GdalWorker`;
- Constants from `GTiff2Tiles.Core.Enums.Image.Gdal` class moved to `GTiff2Tiles.Core.GdalWorker`;
- Using `SrsEpsg4326` string instead of `Wgs84`;
- `RepairTifOptions` renamed to `ConvertCoordinateSystemOptions` and doesn't contain `-t_srs` param anymore;
- `TempFileName`'s value now `_tmp_converted.tif`;
- `public static async ValueTask WarpAsync(FileInfo inputFileInfo, FileInfo outputFileInfo, string[] options, OSGeo.GDAL.Gdal.GDALProgressFuncDelegate callback = null)` changed to `public static Task WarpAsync(string inputFilePath, string outputFilePath, string[] options, IProgress<double> progress = null)`;
- `public static async ValueTask<string> InfoAsync(FileInfo inputFileInfo, string[] options = null)` changed to `public static Task<string> InfoAsync(string inputFilePath, string[] options = null)`;
- `ConfigureGdal()` is now public and calls `MaxRev.Gdal.Core.GdalBase.ConfigureAll()` method instead of self-implemented configuration;

**GTiff2Tiles.Core.Image.Image:**

- `GTiff2Tiles.Core.Image.Image` class is renamed and moved to `GTiff2Tiles.Core.GeoTiffs.Raster`;
- `int RasterXSize` and `int RasterYSize` properties are now one new property `Size Size`;
- `FileInfo InputFileInfo` property is removed;
- `double MinX` and `double MinY` properties are now one new property `GeoCoordinate MinCoordinate`;
- `double MaxX` and `double MaxY` properties are now one new property `GeoCoordinate MaxCoordinate`;
- Constructor `public Image(FileInfo inputFileInfo)` changed to `public Raster(string inputFilePath, long maxMemoryCache = 2147483648)`;
- `public async ValueTask GenerateTilesByJoiningAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, bool tmsCompatible, IProgress<double> progress, int threadsCount)` methods is now `public Task CreateOverviewTilesAsync(ChannelWriter<RasterTile> channelWriter, int minZ, int maxZ, HashSet<RasterTile> tiles, bool isBuffered, CoordinateSystem coordinateSystem, Size tileSize = null, TileExtension extension = TileExtension.Png, bool tmsCompatible = false, int bandsCount = 4)`;
- `public async ValueTask GenerateTilesByCroppingAsync(DirectoryInfo outputDirectoryInfo, int minZ, int maxZ, bool tmsCompatible, IProgress<double> progress, int threadsCount)` method is now `public Task WriteTilesToDirectoryAsync(string outputDirectoryPath, int minZ, int maxZ, bool tmsCompatible = false, Size tileSize = null, TileExtension tileExtension = TileExtension.Png, Interpolation interpolation = Interpolation.Lanczos3, int bandsCount = RasterTile.DefaultBandsCount, int tileCacheCount = 1000, int threadsCount = 0, IProgress<double> progress = null, Action<string> printTimeAction = null)`;

**GTiff2Tiles.Core.Helpers.CheckHelper:**

- `public static void CheckDirectory(DirectoryInfo directoryInfo, bool? shouldBeEmpty = null)` method is now `public static void CheckDirectory(string? directoryPath, bool? shouldBeEmpty = null)`;
- `public static async ValueTask<bool> CheckInputFileAsync(FileInfo inputFileInfo)` method is now `public static async ValueTask<bool> CheckInputFileAsync(string inputFilePath, CoordinateSystem targetSystem)`;

**GTiff2Tiles.Core.Helpers.GdalHelper:**

- `GdalHelper` class is removed, use `MaxRev.Gdal.Core.GdalBase.ConfigureAll()` method to configure gdal instead;

**GTiff2Tiles.Core.Exceptions:**

- `GTiff2Tiles.Core.Exceptions.Tile.TileException` class is removed.
- `GTiff2Tiles.Core.Exceptions.Tile.ImageException` class is renamed and moved to `GTiff2Tiles.Core.Exceptions.RasterException`;
- `GTiff2Tiles.Core.Exceptions.Image.GdalException` class is removed.

**GTiff2Tiles.Core.Enums:**

- Class `Extensions` is renamed and moved to `GTiff2Tiles.Core.Constants.FileExtensions`;
- Class `DateTimePatterns` is moved to `GTiff2Tiles.Core.Constants` namespace;
- Class `Algorithms` is removed;
- Class `GTiff2Tiles.Core.Enums.Image.Gdal` is removed, values moved to `GdalWorker` class (*read it's docs*);

**Changed dependencies:**

- Removed **System.Threading.Tasks.Extensions** package, since project doesn't target *netstandard2.0* anymore;
- Add **Microsoft.SourceLink.GitHub** package ver. **1.0.0**;
- Add **BitMiracle.LibTiff.NET** package ver. **2.4.639**;
- Add **Microsoft.CodeAnalysis.NetAnalyzers** package ver. **5.0.1** (*build from source only*);

**Updated dependencies:**

- Update **MaxRev.Gdal.Core** to **3.1.2.110**;
- Update **MaxRev.Gdal.LinuxRuntime.Minimal** to **3.1.2.110**;
- Update **MaxRev.Gdal.WindowsRuntime.Minimal** to **3.1.2.110**;
- Update **NetVips** to **1.2.4**;
- Update **NetVips.Native** to **8.10.1**;

### Console

Read updated [docs](https://gigas002.github.io/GTiff2Tiles/console-index.html) to learn more about options and using, here's just a quick list of changes.

**Changed parameters:**

- `-a/--algorithm` is removed, now `crop` is default algorithm for creating tiles (*better performance and tiles quality*);
- `-t/--temp` is now optional;
- `--tms` is now optional;
- `--threads` now calculates optimal number of threads by default instead of hardcoded `5` threads;

**New parameters:**

- `-e/--extension` lets you choose **extension** for your output tiles;
- `-c/--coordinates` lets you choose tiles **coordinate system**;
- `--interpolation` lets you choose tiles **interpolation**;
- `-b/--bands` lets you choose **number of bands** in output tiles;
- `--tilecache` lets you choose **number of tiles** to store in memory cache (performance tweaks);
- `-m/--memcache` lets you choose maximal **size** of input files to store in RAM;
- `-p/--progress` lets you hide the **progress output**;
- `--timeleft` lets you see the **estimated time left** in output while working;
- `--tilesize` lets you specify the **size** of tile's images;

**Changed dependencies:**

- Add **Microsoft.CodeAnalysis.NetAnalyzers** package ver. **5.0.1** (*build from source only*);

**Updated dependencies:**

- Update **CommandLineParser** to **2.8.0**;

### GUI

Read updated [docs](https://gigas002.github.io/GTiff2Tiles/gui-index.html) to learn more about options and using, here's just a quick list of changes.

**New parameters for creating tiles:**

Now you're able to select tiles:

- Extension;
- Coordinate system;
- Interpolation;
- Number of bands;

**Other new features:**

Added the block of additional (*optional*) settings, which lets you to:

- Change the **theme** to *Light* or *Dark*;
- Change the **size** of ready tiles;
- Auto-guess the **number of threads** or select it by your own (*just uncheck the checkbox*);
- Change **number of tiles** in cache (*performance tweak*);
- Select **maximal size** for input files to store in *RAM*;
- Save all the **settings** in *settings.json* and see the same page on each start of the application;
- Also now you can see the *Time passed* **timer** on the right side;

**Changed dependencies:**

- Moved from **Caliburn.Micro** to **Prism.DryIoc** ver. **8.0.0.1909**;
- Add **Microsoft.CodeAnalysis.NetAnalyzers** package ver. **5.0.1** (*build from source only*);

**Updated dependencies:**

- Updated **MaterialDesignColors** to **1.2.7**;
- Updated **MaterialDesignThemes** to **3.2.0**;
- Updated **MaterialDesignExtensions** to **3.3.0-a01**;

### Benchmarks

Completely rewritten benchmarks. Now everything's done automatically, though it requires some changes to prerequisites. Running it's a bit tricky, so be sure to read updated [docs](https://gigas002.github.io/GTiff2Tiles/benchmarks-index.html).

- You're no longer need installed **MapTiler Pro** or packed to `.exe` **Gdal2Tiles**, but you need to install **Docker**. Therefore, benchmarks can be run only on devices with Linux or Windows 10 (*with WSL2 recommended*).
- Always using latest version of **MapTiler** and **Gdal2Tiles** to run against it (*thanks to Docker*);

**Changes to parameters:**

- All parameters, except `-i/--input` are removed. By using `input` you can select **input geotiff** to run benchmarks against;

**New dependencies:**

- **BenchmarkDotNet** ver. **0.12.1**;
- **Microsoft.CodeAnalysis.NetAnalyzers** package ver. **5.0.1** (*build from source only*);

**Updated dependencies:**

- Update **CommandLineParser** to **2.8.0**;

### Tests

Tests are now completely rewritten to cover **~94%** of **GTiff2Tiles.Core** new API and some usecases.

**New dependencies:**

Added **coverlet** dependency to generate code coverage reports.

- **coverlet.collector** ver. **1.3.0**;
- **coverlet.msbuild** ver. **2.9.0**;
- Add **Microsoft.CodeAnalysis.NetAnalyzers** package ver. **5.0.1** (*build from source only*);

**Updated dependencies:**

- Update **NUnit3TestAdapter** to **3.17.0**;
- Update **Microsoft.NET.Test.Sdk** to **16.8.3**;

### Documentation

- Core docs now auto-generated by **docfx**;
- Docs migrated from **GitHub Wiki** to **GitHub Pages** (*1.4.x docs will stay on GitHub Wiki*) and **.pdf**-files;
- Completed and improved all **xml-docs**;
- Moved all project-specific docs from **README.md** to their own files;

## 21.10.2019 – Released 1.4.1

Changes since 1.4.0:

### Overall

- Now library also supports **.NET Standard 2.0**;
- Fixed binary releases;
- Return of [System.Threading.Tasks.Extensions](https://www.nuget.org/packages/System.Threading.Tasks.Extensions) package for **netstandard2.0 build**;

## 20.10.2019 – Released 1.4.0

Changes since 1.3.1:

### Overall

**BREAKING CHANGES:**

- Solution is rebuild with **.NET Core 3.0 SDK**;
- Library and some other projects now supports **Linux x64** and **Windows x64** operating systems. Read more below;
- Renamed all async methods to `methodNameAsync`;

**Other:**

- Updated **CommandLineParser** package to **2.6.0**;
- Updated **NUnit3TestAdapter** package to **3.15.1**;
- Replaced **NetVips.Native.win-x64** package with cross-platform **NetVips.Native** package;
- Updated **NetVips.Native** package to **8.8.3**;
- Replaced **GDAL** and **GDAL.Native** packages with **MaxRev.Gdal.Core**, **MaxRev.Gdal.LinuxRuntime.Minimal** and **MaxRev.Gdal.WindowsRuntime.Minimal** packages;
- As result of the above change, **gdal’s binaries** updated from **2.4.2** to **3.0.1**;

### Core

**BREAKING CHANGES:**

- Removed **Image.Gdal.ConfigureOgr** method (now everything initializes with **Image.Gdal.ConfigureGdal** method);

**Other:**

- Now supports **Linux x64** and **Windows x64** operating systems;

- Removed **System.Threading.Tasks.Extensions** package;

### Console

- Now supports **Linux x64** and **Windows x64** operating systems;
- Removed **System.Threading.Tasks.Extensions** package;

### GUI

- Updated **MaterialDesignExtensions** package to **2.8.0**;
- Removed **MahApps.Metro** package;
- Replaced **MahApps.Metro.MetroWindow** with **MaterialDesignExtensions.MaterialWindow**;
- Removed **System.Threading.Tasks.Extensions** package;

### Tests

- Add **Microsoft.NET.Test.Sdk** package;

### Benchmarks

- Now supports **Linux x64** and **Windows x64** operating systems;
- Removed **System.Threading.Tasks.Extensions** package;

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
