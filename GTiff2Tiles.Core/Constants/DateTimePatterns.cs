using System;

namespace GTiff2Tiles.Core.Constants
{
    /// <summary>
    /// String patterns for <see cref="DateTime"/>
    /// </summary>
    public static class DateTimePatterns
    {
        /// <summary>
        /// yyyyMMddHHmmssfff
        /// </summary>
        public const string LongWithMs = "yyyyMMddHHmmssfff";

        /// <summary>
        /// yyyyMMdd
        /// </summary>
        public const string ShortToDate = "yyyyMMdd";

        /// <summary>
        /// yyyyMM
        /// </summary>
        public const string ShortToMonth = "yyyyMM";
    }
}
