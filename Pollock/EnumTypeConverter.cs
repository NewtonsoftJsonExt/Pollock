using Pollock.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Pollock
{
    public class EnumTypeConverter<T> : TypeConverter where T: struct
    {
        private readonly EnumNameMapping<T> mapping;

        public EnumTypeConverter()
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
        
        public override bool CanConvertFrom(ITypeDescriptorContext context,
   Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context,
           CultureInfo culture, object value)
        {
            if (value is string)
            {
                return mapping.Parse((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return mapping.ToString((T)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

}
