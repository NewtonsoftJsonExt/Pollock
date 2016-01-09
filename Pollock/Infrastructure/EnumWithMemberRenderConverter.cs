using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Pollock.Infrastructure
{
    class EnumWithMemberRenderConverter<T> : ITypeConverter
    {
        private readonly EnumNameMapping<T> mapping;
        public EnumWithMemberRenderConverter()
        {
            mapping = new EnumNameMapping<T>(TypeToStringMapping());
        }

        private static Dictionary<T, string> TypeToStringMapping()
        {
            var values = Enum.GetValues(typeof(T)).Cast<T>();
            return values.ToDictionary(v => v, v => GetMemberValue(v));
        }

        private static string GetMemberValue(T v)
        {
            var memInfo = typeof(T).GetMember(v.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute),
                false);
            if (attributes.Length == 0)
            {
                return v.ToString();
            }
            return ((EnumMemberAttribute)attributes[0]).Value;
        }

        public object FromString(CultureInfo culture, string value)
        {
            return mapping.Parse(value);
        }

        public string ToString(CultureInfo culture, object value)
        {
            return mapping.ToString((T)value);
        }
    }
}