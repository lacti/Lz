using System;
using System.IO;
using System.Linq;
using LzEngine.Util;

namespace LzServer
{
    internal class DataManager
    {
        private readonly string[] _names;

        private readonly Random _random = new Random();
        private readonly dynamic _world;
        private readonly XmlDefinition _xmlDef;

        internal DataManager()
        {
            _names =
                File.ReadAllLines(Path.Combine("Datasheet", "Names.txt"))
                    .Where(e => !string.IsNullOrWhiteSpace(e))
                    .Select(e => e.Trim())
                    .ToArray();

            _xmlDef = new XmlDefinition(Path.Combine("Datasheet", "DataDefine.def"));
            _world = XmlObject.Load(Path.Combine("Datasheet", "World.xml"), _xmlDef);
        }

        public dynamic World
        {
            get { return _world; }
        }

        public string GetRandomName()
        {
            return _names[_random.Next(_names.Length)];
        }
    }
}