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
    [TestFixture]
    public sealed class NumberTests
    {
        #region Consts

        private const int GeodeticX = 1819;

        private const int GeodeticY = 309;

        private const int GeodeticFlipY = 714;

        private const int MercatorX = 909;

        private const int MercatorY = 403;

        private readonly GeodeticCoordinate _tokyoGeodeticMin = new GeodeticCoordinate(139.746094, 35.507812);

        private readonly GeodeticCoordinate _tokyoGeodeticMax = new GeodeticCoordinate(139.921875, 35.683594);

        private readonly MercatorCoordinate _tokyoMercatorMin = new MercatorCoordinate(15536896.12, 4226661.92);

        private readonly MercatorCoordinate _tokyoMercatorMax = new MercatorCoordinate(15576031.88, 4265797.67);

        private const CoordinateSystem Cs4326 = CoordinateSystem.Epsg4326;

        private const CoordinateSystem Cs3857 = CoordinateSystem.Epsg3857;

        private const CoordinateSystem CsOther = CoordinateSystem.Other;

        #endregion

        #region Constructors

        [Test]
        public void CreateNumberNormal() => Assert.DoesNotThrow(() =>
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);
        });

        [Test]
        public void CreateNumberBadX() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Number number = new Number(-GeodeticX, GeodeticY, 10);
        });

        [Test]
        public void CreateNumberBadY() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Number number = new Number(GeodeticX, -GeodeticY, 10);
        });

        [Test]
        public void CreateNumberBadZ() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Number number = new Number(GeodeticX, GeodeticY, -1);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

            Assert.True(number.X == GeodeticX);
            Assert.True(number.Y == GeodeticY);
            Assert.True(number.Z == 10);
        }

        #endregion

        #region Methods

        #region Flip

        [Test]
        public void FlipNormal()
        {
            Number expected = new Number(GeodeticX, GeodeticFlipY, 10);
            Number number = new Number(GeodeticX, GeodeticY, 10);
            Number result = null;

            Assert.DoesNotThrow(() => result = number.Flip());
            Assert.True(result == expected);
        }

        [Test]
        public void FlipNullNumber() => Assert.Throws<ArgumentNullException>(() => Number.Flip(null));

        #endregion

        #region ToGeoCoordinates

        #region ToGeodeticCoordinates

        [Test]
        public void ToGeodeticCoordinatesNormal()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

            Coordinate minCoordinate = null;
            Coordinate maxCoordinate = null;

            Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = number.ToGeodeticCoordinates(Tile.DefaultSize, false));

            Coordinate min = (Coordinate)minCoordinate.Round(6);
            Coordinate max = (Coordinate)maxCoordinate.Round(6);

            Assert.True(min == _tokyoGeodeticMin && max == _tokyoGeodeticMax);
        }

        [Test]
        public void ToGeodeticCoordinatesNullSize()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

            Assert.Throws<ArgumentNullException>(() => number.ToGeodeticCoordinates(null, false));
        }

        [Test]
        public void ToGeodeticCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() =>
               Number.ToGeodeticCoordinates(null, Tile.DefaultSize, false));

        #endregion

        #region ToMercatorCoordinates

        [Test]
        public void ToMercatorCoordinatesNormal()
        {
            Number number = new Number(MercatorX, MercatorY, 10);

            Coordinate minCoordinate = null;
            Coordinate maxCoordinate = null;

            Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = number.ToMercatorCoordinates(Tile.DefaultSize, false));

            Coordinate min = (Coordinate)minCoordinate.Round(2);
            Coordinate max = (Coordinate)maxCoordinate.Round(2);

            Assert.True(min == _tokyoMercatorMin && max == _tokyoMercatorMax);
        }

        [Test]
        public void ToMercatorCoordinatesNullSize()
        {
            Number number = new Number(MercatorX, MercatorY, 10);

            Assert.Throws<ArgumentNullException>(() => number.ToMercatorCoordinates(null, false));
        }

        [Test]
        public void ToMercatorCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() =>
               Number.ToMercatorCoordinates(null, Tile.DefaultSize, false));

        #endregion

        #region ToGeoCoordinates

        [Test]
        public void ToGeoCoordinatesGeodetic()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

            Coordinate minCoordinate = null;
            Coordinate maxCoordinate = null;

            Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = number.ToGeoCoordinates(Cs4326, Tile.DefaultSize, false));

            Coordinate min = (Coordinate)minCoordinate.Round(6);
            Coordinate max = (Coordinate)maxCoordinate.Round(6);

            Assert.True(min == _tokyoGeodeticMin && max == _tokyoGeodeticMax);
        }

        [Test]
        public void ToGeoCoordinatesMercator()
        {
            Number number = new Number(MercatorX, MercatorY, 10);

            Coordinate minCoordinate = null;
            Coordinate maxCoordinate = null;

            Assert.DoesNotThrow(() => (minCoordinate, maxCoordinate) = number.ToGeoCoordinates(Cs3857, Tile.DefaultSize, false));

            Coordinate min = (Coordinate)minCoordinate.Round(2);
            Coordinate max = (Coordinate)maxCoordinate.Round(2);

            Assert.True(min == _tokyoMercatorMin && max == _tokyoMercatorMax);
        }

        [Test]
        public void ToGeoCoordinatesOtherSystem()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

            Assert.Throws<NotSupportedException>(() => number.ToGeoCoordinates(CsOther, Tile.DefaultSize, true));
        }

        [Test]
        public void ToGeoCoordinatesNullNumber() => Assert.Throws<ArgumentNullException>(() => Number.ToGeoCoordinates(null, Cs4326, Tile.DefaultSize, false));

        #endregion

        #endregion

        #region GetLowerNumbers

        [Test]
        public void GetLowerNumbersNormal()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

            Number minExpected = new Number(3638, 618, 11);
            Number maxExpected = new Number(3639, 619, 11);

            (Number minNumber, Number maxNumber) = number.GetLowerNumbers(11);

            Assert.True(minNumber == minExpected && maxNumber == maxExpected);
        }

        [Test]
        public void GetLowerNumbersBadZoom()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

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
            int count = 0;

            Assert.DoesNotThrow(() => count = Number.GetCount(_tokyoGeodeticMin, _tokyoGeodeticMax,
                                                              0, 12, false, Tile.DefaultSize));
            Assert.True(count > 0);
        }

        [Test]
        public void GetCountBadMinZ() => Assert.Throws<ArgumentOutOfRangeException>(() => Number.GetCount(_tokyoGeodeticMin, _tokyoGeodeticMax,
                                                                                                          -1, 9, false, Tile.DefaultSize));

        [Test]
        public void GetCountBadMaxZ() => Assert.Throws<ArgumentOutOfRangeException>(() => Number.GetCount(_tokyoGeodeticMin, _tokyoGeodeticMax,
                                                                                                          0, -1, false, Tile.DefaultSize));

        #endregion

        #endregion

        #region Bool comparings

        #region GetHashCode

        [Test]
        public void GetHashCodeNormal()
        {
            Number number = new Number(GeodeticX, GeodeticY, 10);

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
            Number number1 = new Number(GeodeticX, GeodeticY, 10);
            Number number2 = new Number(GeodeticX, GeodeticY, 10);

            Assert.True(number1.Equals((object)number2));
        }

        [Test]
        public void EqualsByRefNormal()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);
            Number number2 = number1;

            Assert.True(number1.Equals((object)number2));
        }

        [Test]
        public void EqualsOtherNull()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);

            Assert.False(number1.Equals(null));
        }

        #region Equal operator

        [Test]
        public void EqualsOperatorNormalTrue()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);
            Number number2 = new Number(GeodeticX, GeodeticY, 10);

            Assert.True(number1 == number2);
        }

        [Test]
        public void EqualsOperatorNormalFalse()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);
            Number number2 = new Number(GeodeticX + 1, GeodeticX, 10);

            Assert.False(number1 == number2);
        }

        [Test]
        public void EqualsOperatorNumber1Null()
        {
            Number number2 = new Number(GeodeticX, GeodeticY, 10);

            Assert.False(null == number2);
        }

        [Test]
        public void EqualsOperatorNumber2Null()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);

            Assert.False(number1 == null);
        }

        #endregion

        #region Not equal operator

        [Test]
        public void NotEqualsOperatorNormalTrue()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);
            Number number2 = new Number(GeodeticX + 1, GeodeticY, 10);

            Assert.True(number1 != number2);
        }

        [Test]
        public void NotEqualsOperatorNormalFalse()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);
            Number number2 = new Number(GeodeticX, GeodeticY, 10);

            Assert.False(number1 != number2);
        }

        [Test]
        public void NotEqualsOperatorNumber1Null()
        {
            Number number2 = new Number(GeodeticX, GeodeticY, 10);

            Assert.True(null != number2);
        }

        [Test]
        public void NotEqualsOperatorNumber2Null()
        {
            Number number1 = new Number(GeodeticX, GeodeticY, 10);

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
