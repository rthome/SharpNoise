using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;

namespace SharpNoise.Tests.ModuleTests
{
    [TestClass]
    public class MiscTests
    {
        [TestMethod]
        public void AbsTest()
        {
            var source = new Constant() { ConstantValue = -1 };
            var module = new Abs() { Source0 = source };

            Assert.AreEqual(Math.Abs(source.GetValue(0, 0, 0)), module.GetValue(0, 0, 0));
        }
    }
}
