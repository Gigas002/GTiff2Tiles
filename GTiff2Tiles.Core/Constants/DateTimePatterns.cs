using System;

// ReSharper disable UnusedMember.Global

namespace GTiff2Tiles.Core.Constants;

/// <summary>
/// String patterns for <see cref="DateTime"/>
/// </summary>
public static class DateTimePatterns
{
    /// <summary>
    /// Year, month, date, hour, minutes, seconds, ms
    /// </summary>
    public const string LongWithMs = "yyyyMMddHHmmssfff";

    /// <summary>
    /// Year, month and date
    /// </summary>
    public const string ShortToDate = "yyyyMMdd";

    /// <summary>
    /// Year and month
    /// </summary>
    public const string ShortToMonth = "yyyyMM";
}
