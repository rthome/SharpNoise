using System;
using Xunit;

namespace SharpNoise.Tests
{
    /// <summary>
    /// Tests for <see cref="NoiseGenerator"/>
    /// </summary>
    public class NoiseGeneratorTests
    {
        public NoiseGeneratorTests()
        {
            NoiseGenerator.SetDefaultVectorTable();
        }

        [Fact]
        public void SetVectorTableTest()
        {
            var table = NoiseGenerator.GenerateRandomVectorTable(1);
            NoiseGenerator.SetVectorTable(table);

            Assert.Equal(table, NoiseGenerator.VectorTable);
        }
    }
}
