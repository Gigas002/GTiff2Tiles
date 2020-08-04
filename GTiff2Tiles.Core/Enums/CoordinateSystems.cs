using System;

// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Enums
{
    /// <summary>
    /// Supported EPSG coordinate systems
    /// </summary>
    [Flags]
    public enum CoordinateSystems
    {
        /// <summary>
        /// EPSG:4326
        /// </summary>
        Epsg4326 = 0,

        /// <summary>
        /// EPSG:3857
        /// </summary>
        Epsg3857 = 1,

        /// <summary>
        /// Replaced by <see cref="Epsg3857"/>
        /// </summary>
        [Obsolete("Replaced by EPSG:3857")]
        Epsg3587 = Epsg3857,

        /// <summary>
        /// Replaced by <see cref="Epsg3857"/>
        /// </summary>
        [Obsolete("Replaced by EPSG:3857")]
        Epsg3785 = Epsg3857,

        /// <summary>
        /// Replaced by <see cref="Epsg3857"/>
        /// </summary>
        [Obsolete("Replaced by EPSG:3857")]
        Epsg41001 = Epsg3857,

        /// <summary>
        /// Replaced by <see cref="Epsg3857"/>
        /// </summary>
        [Obsolete("Replaced by EPSG:3857")]
        Epsg54004 = Epsg3857,

        /// <summary>
        /// Replaced by <see cref="Epsg3857"/>
        /// </summary>
        [Obsolete("Replaced by EPSG:3857")]
        Epsg102113 = Epsg3857,

        /// <summary>
        /// Replaced by <see cref="Epsg3857"/>
        /// </summary>
        [Obsolete("Replaced by EPSG:3857")]
        Epsg102100 = Epsg3857,

        /// <summary>
        /// Replaced by <see cref="Epsg3857"/>
        /// </summary>
        [Obsolete("Replaced by EPSG:3857")]
        Epsg900913 = Epsg3857
    }
}
