using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Utilities;
using System;

namespace SharpNoise.Tests
{
    [TestClass]
    public class GradientColorTests
    {
        GradientColor gradient;

        [TestInitialize]
        public void SetUp()
        {
            gradient = new GradientColor();
        }

        [TestMethod]
        public void Gradient_Insert_Test()
        {
            gradient.AddGradientPoint(0, new Color());
            gradient.AddGradientPoint(1, new Color());

            var expected  = 2;
            var actual = gradient.PointCount;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Gradient_Insert_SameValue_Test()
        {
            gradient.AddGradientPoint(0, new Color());
            gradient.AddGradientPoint(1, new Color());
            gradient.AddGradientPoint(1, new Color());
        }

        [TestMethod]
        public void Gradient_GetColor_Middle_Test()
        {
            gradient.AddGradientPoint(0, new Color(0, 0, 0, byte.MaxValue));
            gradient.AddGradientPoint(1, new Color(byte.MaxValue, 0, 0, byte.MaxValue));

            var color = gradient.GetColor(0.5);
            var expectedColor = new Color(byte.MaxValue / 2, 0, 0, byte.MaxValue);

            Assert.AreEqual(expectedColor, color);
        }

        [TestMethod]
        public void Gradient_GetColor_UpperEnd_Test()
        {
            gradient.AddGradientPoint(0, new Color(0, 0, 0, byte.MaxValue));
            gradient.AddGradientPoint(1, new Color(byte.MaxValue, 0, 0, byte.MaxValue));

            var color = gradient.GetColor(1);
            var expectedColor = new Color(byte.MaxValue, 0, 0, byte.MaxValue);

            Assert.AreEqual(expectedColor, color);
        }

        [TestMethod]
        public void Gradient_GetColor_LowerEnd_Test()
        {
            gradient.AddGradientPoint(0, new Color(0, 0, 0, byte.MaxValue));
            gradient.AddGradientPoint(1, new Color(byte.MaxValue, 0, 0, byte.MaxValue));

            var color = gradient.GetColor(0);
            var expectedColor = new Color(0, 0, 0, byte.MaxValue);

            Assert.AreEqual(expectedColor, color);
        }

        [TestMethod]
        public void Gradient_ClearPoints_Test()
        {
            gradient.AddGradientPoint(0, new Color());
            gradient.AddGradientPoint(1, new Color());

            gradient.ClearGradientPoints();

            Assert.AreEqual(0, gradient.PointCount);
        }
    }
}
