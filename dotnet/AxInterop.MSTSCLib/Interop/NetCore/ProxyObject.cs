using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

#nullable enable

namespace MSTSCLib
{
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    internal sealed class ProxyGuidAttribute(string guid, Type type) : Attribute
    {
        public Guid IID { get; } = new(guid);
        public Type Type { get; } = type;
    }

    public sealed class ProxyObject : IDynamicInterfaceCastable
    {
        public static T? Unpack<T>(object? obj) where T : class
        {
            return (T?)Unpack(obj);
        }

        public static object? Unpack(object? obj)
        {
            return obj is null ? null : ((ProxyObject)obj).obj;
        }

        public static T? Pack<T>(object? obj) where T : class
        {
            return (T?)Pack(obj);
        }

        public static object? Pack(object? obj)
        {
            if (obj is null)
                return null;

            if (obj is ProxyObject proxy)
                return proxy;

            if (obj is ComObject comObject)
                return new ProxyObject(comObject);

            if (Marshal.IsComObject(obj))
            {
                var pUnk = Marshal.GetIUnknownForObject(obj);
                try { return new ProxyObject(GetComObjectForIUnknown(pUnk)); }
                finally { Marshal.Release(pUnk); }

                static unsafe object GetComObjectForIUnknown(nint pUnk) => ComInterfaceMarshaller<object>.ConvertToManaged((void*)pUnk)!;
            }

            throw new InvalidOperationException("Not a COM object.");
        }

        private readonly object obj;

        private ProxyObject(object obj)
        {
            this.obj = obj;
        }

        bool IDynamicInterfaceCastable.IsInterfaceImplemented(RuntimeTypeHandle interfaceType, bool throwIfNotImplemented)
        {
            return Type.GetTypeFromHandle(interfaceType)?.GetCustomAttribute<ProxyGuidAttribute>() is not null;
        }

        RuntimeTypeHandle IDynamicInterfaceCastable.GetInterfaceImplementation(RuntimeTypeHandle interfaceType)
        {
            return Type.GetTypeFromHandle(interfaceType)?.GetCustomAttribute<ProxyGuidAttribute>()?.Type.TypeHandle ?? default;
        }
    }
}
