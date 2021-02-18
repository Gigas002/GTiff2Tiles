using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Args;
using GTiff2Tiles.Core.Constants;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Helpers;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;

// ReSharper disable VirtualMemberNeverOverridden.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// Basic implementation of <see cref="ITile"/> interface
    /// </summary>
    public abstract class Tile : ITile
    {
        #region Properties

        /// <summary>
        /// Default value of <see cref="Tile"/>'s side size
        /// </summary>
        private const int DefaultSideSizeValue = 256;

        /// <summary>
        /// Default <see cref="Tile"/>'s <see cref="Images.Size"/>
        /// <remarks><para/>Uses <see cref="DefaultSideSizeValue"/>
        /// as values for width and height</remarks>
        /// </summary>
        public static readonly Size DefaultSize = new(DefaultSideSizeValue, DefaultSideSizeValue);

        /// <inheritdoc />
        public bool IsDisposed { get; private set; }

        /// <inheritdoc />
        public CoordinateSystem CoordinateSystem { get; } = CoordinateSystem.Epsg4326;

        /// <inheritdoc />
        public GeoCoordinate MinCoordinate { get; }

        /// <inheritdoc />
        public GeoCoordinate MaxCoordinate { get; }

        /// <inheritdoc />
        public Number Number { get; }

        /// <inheritdoc />
        public IEnumerable<byte> Bytes { get; set; }

        /// <inheritdoc />
        public Size Size { get; set; }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public TileExtension Extension { get; set; } = TileExtension.Png;

        /// <inheritdoc />
        public bool TmsCompatible { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// <remarks><para/>355 by default</remarks>
        /// </summary>
        public int MinimalBytesCount { get; set; } = 355;

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Creates new <see cref="Tile"/>
        /// </summary>
        /// <param name="number"><see cref="Number"/></param>
        /// <param name="coordinateSystem">Desired coordinate system</param>
        /// <param name="size"><see cref="Size"/>;
        /// <remarks>should be a square, e.g. 256x256;
        /// <para/>If set to <see langword="null"/>, uses <see cref="DefaultSize"/>
        /// as value</remarks></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <exception cref="ArgumentException"/>
        protected Tile(Number number, CoordinateSystem coordinateSystem, Size size = null, bool tmsCompatible = false)
        {
            (Number, CoordinateSystem, TmsCompatible, Size) = (number, coordinateSystem, tmsCompatible, size ?? DefaultSize);

            if (!Size.IsSquare) throw new ArgumentException(Strings.NotSqare, nameof(size));

            (MinCoordinate, MaxCoordinate) = Number.ToGeoCoordinates(CoordinateSystem, Size, TmsCompatible);
        }

        /// <summary>
        /// Creates new <see cref="Tile"/> from <see cref="GeoCoordinate"/> values
        /// </summary>
        /// <param name="minCoordinate">Minimal <see cref="GeoCoordinate"/></param>
        /// <param name="maxCoordinate">Maximal <see cref="GeoCoordinate"/></param>
        /// <param name="zoom">Zoom</param>
        /// <param name="size"><see cref="Size"/>;
        /// <remarks>should be a square, e.g. 256x256;
        /// <para/>If set to <see langword="null"/>, uses <see cref="DefaultSize"/>
        /// as value</remarks></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <exception cref="ArgumentException"/>
        protected Tile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int zoom, Size size = null, bool tmsCompatible = false)
        {
            (MinCoordinate, MaxCoordinate, TmsCompatible, Size) = (minCoordinate, maxCoordinate, tmsCompatible, size ?? DefaultSize);

            if (!Size.IsSquare) throw new ArgumentException(Strings.NotSqare, nameof(size));

            (Number minNumber, Number maxNumber) = GeoCoordinate.GetNumbers(MinCoordinate, MaxCoordinate, zoom, Size, TmsCompatible);

            if (!minNumber.Equals(maxNumber))
                throw new ArgumentException(Strings.CoordinatesDoesntFit);

            Number = minNumber;
        }


        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> on this <see cref="Tile"/>
        /// </summary>
        ~Tile() => Dispose(false);

        #endregion

        #region Methods

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc cref="Dispose()"/>
        /// <param name="disposing">Dispose static fields?</param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                // Occurs only if called by programmer. Dispose static things here
            }

            Bytes = null;

            IsDisposed = true;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
#pragma warning disable CA1031 // Do not catch general exception types

            try
            {
                Dispose();

                return default;
            }
            catch (Exception exception)
            {
                return ValueTask.FromException(exception);
            }

#pragma warning restore CA1031 // Do not catch general exception types
        }

        #endregion

        #region Validate

        /// <inheritdoc />
        public bool Validate(bool isCheckPath) => Validate(this, isCheckPath);

        /// <inheritdoc cref="Validate(bool)"/>
        /// <param name="tile"><see cref="Tile"/> to check</param>
        /// <param name="isCheckPath"></param>
        public static bool Validate(ITile tile, bool isCheckPath)
        {
            if (tile?.Bytes == null || tile.Bytes.Count() <= tile.MinimalBytesCount) return false;

            // ReSharper disable once InvertIf
            // ReSharper disable once RemoveRedundantBraces
            if (isCheckPath)
            {
                try
                {
                    CheckHelper.CheckFile(tile.Path);
                }
                catch (Exception exception)
                {
                    if (exception is ArgumentNullException || exception is FileNotFoundException) return false;

                    throw;
                }
            }

            return true;
        }

        #endregion

        #region CalculatePosition

        /// <inheritdoc />
        public int CalculatePosition() => CalculatePosition(Number, TmsCompatible);

        /// <inheritdoc cref="CalculatePosition()"/>
        /// <param name="number"><see cref="Tiles.Number"/> of <see cref="Tile"/></param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <exception cref="ArgumentNullException"/>
        public static int CalculatePosition(Number number, bool tmsCompatible)
        {
            // 0 1
            // 2 3

            #region Preconditions checks

            if (number == null) throw new ArgumentNullException(nameof(number));

            #endregion

            int tilePosition;

            if (tmsCompatible)
            {
                if (number.X % 2 == 0) tilePosition = number.Y % 2 == 0 ? 2 : 0;
                else tilePosition = number.Y % 2 == 0 ? 3 : 1;
            }
            else
            {
                if (number.X % 2 == 0) tilePosition = number.Y % 2 == 0 ? 0 : 2;
                else tilePosition = number.Y % 2 == 0 ? 1 : 3;
            }

            return tilePosition;
        }

        #endregion

        #region GetExtensionString

        /// <inheritdoc />
        public string GetExtensionString() => GetExtensionString(Extension);

        /// <param name="extension"><see cref="TileExtension"/> to convert</param>
        /// <inheritdoc cref="GetExtensionString()"/>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public static string GetExtensionString(TileExtension extension) => extension switch
        {
            TileExtension.Png => FileExtensions.Png,
            TileExtension.Jpg => FileExtensions.Jpg,
            TileExtension.Webp => FileExtensions.Webp,
            _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, null)
        };

        #endregion

        #region WriteToFile

        /// <exception cref="ArgumentNullException"/>
        /// <inheritdoc />
        public void WriteToFile(string path = null) => WriteToFile(this, path);

        /// <inheritdoc cref="WriteToFile(string)"/>
        /// <summary></summary>
        /// <param name="tile"><see cref="ITile"/> to write</param>
        /// <param name="path"></param>
        public static void WriteToFile(ITile tile, string path = null)
        {
            #region Preconditions checks

            if (tile == null) throw new ArgumentNullException(nameof(tile));
            if (tile.Bytes == null) throw new ArgumentNullException(nameof(tile));

            if (string.IsNullOrWhiteSpace(path)) path = tile.Path;

            #endregion

            File.WriteAllBytes(path, tile.Bytes.ToArray());
        }

        #endregion

        #region WriteTile

        /// <inheritdoc />
        public abstract void WriteToFile(IGeoTiff sourceGeoTiff, IWriteTilesArgs args);

        /// <inheritdoc />
        public abstract IEnumerable<byte> WriteToEnumerable(IGeoTiff sourceGeoTiff, IWriteTilesArgs args);

        /// <inheritdoc />
        public abstract bool WriteToChannel<T>(IGeoTiff sourceGeoTiff, ChannelWriter<T> tileWriter,
                                               IWriteTilesArgs args) where T : class, ITile;

        /// <inheritdoc />
        public abstract IEnumerable<byte> WriteOverviewTileBytes<T>(T[] allBaseTiles) where T : class, ITile;

        #endregion

        #endregion
    }
}
