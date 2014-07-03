using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;

namespace SharpNoise.Tests.ModuleTests
{
    [TestClass]
    public class ScaleTests
    {
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
    }
}
