#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0219 // The variable is assigned but it's value is never used

using System;
using GTiff2Tiles.Core.Coordinates;
using GTiff2Tiles.Core.Enums;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Tiles
{
    public sealed class NumberTests
    {
        #region Setup

        [SetUp]
        public void Setup() { }

        #endregion

        #region Constructors

        [Test]
        public void CreateNumberNormal()
        {
            Number number = new Number(0, 0, 0);
            Assert.Pass();
        }

        [Test]
        public void CreateNumberBadX() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Number number = new Number(-1, 0, 0);
        });

        [Test]
        public void CreateNumberBadY() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Number number = new Number(0, -1, 0);
        });

        [Test]
        public void CreateNumberBadZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Number number = new Number(0, 0, -1);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties()
        {
            Number number = new Number(0, 0, 0);
            int x = number.X;
            int y = number.Y;
            int z = number.Z;

            Assert.Pass();
        }

        #endregion

        #region Methods

        #region Flip

        [Test]
        public void FlipNormal()
        {
            Number number = new Number(1234, 123, 10);
            number.Flip();

            Assert.Pass();
        }

        [Test]
        public void FlipNullNumber() => Assert.Throws<ArgumentNullException>(() => Number.Flip(null));

        #endregion

        #region ToGeoCoordinates

        #region ToGeodeticCoordinates

        [Test]
        public void ToGeodeticCoordinatesNormal()
        {
            Number number = new Number(1234, 123, 10);

            number.ToGeodeticCoordinates(Tile.DefaultSize);

            Assert.Pass();
        }

        [Test]
        public void ToGeodeticCoordinatesNullSize()
        {
            Number number = new Number(1234, 123, 10);

            Assert.Throws<ArgumentNullException>(() => number.ToGeodeticCoordinates(null));
        }

        [Test]
        public void ToGeodeticCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() =>
               Number.ToGeodeticCoordinates(null, Tile.DefaultSize));

        #endregion

        #region ToMercatorCoordinates

        [Test]
        public void ToMercatorCoordinatesNormal()
        {
            Number number = new Number(1234, 123, 11);

            number.ToMercatorCoordinates(Tile.DefaultSize);

            Assert.Pass();
        }

        [Test]
        public void ToMercatorCoordinatesNullSize()
        {
            Number number = new Number(1234, 123, 10);

            Assert.Throws<ArgumentNullException>(() => number.ToMercatorCoordinates(null));
        }

        [Test]
        public void ToMercatorCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() =>
               Number.ToMercatorCoordinates(null, Tile.DefaultSize));

        #endregion

        #region ToGeoCoordinates

        [Test]
        public void ToGeoCoordinatesGeodesic()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            number.ToGeoCoordinates(cs, Tile.DefaultSize, false);

            Assert.Pass();
        }

        [Test]
        public void ToGeoCoordinatesMercator()
        {
            Number number = new Number(1234, 123, 11);
            const CoordinateSystem cs = CoordinateSystem.Epsg3857;

            number.ToGeoCoordinates(cs, Tile.DefaultSize, true);

            Assert.Pass();
        }

        [Test]
        public void ToGeoCoordinatesOtherSystem()
        {
            Number number = new Number(1234, 123, 10);
            const CoordinateSystem cs = CoordinateSystem.Other;

            Assert.Throws<NotSupportedException>(() => number.ToGeoCoordinates(cs, Tile.DefaultSize, true));
        }

        [Test]
        public void ToGeoCoordinatesNullNumber()
        {
            const CoordinateSystem cs = CoordinateSystem.Epsg4326;

            Assert.Throws<ArgumentNullException>(() => Number.ToGeoCoordinates(null, cs, Tile.DefaultSize, false));
        }

        #endregion

        #endregion

        #region GetLowerNumbers

        [Test]
        public void GetLowerNumbersNormal()
        {
            Number number = new Number(1234, 123, 10);
            (Number minNumber, Number maxNumber) = number.GetLowerNumbers(11);

            Assert.Pass();
        }

        [Test]
        public void GetLowerNumbersBadZoom()
        {
            Number number = new Number(1234, 123, 10);

            Assert.Throws<ArgumentOutOfRangeException>(() => number.GetLowerNumbers(9));
        }

        [Test]
        public void GetLowerNumbersNullNumberm() => Assert.Throws<ArgumentNullException>(() =>
               Number.GetLowerNumbers(null, 10));

        #endregion

        #region GetCount

        [Test]
        public void GetCountNormal()
        {
            GeoCoordinate min = new GeodeticCoordinate(0.0, 0.0);
            GeoCoordinate max = new GeodeticCoordinate(180.0, 90.0);

            int count = Number.GetCount(min, max, 0, 9, false, Tile.DefaultSize);

            Assert.True(count > 0);
        }

        [Test]
        public void GetCountBadMinZ()
        {
            GeoCoordinate min = new GeodeticCoordinate(0.0, 0.0);
            GeoCoordinate max = new GeodeticCoordinate(180.0, 90.0);

            Assert.Throws<ArgumentOutOfRangeException>(() => Number.GetCount(min, max, -1, 9, false, Tile.DefaultSize));
        }

        [Test]
        public void GetCountBadMaxZ()
        {
            GeoCoordinate min = new GeodeticCoordinate(0.0, 0.0);
            GeoCoordinate max = new GeodeticCoordinate(180.0, 90.0);

            Assert.Throws<ArgumentOutOfRangeException>(() => Number.GetCount(min, max, 0, -1, false, Tile.DefaultSize));
        }

        #endregion

        #endregion

        #region Bool comparings

        #region GetHashCode

        [Test]
        public void GetHashCodeNormal()
        {
            Number number = new Number(1234, 123, 10);

            int hashCode = number.GetHashCode();

            Assert.Pass();
        }

        #endregion

        #region Equals

        [Test]
        public void EqualsByValueNormal()
        {
            Number number1 = new Number(1234, 123, 10);
            Number number2 = new Number(1234, 123, 10);

            Assert.True(number1.Equals((object)number2));
        }

        [Test]
        public void EqualsByRefNormal()
        {
            Number number1 = new Number(1234, 123, 10);
            Number number2 = number1;

            Assert.True(number1.Equals((object)number2));
        }

        [Test]
        public void EqualsOtherNull()
        {
            Number number1 = new Number(1234, 123, 10);

            Assert.False(number1.Equals(null));
        }

        #region Equal operator

        [Test]
        public void EqualsOperatorNormalTrue()
        {
            Number number1 = new Number(1234, 123, 10);
            Number number2 = new Number(1234, 123, 10);

            Assert.True(number1 == number2);
        }

        [Test]
        public void EqualsOperatorNormalFalse()
        {
            Number number1 = new Number(1234, 123, 10);
            Number number2 = new Number(1235, 123, 10);

            Assert.False(number1 == number2);
        }

        [Test]
        public void EqualsOperatorNumber1Null()
        {
            Number number2 = new Number(1234, 123, 10);

            Assert.False(null == number2);
        }

        [Test]
        public void EqualsOperatorNumber2Null()
        {
            Number number1 = new Number(1234, 123, 10);

            Assert.False(number1 == null);
        }

        #endregion

        #region Not equal operator

        [Test]
        public void NotEqualsOperatorNormalTrue()
        {
            Number number1 = new Number(1234, 123, 10);
            Number number2 = new Number(1235, 123, 10);

            Assert.True(number1 != number2);
        }

        [Test]
        public void NotEqualsOperatorNormalFalse()
        {
            Number number1 = new Number(1234, 123, 10);
            Number number2 = new Number(1234, 123, 10);

            Assert.False(number1 != number2);
        }

        [Test]
        public void NotEqualsOperatorNumber1Null()
        {
            Number number2 = new Number(1234, 123, 10);

            Assert.True(null != number2);
        }

        [Test]
        public void NotEqualsOperatorNumber2Null()
        {
            Number number1 = new Number(1234, 123, 10);

            Assert.True(number1 != null);
        }

        #endregion

        #endregion

        #endregion

        #region Math

        #region Add

        [Test]
        public void AddNormal()
        {
            Number number1 = new Number(0, 0, 0);
            Number number2 = new Number(1, 1, 0);
            Number result = new Number(1, 1, 0);

            Number add = number1.Add(number2);
            Assert.True(add == result);
        }

        [Test]
        public void AddNullNumber1()
        {
            Number number2 = new Number(1, 1, 0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Number add = null + number2;
            });
        }

        [Test]
        public void AddNullNumber2()
        {
            Number number1 = new Number(0, 0, 0);

            Assert.Throws<ArgumentNullException>(() => number1.Add(null));
        }

        [Test]
        public void AddDifferentZ()
        {
            Number number1 = new Number(0, 0, 0);
            Number number2 = new Number(1, 1, 1);

            Assert.Throws<ArgumentException>(() => number1.Add(number2));
        }

        #endregion

        #region Subtract

        [Test]
        public void SubtractNormal()
        {
            Number number1 = new Number(1, 1, 0);
            Number number2 = new Number(0, 0, 0);
            Number result = new Number(1, 1, 0);

            Number sub = number1.Subtract(number2);
            Assert.True(sub == result);
        }

        [Test]
        public void SubtractNullNumber1()
        {
            Number number2 = new Number(0, 0, 0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Number sub = null - number2;
            });
        }

        [Test]
        public void SubtractNullNumber2()
        {
            Number number1 = new Number(1, 1, 0);

            Assert.Throws<ArgumentNullException>(() => number1.Subtract(null));
        }

        [Test]
        public void SubtractDifferentZ()
        {
            Number number1 = new Number(1, 1, 1);
            Number number2 = new Number(0, 0, 0);

            Assert.Throws<ArgumentException>(() => number1.Subtract(number2));
        }

        #endregion

        #region Multiply

        [Test]
        public void MultiplyNormal()
        {
            Number number1 = new Number(0, 0, 0);
            Number number2 = new Number(1, 1, 0);
            Number result = new Number(0, 0, 0);

            Number mul = number1.Multiply(number2);
            Assert.True(mul == result);
        }

        [Test]
        public void MultiplyNullNumber1()
        {
            Number number2 = new Number(1, 1, 0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Number mul = null * number2;
            });
        }

        [Test]
        public void MultiplyNullNumber2()
        {
            Number number1 = new Number(0, 0, 0);

            Assert.Throws<ArgumentNullException>(() => number1.Multiply(null));
        }

        [Test]
        public void MultiplyDifferentZ()
        {
            Number number1 = new Number(0, 0, 0);
            Number number2 = new Number(1, 1, 1);

            Assert.Throws<ArgumentException>(() => number1.Multiply(number2));
        }

        #endregion

        #region Divide

        [Test]
        public void DivideNormal()
        {
            Number number1 = new Number(0, 0, 0);
            Number number2 = new Number(1, 1, 0);
            Number result = new Number(0, 0, 0);

            Number div = number1.Divide(number2);
            Assert.True(div == result);
        }

        [Test]
        public void DivideNullNumber1()
        {
            Number number2 = new Number(1, 1, 0);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Number div = null / number2;
            });
        }

        [Test]
        public void DivideNullNumber2()
        {
            Number number1 = new Number(0, 0, 0);

            Assert.Throws<ArgumentNullException>(() => number1.Divide(null));
        }

        [Test]
        public void DivideDifferentZ()
        {
            Number number1 = new Number(0, 0, 0);
            Number number2 = new Number(1, 1, 1);

            Assert.Throws<ArgumentException>(() => number1.Divide(number2));
        }

        #endregion

        #endregion
    }
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0219 // The variable is assigned but it's value is never used
