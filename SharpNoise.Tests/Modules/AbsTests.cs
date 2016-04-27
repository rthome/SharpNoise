using SharpNoise.Modules;
using System;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    public class AbsTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(-1)]
        [InlineData(100)]
        public void AbsTest(double data)
        {
            var source = new Constant { ConstantValue = data };
            var module = new Abs { Source0 = source };

            Assert.Equal(Math.Abs(source.GetValue(0, 0, 0)), module.GetValue(0, 0, 0));
        }
    }
}
