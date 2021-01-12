using System;
using System.Collections.Generic;

namespace TFaller.Jsonball.Client.Tracing
{
    /// <summary>
    /// A helper to genereate proxies to track usage of values.
    /// </summary>
    public static class ProxyFactory
    {
        public static T CreateProxy<T>(T target, Tracer tracer) where T : ITraceable
        {
            return (T)CreateProxy(typeof(T), target, tracer);
        }

        internal static object CreateProxy(Type type, object target, Tracer tracer)
        {
            // some types don't need a proxy, just return the plain value
            if (target == null || type.IsPrimitive || type == typeof(string) || type.IsEnum)
            {
                return target;
            }

            if (type.IsGenericType)
            {
                var typeDef = type.GetGenericTypeDefinition();
                if (typeDef == typeof(IReadOnlyList<>))
                {
                    // it's a readonly list ... create a proxy.
                    return Activator.CreateInstance(typeof(ReadOnlyListProxy<>)
                        .MakeGenericType(type.GenericTypeArguments[0]), new object[] { target, tracer });
                }
                else if (typeDef == typeof(Nullable<>))
                {
                    // it's nullable ... which is not null. unpack value and proxy it.
                    return CreateProxy(type.GenericTypeArguments[0], type.GetMethod("get_Value")
                        .Invoke(target, null), tracer);
                }
            }

            if (!(typeof(ITraceable)).IsAssignableFrom(type))
            {
                // We can't create a proxy for this type.
                // We could just return the plain object ... but it is possible that we would
                // unintentionally not trace a object type we should trace.
                throw new Exception(String.Format("Type {0} does not implement ITraceable", type.Name));
            }

            if (!type.IsInterface)
            {
                throw new Exception(String.Format("Type {0} must be an interface", type.Name));
            }

            return typeof(PropertyProxy<>)
                .MakeGenericType(type)
                .GetMethod("Create")
                .Invoke(null, new object[] { target, tracer });
        }
    }
}