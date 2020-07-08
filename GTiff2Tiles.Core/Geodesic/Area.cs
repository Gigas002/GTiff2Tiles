using GTiff2Tiles.Core.Image;

namespace GTiff2Tiles.Core.Geodesic
{
    public struct Area
    {
        public Position Position { get; }
        public Size Size { get; }

        public Area(int x, int y, int width, int height) => (Position, Size) = (new Position(x, y), new Size(width, height));
    }
}
