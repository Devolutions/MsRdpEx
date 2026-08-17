using Devolutions.MsRdpEx.Avalonia;

namespace MsRdpEx_AvaloniaApp;

internal sealed record RdpLaunchOptions(
    string HostName,
    string UserName,
    string Password,
    string Domain,
    int DesktopWidth,
    int DesktopHeight,
    string RdpFileContents,
    string? Error,
    Guid ClassId,
    string AxName,
    string? RdpExDll)
{
    public static RdpLaunchOptions Parse(IReadOnlyList<string> arguments)
    {
        return Parse(
            arguments,
            Environment.GetEnvironmentVariable,
            File.ReadAllText);
    }

    internal static RdpLaunchOptions Parse(
        IReadOnlyList<string> arguments,
        Func<string, string?> getEnvironmentVariable,
        Func<string, string> readFile)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        ArgumentNullException.ThrowIfNull(getEnvironmentVariable);
        ArgumentNullException.ThrowIfNull(readFile);

        Dictionary<string, string> commandLine = new(StringComparer.OrdinalIgnoreCase);
        string? rdpFileName = null;
        string? error = null;

        for (int index = 0; index < arguments.Count; index++)
        {
            string argument = arguments[index];
            if (!TrySplitOption(argument, out string name, out string? value))
            {
                if (Path.GetExtension(argument).Equals(".rdp", StringComparison.OrdinalIgnoreCase) && rdpFileName is null)
                {
                    rdpFileName = argument;
                    continue;
                }

                error = $"Unknown command-line argument: {argument}";
                break;
            }

            string normalizedName = NormalizeOptionName(name);
            if (value is null)
            {
                if (++index >= arguments.Count ||
                    TrySplitOption(arguments[index], out _, out _))
                {
                    error = $"Missing value for command-line option: {argument}";
                    break;
                }

                value = arguments[index];
            }

            if (!IsSupportedOption(normalizedName))
            {
                error = $"Unknown command-line option: {argument}";
                break;
            }

            commandLine[normalizedName] = value;
        }

        string environmentHost = getEnvironmentVariable("RDP_HOSTNAME") ?? string.Empty;
        string environmentUser = getEnvironmentVariable("RDP_USERNAME") ?? string.Empty;
        string environmentPassword = getEnvironmentVariable("RDP_PASSWORD") ?? string.Empty;
        string environmentDomain = getEnvironmentVariable("RDP_DOMAIN") ?? string.Empty;
        string? configuredClassId = GetOption(commandLine, "class-id")
            ?? getEnvironmentVariable("RDP_CLASS_ID");
        string axName = GetOption(commandLine, "ax-name")
            ?? getEnvironmentVariable("RDP_AXNAME")
            ?? "mstsc";
        string? rdpExDll = GetOption(commandLine, "rdpex-dll")
            ?? getEnvironmentVariable("MSRDPEX_DLL");

        Guid classId = RdpClientView.DefaultClassId;
        if (!string.IsNullOrWhiteSpace(configuredClassId) &&
            !Guid.TryParse(configuredClassId, out classId))
        {
            error ??= $"Invalid RDP ActiveX class identifier: {configuredClassId}";
            classId = RdpClientView.DefaultClassId;
        }

        rdpFileName = GetOption(commandLine, "filename")
            ?? getEnvironmentVariable("RDP_FILENAME")
            ?? rdpFileName;

        RdpFileValues rdpFile = RdpFileValues.Empty;
        if (!string.IsNullOrWhiteSpace(rdpFileName))
        {
            try
            {
                rdpFile = ReadRdpFile(rdpFileName, readFile);
            }
            catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
            {
                error ??= $"Unable to open RDP file '{rdpFileName}': {exception.Message}";
            }
        }

        string hostName = GetOption(commandLine, "hostname") ?? rdpFile.HostName ?? environmentHost;
        string userName = GetOption(commandLine, "username") ?? rdpFile.UserName ?? environmentUser;
        string password = GetOption(commandLine, "password") ?? environmentPassword;
        string domain = GetOption(commandLine, "domain") ?? rdpFile.Domain ?? environmentDomain;

        int desktopWidth = rdpFile.DynamicResolution ? 0 : rdpFile.DesktopWidth;
        int desktopHeight = rdpFile.DynamicResolution ? 0 : rdpFile.DesktopHeight;

        return new RdpLaunchOptions(
            hostName,
            userName,
            password,
            domain,
            desktopWidth,
            desktopHeight,
            rdpFile.Contents,
            error,
            classId,
            axName,
            rdpExDll);
    }

    private static bool TrySplitOption(string argument, out string name, out string? value)
    {
        name = string.Empty;
        value = null;

        int prefixLength = argument.StartsWith("--", StringComparison.Ordinal) ? 2
            : argument.StartsWith("-", StringComparison.Ordinal) || argument.StartsWith("/", StringComparison.Ordinal) ? 1
            : 0;
        if (prefixLength == 0 || argument.Length <= prefixLength)
            return false;

        string option = argument[prefixLength..];
        int separator = option.IndexOfAny(['=', ':']);
        if (separator < 0)
        {
            name = option;
            return true;
        }

        name = option[..separator];
        value = option[(separator + 1)..];
        return true;
    }

    private static string NormalizeOptionName(string name)
    {
        string normalized = name.Trim().Replace('_', '-').ToLowerInvariant();
        return normalized switch
        {
            "rdp-hostname" or "server" or "v" => "hostname",
            "rdp-username" or "user" or "u" => "username",
            "rdp-password" or "p" => "password",
            "rdp-domain" or "d" => "domain",
            "rdp-filename" or "rdp-file" or "file" => "filename",
            "clsid" or "classid" => "class-id",
            "axname" => "ax-name",
            "rdpex" or "rdpex-dll-path" => "rdpex-dll",
            _ => normalized
        };
    }

    private static bool IsSupportedOption(string name)
    {
        return name is "hostname" or "username" or "password" or "domain" or "filename" or
            "class-id" or "ax-name" or "rdpex-dll";
    }

    private static string? GetOption(Dictionary<string, string> options, string name)
    {
        return options.TryGetValue(name, out string? value) ? value : null;
    }

    private static RdpFileValues ReadRdpFile(
        string fileName,
        Func<string, string> readFile)
    {
        string contents = readFile(fileName);
        string? hostName = null;
        string? userName = null;
        string? domain = null;
        int desktopWidth = 0;
        int desktopHeight = 0;
        bool dynamicResolution = false;

        foreach (string line in contents.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            int firstColon = line.IndexOf(':');
            if (firstColon <= 0 || firstColon + 2 >= line.Length || line[firstColon + 2] != ':')
                continue;

            string name = line[..firstColon].Trim();
            char type = line[firstColon + 1];
            string value = line[(firstColon + 3)..];

            if (type == 's')
            {
                switch (name.ToLowerInvariant())
                {
                    case "full address":
                        hostName = value;
                        break;
                    case "username":
                        userName = value;
                        break;
                    case "domain":
                        domain = value;
                        break;
                }
            }
            else if (type == 'i' && int.TryParse(value, out int integerValue))
            {
                switch (name.ToLowerInvariant())
                {
                    case "desktopwidth":
                        desktopWidth = integerValue;
                        break;
                    case "desktopheight":
                        desktopHeight = integerValue;
                        break;
                    case "dynamic resolution":
                        dynamicResolution = integerValue != 0;
                        break;
                }
            }
        }

        return new RdpFileValues(
            contents, hostName, userName, domain, desktopWidth, desktopHeight, dynamicResolution);
    }

    private sealed record RdpFileValues(
        string Contents,
        string? HostName,
        string? UserName,
        string? Domain,
        int DesktopWidth,
        int DesktopHeight,
        bool DynamicResolution)
    {
        public static RdpFileValues Empty { get; } = new(string.Empty, null, null, null, 0, 0, false);
    }
}
