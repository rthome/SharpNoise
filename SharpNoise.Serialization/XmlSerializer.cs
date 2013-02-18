using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Module = SharpNoise.Modules.Module;

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

        #region Save Helpers

        IEnumerable<XElement> CreateModuleElements(IEnumerable<Module> modules, Dictionary<Module, string> idMapping)
        {
            var moduleElements = new List<XElement>();

            foreach (var module in modules)
            {
                var propertyMapping = ReadProperties(module);
                var element = new XElement("Module",
                    new XAttribute("id", idMapping[module]),
                    new XAttribute("type", module.GetType().Name),
                    from property in propertyMapping
                    select new XElement("Property",
                        new XAttribute("name", property.Key),
                        new XAttribute("value", property.Value)));
                moduleElements.Add(element);
            }

            return moduleElements;
        }

        IEnumerable<XElement> CreateConnectionElements(IEnumerable<Module> modules, Dictionary<Module, string> idMapping)
        {
            var connections = new List<XElement>();

            foreach (var module in modules)
            {
                for (int i = 0; i < module.SourceModuleCount; ++i)
                {
                    var source = module.GetSourceModule(i);
                    var element = new XElement("Connection",
                        new XAttribute("for", idMapping[module]),
                        new XAttribute("source", idMapping[source]),
                        new XAttribute("slot", i));
                    connections.Add(element);
                }
            }

            return connections;
        }

        XDocument CreateModuleGraphDocument(IEnumerable<XElement> moduleElements, IEnumerable<XElement> connectionElements, string rootID)
        {
            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("ModuleGraph",
                    new XAttribute("root", rootID),
                    moduleElements,
                    connectionElements));
            return document;
        }

        #endregion

        #region Restore Helpers

        XDocument ReadModuleGraphDocument(Stream source)
        {
            var doc = XDocument.Load(source);
            return doc;
        }

        Dictionary<string, Module> RestoreModules(IEnumerable<XElement> moduleElements)
        {
            throw new NotImplementedException();
        }

        void SetProperties(Module instance, Dictionary<string, string> propertyMapping)
        {
            throw new NotImplementedException();
        }

        void SetSources(IEnumerable<XElement> connectionElements, Dictionary<string, Module> modules)
        {
            foreach (var connection in connectionElements)
            {
                var moduleID = connection.Attribute("for").Value;
                var sourceModuleID = connection.Attribute("source").Value;
                var slot = int.Parse(connection.Attribute("slot").Value);

                var module = modules[moduleID];
                var source = modules[sourceModuleID];

                module.SetSourceModule(slot, source);
            }
        }

        #endregion

        protected override void Serialize(Stream targetStream, Module module)
        {
            var modules = GetModulesRecursive(module);
            var mapping = CreateIDMapping(modules);

            var moduleElements = CreateModuleElements(modules, mapping);
            var connectionElements = CreateConnectionElements(modules, mapping);
            var rootID = mapping[module];

            var document = CreateModuleGraphDocument(moduleElements, connectionElements, rootID);
            document.Save(targetStream);
        }

        protected override Module Deserialize(Stream sourceStream)
        {
            var document = ReadModuleGraphDocument(sourceStream);

            var moduleElements = document.Root.Elements("Module");
            var connectionElements = document.Root.Elements("Connection");
            var rootElement = document.Root.Element("Root");

            var moduleMapping = RestoreModules(moduleElements);
            SetSources(connectionElements, moduleMapping);

            var root = moduleMapping[rootElement.Value];
            return root;
        }
    }
}
