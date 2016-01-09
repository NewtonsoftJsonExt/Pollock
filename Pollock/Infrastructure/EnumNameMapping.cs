using System;
using System.Collections.Generic;
using System.Linq;

namespace Pollock.Infrastructure
{
    class EnumNameMapping<T>
    {
        private Dictionary<T, string> _typeToStringMapping;
        private Dictionary<string, T> _stringToType;

        public EnumNameMapping(Dictionary<T, string> typeToStringMapping)
        {
            _typeToStringMapping = typeToStringMapping;
            _stringToType = typeToStringMapping.ToDictionary(kv => kv.Value, kv => kv.Key);
        }
        private T ParseAsStandardEnum(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public T Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            if (_stringToType.ContainsKey(value))
            {
                return _stringToType[value];
            }
            // if not found, then try to parse using standard enum parse:
            return ParseAsStandardEnum(value);
        }

        public string ToString(T value)
        {
            if (value == null) { return ""; }
            if (!_typeToStringMapping.ContainsKey(value))
            {
                return value.ToString();
            }
            return _typeToStringMapping[value];
        }
    }
}
