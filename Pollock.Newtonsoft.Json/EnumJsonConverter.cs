using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Pollock
{
    public class EnumJsonConverter<T> : JsonConverter where T : struct
    {
        private readonly TypeConverter _converter;

        public EnumJsonConverter()
        {
            _converter = TypeDescriptor.GetConverter(typeof(T));
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
            return (T)_converter.ConvertFrom(val);
        }

        protected virtual string ValueToString(T value)
        {
            return (string)_converter.ConvertTo(value, typeof(string));
        }
    }
}
