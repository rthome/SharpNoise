using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;
using SharpNoise.Builders;
using System.IO;

namespace SharpNoise.Tests
{
    /// <summary>
    /// A port of the granite texture sample of the original libNoise.
    /// Several modules, models, and noise map builders are being tested
    /// </summary>
    [TestClass]
    public class GraniteTextureTest
    {
        private static Module testModule;
        
        private ImageRenderer testRenderer;
        private Image testTextureImage;

        [ClassInitialize]
        public static void CreateTestModuleTree(TestContext context)
        {
            var primaryGranite = new Billow
            {
                Seed = 0,
                Frequency = 8,
                Persistence = 0.625,
                Lacunarity = 2.18359375,
                OctaveCount = 6,
                Quality = NoiseQuality.Standard,
            };

            var baseGrains = new Voronoi
            {
                Seed = 1,
                Frequency = 16,
                EnableDistance = true,
            };

            var scaledGrains = new ScaleBias
            {
                Source0 = baseGrains,
                Scale = -0.5,
                Bias = 0,
            };

            var combinedGranite = new Add
            {
                Source0 = primaryGranite,
                Source1 = scaledGrains,
            };

            var finalGranite = new Turbulence
            {
                Source0 = combinedGranite,
                Seed = 2,
                Frequency = 4,
                Power = 1.0 / 8.0,
                Roughness = 6,
            };

            testModule = finalGranite;
        }

        [TestInitialize]
        public void CreateTestImageRenderer()
        {
            testTextureImage = new Image();

            testRenderer = new ImageRenderer();
            testRenderer.ClearGradient();
            testRenderer.AddGradientPoint(-1.0000, new Color(0, 0, 0, 255));
            testRenderer.AddGradientPoint(-0.9375, new Color(0, 0, 0, 255));
            testRenderer.AddGradientPoint(-0.8750, new Color(216, 216, 242, 255));
            testRenderer.AddGradientPoint(0.0000, new Color(191, 191, 191, 255));
            testRenderer.AddGradientPoint(0.5000, new Color(210, 116, 125, 255));
            testRenderer.AddGradientPoint(0.7500, new Color(210, 113, 98, 255));
            testRenderer.AddGradientPoint(1.0000, new Color(255, 176, 192, 255));
            testRenderer.EnableLight = true;
            testRenderer.LightAzimuth = 135;
            testRenderer.LightElevation = 60;
            testRenderer.LightContrast = 2;
            testRenderer.LightColor = new Color(255, 255, 255, 0);
        }

        [Ignore]
        [TestMethod]
        public void GraniteTexture_PlanarRender()
        {
            var noiseMap = new NoiseMap();
            var plane = new PlaneNoiseMapBuilder();
            plane.SetBounds(-1, 1, -1, 1);
            plane.SetDestSize(256, 256);
            plane.SourceModule = testModule;
            plane.DestNoiseMap = noiseMap;
            plane.EnableSeamless = true;
            plane.Build();

            testRenderer.SourceNoiseMap = noiseMap;
            testRenderer.DestinationImage = testTextureImage;
            testRenderer.Render();

            Assert.Fail();
        }

        [Ignore]
        [TestMethod]
        public void GraniteTexture_SphericalRender()
        {

        }
    }
}
