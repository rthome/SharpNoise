using System;
using System.IO;
using System.Drawing.Imaging;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using SharpNoise.Utilities.Imaging;

namespace BeginnerSample
{
	class Program
	{
		public static void Main(string[] args)
		{
			// The noise source - a simple Perlin noise generator will do for this sample
			var noiseSource = new Perlin
			{
				Seed = new Random().Next()
			};
				
			// Create a new, empty, noise map and initialize a new planar noise map builder with it
			var noiseMap = new NoiseMap();
			var noiseMapBuilder = new PlaneNoiseMapBuilder
			{
				DestNoiseMap = noiseMap,
				SourceModule = noiseSource
			};

			// Set the size of the noise map
			noiseMapBuilder.SetDestSize(1280, 720);

			// Set the bounds of the noise mpa builder
			// These are the coordinates in the noise source from which noise values will be sampled
			noiseMapBuilder.SetBounds(-3, 3, -2, 2);

			// Build the noise map - samples values from the noise module above,
			// using the bounds coordinates we have passed in the builder
			noiseMapBuilder.Build();

			// Create a new image and image renderer
			var image = new Image();
			var renderer = new ImageRenderer
			{
				SourceNoiseMap = noiseMap,
				DestinationImage = image
			};

			// The renderer needs to know how to map noise values to colors.
			// In this case, we use one of the predefined gradients, specifically the terrain gradient, 
			// which maps lower noise values to blues and greens and higher values to brouns and whites.
			// This simulates the look of a map with water, grass and vegetation, dirt and mountains.
			renderer.BuildTerrainGradient();

			// Before rendering the image, we could set various parameters on the renderer,
			// such as the position and color of the light source.
			// But we aren't going to bother for this sample.

			// Finally, render the image
			renderer.Render();

			// Finally, save the rendered image as a PNG in the current directory
			using (var fs = File.OpenWrite("NoiseMap.png"))
			{
				image.SaveGdiBitmap(fs, ImageFormat.Png);
			}
		}
	}
}
