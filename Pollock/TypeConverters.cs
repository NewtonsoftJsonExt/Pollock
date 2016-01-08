using Pollock.Infrastructure;
using System;
using System.Collections.Generic;

namespace Pollock
{
    public class TypeConverters
    {
        private static IDictionary<Type, ITypeConverter> converters =
            new Dictionary<Type, ITypeConverter>();

        private static void RegisterEnumWithMemberRender<T>()
        {
            Register<T>(new EnumWithMemberRenderConverter<T>());
        }

        private static void Register<T>(ITypeConverter converter) 
        {
            converters[typeof(T)]= converter;
        }

        public static ITypeConverter Get<T>() where T: struct
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
