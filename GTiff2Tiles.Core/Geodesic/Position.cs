namespace GTiff2Tiles.Core.Geodesic
{
    public struct Position
    {
        public readonly int X;
        public readonly int Y;

        public Position(int x, int y) => (X, Y) = (x, y);
    }
}
