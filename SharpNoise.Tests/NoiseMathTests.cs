using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SharpNoise.Tests
{
    [TestClass]
    public class NoiseMathTests
    {
        [TestMethod]
        public void Math_Clamp_OutOfRange_Upper_Test()
        {
            var expected = 10;
            Assert.AreEqual(expected, NoiseMath.Clamp(15, 0, expected));
        }

        [TestMethod]
        public void Math_Clamp_InRange_Test()
        {
            var expected = 5;
            Assert.AreEqual(expected, NoiseMath.Clamp(expected, 0, 10));
        }

        [TestMethod]
        public void Math_Clamp_OutOfRange_Lower_Test()
        {
            var expected = 10;
            Assert.AreEqual(expected, NoiseMath.Clamp(5, expected, 20));
        }

        [TestMethod]
        public void Math_Max_Test()
        {
            var min = 0D;
            var max = 1D; 
            Assert.AreEqual(max, NoiseMath.Max(min, max));
        }

        [TestMethod]
        public void Math_Max_Reversed_Test()
        {
            var min = 0D;
            var max = 1D;
            Assert.AreEqual(max, NoiseMath.Max(max, min));
        }

        [TestMethod]
        public void Math_Min_Test()
        {
            var min = 0D;
            var max = 1D;
            Assert.AreEqual(min, NoiseMath.Min(min, max));
        }

        [TestMethod]
        public void Math_Min_Reversed_Test()
        {
            var min = 0D;
            var max = 1D;
            Assert.AreEqual(min, NoiseMath.Min(max, min));
        }

        [TestMethod]
        public void Math_Interp_Linear_Middle_Test()
        {
            var value1 = 0D;
            var value2 = 1D;
            var alpha = 0.5D;

            var expected = 0.5D;
            var actual = NoiseMath.Linear(value1, value2, alpha);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Math_Interp_Linear_Left_Test()
        {
            var value1 = 0D;
            var value2 = 1D;
            var alpha = 0D;

            var expected = 0D;
            var actual = NoiseMath.Linear(value1, value2, alpha);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Math_Interp_Linear_Right_Test()
        {
            var value1 = 0D;
            var value2 = 5D;
            var alpha = 1D;

            var expected = 5D;
            var actual = NoiseMath.Linear(value1, value2, alpha);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Math_LatLonToXYZ_Test_1()
        {
            double x, y, z;
            NoiseMath.LatLonToXYZ(0, 0, out x, out y, out z);

            double expectedX = 1, 
                   expectedY = 0, 
                   expectedZ = 0;

            Assert.AreEqual(expectedX, x);
            Assert.AreEqual(expectedY, y);
            Assert.AreEqual(expectedZ, z);
        }

        [TestMethod]
        public void Math_LatLonToXYZ_Test_2()
        {
            double x, y, z;
            NoiseMath.LatLonToXYZ(-51, 63, out x, out y, out z);

            double expectedX = 0.285705,
                   expectedY = -0.777146,
                   expectedZ = 0.560729;

            Assert.AreEqual(expectedX, Math.Round(x, 6));
            Assert.AreEqual(expectedY, Math.Round(y, 6));
            Assert.AreEqual(expectedZ, Math.Round(z, 6));
        }

        [TestMethod]
        public void Math_LatLonToXYZ_Test_3()
        {
            double x, y, z;
            NoiseMath.LatLonToXYZ(45, 0, out x, out y, out z);

            double expectedX = 0.707107,
                   expectedY = 0.707107,
                   expectedZ = 0;

            Assert.AreEqual(expectedX, Math.Round(x, 6));
            Assert.AreEqual(expectedY, Math.Round(y, 6));
            Assert.AreEqual(expectedZ, Math.Round(z, 6));
        }

        [TestMethod]
        public void Math_LatLonToXYZ_Test_4()
        {
            double x, y, z;
            NoiseMath.LatLonToXYZ(45, 45, out x, out y, out z);

            double expectedX = 0.5,
                   expectedY = 0.707107,
                   expectedZ = 0.5;

            Assert.AreEqual(expectedX, Math.Round(x, 6));
            Assert.AreEqual(expectedY, Math.Round(y, 6));
            Assert.AreEqual(expectedZ, Math.Round(z, 6));
        }

        [TestMethod]
        public void Math_Interp_SCurve3_Lower_Test()
        {
            double v = 0;

            double expected = 0;

            Assert.AreEqual(expected, NoiseMath.SCurve3(v));
        }

        [TestMethod]
        public void Math_Interp_SCurve3_Upper_Test()
        {
            double v = 1;

            double expected = 1;

            Assert.AreEqual(expected, NoiseMath.SCurve3(v));
        }

        [TestMethod]
        public void Math_Interp_SCurve3_Middle_Test()
        {
            double v = 0.5;

            double expected = 0.5;

            Assert.AreEqual(expected, NoiseMath.SCurve3(v));
        }

        [TestMethod]
        public void Math_Interp_SCurve3_Somewhere_Test_1()
        {
            double v = 0.333;

            double expected = 0.258815;

            Assert.AreEqual(expected, Math.Round(NoiseMath.SCurve3(v), 6));
        }

        [TestMethod]
        public void Math_Interp_SCurve3_Somewhere_Test_2()
        {
            double v = 0.9;

            double expected = 0.972;

            Assert.AreEqual(expected, NoiseMath.SCurve3(v));
        }

        [TestMethod]
        public void Math_Interp_SCurve5_Lower_Test()
        {
            double v = 0;

            double expected = 0;

            Assert.AreEqual(expected, NoiseMath.SCurve5(v));
        }

        [TestMethod]
        public void Math_Interp_SCurve5_Upper_Test()
        {
            double v = 1;

            double expected = 1;

            Assert.AreEqual(expected, NoiseMath.SCurve5(v));
        }

        [TestMethod]
        public void Math_Interp_SCurve5_Middle_Test()
        {
            double v = 0.5;

            double expected = 0.5;

            Assert.AreEqual(expected, NoiseMath.SCurve5(v));
        }

        [TestMethod]
        public void Math_Interp_SCurve5_Somewhere_Test_1()
        {
            double v = 0.333;

            double expected = 0.209383;

            Assert.AreEqual(expected, Math.Round(NoiseMath.SCurve5(v), 6));
        }

        [TestMethod]
        public void Math_Interp_SCurve5_Somewhere_Test_2()
        {
            double v = 0.9;

            double expected = 0.99144;

            Assert.AreEqual(expected, Math.Round(NoiseMath.SCurve5(v), 6));
        }
    }
}
