using SharpNoise.Modules;

namespace SharpNoise.Models
{
    /// <summary>
    /// Model that defines the surface of a sphere.
    /// </summary>
    /// <remarks>
    /// This model returns an output value from a noise module given the
    /// coordinates of an input value located on the surface of a sphere.
    ///
    /// To generate an output value, pass the (latitude, longitude)
    /// coordinates of an input value to the <see cref="GetValue"/> method.
    ///
    /// This model is useful for creating:
    /// - seamless textures that can be mapped onto a sphere
    /// - terrain height maps for entire planets
    ///
    /// This sphere has a radius of 1.0 unit and its center is located at
    /// the origin.
    /// </remarks>
    public class Sphere : Model
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Sphere()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sourceModule">The noise module that is used to generate the output
        /// values.</param>
        public Sphere(Module sourceModule)
        {
            Source = sourceModule;
        }

        /// <summary>
        /// Returns the output value from the noise module given the
        /// (latitude, longitude) coordinates of the specified input value
        /// located on the surface of the sphere.
        /// </summary>
        /// <param name="lat">The latitude of the input value, in degrees.</param>
        /// <param name="lon">The longitude of the input value, in degrees.</param>
        /// <returns>The output value from the noise module.</returns>
        /// <remarks>
        /// Use a negative latitude if the input value is located on the
        /// southern hemisphere.
        ///
        /// Use a negative longitude if the input value is located on the
        /// western hemisphere.
        /// </remarks>
        public double GetValue(double lat, double lon)
        {
            NoiseMath.LatLonToXYZ(lat, lon, out double x, out double y, out double z);
            return Source.GetValue(x, y, z);
        }
    }
}
