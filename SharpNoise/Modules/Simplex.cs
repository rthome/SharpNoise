using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs 3-dimensional Simplex noise.
    /// </summary>
    public class Simplex : Module
    {
        /// <summary>
        /// See documentation on the base class
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="z">Z coordinate</param>
        /// <returns>The computed value</returns>
        public override double GetValue(double x, double y, double z)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Simplex()
            : base(0)
        {

        }
    }
}
