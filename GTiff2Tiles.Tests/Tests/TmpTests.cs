using GTiff2Tiles.Core.GeoTiffs;
using GTiff2Tiles.Core.Images;
using GTiff2Tiles.Core.Tiles;
using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests
{
    public sealed class TmpTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void NumbersEqualityTest()
        {
            Number number000 = new Number(0, 0, 0);
            Number number000_2 = new Number(0, 0, 0);
            Number number100 = new Number(1, 0, 0);
            Number number010 = new Number(0, 1, 0);
            Number number001 = new Number(0, 0, 1);

            Assert.IsTrue(number000.Equals(number000_2));
            Assert.IsFalse(number000.Equals(null));
            Assert.IsFalse(number000.Equals(number100));
            Assert.IsFalse(number000.Equals(number010));
            Assert.IsFalse(number000.Equals(number001));

            Assert.IsTrue(number000 == number000_2);
            Assert.IsFalse(number000 != number000_2);
            Assert.IsFalse(number000 == null);
            Assert.IsTrue(number000 != null);
            Assert.IsFalse(number000 == number100);
            Assert.IsTrue(number000 != number100);
            Assert.IsFalse(number000 == number010);
            Assert.IsTrue(number000 != number010);
            Assert.IsFalse(number000 == number001);
            Assert.IsTrue(number000 != number001);
        }

        [Test]
        public void NumberMathsTest()
        {
            Number number100 = new Number(1, 0, 0);
            Number number010 = new Number(0, 1, 0);

            Assert.IsTrue(number100.Add(number010) == new Number(1, 1, 0));
            //Assert.IsTrue(number100.Subtract(number010) == new Number(1, -1, 0));
            Assert.IsTrue(number100.Multiply(number010) == new Number(0, 0, 0));
            Assert.IsTrue(number100.Divide(new Number(1, 1, 0)) == new Number(1, 0, 0));
        }

        [Test]
        public void SizesEqualityTest()
        {
            Size size1010 = new Size(10, 10);
            Size size1010_2 = new Size(10, 10);
            Size size2010 = new Size(20, 10);
            Size size1020 = new Size(10, 20);

            Assert.IsTrue(size1010.Equals(size1010_2));
            Assert.IsFalse(size1010.Equals(null));
            Assert.IsFalse(size1010.Equals(size2010));
            Assert.IsFalse(size1010.Equals(size1020));

            Assert.IsTrue(size1010 == size1010_2);
            Assert.IsFalse(size1010 != size1010_2);
            Assert.IsFalse(size1010 == null);
            Assert.IsTrue(size1010 != null);
            Assert.IsFalse(size1010 == size2010);
            Assert.IsTrue(size1010 != size2010);
            Assert.IsFalse(size1010 == size1020);
            Assert.IsTrue(size1010 != size1020);
        }

        [Test]
        public void SizesMathsTest()
        {
            Size size2010 = new Size(20, 10);
            Size size1020 = new Size(10, 20);

            Assert.IsTrue(size2010.Add(size1020) == new Size(30, 30));
            Assert.IsTrue(size2010.Subtract(new Size(10, 9)) == new Size(10, 1));
            Assert.IsTrue(size2010.Multiply(size1020) == new Size(200, 200));
            Assert.IsTrue(size2010.Divide(new Size(10, 10)) == new Size(2, 1));
        }
    }
}
