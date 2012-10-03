using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs 3-dimensional Perlin noise.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Perlin noise is the sum of several coherent-noise functions of
    /// ever-increasing frequencies and ever-decreasing amplitudes.
    ///
    /// An important property of Perlin noise is that a small change in the
    /// input value will produce a small change in the output value, while a
    /// large change in the input value will produce a random change in the
    /// output value.
    ///
    /// This noise module outputs Perlin-noise values that usually range from
    /// -1.0 to +1.0, but there are no guarantees that all output values will
    /// exist within that range.
    ///
    /// For a better description of Perlin noise, see the links in the
    /// References and Acknowledgments section.
    ///
    /// This noise module does not require any source modules.
    /// </para>
    /// 
    /// <para>
    /// Octaves
    ///
    /// The number of octaves control the amount of detail of the
    /// Perlin noise.  Adding more octaves increases the detail of the Perlin
    /// noise, but with the drawback of increasing the calculation time.
    ///
    /// An octave is one of the coherent-noise functions in a series of
    /// coherent-noise functions that are added together to form Perlin
    /// noise.
    ///
    /// An application may specify the frequency of the first octave by
    /// changing the <see cref="Frequency" /> property.
    ///
    /// An application may specify the number of octaves that generate Perlin
    /// noise by changing the <see cref="OctaveCount" /> property.
    ///
    /// These coherent-noise functions are called octaves because each octave
    /// has, by default, double the frequency of the previous octave.  Musical
    /// tones have this property as well; a musical C tone that is one octave
    /// higher than the previous C tone has double its frequency.
    /// </para>
    /// 
    /// <para>
    /// Frequency
    ///
    /// An application may specify the frequency of the first octave by
    /// changing the <see cref="Frequency" /> property.
    /// </para>
    /// 
    /// <para>
    /// Persistence
    ///
    /// The persistence value controls the roughness of the Perlin
    /// noise.  Larger values produce rougher noise.
    ///
    /// The persistence value determines how quickly the amplitudes diminish
    /// for successive octaves.  The amplitude of the first octave is 1.0.
    /// The amplitude of each subsequent octave is equal to the product of the
    /// previous octave's amplitude and the persistence value.  So a
    /// persistence value of 0.5 sets the amplitude of the first octave to
    /// 1.0; the second, 0.5; the third, 0.25; etc.
    ///
    /// An application may specify the persistence value by changing the
    /// <see cref="Persistence" /> property.
    /// </para>
    /// 
    /// <para>
    /// Lacunarity
    ///
    /// The lacunarity specifies the frequency multipler between successive
    /// octaves.
    ///
    /// The effect of modifying the lacunarity is subtle; you may need to play
    /// with the lacunarity value to determine the effects.  For best results,
    /// set the lacunarity to a number between 1.5 and 3.5.
    /// </para>
    /// 
    /// <para>
    /// References &amp; acknowledgments
    ///
    /// http://www.noisemachine.com/talk1/
    /// The Noise Machine - From the master, Ken Perlin himself.  
    /// This page contains a presentation that describes Perlin noise
    /// and some of its variants.
    /// He won an Oscar for creating the Perlin noise algorithm!
    ///
    /// http://freespace.virgin.net/hugo.elias/models/m_perlin.htm
    /// Perlin Noise - Hugo Elias's webpage contains a very good
    /// description of Perlin noise and describes its many applications.  This
    /// page gave me the inspiration to create libnoise in the first place.
    /// Now that I know how to generate Perlin noise, I will never again use
    /// cheesy subdivision algorithms to create terrain (unless I absolutely
    /// need the speed.)
    ///
    /// http://www.robo-murito.net/code/perlin-noise-math-faq.html
    /// The Perlin noise math FAQ - A good page that describes Perlin noise in
    /// plain English with only a minor amount of math.  During development of
    /// libnoise, I noticed that my coherent-noise function generated terrain
    /// with some "regularity" to the terrain features.  This page describes a
    /// better coherent-noise function called gradient noise.  This
    /// version of Perlin uses gradient coherent noise to
    /// generate Perlin noise.
    /// </para>
    /// </remarks>
    [Serializable]
    public class Perlin : Module
    {
        /// <summary>
        /// Default frequency for the Perlin noise module.
        /// </summary>
        public const double DefaultFrequency = 1D;

        /// <summary>
        /// Default lacunarity for the Perlin noise module.
        /// </summary>
        public const double DefaultLacunarity = 2D;

        /// <summary>
        /// Default number of octaves for the Perlin noise module.
        /// </summary>
        public const int DefaultOctaveCount = 6;

        /// <summary>
        /// Default persistence value for the Perlin noise module.
        /// </summary>
        public const double DefaultPersistence = 0.5D;

        /// <summary>
        /// Default noise quality for the Perlin noise module.
        /// </summary>
        public const NoiseQuality DefaultQuality = NoiseQuality.Standard;

        /// <summary>
        /// Default noise seed for the Perlin noise module.
        /// </summary>
        public const int DefaultSeed = 0;

        /// <summary>
        /// Gets or sets the frequency of the first octave.
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// Gets or sets the lacunarity of the Perlin noise.
        /// </summary>
        /// <remarks>
        /// The lacunarity is the frequency multiplier between successive
        /// octaves.
        ///
        /// For best results, set the lacunarity to a number between 1.5 and
        /// 3.5.
        /// </remarks>
        public double Lacunarity { get; set; }

        /// <summary>
        /// Gets or sets the number of octaves that generate the Perlin noise.
        /// </summary>
        /// <remarks>
        /// The number of octaves controls the amount of detail in the Perlin
        /// noise.
        ///
        /// The larger the number of octaves, the more time required to
        /// calculate the Perlin-noise value.
        /// </remarks>
        public int OctaveCount { get; set; }

        /// <summary>
        /// Gets or sets the persistence value of the Perlin noise.
        /// </summary>
        /// <remarks>
        /// The persistence value controls the roughness of the Perlin noise.
        ///
        /// For best results, set the persistence to a number between 0.0 and
        /// 1.0.
        /// </remarks>
        public double Persistence { get; set; }

        /// <summary>
        /// Gets or sets the quality of the Perlin noise.
        /// </summary>
        public NoiseQuality Quality { get; set; }

        /// <summary>
        /// Gets or sets the seed value used by the Perlin-noise function.
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Perlin()
            : base(0)
        {
            Frequency = DefaultFrequency;
            Lacunarity = DefaultLacunarity;
            OctaveCount = DefaultOctaveCount;
            Persistence = DefaultPersistence;
            Quality = DefaultQuality;
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
            double value = 0D;
            double signal = 0D;
            double currentPersistence = 1D;
            int seed;

            x *= Frequency;
            y *= Frequency;
            z *= Frequency;

            for (var currentOctave = 0; currentOctave < OctaveCount; currentOctave++)
            {
                seed = (Seed + currentOctave) & int.MaxValue;
                signal = NoiseGenerator.GradientCoherentNoise3D(x, y, z, seed, Quality);
                value += signal * currentPersistence;

                x *= Lacunarity;
                y *= Lacunarity;
                z *= Lacunarity;
                currentPersistence *= Persistence;
            }

            return value;
        }
    }
}
