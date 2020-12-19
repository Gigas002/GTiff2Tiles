using System;
using System.Xml.Serialization;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Localization;

// TODO: tests

namespace GTiff2Tiles.Core.TileMapResource
{
    /// <summary>
    /// TileMap's TileFormat
    /// </summary>
    [Serializable]
    [XmlRoot(ElementName = "TileFormat")]
    public class TileFormat
    {
        #region Properties

        /// <summary>
        /// Default mime type is image
        /// </summary>
        private const string DefaultMimeType = "image";

        /// <summary>
        /// Tile's width
        /// </summary>
        [XmlAttribute(AttributeName = "width")]
        public int Width { get; init; }

        /// <summary>
        /// Tile's height
        /// </summary>
        [XmlAttribute(AttributeName = "height")]
        public int Height { get; init; }

        /// <summary>
        /// Tile's type
        /// </summary>
        [XmlAttribute(AttributeName = "mime-type")]
        public string MimeType { get; init; }

        /// <summary>
        /// Tile's extension
        /// </summary>
        [XmlAttribute(AttributeName = "extension")]
        public string Extension { get; init; }

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public TileFormat() { }

        /// <summary>
        /// Initialize new tile format
        /// </summary>
        /// <param name="width">Tile's width</param>
        /// <param name="height">Tile's height</param>
        /// <param name="mimeType">Tile's type</param>
        /// <param name="extension">Tile's extension</param>
        public TileFormat(int width, int height, string mimeType, string extension) => (Width, Height, MimeType, Extension) = (width, height, mimeType, extension);

        /// <summary>
        /// Initialize new tile format with "image/extension" mime type
        /// </summary>
        /// <param name="size"></param>
        /// <param name="tileExtension"></param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="NotSupportedException"/>
        public TileFormat(Size size, TileExtension tileExtension)
        {
            #region Preconditions checks

            if (size is null) throw new ArgumentNullException(nameof(size));

            #endregion

            (Width, Height) = (size.Width, size.Height);

            Extension = tileExtension switch
            {
#pragma warning disable CA1308 // Normalize strings to uppercase
                TileExtension.Png => nameof(TileExtension.Png).ToLowerInvariant(),
                TileExtension.Jpg => nameof(TileExtension.Jpg).ToLowerInvariant(),
                TileExtension.Webp => nameof(TileExtension.Webp).ToLowerInvariant(),
                _ => throw new NotSupportedException(string.Format(Strings.Culture, Strings.NotSupported, tileExtension))
#pragma warning restore CA1308 // Normalize strings to uppercase
            };

            MimeType = tileExtension switch
            {
                TileExtension.Jpg => $"{DefaultMimeType}/jpeg",
                _ => $"{DefaultMimeType}/{Extension}"
            };
        }
        
        #endregion
    }
}
