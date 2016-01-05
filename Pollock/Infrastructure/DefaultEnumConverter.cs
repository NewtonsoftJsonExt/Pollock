using System;
using System.Globalization;

namespace Pollock.Infrastructure
{
    class DefaultEnumConverter<T> : ITypeConverter
    {
        public object FromString(CultureInfo culture, string value)
        {
            return Enum.Parse(typeof(T), value);
        }

        public string ToString(CultureInfo culture, object value)
        {
            return ((T)value).ToString();
        }
    }
}
