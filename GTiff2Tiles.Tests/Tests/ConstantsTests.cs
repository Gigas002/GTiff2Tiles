#pragma warning disable CS0219 // The variable is assigned but it's value is never used

using GTiff2Tiles.Core.Constants;
using NUnit.Framework;

// ReSharper disable RedundantAssignment
// ReSharper disable NotAccessedVariable

namespace GTiff2Tiles.Tests.Tests
{
    [TestFixture]
    public sealed class ConstantsTests
    {
        [Test]
        public void GetProjProperties() => Assert.DoesNotThrow(() =>
        {
            string str = Proj.MercFull;
            str = Proj.LongLatFull;
            str = Proj.NoDefs;
            str = Proj.ProjMerc;
            str = Proj.ProjLongLat;
            str = Proj.DatumWgs84;
        });

        [Test]
        public void GetGeodesicProperties() => Assert.DoesNotThrow(() =>
        {
            double val = Geodesic.EquatorRadius;
            val = Geodesic.PolarRadius;
            val = Geodesic.OriginShift;
        });

        [Test]
        public void GetFileExtensionsProperties() => Assert.DoesNotThrow(() =>
        {
            string e = FileExtensions.Png;
            e = FileExtensions.Tif;
            e = FileExtensions.Jpg;
            e = FileExtensions.Webp;
        });

        [Test]
        public void GetDateTimePatternsProperties() => Assert.DoesNotThrow(() =>
        {
            string str = DateTimePatterns.LongWithMs;
            str = DateTimePatterns.ShortToDate;
            str = DateTimePatterns.ShortToMonth;
        });
    }
}

#pragma warning restore CS0219 // The variable is assigned but it's value is never used
