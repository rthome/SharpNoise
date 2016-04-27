using SharpNoise.Modules;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    /// <summary>
    /// Tests for the <see cref="Multiply"/> module
    /// </summary>
    public class MultiplyTests
    {
        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 0)]
        [InlineData(1, 1)]
        [InlineData(-2, 2)]
        public void MultiplyTest(double a, double b)
        {
            var source0 = new Constant { ConstantValue = a };
            var source1 = new Constant { ConstantValue = b };
            var module = new Multiply { Source0 = source0, Source1 = source1 };
            Assert.Equal(a * b, module.GetValue(0, 0, 0));
        }
    }
}
