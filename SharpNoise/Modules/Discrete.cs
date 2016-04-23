using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// This module take a value from a source module and from a given input "slice", return a discrete value, so its name, in the interval [i/slice, (i + 1)/slice]
    /// For example, if slice count is set to 4, the Perlin source module values are converted into 0, 1/4, 2/4, 3/4 and 1 (4/4) ONLY, creating a sharp map which can be use
    /// as filter for a select module or as-is for a tile map based game.
    /// </summary>
    /// <remarks>
    /// This noise module requires one source module.
    /// </remarks>
    [Serializable]
    public class Discrete : Module
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Discrete()
            : base(1)
        {

        }

        /// <summary>
        /// Gets or sets the number of maximum values to get from
        /// input value.
        /// </summary>
        public UInt16 Values { get; set; }

        /// <summary>
        /// Gets or sets the first, and only, source module
        /// </summary>
        public Module Source0
        {
            get { return SourceModules[0]; }
            set { SourceModules[0] = value; }
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
            double srcModValue = SourceModules[0].GetValue(x, y, z);

            if (srcModValue <= 0)
            {
                return 0.0;
            }

            for (uint i = 0; i < Values; i++)
            {
                if (srcModValue >= i * 1.0 / Values && srcModValue < (i + 1) * 1.0 / Values)
                {
                    return i * 1.0/Values;
                }
            }

            return 0.0;
        }
    }
}
