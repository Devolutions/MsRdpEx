#include <MsRdpEx/RdpInstance.h>

#include <iostream>
#include <stdexcept>
#include <string>
#include <tsvirtualchannels.h>

static void Check(bool condition, const char* message)
{
    if (!condition) throw std::runtime_error(message);
}

class CountedPlugin : public IWTSPlugin
{
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID iid, void** object) override
    {
        if (!object) return E_POINTER;
        *object = NULL;
        if (iid != IID_IUnknown && iid != IID_IWTSPlugin) return E_NOINTERFACE;
        if (replaceOnQuery) {
            replaceOnQuery = false;
            if (FAILED(instance->SetWTSPluginObject(replacement))) return E_FAIL;
            refsAfterReplacement = refs;
        }
        *object = static_cast<IWTSPlugin*>(this);
        AddRef();
        return S_OK;
    }

    ULONG STDMETHODCALLTYPE AddRef() override { return ++refs; }
    ULONG STDMETHODCALLTYPE Release() override { return --refs; }
    HRESULT STDMETHODCALLTYPE Initialize(IWTSVirtualChannelManager*) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE Connected() override { return S_OK; }
    HRESULT STDMETHODCALLTYPE Disconnected(DWORD) override { return S_OK; }
    HRESULT STDMETHODCALLTYPE Terminated() override { return S_OK; }

    ULONG refs = 1;
    IMsRdpExInstance* instance = NULL;
    IWTSPlugin* replacement = NULL;
    bool replaceOnQuery = false;
    ULONG refsAfterReplacement = 0;
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
        using Register = bool(*)(IMsRdpExInstance*);
        using Factory = HRESULT(*)(REFCLSID, IClassFactory**);
        auto create = reinterpret_cast<Create>(
            GetProcAddress(module, "CreatePluginReferenceInstance"));
        auto registerInstance = reinterpret_cast<Register>(
            GetProcAddress(module, "RegisterPluginReferenceInstance"));
        auto unregisterInstance = reinterpret_cast<Register>(
            GetProcAddress(module, "UnregisterPluginReferenceInstance"));
        auto createFactory = reinterpret_cast<Factory>(
            GetProcAddress(module, "CreatePluginReferenceFactory"));
        Check(create && registerInstance && unregisterInstance && createFactory,
            "DVC factory fixture exports missing");

        IMsRdpExInstance* instance = create();
        Check(instance != NULL, "Could not create plugin instance");
        GUID sessionId = {};
        Check(SUCCEEDED(instance->GetSessionId(&sessionId)), "Could not get session ID");
        Check(registerInstance(instance), "Could not register plugin instance");

        IClassFactory* factory = NULL;
        Check(SUCCEEDED(createFactory(sessionId, &factory)) && factory,
            "Could not create DVC plugin class factory");
        IUnknown* identity = NULL;
        Check(SUCCEEDED(factory->QueryInterface(IID_IUnknown, (void**)&identity)) && identity,
            "Class factory does not support IUnknown");
        Check(identity->Release() == 1, "Class factory initial reference is unbalanced");
        void* unsupported = factory;
        Check(factory->QueryInterface(IID_IWTSPlugin, &unsupported) == E_NOINTERFACE && !unsupported,
            "Unsupported factory interface did not clear the output");
        Check(factory->QueryInterface(IID_IUnknown, NULL) == E_POINTER,
            "Null factory QueryInterface output was accepted");
        Check(factory->CreateInstance(NULL, IID_IWTSPlugin, NULL) == E_POINTER,
            "Null CreateInstance output was accepted");
        IWTSPlugin* invalid = reinterpret_cast<IWTSPlugin*>(factory);
        Check(factory->CreateInstance(identity, IID_IWTSPlugin, (void**)&invalid)
            == CLASS_E_NOAGGREGATION && !invalid,
            "Factory accepted aggregation");

        CountedPlugin first;
        first.AddRef();
        Check(SUCCEEDED(instance->SetWTSPluginObject(&first)), "Initial plugin setter failed");
        IWTSPlugin* returned = NULL;
        Check(SUCCEEDED(factory->CreateInstance(NULL, IID_IWTSPlugin, (void**)&returned))
            && returned == static_cast<IWTSPlugin*>(&first),
            "Factory did not return the registered plugin");
        Check(returned->Release() == 2, "Factory did not balance its temporary plugin reference");

        const std::wstring scenario = argv[2];
        if (scenario == L"removed") {
            Check(unregisterInstance(instance), "Could not remove plugin instance");
            Check(instance->Release() == 0, "Instance remained alive after removal");
            Check(first.refs == 1, "Instance did not release the plugin");
            returned = reinterpret_cast<IWTSPlugin*>(factory);
            Check(factory->CreateInstance(NULL, IID_IWTSPlugin, (void**)&returned)
                == REGDB_E_CLASSNOTREG && !returned,
                "Factory used a removed instance or fell back to a built-in plugin");
            Check(first.Release() == 0, "Test plugin reference not balanced");
        } else if (scenario == L"replacement") {
            CountedPlugin second;
            second.AddRef();
            first.instance = instance;
            first.replacement = &second;
            first.replaceOnQuery = true;
            Check(SUCCEEDED(factory->CreateInstance(NULL, IID_IWTSPlugin, (void**)&returned))
                && returned == static_cast<IWTSPlugin*>(&first),
                "Factory failed when the plugin replaced itself during QueryInterface");
            Check(first.refsAfterReplacement == 2,
                "Factory did not retain the old plugin across replacement");
            Check(returned->Release() == 1, "Factory leaked its temporary old plugin reference");
            Check(SUCCEEDED(factory->CreateInstance(NULL, IID_IWTSPlugin, (void**)&returned))
                && returned == static_cast<IWTSPlugin*>(&second),
                "Existing factory did not return the replacement plugin");
            Check(returned->Release() == 2, "Factory did not balance the replacement reference");
            Check(SUCCEEDED(instance->SetWTSPluginObject(NULL)), "Plugin clearing failed");
            Check(second.refs == 1, "Clearing did not release the replacement plugin");
            Check(SUCCEEDED(factory->CreateInstance(NULL, IID_IWTSPlugin, (void**)&returned))
                && returned && returned != static_cast<IWTSPlugin*>(&second),
                "Live session without a plugin did not use the built-in plugin");
            returned->Release();
            Check(unregisterInstance(instance), "Could not remove plugin instance");
            Check(instance->Release() == 0, "Instance remained alive after removal");
            Check(first.Release() == 0 && second.Release() == 0,
                "Test plugin references not balanced");
        } else {
            throw std::runtime_error("Unknown DVC factory scenario");
        }

        Check(factory->Release() == 0, "Class factory retained an extra reference");
        Check(FreeLibrary(module) != FALSE, "Could not unload test DLL");
        std::wcout << L"PASS DVC factory lifetime: " << scenario << L'\n';
        return 0;
    } catch (const std::exception& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
