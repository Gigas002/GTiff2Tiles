using System.Diagnostics;
using System.Threading.Channels;
using GTiff2Tiles.Core.Args;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Exceptions;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Localization;
using GTiff2Tiles.Core.Tiles;
using NetVips;

namespace GTiff2Tiles.Core;
// TODO: WIP

public static class TileGenerator
{
    public static void WriteRasterTilesToDirectory(Raster raster, WriteRasterTilesArgs args)
    {
        #region Preconditions checks

        CheckHelper.CheckDirectory(args.OutputDirectoryPath, true);
        // minZ and maxZ checked inside Number.GetCount
        args.TileSize ??= Tile.DefaultSize;

        // interpolation is checked on lower levels
        // bandsCount checked inside RasterTile ctor
        //if (rasterArgs.TileCacheCount <= 0) throw new ArgumentException(nameof(args));

        ParallelOptions parallelOptions = new();
        if (args.ThreadsCount > 0) parallelOptions.MaxDegreeOfParallelism = args.ThreadsCount;

        // It's safe to set progress to null

        Stopwatch stopwatch = args.TimePrinter == null ? null : Stopwatch.StartNew();

        // TODO: don't count if progress is null
        int tilesCount = Number.GetCount(raster.MinCoordinate, raster.MaxCoordinate, args.MinZ,
                                         args.MaxZ, args.TmsCompatible,
                                         args.TileSize);

        // if there's no tiles to crop
        if (tilesCount <= 0) throw new RasterException(Strings.NoTilesToCrop);

        double counter = 0.0;

        #endregion

        // Create tile cache to read data from it
        using Image tileCache = raster.Data.Tilecache(args.TileSize.Width,
                                                      args.TileSize.Height,
                                                      args.TileCacheCount, threaded: true);

        // For each zoom
        for (int zoom = args.MinZ; zoom <= args.MaxZ; zoom++)
        {
            // Get tiles min/max numbers
            GeoCoordinate minCoord = args.MinCoordinate == null ? raster.MinCoordinate : args.MinCoordinate;
            GeoCoordinate maxCoord = args.MaxCoordinate == null ? raster.MaxCoordinate : args.MaxCoordinate;
            (Number minNumber, Number maxNumber) =
                GeoCoordinate.GetNumbers(minCoord, maxCoord, zoom, args.TileSize, args.TmsCompatible);

            // For each tile on given zoom calculate positions/sizes and save as file
            for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
            {
                int y = tileY;
                int z = zoom;

                Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, x =>
                {
                    Number tileNumber = new(x, y, z);
                    using RasterTile tile = new(tileNumber, raster.GeoCoordinateSystem, args.TileSize, args.TmsCompatible)
                    {
                        Extension = args.TileExtension,
                        BandsCount = args.BandsCount,
                        Interpolation = args.TileInterpolation
                    };

                    tile.WriteToFile(raster, args);

                    counter++;
                    double percentage = counter / tilesCount * 100.0;
                    args.Progress?.Report(percentage);

                    //ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch, rasterArgs.TimePrinter);
                });
            }
        }
    }

    public static void WriteRasterTilesToChannel(Raster raster, ChannelWriter<RasterTile> tileWriter, WriteRasterTilesArgs args)
    {
        #region Preconditions checks

        // channelWriter is checked on lower levels
        // minZ and maxZ checked inside Number.GetCount
        args.TileSize ??= Tile.DefaultSize;

        // interpolation is checked on lower levels
        // bandsCount checked inside RasterTile ctor
        //if (args.TileCacheCount <= 0) throw new ArgumentOutOfRangeException(nameof(tileCacheCount));

        ParallelOptions parallelOptions = new();
        if (args.ThreadsCount > 0) parallelOptions.MaxDegreeOfParallelism = args.ThreadsCount;

        // It's safe to set progress to null

        Stopwatch stopwatch = args.TimePrinter == null ? null : Stopwatch.StartNew();

        int tilesCount = Number.GetCount(raster.MinCoordinate, raster.MaxCoordinate,
                                         args.MinZ, args.MaxZ,
                                         args.TmsCompatible, args.TileSize);

        // if there's no tiles to crop
        if (tilesCount <= 0) throw new RasterException(Strings.NoTilesToCrop);

        double counter = 0.0;

        #endregion

        // Create tile cache to read data from it
        using Image tileCache = raster.Data.Tilecache(args.TileSize.Width,
                                                      args.TileSize.Height,
                                                      args.TileCacheCount, threaded: true);

        void MakeTile(int x, int y, int z)
        {
            Number tileNumber = new(x, y, z);
            RasterTile tile =
                new(tileNumber, raster.GeoCoordinateSystem, args.TileSize, args.TmsCompatible)
                {
                    BandsCount = args.BandsCount, Interpolation = args.TileInterpolation
                };

            // Should not throw exception if tile was skipped
            // ReSharper disable once AccessToDisposedClosure
            if (!tile.WriteToChannel(raster, tileWriter, args)) return;

            // Report progress
            //counter++;
            //double percentage = counter / tilesCount * 100.0;
            //progress?.Report(percentage);

            //ProgressHelper.PrintEstimatedTimeLeft(percentage, stopwatch, printTimeAction);
        }

        // For each zoom
        for (int zoom = args.MinZ; zoom <= args.MaxZ; zoom++)
        {
            // Get tiles min/max numbers
            (Number minNumber, Number maxNumber) =
                GeoCoordinate.GetNumbers(raster.MinCoordinate, raster.MaxCoordinate,
                                         zoom, args.TileSize, args.TmsCompatible);

            // For each tile on given zoom calculate positions/sizes and save as file
            for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
            {
                int y = tileY;
                int z = zoom;

                Parallel.For(minNumber.X, maxNumber.X + 1, parallelOptions, x => MakeTile(x, y, z));
            }
        }
    }

    public static IEnumerable<RasterTile> WriteRasterTilesToEnumerable(Raster raster, WriteRasterTilesArgs args)
    {
        using Image tileCache = raster.Data.Tilecache(args.TileSize.Width,
                                                      args.TileSize.Height,
                                                      args.TileCacheCount, threaded: true);
        args.TileCache = tileCache;

        // For each specified zoom
        for (int zoom = args.MinZ; zoom <= args.MaxZ; zoom++)
        {
            // Get tiles min/max numbers
            (Number minNumber, Number maxNumber) =
                GeoCoordinate.GetNumbers(raster.MinCoordinate, raster.MaxCoordinate, zoom, args.TileSize, args.TmsCompatible);

            // For each tile on given zoom calculate positions/sizes and save as file
            for (int tileY = minNumber.Y; tileY <= maxNumber.Y; tileY++)
            {
                for (int tileX = minNumber.X; tileX <= maxNumber.X; tileX++)
                {
                    Number tileNumber = new(tileX, tileY, zoom);
                    using RasterTile tile = new(tileNumber, raster.GeoCoordinateSystem, args.TileSize, args.TmsCompatible)
                    {
                        Extension = args.TileExtension,
                        BandsCount = args.BandsCount,
                        Interpolation = args.TileInterpolation,
                    };

                    tile.Bytes = tile.WriteToEnumerable(raster, args);

                    //tile.Dispose();
                    //yield return new RasterTile(tile.Number, GeoCoordinateSystem, tile.Size, tileBytes,
                    //                            tile.Extension, tile.TmsCompatible, tile.BandsCount,
                    //                            tile.Interpolation);

                    yield return tile;
                }
            }
        }
    }

    public static void WriteOverviewRasterTilesToChannel(IEnumerable<RasterTile> baseTiles,
                                                         ChannelWriter<RasterTile> tileWriter,
                                                         WriteRasterTilesArgs args)
    {
        for (int z = args.MinZ; z <= args.MaxZ; z++)
        {
            (Number minNumber, Number maxNumber) =
                GeoCoordinate.GetNumbers(args.MinCoordinate, args.MaxCoordinate, z, args.TileSize,
                                         args.TmsCompatible);

            for (int x = minNumber.X; x <= maxNumber.X; x++)
            {
                int x1 = x;
                int z1 = z;
                Parallel.For(minNumber.Y, maxNumber.Y + 1, y =>
                {
                    Number number = new(x1, y, z1);

                    RasterTile tile = new(number, args.GeoCoordinateSystem, args.TileSize, args.TmsCompatible)
                    {
                        Extension = args.TileExtension, BandsCount = args.BandsCount,
                        Interpolation = args.TileInterpolation
                    };
                    tile.Bytes = tile.WriteOverviewTileBytes(baseTiles.ToArray());

                    tileWriter.TryWrite(tile);
                });
            }
        }
    }

    public static void WriteLowerRasterTiles(RasterTile tile)
    {
        // TODO: in ITIle?
        // Writes LOWER tiles of this current tile
    }

    private static bool CheckRasterArgs(WriteRasterTilesArgs args)
    {
        return true;
    }
}