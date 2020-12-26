# Simple script to run GTiff2Tiles.Console application, using PowerShell or Powershell.Core on any system.
# Place this script near GTiff2Tiles.Console binaries and uncomment needed lines.
# Read full docs for application on https://gigas002.github.io/GTiff2Tiles/console-index.html or in offline file.

# Minimal run
# Run tiling of Input.tif file to Output directory for 0-12 zooms using auto-calculated threads number, temp files throw in current directory.
# Tile's extension is .png, tmscompatible, geodetic, lanczos3 interpolation, 4 bands, 256x256 resolution.
# Tile cache set to 1000, memory cache 2147483648 (2Gb).
# You'll see the progress, but not time left.
# ./GTiff2Tiles.Console -i "D:/Examples/Input.tif" -o "D:/Examples/Output" --minz 0 --maxz 12

# Overriding all args
# Run tiling of Input.tif file to Output directory for 0-12 zooms using 5 threads, temp files throw in tmp directory.
# Tile's extension is .webp, tmscompatible, mercator, cubic interpolation, 3 bands, 128x128 resolution.
# Tile cache set to 1001, memory cache 2147483649.
# You'll see the progress and time left.
# ./GTiff2Tiles.Console -i "D:/Examples/Input.tif" -o "D:/Examples/Output" --minz 0 --maxz 12 --threads 5 --extension .webp -t "D:/Examples/tmp" --tms false -c mercator --interpolation cubic -b 3 --tilecache 1001 -m 2147483649 --progress true --timeleft true --tilesize 128 --tmr true
