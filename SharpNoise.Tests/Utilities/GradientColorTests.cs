using SharpNoise.Utilities.Imaging;
using System;
using Xunit;

namespace SharpNoise.Tests.Utilities
{
    public class GradientColorTests
    {
        readonly GradientColor gradient;

        public GradientColorTests()
        {
            gradient = new GradientColor();
            gradient.AddGradientPoint(0, new Color(0, 0, 0, byte.MaxValue));
            gradient.AddGradientPoint(1, new Color(byte.MaxValue, 0, 0, byte.MaxValue));
        }

        [Fact]
        public void InsertPointTest()
        {
            var expected = 2;
            var actual = gradient.PointCount;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void InsertSamePointTest()
        {
            Assert.Throws<ArgumentException>(() => gradient.AddGradientPoint(1, new Color()));
        }

        [Theory]
        [InlineData(1.0, byte.MaxValue, 0, 0, byte.MaxValue)]
        [InlineData(0.5, byte.MaxValue / 2, 0, 0, byte.MaxValue)]
        [InlineData(0.0, 0, 0, 0, byte.MaxValue)]
        public void GetColorTest(double alpha, byte r, byte g, byte b, byte a)
        {
            var color = gradient.GetColor(alpha);
            var expectedColor = new Color(r, g, b, a);

            Assert.Equal(expectedColor, color);
        }

        [Fact]
        public void ClearPointsTest()
        {
            gradient.ClearGradientPoints();

            Assert.Equal(0, gradient.PointCount);
        }
    }
}
