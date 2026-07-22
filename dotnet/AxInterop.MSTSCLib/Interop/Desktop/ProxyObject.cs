#nullable enable

namespace MSTSCLib
{
    public static class ProxyObject
    {
        public static object? Pack(object? value) => value;
        public static object? Unpack(object? value) => value;
    }
}
