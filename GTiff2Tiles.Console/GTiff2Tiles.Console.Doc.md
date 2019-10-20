# GTiff2Tiles.Console documentation

The following documentation is written for **1.4.0** release of application.

## Requirements

Application runs on **Linux x64**, **OSX x64** and **Windows x64** operating systems.

If you’re using Windows 7 SP1, you can experience weird error with **GDAL** package. It’s recommended to install [KB2533623](<https://www.microsoft.com/en-us/download/details.aspx?id=26764>) to fix it. You can read about this Windows update on [MSDN](<https://support.microsoft.com/en-us/help/2533623/microsoft-security-advisory-insecure-library-loading-could-allow-remot>).

## Usage

| Short |    Long     |                 Description                 | Required? |
| :---: | :---------: | :-----------------------------------------: | :-------: |
|  -i   |   --input   |           Full path to input file           |    Yes    |
|  -o   |  --output   |        Full path to output directory        |    Yes    |
|  -t   |   --temp    |         Full path to temp directory         |    Yes    |
|       |   --minz    |            Minimum cropped zoom             |    Yes    |
|       |   --maxz    |            Maximum cropped zoom             |    Yes    |
|  -a   | --algorithm |          Algorithm to create tiles          |    Yes    |
|       |    --tms    | Do you want to create tms-compatible tiles? |    Yes    |
|       |  --threads  |                Threads count                |    No     |
|       |  --version  |               Current version               |           |
|       |   --help    |        Message about console options        |           |

Simple example looks like this: `./GTiff2Tiles.Console -i "D:/Examples/Input.tif" -o "D:/Examples/Output" -t "D:/Examples/Temp" --minz 8 –maxz 11 -a crop --tms true --threads 3`

Also take a look at `Start.ps1` **PowerShell** script for automating the work. Note, that running this script requires installed **PowerShell** or **[PowerShell Core](https://github.com/PowerShell/PowerShell)** for **Linux**/**OSX** systems.

## Detailed options description

**input** is `string`, representing full path to input **GeoTIFF** file. Please, specify the path in double quotes (`“like this”`) if it contains spaces.

**output** is `string`, representing full path to directory, where tiles in will be created. Please, specify the path in double quotes (`“like this”`) if it contains spaces. **Directory should be empty.**

**temp** is `string`, representing full path to temporary directory. Please, specify the path in double quotes (`“like this”`) if it contains spaces. Inside will be created directory, which name is a **timestamp** in format `yyyyMMddHHmmssfff`.

**minz** is `int` parameter, representing minimum zoom, which you want to crop.

**maxz** is `int` parameter, representing maximum zoom, which you want to crop.

**algorithm** is `string`, representing cropping algorithm. Can be **crop** or **join**. When using **crop**, the input image will be cropped for each zoom. When using **join**, the input image will be cropped for the lowest zoom, and the upper tiles created by joining lowest ones.

**tms** is `string`, which shows if you want to create tms-compatible or non-tms-compatible tiles on output. Can have values `true` or `false`.

**threads** is `int` parameter, representing threads count. By default (if not set) uses **5 threads**.
