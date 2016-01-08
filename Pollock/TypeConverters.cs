using Pollock.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Pollock
{
    public class TypeConverters
    {
        private static IDictionary<Type, ITypeConverter> converters =
            new Dictionary<Type, ITypeConverter>();

        public static void RegisterEnumWithMemberRender<T>()
        {
            Register<T>(new EnumWithMemberRenderConverter<T>());
        }

        public static void Register<T>(ITypeConverter converter)
        {
            if (!converters.ContainsKey(typeof(T)))
            {
                TypeDescriptor.AddAttributes(typeof(T),
                    new TypeConverterAttribute(typeof(EnumTypeConverter<T>)));
            }

            converters[typeof(T)]= converter;
        }

        public static ITypeConverter Get<T>()
        {
            var type = typeof(T);
            if (!converters.ContainsKey(type))
            {
                RegisterEnumWithMemberRender<T>();
            }
            return converters[type];
        }
    }
}
