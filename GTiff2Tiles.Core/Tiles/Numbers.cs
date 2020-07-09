namespace GTiff2Tiles.Core.Tiles
{
    public class Number
    {
        public int X { get; set; }

        public int Y { get; set; }

        public Number(int x, int y) => (X, Y) = (x, y);
    }
}
