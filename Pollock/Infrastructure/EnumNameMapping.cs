using System;
using System.Collections.Generic;
using System.Linq;

namespace Pollock.Infrastructure
{
    abstract class EnumNameMapping<T>
    {
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
            var dic = StringToTypeMapping();
            if (dic.ContainsKey(value))
            {
                return dic[value];
            }
            // if not found, then try to parse using standard enum parse:
            return ParseAsStandardEnum(value);
        }

        protected abstract Dictionary<T, string> TypeToStringMappingAbstract();
        private Dictionary<T, string> _TypeToStringMapping;
        protected Dictionary<T, string> TypeToStringMapping()
        {
            if (_TypeToStringMapping == null)
            {
                _TypeToStringMapping = TypeToStringMappingAbstract();
            }
            return _TypeToStringMapping;
        }

        private Dictionary<string, T> _stringToManualItemMarkupType;
        protected Dictionary<string, T> StringToTypeMapping()
        {
            if (_stringToManualItemMarkupType == null)// no need for thread safety here
            {
                _stringToManualItemMarkupType = TypeToStringMapping().ToDictionary(kv => kv.Value, kv => kv.Key);
            }
            return _stringToManualItemMarkupType;
        }

        public string ToString(T value)
        {
            if (value == null) { return ""; }
            var manualItemMarkupType = TypeToStringMapping();
            if (!manualItemMarkupType.ContainsKey(value))
            {
                return value.ToString();
            }
            return manualItemMarkupType[value];
        }
    }
}
