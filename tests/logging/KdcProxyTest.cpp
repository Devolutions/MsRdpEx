#include <windows.h>

#include <iostream>
#include <stdexcept>

static void Check(bool condition, const char* message)
{
    if (!condition) throw std::runtime_error(message);
}

int wmain(int argc, wchar_t** argv)
{
    try {
        Check(argc == 2, "Expected test DLL path");
        Check(SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", L"0") != 0,
            "Could not disable ActiveX hooks");
        Check(SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", L"0") != 0,
            "Could not disable logging");
        HMODULE module = LoadLibraryW(argv[1]);
        Check(module != NULL, "Could not load test DLL");
        using Convert = bool(*)(const char*, const char*);
        auto convert = reinterpret_cast<Convert>(
            GetProcAddress(module, "CheckKdcProxyUrlToName"));
        Check(convert != NULL, "KDC proxy fixture export missing");

        const struct {
            const char* url;
            const char* expected;
        } cases[] = {
            { "https://gateway/jet/KdcProxy/token", "gateway:443:jet/KdcProxy/token" },
            { "http://gateway/jet/KdcProxy/token", "gateway:80:jet/KdcProxy/token" },
            { "HTTPS://gateway/KdcProxy", "gateway:443:KdcProxy" },
            { "HTTP://gateway/KdcProxy", "gateway:80:KdcProxy" },
            { "https://gateway:8443/jet/KdcProxy/token", "gateway:8443:jet/KdcProxy/token" },
            { "http://gateway:8080/KdcProxy", "gateway:8080:KdcProxy" },
            { "https://gateway:443/KdcProxy", "gateway:443:KdcProxy" },
            { "https://gateway/", "gateway:443:" },
            { "http://gateway/", "gateway:80:" },
            { "https://gateway/path:segment/more", "gateway:443:path:segment/more" },
            { "https://[2001:db8::1]/KdcProxy", "[2001:db8::1]:443:KdcProxy" },
            { "http://[2001:db8::1]/KdcProxy", "[2001:db8::1]:80:KdcProxy" },
            { "https://[2001:db8::1]:8443/KdcProxy", "[2001:db8::1]:8443:KdcProxy" },
            { "https://gateway", "gateway" },
            { "http://gateway", "gateway" },
            { "https://gateway:8443", "gateway:8443" },
            { "https://[2001:db8::1]", "[2001:db8::1]" },
            { "custom://gateway/path", "gateway:path" },
            { "gateway:443:KdcProxy", NULL },
            { "gateway/KdcProxy", NULL },
            { "gateway", NULL },
            { "", NULL },
            { NULL, NULL },
        };

        size_t failures = 0;
        for (size_t i = 0; i < ARRAYSIZE(cases); ++i) {
            if (!convert(cases[i].url, cases[i].expected)) {
                std::cerr << "FAIL KDC proxy case " << i << '\n';
                ++failures;
            }
        }
        Check(FreeLibrary(module) != FALSE, "Could not unload test DLL");
        Check(failures == 0, "KDC proxy conversion failed");
        std::cout << "PASS KDC proxy: " << ARRAYSIZE(cases) << " cases\n";
        return 0;
    } catch (const std::exception& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
