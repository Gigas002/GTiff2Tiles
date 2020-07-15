using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Core.Images
{
    public struct Area
    {
        public PixelCoordinate OriginCoordinate { get; }

        public Size Size { get; }

        public Area(PixelCoordinate originCoordinate, int width, int height) => (OriginCoordinate, Size) = (originCoordinate, new Size(width, height));

        public static (Area readArea, Area writeArea) GetAreas(GeoCoordinate imageMinCoordinate,
                                                               GeoCoordinate imageMaxCoordinate, Size imageSize,
                                                               GeoCoordinate tileMinCoordinate,
                                                               GeoCoordinate tileMaxCoordinate, Size tileSize)
        {
            //Read from input geotiff in pixels.
            double readPosMinX = imageSize.Width * (tileMinCoordinate.X - imageMinCoordinate.X) / (imageMaxCoordinate.X - imageMinCoordinate.X);
            double readPosMaxX = imageSize.Width * (tileMaxCoordinate.X - imageMinCoordinate.X) / (imageMaxCoordinate.X - imageMinCoordinate.X);
            double readPosMinY = imageSize.Height - imageSize.Height * (tileMaxCoordinate.Y - imageMinCoordinate.Y) / (imageMaxCoordinate.Y - imageMinCoordinate.Y);
            double readPosMaxY = imageSize.Height - imageSize.Height * (tileMinCoordinate.Y - imageMinCoordinate.Y) / (imageMaxCoordinate.Y - imageMinCoordinate.Y);

            //If outside of tiff.
            readPosMinX = readPosMinX < 0.0 ? 0.0 : readPosMinX > imageSize.Width ? imageSize.Width : readPosMinX;
            readPosMaxX = readPosMaxX < 0.0 ? 0.0 : readPosMaxX > imageSize.Width ? imageSize.Width : readPosMaxX;
            readPosMinY = readPosMinY < 0.0 ? 0.0 : readPosMinY > imageSize.Height ? imageSize.Height : readPosMinY;
            readPosMaxY = readPosMaxY < 0.0 ? 0.0 : readPosMaxY > imageSize.Height ? imageSize.Height : readPosMaxY;

            //Output tile's borders in pixels.
            double tilePixMinX = readPosMinX.Equals(0.0) ? imageMinCoordinate.X :
                                 readPosMinX.Equals(imageSize.Width) ? imageMaxCoordinate.X : tileMinCoordinate.X;
            double tilePixMaxX = readPosMaxX.Equals(0.0) ? imageMinCoordinate.X :
                                 readPosMaxX.Equals(imageSize.Width) ? imageMaxCoordinate.X : tileMaxCoordinate.X;
            double tilePixMinY = readPosMaxY.Equals(0.0) ? imageMaxCoordinate.Y :
                                 readPosMaxY.Equals(imageSize.Height) ? imageMinCoordinate.Y : tileMinCoordinate.Y;
            double tilePixMaxY = readPosMinY.Equals(0.0) ? imageMaxCoordinate.Y :
                                 readPosMinY.Equals(imageSize.Height) ? imageMinCoordinate.Y : tileMaxCoordinate.Y;


            //Positions of dataset to write in tile.
            double writePosMinX = tileSize.Width - tileSize.Width * (tileMaxCoordinate.X - tilePixMinX) / (tileMaxCoordinate.X - tileMinCoordinate.X);
            double writePosMaxX = tileSize.Width - tileSize.Width * (tileMaxCoordinate.X - tilePixMaxX) / (tileMaxCoordinate.X - tileMinCoordinate.X);
            double writePosMinY = tileSize.Height * (tileMaxCoordinate.Y - tilePixMaxY) / (tileMaxCoordinate.Y - tileMinCoordinate.Y);
            double writePosMaxY = tileSize.Height * (tileMaxCoordinate.Y - tilePixMinY) / (tileMaxCoordinate.Y - tileMinCoordinate.Y);

            //Sizes to read and write.
            double readXSize = readPosMaxX - readPosMinX;
            double writeXSize = writePosMaxX - writePosMinX;
            double readYSize = Math.Abs(readPosMaxY - readPosMinY);
            double writeYSize = Math.Abs(writePosMaxY - writePosMinY);

            //Shifts.
            double readXShift = readPosMinX - (int)readPosMinX;
            readXSize += readXShift;
            double readYShift = readPosMinY - (int)readPosMinY;
            readYSize += readYShift;
            double writeXShift = writePosMinX - (int)writePosMinX;
            writeXSize += writeXShift;
            double writeYShift = writePosMinY - (int)writePosMinY;
            writeYSize += writeYShift;

            //If output image sides are lesser then 1 - make image 1x1 pixels to prevent division by 0.
            writeXSize = writeXSize > 1.0 ? writeXSize : 1.0;
            writeYSize = writeYSize > 1.0 ? writeYSize : 1.0;

            var readOriginCoordinate = new PixelCoordinate(readPosMinX, readPosMinY);
            var writeOriginCoordinate = new PixelCoordinate(writePosMinX, writePosMinY);
            Area readArea = new Area(readOriginCoordinate, (int)readXSize, (int)readYSize);
            Area writeArea = new Area(writeOriginCoordinate, (int)writeXSize, (int)writeYSize);

            return (readArea, writeArea);
        }

        public static (Area readArea, Area writeArea) GetAreas(IImage image, ITile tile) => GetAreas(image.MinCoordinate, image.MaxCoordinate, image.Size,
                                                                                                     tile.MinCoordinate, tile.MaxCoordinate, tile.Size);
    }
}
