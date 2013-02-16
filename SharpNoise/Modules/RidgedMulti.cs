using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs 3-dimensional ridged-multifractal noise.
    /// </summary>
    /// <remarks>
    /// This noise module, heavily based on the Perlin-noise module, generates
    /// ridged-multifractal noise.  Ridged-multifractal noise is generated in
    /// much of the same way as Perlin noise, except the output of each octave
    /// is modified by an absolute-value function.  Modifying the octave
    /// values in this way produces ridge-like formations.
    ///
    /// Ridged-multifractal noise does not use a persistence value.  This is
    /// because the persistence values of the octaves are based on the values
    /// generated from from previous octaves, creating a feedback loop (or
    /// that's what it looks like after reading the code.)
    ///
    /// This noise module outputs ridged-multifractal-noise values that
    /// usually range from -1.0 to +1.0, but there are no guarantees that all
    /// output values will exist within that range.
    ///
    /// For ridged-multifractal noise generated with only one octave,
    /// the output value ranges from -1.0 to 0.0.
    ///
    /// Ridged-multifractal noise is often used to generate craggy mountainous
    /// terrain or marble-like textures.
    ///
    /// This noise module does not require any source modules.
    ///
    /// 
    /// Octaves
    ///
    /// The number of octaves control the amount of detail of the
    /// ridged-multifractal noise.  Adding more octaves increases the detail
    /// of the ridged-multifractal noise, but with the drawback of increasing
    /// the calculation time.
    ///
    /// An application may specify the number of octaves that generate
    /// ridged-multifractal noise by modifying the OctaveCount property.
    ///
    /// Frequency
    ///
    /// An application may specify the frequency of the first octave by
    /// modifying the Frequency property.
    ///
    /// Lacunarity
    ///
    /// The lacunarity specifies the frequency multipler between successive
    /// octaves.
    ///
    /// The effect of modifying the lacunarity is subtle; you may need to play
    /// with the lacunarity value to determine the effects.  For best results,
    /// set the lacunarity to a number between 1.5 and 3.5.
    ///
    /// References &amp; Acknowledgments
    ///
    /// http://www.texturingandmodeling.com/Musgrave.html
    /// F. Kenton "Doc Mojo" Musgrave's texturing page - This page contains
    /// links to source code that generates ridged-multfractal noise, among
    /// other types of noise.  The source file
    /// http://www.texturingandmodeling.com/CODE/MUSGRAVE/CLOUD/fractal.c
    /// contains the code I used in my ridged-multifractal class.
    /// This code was written by F. Kenton Musgrave, the person who created
    /// http://www.pandromeda.com/.  He is also one of
    /// the authors in Texturing and Modeling: A Procedural Approach
    /// (Morgan Kaufmann, 2002. ISBN 1-55860-848-6.)
    /// </remarks>
    [Serializable]
    public class RidgedMulti : Module
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
        /// Default noise quality
        /// </summary>
        public const NoiseQuality DefaultQuality = NoiseQuality.Standard;

        /// <summary>
        /// Default noise seed
        /// </summary>
        public const int DefaultSeed = 0;

        /// <summary>
        /// Maximum allowed octave count
        /// </summary>
        public const int MaxOctaves = 30;

        /// <summary>
        /// Gets or sets the frequency of the first octave.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the lacunarity of the ridged-multifractal noise.
        /// </summary>
        /// <remarks>
        /// The lacunarity is the frequency multiplier between successive
        /// octaves.
        /// 
        /// The setter recalculates spectral weights for the module,
        /// which may be a costly operation.
        /// </remarks>
        public double Lacunarity
        {
            get { return lacunarity; }
            set
            {
                lacunarity = value;
                CalcSpectralWeights();
            }
        }

        /// <summary>
        /// Gets or sets the quality of the ridged-multifractal noise.
        /// </summary>
        public NoiseQuality Quality { get; set; }

        /// <summary>
        /// Gets or sets the number of octaves that generate the
        /// ridged-multifractal noise.
        /// </summary>
        /// <remarks>
        /// The number of octaves controls the amount of detail in the
        /// ridged-multifractal noise.
        /// </remarks>
        public int OctaveCount
        {
            get { return octaves; }
            set
            {
                if (value > MaxOctaves)
                    throw new ArgumentException("Octave count cannot be greater than MaxOctaves");
                octaves = value;
            }
        }

        /// <summary>
        /// Gets or sets the seed value used by the ridged-multifractal-noise
        /// function.
        /// </summary>
        public int Seed { get; set; }

        double[] spectralWeights;
        double lacunarity;
        int octaves;

        /// <summary>
        /// Constructor.
        /// </summary>
        public RidgedMulti()
            : base(0)
        {
            Frequency = DefaultFrequency;
            Lacunarity = DefaultLacunarity;
            Quality = DefaultQuality;
            OctaveCount = DefaultOctaveCount;
            Seed = DefaultSeed;
        }

        /// <summary>
        /// Calculates the spectral weights for each octave.
        /// </summary>
        protected void CalcSpectralWeights()
        {
            // This exponent parameter should be user-defined; it may be exposed in a
            // future version of libnoise.
            double h = 1.0;

            spectralWeights = new double[MaxOctaves];

            double frequency = 1.0;
            for (var i = 0; i < MaxOctaves; i++)
            {
                // Compute weight for each frequency.
                spectralWeights[i] = Math.Pow(frequency, -h);
                frequency *= Lacunarity;
            }
        }

        // Multifractal code originally written by F. Kenton "Doc Mojo" Musgrave,
        // 1998.  Modified by jas for use with libnoise.
        // And ported over to C# by me!
        public override double GetValue(double x, double y, double z)
        {
            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            double signal = 0.0;
            double value = 0.0;
            double weight = 1.0;

            // These parameters should be user-defined; they may be exposed in a
            // future version of libnoise.
            double offset = 1.0;
            double gain = 2.0;

            for (var curOctave = 0; curOctave < OctaveCount; curOctave++)
            {
                // Get the coherent-noise value.
                int seed = (Seed + curOctave) & 0x7fffffff;
                signal = NoiseGenerator.GradientCoherentNoise3D(x, y, z, seed, Quality);

                // Make the ridges.
                signal = offset - Math.Abs(signal);

                // Square the signal to increase the sharpness of the ridges.
                signal *= signal;

                // The weighting from the previous octave is applied to the signal.
                // Larger values have higher weights, producing sharp points along the
                // ridges.
                signal *= weight;

                // Weight successive contributions by the previous signal.
                weight = signal * gain;
                if (weight > 1.0)
                    weight = 1.0;
                if (weight < 0.0)
                    weight = 0.0;

                // Add the signal to the output value.
                value += (signal * spectralWeights[curOctave]);

                // Go to the next octave.
                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
            }

            return (value * 1.25) - 1.0;
        }
    }
}
