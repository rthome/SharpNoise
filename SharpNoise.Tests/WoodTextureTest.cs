using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpNoise.Builders;
using SharpNoise.Utilities.Imaging;
using SharpNoise.Modules;
using System.IO;

namespace SharpNoise.Tests
{
    [TestClass]
    public class WoodTextureTest
    {
        private static Module testModule;

        private ImageRenderer testRenderer;
        private Image testTextureImage;

        [ClassInitialize]
        public static void CreateTestModuleTree(TestContext context)
        {
            var baseWood = new Cylinders
            {
                Frequency = 16,
            };

            var woodGrainNoise = new Perlin
            {
                Seed = 2135,
                Frequency = 48,
                Persistence = 0.5,
                Lacunarity = 2.20703125,
                OctaveCount = 3,
                Quality = NoiseQuality.Standard,
            };

            var scaledBaseWoodGrain = new ScalePoint
            {
                Source0 = woodGrainNoise,
                YScale = 0.25,
            };

            var woodGrain = new ScaleBias
            {
                Source0 = scaledBaseWoodGrain,
                Scale = 0.25,
                Bias = 0.125,
            };

            var combinedWood = new Add
            {
                Source0 = baseWood,
                Source1 = woodGrain,
            };

            var perturbedWood = new Turbulence
            {
                Source0 = combinedWood,
                Seed = 1,
                Frequency = 4,
                Power = 1.0 / 256.0,
                Roughness = 4,
            };

            var translatedWood = new TranslatePoint
            {
                Source0 = perturbedWood,
                ZTranslation = 1.48,
            };

            var rotatedWood = new RotatePoint
            {
                Source0 = translatedWood,
            };
            rotatedWood.SetAngles(84, 0, 0);

            var finalWood = new Turbulence
            {
                Source0 = rotatedWood,
                Seed = 2,
                Frequency = 2,
                Power = 1.0 / 64.0,
                Roughness = 4,
            };

            testModule = finalWood;
        }

        [TestInitialize]
        public void CreateTestImageRenderer()
        {
            testTextureImage = new Image();

            testRenderer = new ImageRenderer();
            testRenderer.ClearGradient();
            testRenderer.AddGradientPoint(-1.00, new Color(189, 94, 4, 255));
            testRenderer.AddGradientPoint(0.50, new Color(144, 48, 6, 255));
            testRenderer.AddGradientPoint(1.00, new Color(60, 10, 8, 255));

            testRenderer.EnableLight = false;
        }

        [TestMethod]
        public void WoodTexture_PlanarRender()
        {
            var noiseMap = new NoiseMap();
            var plane = new PlaneNoiseMapBuilder();
            plane.SetBounds(-1, 1, -1, 1);
            plane.SetDestSize(256, 256);
            plane.SourceModule = testModule;
            plane.DestNoiseMap = noiseMap;
            plane.EnableSeamless = false;
            plane.Build();

            testRenderer.SourceNoiseMap = noiseMap;
            testRenderer.DestinationImage = testTextureImage;
            testRenderer.Render();
        }

        [TestMethod]
        public void WoodTexture_SphericalRender()
        {
            var noiseMap = new NoiseMap();
            var sphere = new SphereNoiseMapBuilder();
            sphere.SetBounds(-90, 90, -180, 180);
            sphere.SetDestSize(512, 256);
            sphere.SourceModule = testModule;
            sphere.DestNoiseMap = noiseMap;
            sphere.Build();

            testRenderer.SourceNoiseMap = noiseMap;
            testRenderer.DestinationImage = testTextureImage;
            testRenderer.Render();
        }
    }
}
