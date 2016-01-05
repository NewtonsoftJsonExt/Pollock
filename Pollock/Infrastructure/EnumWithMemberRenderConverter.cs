using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Pollock.Infrastructure
{
    class EnumWithMemberRenderConverter<T> : ITypeConverter
    {
        private readonly EnumNameMapping<T> mapping=new NameFromMember<T>();
        private class NameFromMember<TA> : EnumNameMapping<TA>
        {
            protected override Dictionary<TA, string> TypeToStringMappingAbstract()
            {
                var values = Enum.GetValues(typeof(TA)).Cast<TA>();
                return values.ToDictionary(v => v, v => GetMemberValue(v));
            }

            private string GetMemberValue(TA v)
            {
                var memInfo = typeof(TA).GetMember(v.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute),
                    false);
                if (attributes.Length == 0)
                {
                    return v.ToString();
                }
                return ((EnumMemberAttribute)attributes[0]).Value;
            }
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