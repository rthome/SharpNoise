using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;
using Xunit;

namespace SharpNoise.Tests
{
    public class GraniteTextureTest
    {
        readonly Module module;
        readonly NoiseMap noiseMap;
        readonly ImageRenderer renderer;
        readonly Image textureImage;

        public GraniteTextureTest()
        {
            module = new Turbulence
            {
                Seed = 2,
                Frequency = 4,
                Power = 1.0 / 8.0,
                Roughness = 6,
                Source0 = new Add
                {
                    Source0 = new Billow
                    {
                        Seed = 1,
                        Frequency = 8,
                        Persistence = 0.625,
                        Lacunarity = 2.18359375,
                        OctaveCount = 6,
                    },
                    Source1 = new ScaleBias
                    {
                        Scale = -0.5,
                        Bias = 0,
                        Source0 = new Cell
                        {
                            Seed = 1,
                            Frequency = 16,
                            EnableDistance = true,
                        }
                    }
                }
            };

            noiseMap = new NoiseMap();

            textureImage = new Image();

            renderer = new ImageRenderer
            {
                EnableLight = true,
                LightAzimuth = 135,
                LightElevation = 60,
                LightContrast = 2,
                LightColor = new Color(255, 255, 255, 0),
            };
            renderer.AddGradientPoint(-1.0000, new Color(0, 0, 0, 255));
            renderer.AddGradientPoint(-0.9375, new Color(0, 0, 0, 255));
            renderer.AddGradientPoint(-0.8750, new Color(216, 216, 242, 255));
            renderer.AddGradientPoint(0.0000, new Color(191, 191, 191, 255));
            renderer.AddGradientPoint(0.5000, new Color(210, 116, 125, 255));
            renderer.AddGradientPoint(0.7500, new Color(210, 113, 98, 255));
            renderer.AddGradientPoint(1.0000, new Color(255, 176, 192, 255));
        }

        [Fact]
        public void PlanarRenderTest()
        {
            var plane = new PlaneNoiseMapBuilder();
            plane.SetBounds(-1, 1, -1, 1);
            plane.SetDestSize(256, 256);
            plane.SourceModule = module;
            plane.DestNoiseMap = noiseMap;
            plane.EnableSeamless = true;
            plane.Build();

            renderer.SourceNoiseMap = noiseMap;
            renderer.DestinationImage = textureImage;
            renderer.Render();

            // TODO: Add some asserts
        }

        [Fact]
        public void SphericalRenderTest()
        {
            var sphere = new SphereNoiseMapBuilder();
            sphere.SetBounds(-90, 90, -180, 180);
            sphere.SetDestSize(512, 256);
            sphere.SourceModule = module;
            sphere.DestNoiseMap = noiseMap;
            sphere.Build();

            renderer.SourceNoiseMap = noiseMap;
            renderer.DestinationImage = textureImage;
            renderer.Render();

            // TODO: Add some asserts
        }
    }
}
