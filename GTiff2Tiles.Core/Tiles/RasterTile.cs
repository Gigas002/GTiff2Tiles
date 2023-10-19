using System.Threading.Channels;
using GTiff2Tiles.Core.Args;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Images;
using NetVips;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Tiles;

/// <summary>
/// <see cref="Raster"/> <see cref="Tile"/>
/// </summary>
public class RasterTile : Tile
{
    /// <summary>
    /// <see cref="BandsCount"/> backing field
    /// </summary>
    private int _bandsCount = DefaultBandsCount;

    #region Properties/Constants

    /// <summary>
    /// Default count of bands
    /// </summary>
    public const int DefaultBandsCount = 4;

    /// <summary>
    /// Count of bands in <see cref="RasterTile"/>
    /// </summary>
    public int BandsCount
    {
        get => _bandsCount;
        set
        {
            if (value <= 0 || value > 4) throw new ArgumentOutOfRangeException(nameof(value));

            _bandsCount = value;
        }
    }

    /// <summary>
    /// Interpolation of this <see cref="RasterTile"/>
    /// </summary>
    public NetVips.Enums.Kernel Interpolation { get; set; } = NetVips.Enums.Kernel.Lanczos3;

    #endregion

    #region Constructors

    /// <inheritdoc cref="Tile(Number,CoordinateSystem,Size,bool)"/>
    /// <param name="number"></param>
    /// <param name="coordinateSystem"></param>
    /// <param name="size"></param>
    /// <param name="tmsCompatible"></param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public RasterTile(Number number, CoordinateSystem coordinateSystem, Size size = null, bool tmsCompatible = false) :
        base(number, coordinateSystem, size, tmsCompatible) { }

    /// <inheritdoc cref="Tile(GeoCoordinate,GeoCoordinate,int,Size,bool)"/>
    /// <param name="minCoordinate"></param>
    /// <param name="maxCoordinate"></param>
    /// <param name="zoom"></param>
    /// <param name="size"></param>
    /// <param name="tmsCompatible"></param>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom, Size size = null,
                      bool tmsCompatible = false) : base(minCoordinate, maxCoordinate, zoom, size, tmsCompatible) { }

    #endregion

    #region Methods

    // TODO: TESTS!

    /// <summary>
    /// Create <see cref="Image"/> for one <see cref="RasterTile"/>
    /// from input <see cref="Image"/> or tile cache
    /// </summary>
    /// <param name="sourceGeoTiff">Source <see cref="IGeoTiff"/></param>
    /// <param name="tileCache">Source <see cref="Image"/>
    /// or tile cache</param>
    /// <param name="readArea">Source area</param>
    /// <param name="writeArea">Tile write area</param>
    /// <returns>Ready <see cref="Image"/> for <see cref="RasterTile"/></returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="ArgumentException"/>
    public Image CreateImage(IGeoTiff sourceGeoTiff, Image tileCache, Area readArea, Area writeArea)
    {
        // TODO: check geotiff?

        #region Preconditions checks

        if (tileCache == null) throw new ArgumentNullException(nameof(tileCache));
        if (readArea == null) throw new ArgumentNullException(nameof(readArea));
        if (writeArea == null) throw new ArgumentNullException(nameof(writeArea));

        #endregion


        // Scaling calculations
        double xScale = (double)writeArea.Size.Width / readArea.Size.Width;
        double yScale = (double)writeArea.Size.Height / readArea.Size.Height;

        // Crop and resize tile
        Image tempTileImage = tileCache.Crop((int)readArea.OriginCoordinate.X, (int)readArea.OriginCoordinate.Y,
                                             readArea.Size.Width, readArea.Size.Height)
                                       .Resize(xScale, Interpolation, yScale);

        // Add alpha channel if needed
        Band.AddDefaultBands(ref tempTileImage, BandsCount);

        // Make transparent image and insert tile
        return Image.Black(Size.Width, Size.Height).NewFromImage(new int[BandsCount])
                    .Insert(tempTileImage, (int)writeArea.OriginCoordinate.X, (int)writeArea.OriginCoordinate.Y);
    }

    /// <inheritdoc />
    public override void WriteToFile(IGeoTiff sourceGeoTiff, IWriteTilesArgs args)
    {
        // Get postitions and sizes for current tile
        (Area readArea, Area writeArea)? areas = Area.GetAreas(sourceGeoTiff, this);

        if (areas == null) return;

        (Area readArea, Area writeArea) = areas.Value;
        WriteRasterTilesArgs rasterArgs = (WriteRasterTilesArgs)args;

        using Image tileImage = CreateImage(sourceGeoTiff, rasterArgs.TileCache, readArea, writeArea);

        tileImage.WriteToFile(Path);
    }

    /// <inheritdoc />
    public override IEnumerable<byte> WriteToEnumerable(IGeoTiff sourceGeoTiff, IWriteTilesArgs args)
    {
        // Get postitions and sizes for current tile
        (Area readArea, Area writeArea)? areas = Area.GetAreas(sourceGeoTiff, this);

        if (areas == null) return null;

        (Area readArea, Area writeArea) = areas.Value;

        WriteRasterTilesArgs rasterArgs = args as WriteRasterTilesArgs;

        using Image tileImage = CreateImage(sourceGeoTiff, rasterArgs.TileCache, readArea, writeArea);

        return tileImage.WriteToBuffer(GetExtensionString());
    }

    /// <inheritdoc />
    public override bool WriteToChannel<T>(IGeoTiff sourceGeoTiff, ChannelWriter<T> tileWriter, IWriteTilesArgs args)
    {
        Bytes = WriteToEnumerable(sourceGeoTiff, args);

        return Bytes != null && Validate(false) && tileWriter.TryWrite(this as T);
    }

    /// <inheritdoc />
    public override IEnumerable<byte> WriteOverviewTileBytes<T>(T[] allBaseTiles)
    {
        Number[] numbers = Number.GetLowerNumbers();
        T[] lowerTiles = new T[4];
        lowerTiles[0] = allBaseTiles.FirstOrDefault(t => t.Number == numbers[0]);
        lowerTiles[1] = allBaseTiles.FirstOrDefault(t => t.Number == numbers[1]);
        lowerTiles[2] = allBaseTiles.FirstOrDefault(t => t.Number == numbers[2]);
        lowerTiles[3] = allBaseTiles.FirstOrDefault(t => t.Number == numbers[3]);
        bool isBuffered = lowerTiles[0].Bytes != null;

        Image image = WriteOverviewTileImage(lowerTiles, isBuffered);

        byte[] result = image?.WriteToBuffer(GetExtensionString(Extension));
        image?.Dispose();

        return result;
    }

    /// <summary>
    /// Create <see cref="Image"/> from 4 underlying lower tiles
    /// </summary>
    /// <typeparam name="T">Inheritors of <see cref="ITile"/></typeparam>
    /// <param name="fourBaseTiles">4 lower tiles</param>
    /// <param name="isBuffered">Do base tiles have bytes or they use paths?</param>
    /// <returns>Upper tile <see cref="Image"/></returns>
    public Image WriteOverviewTileImage<T>(T[] fourBaseTiles, bool isBuffered) where T : class, ITile
    {
        if (!isBuffered)
        {
            foreach (T tile in fourBaseTiles) tile.Bytes = File.ReadAllBytes(tile.Path);
        }

        Image[] images = new Image[4];

        bool empty = true;

        for (int i = 0; i < 4; i++)
        {
            Size size = new(fourBaseTiles[i].Size.Width / 2, fourBaseTiles[i].Size.Height / 2);
            byte[] bytes = fourBaseTiles[i].Bytes?.ToArray();

            if (bytes.Any())
            {
                empty = false;
                images[i] = Image.NewFromBuffer(bytes).ThumbnailImage(size.Width, size.Height);
            }
            else { images[i] = Image.Black(size.Width, size.Height, BandsCount); }
        }

        return empty ? null : Image.Arrayjoin(images, 2);
    }

    #endregion
}
