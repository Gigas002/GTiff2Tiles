using System;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Core.Coordinates
{
    /// <summary>
    /// Coordinates in pixels
    /// </summary>
    public class PixelCoordinate : Coordinate
    {
        #region Constructors

        /// <summary>
        /// Create instance of class
        /// </summary>
        /// <param name="x">X coordinate value</param>
        /// <param name="y">Y coordinate value</param>
        public PixelCoordinate(double x, double y) => (X, Y) = (x, y);

        #endregion

        #region Methods

        /// <inheritdoc />
        public override Number ToNumber(int zoom, int tileSize, bool tmsCompatible)
        {
            int tileX = Convert.ToInt32(Math.Ceiling(X / tileSize) - 1.0);
            int tileY = Convert.ToInt32(Math.Ceiling(Y / tileSize) - 1.0);

            Number result = new Number(tileX, tileY, zoom);
            if (!tmsCompatible) result = result.Flip();

            return result;
        }

        /// <summary>
        /// Convert current coordinate to child of <see cref="GeoCoordinate"/>
        /// </summary>
        /// <typeparam name="T">Child of <see cref="GeoCoordinate"/></typeparam>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns>Converted to <see cref="GeoCoordinate"/> value
        /// or null if comething goes wrong</returns>
        public T ToGeoCoordinate<T>(int zoom, int tileSize) where T : GeoCoordinate
        {
            if (typeof(T) == typeof(GeodeticCoordinate)) return ToGeodeticCoordinate(zoom, tileSize) as T;
            if (typeof(T) == typeof(MercatorCoordinate)) return ToMercatorCoordinate(zoom, tileSize) as T;

            return null;
        }

        /// <summary>
        /// Convert current coordinate to <see cref="GeodeticCoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns>Converted <see cref="GeodeticCoordinate"/></returns>
        public GeodeticCoordinate ToGeodeticCoordinate(int zoom, int tileSize)
        {
            MercatorCoordinate mercatorCoordinate = ToMercatorCoordinate(zoom, tileSize);
            return mercatorCoordinate.ToGeodeticCoordinate();
        }

        /// <summary>
        /// Convert current coordinate to <see cref="MercatorCoordinate"/>
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns>Converted <see cref="MercatorCoordinate"/></returns>
        public MercatorCoordinate ToMercatorCoordinate(int zoom, int tileSize)
        {
            double resolution = MercatorCoordinate.Resolution(null, zoom, tileSize);
            double mx = X * resolution - Constants.Geodesic.OriginShift;
            double my = Y * resolution - Constants.Geodesic.OriginShift;

            return new MercatorCoordinate(mx, my);
        }

        /// <summary>
        /// Move the origin of pixel coordinates to top-left corner
        /// </summary>
        /// <param name="zoom">Zoom</param>
        /// <param name="tileSize">Tile's size</param>
        /// <returns>Converted <see cref="PixelCoordinate"/></returns>
        public PixelCoordinate ToRasterPixelCoordinate(int zoom, int tileSize)
        {
            // TODO: Test

            int mapSize = tileSize << zoom;

            return new PixelCoordinate(X, mapSize - Y);
        }

        #region Math operations

        /// <summary>
        /// Sum coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static PixelCoordinate operator +(PixelCoordinate coordinate1, PixelCoordinate coordinate2) =>
            new PixelCoordinate(coordinate1.X + coordinate2.X, coordinate1.Y + coordinate2.Y);

        /// <summary>
        /// Subtruct coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static PixelCoordinate operator -(PixelCoordinate coordinate1, PixelCoordinate coordinate2) =>
            new PixelCoordinate(coordinate1.X - coordinate2.X, coordinate1.Y - coordinate2.Y);

        /// <summary>
        /// Multiply coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static PixelCoordinate operator *(PixelCoordinate coordinate1, PixelCoordinate coordinate2) =>
            new PixelCoordinate(coordinate1.X * coordinate2.X, coordinate1.Y * coordinate2.Y);

        /// <summary>
        /// Divide coordinates
        /// </summary>
        /// <param name="coordinate1">Coordinate 1</param>
        /// <param name="coordinate2">Coordinate 2</param>
        /// <returns>New coordinate</returns>
        public static PixelCoordinate operator /(PixelCoordinate coordinate1, PixelCoordinate coordinate2) =>
            new PixelCoordinate(coordinate1.X / coordinate2.X, coordinate1.Y / coordinate2.Y);

        #endregion

        #endregion
    }
}
