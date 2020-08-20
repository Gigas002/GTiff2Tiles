# GTiff2Tiles.Console

**GTiff2Tiles.Console** is a simple console application, that implements methods from **GTiff2Tiles.Core** to create tiles. The app is available to download from [GitHub Releases Page](https://github.com/Gigas002/GTiff2Tiles/releases) and Docker Images are available on [Docker Hub](https://hub.docker.com/r/gigas002/gtiff2tiles-console) and [GitHub Packages Feed](https://github.com/Gigas002/GTiff2Tiles/packages).

Supports **only GeoTIFF** as input data and creates **geodetic or mercator** tiles on output in **[tms](https://wiki.osgeo.org/wiki/Tile_Map_Service_Specification)** or **non-tms** (*Google maps like*) structure.
Any **GeoTIFF** (with less, than **5 bands**) on input is supported, if it's not **EPSG:4326** or **EPSG:3857**, it'll be converted to your selected target coordinate system and saved inside **temp** directory before cropping.

## Requirements

Application runs on **Linux x64** (*tested on Ubuntu 18.04+*) and **Windows x64** (*tested on Win 7 SP1+*) operating systems.

If you’re using **Windows 7 SP1**, you can experience weird error with **GDAL** packages. It’s recommended to install [KB2533623](https://www.microsoft.com/en-us/download/details.aspx?id=26764) to fix it. You can read about this Windows update on [MSDN](https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot).

## Usage

*TODO: needs update for 2.0.0 release. Current version is written for old dev pre-releases.*

| Short |    Long     |                 Description                 | Required? |
| :---: | :---------: | :-----------------------------------------: | :-------: |
|  -i   |   --input   |           Full path to input file           |    Yes    |
|  -o   |  --output   |        Full path to output directory        |    Yes    |
|  -t   |   --temp    |         Full path to temp directory         |    No     |
|       |   --minz    |            Minimum cropped zoom             |    Yes    |
|       |   --maxz    |            Maximum cropped zoom             |    Yes    |
|       |    --tms    | Do you want to create tms-compatible tiles? |    No     |
|       | --extension |            Ready tiles extension            |    No     |
|       |  --threads  |                Threads count                |    No     |
|       |  --version  |               Current version               |           |
|       |   --help    |        Message about console options        |           |

Simple example looks like this: `./GTiff2Tiles.Console -i "D:/Examples/Input.tif" -o "D:/Examples/Output" -t "D:/Examples/Temp" --minz 8 –maxz 11 -a crop --tms true --extension .webp --threads 3`

Also take a look at [Start.ps1](https://github.com/Gigas002/GTiff2Tiles/blob/master/GTiff2Tiles.Console/Start.ps1) **PowerShell** script for automating and simplifying the work. Note, that running this script requires installed **PowerShell** or **[PowerShell Core](https://github.com/PowerShell/PowerShell)** (also available on **Linux**/**OSX** systems!).

### Detailed options description

**-i/--input** is `string`, representing full path to input **GeoTIFF** file. Please, specify the path in double quotes (`“like this”`) if it contains spaces.

**-o/--output** is `string`, representing full path to directory, where tiles in will be created. Please, specify the path in double quotes (`“like this”`) if it contains spaces. **Directory should be empty.**

**-t/--temp** is `string`, representing full path to temporary directory. Please, specify the path in double quotes (`“like this”`) if it contains spaces. Inside will be created directory, which name is a **timestamp** in format `yyyyMMddHHmmssfff`. By default – the same directory, where application is located.

**--minz** is `int` parameter, representing minimum zoom, which you want to crop.

**--maxz** is `int` parameter, representing maximum zoom, which you want to crop.

**--tms** is `string`, which shows if you want to create tms-compatible or non-tms-compatible tiles on output. Can have values `true` or `false`. By default is `true`.

**--extension** is a `string`, representing ready tiles extension. By default is set to `.png`. Currently supported extensions are: `.webp`, `.jpg`, `.png`.

**--threads** is `int` parameter, representing threads count. By default (if not set) uses **5 threads**.

### Offline docs

Offline docs are also available as [pdf](https://github.com/Gigas002/GTiff2Tiles/blob/master/GTiff2Tiles.Console/console-index.pdf) and distributed alongside the application.

## Build dependencies

- GTiff2Tiles.Core;
- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) – 2.8.0;
