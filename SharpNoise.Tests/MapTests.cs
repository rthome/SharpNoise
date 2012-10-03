using NUnit.Framework;

using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities;

namespace SharpNoise.Tests
{
    [TestFixture]
    class MapTests
    {
        NoiseMap map;

        [SetUp]
        public void SetUp()
        {
            map = new NoiseMap(2, 2);
        }

        [Test]
        public void GetSetValueTest()
        {
            var expected = 99f;
            map.SetValue(1, 1, expected);

            var actual = map.GetValue(1, 1);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void BorderValueTest()
        {
            var expected = 1f;
            map.BorderValue = expected;
            map.Clear(10f);

            Assert.AreEqual(expected, map.GetValue(-1, -1));
            Assert.AreEqual(expected, map.GetValue(-1, 0));
            Assert.AreEqual(expected, map.GetValue(-1, 1));
            Assert.AreEqual(expected, map.GetValue(-1, 2));

            Assert.AreEqual(expected, map.GetValue(2, -1));
            Assert.AreEqual(expected, map.GetValue(2, 0));
            Assert.AreEqual(expected, map.GetValue(2, 1));
            Assert.AreEqual(expected, map.GetValue(2, 2));
        }

        [Test]
        public void ClearTest()
        {
            var expected = 5f;
            map.Clear(expected);
            foreach (var line in map.GetLineReaders())
                foreach (var value in line)
                    Assert.AreEqual(expected, value);
        }

        [Test]
        public void LineReaderTest()
        {
            var builder = new CylinderNoiseMapBuilder();
            builder.DestNoiseMap = new NoiseMap();
            builder.SourceModule = new Perlin();
            builder.SetBounds(0, 180, 0, 5);
            builder.SetDestSize(12, 6);
            builder.Build();

            int row = 0, x = 0;
            foreach (var line in builder.DestNoiseMap.GetLineReaders())
            {
                foreach (var value in line)
                {
                    Assert.AreEqual(builder.DestNoiseMap.GetValue(x, row), value);
                    x++;
                }
                row++;
                x = 0;
            }
        }
    }
}
