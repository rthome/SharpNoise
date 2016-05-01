using SharpNoise.Modules;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    /// <summary>
    /// Tests for the <see cref="Constant"/> module
    /// </summary>
    public class ConstantTests
    {
        public static IEnumerable<object[]> ConstantDataSource => Enumerable.Range(-5, 11).Select(i => i / 10.0).Select(d => new object[] { d });

        [Theory]
        [MemberData("ConstantDataSource")]
        public void TestValues(double data)
        {
            var constant = new Constant { ConstantValue = data };
            Assert.Equal(data, constant.ConstantValue);
            Assert.Equal(data, constant.GetValue(0, 0, 0));
        }

        [Theory]
        [MemberData("ConstantDataSource")]
        public void TestSettingValues(double data)
        {
            var constant = new Constant { ConstantValue = data };
            Assert.Equal(data, constant.GetValue(0, 0, 0));
            constant.ConstantValue += data;
            Assert.Equal(data * 2, constant.GetValue(0, 0, 0));
        }
    }
}
