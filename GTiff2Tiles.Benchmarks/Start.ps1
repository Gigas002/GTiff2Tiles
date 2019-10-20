# Simple script to run GTiff2Tiles.Benchmarks application, using PowerShell or Powershell.Core on any system.
# Place this script near GTiff2Tiles.Benchmarks file and uncomment needed lines.
# Read full docs for application in GTiff2Tiles.Benchmarks.Doc.md or GTiff2Tiles.Benchmarks.Doc.pdf files.

# Run tiling from Input.tif file to Output directory for 8-11 zooms, crop algorythm, tms-compatible, in 3-threads mode.
# ./GTiff2Tiles.Benchmarks -i "D:/Examples/Input.tif" -o "D:/Examples/Output" -t "D:/Examples/Temp" --minz 8 –maxz 11 -a crop --tms true --threads 3