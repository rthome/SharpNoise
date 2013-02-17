using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SharpNoise.Serialization
{
    /// <summary>
    /// Serializes a module graph to an XML representation
    /// </summary>
    /// <remarks>
    /// This is supposed to be a more or less human-readable format for module graphs.
    /// For efficient serialization, try the .NET serialization built into Module.
    /// </remarks>
    public class XmlSerializer : Serializer
    {
        Dictionary<string, string> ReadProperties(SharpNoise.Modules.Module module)
        {
            var dict = new Dictionary<string, string>();

            var properties = module.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var property in properties)
            {
                var value = property.GetValue(module);
                dict.Add(property.Name, value.ToString());
            }

            return dict;
        }

        protected override void Serialize(Stream targetStream, SharpNoise.Modules.Module module)
        {
            throw new NotImplementedException();
        }

        protected override SharpNoise.Modules.Module Deserialize(Stream sourceStream)
        {
            throw new NotImplementedException();
        }
    }
}
