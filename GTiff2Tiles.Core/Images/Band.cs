using System;
using System.Collections.Generic;
using System.Linq;
using NetVips;

// ReSharper disable MemberCanBePrivate.Global

namespace GTiff2Tiles.Core.Images
{
    /// <summary>
    /// Represents image's band
    /// </summary>
    public class Band
    {
        #region Properties

        /// <summary>
        /// Default <see cref="Band"/> value
        /// </summary>
        public const int DefaultValue = 255;

        /// <summary>
        /// Current value
        /// </summary>
        public int Value { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <see cref="Band"/>
        /// </summary>
        /// <param name="value"><see cref="int"/> in range
        /// from 0 to 255</param>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public Band(int value = DefaultValue)
        {
            #region Preconditions checks

            if (value < 0 || value > 255) throw new ArgumentOutOfRangeException(nameof(value));

            #endregion

            Value = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add <see cref="Band"/>s to <see cref="Image"/>
        /// </summary>
        /// <param name="image">Reference on <see cref="Image"/>
        /// to add <see cref="Band"/>s to</param>
        /// <param name="bands">Collection of <see cref="Band"/>s to add</param>
        /// <exception cref="ArgumentNullException"/>
        public static void AddBands(ref Image image, IEnumerable<Band> bands)
        {
            #region Preconditions checks

            if (image == null) throw new ArgumentNullException(nameof(image));
            if (bands == null) throw new ArgumentNullException(nameof(bands));

            #endregion

            image = bands.Aggregate(image, (current, band) => current.Bandjoin(band.Value));
        }

        /// <summary>
        /// Add default <see cref="Band"/>s to <see cref="Image"/> until
        /// bands count is lesser than <see cref="Image"/>'s current bands count
        /// </summary>
        /// <param name="image">Reference on <see cref="Image"/>
        /// to add <see cref="Band"/>s to</param>
        /// <param name="bandsCount">Count of desired <see cref="Band"/>s
        /// in <see cref="Image"/>;
        /// <remarks><para/>NOT the count of <see cref="Band"/>s to add</remarks></param>
        /// <exception cref="ArgumentNullException"/>
        public static void AddDefaultBands(ref Image image, int bandsCount)
        {
            #region Preconditions checks

            if (image == null) throw new ArgumentNullException(nameof(image));

            #endregion

            // Don't use HashSet
            List<Band> bandsToAdd = new List<Band>();
            for (int i = bandsCount; i > image.Bands; i--) bandsToAdd.Add(new Band());
            AddBands(ref image, bandsToAdd);
        }

        #endregion
    }
}
