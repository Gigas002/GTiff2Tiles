# GTiff2Tiles.GUI

**GTiff2Tiles.GUI** is a simple GUI app, that implements methods from **GTiff2Tiles.Core** to create tiles. The app is available to download from [GitHub Releases Page](https://github.com/Gigas002/GTiff2Tiles/releases).

Supports **only GeoTIFF** as input data and creates **geodetic or mercator** tiles on output in **[tms](https://wiki.osgeo.org/wiki/Tile_Map_Service_Specification)** or **non-tms** (*Google maps like*) structure.
Any **GeoTIFF** (with less, than **5 bands**) on input is supported, if it's not **EPSG:4326** or **EPSG:3857**, it'll be converted to your selected target coordinate system and saved inside **temp** directory before cropping.

![Main page](images/MainPage.png)

## Requirements

Application runs only on **Windows x64** (*tested on Win 7 SP1+*) operating system.

If you’re using **Windows 7 SP1**, you can experience weird error with **GDAL** package. It’s recommended to install [KB2533623](<https://www.microsoft.com/en-us/download/details.aspx?id=26764>) to fix it. You can read about this Windows update on [MSDN](<https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot>).

### Build dependencies

- GTiff2Tiles.Core;
- [Prism.DryIoc](https://www.nuget.org/packages/Prism.DryIoc) – 8.0.0.1850-pre;
- [MaterialDesignColors](https://www.nuget.org/packages/MaterialDesignColors) – 1.2.6;
- [MaterialDesignThemes](https://www.nuget.org/packages/MaterialDesignThemes) – 3.1.3;
- [MaterialDesignExtensions](https://www.nuget.org/packages/MaterialDesignExtensions) – 3.2.0;
