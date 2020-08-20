# GTiff2Tiles.Benchmarks

**GTiff2Tiles.Tests** is a benchmarking project for **GTiff2Tiles.Core**.

The following benchmarks were made at **06.07.2019**.

**MapTiler Pro** was running as process *maptiler.exe*, **GTiff2Tiles** tiling was called from library and **gdal2tiles.py** was converted by **PyInstaller** into *Gdal2Tiles.exe* and was running as process.

Unfortunately, I couldn’t create *Gdal2Tiles.exe* with **multiprocessing**, so it’s only in **single-threaded** tests for it at the moment. I’ll try to fix that moment in the future and update benchmarks as well.

Time format in tables: `{minutes}:{seconds}:{milliseconds}`.

Benchmarks were made on PC with **Windows 10 x64 (18362.239)** equipped with **Intel Core i7 6700K 4.0 GHz**.

## Versions

Used **MapTiler Pro** version is **0.5.3**. Used **gdal2tiles.py** version from **[GDAL repo](https://github.com/OSGeo/gdal/blob/master/gdal/swig/python/scripts/gdal2tiles.py)** (GDAL’s version **3.0.0**, **06.07.2019**). Used **GTiff2Tiles.Core** version is **1.2.0.139**.

## Requirements

- MapTiler Pro 0.5.3 or newer;
- gdal2tiles.py, converted to `.exe` and placed in directory `Gdal2Tiles` near benchmarks binaries;

If you’re using Windows 7 SP1, you can experience weird error with **GDAL** package. It’s recommended to install [KB2533623](<https://www.microsoft.com/en-us/download/details.aspx?id=26764>) to fix it. You can read about this Windows update on [MSDN](<https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot>).

## Build dependencies

- GTiff2Tiles.Core;
- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) – 2.8.0;

## Usage

*TODO: needs update for 2.0.0 release. Current version is written for old dev pre-releases.*

| Short |   Long    |          Description          | Required? |
| :---: | :-------: | :---------------------------: | :-------: |
|  -i   |  --input  |    Full path to input file    |    Yes    |
|  -o   | --output  | Full path to output directory |    Yes    |
|  -t   |  --temp   |  Full path to temp directory  |    No     |
|       |  --minz   |     Minimum cropped zoom      |    Yes    |
|       |  --maxz   |     Maximum cropped zoom      |    Yes    |
|       | --threads |         Threads count         |    No     |
|       | --version |        Current version        |           |
|       |  --help   | Message about console options |           |

Simple example looks like this: `./GTiff2Tiles.Benchmarks -i "D:/Examples/Input.tif" -o "D:/Examples/Output" -t "D:/Examples/Temp" --minz 8 –maxz 11 --threads 3`

Also take a look at [Start.ps1](https://github.com/Gigas002/GTiff2Tiles/blob/master/GTiff2Tiles.Benchmarks/Start.ps1) **PowerShell** script for automating and simplifying the work. Note, that running this script requires installed **PowerShell** or **[PowerShell Core](https://github.com/PowerShell/PowerShell)** (also available on **Linux**/**OSX** systems!).

### Detailed options description

**-i/--input** is `string`, representing full path to input **GeoTIFF** file. Please, specify the path in double quotes (`“like this”`) if it contains spaces.

**-o/--output** is `string`, representing full path to directory, where tiles in will be created. Please, specify the path in double quotes (`“like this”`) if it contains spaces. **Directory should be empty.**

**-t/--temp** is `string`, representing full path to temporary directory. Please, specify the path in double quotes (`“like this”`) if it contains spaces. Inside will be created directory, which name is a **timestamp** in format `yyyyMMddHHmmssfff`. By default – the same directory, where application is located.

**--minz** is `int` parameter, representing minimum zoom, which you want to crop.

**--maxz** is `int` parameter, representing maximum zoom, which you want to crop.

**--threads** is `int` parameter, representing threads count. By default (if not set) uses **5 threads**.

### Offline docs

Offline docs are also available as [pdf](https://github.com/Gigas002/GTiff2Tiles/blob/master/GTiff2Tiles.Benchmarks/benchmarks-index.pdf) and distributed alongside the application.

## Input data

As input data was used **4326** GeoTIFF, located in repo’s directory: `Examples/Input/Benchmark.tif`.

## Benchmarks metadata

The differences between benchmarks are only in **maximum zoom** and **threads count** values.

**GTiff2Tiles** was running with the following arguments: `-i {inputFilePath} -o {outputDirectoryPath} -t {tempDirectoryPath} --tms true --minz 0 --maxz {maxZ} --threads {threadsCount}`.

**MapTiler Pro** was running with the following arguments: `-geodetic -tms -resampling cubic -f png32 -P {threadsCount} -o {outputDirectoryPath} -work_dir {tempDirectoryPath} -srs EPSG:4326 -zoom 0 {maxZ} {inputFilePath}`.

**gdal2tiles** was running with the following arguments: `-s EPSG:4326 -p geodetic -r cubic --tmscompatible -z 0-{maxZ} {inputFilePath} {outputDirectoryPath}`.

## Results

Avarage from 10 runs:

- Threads count = 1;
- Maximum zoom = 16;

| GTiff2Tiles | MapTiler Pro | Gdal2Tiles |
| :---------: | :----------: | :--------: |
|  00:27:061  |  00:33:901   | 01:45:901  |

- Threads count = 5;
- Maximum zoom = 16;

| GTiff2Tiles | MapTiler Pro |
| :---------: | :----------: |
|  00:07:723  |  00:15:057   |

- Threads count = 1;
- Maximum zoom = 17;

| GTiff2Tiles | MapTiler Pro | Gdal2Tiles |
| :---------: | :----------: | :--------: |
|  01:41:500  |  02:13:183   | 06:43:683  |

- Threads count = 5;
- Maximum zoom = 17;

| GTiff2Tiles | MapTiler Pro |
| :---------: | :----------: |
|  00:30:915  |  00:53:502   |
