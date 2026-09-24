#include <MsRdpEx/RdpInstance.h>

#include <iostream>
#include <stdexcept>
#include <string>

static void Check(bool condition, const char* message)
{
    if (!condition) throw std::runtime_error(message);
}

class CountedUnknown : public IUnknown
{
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID iid, void** object) override
    {
        if (!object) return E_POINTER;
        *object = NULL;
        if (iid != IID_IUnknown) return E_NOINTERFACE;
        *object = static_cast<IUnknown*>(this);
        AddRef();
        return S_OK;
    }

    ULONG STDMETHODCALLTYPE AddRef() override { return ++refs; }
    ULONG STDMETHODCALLTYPE Release() override { return --refs; }

    ULONG refs = 1; // The test keeps its own reference; setters receive another.
};

int wmain(int argc, wchar_t** argv)
{
    try {
        Check(argc == 3, "Expected test DLL path and scenario");
        Check(SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", L"0") != 0,
            "Could not disable ActiveX hooks");
        Check(SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", L"0") != 0,
            "Could not disable logging");
        HMODULE module = LoadLibraryW(argv[1]);
        Check(module != NULL, "Could not load test DLL");
        using Create = IMsRdpExInstance*(*)();
        auto create = reinterpret_cast<Create>(
            GetProcAddress(module, "CreatePluginReferenceInstance"));
        Check(create != NULL, "Plugin instance fixture export missing");
        IMsRdpExInstance* instance = create();
        Check(instance != NULL, "Could not create plugin instance");

        CountedUnknown first;
        first.AddRef();
        Check(SUCCEEDED(instance->SetWTSPluginObject(&first)), "Initial plugin setter failed");
        void* borrowed = NULL;
        Check(SUCCEEDED(instance->GetWTSPluginObject(&borrowed)) && borrowed == &first,
            "Plugin getter did not return the registered pointer");

        const std::wstring scenario = argv[2];
        if (scenario == L"replacement") {
            CountedUnknown second;
            second.AddRef();
            Check(SUCCEEDED(instance->SetWTSPluginObject(&second)), "Replacement setter failed");
            Check(first.refs == 1, "Replacement did not release the previous plugin reference");
            Check(second.refs == 2, "Replacement did not retain the transferred reference");
            instance->Release();
            Check(second.refs == 1, "Instance destruction did not release replacement");
            Check(first.Release() == 0 && second.Release() == 0, "Test references not balanced");
        } else if (scenario == L"same-pointer") {
            first.AddRef();
            Check(SUCCEEDED(instance->SetWTSPluginObject(&first)), "Repeated setter failed");
            Check(first.refs == 2, "Repeating the pointer leaked a separate transferred reference");
            instance->Release();
            Check(first.refs == 1, "Instance destruction did not release the plugin");
            Check(first.Release() == 0, "Test reference not balanced");
        } else if (scenario == L"clearing") {
            Check(SUCCEEDED(instance->SetWTSPluginObject(NULL)), "Plugin clearing failed");
            Check(first.refs == 1, "Clearing did not release the plugin");
            borrowed = &first;
            Check(SUCCEEDED(instance->GetWTSPluginObject(&borrowed)) && !borrowed,
                "Clearing did not remove the plugin");
            instance->Release();
            Check(first.Release() == 0, "Test reference not balanced");
        } else if (scenario == L"destruction") {
            instance->Release();
            Check(first.refs == 1, "Instance destruction did not release the plugin");
            Check(first.Release() == 0, "Test reference not balanced");
        } else {
            throw std::runtime_error("Unknown plugin reference scenario");
        }

        Check(FreeLibrary(module) != FALSE, "Could not unload test DLL");
        std::wcout << L"PASS plugin reference: " << scenario << L'\n';
        return 0;
    } catch (const std::exception& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
