using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml;

namespace LzEngine.Util
{
    public class XmlObject : DynamicObject
    {
        private readonly Dictionary<string, object> _attributes;
        private readonly Dictionary<string, IEnumerable<XmlObject>> _multipleChildren;
        private readonly Dictionary<string, XmlObject> _singleChildren;

        private XmlObject(XmlElement element, XmlObject parent, XmlDefinitionNode defNode)
        {
            Name = element.Name;
            Parent = parent;
            DefinitionNode = defNode;

            _attributes = element.Attributes.OfType<XmlAttribute>().ToDictionary(e => e.Name, e =>
                                                                                              defNode.SelectAttribute(e.Name).ReadValue(e.Value));

            var childrenGroup = element.ChildNodes.OfType<XmlElement>().GroupBy(e => e.Name).ToArray();
            _singleChildren =
                childrenGroup.Where(e => e.Count() == 1)
                             .ToDictionary(e => e.Key, e => new XmlObject(e.First(), this, defNode.SelectNode(e.Key)));
            _multipleChildren =
                childrenGroup.Where(e => e.Count() > 1)
                             .ToDictionary(e => e.Key,
                                           e => e.Select(i => new XmlObject(i, this, defNode.SelectNode(i.Name))));
        }

        public string Name { get; private set; }

        public XmlObject Parent { get; private set; }
        public XmlDefinitionNode DefinitionNode { get; private set; }

        private bool TryGetValue<T>(Dictionary<string, T> dic, string name, out object result)
        {
            result = null;

            T value;
            if (!dic.TryGetValue(name, out value))
                return false;
            result = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetValue(_attributes, binder.Name, out result) ||
                   TryGetValue(_singleChildren, binder.Name, out result) ||
                   TryGetValue(_multipleChildren, binder.Name, out result);
        }

        public static XmlObject Load(string path, XmlDefinition def)
        {
            var doc = new XmlDocument();
            doc.Load(path);
            return new XmlObject(doc.DocumentElement, null, def.SelectNode(doc.DocumentElement.Name));
        }
    }
}