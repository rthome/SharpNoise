using SharpNoise.Modules;
using System;
using System.Diagnostics;
using System.IO;

namespace SharpNoise.Serialization
{
    /// <summary>
    /// Base class for serializers.
    /// </summary>
    /// <remarks>
    /// To implement a custom serializer, create a class which derives from Serializer and 
    /// override the Serialize() and Deserialize() methods.
    /// </remarks>
    public abstract class Serializer
    {
        /// <summary>
        /// Override in derived classes to implement serialization
        /// </summary>
        /// <param name="targetStream">The stream to write serialized data to</param>
        /// <param name="module">The root module of the module graph</param>
        protected abstract void Serialize(Stream targetStream, Module module);

        /// <summary>
        /// Override in derived classes to implement deserialization
        /// </summary>
        /// <param name="sourceStream">The stream that contains serialized data</param>
        /// <returns>Returns the root module of the deserialized module graph</returns>
        protected abstract Module Deserialize(Stream sourceStream);

        /// <summary>
        /// Save a module graph to a file
        /// </summary>
        /// <param name="module">The root module of the graph</param>
        /// <param name="path">The path of the file to save to</param>
        /// <remarks>
        /// This saves the given module and, recursively, all of it's sources to the given file
        /// </remarks>
        public void Save(Module module, string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (module == null)
                throw new ArgumentNullException("module");

            try
            {
                using (var stream = File.OpenWrite(path))
                {
                    Save(module, stream);
                }
            }
            catch (IOException e)
            {
                throw new ArgumentException("Error writing to the given path.", e);
            }
        }

        /// <summary>
        /// Save a module graoh to a Stream
        /// </summary>
        /// <param name="module">The root module of the graph</param>
        /// <param name="stream">The stream to save to</param>
        /// <remarks>
        /// This saves the given module and, recursively, all of it's sources to the given stream
        /// </remarks>
        public void Save(Module module, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (module == null)
                throw new ArgumentNullException("module");

            Debug.Assert(stream.CanWrite, "The given stream is not writable.");

            Serialize(stream, module);
        }

        /// <summary>
        /// Load a module graph from a file
        /// </summary>
        /// <param name="path">The path of the file to read from</param>
        /// <returns>Returns the root module of the graph</returns>
        public Module Restore(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            try
            {
                using (var stream = File.OpenRead(path))
                {
                    return Restore(stream);
                }
            }
            catch (IOException e)
            {
                throw new ArgumentException("Error reading from the given file.", e);
            }
        }

        /// <summary>
        /// Load a module graph from a stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <returns>Returns the root module of the graph</returns>
        public Module Restore(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            Debug.Assert(stream.CanRead, "The given stream is not readable.");

            return Deserialize(stream);
        }

        /// <summary>
        /// Load a module graph from a file
        /// </summary>
        /// <typeparam name="T">The type of the module that is being loaded.</typeparam>
        /// <param name="path">The path of the file to read from</param>
        /// <returns>Returns the root module of the graph</returns>
        public T Restore<T>(string path) where T : Module
        {
            return (T)Restore(path);
        }

        /// <summary>
        /// Load a module graph from a stream
        /// </summary>
        /// <typeparam name="T">The type of the module that is being loaded.</typeparam>
        /// <param name="stream">The stream to read from</param>
        /// <returns>Returns the root module of the graph</returns>
        public T Restore<T>(Stream stream) where T : Module
        {
            return (T)Restore(stream);
        }
    }
}
