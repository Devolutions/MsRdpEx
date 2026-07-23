using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

internal static class Program
{
    private static Assembly LoadEmbeddedAssembly(string EmbeddedFileName)
    {
        using var stream = typeof(Program).Assembly.GetManifestResourceStream(EmbeddedFileName);

        if (stream is null)
            throw new InvalidOperationException($"Could not find embedded {EmbeddedFileName}.");

        var buffer = new byte[stream.Length];
        stream.ReadExactly(buffer);
        return Assembly.Load(buffer);
    }

    private static void Main()
    {
        var assembly = LoadEmbeddedAssembly("Interop.MSTSCLib.dll");
        var outpath = "../../../dotnet/AxInterop.MSTSCLib/Generated";

        // This assumes CMake was generated into a subdirectory of the git root.
        if (!Path.Exists(outpath))
            throw new InvalidOperationException("Invalid relative output path.");

        CodeGenerator.Generate(assembly, Path.Combine(outpath, "Enums.cs"), GeneratorMode.Enums);
        CodeGenerator.Generate(assembly, Path.Combine(outpath, "Classic.cs"), GeneratorMode.Classic);
        CodeGenerator.Generate(assembly, Path.Combine(outpath, "Desktop", "Interfaces.cs"), GeneratorMode.Desktop);
        CodeGenerator.Generate(assembly, Path.Combine(outpath, "NetCore", "Interfaces.cs"), GeneratorMode.NetCore);
        CodeGenerator.Generate(assembly, Path.Combine(outpath, "NetCore", "Redirection.cs"), GeneratorMode.Proxy);
    }
}

internal enum GeneratorMode
{
    Classic, Desktop, NetCore, Enums, Proxy
}

internal static class CodeGenerator
{
    private static GeneratorMode Mode;

    public static void Generate(string assembly, string outpath, GeneratorMode mode)
    {
        Generate(Assembly.LoadFrom(assembly), outpath, mode);
    }

    public static void Generate(Assembly assembly, string outpath, GeneratorMode mode)
    {
        Mode = mode;

        using var stream = new FileStream(outpath, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete);
        using var sw = new StreamWriter(stream);
        using var sb = new IndentedTextWriter(sw, "    ");

        if (Mode != GeneratorMode.Enums)
        {
            sb.WriteLine($"using System.Runtime.InteropServices;");
            if (Mode != GeneratorMode.Classic)
                sb.WriteLine($"using MsRdpEx.Interop;");
            sb.WriteLine();
        }
        sb.WriteLine($"namespace MSTSCLib");
        sb.WriteLine($"{{");
        sb.Indent++;
        foreach (var type in assembly.GetTypes())
        {
            if (type.Name.StartsWith("__"))
                continue;

            if (type.IsEnum && Mode == GeneratorMode.Enums)
            {
                sb.WriteLine($"public enum {type.Name}");
                sb.WriteLine($"{{");
                sb.Indent++;
                sb.WriteLine($"#region {type.Name}");
                foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly))
                {
                    sb.WriteLine($"{field.Name} = {field.GetRawConstantValue()},");
                }
                sb.WriteLine($"#endregion");
                sb.Indent--;
                sb.WriteLine($"}}");
            }

            if (type.IsInterface && Mode != GeneratorMode.Enums)
            {
                if (type.GetCustomAttribute<CoClassAttribute>() != null)
                    continue;

                if (type.GetCustomAttribute<ComEventInterfaceAttribute>() != null)
                    continue;

                var baseInterfaces = type.GetInterfaces().ToHashSet();
                baseInterfaces.ExceptWith(baseInterfaces.SelectMany(x => x.GetInterfaces()).ToList());

                string bt = "";
                foreach (var iface in baseInterfaces)
                {
                    if (bt == "")
                        bt = " : ";
                    else
                        bt += ", ";

                    bt += iface.Name;
                }

                bool hasInterfaceType = false;
                foreach (var attr in type.CustomAttributes.OrderBy(x => x.AttributeType.Name))
                {
                    switch (attr.AttributeType.Name)
                    {
                        case "ComImportAttribute": if (mode != GeneratorMode.NetCore && mode != GeneratorMode.Proxy) sb.WriteLine($"[ComImport]"); break;
                        case "GuidAttribute":
                            if (Mode == GeneratorMode.NetCore)
                                sb.WriteLine($"[ProxyGuid(\"{attr.ConstructorArguments.Single().Value}\", typeof({type.Name}Proxy))]");
                            else if (Mode != GeneratorMode.Proxy)
                                sb.WriteLine($"[Guid(\"{attr.ConstructorArguments.Single().Value}\")]");
                            break;
                        case "InterfaceTypeAttribute":
                            hasInterfaceType = true;
                            var kind = (ComInterfaceType)attr.ConstructorArguments.Single().Value!;
                            if (mode != GeneratorMode.NetCore && mode != GeneratorMode.Proxy)
                            {
                                sb.WriteLine($"[InterfaceType(ComInterfaceType.{kind})]");
                            }
                            else
                            {
                                if (kind == ComInterfaceType.InterfaceIsIDispatch)
                                {
                                    if (bt == "")
                                        bt = " /* : IDispatch */";
                                    kind = ComInterfaceType.InterfaceIsIUnknown;
                                }
                                //sb.WriteLine($"[InterfaceType(ComInterfaceType.{kind})]");
                            }
                            break;
                        case "TypeLibTypeAttribute": break;
                        case "ComConversionLossAttribute": break;
                        default: throw new NotImplementedException();
                    }
                }

                // dual mode is default when nothing is specified, NetCore needs to be explicit about it
                if ((mode == GeneratorMode.NetCore || mode == GeneratorMode.Proxy) && !hasInterfaceType)
                {
                    if (bt == "")
                        bt = " /* : IDispatch */";
                    //sb.WriteLine($"[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]");
                }

                if (Mode != GeneratorMode.Proxy)
                    sb.WriteLine($"public interface {type.Name}{bt}");
                else
                {
                    sb.WriteLine("[DynamicInterfaceCastableImplementation]");
                    sb.WriteLine($"internal interface {type.Name}Proxy : {type.Name} // {bt}");
                }
                sb.WriteLine($"{{");
                sb.Indent++;

                WriteInterface(sb, type, false);

                sb.Indent--;
                sb.WriteLine($"}}");
            }
        }
        sb.Indent--;
        sb.WriteLine($"}}");
    }

    private static void WriteInterface(IndentedTextWriter sb, Type type, bool inherited)
    {
        var baseInterfaces = type.GetInterfaces().ToHashSet();
        baseInterfaces.ExceptWith(baseInterfaces.SelectMany(x => x.GetInterfaces()).ToList());

        if (baseInterfaces.Count == 1 && Mode != GeneratorMode.NetCore)
            WriteInterface(sb, baseInterfaces.Single(), true);

        var baseMethods = baseInterfaces.SingleOrDefault()?.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(x => x.MetadataToken).ToArray() ?? Array.Empty<MethodInfo>();
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).OrderBy(x => x.MetadataToken).ToArray();
        Debug.Assert(methods.Length >= baseMethods.Length);
        for (int i = 0; i < baseMethods.Length; i++)
            Debug.Assert(methods[i].Name == baseMethods[i].Name);

        sb.WriteLine($"#region {type.Name}");

        var isnew = inherited && Mode != GeneratorMode.Proxy ? "new " : "";
        var pf = "";
        if (Mode == GeneratorMode.Proxy)
            pf = $"{type.Name}.";

        for (int i = baseMethods.Length; i < methods.Length; i++)
        {
            var method = methods[i];
            if (method.IsSpecialName)
            {
                ParameterInfo para;
                Type type1;
                string sig;

                if (method.Name.StartsWith("get_"))
                {
                    if (method.GetParameters().Length != 0)
                        goto normal;

                    para = method.ReturnParameter;
                    type1 = method.ReturnType;
                    sig = WriteAttributes(method) + WriteAttributes(para, "return: ") + "get";
                    if (Mode == GeneratorMode.Proxy)
                        sig += $" => {GetCast()}ProxyObject.Unpack<MsRdpEx.Interop.{type.Name}>(this).Get{fname()}(){sf()}";
                    sig += ";";
                }
                else if (method.Name.StartsWith("set_"))
                {
                    if (method.GetParameters().Length != 1)
                        goto normal;

                    para = method.GetParameters().Single();
                    type1 = para.ParameterType;
                    sig = WriteAttributes(method) + WriteAttributes(para, "param: ") + "set";
                    if (Mode == GeneratorMode.Proxy)
                        sig += $" => {GetCast()}ProxyObject.Unpack<MsRdpEx.Interop.{type.Name}>(this).Set{fname()}(value){sf()}";
                    sig += ";";
                }
                else
                    throw new NotImplementedException();

                if (type1.Name == "_RemotableHandle&")
                    type1 = typeof(nint);

                var name = method.Name.Substring(4);
                var prop = type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) ?? throw new NotImplementedException();
                if (prop.PropertyType != type1)
                {
                    Debug.WriteLine($"property {type.Name}.{prop.Name} type mismatch {prop.PropertyType.Name} - method returns {type1.Name}");
                    goto normal;
                }

                if (i + 1 < methods.Length && methods[i + 1].IsSpecialName && methods[i + 1].Name.Substring(4) == name)
                {
                    sig += " " + WriteAttributes(methods[i + 1]) + WriteAttributes(para, methods[i + 1].Name.StartsWith("get_") ? "return: " : "param: ") + methods[i + 1].Name.Remove(3);
                    if (Mode == GeneratorMode.Proxy)
                    {
                        if (methods[i + 1].Name.StartsWith("get_"))
                            sig += $" => {GetCast()}ProxyObject.Unpack<MsRdpEx.Interop.{type.Name}>(this).Get{fname()}(){sf()}";
                        else
                            sig += $" => {GetCast()}ProxyObject.Unpack<MsRdpEx.Interop.{type.Name}>(this).Set{fname()}(value){sf()}";
                    }
                    sig += ";";
                    i++;
                }

                string fname()
                {
                    var name = method.Name.Substring(4);
                    return char.ToUpperInvariant(name[0]).ToString() + name.Substring(1);
                }

                string sf() => type1.IsInterface ? ")" : "";

                string GetCast()
                {
                    if (type1.IsInterface)
                        return $"ProxyObject.Pack<{type1.Name}>(";
                    return "";
                }

                var propattr = WriteAttributes(prop);
                sb.WriteLine($"{propattr}{isnew}{FormatType(para)} {pf}{name} {{ {sig} }}");
                continue;
            }

        normal:
            sb.Write(WriteAttributes(method));
            WriteAttributes(sb, method.ReturnParameter, "return: ");
            sb.Write($"{isnew}{FormatType(method.ReturnParameter)} {pf}{method.Name}(");
            foreach (var arg in method.GetParameters())
            {
                if (arg.Position != 0)
                    sb.Write($", ");
                WriteAttributes(sb, arg, "");
                sb.Write($"{FormatType(arg)} {arg.Name}");
            }
            sb.Write($")");
            if (Mode != GeneratorMode.Proxy)
                sb.WriteLine(";");
            else
            {
                sb.Write($" => ");
                if (method.ReturnType.IsInterface)
                    sb.Write($"ProxyObject.Pack<{method.ReturnType.Name}>(");
                sb.Write($"ProxyObject.Unpack<MsRdpEx.Interop.{type.Name}>(this).{method.Name}(");
                foreach (var arg in method.GetParameters())
                {
                    if (arg.Position != 0) sb.Write($", ");
                    if (arg.IsOut && !arg.IsIn)
                        sb.Write("out ");
                    else if (arg.ParameterType.IsByRef && arg.ParameterType.Name != "_RemotableHandle&")
                        sb.Write(arg.IsOut ? "ref " : "in ");
                    sb.Write(arg.Name);
                }
                if (method.ReturnType.IsInterface)
                    sb.Write(")");
                sb.WriteLine(");");
            }
        }

        sb.WriteLine($"#endregion");
    }

    private static string WriteAttributes(PropertyInfo prop)
    {
        var sb = new StringBuilder();
        foreach (var attr in prop.CustomAttributes)
        {
            switch (attr.AttributeType.Name)
            {
                case "DispIdAttribute":
                    if (Mode != GeneratorMode.NetCore && Mode != GeneratorMode.Proxy)
                        sb.Append($"[DispId({attr.ConstructorArguments.Single().Value})] ");
                    break;
                case "ComAliasNameAttribute": break;
                default: throw new NotImplementedException();
            }
        }
        return sb.ToString();
    }
    private static string WriteAttributes(MethodInfo method)
    {
        var sb = new StringBuilder();
        foreach (var attr in method.CustomAttributes)
        {
            switch (attr.AttributeType.Name)
            {
                case "PreserveSigAttribute": sb.Append("[PreserveSig] "); break;
                case "DispIdAttribute":
                    // for properties we don't need to duplicate the DispId on the accessor
                    if (!method.IsSpecialName && Mode != GeneratorMode.NetCore && Mode != GeneratorMode.Proxy)
                        sb.Append($"[DispId({attr.ConstructorArguments.Single().Value})] ");
                    break;
                case "TypeLibFuncAttribute":
                    {
                        var value = (TypeLibFuncFlags)attr.ConstructorArguments.Single().Value!;
                        Debug.Assert(value == TypeLibFuncFlags.FHidden);
                        break;
                    }
                default: throw new NotImplementedException();
            }
        }
        return sb.ToString();
    }
    private static void WriteAttributes(IndentedTextWriter sb, ParameterInfo arg, string prefix)
    {
        sb.Write(WriteAttributes(arg, prefix));
    }
    private static string WriteAttributes(ParameterInfo arg, string prefix)
    {
        string sb = "";
        foreach (var attr in arg.CustomAttributes)
        {
            switch (attr.AttributeType.Name)
            {
                case "InAttribute":
                    switch (arg.ParameterType.Name)
                    {
                        case "Object":
                        case "Boolean":
                        case "Double":
                        case "Int32":
                        case "UInt32":
                        case "Int64":
                        case "UInt64":
                        case "String":
                        case "_RemotableHandle&":
                            break;
                        default:
                            if (arg.ParameterType.IsByRef)
                            {
                                sb += "[In] ";
                                break;
                            }
                            if (arg.ParameterType.IsEnum)
                                break;
                            throw new NotImplementedException();
                    }
                    break;
                case "OutAttribute":
                    if (!arg.ParameterType.IsByRef)
                        throw new InvalidOperationException();
                    break;
                case "ComAliasNameAttribute": break;
                case "MarshalAsAttribute":
                    var kind = (UnmanagedType)(int)attr.ConstructorArguments.Single().Value!;
                    if (kind == UnmanagedType.BStr)
                    {
                        if (Mode == GeneratorMode.Classic)
                            sb += ($"[{prefix}MarshalAs(UnmanagedType.{kind})] ");
                        if (Mode == GeneratorMode.Desktop)
                            sb += ($"[{prefix}MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BinaryStringMarshaler))] ");
                    }
                    else if (kind == UnmanagedType.Interface)
                    {
                        // don't need the attribute if the parameter is an interface anyways, only need it on values of type "object"
                        Debug.Assert(arg.ParameterType.IsInterface);
                    }
                    else
                    {
                        sb += ($"[{prefix}MarshalAs(UnmanagedType.{kind})] ");
                    }
                    break;
                default: throw new NotImplementedException();
            }
        }
        return sb;
    }

    private static string FormatType(ParameterInfo arg) => FormatType(arg.ParameterType, arg);
    private static string FormatType(Type type, ParameterInfo? arg)
    {
        switch (type.Name)
        {
            case "Void": return "void";
            case "Object": return "object";
            case "String": return (Mode == GeneratorMode.Classic) ? "string" : "BinaryString";
            case "Boolean": return "bool";
            case "Double": return "double";
            case "Int16": return "short";
            case "Int32": return "int";
            case "UInt32": return "uint";
            case "Int64": return "long";
            case "UInt64": return "ulong";
            case "IntPtr": return "nint";
            case "_RemotableHandle&": return "nint";
        }

        if (type.IsPointer)
            return FormatType(type.GetElementType()!, null) + "*";

        if (type.IsArray)
            return FormatType(type.GetElementType()!, null) + "[]";

        if (type.IsByRef)
        {
            if (arg is not null && arg.GetCustomAttribute<OutAttribute>() is not null)
                return "out " + FormatType(type.GetElementType()!, null);
            else
                return "ref " + FormatType(type.GetElementType()!, null);
        }

        if (type.IsInterface || type.IsEnum || type.IsClass)
            return type.Name;

        throw new NotImplementedException();
    }
}
