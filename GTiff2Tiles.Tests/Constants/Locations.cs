using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Tests.Constants
{
    public static class Locations
    {
        #region Geodetic

        public const int TokyoGeodeticNumberX = 1819;

        public const int TokyoGeodeticNumberNtmsY = 309;

        public const int TokyoGeodeticNumberTmsY = 714;

        public static readonly Number TokyoGeodeticNtmsNumber =
            new(TokyoGeodeticNumberX, TokyoGeodeticNumberNtmsY, 10);

        public static readonly Number TokyoGeodeticTmsNumber =
            new(TokyoGeodeticNumberX, TokyoGeodeticNumberTmsY, 10);

        public const double TokyoGeodeticLongitude = 139.839478;

        public const double TokyoGeodeticLatitude = 35.652832;

        public static readonly GeodeticCoordinate TokyoGeodeticCoordinate =
            new(TokyoGeodeticLongitude, TokyoGeodeticLatitude);

        public static readonly GeodeticCoordinate TokyoGeodeticMin = new(139.746094, 35.507812);

        public static readonly GeodeticCoordinate TokyoGeodeticMax = new(139.921875, 35.683594);

        #endregion

        #region Mercator

        public const int TokyoMercatorNumberX = 909;

        public const int TokyoMercatorNumberNtmsY = 403;

        public const int TokyoMercatorNumberTmsY = 620;

        public static readonly Number TokyoMercatorNtmsNumber =
            new(TokyoMercatorNumberX, TokyoMercatorNumberNtmsY, 10);

        public static readonly Number TokyoMercatorTmsNumber =
            new(TokyoMercatorNumberX, TokyoMercatorNumberTmsY, 10);

        public const double TokyoMercatorLongitude = 15566859.48;

        public const double TokyoMercatorLatitude = 4252956.14;

        public static readonly MercatorCoordinate TokyoMercatorCoordinate =
            new(TokyoMercatorLongitude, TokyoMercatorLatitude);

        public static readonly MercatorCoordinate TokyoMercatorMin = new(15536896.12, 4226661.92);

        public static readonly MercatorCoordinate TokyoMercatorMax = new(15576031.88, 4265797.67);

        #endregion

        #region Pixel

        public const double TokyoGeodeticPixelLongitude = 465800.00067128887;

        public const double TokyoGeodeticPixelLatitude = 182995.19995448887;

        public static readonly PixelCoordinate TokyoGeodeticPixelCoordinate =
            new(TokyoGeodeticPixelLongitude, TokyoGeodeticPixelLatitude);

        public const double TokyoMercatorPixelLongitude = 232900.00031106747;

        public const double TokyoMercatorPixelLatitude = 158891.99925568007;

        public static readonly PixelCoordinate TokyoMercatorPixelCoordinate =
            new(TokyoMercatorPixelLongitude, TokyoMercatorPixelLatitude);

        #endregion
    }
}
