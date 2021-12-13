using System.Xml.Serialization;

namespace GTiff2Tiles.Core.TileMapResource;

/// <summary>
/// TileMap's TileSets's TileSet
/// </summary>
[Serializable]
[XmlRoot(ElementName = "TileSet")]
public record TileSet
{
    #region Propertes

    /// <summary>
    /// Link to a zoom
    /// </summary>
    [XmlAttribute(AttributeName = "href")]
    public string Href { get; init; }

    /// <summary>
    /// Pixel resolution
    /// </summary>
    [XmlAttribute(AttributeName = "units-per-pixel")]
    public double UnitsPerPixel { get; init; }

    /// <summary>
    /// Zoom
    /// </summary>
    [XmlAttribute(AttributeName = "order")]
    public int Order { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor
    /// </summary>
    public TileSet() { }

    /// <summary>
    /// Initialize a new tile set
    /// </summary>
    /// <param name="href">Link to a zoom</param>
    /// <param name="unitsPerPixel">Pixel resolution</param>
    /// <param name="order">Zoom</param>
    public TileSet(string href, double unitsPerPixel, int order) => (Href, UnitsPerPixel, Order) = (href, unitsPerPixel, order);

    #endregion
}