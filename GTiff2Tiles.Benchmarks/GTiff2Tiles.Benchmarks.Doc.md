# GTiff2Tiles.Benchmarks documentation

The following documentation is written for **1.4.1** release of application.

## Requirements

- MapTiler Pro 0.5.3 or newer;
- Gdal2Tiles.py, converted to `.exe` and placed in directory `Gdal2Tiles` near benchmarks binaries;

If you’re using Windows 7 SP1, you can experience weird error with **GDAL** package. It’s recommended to install [KB2533623](<https://www.microsoft.com/en-us/download/details.aspx?id=26764>) to fix it. You can read about this Windows update on [MSDN](<https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot>).

## Dependencies

- GTiff2Tiles.Core;
- [CommandLineParser](https://www.nuget.org/packages/CommandLineParser/) – 2.6.0;

## Usage

| Short |   Long    |          Description          | Required? |
| :---: | :-------: | :---------------------------: | :-------: |
|  -i   |  --input  |    Full path to input file    |    Yes    |
|  -o   | --output  | Full path to output directory |    Yes    |
|  -t   |  --temp   |  Full path to temp directory  |    Yes    |
|       |  --minz   |     Minimum cropped zoom      |    Yes    |
|       |  --maxz   |     Maximum cropped zoom      |    Yes    |
|       | --threads |         Threads count         |    No     |
|       | --version |        Current version        |           |
|       |  --help   | Message about console options |           |

Simple example looks like this: `./GTiff2Tiles.Benchmarks -i "D:/Examples/Input.tif" -o "D:/Examples/Output" -t "D:/Examples/Temp" --minz 8 –maxz 11 --threads 3`

Also take a look at `Start.ps1` **PowerShell** script for automating the work. Note, that running this script requires installed **PowerShell** or **[PowerShell Core](https://github.com/PowerShell/PowerShell)** for **Linux**/**OSX** systems.

## Detailed options description

**input** is `string`, representing full path to input **GeoTIFF** file. Please, specify the path in double quotes (`“like this”`) if it contains spaces.

**output** is `string`, representing full path to directory, where tiles in will be created. Please, specify the path in double quotes (`“like this”`) if it contains spaces. **Directory should be empty.**

**temp** is `string`, representing full path to temporary directory. Please, specify the path in double quotes (`“like this”`) if it contains spaces. Inside will be created directory, which name is a **timestamp** in format `yyyyMMddHHmmssfff`.

**minz** is `int` parameter, representing minimum zoom, which you want to crop.

**maxz** is `int` parameter, representing maximum zoom, which you want to crop.

**threads** is `int` parameter, representing threads count. By default (if not set) uses **5 threads**.
