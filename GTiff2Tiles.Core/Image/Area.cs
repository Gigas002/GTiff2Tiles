using GTiff2Tiles.Core.Tiles;

namespace GTiff2Tiles.Core.Image
{
    public struct Area
    {
        //Pixel coordinates
        public int X { get; set; }
        public int Y { get; set; }

        public Size Size { get; }

        public Area(int x, int y, int width, int height) => (X, Y, Size) = (x, y, new Size(width, height));
    }
}
