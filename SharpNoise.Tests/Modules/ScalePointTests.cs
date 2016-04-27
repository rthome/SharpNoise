using SharpNoise.Modules;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    public class ScalePointTests
    {
        [Theory]
        [InlineData(1, 1, 1)]
        [InlineData(2, 2, 2)]
        [InlineData(1, 0.5, 2)]
        [InlineData(0, 0, 0)]
        public void ScalePointTest(double sx, double sy, double sz)
        {
            var source = new Perlin();
            var module = new ScalePoint
            {
                XScale = sx,
                YScale = sy,
                ZScale = sz,
                Source0 = source,
            };

            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        var expected = source.GetValue(x * sx, y * sy, z * sz);
                        var actual = module.GetValue(x, y, z);
                        Assert.Equal(expected, actual);
                    }
                }
            }

        }
    }
}
