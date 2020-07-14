#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Exceptions.Tile;
using GTiff2Tiles.Core.Geodesic;
using GTiff2Tiles.Core.Images;

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace GTiff2Tiles.Core.Tiles
{
    /// <summary>
    /// Basic implementation of <see cref="ITile"/> interface
    /// </summary>
    public class Tile : ITile
    {
        //TODO: Move some methods to Coords/Numbers?

        #region Properties

        /// <summary>
        /// Default tile size
        /// </summary>
        public static readonly Size DefaultSize = new Size(256, 256);

        /// <inheritdoc />
        public bool IsDisposed { get; set; }

        /// <inheritdoc />
        public Coordinate MinCoordinate { get; set; }

        /// <inheritdoc />
        public Coordinate MaxCoordinate { get; set; }

        /// <inheritdoc />
        public Number Number { get; set; }

        /// <inheritdoc />
        public IEnumerable<byte> Bytes { get; set; }

        /// <inheritdoc />
        public Size Size { get; set; }

        /// <inheritdoc />
        public FileInfo FileInfo { get; set; }

        /// <inheritdoc />
        public string Extension { get; set; }

        /// <inheritdoc />
        public bool TmsCompatible { get; set; }

        #endregion

        #region Constructors/Destructors

        /// <summary>
        /// Creates new tile
        /// </summary>
        /// <param name="number">Tile number</param>
        /// <param name="size">Tile size</param>
        /// <param name="d">Tile bytes</param>
        /// <param name="extension">Tile extension</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        public Tile(Number number, Size size = null, IEnumerable<byte> d = null, string extension = Constants.FileExtensions.Png,
                    bool tmsCompatible = false)
        {
            (Number, Bytes, Extension, TmsCompatible, Size) = (number, d, extension, tmsCompatible, size ?? DefaultSize);
            (MinCoordinate, MaxCoordinate) = GetCoordinates();
        }

        /// <summary>
        /// Creates new tile from coordinate values
        /// </summary>
        /// <param name="minCoordinate">Minimum coordinate</param>
        /// <param name="maxCoordinate">Maximum coordinate</param>
        /// <param name="z">Zoom</param>
        /// <param name="size">Tile size</param>
        /// <param name="d">Tile bytes</param>
        /// <param name="extension">Tile extension</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        public Tile(Coordinate minCoordinate, Coordinate maxCoordinate, int z, Size size = null, IEnumerable<byte> d = null, string extension = Constants.FileExtensions.Png,
                    bool tmsCompatible = false)
        {
            Size = size ?? DefaultSize;
            (Number minNumber, Number maxNumber) =
                GetNumbersFromCoords(minCoordinate, maxCoordinate, z, tmsCompatible, Size);

            if (!minNumber.Equals(maxNumber))
                throw new TileException();

            (Number, Bytes, Extension, TmsCompatible) = (minNumber, d, extension, tmsCompatible);
            (MinCoordinate, MaxCoordinate) = GetCoordinates();
        }

        /// <summary>
        /// Calls <see cref="Dispose(bool)"/> on this tile.
        /// </summary>
        ~Tile() => Dispose(false);

        #endregion

        #region Dispose

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Actually disposes the data.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                // Occurs only if called by programmer. Dispose static things here.
            }

            Bytes = null;

            IsDisposed = true;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            try
            {
                Dispose();

                return default;
            }
            catch (Exception exception)
            {
                return new ValueTask(Task.FromException(exception));
            }
        }

        #endregion

        #region Methods

        #region Resoultion

        /// <summary>
        /// Resolution for tile
        /// </summary>
        /// <returns>Resoultion value</returns>
        internal double Resolution() => Resolution(Number.Z, Size.Width);

        /// <summary>
        /// Resolution for tile
        /// </summary>
        /// <param name="zoom">Zoom value</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns>Resoultion value</returns>
        internal static double Resolution(int zoom, int tileSize) => 180.0 / tileSize / Math.Pow(2.0, zoom);

        #endregion

        #region Validate

        /// <inheritdoc />
        public bool Validate(bool isCheckFileInfo) => Validate(this, isCheckFileInfo);

        /// <summary>
        /// Check if tile is not null, empty, not soo small and exists
        /// </summary>
        /// <param name="tile">Tile to check</param>
        /// <param name="isCheckFileInfo">Do you want to check if file exists?</param>
        /// <returns><see langword="true"/> if tile's valid, <see langword="false"/> otherwise</returns>
        public static bool Validate(ITile tile, bool isCheckFileInfo)
        {
            if (tile?.Bytes == null || tile.Bytes.Count() <= 355) return false;

            if (!isCheckFileInfo) return true;

            return tile.FileInfo != null;
        }

        #endregion

        #region CalculatePosition

        /// <inheritdoc />
        public int CalculatePosition() => CalculatePosition(Number, TmsCompatible);

        /// <summary>
        /// Calculates tile position in upper tile
        /// </summary>
        /// <param name="number">Number of tile</param>
        /// <param name="tmsCompatible">Is tile tms compatible?</param>
        /// <returns>Value in range from 0 to 3</returns>
        public static int CalculatePosition(Number number, bool tmsCompatible)
        {
            // 0 1
            // 2 3

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

        #region GetNumbersFromCoords

        /// <inheritdoc />
        public (Number minNumber, Number maxNumber) GetNumbersFromCoords() =>
            GetNumbersFromCoords(MinCoordinate, MaxCoordinate, Number.Z, TmsCompatible, Size);

        /// <summary>
        /// Calculates tile numbers for zoom which covers given WGS84 coordinates
        /// </summary>
        /// <param name="minCoordinate">Minimum coordinate</param>
        /// <param name="maxCoordinate">Maximum coordinate</param>
        /// <param name="zoom">Zoom</param>
        /// <param name="tmsCompatible">Is tile tms compatible?</param>
        /// <param name="size">Tile size</param>
        /// <returns><see cref="ValueTuple"/> of numbers</returns>
        public static (Number minNumber, Number maxNumber) GetNumbersFromCoords(
            Coordinate minCoordinate, Coordinate maxCoordinate, int zoom, bool tmsCompatible, Size size)
        {
            int[] tilesXs = new int[2];
            int[] tilesYs = new int[2];

            tilesXs[0] = Convert.ToInt32(Math.Ceiling((180.0 + minCoordinate.Longitude) / Resolution(zoom, size.Width) / size.Width) - 1.0);
            tilesXs[1] = Convert.ToInt32(Math.Ceiling((180.0 + maxCoordinate.Longitude) / Resolution(zoom, size.Width) / size.Width) - 1.0);
            tilesYs[0] = Convert.ToInt32(Math.Ceiling((90.0 + minCoordinate.Latitude) / Resolution(zoom, size.Height) / size.Height) - 1.0);
            tilesYs[1] = Convert.ToInt32(Math.Ceiling((90.0 + maxCoordinate.Latitude) / Resolution(zoom, size.Height) / size.Height) - 1.0);

            // Flip y's
            if (!tmsCompatible)
            {
                tilesYs[0] = Number.FlipY(tilesYs[0], zoom);
                tilesYs[1] = Number.FlipY(tilesYs[1], zoom);
            }

            // Ensure that no bad tile numbers returned
            Number minNumber = new Number(Math.Max(0, tilesXs.Min()), Math.Max(0, tilesYs.Min()), zoom);
            Number maxNumber = new Number(Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom + 1)) - 1, tilesXs.Max()),
                                          Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom)) - 1, tilesYs.Max()), zoom);

            return (minNumber, maxNumber);
        }

        #endregion

        #region GetLowerNumbers

        /// <inheritdoc />
        public (Number minNumber, Number maxNumber) GetLowerNumbers(int zoom) =>
            GetLowerNumbers(Number, zoom);

        /// <summary>
        /// Get lower numbers for specified number and zoom
        /// </summary>
        /// <param name="number">Base number</param>
        /// <param name="zoom">Zoom</param>
        /// <returns><see cref="ValueTuple"/> of lower numbers</returns>
        public static (Number minNumber, Number maxNumber) GetLowerNumbers(Number number, int zoom)
        {
            // TODO: Check on zoom <= 10
            int resolution = Convert.ToInt32(Math.Pow(2.0, zoom - 10));

            int[] tilesXs = { number.X * resolution, (number.X + 1) * resolution - 1 };
            int[] tilesYs = { number.Y * resolution, (number.Y + 1) * resolution - 1 };

            Number minNumber = new Number(tilesXs.Min(), tilesYs.Min(), zoom);
            Number maxNumber = new Number(tilesXs.Max(), tilesYs.Max(), zoom);

            return (minNumber, maxNumber);
        }

        #endregion

        #region GetCoordinates

        /// <inheritdoc />
        public (Coordinate minCoordinate, Coordinate maxCoordinate) GetCoordinates() =>
            GetCoordinates(Number, TmsCompatible, Size);

        /// <summary>
        /// Get coordinates by specified parameters
        /// </summary>
        /// <param name="number">Tile number</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="size">Tile size</param>
        /// <returns><see cref="ValueTuple"/> of WGS84 coordinates</returns>
        public static (Coordinate minCoordinate, Coordinate maxCoordinate) GetCoordinates(
            Number number, bool tmsCompatible, Size size)
        {
            // Flip the y number for non-tms
            if (!tmsCompatible) number = number.Flip();

            double minX = number.X * size.Width * Resolution(number.Z, size.Width) - 180.0;
            double minY = number.Y * size.Height * Resolution(number.Z, size.Height) - 90.0;
            double maxX = (number.X + 1) * size.Width * Resolution(number.Z, size.Width) - 180.0;
            double maxY = (number.Y + 1) * size.Height * Resolution(number.Z, size.Height) - 90.0;

            Coordinate minCoordinate = new Coordinate(minX, minY);
            Coordinate maxCoordinate = new Coordinate(maxX, maxY);

            return (minCoordinate, maxCoordinate);
        }

        #endregion

        #region GetCount

        /// <inheritdoc />
        public int GetCount(int minZ, int maxZ) => GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, TmsCompatible, Size);

        /// <summary>
        /// Get number of tiles in specified region
        /// </summary>
        /// <param name="minCoordinate">Minimum coordinate</param>
        /// <param name="maxCoordinate">Maximum coordinate</param>
        /// <param name="minZ">Minimum zoom</param>
        /// <param name="maxZ">Maximum zoom</param>
        /// <param name="tmsCompatible">Is tms compatible?</param>
        /// <param name="size">Tile size</param>
        /// <returns>Tiles count</returns>
        public static int GetCount(Coordinate minCoordinate, Coordinate maxCoordinate,
                                   int minZ, int maxZ, bool tmsCompatible, Size size)
        {
            int tilesCount = 0;

            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GetNumbersFromCoords(minCoordinate, maxCoordinate, zoom, tmsCompatible, size);

                int xsCount = Enumerable.Range(minNumber.X, maxNumber.X - minNumber.X + 1).Count();
                int ysCount = Enumerable.Range(minNumber.Y, maxNumber.Y - minNumber.Y + 1).Count();

                tilesCount += xsCount * ysCount;
            }

            return tilesCount;
        }

        #endregion

        // TODO: IEquatable
        //#region Equals

        ///// <inheritdoc />
        //public override bool Equals(object tile) => Equals(tile as ITile);

        ///// <inheritdoc />
        //public bool Equals(ITile other)
        //{
        //    if (other is null) return false;
        //    if (ReferenceEquals(this, other)) return true;

        //    return other.Number.Equals(Number) && other.TmsCompatible == TmsCompatible && other.Size.Equals(Size)
        //        && other.MaxCoordinate.Equals(MaxCoordinate) && other.MinCoordinate.Equals(MinCoordinate);
        //}

        ///// <inheritdoc />
        //public override int GetHashCode()
        //{
        //    // ReSharper disable NonReadonlyMemberInGetHashCode
        //    HashCode hashCode = new HashCode();
        //    hashCode.Add(MinCoordinate);
        //    hashCode.Add(MaxCoordinate);
        //    hashCode.Add(Number);
        //    hashCode.Add(Size);
        //    hashCode.Add(TmsCompatible);
        //    // ReSharper restore NonReadonlyMemberInGetHashCode

        //    return hashCode.ToHashCode();
        //}

        ///// <summary>
        ///// Check two tiles for equality
        ///// </summary>
        ///// <param name="tile1">Tile 1</param>
        ///// <param name="tile2">Tile 2</param>
        ///// <returns><see langword="true"/> if tiles are equal;
        ///// <see langword="false"/>otherwise</returns>
        //public static bool? operator ==(Tile tile1, Tile tile2) =>
        //    tile1?.Equals(tile2);

        ///// <summary>
        ///// Check two tiles for non-equality
        ///// </summary>
        ///// <param name="tile1">Tile 1</param>
        ///// <param name="tile2">Tile 2</param>
        ///// <returns><see langword="true"/> if tiles are not equal;
        ///// <see langword="false"/>otherwise</returns>
        //public static bool? operator !=(Tile tile1, Tile tile2) =>
        //    !(tile1 == tile2);

        //#endregion

        #endregion
    }
}

#pragma warning restore CA1031 // Do not catch general exception types
