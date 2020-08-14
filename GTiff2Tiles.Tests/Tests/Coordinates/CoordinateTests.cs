#pragma warning disable IDE0059 // Unnecessary assignment of a value

using System;
using GTiff2Tiles.Core.Coordinates;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Coordinates
{
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

            Assert.True(Math.Abs(expected - res) < double.Epsilon);
        }

        [Test]
        public void RadiansToDegreesTest()
        {
            const double expected = 57.2958;
            double res = Math.Round(Coordinate.RadiansToDegrees(1.0), 4);

            Assert.True(Math.Abs(expected - res) < double.Epsilon);
        }

        #region Round

        [Test]
        public void RoundNormal()
        {
            Coordinate coord = new GeodeticCoordinate(12.341, 13.561);
            Coordinate round = (Coordinate)coord.Round(2);
            Coordinate expected = new GeodeticCoordinate(12.34, 13.56);

            Assert.True(round == expected);
        }

        [Test]
        public void RoundCoordinateNull()
        {
            Coordinate coord = new GeodeticCoordinate(12.341, 13.561);

            Assert.Throws<ArgumentNullException>(() => Coordinate.Round(null, 2));
        }

        [Test]
        public void RoundSmallDigits()
        {
            Coordinate coord = new GeodeticCoordinate(12.341, 13.561);
            Assert.Throws<ArgumentOutOfRangeException>(() => Coordinate.Round(coord, -1));
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

            Assert.True(coordinate1.Equals((object)coordinate2));
        }

        [Test]
        public void EqualsByRefNormal()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
            Coordinate coordinate2 = coordinate1;

            Assert.True(coordinate1.Equals((object)coordinate2));
        }

        [Test]
        public void EqualsOtherNull()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);

            Assert.False(coordinate1.Equals(null));
        }

        #region Equal operator

        [Test]
        public void EqualsOperatorNormalTrue()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
            Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

            Assert.True(coordinate1 == coordinate2);
        }

        [Test]
        public void EqualsOperatorNormalFalse()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
            Coordinate coordinate2 = new GeodeticCoordinate(11.0, 10.0);

            Assert.False(coordinate1 == coordinate2);
        }

        [Test]
        public void EqualsOperatorCoordinate1Null()
        {
            Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

            Assert.False(null == coordinate2);
        }

        [Test]
        public void EqualsOperatorCoordinate2Null()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);

            Assert.False(coordinate1 == null);
        }

        #endregion

        #region Not equal operator

        [Test]
        public void NotEqualsOperatorNormalTrue()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
            Coordinate coordinate2 = new GeodeticCoordinate(11.0, 10.0);

            Assert.True(coordinate1 != coordinate2);
        }

        [Test]
        public void NotEqualsOperatorNormalFalse()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);
            Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

            Assert.False(coordinate1 != coordinate2);
        }

        [Test]
        public void NotEqualsOperatorCoordinate1Null()
        {
            Coordinate coordinate2 = new GeodeticCoordinate(10.0, 10.0);

            Assert.True(null != coordinate2);
        }

        [Test]
        public void NotEqualsOperatorCoordinate2Null()
        {
            Coordinate coordinate1 = new GeodeticCoordinate(10.0, 10.0);

            Assert.True(coordinate1 != null);
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
            Assert.True(add == result);
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
            Assert.True(sub == result);
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
            Assert.True(mul == result);
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
            Assert.True(div == result);
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
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
