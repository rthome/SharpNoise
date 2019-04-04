using Xunit;

namespace SharpNoise.Tests
{
    /// <summary>
    /// Tests for <see cref="NoiseMath"/>
    /// </summary>
    public class NoiseMathTests
    {
        [Theory]
        [InlineData(10.0)]
        [InlineData(-10.0)]
        public void ClampOutOfRangeTest(double data)
        {
            Assert.NotInRange(data, -1.0, 1.0);
            Assert.InRange(NoiseMath.Clamp(data, -1.0, 1.0), -1.0, 1.0);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(-1.0)]
        [InlineData(0.0)]
        [InlineData(0.5)]
        public void ClampInRangeTest(double data)
        {
            Assert.InRange(data, -1.0, 1.0);
            Assert.Equal(data, NoiseMath.Clamp(data, -1.0, 1.0));
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(0.0)]
        [InlineData(0.1)]
        [InlineData(1.0 / 3.0)]
        public void LinearInterpTest(double alpha)
        {
            const double N0 = 0.0;
            const double N1 = 1.0;

            Assert.Equal(alpha, NoiseMath.Linear(N0, N1, alpha));
        }

        [Theory]
        [InlineData(0.0, 0.0, 1.0, 0.0, 0.0)]
        [InlineData(-51.0, 63.0, 0.285705, -0.777146, 0.560729)]
        [InlineData(45.0, 0.0, 0.707107, 0.707107, 0.0)]
        [InlineData(45.0, 45.0, 0.5, 0.707107, 0.5)]
        public void LatLonTest(double lat, double lon, double expX, double expY, double expZ)
        {
            NoiseMath.LatLonToXYZ(lat, lon, out double x, out double y, out double z);
            Assert.Equal(expX, x, 6);
            Assert.Equal(expY, y, 6);
            Assert.Equal(expZ, z, 6);
        }

        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(1.0, 1.0)]
        [InlineData(0.5, 0.5)]
        [InlineData(0.333, 0.258815)]
        [InlineData(0.9, 0.972)]
        public void SCurve3Test(double a, double expected)
        {
            Assert.Equal(expected, NoiseMath.SCurve3(a), 6);
        }

        [Theory]
        [InlineData(0.0, 0.0)]
        [InlineData(1.0, 1.0)]
        [InlineData(0.5, 0.5)]
        [InlineData(0.333, 0.209383)]
        [InlineData(0.9, 0.99144)]
        public void SCurve5Test(double a, double expected)
        {
            Assert.Equal(expected, NoiseMath.SCurve5(a), 6);
        }
    }
}
