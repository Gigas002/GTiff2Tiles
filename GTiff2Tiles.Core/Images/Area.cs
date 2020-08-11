using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Tiles;

// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// Represents read/write <see cref="Area"/>s of image
    /// </summary>
    public class Area
    {
        #region Properties

        /// <summary>
        /// Origin <see cref="PixelCoordinate"/>
        /// </summary>
        public PixelCoordinate OriginCoordinate { get; }

        /// <summary>
        /// <see cref="Images.Size"/> of <see cref="Area"/>
        /// </summary>
        public Size Size { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <see cref="Area"/>
        /// </summary>
        /// <param name="originCoordinate"><see cref="OriginCoordinate"/></param>
        /// <param name="size"><see cref="Size"/></param>
        /// <exception cref="ArgumentNullException"/>
        public Area(PixelCoordinate originCoordinate, Size size)
        {
            #region Preconditions checks

            if (originCoordinate == null) throw new ArgumentNullException(nameof(originCoordinate));
            if (size == null) throw new ArgumentNullException(nameof(size));

            #endregion

            (OriginCoordinate, Size) = (originCoordinate, size);
        }

        #endregion

        #region Methods

        /// <inheritdoc cref="GetAreas(IGeoTiff, ITile)"/>
        /// <param name="imageMinCoordinate">Minimal <see cref="GeoCoordinate"/>
        /// of <see cref="IGeoTiff"/></param>
        /// <param name="imageMaxCoordinate">Maximal <see cref="GeoCoordinate"/>
        /// of <see cref="IGeoTiff"/></param>
        /// <param name="imageSize"><see cref="Images.Size"/> of <see cref="IGeoTiff"/></param>
        /// <param name="tileMinCoordinate">Minimal <see cref="GeoCoordinate"/>
        /// of <see cref="ITile"/></param>
        /// <param name="tileMaxCoordinate">Maximal <see cref="GeoCoordinate"/>
        /// of <see cref="ITile"/></param>
        /// <param name="tileSize"><see cref="Images.Size"/> of <see cref="ITile"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static (Area readArea, Area writeArea) GetAreas(GeoCoordinate imageMinCoordinate,
                                                               GeoCoordinate imageMaxCoordinate, Size imageSize,
                                                               GeoCoordinate tileMinCoordinate,
                                                               GeoCoordinate tileMaxCoordinate, Size tileSize)
        {
            #region Preconditions checks

            if (imageMinCoordinate == null) throw new ArgumentNullException(nameof(imageMinCoordinate));
            if (imageMaxCoordinate == null) throw new ArgumentNullException(nameof(imageMaxCoordinate));

            // This is to prevent unclear DivideByZeroException exception
            if (imageMinCoordinate == imageMaxCoordinate)
                throw new ArgumentException($"{nameof(imageMinCoordinate)} equals to {nameof(imageMaxCoordinate)}");

            if (imageSize == null) throw new ArgumentNullException(nameof(imageSize));
            if (tileMinCoordinate == null) throw new ArgumentNullException(nameof(tileMinCoordinate));
            if (tileMaxCoordinate == null) throw new ArgumentNullException(nameof(tileMaxCoordinate));

            // This is to prevent unclear DivideByZeroException exception
            if (tileMinCoordinate == tileMaxCoordinate)
                throw new ArgumentException($"{nameof(tileMinCoordinate)} equals to {nameof(tileMaxCoordinate)}");

            if (tileSize == null) throw new ArgumentNullException(nameof(tileSize));

            #endregion

            // Read from input geotiff in pixels
            double readPosMinX = imageSize.Width * (tileMinCoordinate.X - imageMinCoordinate.X) / (imageMaxCoordinate.X - imageMinCoordinate.X);
            double readPosMaxX = imageSize.Width * (tileMaxCoordinate.X - imageMinCoordinate.X) / (imageMaxCoordinate.X - imageMinCoordinate.X);
            double readPosMinY = imageSize.Height - imageSize.Height * (tileMaxCoordinate.Y - imageMinCoordinate.Y) / (imageMaxCoordinate.Y - imageMinCoordinate.Y);
            double readPosMaxY = imageSize.Height - imageSize.Height * (tileMinCoordinate.Y - imageMinCoordinate.Y) / (imageMaxCoordinate.Y - imageMinCoordinate.Y);

            // If outside of tiff -- set to 0.0/Max
            readPosMinX = readPosMinX < 0.0 ? 0.0 : readPosMinX > imageSize.Width ? imageSize.Width : readPosMinX;
            readPosMaxX = readPosMaxX < 0.0 ? 0.0 : readPosMaxX > imageSize.Width ? imageSize.Width : readPosMaxX;
            readPosMinY = readPosMinY < 0.0 ? 0.0 : readPosMinY > imageSize.Height ? imageSize.Height : readPosMinY;
            readPosMaxY = readPosMaxY < 0.0 ? 0.0 : readPosMaxY > imageSize.Height ? imageSize.Height : readPosMaxY;

            // Output tile's borders in pixels
            double tilePixMinX = readPosMinX.Equals(0.0) ? imageMinCoordinate.X :
                                 readPosMinX.Equals(imageSize.Width) ? imageMaxCoordinate.X : tileMinCoordinate.X;
            double tilePixMaxX = readPosMaxX.Equals(0.0) ? imageMinCoordinate.X :
                                 readPosMaxX.Equals(imageSize.Width) ? imageMaxCoordinate.X : tileMaxCoordinate.X;
            double tilePixMinY = readPosMaxY.Equals(0.0) ? imageMaxCoordinate.Y :
                                 readPosMaxY.Equals(imageSize.Height) ? imageMinCoordinate.Y : tileMinCoordinate.Y;
            double tilePixMaxY = readPosMinY.Equals(0.0) ? imageMaxCoordinate.Y :
                                 readPosMinY.Equals(imageSize.Height) ? imageMinCoordinate.Y : tileMaxCoordinate.Y;


            // Positions of dataset to write in tile
            double writePosMinX = tileSize.Width - tileSize.Width * (tileMaxCoordinate.X - tilePixMinX) / (tileMaxCoordinate.X - tileMinCoordinate.X);
            double writePosMaxX = tileSize.Width - tileSize.Width * (tileMaxCoordinate.X - tilePixMaxX) / (tileMaxCoordinate.X - tileMinCoordinate.X);
            double writePosMinY = tileSize.Height * (tileMaxCoordinate.Y - tilePixMaxY) / (tileMaxCoordinate.Y - tileMinCoordinate.Y);
            double writePosMaxY = tileSize.Height * (tileMaxCoordinate.Y - tilePixMinY) / (tileMaxCoordinate.Y - tileMinCoordinate.Y);

            // Sizes to read and write
            double readXSize = readPosMaxX - readPosMinX;
            double writeXSize = writePosMaxX - writePosMinX;
            double readYSize = Math.Abs(readPosMaxY - readPosMinY);
            double writeYSize = Math.Abs(writePosMaxY - writePosMinY);

            // Shifts
            double readXShift = readPosMinX - (int)readPosMinX;
            readXSize += readXShift;
            double readYShift = readPosMinY - (int)readPosMinY;
            readYSize += readYShift;
            double writeXShift = writePosMinX - (int)writePosMinX;
            writeXSize += writeXShift;
            double writeYShift = writePosMinY - (int)writePosMinY;
            writeYSize += writeYShift;

            // If output image sides are lesser then 1 - make image 1x1 pixels to prevent division by 0
            writeXSize = writeXSize > 1.0 ? writeXSize : 1.0;
            writeYSize = writeYSize > 1.0 ? writeYSize : 1.0;

            PixelCoordinate readOriginCoordinate = new PixelCoordinate(readPosMinX, readPosMinY);
            PixelCoordinate writeOriginCoordinate = new PixelCoordinate(writePosMinX, writePosMinY);
            Size readSize = new Size((int)readXSize, (int)readYSize);
            Size writeSize = new Size((int)writeXSize, (int)writeYSize);

            Area readArea = new Area(readOriginCoordinate, readSize);
            Area writeArea = new Area(writeOriginCoordinate, writeSize);

            return (readArea, writeArea);
        }

        /// <summary>
        /// Get <see cref="Area"/>s to read from input <see cref="IGeoTiff"/>
        /// and to write to target <see cref="ITile"/>
        /// </summary>
        /// <param name="image">Source <see cref="IGeoTiff"/></param>
        /// <param name="tile">Target <see cref="ITile"/></param>
        /// <returns><see cref="ValueTuple{T1, T2}"/> of <see cref="Area"/>s to read and write</returns>
        /// <exception cref="ArgumentNullException"/>
        public static (Area readArea, Area writeArea) GetAreas(IGeoTiff image, ITile tile)
        {
            #region Preconditions checks

            if (image == null) throw new ArgumentNullException(nameof(image));
            if (tile == null) throw new ArgumentNullException(nameof(tile));

            #endregion

            return GetAreas(image.MinCoordinate, image.MaxCoordinate, image.Size, tile.MinCoordinate,
                            tile.MaxCoordinate, tile.Size);
        }

        #endregion
    }
}
