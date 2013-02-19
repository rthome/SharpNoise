using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
        /// Create a mapping of module names to module types in SharpNoise
        /// </summary>
        /// <returns>A dictionary of module name strings to module types</returns>
        protected ReadOnlyDictionary<string, Type> CreateModuleTypeMapping()
        {
            var dict = new Dictionary<string, Type>();

            var moduleType = typeof(Module);
            var types = moduleType.Assembly.GetTypes().Where(t => t.IsSubclassOf(moduleType));

            var allTypes = types.Concat(customModuleTypes);

            foreach (var type in allTypes)
            {
                dict.Add(type.Name, type);
            }

            return new ReadOnlyDictionary<string, Type>(dict);
        }

        /// <summary>
        /// Create a mapping of properties to their values for a module
        /// </summary>
        /// <param name="module">The module to create a property mapping for</param>
        /// <returns>A dictionary of property names to property values</returns>
        protected ReadOnlyDictionary<string, string> ReadProperties(Module module)
        {
            var dict = new Dictionary<string, string>();

            var properties = module.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                if (property.DeclaringType == typeof(Module))
                    continue;
                if (property.PropertyType == typeof(Module) || property.PropertyType.IsSubclassOf(typeof(Module)))
                    continue;

                var value = property.GetValue(module);
                dict.Add(property.Name, value.ToString());
            }

            return new ReadOnlyDictionary<string, string>(dict);
        }

        /// <summary>
        /// Flattens a module graoh into a list of modules
        /// </summary>
        /// <param name="module">The root module of the module graph</param>
        /// <returns>Returns an enumerable with the given module and, recursively, it's source modules</returns>
        /// <remarks>
        /// This will filter out duplicate modules from the graph. Only distinct modules will be returned.
        /// </remarks>
        protected IEnumerable<Module> GetModulesRecursive(Module module)
        {
            IEnumerable<Module> result = Enumerable.Repeat(module, 1);

            if (module.SourceModuleCount > 0)
            {
                for (int i = 0; i < module.SourceModuleCount; ++i)
                {
                    var childModules = GetModulesRecursive(module.GetSourceModule(i));
                    result = result.Concat(childModules);
                }
            }

            return result.Distinct();
        }

        /// <summary>
        /// Create a mapping of modules to unique IDs
        /// </summary>
        /// <param name="modules">The modules to create IDs for</param>
        /// <returns>Returns a dictionary of modules to strings</returns>
        /// <remarks>
        /// The given enumerable of modules must not contain duplicate elements. All modules must be distinct.
        /// </remarks>
        protected Dictionary<Module, string> CreateIDMapping(IEnumerable<Module> modules)
        {
            var dict = new Dictionary<Module, string>();

            foreach (var module in modules)
            {
                dict.Add(module, Guid.NewGuid().ToString());
            }

            return dict;
        }

        #region Custom Module Types

        readonly List<Type> customModuleTypes;

        /// <summary>
        /// Gets an enumeration of added custom noise module types
        /// </summary>
        public IEnumerable<Type> CustomModuleTypes
        {
            get
            {
                return customModuleTypes.AsReadOnly();
            }
        }

        /// <summary>
        /// Add some custom noise module types to the serializer
        /// </summary>
        /// <param name="moduleTypes">An enumeration of Type, where each item must derive from Module</param>
        public void AddCustomModuleTypes(IEnumerable<Type> moduleTypes)
        {
            if (moduleTypes == null)
                throw new ArgumentNullException("moduleTypes");

            foreach (var type in moduleTypes)
            {
                if (!type.IsSubclassOf(typeof(Module)))
                    throw new ArgumentException(String.Format("A given Type does not derive from Module: <[0}>.", type));
                if (customModuleTypes.Contains(type))
                    throw new ArgumentException(String.Format("A given Type was already registered: <[0}>.", type));
            }

            customModuleTypes.AddRange(moduleTypes);
        }

        /// <summary>
        /// Remove all previously added custom noise module types
        /// </summary>
        public void ClearCustomModuleTypes()
        {
            customModuleTypes.Clear();
        }

        #endregion

        #region Public Interface

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

        #endregion

        protected Serializer()
        {
            customModuleTypes = new List<Type>();
        }
    }
}
