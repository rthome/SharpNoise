using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// Noise module that moves the coordinates of the input value before
    /// returning the output value from a source module.
    /// </summary>
    /// <remarks>
    /// The <see cref="GetValue"/> method moves the ( x, y, z ) coordinates of
    /// the input value by a translation amount before returning the output
    /// value from the source module.  To set the translation amount, call
    /// the <see cref="SetTranslation"/> method.  To set the translation amount to
    /// apply to the individual x, y, or z coordinates, modify the
    /// XTranslation, YTranslation or ZTranslation properties,
    /// respectively.
    ///
    /// This noise module requires one source module.
    /// </remarks>
    [Serializable]
    public class TranslatePoint : Module
    {
        /// <summary>
        /// Default translation amount
        /// </summary>
        public const double DefaultTranslation = 0D;

        /// <summary>
        /// Gets or sets the first source module
        /// </summary>
        public Module Source0
        {
            get { return GetSourceModule(0); }
            set { SetSourceModule(0, value); }
        }

        /// <summary>
        /// Gets or sets the translation amount to apply to theb x coordinate of
        /// the input value.
        /// </summary>
        public double XTranslation { get; set; }

        /// <summary>
        /// Gets or sets the translation amount to apply to theb y coordinate of
        /// the input value.
        /// </summary>
        public double YTranslation { get; set; }

        /// <summary>
        /// Gets or sets the translation amount to apply to the z coordinate of
        /// the input value.
        /// </summary>
        public double ZTranslation { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TranslatePoint()
            : base(1)
        {
            XTranslation = DefaultTranslation;
            YTranslation = DefaultTranslation;
            ZTranslation = DefaultTranslation;
        }

        /// <summary>
        /// Sets the translation amount to apply to the input value.
        /// </summary>
        /// <param name="translation">The translation amount to apply.</param>
        /// <remarks>
        /// The <see cref="GetValue"/> method moves the ( x, y, z ) coordinates
        /// of the input value by a translation amount before returning the
        /// output value from the source module
        /// </remarks>
        public void SetTranslation(double translation)
        {
            XTranslation = translation;
            YTranslation = translation;
            ZTranslation = translation;
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
            return sourceModules[0].GetValue(
                x + XTranslation,
                y + YTranslation,
                z + ZTranslation);
        }
    }
}
