using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;

namespace SharpNoise.Tests.ModuleTests
{
    [TestClass]
    public class ClampTests
    {
        [TestMethod]
        public void ClampTest1()
        {
            var max = 10D;
            var min = 0D;
            var source = new Constant() { ConstantValue = 25D };

            var module = new Clamp() { LowerBound = min, UpperBound = max, Source0 = source };

            Assert.AreEqual(max, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void ClampTest2()
        {
            var max = 10D;
            var min = 0D;
            var source = new Constant() { ConstantValue = -1D };

            var module = new Clamp() { LowerBound = min, UpperBound = max, Source0 = source };

            Assert.AreEqual(min, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void ClampTest3()
        {
            var max = 10D;
            var min = 0D;
            var source = new Constant() { ConstantValue = 5D };

            var module = new Clamp() { LowerBound = min, UpperBound = max, Source0 = source };

            Assert.AreEqual(source.GetValue(0, 0, 0), module.GetValue(0, 0, 0));
        }
    }
}
