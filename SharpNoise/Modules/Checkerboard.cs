using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs a checkerboard pattern.
    /// </summary>
    /// <remarks>
    /// This noise module outputs unit-sized blocks of alternating values.
    /// The values of these blocks alternate between -1.0 and +1.0.
    ///
    /// This noise module is not really useful by itself, but it is often used
    /// for debugging purposes.
    ///
    /// This noise module does not require any source modules.
    /// </remarks>
    [Serializable]
    public class Checkerboard : Module
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Checkerboard()
            : base(0)
        {

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
            var ix = (int)Math.Floor(x);
            var iy = (int)Math.Floor(y);
            var iz = (int)Math.Floor(z);
            return ((ix & 1) ^ (iy & 1) ^ (iz & 1)) != 0 ? -1.0 : 1.0;
        }
    }
}
