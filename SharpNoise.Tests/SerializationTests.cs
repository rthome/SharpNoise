using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Serialization;
using SharpNoise.Modules;
using System.Collections.Generic;

namespace SharpNoise.Tests
{
    [TestClass]
    public class SerializationTests
    {
        class TestSerializer : Serializer
        {
            protected override void Serialize(System.IO.Stream targetStream, Modules.Module module)
            {
                throw new NotImplementedException();
            }

            protected override Modules.Module Deserialize(System.IO.Stream sourceStream)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Module> GetModulesRecursiveAccessor(Module root)
            {
                return GetModulesRecursive(root);
            }
        }

        TestSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            serializer = new TestSerializer();
        }

        [TestMethod]
        public void Serialization_GetModulesRecursive_Test()
        {
            var source00 = new Constant();
            var source01 = new Billow();
            var source0 = new Perlin();
            var source1 = new Add { Source0 = source00, Source1 = source01 };
            var root = new Power { Source0 = source0, Source1 = source1 };

            var items = new List<Module>(serializer.GetModulesRecursiveAccessor(root));

            var expected = new Module[] { source00, source01, source0, source1, root };
            CollectionAssert.AreEquivalent(expected, items);
        }

        [TestMethod]
        public void Serialization_GetModulesRecursive_NoModule_Test()
        {
            var source0 = new Perlin();
            var source1 = new Add { Source0 = null, Source1 = null };
            var root = new Power { Source0 = source0, Source1 = source1 };

            var items = new List<Module>(serializer.GetModulesRecursiveAccessor(root));

            var expected = new Module[] { source0, source1, root };
            CollectionAssert.AreEquivalent(expected, items);
        }

        //[TestMethod]
        //public void Serialization_GetModulesRecursive_Cyclic_Test()
        //{
        //    var source0 = new Constant();
        //    var add = new Add { Source0 = source0 };
        //    var add2 = new Add { Source0 = source0 };

        //    add.Source1 = add2;
        //    add2.Source1 = add;

        //    var items = new List<Module>(serializer.GetModulesRecursiveAccessor(add2));

        //    var expected = new Module[] { source0, add, add2 };
        //    CollectionAssert.AreEquivalent(expected, items);
        //}

        [TestMethod]
        public void Serialization_GetModulesRecursive_NullArgument_Test()
        {
            CollectionAssert.AreEqual(new List<Module>(), new List<Module>(serializer.GetModulesRecursiveAccessor(null)));
        }
    }
}
