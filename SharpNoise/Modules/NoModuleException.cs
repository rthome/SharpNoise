using System;

namespace SharpNoise.Modules
{
    /// <summary>
    /// The exception that indicates that a module is missing
    /// </summary>
    [Serializable]
    public class NoModuleException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public NoModuleException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public NoModuleException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of this exception</param>
        public NoModuleException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
