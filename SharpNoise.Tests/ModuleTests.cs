using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpNoise.Modules;
using System;

namespace SharpNoise.Tests
{
    [TestClass]
    public class ModuleTests
    {
        [TestMethod]
        public void GetSourceModuleTest()
        {
            var source0 = new Constant();
            var source1 = new Constant();
            var control = new Constant();

            var module = new Select() { Source0 = source0, Source1 = source1, Control = control };

            Assert.AreSame(source0, module.Source0);
            Assert.AreSame(source1, module.Source1);
            Assert.AreSame(control, module.Control);

            Assert.AreSame(module.Source0, module.GetSourceModule(0));
            Assert.AreSame(module.Source1, module.GetSourceModule(1));
            Assert.AreSame(module.Control, module.GetSourceModule(2));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialization_GetModulesRecursive_CyclicSelf_Test()
        {
            var add = new Add();
            add.Source0 = add;
        }

        [TestMethod]
        [ExpectedException(typeof(NoModuleException))]
        public void InvalidGetSourceModule_NoModule_Test()
        {
            var module = new Abs();
            module.GetSourceModule(0);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void InvalidGetSourceModule_HighIndex_Test()
        {
            var module = new Abs();
            module.GetSourceModule(15);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void InvalidGetSourceModule_NegativeIndex_Test()
        {
            var module = new Abs();
            module.GetSourceModule(-1);
        }

        [TestMethod]
        public void AbsTest()
        {
            var source = new Constant() { Value = -1 };
            var module = new Abs() { Source0 = source };

            Assert.AreEqual(Math.Abs(source.GetValue(0, 0, 0)), module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void AddTest()
        {
            var source = new Constant() { Value = 5D };
            var adder = new Add() { Source0 = source, Source1 = source };

            Assert.AreEqual(2D * source.GetValue(0, 0, 0), adder.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void MaxTest()
        {
            var min = 0D;
            var max = 1D;

            var source0 = new Constant() { Value = min };
            var source1 = new Constant() { Value = max };
            var module = new Max() { Source0 = source0, Source1 = source1 };

            Assert.AreEqual(max, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void MinTest()
        {
            var min = 0D;
            var max = 1D;

            var source0 = new Constant() { Value = min };
            var source1 = new Constant() { Value = max };
            var module = new Min() { Source0 = source0, Source1 = source1 };

            Assert.AreEqual(min, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void MultiplyTest1()
        {
            var value = 3D;
            var source = new Constant() { Value = value };
            var module = new Multiply() { Source0 = source, Source1 = source };

            Assert.AreEqual(value * value, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void MultiplyTest2()
        {
            var value1 = 3D;
            var value2 = 5D;
            var source1 = new Constant() { Value = value1 };
            var source2 = new Constant() { Value = value2 };
            var module = new Multiply() { Source0 = source1, Source1 = source2 };

            Assert.AreEqual(value1 * value2, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void ScalePointTest()
        {
            double x = 1D,
                   y = 2D,
                   z = 3D;
            double scaleX = 1D,
                   scaleY = 2D,
                   scaleZ = 3D;

            var source = new Perlin();
            var module = new ScalePoint() { Source0 = source, XScale = scaleX, YScale = scaleY, ZScale = scaleZ };

            Assert.AreEqual(source.GetValue(x * scaleX, y * scaleY, z * scaleZ), module.GetValue(x, y, z));
        }

        [TestMethod]
        public void ClampTest1()
        {
            var max = 10D;
            var min = 0D;
            var source = new Constant() { Value = 25D };

            var module = new Clamp() { LowerBound = min, UpperBound = max, Source0 = source };

            Assert.AreEqual(max, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void ClampTest2()
        {
            var max = 10D;
            var min = 0D;
            var source = new Constant() { Value = -1D };

            var module = new Clamp() { LowerBound = min, UpperBound = max, Source0 = source };

            Assert.AreEqual(min, module.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void ClampTest3()
        {
            var max = 10D;
            var min = 0D;
            var source = new Constant() { Value = 5D };

            var module = new Clamp() { LowerBound = min, UpperBound = max, Source0 = source };

            Assert.AreEqual(source.GetValue(0, 0, 0), module.GetValue(0, 0, 0));
        }
    }
}
