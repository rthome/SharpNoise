using SharpNoise.Modules;

namespace SharpNoise.Models
{
    /// <summary>
    /// Base class for models
    /// </summary>
    public abstract class Model
    {
        /// <summary>
        /// The module used to generate noise.
        /// </summary>
        /// <remarks>
        /// Must be set before GetValue can be called in derived classes.
        /// </remarks>
        public Module Source { get; set; }
    }
}
