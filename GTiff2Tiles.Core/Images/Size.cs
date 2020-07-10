namespace GTiff2Tiles.Core.Images
{
    public struct Size
    {
        public readonly int Width;
        public readonly int Height;

        public Size(int width, int height) => (Width, Height) = (width, height);

        public long GetResoultion() => Width * Height;
    }
}
