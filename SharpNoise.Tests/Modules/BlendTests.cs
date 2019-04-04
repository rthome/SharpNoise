using System.Collections.Generic;
using System.Linq;

using SharpNoise.Modules;

using Xunit;

namespace SharpNoise.Tests.Modules
{
    /// <summary>
    /// Tests for the <see cref="Blend"/> module
    /// </summary>
    public class BlendTests
    {
        public static IEnumerable<object[]> AlphaDataSource => Enumerable.Range(-3, 7).Select(i => i / 10.0).Select(i => new object[] { i });

        public static IEnumerable<object[]> ScaleDataSource => Enumerable.Range(-2, 5).Select(i => new object[] { (double)i });

        public static IEnumerable<object[]> ScaledAlphas()
        {
            foreach (var a in AlphaDataSource.Select(n => n.Single()))
            {
                foreach (var scale in ScaleDataSource.Select(n => n.Single()))
                {
                    yield return new object[] { a, scale };
                }
            }
        }

        [Theory]
        [MemberData(nameof(AlphaDataSource))]
        public void BlendZeroToOneTest(double a)
        {
            var module = new Blend
            {
                Control = new Constant { ConstantValue = a },
                Source0 = new Constant { ConstantValue = 0 },
                Source1 = new Constant { ConstantValue = 1 },
            };
            var expected = 0.5 * (a + 1); // rescale a to [0, 1]
            Assert.Equal(expected, module.GetValue(0, 0, 0), 12);
            Assert.Equal(expected, module.GetValue(10, 10, 10), 12);
        }

        [Theory]
        [MemberData(nameof(ScaledAlphas))]
        public void BlendScaledRangeTest(double a, double scale)
        {
            //var a = data[0];
            //var scale = 0;
            var module = new Blend
            {
                Control = new Constant { ConstantValue = a },
                Source0 = new Constant { ConstantValue = 0 },
                Source1 = new Constant { ConstantValue = 1 * scale },
            };
            var expected = scale * 0.5 * (a + 1); // rescale a to [0, scale]
            Assert.Equal(expected, module.GetValue(0, 0, 0), 12);
            Assert.Equal(expected, module.GetValue(10, 10, 10), 12);
        }
    }
}

