using SharpNoise.Modules;

namespace SharpNoise.Models
{
    /// <summary>
    /// Model that defines the surface of a plane.
    /// </summary>
    /// <remarks>
    /// This model returns an output value from a noise module given the
    /// coordinates of an input value located on the surface of an ( x,
    /// z ) plane.
    ///
    /// To generate an output value, pass the ( x, z ) coordinates of
    /// an input value to the GetValue() method.
    ///
    /// This model is useful for creating:
    /// - two-dimensional textures
    /// - terrain height maps for local areas
    ///
    /// This plane extends infinitely in both directions.
    /// </remarks>
    public class Plane : Model
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Plane() 
        {
 
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceModule">
        /// The noise module that is used to generate the output
        /// values.
        /// </param>
        public Plane(Module sourceModule)
        {
            Source = sourceModule;
        }

        /// <summary>
        /// Returns the output value from the noise module given the
        /// ( x, z ) coordinates of the specified input value located
        /// on the surface of the plane.
        /// </summary>
        /// <param name="x">The x coordinate of the input value.</param>
        /// <param name="z">The z coordinate of the input value.</param>
        /// <returns>The output value from the noise module.</returns>
        public double GetValue(double x, double z)
        {
            return Source.GetValue(x, 0, z);
        }
    }
}
