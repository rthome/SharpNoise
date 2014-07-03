using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs concentric spheres.
    /// </summary>
    /// <remarks>
    /// This noise module outputs concentric spheres centered on the origin
    /// like the concentric rings of an onion.
    ///
    /// The first sphere has a radius of 1.0.  Each subsequent sphere has a
    /// radius that is 1.0 unit larger than the previous sphere.
    ///
    /// The output value from this noise module is determined by the distance
    /// between the input value and the the nearest spherical surface.  The
    /// input values that are located on a spherical surface are given the
    /// output value 1.0 and the input values that are equidistant from two
    /// spherical surfaces are given the output value -1.0.
    ///
    /// An application can change the frequency of the concentric spheres.
    /// Increasing the frequency reduces the distances between spheres.  To
    /// specify the frequency, modify the <see cref="Frequency"/> method.
    ///
    /// This noise module, modified with some low-frequency, low-power
    /// turbulence, is useful for generating agate-like textures.
    ///
    /// This noise module does not require any source modules. 
    /// </remarks>
    [Serializable]
    public class Spheres : Module
    {
        /// <summary>
        /// Default frequency value
        /// </summary>
        public const double DefaultFrequency = 1D;

        /// <summary>
        /// Gets or sets the frequency of the concentric spheres.
        /// </summary>
        /// <remarks>
        /// Increasing the frequency increases the density of the concentric
        /// spheres, reducing the distances between them.
        /// </remarks>
        public double Frequency { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Spheres()
            : base(0)
        {
            Frequency = DefaultFrequency;
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
            var distFromCenter = Math.Sqrt(x * x + y * y + z * z);
            var distFromSmallerSphere = distFromCenter - Math.Floor(distFromCenter);
            var distFromLargerSphere = 1.0 - distFromSmallerSphere;
            var nearestDist = NoiseMath.Min(distFromSmallerSphere, distFromLargerSphere);
            return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.
        }
    }
}
