using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;
using System;
using System.Diagnostics;
using System.IO;

namespace SharpNoise.Serialization.Tests
{
    [TestClass]
    public class XmlSerialization_ComplexTests
    {
        XmlSerializer serializer;

        [TestInitialize]
        public void SetUp()
        {
            serializer = new XmlSerializer();
        }

        [TestMethod]
        public void Serialization_Xml_SelfReferencing_Test()
        {
            var source0 = new Constant { Value = 1 };
            var source1 = new Constant { Value = 2 };
            var add = new Add { Source0 = source0, Source1 = source1 };

            var root = new Add { Source0 = add, Source1 = add };

            // check if it's actually correct on the original module
            Debug.Assert(object.ReferenceEquals(root.Source0, root.Source1));

            Module restoredModule;
            using (var ms = new MemoryStream())
            {
                serializer.Save(root, ms);
                restoredModule = serializer.Restore(ms);
            }

            Assert.AreEqual(root.GetValue(0, 0, 0), restoredModule.GetValue(0, 0, 0));
            Assert.AreSame(restoredModule.GetSourceModule(0), restoredModule.GetSourceModule(1));
        }

        [TestMethod]
        public void Serialization_Xml_LargerGraph_Test()
        {
            var source0 = new Constant { Value = -1 };
            var source1 = new Constant { Value = 2 };
            var multiply = new Multiply { Source0 = source0, Source1 = source1 };
            var abs = new Abs { Source0 = multiply };

            var perlin = new Perlin();
            var billow = new Billow();
            var max = new Max { Source0 = perlin, Source1 = billow };

            var root = new Add { Source0 = abs, Source1 = max };

            Module restoredModule;
            using (var ms = new MemoryStream())
            {
                serializer.Save(root, ms);
                restoredModule = serializer.Restore(ms);
            }

            Assert.AreEqual(root.GetType(), restoredModule.GetType());
            for (double x = 0; x < 2; x += 0.25)
            {
                for (double y = 0; y < 2; y += 0.25)
                {
                    for (double z = 0; z < 2; z += 0.25)
                    {
                        var expectedValue =root.GetValue(x, y, z);
                        var actualValue = restoredModule.GetValue(x, y, z);
                        var difference = Math.Abs(expectedValue - actualValue);
                        Assert.IsTrue(difference <= double.Epsilon);
                    }
                }
            }
        }
    }
}
