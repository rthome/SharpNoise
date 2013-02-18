using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;
using System.IO;

namespace SharpNoise.Serialization.Tests
{
    [TestClass]
    public class XmlSerializerTests
    {
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
    }
}
