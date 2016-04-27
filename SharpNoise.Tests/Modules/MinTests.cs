using SharpNoise.Modules;
using System;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    public class MinTests
    {
        [Theory]
        [InlineData(1, 0)]
        [InlineData(-1, 2)]
        [InlineData(1, 1)]
        public void MinTest(double a, double b)
        {
            var source0 = new Constant { ConstantValue = a };
            var source1 = new Constant { ConstantValue = b };
            var module = new Min { Source0 = source0, Source1 = source1 };

            Assert.Equal(Math.Min(a, b), module.GetValue(0, 0, 0));
        }
    }
}
