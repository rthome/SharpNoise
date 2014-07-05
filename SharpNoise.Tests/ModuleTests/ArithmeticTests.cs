using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;

namespace SharpNoise.Tests.ModuleTests
{
    [TestClass]
    public class ArithmeticTests
    {
        [TestMethod]
        public void AddTest()
        {
            var source = new Constant() { ConstantValue = 5D };
            var adder = new Add() { Source0 = source, Source1 = source };

            Assert.AreEqual(2D * source.GetValue(0, 0, 0), adder.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void MultiplyTest1()
        {
            var value = 3D;
            var source = new Constant() { ConstantValue = value };
            var module = new Multiply() { Source0 = source, Source1 = source };

            Assert.AreEqual(value * value, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void MultiplyTest2()
        {
            var value1 = 3D;
            var value2 = 5D;
            var source1 = new Constant() { ConstantValue = value1 };
            var source2 = new Constant() { ConstantValue = value2 };
            var module = new Multiply() { Source0 = source1, Source1 = source2 };

            Assert.AreEqual(value1 * value2, module.GetValue(0, 0, 0));
        }
    }
}
