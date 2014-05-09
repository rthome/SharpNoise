using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;

namespace SharpNoise.Tests.ModuleTests
{
    [TestClass]
    public class MinMaxTests
    {
        [TestMethod]
        public void MaxTest()
        {
            var min = 0D;
            var max = 1D;

            var source0 = new Constant() { ConstantValue = min };
            var source1 = new Constant() { ConstantValue = max };
            var module = new Max() { Source0 = source0, Source1 = source1 };

            Assert.AreEqual(max, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void MinTest()
        {
            var min = 0D;
            var max = 1D;

            var source0 = new Constant() { ConstantValue = min };
            var source1 = new Constant() { ConstantValue = max };
            var module = new Min() { Source0 = source0, Source1 = source1 };

            Assert.AreEqual(min, module.GetValue(0, 0, 0));
        }
    }
}
