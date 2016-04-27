using SharpNoise.Builders;
using SharpNoise.Modules;
using Xunit;

namespace SharpNoise.Tests
{
    /// <summary>
    /// Tests for <see cref="NoiseMap"/>
    /// </summary>
    public class MapTests
    {
        readonly NoiseMap map;

        public MapTests()
        {
            map = new NoiseMap(2, 2);
        }

        [Fact]
        public void GetSetTest()
        {
            for (int i = 0; i < map.Height; i++)
            {
                for (int j = 0; j < map.Width; j++)
                {
                    var expected = 99f;
                    map[j, i] = expected;

                    var actual = map[j, i];
                    Assert.Equal(expected, actual);
                }
            }
        }

        [Fact]
        public void BorderValueTest()
        {
            var expected = 1f;
            map.BorderValue = expected;
            map.Clear(10f);

            Assert.Equal(expected, map[-1, -1]);
            Assert.Equal(expected, map[-1, 0]);
            Assert.Equal(expected, map[-1, 1]);
            Assert.Equal(expected, map[-1, 2]);

            Assert.Equal(expected, map[2, -1]);
            Assert.Equal(expected, map[2, 0]);
            Assert.Equal(expected, map[2, 1]);
            Assert.Equal(expected, map[2, 2]);
        }

        [Fact]
        public void ClearTest()
        {
            var expected = 5f;
            map.Clear(expected);
            foreach (var line in map.IterateAllLines())
                foreach (var value in line)
                    Assert.Equal(expected, value);
        }

        [Fact]
        public void LineReaderTest()
        {
            var builder = new CylinderNoiseMapBuilder();
            builder.DestNoiseMap = new NoiseMap();
            builder.SourceModule = new Perlin();
            builder.SetBounds(0, 180, 0, 5);
            builder.SetDestSize(12, 6);
            builder.Build();

            int row = 0, x = 0;
            foreach (var line in builder.DestNoiseMap.IterateAllLines())
            {
                foreach (var value in line)
                {
                    Assert.Equal(builder.DestNoiseMap[x, row], value);
                    x++;
                }
                row++;
                x = 0;
            }
        }
    }
}
