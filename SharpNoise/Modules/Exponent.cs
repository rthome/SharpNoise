using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that maps the output value from a source module onto an
    /// exponential curve.
    /// </summary>
    /// <remarks>
    /// Because most noise modules will output values that range from -1.0 to
    /// +1.0, this noise module first normalizes this output value (the range
    /// becomes 0.0 to 1.0), maps that value onto an exponential curve, then
    /// rescales that value back to the original range.
    ///
    /// This noise module requires one source module.
    /// </remarks>
    [Serializable]
    public class Exponent : Module
    {
        /// <summary>
        /// Default exponent
        /// </summary>
        public const double DefaultExponent = 1D;

        /// <summary>
        /// Gets or sets the first source module
        /// </summary>
        public Module Source0
        {
            get { return GetSourceModule(0); }
            set { SetSourceModule(0, value); }
        }

        /// <summary>
        /// Gets or sets the exponent value to apply to the output value from the
        /// source module.
        /// </summary>
        /// <remarks>
        /// Because most noise modules will output values that range from -1.0
        /// to +1.0, this noise module first normalizes this output value (the
        /// range becomes 0.0 to 1.0), maps that value onto an exponential
        /// curve, then rescales that value back to the original range.
        /// </remarks>
        public double Exp { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Exponent()
            : base(1)
        {
            Exp = DefaultExponent;
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
            double value = SourceModules[0].GetValue(x, y, z);
            return (Math.Pow(Math.Abs((value + 1.0) / 2.0), Exp) * 2.0 - 1.0);
        }
    }
}
