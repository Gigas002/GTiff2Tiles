using System;
using System.Xml.Serialization;
using GTiff2Tiles.Core.Coordinates;

namespace GTiff2Tiles.Core.TileMapResource
{
    /// <summary>
    /// TileMap's bounding box
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "BoundingBox")]
    public record BoundingBox
    {
        #region Properties

        /// <summary>
        /// Minimal longitude
        /// </summary>
        [XmlAttribute(AttributeName = "minx")]
        public double MinX { get; init; }

        /// <summary>
        /// Minimal latitude
        /// </summary>
        [XmlAttribute(AttributeName = "miny")]
        public double MinY { get; init; }

        /// <summary>
        /// Maximal longitude
        /// </summary>
        [XmlAttribute(AttributeName = "maxx")]
        public double MaxX { get; init; }

        /// <summary>
        /// Maximal latitude
        /// </summary>
        [XmlAttribute(AttributeName = "maxy")]
        public double MaxY { get; init; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BoundingBox() { }

        /// <summary>
        /// Initialize new bounding box
        /// </summary>
        /// <param name="minx">Minimal longitude</param>
        /// <param name="miny">Minimal latitude</param>
        /// <param name="maxx">Maximal longitude</param>
        /// <param name="maxy">Maximal latitude</param>
        public BoundingBox(double minx, double miny, double maxx, double maxy) => (MinX, MinY, MaxX, MaxY) = (minx, miny, maxx, maxy);

        /// <summary>
        /// Initialize new bounding box
        /// </summary>
        /// <param name="minCoordinate">Minimal coordinate</param>
        /// <param name="maxCoordinate">Maximal coordinate</param>
        /// <exception cref="ArgumentNullException"/>
        public BoundingBox(ICoordinate minCoordinate, ICoordinate maxCoordinate)
        {
            #region Preconditions checks

            if (minCoordinate is null) throw new ArgumentNullException(nameof(minCoordinate));
            if (maxCoordinate is null) throw new ArgumentNullException(nameof(maxCoordinate));

            #endregion

            (MinX, MinY) = (minCoordinate.X, minCoordinate.Y);
            (MaxX, MaxY) = (maxCoordinate.X, maxCoordinate.Y);
        }

        #endregion
    }
}
