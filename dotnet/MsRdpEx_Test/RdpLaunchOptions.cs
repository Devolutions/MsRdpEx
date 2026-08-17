namespace MsRdpEx_AvaloniaApp;

public sealed class RdpLaunchOptionsTests
{
    private static readonly Func<string, string?> EmptyEnvironment = _ => null;

    [Fact]
    public void ParseSupportsAliasesAndInlineValues()
    {
        Guid classId = Guid.NewGuid();
        RdpLaunchOptions options = RdpLaunchOptions.Parse(
            [
                "/v:server.example.com",
                "--user", "alice",
                "-p=sample-password",
                "/d:CONTOSO",
                $"--clsid={classId}",
                "--axname", "msrdc",
                "--rdpex-dll-path", "custom.dll"
            ],
            EmptyEnvironment,
            _ => throw new InvalidOperationException());

        Assert.Null(options.Error);
        Assert.Equal("server.example.com", options.HostName);
        Assert.Equal("alice", options.UserName);
        Assert.Equal("sample-password", options.Password);
        Assert.Equal("CONTOSO", options.Domain);
        Assert.Equal(classId, options.ClassId);
        Assert.Equal("msrdc", options.AxName);
        Assert.Equal("custom.dll", options.RdpExDll);
    }

    [Fact]
    public void ParseReportsMissingSeparateValue()
    {
        RdpLaunchOptions atEnd = RdpLaunchOptions.Parse(
            ["--hostname"], EmptyEnvironment, _ => string.Empty);
        RdpLaunchOptions beforeOption = RdpLaunchOptions.Parse(
            ["--hostname", "--username=alice"], EmptyEnvironment, _ => string.Empty);

        Assert.Equal("Missing value for command-line option: --hostname", atEnd.Error);
        Assert.Equal("Missing value for command-line option: --hostname", beforeOption.Error);
    }

    [Fact]
    public void CommandLineOverridesRdpFileAndEnvironment()
    {
        const string rdpContents = """
            full address:s:file-host
            username:s:file-user
            domain:s:FILE-DOMAIN
            desktopwidth:i:1600
            desktopheight:i:900
            """;
        Dictionary<string, string> environment = new(StringComparer.OrdinalIgnoreCase)
        {
            ["RDP_HOSTNAME"] = "environment-host",
            ["RDP_USERNAME"] = "environment-user",
            ["RDP_PASSWORD"] = "environment-password",
            ["RDP_DOMAIN"] = "ENVIRONMENT-DOMAIN"
        };

        RdpLaunchOptions options = RdpLaunchOptions.Parse(
            ["settings.rdp", "--hostname=command-host", "--password", "command-password"],
            name => environment.GetValueOrDefault(name),
            fileName => fileName == "settings.rdp"
                ? rdpContents
                : throw new FileNotFoundException(fileName));

        Assert.Null(options.Error);
        Assert.Equal("command-host", options.HostName);
        Assert.Equal("file-user", options.UserName);
        Assert.Equal("command-password", options.Password);
        Assert.Equal("FILE-DOMAIN", options.Domain);
        Assert.Equal(1600, options.DesktopWidth);
        Assert.Equal(900, options.DesktopHeight);
        Assert.Equal(rdpContents, options.RdpFileContents);
    }

    [Fact]
    public void ExplicitRdpFileOverridesEnvironmentAndPositionalFiles()
    {
        Dictionary<string, string> environment = new(StringComparer.OrdinalIgnoreCase)
        {
            ["RDP_FILENAME"] = "environment.rdp"
        };

        RdpLaunchOptions options = RdpLaunchOptions.Parse(
            ["positional.rdp", "--file=command.rdp"],
            name => environment.GetValueOrDefault(name),
            fileName => $"full address:s:{fileName}");

        Assert.Null(options.Error);
        Assert.Equal("command.rdp", options.HostName);
    }

    [Fact]
    public void DynamicResolutionOverridesRdpFileDimensions()
    {
        const string rdpContents = """
            full address:s:dynamic-host
            desktopwidth:i:1920
            desktopheight:i:1080
            dynamic resolution:i:1
            """;

        RdpLaunchOptions options = RdpLaunchOptions.Parse(
            ["dynamic.rdp"], EmptyEnvironment, _ => rdpContents);

        Assert.Null(options.Error);
        Assert.Equal(0, options.DesktopWidth);
        Assert.Equal(0, options.DesktopHeight);
    }

    [Fact]
    public void InvalidClassIdReturnsDefaultAndError()
    {
        RdpLaunchOptions options = RdpLaunchOptions.Parse(
            ["--class-id=not-a-guid"], EmptyEnvironment, _ => string.Empty);

        Assert.Equal("Invalid RDP ActiveX class identifier: not-a-guid", options.Error);
        Assert.Equal(Devolutions.MsRdpEx.Avalonia.RdpClientView.DefaultClassId, options.ClassId);
    }

    [Fact]
    public void UnknownArgumentsReturnAnError()
    {
        RdpLaunchOptions option = RdpLaunchOptions.Parse(
            ["--unsupported=value"], EmptyEnvironment, _ => string.Empty);
        RdpLaunchOptions argument = RdpLaunchOptions.Parse(
            ["notes.txt"], EmptyEnvironment, _ => string.Empty);

        Assert.Equal("Unknown command-line option: --unsupported=value", option.Error);
        Assert.Equal("Unknown command-line argument: notes.txt", argument.Error);
    }
}
