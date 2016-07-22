using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SharpNoise.Tests.Modules
{
    public class ModuleTests
    {
        [Fact]
        public void GetSourceModuleTest()
        {
            var source0 = new Constant();
            var source1 = new Constant();
            var control = new Constant();

            var module = new Select() { Source0 = source0, Source1 = source1, Control = control };

            Assert.Same(source0, module.Source0);
            Assert.Same(source1, module.Source1);
            Assert.Same(control, module.Control);

            Assert.Same(module.Source0, module.SourceModules[0]);
            Assert.Same(module.Source1, module.SourceModules[1]);
            Assert.Same(module.Control, module.SourceModules[2]);
        }

        [Fact]
        public void NoModuleTest()
        {
            var module = new Abs();
            var source = module.SourceModules[0];

            Assert.Null(source);
        }

        [Fact]
        public void SourceIndexOutOfRangeTest()
        {
            var module = new Abs();
#pragma warning disable CS0251 // Indexing an array with a negative index
            Assert.Throws<IndexOutOfRangeException>(() => module.SourceModules[-1]);
#pragma warning restore CS0251 // Indexing an array with a negative index
            Assert.Throws<IndexOutOfRangeException>(() => module.SourceModules[module.SourceModuleCount]);
            Assert.Throws<IndexOutOfRangeException>(() => module.SourceModules[module.SourceModuleCount + 2]);
        }
    }
}
