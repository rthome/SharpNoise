using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;

namespace SharpNoise.Tests
{
    [TestClass]
    public class ModuleSerializationTests
    {
        [TestMethod]
        public void SerializeAllModules()
        {
            var source0 = new Constant() { Value = 5D };
            var source1 = new Add() { Source0 = source0, Source1 = source0 };
            var source2 = new Multiply() { Source0 = source0, Source1 = source1 };

            // create modules with custom sources and parameters
            var modules = new Module[]
            {
                new Abs() { Source0=source0},
                new Add() { Source0=source0, Source1=source1},
                new Billow() { Quality= NoiseQuality.Fast, Lacunarity=Billow.DefaultLacunarity*2},
                new Blend(){ Control=new Constant(){ Value=0.5D}, Source0=source0, Source1=source1},
                new Cache() { Source0=source2},
                new Checkerboard(),
                new Clamp() { LowerBound=0D, UpperBound=5D, Source0=source0},
                new Constant() { Value=9D},
                new Cylinder() { Frequency=Cylinder.DefaultFrequency*2},
                new Displace() { Source0=source0, XDisplace=source0, YDisplace=source1, ZDisplace=source2},
                new Exponent(){ Exp=Exponent.DefaultExponent*3, Source0=source0},
                new Invert() {  Source0=source0},
                new Max() { Source0=source0, Source1=source1},
                new Min() {Source0=source0, Source1=source1},
                new Multiply(){ Source0=source0, Source1=source1},
                new Perlin() { Frequency=Perlin.DefaultFrequency*2, Quality=NoiseQuality.Best},
                new Power(){ Source0=source1, Source1=source0},
                new RidgedMulti(){ Frequency=RidgedMulti.DefaultFrequency*2},
                new RotatePoint(){ Source0=source0, XAngle=1, YAngle=2, ZAngle=3},
                new ScaleBias() { Bias=0.7, Scale=2, Source0=source0},
                new ScalePoint(){ Source0=source0, XScale=2, YScale=3, ZScale=4},
                new Select() { LowerBound=0, UpperBound=2, Control=source0, Source0=source1, Source1=source2},
                new Spheres() { Frequency=Spheres.DefaultFrequency*2},
                new TranslatePoint() { Source0=source0, XTranslation=2, YTranslation=3, ZTranslation=4},
                new Turbulence() { Source0=source0, Frequency=Turbulence.DefaultFrequency*2},
                new Voronoi(){ Displacement=Voronoi.DefaultDisplacement*2},
            };

            var serializedModules = new byte[modules.Length][];
            var deserializedModules = new Module[modules.Length];
            var formatter = new BinaryFormatter();

            for (var i = 0; i < modules.Length; i++)
            {
                using (var ms = new MemoryStream())
                {
                    modules[i].Serialize(ms, formatter);
                    serializedModules[i] = ms.ToArray();
                }
            }

            for (var i = 0; i < serializedModules.Length; i++)
            {
                using (var ms = new MemoryStream(serializedModules[i]))
                {
                    deserializedModules[i] = Module.Deserialize<Module>(ms, formatter);
                }
            }

            for (var i = 0; i < modules.Length; i++)
            {
                Assert.AreEqual(modules[i].GetValue(0, 0, 0), deserializedModules[i].GetValue(0, 0, 0));
                Assert.AreEqual(modules[i].GetValue(1, 2, 3), deserializedModules[i].GetValue(1, 2, 3));
            }
        }

        [TestMethod]
        public void ConstantSerializeTest()
        {
            var module = new Constant() { Value = 5D };
            var formatter = new BinaryFormatter();

            byte[] data;
            using (var ms = new MemoryStream())
            {
                module.Serialize(ms);
                data = ms.ToArray();
            }

            Constant deserializedModule;
            using (var ms = new MemoryStream(data))
                deserializedModule = Constant.Deserialize<Constant>(ms, formatter);

            Assert.AreEqual(module.Value, deserializedModule.Value);
        }
    }
}
