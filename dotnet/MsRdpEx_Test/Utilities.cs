using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MsRdpEx.Tests
{
    internal static partial class Utilities
    {
        public static string Join(this IEnumerable<string> values, string? separator) => string.Join(separator, values);

        public static IEnumerable<T1> Unpack<T1>(this IEnumerable<object[]> values)
        {
            return values.Select(static values =>
            {
                if (values is null || values.Length != 1)
                    throw new InvalidOperationException();

                return (T1)values[0];
            });
        }

        public static IEnumerable<(T1, T2)> Unpack<T1, T2>(this IEnumerable<object[]> values)
        {
            return values.Select(static values =>
            {
                if (values is null || values.Length != 2)
                    throw new InvalidOperationException();

                return ((T1)values[0], (T2)values[1]);
            });
        }
    }

    #region Metadata Helpers

    public sealed class MethodNameComparer : IEqualityComparer<MethodInfo>
    {
        public static MethodNameComparer Instance { get; } = new();

        public bool Equals(MethodInfo? lhs, MethodInfo? rhs)
        {
            return (lhs is null) == (rhs is null) && lhs?.Name == rhs?.Name;
        }

        public int GetHashCode([DisallowNull] MethodInfo obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public sealed class MethodDesc
    {
        public MethodDesc(MethodInfo Method)
        {
            Type = Method.DeclaringType!;
            Name = Method.Name;

            foreach (var parameter in Method.GetParameters())
                Parameters.Add(new(parameter));

            if (Method.ReturnType != typeof(void))
                Parameters.Add(new(Method.ReturnParameter));
        }

        public Type Type { get; }
        public string TypeName => Type.Name;
        public string Name { get; }
        public List<ParameterDesc> Parameters { get; } = new();
        public override string ToString() => $"{TypeName}.{Name}({Parameters.Select(x => x.ToString()).Join(", ")})";
    }

    public sealed class ParameterDesc
    {
        public ParameterDesc(ParameterInfo info)
        {
            Name = info.Name ?? "";
            Type = info.ParameterType;
            Attributes.AddRange(info.GetCustomAttributes());
            Mode = Utilities.GetParameterMode(info.IsIn, info.IsOut);

            Assert.False(info.IsRetval);
            Assert.False(info.IsOptional);
            Assert.False(info.HasDefaultValue);
            Assert.NotEqual(typeof(void), Type);

            if (info.Position == -1)
            {
                Assert.Equal(ParameterMode.Undefined, Mode);
                Assert.False(Type.IsByRef);
                Assert.True(string.IsNullOrEmpty(Name));

                Name = "result";
                Type = Type.MakeByRefType();
                Mode = ParameterMode.Output;
            }
            else if (string.IsNullOrEmpty(Name))
            {
                Name = "value";
            }

            if (Mode.IsOutputOrReference())
                Assert.True(Type.IsByRef || Type.IsPointer);

            if (Mode == ParameterMode.Undefined)
                Mode = (Type.IsByRef || Type.IsPointer) ? ParameterMode.Reference : ParameterMode.Input;
        }

        public TAttribute? GetCustomAttribute<TAttribute>() where TAttribute : Attribute => Attributes.OfType<TAttribute>().SingleOrDefault();

        public string Name { get; set; }
        public Type Type { get; set; }
        public string TypeName => Type.Name;
        public List<Attribute> Attributes { get; } = new();
        public ParameterMode Mode { get; set; }

        public override string ToString() => $"{(Mode.IsOutputOrReference() ? "[out] " : "")}{TypeName} {Name}";
    }

    public enum ParameterMode { Undefined, Input, Output, Reference }

    internal static partial class Utilities
    {
        public static ParameterMode GetParameterMode(bool input, bool output)
        {
            switch ((input, output))
            {
                case (false, false): return ParameterMode.Undefined;
                case (true, false): return ParameterMode.Input;
                case (false, true): return ParameterMode.Output;
                case (true, true): return ParameterMode.Reference;
                default: throw new InvalidOperationException();
            }
        }

        public static bool IsOutputOrReference(this ParameterMode mode) => mode == ParameterMode.Output || mode == ParameterMode.Reference;
    }

    #endregion
}
