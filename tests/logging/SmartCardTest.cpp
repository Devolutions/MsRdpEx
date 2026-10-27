#include <windows.h>
#include <filesystem>
#include <iostream>
#include <stdexcept>

int wmain(int argc, wchar_t** argv)
{
    try {
        if (argc != 3 || !std::filesystem::path(argv[1]).is_absolute())
            throw std::runtime_error("Expected absolute fixture DLL path and scenario");
        SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", L"0");
        SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", L"0");
        SetEnvironmentVariableW(L"MSRDPEX_SSPI_DUMP", L"0");
        SetEnvironmentVariableW(L"MSRDPEX_PCAP_DUMP", L"0");
        SetEnvironmentVariableW(L"MSRDPEX_SSPI_SMARTCARD_CONTEXT", NULL);
        char scenario[64] = {};
        if (!WideCharToMultiByte(CP_UTF8, 0, argv[2], -1, scenario, sizeof(scenario), NULL, NULL))
            throw std::runtime_error("Invalid scenario");
        const bool hadWinSCard = GetModuleHandleW(L"WinSCard.dll") != NULL;
        const bool hadNCrypt = GetModuleHandleW(L"ncrypt.dll") != NULL;
        HMODULE module = LoadLibraryW(argv[1]);
        if (!module)
            throw std::runtime_error("Cannot load smart-card fixture");
        if (strcmp(scenario, "context-default-load") == 0) {
            if ((!hadWinSCard && GetModuleHandleW(L"WinSCard.dll")) ||
                (!hadNCrypt && GetModuleHandleW(L"ncrypt.dll")))
                throw std::runtime_error("Default DLL load preloaded an experimental provider dependency");
            FreeLibrary(module);
            std::cout << "PASS smart-card " << scenario << '\n';
            return 0;
        }
        using RunTest = const char*(*)(const char*);
        const char* entry = "RunSmartCardSspiTest";
        if (strcmp(scenario, "settings") == 0 || strcmp(scenario, "context-settings") == 0)
            entry = "RunSmartCardSettingsTest";
        else if (strcmp(scenario, "context-resolver") == 0 || strcmp(scenario, "context-modes") == 0)
            entry = "RunSmartCardContextTest";
        auto run = reinterpret_cast<RunTest>(GetProcAddress(module, entry));
        if (!run)
            throw std::runtime_error("Missing private smart-card test entry point");
        const char* failure = run(scenario);
        if (failure)
            throw std::runtime_error(failure);
        FreeLibrary(module);
        std::cout << "PASS smart-card " << scenario << '\n';
        return 0;
    } catch (const std::exception& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
