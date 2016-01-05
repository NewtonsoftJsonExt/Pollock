using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Pollock
{
    public class EnumJsonConverter<T> : JsonConverter
    {
        private readonly ITypeConverter _converter;

        public EnumJsonConverter()
        {
            _converter = TypeConverters.Get<T>();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if ((objectType == typeof(T)))
            {
                var val = serializer.Deserialize<string>(reader);
                if (string.IsNullOrEmpty(val))
                {
                    return default(T);
                }
                return ParseValue(val);
            }
            throw new Exception("Cant handle type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is T)
            {
                writer.WriteValue(ValueToString((T)value));
            }
            else
            {
                throw new Exception("not implemented");
            }
        }

        protected virtual T ParseValue(string val)
        {
            return (T)_converter.FromString(CultureInfo.CurrentCulture, val);
        }

        protected virtual string ValueToString(T value)
        {
            return _converter.ToString(CultureInfo.CurrentCulture, value);
        }
    }
}
