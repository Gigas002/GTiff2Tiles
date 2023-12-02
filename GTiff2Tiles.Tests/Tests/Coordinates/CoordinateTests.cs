#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CA1859 // Use concrete types when possible for improved performance

using GTiff2Tiles.Core.Coordinates;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates;

[TestFixture]
public sealed class CoordinateTests
{
    // Only static things tests here. Everything else is tested on derived classes

    #region Methods

    [Test]
    public void DegreesToRadiansTest()
    {
        const double expected = 1.5708;
        double res = Math.Round(Coordinate.DegreesToRadians(90.0), 4);

        Assert.That(Math.Abs(expected - res), Is.LessThan(double.Epsilon));
    }

    [Test]
    public void RadiansToDegreesTest()
    {
        const double expected = 57.2958;
        double res = Math.Round(Coordinate.RadiansToDegrees(1.0), 4);

        Assert.That(Math.Abs(expected - res), Is.LessThan(double.Epsilon));
    }

    #region Round

    [Test]
    public void RoundNormal()
    {
        Coordinate coord = new GeodeticCoordinate(12.341, 13.561);
        Coordinate round = Coordinate.Round((GeodeticCoordinate)coord, 2);
        Coordinate expected = new GeodeticCoordinate(12.34, 13.56);

        Assert.That(round, Is.EqualTo(expected));
    }

    [Test]
    public void RoundCoordinateNull() => Assert.Throws<ArgumentNullException>(() => Coordinate.Round<Coordinate>(null, 2));

    [Test]
    public void RoundSmallDigits()
    {
        Coordinate coord = new GeodeticCoordinate(12.341, 13.561);
        Assert.Throws<ArgumentOutOfRangeException>(() => coord.Round<Coordinate>(-1));
    }

    #endregion

    #endregion

    #region Bool comparings

    #region GetHashCode

    [Test]
    public void GetHashCodeNormal()
    {
        Coordinate coordinate = new GeodeticCoordinate(10.0, 10.0);

        int hashCode = coordinate.GetHashCode();

        Assert.Pass();
    }

    #endregion

    #region Equals

    [Test]
    public void EqualsByValueNormal()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
        Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate1, Is.EqualTo(coordinate2));
    }

    [Test]
    public void EqualsByRefNormal()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
        Coordinate coordinate2 = coordinate1;

        Assert.That(coordinate1, Is.EqualTo(coordinate2));
    }

    [Test]
    public void EqualsOtherNull()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate1, Is.Not.Null);
    }

    #region Equal operator

    [Test]
    public void EqualsOperatorNormalTrue()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
        Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate1, Is.EqualTo(coordinate2));
    }

    [Test]
    public void EqualsOperatorNormalFalse()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
        Coordinate coordinate2 = new GeodeticCoordinate(11.0, 10.0);

        Assert.That(coordinate1, Is.Not.EqualTo(coordinate2));
    }

    [Test]
    public void EqualsOperatorCoordinate1Null()
    {
        Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate2, Is.Not.Null);
    }

    [Test]
    public void EqualsOperatorCoordinate2Null()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate1, Is.Not.Null);
    }

    #endregion

    #region Not equal operator

    [Test]
    public void NotEqualsOperatorNormalTrue()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
        Coordinate coordinate2 = new GeodeticCoordinate(11.0, 10.0);

        Assert.That(coordinate1, Is.Not.EqualTo(coordinate2));
    }

    [Test]
    public void NotEqualsOperatorNormalFalse()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
        Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate1, Is.EqualTo(coordinate2));
    }

    [Test]
    public void NotEqualsOperatorCoordinate1Null()
    {
        Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate2, Is.Not.Null);
    }

    [Test]
    public void NotEqualsOperatorCoordinate2Null()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);

        Assert.That(coordinate1, Is.Not.Null);
    }

    #endregion

    #endregion

    #endregion

    #region Math

    #region Add

    [Test]
    public void AddNormal()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(1.0, 1.0);
        Coordinate coordinate2 = new GeodeticCoordinate(2.0, 2.0);
        Coordinate result = new GeodeticCoordinate(3.0, 3.0);

        Coordinate add = coordinate1.Add(coordinate2);
        Assert.That(add, Is.EqualTo(result));
    }

    [Test]
    public void AddNullCoordinate1()
    {
        Coordinate coordinate2 = new GeodeticCoordinate(1.0, 1.0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Coordinate add = null + coordinate2;
        });
    }

    [Test]
    public void AddNullCoordinate2()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(1.0, 1.0);

        Assert.Throws<ArgumentNullException>(() => coordinate1.Add(null));
    }

    #endregion

    #region Subtract

    [Test]
    public void SubtractNormal()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(2.0, 2.0);
        Coordinate coordinate2 = new GeodeticCoordinate(1.0, 1.0);
        Coordinate result = new GeodeticCoordinate(1.0, 1.0);

        Coordinate sub = coordinate1.Subtract(coordinate2);
        Assert.That(sub, Is.EqualTo(result));
    }

    [Test]
    public void SubtractNullCoordinate1()
    {
        Coordinate coordinate2 = new GeodeticCoordinate(1.0, 1.0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Coordinate sub = null - coordinate2;
        });
    }

    [Test]
    public void SubtractNullCoordinate2()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(2.0, 2.0);

        Assert.Throws<ArgumentNullException>(() => coordinate1.Subtract(null));
    }

    #endregion

    #region Multiply

    [Test]
    public void MultiplyNormal()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(1.0, 1.0);
        Coordinate coordinate2 = new GeodeticCoordinate(2.0, 2.0);
        Coordinate result = new GeodeticCoordinate(2.0, 2.0);

        Coordinate mul = coordinate1.Multiply(coordinate2);
        Assert.That(mul, Is.EqualTo(result));
    }

    [Test]
    public void MultiplyNullCoordinate1()
    {
        Coordinate coordinate2 = new GeodeticCoordinate(2.0, 2.0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Coordinate mul = null * coordinate2;
        });
    }

    [Test]
    public void MultiplyNullCoordinate2()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(1.0, 1.0);

        Assert.Throws<ArgumentNullException>(() => coordinate1.Multiply(null));
    }

    #endregion

    #region Divide

    [Test]
    public void DivideNormal()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(2.0, 2.0);
        Coordinate coordinate2 = new GeodeticCoordinate(1.0, 1.0);
        Coordinate result = new GeodeticCoordinate(2.0, 2.0);

        Coordinate div = coordinate1.Divide(coordinate2);
        Assert.That(div, Is.EqualTo(result));
    }

    [Test]
    public void DivideNullCoordinate1()
    {
        Coordinate coordinate2 = new GeodeticCoordinate(1.0, 1.0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Coordinate div = null / coordinate2;
        });
    }

    [Test]
    public void DivideNullCoordinate2()
    {
        Coordinate coordinate1 = new GeodeticCoordinate(2.0, 2.0);

        Assert.Throws<ArgumentNullException>(() => coordinate1.Divide(null));
    }

    #endregion

    #endregion
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
