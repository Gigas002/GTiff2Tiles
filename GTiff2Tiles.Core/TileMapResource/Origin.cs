using System;
using System.Xml.Serialization;
using GTiff2Tiles.Core.Coordinates;

namespace GTiff2Tiles.Core.TileMapResource
{
    /// <summary>
    /// TileMap's origin
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "Origin")]
    public record Origin
    {
        #region Properties

        /// <summary>
        /// X
        /// </summary>
        [XmlAttribute(AttributeName = "x")]
        public double X { get; init; } = -180.0;

        /// <summary>
        /// Y
        /// </summary>
        [XmlAttribute(AttributeName = "y")]
        public double Y { get; init; } = -90.0;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public Origin() { }

        /// <summary>
        /// Initialize new origin
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public Origin(double x, double y) => (X, Y) = (x, y);

        /// <summary>
        /// Initialize new origin
        /// </summary>
        /// <param name="coordinate">Coordinate</param>
        /// <exception cref="ArgumentNullException"/>
        public Origin(ICoordinate coordinate)
        {
            #region Preconditions checks

            if (coordinate is null) throw new ArgumentNullException(nameof(coordinate));

            #endregion

            (X, Y) = (coordinate.X, coordinate.Y);
        }

        #endregion
    }
}
