using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs concentric cylinders.
    /// </summary>
    /// <remarks>
    /// This noise module outputs concentric cylinders centered on the origin.
    /// These cylinders are oriented along the y axis similar to the
    /// concentric rings of a tree.  Each cylinder extends infinitely along
    /// the y axis.
    ///
    /// The first cylinder has a radius of 1.0.  Each subsequent cylinder has
    /// a radius that is 1.0 unit larger than the previous cylinder.
    ///
    /// The output value from this noise module is determined by the distance
    /// between the input value and the the nearest cylinder surface.  The
    /// input values that are located on a cylinder surface are given the
    /// output value 1.0 and the input values that are equidistant from two
    /// cylinder surfaces are given the output value -1.0.
    ///
    /// An application can change the frequency of the concentric cylinders.
    /// Increasing the frequency reduces the distances between cylinders.  To
    /// specify the frequency, modify the <see cref="Frequency"/> method.
    ///
    /// This noise module, modified with some low-frequency, low-power
    /// turbulence, is useful for generating wood-like textures.
    ///
    /// This noise module does not require any source modules.
    /// </remarks>
    [Serializable]
    public class Cylinders : Module
    {
        /// <summary>
        /// Default frequency value
        /// </summary>
        public const double DefaultFrequency = 1D;

        /// <summary>
        /// Gets or sets the frequency of the concentric cylinders.
        /// </summary>
        /// <remarks>
        /// Increasing the frequency increases the density of the concentric
        /// cylinders, reducing the distances between them.
        /// </remarks>
        public double Frequency { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Cylinders()
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
            x *= Frequency;
            z *= Frequency;

            var distFromCenter = Math.Sqrt(x * x + z * z);
            var distFromSmallerSphere = distFromCenter - Math.Floor(distFromCenter);
            var distFromLargerSphere = 1.0 - distFromSmallerSphere;
            var nearestDist = Math.Min(distFromSmallerSphere, distFromLargerSphere);
            return 1.0 - (nearestDist * 4.0); // Puts it in the -1.0 to +1.0 range.
        }
    }
}
