using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs three-dimensional "billowy" noise.
    /// </summary>
    /// <remarks>
    /// This noise module generates "billowy" noise suitable for clouds and
    /// rocks.
    ///
    /// This noise module is nearly identical to Perlin except
    /// this noise module modifies each octave with an absolute-value
    /// function.  See the documentation of Perlin for more
    /// information.
    /// <seealso cref="Perlin"/>
    /// </remarks>
    [Serializable]
    public class Billow : Module
    {
        /// <summary>
        /// Default frequency
        /// </summary>
        public const double DefaultFrequency = 1D;

        /// <summary>
        /// Default lacunarity
        /// </summary>
        public const double DefaultLacunarity = 2D;

        /// <summary>
        /// Default number of octaves
        /// </summary>
        public const int DefaultOctaveCount = 6;

        /// <summary>
        /// Default persistence value
        /// </summary>
        public const double DefaultPersistence = 0.5D;

        /// <summary>
        /// Default noise quality
        /// </summary>
        public const NoiseQuality DefaultQuality = NoiseQuality.Standard;

        /// <summary>
        /// Default noise seed
        /// </summary>
        public const int DefaultSeed = 0;

        /// <summary>
        /// Gets or sets the frequency of the first octave.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the lacunarity of the billowy noise.
        /// </summary>
        /// <remarks>
        /// The lacunarity is the frequency multiplier between successive
        /// octaves.
        /// </remarks>
        public double Lacunarity { get; set; }

        /// <summary>
        /// Gets or sets the quality of the billowy noise.
        /// </summary>
        /// <remarks>
        /// See <see cref="NoiseQuality"/> for definitions of the various
        /// coherent-noise qualities.
        /// </remarks>
        public NoiseQuality Quality { get; set; }

        /// <summary>
        /// Gets or sets the number of octaves that generate the billowy noise.
        /// </summary>
        /// <remarks>
        /// The number of octaves controls the amount of detail in the billowy
        /// noise.
        /// </remarks>
        public int OctaveCount { get; set; }

        /// <summary>
        /// Gets or sets the persistence value of the billowy noise.
        /// </summary>
        /// <remarks>
        /// The persistence value controls the roughness of the billowy noise.
        /// </remarks>
        public double Persistence { get; set; }

        /// <summary>
        /// Gets or sets the seed value used by the billowy-noise function.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Billow()
            : base(0)
        {
            Frequency = DefaultFrequency;
            Lacunarity = DefaultLacunarity;
            Quality = DefaultQuality;
            OctaveCount = DefaultOctaveCount;
            Persistence = DefaultPersistence;
            Seed = DefaultSeed;
        }

        /// <summary>
        /// See the documentation on the base class.
        /// <seealso cref="Module"/>
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <returns>Returns the computed value</returns>
        public override double GetValue(double x, double y, double z)
        {
            double value = 0.0;
            double signal = 0.0;
            double curPersistence = 1.0;
            int seed;

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (int curOctave = 0; curOctave < OctaveCount; curOctave++)
            {
                // Get the coherent-noise value from the input value and add it to the
                // final result.
                seed = (Seed + curOctave) & int.MaxValue;
                signal = NoiseGenerator.GradientCoherentNoise3D(x, y, z, seed, Quality);
                signal = 2.0 * Math.Abs(signal) - 1.0;
                value += signal * curPersistence;

                // Prepare the next octave.
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                curPersistence *= Persistence;
            }
            value += 0.5;

            return value;
        }
    }
}
