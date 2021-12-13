#pragma warning disable CS0219 // The variable is assigned but it's value is never used
#pragma warning disable CS0618 // Something is obsolete

using GTiff2Tiles.Core.Enums;
using NUnit.Framework;

// ReSharper disable RedundantAssignment
// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests;

[TestFixture]
public sealed class EnumsTests
{
    [Test]
    public void GetTileExtensions() => Assert.DoesNotThrow(() =>
    {
        TileExtension e = TileExtension.Png;
        e = TileExtension.Jpg;
        e = TileExtension.Webp;
    });

    [Test]
    public void GetInterpolations() => Assert.DoesNotThrow(() =>
    {
        NetVips.Enums.Kernel i = NetVips.Enums.Kernel.Nearest;
        i = NetVips.Enums.Kernel.Linear;
        i = NetVips.Enums.Kernel.Cubic;
        i = NetVips.Enums.Kernel.Mitchell;
        i = NetVips.Enums.Kernel.Lanczos2;
        i = NetVips.Enums.Kernel.Lanczos3;
    });

    [Test]
    public void GetCoordinateSystems() => Assert.DoesNotThrow(() =>
    {
        CoordinateSystem cs = CoordinateSystem.Epsg4326;
        cs = CoordinateSystem.Epsg3857;
        cs = CoordinateSystem.Epsg3587;
        cs = CoordinateSystem.Epsg3785;
        cs = CoordinateSystem.Epsg41001;
        cs = CoordinateSystem.Epsg54004;
        cs = CoordinateSystem.Epsg102113;
        cs = CoordinateSystem.Epsg102100;
        cs = CoordinateSystem.Epsg900913;
        cs = CoordinateSystem.Other;
    });
}

#pragma warning restore CS0219 // The variable is assigned but it's value is never used
#pragma warning restore CS0618 // Something is obsolete
