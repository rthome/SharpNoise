using System;

using SharpNoise.Modules;

namespace SharpNoise.Models
{
    /// <summary>
    /// Model that defines the surface of a cylinder.
    /// </summary>
    /// <remarks>
    /// This model returns an output value from a noise module given the
    /// coordinates of an input value located on the surface of a cylinder.
    ///
    /// To generate an output value, pass the (angle, height) coordinates of
    /// an input value to the <see cref="GetValue"/> method.
    ///
    /// This model is useful for creating:
    /// - seamless textures that can be mapped onto a cylinder
    ///
    /// This cylinder has a radius of 1.0 unit and has infinite height.  It is
    /// oriented along the y axis.  Its center is located at the origin.
    /// </remarks>
    public class Cylinder : Model
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Cylinder()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceModule">The noise module that is used to generate the output
        /// values.</param>
        public Cylinder(Module sourceModule)
        {
            Source = sourceModule;
        }

        /// <summary>
        /// Returns the output value from the noise module given the
        /// (angle, height) coordinates of the specified input value located
        /// on the surface of the cylinder.
        /// </summary>
        /// <param name="angle">The angle around the cylinder's center, in degrees.</param>
        /// <param name="height">The height along the y axis.</param>
        /// <returns>The output value from the noise module.</returns>
        /// <remarks>
        /// This cylinder has a radius of 1.0 unit and has infinite height.
        /// It is oriented along the y axis.  Its center is located at the
        /// origin.
        /// </remarks>
        public double GetValue(double angle, double height)
        {
            var x = Math.Cos(angle * NoiseMath.DegToRad);
            var y = height;
            var z = Math.Sin(angle * NoiseMath.DegToRad);
            return Source.GetValue(x, y, z);
        }
    }
}
