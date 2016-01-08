using System;
using System.ComponentModel;
using System.Globalization;

namespace Pollock
{
    public class EnumTypeConverter<T> : TypeConverter where T: struct
    {
        private readonly ITypeConverter _converter;

        public EnumTypeConverter()
        {
            _converter = TypeConverters.Get<T>();
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
                return _converter.FromString(culture, (string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context,
           CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return _converter.ToString(culture, (T)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

}
