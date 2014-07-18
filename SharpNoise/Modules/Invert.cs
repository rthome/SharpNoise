using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that inverts the output value from a source module.
    /// </summary>
    /// <remarks>
    /// This noise module requires one source module.
    /// </remarks>
    [Serializable]
    public class Invert : Module
    {
        /// <summary>
        /// Gets or sets the first source module
        /// </summary>
        public Module Source0
        {
            get { return SourceModules[0]; }
            set { SourceModules[0] = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Invert()
            : base(1)
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
            return -1D * SourceModules[0].GetValue(x, y, z);
        }
    }
}
