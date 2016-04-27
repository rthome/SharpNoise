using SharpNoise.Modules;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    /// <summary>
    /// Tests for the <see cref="Add"/> module
    /// </summary>
    public class AddTests
    {
        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(50)]
        public void AddTest(double data)
        {
            var source = new Constant { ConstantValue = data };
            var add = new Add { Source0 = source, Source1 = source };

            Assert.Equal(2 * data, add.GetValue(0, 0, 0));
        }
    }
}
