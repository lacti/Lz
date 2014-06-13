using System.Collections.Generic;
using System.Linq;

namespace LzEngine.Util
{
    public class XmlDefinitionNode
    {
        public readonly Dictionary<string, XmlDefinitionAttribute> Attributes = new Dictionary<string, XmlDefinitionAttribute>();
        public readonly Dictionary<string, XmlDefinitionNode> Children = new Dictionary<string, XmlDefinitionNode>();
        public string Name { get; internal set; }
        public XmlDefinitionNode Parent { get; internal set; }

        public XmlDefinitionNode SelectNode(string path)
        {
            if (!path.Contains('.'))
                return Children.ContainsKey(path) ? Children[path] : null;

            var childName = path.Substring(0, path.IndexOf('.'));
            return Children.ContainsKey(childName) ? Children[childName].SelectNode(path.Substring(path.IndexOf('.') + 1)) : null;
        }

        public XmlDefinitionAttribute SelectAttribute(string path)
        {
            if (!path.Contains('@'))
                return Attributes.ContainsKey(path) ? Attributes[path] : null;

            var node = SelectNode(path.Substring(0, path.IndexOf('@')));
            return node != null ? node.SelectAttribute(path.Substring(path.IndexOf('@') + 1)) : null;
        }

        internal XmlDefinitionNode AddOrGetNode(string path)
        {
            if (!path.Contains('.'))
            {
                if (!Children.ContainsKey(path))
                    Children.Add(path, new XmlDefinitionNode {Name = path, Parent = this});
                return Children[path];
            }

            var childName = path.Substring(0, path.IndexOf('.'));
            if (!Children.ContainsKey(childName))
                Children.Add(childName, new XmlDefinitionNode {Name = childName, Parent = this});
            return Children[childName].AddOrGetNode(path.Substring(path.IndexOf('.') + 1));
        }

        internal XmlDefinitionAttribute AddOrGetAttribute(string path)
        {
            if (path.Contains('@'))
                return AddOrGetNode(path.Substring(0, path.IndexOf('@'))).AddOrGetAttribute(path.Substring(path.IndexOf('@') + 1));

            if (!Attributes.ContainsKey(path))
                Attributes.Add(path, new XmlDefinitionAttribute {Name = path, Node = this});
            return Attributes[path];
        }
    }
}