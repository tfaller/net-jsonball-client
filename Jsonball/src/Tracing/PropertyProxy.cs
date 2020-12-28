using System;
using System.Reflection;
using System.Text.Json.Serialization;

namespace TFaller.Jsonball.Client.Tracing
{
    /// <summary>
    /// Traces property usage.
    /// </summary>
    /// <typeparam name="T">The interface that acts as the proxy interface</typeparam>
    public class PropertyProxy<T> : DispatchProxy where T : ITraceable
    {
        private T _target;
        private Tracer _tracer;

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (!targetMethod.IsSpecialName || targetMethod.MemberType != MemberTypes.Method || !targetMethod.Name.StartsWith("get_"))
            {
                // Not a getter ... we only trace property getter
                return targetMethod.Invoke(this._target, args);
            }

            // trace the property access
            var t = _tracer.Trace(getPropertyName(targetMethod.Name.Substring("get_".Length)));

            // get the actual property value
            var propValue = targetMethod.Invoke(this._target, args);

            return ProxyFactory.CreateProxy(targetMethod.ReturnType, propValue, t);
        }

        private string getPropertyName(string propName)
        {
            var prop = _target.GetType().GetProperty(propName);
            var nameAttr = prop.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (nameAttr != null)
            {
                return nameAttr.Name;
            }
            return prop.Name;
        }

        /// <summary>
        /// Creates a generic interface proxy to trace getter calls of an
        /// interface. 
        /// </summary>
        /// <param name="target">The proxy target</param>
        /// <param name="tracer">The trace which logs the calls</param>
        /// <returns></returns>
        public static T Create(T target, Tracer tracer)
        {
            T proxy = DispatchProxy.Create<T, PropertyProxy<T>>();
            PropertyProxy<T> proxyTyped = ((PropertyProxy<T>)(object)proxy);
            proxyTyped._target = target;
            proxyTyped._tracer = tracer;
            return proxy;
        }
    }
}