using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    public class MaxTests
    {
        [Theory]
        [InlineData(1, 0)]
        [InlineData(-1, 2)]
        [InlineData(1, 1)]
        public void MaxTest(double a, double b)
        {
            var source0 = new Constant { ConstantValue = a };
            var source1 = new Constant { ConstantValue = b };
            var module = new Max { Source0 = source0, Source1 = source1 };

            Assert.Equal(Math.Max(a, b), module.GetValue(0, 0, 0));
        }
    }
}
