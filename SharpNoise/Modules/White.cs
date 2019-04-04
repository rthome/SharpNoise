using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs 3-dimensional White noise.
    /// </summary>
    /// References &amp; acknowledgments
    /// http://www.dspguru.com/dsp/howtos/how-to-generate-white-gaussian-noise
    [Serializable]
    public class White : Module
    {
        public int Scale { get; set; }

        public int Seed { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public White()
            : base(0)
        {
            Scale = 256;
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
            return NoiseGenerator.ValueNoise3D((int)(x * Scale), (int)(y * Scale), (int)(z * Scale), Seed);
        }
    }
}