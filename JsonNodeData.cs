using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonEditor
{
    class JsonNodeData
    {
        private string _key;
        private ValueType _type;
        private string _value;

        public JsonNodeData(string key, ValueType type, string value)
        {
            _key = key;
            _type = type;
            _value = value;
        }

        public string GetKey()
        {
            return _key;
        }

        public ValueType GetValueType()
        {
            return _type;
        }

        public string GetValue()
        {
            return _value;
        }
    }
}
