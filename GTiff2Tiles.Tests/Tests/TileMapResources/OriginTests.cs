using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.TileMapResource;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.TileMapResources;

[TestFixture]
public sealed class Origintests
{
    #region Consts

    private readonly ICoordinate _originCoordinate = new GeodeticCoordinate(-180.0, -90.0);

    #endregion

    #region Constructors

    [Test]
    public void DefaultConstructor() => Assert.DoesNotThrow(() => _ = new Origin());

    [Test]
    public void FromDouble()
    {
        Origin origin = null;

        Assert.DoesNotThrow(() => origin = new(_originCoordinate.X, _originCoordinate.Y));

        Assert.That(Math.Abs(origin.X - _originCoordinate.X) < double.Epsilon &&
                    Math.Abs(origin.Y - _originCoordinate.Y) < double.Epsilon, Is.True);
    }

    [Test]
    public void FromCoordinateNormal()
    {
        Origin origin = null;

        Assert.DoesNotThrow(() => origin = new(_originCoordinate));

        Assert.That(Math.Abs(origin.X - _originCoordinate.X) < double.Epsilon &&
                    Math.Abs(origin.Y - _originCoordinate.Y) < double.Epsilon, Is.True);
    }

    [Test]
    public void FromCoordinateBad() => Assert.Throws<ArgumentNullException>(() => _ = new Origin(null));

    #endregion
}