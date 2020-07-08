namespace GTiff2Tiles.Core.Geodesic
{
    public struct Coordinate
    {
        public readonly double Longitude;
        public readonly double Latitude;

        public Coordinate(double longitude, double latitude) => (Longitude, Latitude) = (longitude, latitude);
    }
}
