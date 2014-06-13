using System;
using System.IO;
using System.Linq;

namespace LzEngine.Util
{
    public class XmlDefinition
    {
        private readonly XmlDefinitionNode _internalRoot = new XmlDefinitionNode();

        public XmlDefinition(string defPath)
        {
            foreach (var each in File.ReadLines(defPath).Where(e => e.Contains(':')).Select(e => e.Split(':'))
                                     .Select(e => new {Path = e[0].Trim(), Type = ParseType(e[1].Trim())}))
            {
                _internalRoot.AddOrGetAttribute(each.Path).ValueType = each.Type;
            }
        }

        public XmlDefinitionNode SelectNode(string path)
        {
            return _internalRoot.SelectNode(path);
        }

        public XmlDefinitionAttribute SelectAttribute(string path)
        {
            return _internalRoot.SelectAttribute(path);
        }

        private static Type ParseType(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "bool":
                    return typeof (bool);
                case "int":
                    return typeof (int);
                case "float":
                    return typeof (float);
                case "double":
                    return typeof (double);
                case "long":
                    return typeof (long);
                case "short":
                    return typeof (short);
            }
            return typeof (string);
        }
    }
}