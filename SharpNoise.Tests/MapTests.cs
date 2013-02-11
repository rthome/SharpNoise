using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities;

namespace SharpNoise.Tests
{
    [TestClass]
    class MapTests
    {
        NoiseMap map;

        [TestInitialize]
        public void SetUp()
        {
            map = new NoiseMap(2, 2);
        }

        [TestMethod]
        public void GetSetValueTest()
        {
            var expected = 99f;
            map.SetValue(1, 1, expected);

            var actual = map.GetValue(1, 1);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
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

        [TestMethod]
        public void ClearTest()
        {
            var expected = 5f;
            map.Clear(expected);
            foreach (var line in map.GetLineReaders())
                foreach (var value in line)
                    Assert.AreEqual(expected, value);
        }

        [TestMethod]
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
