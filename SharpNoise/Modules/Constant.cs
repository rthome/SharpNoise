using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs a constant value.
    /// </summary>
    /// <remarks>
    /// To specify the constant value, modify the <see cref="ConstantValue"/> property.
    ///
    /// This noise module is not useful by itself, but it is often used as a
    /// source module for other noise modules.
    ///
    /// This noise module does not require any source modules.
    /// </remarks>
    [Serializable]
    public class Constant : Module
    {
        /// <summary>
        /// The default constant value of the Constant module
        /// </summary>
        public const double DefaultConstantValue = 0D;

        /// <summary>
        /// The value that will be produced by the module
        /// </summary>
        public double ConstantValue { get; set; } = DefaultConstantValue;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Constant()
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
            return ConstantValue;
        }
    }
}
