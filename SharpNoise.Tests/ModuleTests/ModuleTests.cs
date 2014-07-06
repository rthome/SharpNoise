using Microsoft.VisualStudio.TestTools.UnitTesting;

using SharpNoise.Modules;
using System;

namespace SharpNoise.Tests.ModuleTests
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
    }
}
