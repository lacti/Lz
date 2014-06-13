using System;

namespace LzEngine.Util
{
    public class XmlDefinitionAttribute
    {
        public string Name { get; internal set; }
        public XmlDefinitionNode Node { get; internal set; }
        public Type ValueType { get; internal set; }

        public object ReadValue(object value)
        {
            return Convert.ChangeType(value, ValueType);
        }
    }
}