using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that outputs a weighted blend of the output values from
    /// two source modules given the output value supplied by a control module.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Unlike most other noise modules, the index value assigned to a source
    /// module determines its role in the blending operation:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// Source module 0 (upper left in the diagram) outputs one of the
    ///   values to blend.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Source module 1 (lower left in the diagram) outputs one of the
    ///   values to blend.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Source module 2 (bottom of the diagram) is known as the control
    /// module. The control module determines the weight of the
    /// blending operation. Negative values weigh the blend towards the
    /// output value from the source module with an index value of 0.
    /// Positive values weigh the blend towards the output value from the
    /// source module with an index value of 1.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// 
    /// An application can set the control module with the <see cref="ControlModule"/>
    /// property instead of the <see cref="SetSourceModule"/> method.  This may make the
    /// application code easier to read.
    ///
    /// This noise module uses linear interpolation to perform the blending
    /// operation.
    ///
    /// This noise module requires three source modules.
    /// </remarks>
    [Serializable]
    public class Blend : Module
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
        /// Gets or sets the control module
        /// </summary>
        public Module Control
        {
            get { return GetSourceModule(2); }
            set { SetSourceModule(2, value); }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Blend()
            : base(3)
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
            var v0 = sourceModules[0].GetValue(x, y, z);
            var v1 = sourceModules[1].GetValue(x, y, z);
            var alpha = (sourceModules[2].GetValue(x, y, z) + 1) / 2;
            return NoiseMath.Linear(v0, v1, alpha);
        }
    }
}
