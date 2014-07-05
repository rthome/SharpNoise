using System;
using System.Runtime.Serialization;

namespace SharpNoise.Modules
{
    /// <summary>
    /// The exception that indicates that an error has occured during serialization or deserialization of a module
    /// </summary>
    [Serializable]
    public class ModuleSerializationException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ModuleSerializationException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        public ModuleSerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">The message that describes the error</param>
        /// <param name="inner">The exception that is the cause of this exception</param>
        public ModuleSerializationException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Serialization context</param>
        protected ModuleSerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
