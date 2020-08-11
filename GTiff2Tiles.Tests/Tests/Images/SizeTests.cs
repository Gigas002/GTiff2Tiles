#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0219 // The variable is assigned but it's value is never used

using System;
using GTiff2Tiles.Core.Images;
using NUnit.Framework;

// ReSharper disable UnusedVariable

namespace GTiff2Tiles.Tests.Tests.Images
{
    [TestFixture]
    public sealed class SizeTests
    {
        #region Constructors

        [Test]
        public void CreateSizeNormal()
        {
            Size size = new Size(1, 1);

            Assert.Pass();
        }

        [Test]
        public void CreateSizeBadWidth() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Size size = new Size(0, 1);
        });

        [Test]
        public void CreateSizeBadHeight() => Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            Size size = new Size(1, 0);
        });

        #endregion

        #region Properties

        [Test]
        public void GetProperties()
        {
            Size size = new Size(1, 2);

            int h = size.Height;
            int w = size.Width;
            int r = size.Resolution;
            bool isSquare = size.IsSquare;

            Assert.Pass();
        }

        #endregion

        #region Bool comparings

        #region GetHashCode

        [Test]
        public void GetHashCodeNormal()
        {
            Size size = new Size(10, 10);

            int hashCode = size.GetHashCode();

            Assert.Pass();
        }

        #endregion

        #region Equals

        [Test]
        public void EqualsByValueNormal()
        {
            Size size1 = new Size(10, 10);
            Size size2 = new Size(10, 10);

            Assert.True(size1.Equals((object)size2));
        }

        [Test]
        public void EqualsByRefNormal()
        {
            Size size1 = new Size(10, 10);
            Size size2 = size1;

            Assert.True(size1.Equals((object)size2));
        }

        [Test]
        public void EqualsOtherNull()
        {
            Size size1 = new Size(10, 10);

            Assert.False(size1.Equals(null));
        }

        #region Equal operator

        [Test]
        public void EqualsOperatorNormalTrue()
        {
            Size size1 = new Size(10, 10);
            Size size2 = new Size(10, 10);

            Assert.True(size1 == size2);
        }

        [Test]
        public void EqualsOperatorNormalFalse()
        {
            Size size1 = new Size(10, 10);
            Size size2 = new Size(11, 10);

            Assert.False(size1 == size2);
        }

        [Test]
        public void EqualsOperatorSize1Null()
        {
            Size size2 = new Size(10, 10);

            Assert.False(null == size2);
        }

        [Test]
        public void EqualsOperatorSize2Null()
        {
            Size size1 = new Size(10, 10);

            Assert.False(size1 == null);
        }

        #endregion

        #region Not equal operator

        [Test]
        public void NotEqualsOperatorNormalTrue()
        {
            Size size1 = new Size(10, 10);
            Size size2 = new Size(11, 10);

            Assert.True(size1 != size2);
        }

        [Test]
        public void NotEqualsOperatorNormalFalse()
        {
            Size size1 = new Size(10, 10);
            Size size2 = new Size(10, 10);

            Assert.False(size1 != size2);
        }

        [Test]
        public void NotEqualsOperatorSize1Null()
        {
            Size size2 = new Size(10, 10);

            Assert.True(null != size2);
        }

        [Test]
        public void NotEqualsOperatorSize2Null()
        {
            Size size1 = new Size(10, 10);

            Assert.True(size1 != null);
        }

        #endregion

        #endregion

        #endregion

        #region Math

        #region Add

        [Test]
        public void AddNormal()
        {
            Size size1 = new Size(1, 1);
            Size size2 = new Size(2, 2);
            Size result = new Size(3, 3);

            Size add = size1.Add(size2);
            Assert.True(add == result);
        }

        [Test]
        public void AddNullSize1()
        {
            Size size2 = new Size(1, 1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Size add = null + size2;
            });
        }

        [Test]
        public void AddNullSize2()
        {
            Size size1 = new Size(1, 1);

            Assert.Throws<ArgumentNullException>(() => size1.Add(null));
        }

        #endregion

        #region Subtract

        [Test]
        public void SubtractNormal()
        {
            Size size1 = new Size(2, 2);
            Size size2 = new Size(1, 1);
            Size result = new Size(1, 1);

            Size sub = size1.Subtract(size2);
            Assert.True(sub == result);
        }

        [Test]
        public void SubtractNullSize1()
        {
            Size size2 = new Size(1, 1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Size sub = null - size2;
            });
        }

        [Test]
        public void SubtractNullSize2()
        {
            Size size1 = new Size(2, 2);

            Assert.Throws<ArgumentNullException>(() => size1.Subtract(null));
        }

        #endregion

        #region Multiply

        [Test]
        public void MultiplyNormal()
        {
            Size size1 = new Size(1, 1);
            Size size2 = new Size(2, 2);
            Size result = new Size(2, 2);

            Size mul = size1.Multiply(size2);
            Assert.True(mul == result);
        }

        [Test]
        public void MultiplyNullSize1()
        {
            Size size2 = new Size(2, 2);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Size mul = null * size2;
            });
        }

        [Test]
        public void MultiplyNullSize2()
        {
            Size size1 = new Size(1, 1);

            Assert.Throws<ArgumentNullException>(() => size1.Multiply(null));
        }

        #endregion

        #region Divide

        [Test]
        public void DivideNormal()
        {
            Size size1 = new Size(2, 2);
            Size size2 = new Size(1, 1);
            Size result = new Size(2, 2);

            Size div = size1.Divide(size2);
            Assert.True(div == result);
        }

        [Test]
        public void DivideNullSize1()
        {
            Size size2 = new Size(1, 1);

            Assert.Throws<ArgumentNullException>(() =>
            {
                Size div = null / size2;
            });
        }

        [Test]
        public void DivideNullSize2()
        {
            Size size1 = new Size(2, 2);

            Assert.Throws<ArgumentNullException>(() => size1.Divide(null));
        }

        #endregion

        #endregion
    }
}

#pragma warning restore IDE0059 // Unnecessary assignment of a value
#pragma warning restore CS0219 // The variable is assigned but it's value is never used
