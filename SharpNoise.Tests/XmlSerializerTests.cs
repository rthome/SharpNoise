using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;
using SharpNoise.Serialization;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace SharpNoise.Tests
{
    [TestClass]
    public class XmlSerializerTests
    {
        #region custom module class

        class CustomModule : Module
        {
            public const double DefaultValue = 5;

            public double CustomValue { get; set; }

            public override double GetValue(double x, double y, double z)
            {
                return CustomValue;
            }

            public CustomModule()
                : base(0)
            {
                CustomValue = DefaultValue;
            }
        }

        #endregion

        XmlSerializer serializer;

        Add root;
        Constant source0, source1;

        public static MemoryStream SwitchStream(MemoryStream stream)
        {
            var bytes = stream.ToArray();
            return new MemoryStream(bytes);
        }

        [TestInitialize]
        public void SetUp()
        {
            serializer = new XmlSerializer();

            source0 = new Constant { Value = 1 };
            source1 = new Constant { Value = 2 };
            root = new Add { Source0 = source0, Source1 = source1 };
        }

        [TestMethod]
        public void Serialization_Xml_Restore_ValuesEqual_Test()
        {
            var saveStream = new MemoryStream();
            serializer.Save(root, saveStream);

            var restoreStream = SwitchStream(saveStream);
            var restoredModule = serializer.Restore<Add>(restoreStream);

            saveStream.Close();
            restoreStream.Close();

            Assert.AreEqual(root.GetValue(0, 0, 0), restoredModule.GetValue(0, 0, 0));
            Assert.AreEqual(root.Source0.GetValue(0, 0, 0), restoredModule.Source0.GetValue(0, 0, 0));
            Assert.AreEqual(root.Source1.GetValue(0, 0, 0), restoredModule.Source1.GetValue(0, 0, 0));
        }

        [TestMethod]
        public void Serialization_Xml_Restore_TypesEqual_Test()
        {
            var saveStream = new MemoryStream();
            serializer.Save(root, saveStream);

            var restoreStream = SwitchStream(saveStream);
            var restoredModule = serializer.Restore(restoreStream);

            saveStream.Close();
            restoreStream.Close();

            Assert.IsInstanceOfType(restoredModule, root.GetType());
            Assert.IsInstanceOfType(restoredModule.GetSourceModule(0), source0.GetType());
            Assert.IsInstanceOfType(restoredModule.GetSourceModule(1), source1.GetType());
        }

        [TestMethod]
        public void Serialization_Terrace_DefaultControlPoints_Test()
        {
            var terrace = new Terrace();
            terrace.MakeControlPoints(4);

            var saveStream = new MemoryStream();
            serializer.Save(terrace, saveStream);

            var restoreStream = SwitchStream(saveStream);
            var restoredModule = serializer.Restore<Terrace>(restoreStream);

            saveStream.Close();
            restoreStream.Close();

            Assert.AreEqual(terrace.ControlPointCount, restoredModule.ControlPointCount);
            CollectionAssert.AreEquivalent(terrace.ControlPoints, restoredModule.ControlPoints);
        }

        [TestMethod]
        public void Serialization_Curve_ControlPoints_Test()
        {
            var curve = new Curve();
            curve.AddControlPoint(-1, 1);
            curve.AddControlPoint(-0.75, 0.5);
            curve.AddControlPoint(0, 0);
            curve.AddControlPoint(0.75, -0.5);
            curve.AddControlPoint(1, -1);

            var saveStream = new MemoryStream();
            serializer.Save(curve, saveStream);

            var restoreStream = SwitchStream(saveStream);
            var restoredModule = serializer.Restore<Terrace>(restoreStream);

            saveStream.Close();
            restoreStream.Close();

            Assert.AreEqual(curve.ControlPointCount, restoredModule.ControlPointCount);
            CollectionAssert.AreEquivalent(curve.ControlPoints, restoredModule.ControlPoints);
        }

        [TestMethod]
        public void Serialization_CustomModule_TypeEqual_Test()
        {
            serializer.AddCustomModuleTypes(new[] { typeof(CustomModule) });

            var source0 = new Constant { Value = 1 };
            var source1 = new CustomModule { CustomValue = 2 };
            var root = new Add { Source0 = source0, Source1 = source1 };

            var saveStream = new MemoryStream();
            serializer.Save(root, saveStream);

            var restoreStream = SwitchStream(saveStream);
            var restoredModule = serializer.Restore(restoreStream);

            saveStream.Close();
            restoreStream.Close();

            Assert.IsInstanceOfType(restoredModule, root.GetType());
            Assert.IsInstanceOfType(restoredModule.GetSourceModule(0), source0.GetType());
            Assert.IsInstanceOfType(restoredModule.GetSourceModule(1), source1.GetType());
        }

        [TestMethod]
        public void Serialization_CustomModule_ValueEqual_Test()
        {
            serializer.AddCustomModuleTypes(new[] { typeof(CustomModule) });

            var source0 = new Constant { Value = 1 };
            var source1 = new CustomModule { CustomValue = 2 };
            var root = new Add { Source0 = source0, Source1 = source1 };

            var saveStream = new MemoryStream();
            serializer.Save(root, saveStream);

            var restoreStream = SwitchStream(saveStream);
            var restoredModule = serializer.Restore<Add>(restoreStream);

            saveStream.Close();
            restoreStream.Close();

            Assert.AreEqual(root.GetValue(0, 0, 0), restoredModule.GetValue(0, 0, 0));
            Assert.AreEqual(root.Source0.GetValue(0, 0, 0), restoredModule.Source0.GetValue(0, 0, 0));
            Assert.AreEqual(root.Source1.GetValue(0, 0, 0), restoredModule.Source1.GetValue(0, 0, 0));
        }
    }
}
