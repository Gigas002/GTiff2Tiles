using GTiff2Tiles.Core.Enums;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Enums
{
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
            Interpolation i = Interpolation.Nearest;
            i = Interpolation.Linear;
            i = Interpolation.Cubic;
            i = Interpolation.Mitchell;
            i = Interpolation.Lanczos2;
            i = Interpolation.Lanczos3;
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
}
