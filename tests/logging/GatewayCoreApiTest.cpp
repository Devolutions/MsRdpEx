#include <MsRdpEx/RdpCoreApi.h>
#include <iostream>
#include <stdexcept>

static void Check(bool value, const char* message)
{
    if (!value) throw std::runtime_error(message);
}

int wmain(int argc, wchar_t** argv)
{
    try {
        Check(argc == 3, "Expected absolute DLL path and default/off/hooks-off mode");
        bool expected = wcscmp(argv[2], L"off") != 0;
        bool hooksOff = wcscmp(argv[2], L"hooks-off") == 0;
        SetEnvironmentVariableW(L"MSRDPEX_GATEWAY_UNIQUE_BINDING", expected ? NULL : L"0");
        SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", hooksOff ? L"0" : L"1");
        SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", L"0");
        for (int cycle = 0; cycle < 2; ++cycle) {
            HMODULE module = LoadLibraryW(argv[1]);
            Check(module != NULL, "DLL load failed");
            if (hooksOff) SetEnvironmentVariableW(L"MSRDPEX_GATEWAY_UNIQUE_BINDING", L"0");
            using Create = HRESULT(CDECL*)(REFIID, void**);
            auto create = (Create)GetProcAddress(module, "MsRdpEx_CreateInstance");
            Check(create != NULL, "Factory missing");
            IMsRdpExCoreApi *first = NULL, *second = NULL;
            Check(SUCCEEDED(create(__uuidof(IMsRdpExCoreApi), (void**)&first)), "Original core API failed");
            Check(SUCCEEDED(create(__uuidof(IMsRdpExCoreApi), (void**)&second)), "Second core API failed");
            Check(SUCCEEDED(first->Load()) && first->GetMsRdpExDllPath(), "Original vtable changed");
            IMsRdpExGatewaySettings *a = NULL, *b = NULL;
            Check(SUCCEEDED(first->QueryInterface(__uuidof(IMsRdpExGatewaySettings), (void**)&a)), "New interface missing");
            Check(SUCCEEDED(second->QueryInterface(__uuidof(IMsRdpExGatewaySettings), (void**)&b)), "New interface missing on second object");
            Check(a->GetGatewayIsolationEnabled() == expected, "Startup default/override incorrect");
            first->SetAxHookEnabled(true);
            IUnknown *identity = NULL, *original = NULL;
            Check(SUCCEEDED(a->QueryInterface(IID_IUnknown, (void**)&identity)), "Extension IUnknown failed");
            Check(SUCCEEDED(first->QueryInterface(IID_IUnknown, (void**)&original)), "Core IUnknown failed");
            Check(identity == original, "COM identity differs across interfaces");
            identity->Release(); original->Release();
            a->SetGatewayIsolationEnabled(false);
            Check(!b->GetGatewayIsolationEnabled(), "Property not process-wide");
            SetEnvironmentVariableW(L"MSRDPEX_GATEWAY_UNIQUE_BINDING", L"1");
            first->SetAxHookEnabled(false); first->SetAxHookEnabled(true);
            Check(!a->GetGatewayIsolationEnabled(), "Hook toggle reset the property");
            b->SetGatewayIsolationEnabled(true);
            Check(a->GetGatewayIsolationEnabled(), "Property did not reenable");
            a->Release(); b->Release();
            first->SetAxHookEnabled(false);
            first->Unload(); first->Release(); second->Release();
            Check(FreeLibrary(module) != FALSE, "DLL free failed");
            HMODULE remaining = NULL;
            Check(!GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS |
                GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT, (LPCWSTR)create, &remaining), "DLL remains pinned");
            SetEnvironmentVariableW(L"MSRDPEX_GATEWAY_UNIQUE_BINDING", expected ? NULL : L"0");
        }
        std::cout << "Gateway core API and unloading tests passed\n";
        return 0;
    } catch (const std::exception& e) { std::cerr << e.what() << '\n'; return 1; }
}
