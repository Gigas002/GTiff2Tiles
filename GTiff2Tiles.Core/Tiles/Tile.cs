using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Geodesic;
using GTiff2Tiles.Core.Images;

namespace GTiff2Tiles.Core.Tiles
{
    /// <inheritdoc />
    public class Tile : ITile
    {
        //TODO: RasterTile child with bands property?

        #region Properties

        /// <inheritdoc />
        public bool IsDisposed { get; set; }

        /// <inheritdoc />
        public Coordinate MinCoordinate { get; set; }

        /// <inheritdoc />
        public Coordinate MaxCoordinate { get; set; }

        /// <inheritdoc />
        public Number Number { get; set; }

        /// <inheritdoc />
        public int Z { get; set; }

        /// <inheritdoc />
        public IEnumerable<byte> D { get; set; }

        /// <inheritdoc />
        public bool TmsCompatible { get; set; }

        /// <inheritdoc />
        public Size Size { get; set; }

        /// <inheritdoc />
        public string Extension { get; set; }

        /// <inheritdoc />
        public FileInfo FileInfo { get; set; }

        #endregion

        #region Constructors/Destructors

        public Tile(int x, int y, int z, Size size, IEnumerable<byte> d = null, string extension = Constants.Extensions.Png,
                    bool tmsCompatible = false)
        {
            Number number = new Number(x, y);
            (Number, Z, D, Extension, TmsCompatible, Size) = (number, z, d, extension, tmsCompatible, size);
            SetBounds();
        }

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
        private void Dispose(bool disposing)
        {
            if (IsDisposed) return;

            if (disposing)
            {
                //Occurs only if called by programmer. Dispose static things here.
            }

            D = null;

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

        #region Private

        /// <summary>
        /// Resolution for tiles.
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>Resoultion value.</returns>
        private static double Resolution(int zoom, int tileSize) => 180.0 / tileSize / Math.Pow(2.0, zoom);

        private double Resolution() => Resolution(Z, Size.Width);

        #endregion

        #region Public

        public static int FlipY(int y, int z) => Convert.ToInt32(Math.Pow(2.0, z) - y - 1);

        /// <inheritdoc />
        public void FlipNumber() => Number.Y = FlipY(Number.Y, Z);

        public static Number FlipNumber(Number number, int z) => new Number(number.X, FlipY(number.Y, z));

        /// <inheritdoc />
        public bool Validate(bool isCheckFileInfo)
        {
            if (D == null || D.Count() <= 355) return false;

            if (!isCheckFileInfo) return true;

            return FileInfo != null;
        }

        public static bool Validate(ITile tile, bool isCheckFileInfo)
        {
            if (tile?.D == null || tile.D.Count() <= 355) return false;

            if (!isCheckFileInfo) return true;

            return tile.FileInfo != null;
        }

        /// <inheritdoc />
        public int CalculatePosition() => CalculatePosition(Number, TmsCompatible);

        /// <summary>
        /// Calculates tile position on upper level (0-3)
        /// </summary>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        /// <returns>Position (0-3 int)</returns>
        public static int CalculatePosition(Number number, bool tmsCompatible)
        {
            /*
             * 0 1
             * 2 3
             */

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

        /// <inheritdoc />
        public (Number minNumber, Number maxNumber) GetNumbersFromCoords(bool tmsCompatible) =>
            GetNumbersFromCoords(MinCoordinate, MaxCoordinate, Z, tmsCompatible, Size);

        /// <summary>
        /// Calculates the tile numbers for zoom which covers given lon/lat coordinates.
        /// </summary>
        /// <param name="minX">Minimum longitude.</param>
        /// <param name="minY">Minimum latitude.</param>
        /// <param name="maxX">Maximum longitude.</param>
        /// <param name="maxY">Maximum latitude.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <param name="tmsCompatible">Do you want tms tiles on output?</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> of tiles numbers.</returns>
        public static (Number minNumber, Number maxNumber) GetNumbersFromCoords(
            Coordinate minCoordinate, Coordinate maxCoordinate, int zoom, bool tmsCompatible, Size size)
        {
            int[] tilesXs = new int[2];
            int[] tilesYs = new int[2];

            tilesXs[0] = Convert.ToInt32(Math.Ceiling((180.0 + minCoordinate.Longitude) / Resolution(zoom, size.Width) / size.Width) - 1.0);
            tilesXs[1] = Convert.ToInt32(Math.Ceiling((180.0 + maxCoordinate.Longitude) / Resolution(zoom, size.Width) / size.Width) - 1.0);
            tilesYs[0] = Convert.ToInt32(Math.Ceiling((90.0 + minCoordinate.Latitude) / Resolution(zoom, size.Height) / size.Height) - 1.0);
            tilesYs[1] = Convert.ToInt32(Math.Ceiling((90.0 + maxCoordinate.Latitude) / Resolution(zoom, size.Height) / size.Height) - 1.0);

            //Flip y's
            if (!tmsCompatible)
            {
                tilesYs[0] = FlipY(tilesYs[0], zoom);
                tilesYs[1] = FlipY(tilesYs[1], zoom);
            }

            //Ensure that no bad tiles returned
            Number minNumber = new Number(Math.Max(0, tilesXs.Min()), Math.Max(0, tilesYs.Min()));
            Number maxNumber = new Number(Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom + 1)) - 1, tilesXs.Max()),
                                          Math.Min(Convert.ToInt32(Math.Pow(2.0, zoom)) - 1, tilesYs.Max()));

            return (minNumber, maxNumber);
        }

        /// <inheritdoc />
        public (Number minNumber, Number maxNumber) GetLowerNumbers(int zoom) =>
            GetLowerNumbers(Number, zoom);

        /// <summary>
        /// Calculates lower levels for current 10 lvl tile
        /// </summary>
        /// <param name="tileX">Tile's x number on 10 lvl.</param>
        /// <param name="tileY">Tile's y number on 10 lvl.</param>
        /// <param name="zoom">Interested zoom.</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> тайлов указанного уровня.</returns>
        public static (Number minNumber, Number maxNumber) GetLowerNumbers(Number number, int zoom)
        {
            int resolution = Convert.ToInt32(Math.Pow(2.0, zoom - 10));

            int[] tilesXs = { number.X * resolution, (number.X + 1) * resolution - 1 };
            int[] tilesYs = { number.Y * resolution, (number.Y + 1) * resolution - 1 };

            Number minNumber = new Number(tilesXs.Min(), tilesYs.Min());
            Number maxNumber = new Number(tilesXs.Max(), tilesYs.Max());
            return (minNumber, maxNumber);
        }

        /// <summary>
        /// Calculates tile's coordinate borders for passed tiles numbers and zoom.
        /// </summary>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <param name="tmsCompatible">Do you want tms tiles on output?</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> of WGS84 coordinates.</returns>
        public static (Coordinate minCoordinate, Coordinate maxCoordinate) GetCoordinates(
            Number number, int zoom, bool tmsCompatible, Size size)
        {
            //Flip the y number for non-tms
            if (!tmsCompatible) number = FlipNumber(number, zoom);

            double minX = number.X * size.Width * Resolution(zoom, size.Width) - 180.0;
            double minY = number.Y * size.Height * Resolution(zoom, size.Height) - 90.0;
            double maxX = (number.X + 1) * size.Width * Resolution(zoom, size.Width) - 180.0;
            double maxY = (number.Y + 1) * size.Height * Resolution(zoom, size.Height) - 90.0;

            Coordinate minCoordinate = new Coordinate(minX, minY);
            Coordinate maxCoordinate = new Coordinate(maxX, maxY);

            return (minCoordinate, maxCoordinate);
        }

        /// <inheritdoc />
        public void SetBounds() => (MinCoordinate, MaxCoordinate) = GetCoordinates(Number, Z, TmsCompatible, Size);

        #region GetCount

        public int GetCount(int minZ, int maxZ) => GetCount(MinCoordinate, MaxCoordinate, minZ, maxZ, TmsCompatible, Size);

        public static int GetCount(Coordinate minCoordinate, Coordinate maxCoordinate,
                                   int minZ, int maxZ, bool tmsCompatible, Size tileSize)
        {
            int tilesCount = 0;

            for (int zoom = minZ; zoom <= maxZ; zoom++)
            {
                // Get tiles min/max numbers
                (Number minNumber, Number maxNumber) = GetNumbersFromCoords(minCoordinate, maxCoordinate, zoom, tmsCompatible, tileSize);

                int ysCount = Enumerable.Range(minNumber.Y, maxNumber.Y - minNumber.Y + 1).Count();
                int xsCount = Enumerable.Range(minNumber.X, maxNumber.X - minNumber.X + 1).Count();

                tilesCount += ysCount * xsCount;
            }

            return tilesCount;
        }

        #endregion

        #endregion

        #endregion
    }
}
