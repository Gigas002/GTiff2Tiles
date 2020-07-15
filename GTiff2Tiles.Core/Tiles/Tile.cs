#pragma warning disable CA1031 // Do not catch general exception types

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Exceptions.Tile;
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
        //TODO: Abstract?

        #region Properties

        /// <summary>
        /// Default tile size
        /// </summary>
        public static readonly Size DefaultSize = new Size(256, 256);

        /// <inheritdoc />
        public bool IsDisposed { get; set; }

        /// <inheritdoc />
        public GeoCoordinate MinCoordinate { get; set; }

        /// <inheritdoc />
        public GeoCoordinate MaxCoordinate { get; set; }

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
                    bool tmsCompatible = false, CoordinateType coordinateType = CoordinateType.Geodetic)
        {
            (Number, Bytes, Extension, TmsCompatible, Size) = (number, d, extension, tmsCompatible, size ?? DefaultSize);
            (MinCoordinate, MaxCoordinate) = Number.ToGeoCoordinates(coordinateType, Size.Width, tmsCompatible);
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
        public Tile(GeoCoordinate minCoordinate, GeoCoordinate maxCoordinate, int z, Size size = null, IEnumerable<byte> d = null, string extension = Constants.FileExtensions.Png,
                    bool tmsCompatible = false)
        {
            Size = size ?? DefaultSize;
            (Number minNumber, Number maxNumber) =
                GeoCoordinate.GetNumbers(minCoordinate, maxCoordinate, z, Size.Width, tmsCompatible);

            if (!minNumber.Equals(maxNumber))
                throw new TileException();

            (Number, Bytes, Extension, TmsCompatible) = (minNumber, d, extension, tmsCompatible);
            (MinCoordinate, MaxCoordinate) = (minCoordinate, maxCoordinate);
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
