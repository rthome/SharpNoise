using SharpNoise.Modules;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    /// <summary>
    /// Tests for the <see cref="Clamp"/> module
    /// </summary>
    public class ClampTests
    {
        [Theory]
        [InlineData(25)]
        [InlineData(5)]
        [InlineData(-1)]
        public void ClampTest(double data)
        {
            const double max = 10D;
            const double min = 0D;

            var source = new Constant { ConstantValue = data };
            var module = new Clamp { LowerBound = min, UpperBound = max, Source0 = source };

            Assert.InRange(module.GetValue(0, 0, 0), min, max);
        }
    }
}
