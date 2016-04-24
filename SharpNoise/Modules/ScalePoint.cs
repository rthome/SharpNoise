using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that scales the coordinates of the input value before
    /// returning the output value from a source module.
    /// </summary>
    /// <remarks>
    /// The <see cref="GetValue"/> method multiplies the ( x, y, z ) coordinates
    /// of the input value with a scaling factor before returning the output
    /// value from the source module.  To set the scaling factor, call the
    /// <see cref="SetScale"/> method.  To set the scaling factor to apply to the
    /// individual x, y, or z coordinates, modify the <see cref="XScale"/>,
    /// <see cref="YScale"/> or <see cref="ZScale"/> properties, respectively.
    ///
    /// This noise module requires one source module.
    /// </remarks>
    [Serializable]
    public class ScalePoint : Module
    {
        /// <summary>
        /// Default scaling factor applied to all coordinates
        /// </summary>
        public const double DefaultScale = 1D;

        /// <summary>
        /// Gets or sets the first source module
        /// </summary>
        public Module Source0
        {
            get { return SourceModules[0]; }
            set { SourceModules[0] = value; }
        }

        /// <summary>
        /// Gets or sets the scaling factor applied to the x coordinate of the
        /// input value.
        /// </summary>
        public double XScale { get; set; } = DefaultScale;

        /// <summary>
        /// Gets or sets the scaling factor applied to the y coordinate of the
        /// input value.
        /// </summary>
        public double YScale { get; set; } = DefaultScale;

        /// <summary>
        /// Gets or sets the scaling factor applied to the z coordinate of the
        /// input value.
        /// </summary>
        public double ZScale { get; set; } = DefaultScale;

        /// <summary>
        /// Sets the scaling factor to apply to the input value.
        /// </summary>
        /// <param name="scale">The scaling factor to apply.</param>
        public void SetScale(double scale)
        {
            XScale = scale;
            YScale = scale;
            ZScale = scale;
        }

        /// <summary>
        /// Sets the scaling factors to apply to the input value.
        /// </summary>
        /// <param name="xScale">The scaling factor to apply to the x coordinate.</param>
        /// <param name="yScale">The scaling factor to apply to the y coordinate.</param>
        /// <param name="zScale">The scaling factor to apply to the z coordinate.</param>
        public void SetScale(double xScale, double yScale, double zScale)
        {
            XScale = xScale;
            YScale = yScale;
            ZScale = zScale;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScalePoint()
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
            return SourceModules[0].GetValue(
                x * XScale, 
                y * YScale,
                z * ZScale);
        }
    }
}
