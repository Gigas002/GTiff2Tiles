using System;
using System.Collections.Generic;
using System.Threading.Channels;
using GTiff2Tiles.Core.Args;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;
using NetVips;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Tiles
{
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
        public Interpolation Interpolation { get; set; } = Interpolation.Lanczos3;

        #endregion

        #region Constructors

        /// <inheritdoc cref="Tile(Number,CoordinateSystem,Size,bool)"/>
        /// <param name="number"></param>
        /// <param name="coordinateSystem"></param>
        /// <param name="size"></param>
        /// <param name="tmsCompatible"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public RasterTile(Number number, CoordinateSystem coordinateSystem, Size size = null, bool tmsCompatible = false) : base(number, coordinateSystem, size, tmsCompatible) { }

        /// <inheritdoc cref="Tile(GeoCoordinate,GeoCoordinate,int,Size,bool)"/>
        /// <param name="minCoordinate"></param>
        /// <param name="maxCoordinate"></param>
        /// <param name="zoom"></param>
        /// <param name="size"></param>
        /// <param name="tmsCompatible"></param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public RasterTile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom, Size size = null, bool tmsCompatible = false) : base(minCoordinate, maxCoordinate, zoom, size, tmsCompatible) { }

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
        /// <returns>Ready <see cref="Image"/> for <see cref="RasterTile"/></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public Image CreateImage(IGeoTiff sourceGeoTiff, Image tileCache)
        {
            // TODO: check geotiff?

            #region Preconditions checks

            if (tileCache == null) throw new ArgumentNullException(nameof(tileCache));

            string interpolation = Interpolation switch
            {
#pragma warning disable CA1308 // Normalize strings to uppercase
                Interpolation.Nearest => nameof(Interpolation.Nearest).ToLowerInvariant(),
                Interpolation.Linear => nameof(Interpolation.Linear).ToLowerInvariant(),
                Interpolation.Cubic => nameof(Interpolation.Cubic).ToLowerInvariant(),
                Interpolation.Mitchell => nameof(Interpolation.Mitchell).ToLowerInvariant(),
                Interpolation.Lanczos2 => nameof(Interpolation.Lanczos2).ToLowerInvariant(),
                Interpolation.Lanczos3 => nameof(Interpolation.Lanczos3).ToLowerInvariant(),
                _ => throw new NotSupportedException(string.Format(Strings.Culture, Strings.NotSupported, Interpolation))
#pragma warning restore CA1308 // Normalize strings to uppercase
            };

            #endregion

            // Get postitions and sizes for current tile
            (Area readArea, Area writeArea) = Area.GetAreas(sourceGeoTiff, this);

            // Scaling calculations
            double xScale = (double)writeArea.Size.Width / readArea.Size.Width;
            double yScale = (double)writeArea.Size.Height / readArea.Size.Height;

            // Crop and resize tile
            Image tempTileImage = tileCache.Crop((int)readArea.OriginCoordinate.X, (int)readArea.OriginCoordinate.Y,
                                                 readArea.Size.Width, readArea.Size.Height)
                                           .Resize(xScale, interpolation, yScale);

            // Add alpha channel if needed
            Band.AddDefaultBands(ref tempTileImage, BandsCount);

            // Make transparent image and insert tile
            return Image.Black(Size.Width, Size.Height).NewFromImage(new int[BandsCount])
                        .Insert(tempTileImage, (int)writeArea.OriginCoordinate.X,
                                (int)writeArea.OriginCoordinate.Y);
        }

        /// <inheritdoc />
        public override void WriteToFile(IGeoTiff sourceGeoTiff, IWriteTilesArgs args)
        {
            WriteRasterTilesArgs rasterArgs = (WriteRasterTilesArgs)args;

            using Image tileImage = CreateImage(sourceGeoTiff, rasterArgs.TileCache);

            tileImage.WriteToFile(Path);
        }

        /// <inheritdoc />
        public override IEnumerable<byte> WriteToEnumerable(IGeoTiff sourceGeoTiff, IWriteTilesArgs args)
        {
            WriteRasterTilesArgs rasterArgs = args as WriteRasterTilesArgs;

            using Image tileImage = CreateImage(sourceGeoTiff, rasterArgs.TileCache);

            return tileImage.WriteToBuffer(GetExtensionString());
        }

        /// <inheritdoc />
        public override bool WriteToChannel<T>(IGeoTiff sourceGeoTiff, ChannelWriter<T> tileWriter,
                                               IWriteTilesArgs args)
        {
            Bytes = WriteToEnumerable(sourceGeoTiff, args);

            return Validate(false) && tileWriter.TryWrite(this as T);
        }

        #endregion
    }
}
