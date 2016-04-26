using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;
using Xunit;

namespace SharpNoise.Tests
{
    public class WoodTextureTest
    {
        readonly Module module;
        readonly NoiseMap noiseMap;
        readonly ImageRenderer renderer;
        readonly Image textureImage;

        public WoodTextureTest()
        {
            module = new Turbulence
            {
                Seed = 2,
                Frequency = 2,
                Power = 1.0 / 64.0,
                Roughness = 4,
                Source0 = new RotatePoint
                {
                    XAngle = 84,
                    Source0 = new TranslatePoint
                    {
                        ZTranslation = 1.48,
                        Source0 = new Turbulence
                        {
                            Seed = 1,
                            Frequency = 4,
                            Power = 1.0 / 256.0,
                            Roughness = 4,
                            Source0 = new Add
                            {
                                Source0 = new Cylinders
                                {
                                    Frequency = 16,
                                },
                                Source1 = new ScaleBias
                                {
                                    Scale = 0.25,
                                    Bias = 0.125,
                                    Source0 = new ScalePoint
                                    {
                                        YScale = 0.25,
                                        Source0 = new Perlin
                                        {
                                            Seed = 2135,
                                            Frequency = 48,
                                            Persistence = 0.5,
                                            Lacunarity = 2.20703125,
                                            OctaveCount = 3,
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            noiseMap = new NoiseMap();

            textureImage = new Image();

            renderer = new ImageRenderer
            {
                EnableLight = false,
            };
            renderer.AddGradientPoint(-1.00, new Color(189, 94, 4, 255));
            renderer.AddGradientPoint(0.50, new Color(144, 48, 6, 255));
            renderer.AddGradientPoint(1.00, new Color(60, 10, 8, 255));
        }

        [Fact]
        public void PlanarRenderTest()
        {
            var plane = new PlaneNoiseMapBuilder();
            plane.SetBounds(-1, 1, -1, 1);
            plane.SetDestSize(256, 256);
            plane.SourceModule = module;
            plane.DestNoiseMap = noiseMap;
            plane.EnableSeamless = false;
            plane.Build();

            renderer.SourceNoiseMap = noiseMap;
            renderer.DestinationImage = textureImage;
            renderer.Render();
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
        }
    }
}
