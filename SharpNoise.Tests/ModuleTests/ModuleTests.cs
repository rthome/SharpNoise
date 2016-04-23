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

            Assert.AreSame(module.Source0, module.SourceModules[0]);
            Assert.AreSame(module.Source1, module.SourceModules[1]);
            Assert.AreSame(module.Control, module.SourceModules[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Serialization_GetModulesRecursive_CyclicSelf_Test()
        {
            var add = new Add();
            add.Source0 = add;
        }

        [TestMethod]
        public void InvalidGetSourceModule_NoModule_Test()
        {
            var module = new Abs();
            var source = module.SourceModules[0];

            Assert.IsNull(source);
        }

        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void InvalidGetSourceModule_HighIndex_Test()
        {
            var module = new Abs();
            var source = module.SourceModules[15];
        }
    }
}
