using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GTiff2Tiles.Core.Constants.Image;

namespace GTiff2Tiles.Core.Tiles
{
    /// <inheritdoc />
    public class Tile : ITile
    {
        #region Properties

        /// <inheritdoc />
        public bool IsDisposed { get; set; }

        /// <inheritdoc />
        public double MinLongtiude { get; set; }

        /// <inheritdoc />
        public double MinLatitude { get; set; }

        /// <inheritdoc />
        public double MaxLongitude { get; set; }

        /// <inheritdoc />
        public double MaxLatitude { get; set; }

        /// <inheritdoc />
        public int X { get; set; }

        /// <inheritdoc />
        public int Y { get; set; }

        /// <inheritdoc />
        public int Z { get; set; }

        /// <inheritdoc />
        public IEnumerable<byte> D { get; set; }

        /// <inheritdoc />
        public int S => D.Count();

        /// <inheritdoc />
        public bool TmsCompatible { get; set; }

        /// <inheritdoc />
        public int Size { get; set; }

        /// <inheritdoc />
        public FileInfo FileInfo { get; set; }

        #endregion

        #region Constructors/Destructors

        public Tile(int x, int y, int z, IEnumerable<byte> d = null, bool tmsCompatible = false, int size = Raster.TileSize)
        {
            (X, Y, Z, TmsCompatible, D, Size) = (x, y, z, tmsCompatible, d, size);
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

        private double Resolution() => Resolution(Z, Size);

        #endregion

        #region Public

        /// <inheritdoc />
        public void FlipY() => Y = FlipY(Y, Z);

        public static int FlipY(int y, int z) => Convert.ToInt32(Math.Pow(2.0, z) - y - 1);

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
        public int CalculatePosition() => CalculatePosition(X, Y, TmsCompatible);

        /// <summary>
        /// Calculates tile position on upper level (0-3)
        /// </summary>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        /// <returns>Position (0-3 int)</returns>
        public static int CalculatePosition(int tileX, int tileY, bool tmsCompatible)
        {
            int tilePosition;
            //Проверяем номера на четность.

            if (tmsCompatible)
            {
                if (tileX % 2 == 0) tilePosition = tileY % 2 == 0 ? 2 : 0;
                else tilePosition = tileY % 2 == 0 ? 3 : 1;
            }
            else
            {
                if (tileX % 2 == 0) tilePosition = tileY % 2 == 0 ? 0 : 2;
                else tilePosition = tileY % 2 == 0 ? 1 : 3;
            }

            return tilePosition;
        }

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
        public static (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetNumbersFromCoords(double minX,
             double minY, double maxX, double maxY, int zoom, bool tmsCompatible, int tileSize = Raster.TileSize)
        {
            int[] tilesXs = new int[2];
            int[] tilesYs = new int[2];

            tilesXs[0] = Convert.ToInt32(Math.Ceiling((180.0 + minX) / Resolution(zoom, tileSize) / tileSize) - 1.0);
            tilesXs[1] = Convert.ToInt32(Math.Ceiling((180.0 + maxX) / Resolution(zoom, tileSize) / tileSize) - 1.0);
            tilesYs[0] = Convert.ToInt32(Math.Ceiling((90.0 + minY) / Resolution(zoom, tileSize) / tileSize) - 1.0);
            tilesYs[1] = Convert.ToInt32(Math.Ceiling((90.0 + maxY) / Resolution(zoom, tileSize) / tileSize) - 1.0);

            //Flip y's
            // ReSharper disable once InvertIf
            if (!tmsCompatible)
            {
                tilesYs[0] = FlipY(tilesYs[0], zoom);
                tilesYs[1] = FlipY(tilesYs[1], zoom);
            }

            return (tilesXs.Min(), tilesYs.Min(), tilesXs.Max(), tilesYs.Max());
        }

        /// <inheritdoc />
        public (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetNumbersFromCoords(bool tmsCompatible) =>
            GetNumbersFromCoords(MinLongtiude, MinLatitude, MaxLongitude, MaxLatitude, Z, tmsCompatible, Size);

        /// <summary>
        /// Calculates lower levels for current 10 lvl tile
        /// </summary>
        /// <param name="tileX">Tile's x number on 10 lvl.</param>
        /// <param name="tileY">Tile's y number on 10 lvl.</param>
        /// <param name="zoom">Interested zoom.</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> тайлов указанного уровня.</returns>
        public static (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetLowerNumbers(int tileX,
                       int tileY, int zoom)
        {
            int resolution = Convert.ToInt32(Math.Pow(2.0, zoom - 10));

            int[] tilesXs = { tileX * resolution, (tileX + 1) * resolution - 1 };
            int[] tilesYs = { tileY * resolution, (tileY + 1) * resolution - 1 };

            return (tilesXs.Min(), tilesYs.Min(), tilesXs.Max(), tilesYs.Max());
        }

        /// <inheritdoc />
        public (int tileMinX, int tileMinY, int tileMaxX, int tileMaxY) GetLowerNumbers(int zoom) =>
            GetLowerNumbers(X, Y, zoom);

        /// <summary>
        /// Calculates tile's coordinate borders for passed tiles numbers and zoom.
        /// </summary>
        /// <param name="tileX">Tile's x number.</param>
        /// <param name="tileY">Tile's y number.</param>
        /// <param name="zoom">Tile's zoom.</param>
        /// <param name="tmsCompatible">Do you want tms tiles on output?</param>
        /// <returns><see cref="ValueTuple{T1, T2, T3, T4}"/> of WGS84 coordinates.</returns>
        public static (double minX, double minY, double maxX, double maxY) GetBounds(
            int tileX, int tileY, int zoom, bool tmsCompatible, int tileSize = Raster.TileSize)
        {
            //Flip the y number for non-tms
            if (!tmsCompatible) tileY = FlipY(tileY, zoom);

            double minX = tileX * tileSize * Resolution(zoom, tileSize) - 180.0;
            double minY = tileY * tileSize * Resolution(zoom, tileSize) - 90.0;
            double maxX = (tileX + 1) * tileSize * Resolution(zoom, tileSize) - 180.0;
            double maxY = (tileY + 1) * tileSize * Resolution(zoom, tileSize) - 90.0;

            return (minX, minY, maxX, maxY);
        }

        /// <inheritdoc />
        public void SetBounds()
        {
            (double minX, double minY, double maxX, double maxY) = GetBounds(X, Y, Z, TmsCompatible, Size);
            MinLongtiude = minX;
            MinLatitude = minY;
            MaxLongitude = maxX;
            MaxLatitude = maxY;
        }

        #endregion

        #endregion
    }
}
