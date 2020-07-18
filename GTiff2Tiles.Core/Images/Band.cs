using System.Collections.Generic;
using NetVips;

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// Represents image's band
    /// </summary>
    public class Band
    {
        /// <summary>
        /// Default <see cref="Band"/> value
        /// </summary>
        public const int DefaultValue = 255;

        /// <summary>
        /// Current value
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Create band with default value
        /// </summary>
        public Band() => Value = DefaultValue;

        /// <summary>
        /// Create band with desired value
        /// </summary>
        /// <param name="value"><see cref="int"/> in range from 0 to 255</param>
        public Band(int value) => Value = value;

        /// <summary>
        /// Add bands to <see cref="Image"/>
        /// </summary>
        /// <param name="image">Target <see cref="Image"/></param>
        /// <param name="bands"><see cref="Band"/>s to add</param>
        /// <returns><see cref="Image"/> with new <see cref="Band"/>s</returns>
        public static Image AddBands(Image image, IEnumerable<Band> bands)
        {
            foreach (Band band in bands) image.Bandjoin(band.Value);

            return image;
        }

        /// <summary>
        /// Add default <see cref="Band"/>s to <see cref="Image"/> until
        /// bands count is lesser than <see cref="Image"/>'s current bands count
        /// </summary>
        /// <param name="image">Target <see cref="Image"/></param>
        /// <param name="bandsCount">Count of desired <see cref="Band"/>s in <see cref="Image"/>,
        /// NOT the count of <see cref="Band"/>s to add</param>
        /// <returns><see cref="Image"/> with new <see cref="Band"/>s</returns>
        public static Image AddDefaultBands(Image image, int bandsCount)
        {
            for (; image.Bands < bandsCount;) image = image.Bandjoin(DefaultValue);

            return image;
        }
    }
}
