﻿using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;
using GTiff2Tiles.Tests.Constants;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Tiles;

[TestFixture]
public sealed class NumberTests
{
    #region Consts

    private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

    private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

    private const CoordinateSystem CsOther = CoordinateSystem.Other;

    #endregion

    #region Constructors

    [Test]
    public void CreateNumberNormal() => Assert.DoesNotThrow(() =>
    {
        Number number = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
    });

    [Test]
    public void CreateNumberBadX() => Assert.Throws<ArgumentOutOfRangeException>(() =>
    {
        Number number = new(-Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
    });

    [Test]
    public void CreateNumberBadY() => Assert.Throws<ArgumentOutOfRangeException>(() =>
    {
        Number number = new(Locations.TokyoGeodeticNumberX, -Locations.TokyoGeodeticNumberNtmsY, 10);
    });

    [Test]
    public void CreateNumberBadZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
    {
        Number number = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, -1);
    });

    #endregion

    #region Properties

    [Test]
    public void GetProperties()
    {
        Number number = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number.X, Is.EqualTo(Locations.TokyoGeodeticNumberX));
        Assert.That(number.Y, Is.EqualTo(Locations.TokyoGeodeticNumberNtmsY));
        Assert.That(number.Z, Is.EqualTo(10));
    }

    #endregion

    #region Methods

    #region Flip

    [Test]
    public void FlipGeodeticNormal()
    {
        Number number = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
        Number result = null;

        Assert.DoesNotThrow(() => result = number.Flip());
        Assert.That(result, Is.EqualTo(Locations.TokyoGeodeticTmsNumber));
    }

    [Test]
    public void FlipMercatorNormal()
    {
        Number number = new(Locations.TokyoMercatorNumberX, Locations.TokyoMercatorNumberNtmsY, 10);
        Number result = null;

        Assert.DoesNotThrow(() => result = number.Flip());
        Assert.That(result, Is.EqualTo(Locations.TokyoMercatorTmsNumber));
    }

    [Test]
    public void FlipNullNumber() => Assert.Throws<ArgumentNullException>(() => Number.Flip(null));

    #endregion

    #region ToGeoCoordinates

    #region ToGeodeticCoordinates

    [Test]
    public void ToGeodeticCoordinatesNormal()
    {
        GeodeticCoordinate minCoordinate = null;
        GeodeticCoordinate maxCoordinate = null;

        Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = Locations.TokyoGeodeticNtmsNumber
                                                                            .ToGeodeticCoordinates(Tile.DefaultSize, false));

        GeodeticCoordinate min = Coordinate.Round(minCoordinate, 6);
        GeodeticCoordinate max = Coordinate.Round(maxCoordinate, 6);

        Assert.That(min == Locations.TokyoGeodeticMin && max == Locations.TokyoGeodeticMax, Is.True);
    }

    [Test]
    public void ToGeodeticCoordinatesNullSize() => Assert.Throws<ArgumentNullException>(() => Locations.TokyoGeodeticNtmsNumber
       .ToGeodeticCoordinates(null, false));

    [Test]
    public void ToGeodeticCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() =>
        Number.ToGeodeticCoordinates(null, Tile.DefaultSize, false));

    #endregion

    #region ToMercatorCoordinates

    [Test]
    public void ToMercatorCoordinatesNormal()
    {
        MercatorCoordinate minCoordinate = null;
        MercatorCoordinate maxCoordinate = null;

        Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = Locations.TokyoMercatorNtmsNumber
                                                                            .ToMercatorCoordinates(Tile.DefaultSize, false));

        MercatorCoordinate min = Coordinate.Round(minCoordinate, 2);
        MercatorCoordinate max = Coordinate.Round(maxCoordinate, 2);

        Assert.That(min == Locations.TokyoMercatorMin && max == Locations.TokyoMercatorMax, Is.True);
    }

    [Test]
    public void ToMercatorCoordinatesNullSize() => Assert.Throws<ArgumentNullException>(() => Locations.TokyoMercatorNtmsNumber
       .ToMercatorCoordinates(null, false));

    [Test]
    public void ToMercatorCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() =>
        Number.ToMercatorCoordinates(null, Tile.DefaultSize, false));

    #endregion

    #region ToGeoCoordinates

    [Test]
    public void ToGeoCoordinatesGeodetic()
    {
        GeoCoordinate minCoordinate = null;
        GeoCoordinate maxCoordinate = null;

        Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = Locations.TokyoGeodeticNtmsNumber
                                                                            .ToGeoCoordinates(Cs4326, Tile.DefaultSize, false));

        GeodeticCoordinate min = Coordinate.Round((GeodeticCoordinate)minCoordinate, 6);
        GeodeticCoordinate max = Coordinate.Round((GeodeticCoordinate)maxCoordinate, 6);

        Assert.That(min == Locations.TokyoGeodeticMin && max == Locations.TokyoGeodeticMax, Is.True);
    }

    [Test]
    public void ToGeoCoordinatesMercator()
    {
        GeoCoordinate minCoordinate = null;
        GeoCoordinate maxCoordinate = null;

        Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = Locations.TokyoMercatorNtmsNumber
                                                                            .ToGeoCoordinates(Cs3857, Tile.DefaultSize, false));

        MercatorCoordinate min = Coordinate.Round((MercatorCoordinate)minCoordinate, 2);
        MercatorCoordinate max = Coordinate.Round((MercatorCoordinate)maxCoordinate, 2);

        Assert.That(min == Locations.TokyoMercatorMin && max == Locations.TokyoMercatorMax, Is.True);
    }

    [Test]
    public void ToGeoCoordinatesOtherSystem() => Assert.Throws<NotSupportedException>(() => Locations.TokyoGeodeticTmsNumber
       .ToGeoCoordinates(CsOther, Tile.DefaultSize, true));

    [Test]
    public void ToGeoCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() => Number.ToGeoCoordinates(null, Cs4326, Tile.DefaultSize, false));

    #endregion

    #endregion

    #region GetLowerNumbers

    [Test]
    public void GetLowerNumbersNormal()
    {
        Number minExpected = new(Locations.TokyoGeodeticNumberX * 2,
                                 Locations.TokyoGeodeticNumberNtmsY * 2, 11);
        Number maxExpected = new(minExpected.X + 1, minExpected.Y + 1, 11);

        (Number minNumber, Number maxNumber) = Locations.TokyoGeodeticNtmsNumber.GetLowerNumbers(11);

        Assert.That(minNumber == minExpected && maxNumber == maxExpected, Is.True);
    }

    [Test]
    public void GetLowerNumbersBadZoom() => Assert.Throws<ArgumentOutOfRangeException>(() => Locations.TokyoGeodeticNtmsNumber.GetLowerNumbers(9));

    [Test]
    public void GetLowerNumbersNullNumber() => Assert.Throws<ArgumentNullException>(() =>
        Number.GetLowerNumbers(null, 10));

    [Test]
    public void Get1ZoomLowerNumbersNormal()
    {
        Number[] numbers = Locations.TokyoGeodeticNtmsNumber.GetLowerNumbers();
        Number num0 = new(3638, 618, 11);
        Assert.That(numbers[0], Is.EqualTo(num0));

        Number num1 = new(3639, 618, 11);
        Assert.That(numbers[1], Is.EqualTo(num1));

        Number num2 = new(3638, 619, 11);
        Assert.That(numbers[2], Is.EqualTo(num2));

        Number num3 = new(3639, 619, 11);
        Assert.That(numbers[3], Is.EqualTo(num3));

        Assert.Pass();
    }

    [Test]
    public void Get1ZoomLowerNumbersNullNumber() => Assert.Throws<ArgumentNullException>(() => Number.GetLowerNumbers(null));

    #endregion

    #region GetCount

    [Test]
    public void GetCountNormal()
    {
        int count = 0;

        Assert.DoesNotThrow(() => count = Number.GetCount(Locations.TokyoGeodeticMin, Locations.TokyoGeodeticMax,
                                                          0, 12, false, Tile.DefaultSize));
        Assert.That(count, Is.GreaterThan(0));
    }

    [Test]
    public void GetCountBadMinZ() => Assert.Throws<ArgumentOutOfRangeException>(() => Number.GetCount(Locations.TokyoGeodeticMin, Locations.TokyoGeodeticMax,
                                                                                    -1, 9, false, Tile.DefaultSize));

    [Test]
    public void GetCountBadMaxZ() => Assert.Throws<ArgumentOutOfRangeException>(() => Number.GetCount(Locations.TokyoGeodeticMin, Locations.TokyoGeodeticMax,
                                                                                    0, -1, false, Tile.DefaultSize));

    /// <summary>
    /// Throw overflow exception. I don't think it's a good idea to create billions of tiles and I think it CAN kill your hdd
    /// see https://github.com/Gigas002/GTiff2Tiles/issues/108
    /// </summary>
    [Test]
    public void GetCountTooMuch()
    {
        GeodeticCoordinate min = new(-180.0, -90.0);
        GeodeticCoordinate max = new(180.0, 90.0);
        Assert.Throws<OverflowException>(() => Number.GetCount(min, max, 0, 15, false, Tile.DefaultSize));
    }

    #endregion

    #endregion

    #region Bool comparings

    #region GetHashCode

    [Test]
    public void GetHashCodeNormal()
    {
        Number number = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.DoesNotThrow(() =>
        {
            int hashCode = number.GetHashCode();
        });
    }

    #endregion

    #region Equals

    [Test]
    public void EqualsByValueNormal()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
        Number number2 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.EqualTo(number2));
    }

    [Test]
    public void EqualsByRefNormal()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
        Number number2 = number1;

        Assert.That(number1, Is.EqualTo(number2));
    }

    [Test]
    public void EqualsOtherNull()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.Not.Null);
    }

    #region Equal operator

    [Test]
    public void EqualsOperatorNormalTrue()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
        Number number2 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.EqualTo(number2));
    }

    [Test]
    public void EqualsOperatorNormalFalse()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
        Number number2 = new(Locations.TokyoGeodeticNumberX + 1, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.Not.EqualTo(number2));
    }

    [Test]
    public void EqualsOperatorNumber1Null()
    {
        Number number2 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number2, Is.Not.Null);
    }

    [Test]
    public void EqualsOperatorNumber2Null()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.Not.Null);
    }

    #endregion

    #region Not equal operator

    [Test]
    public void NotEqualsOperatorNormalTrue()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
        Number number2 = new(Locations.TokyoGeodeticNumberX + 1, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.Not.EqualTo(number2));
    }

    [Test]
    public void NotEqualsOperatorNormalFalse()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);
        Number number2 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.EqualTo(number2));
    }

    [Test]
    public void NotEqualsOperatorNumber1Null()
    {
        Number number2 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number2, Is.Not.Null);
    }

    [Test]
    public void NotEqualsOperatorNumber2Null()
    {
        Number number1 = new(Locations.TokyoGeodeticNumberX, Locations.TokyoGeodeticNumberNtmsY, 10);

        Assert.That(number1, Is.Not.Null);
    }

    #endregion

    #endregion

    #endregion

    #region Math

    #region Add

    [Test]
    public void AddNormal()
    {
        Number number1 = new(0, 0, 0);
        Number number2 = new(1, 1, 0);
        Number result = new(1, 1, 0);

        Number add = number1.Add(number2);
        Assert.That(add, Is.EqualTo(result));
    }

    [Test]
    public void AddNullNumber1()
    {
        Number number2 = new(1, 1, 0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Number add = null + number2;
        });
    }

    [Test]
    public void AddNullNumber2()
    {
        Number number1 = new(0, 0, 0);

        Assert.Throws<ArgumentNullException>(() => number1.Add(null));
    }

    [Test]
    public void AddDifferentZ()
    {
        Number number1 = new(0, 0, 0);
        Number number2 = new(1, 1, 1);

        Assert.Throws<ArgumentException>(() => number1.Add(number2));
    }

    #endregion

    #region Subtract

    [Test]
    public void SubtractNormal()
    {
        Number number1 = new(1, 1, 0);
        Number number2 = new(0, 0, 0);
        Number result = new(1, 1, 0);

        Number sub = number1.Subtract(number2);
        Assert.That(sub, Is.EqualTo(result));
    }

    [Test]
    public void SubtractNullNumber1()
    {
        Number number2 = new(0, 0, 0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Number sub = null - number2;
        });
    }

    [Test]
    public void SubtractNullNumber2()
    {
        Number number1 = new(1, 1, 0);

        Assert.Throws<ArgumentNullException>(() => number1.Subtract(null));
    }

    [Test]
    public void SubtractDifferentZ()
    {
        Number number1 = new(1, 1, 1);
        Number number2 = new(0, 0, 0);

        Assert.Throws<ArgumentException>(() => number1.Subtract(number2));
    }

    #endregion

    #region Multiply

    [Test]
    public void MultiplyNormal()
    {
        Number number1 = new(0, 0, 0);
        Number number2 = new(1, 1, 0);
        Number result = new(0, 0, 0);

        Number mul = number1.Multiply(number2);
        Assert.That(mul, Is.EqualTo(result));
    }

    [Test]
    public void MultiplyNullNumber1()
    {
        Number number2 = new(1, 1, 0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Number mul = null * number2;
        });
    }

    [Test]
    public void MultiplyNullNumber2()
    {
        Number number1 = new(0, 0, 0);

        Assert.Throws<ArgumentNullException>(() => number1.Multiply(null));
    }

    [Test]
    public void MultiplyDifferentZ()
    {
        Number number1 = new(0, 0, 0);
        Number number2 = new(1, 1, 1);

        Assert.Throws<ArgumentException>(() => number1.Multiply(number2));
    }

    #endregion

    #region Divide

    [Test]
    public void DivideNormal()
    {
        Number number1 = new(0, 0, 0);
        Number number2 = new(1, 1, 0);
        Number result = new(0, 0, 0);

        Number div = number1.Divide(number2);
        Assert.That(div, Is.EqualTo(result));
    }

    [Test]
    public void DivideNullNumber1()
    {
        Number number2 = new(1, 1, 0);

        Assert.Throws<ArgumentNullException>(() =>
        {
            Number div = null / number2;
        });
    }

    [Test]
    public void DivideNullNumber2()
    {
        Number number1 = new(0, 0, 0);

        Assert.Throws<ArgumentNullException>(() => number1.Divide(null));
    }

    [Test]
    public void DivideDifferentZ()
    {
        Number number1 = new(0, 0, 0);
        Number number2 = new(1, 1, 1);

        Assert.Throws<ArgumentException>(() => number1.Divide(number2));
    }

    #endregion

    #endregion
}
