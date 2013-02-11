using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SharpNoise.Tests
{
    [TestClass]
    class NoiseMathTests
    {
        [TestMethod]
        public void ClampTest1()
        {
            var expected = 10;
            Assert.AreEqual(expected, NoiseMath.Clamp(15, 0, expected));
        }

        [TestMethod]
        public void ClampTest2()
        {
            var expected = 5;
            Assert.AreEqual(expected, NoiseMath.Clamp(expected, 0, 10));
        }

        [TestMethod]
        public void ClampTest3()
        {
            var expected = 10;
            Assert.AreEqual(expected, NoiseMath.Clamp(5, expected, 20));
        }

        [TestMethod]
        public void MaxTest()
        {
            var min = 0D;
            var max = 1D; 
            Assert.AreEqual(max, NoiseMath.Max(min, max));
        }

        [TestMethod]
        public void MaxTest2()
        {
            var min = 0D;
            var max = 1D;
            Assert.AreEqual(max, NoiseMath.Max(max, min));
        }

        [TestMethod]
        public void MinTest()
        {
            var min = 0D;
            var max = 1D;
            Assert.AreEqual(min, NoiseMath.Min(min, max));
        }

        [TestMethod]
        public void MinTest2()
        {
            var min = 0D;
            var max = 1D;
            Assert.AreEqual(min, NoiseMath.Min(max, min));
        }

        [TestMethod]
        public void LinearTest1()
        {
            var value1 = 0D;
            var value2 = 1D;
            var alpha = 0.5D;

            var expected = 0.5D;
            var actual = NoiseMath.Linear(value1, value2, alpha);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LinearTest2()
        {
            var value1 = 0D;
            var value2 = 1D;
            var alpha = 0D;

            var expected = 0D;
            var actual = NoiseMath.Linear(value1, value2, alpha);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LinearTest3()
        {
            var value1 = 0D;
            var value2 = 5D;
            var alpha = 1D;

            var expected = 5D;
            var actual = NoiseMath.Linear(value1, value2, alpha);

            Assert.AreEqual(expected, actual);
        }
    }
}
