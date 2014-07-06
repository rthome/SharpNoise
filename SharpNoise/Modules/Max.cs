using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs the larger of the two output values from two
    /// source modules.
    /// </summary>
    /// <remarks>
    /// This noise module requires two source modules.
    /// </remarks>
    [Serializable]
    public class Max : Module
    {
        /// <summary>
        /// Gets or sets the first source module
        /// </summary>
        public Module Source0
        {
            get { return GetSourceModule(0); }
            set { SetSourceModule(0, value); }
        }

        /// <summary>
        /// Gets or sets the second source module
        /// </summary>
        public Module Source1
        {
            get { return GetSourceModule(1); }
            set { SetSourceModule(1, value); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Max()
            : base(2)
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
            return Math.Max(
                sourceModules[0].GetValue(x, y, z),
                sourceModules[1].GetValue(x, y, z));
        }
    }
}
