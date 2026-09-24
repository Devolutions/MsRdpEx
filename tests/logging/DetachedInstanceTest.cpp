#include <MsRdpEx/RdpInstance.h>

#include <iostream>
#include <stdexcept>

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

    ULONG refs = 1;
};

int wmain(int argc, wchar_t** argv)
{
    try {
        Check(argc == 2, "Expected test DLL path");
        Check(SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", L"0") != 0,
            "Could not disable ActiveX hooks");
        Check(SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", L"0") != 0,
            "Could not disable logging");
        Check(SetEnvironmentVariableW(L"MSRDPEX_AX_BACKEND", NULL) != 0,
            "Could not select private ActiveX layout");
        Check(SetEnvironmentVariableW(L"MSRDPEX_MSTSCAX_DLL", NULL) != 0,
            "Could not select default ActiveX DLL");

        HMODULE module = LoadLibraryW(argv[1]);
        Check(module != NULL, "Could not load test DLL");
        using Register = ATOM(*)();
        using Acquire = IMsRdpExInstance*(*)(HWND);
        using ReleaseManager = void(*)();
        auto registerClass = reinterpret_cast<Register>(
            GetProcAddress(module, "RegisterDetachedOutputWindowClass"));
        auto acquire = reinterpret_cast<Acquire>(
            GetProcAddress(module, "AcquireDetachedOutputInstance"));
        auto releaseManager = reinterpret_cast<ReleaseManager>(
            GetProcAddress(module, "ReleaseDetachedInstanceManager"));
        Check(registerClass && acquire && releaseManager, "Detached fixture exports missing");
        Check(registerClass() != 0, "Could not register hooked output window class");
        Check(acquire(NULL) == NULL, "NULL matched an unbound output window");

        HINSTANCE application = GetModuleHandleW(NULL);
        HWND first = CreateWindowExW(0, L"OPWindowClass_mstscax", L"first",
            WS_OVERLAPPED, 0, 0, 10, 10, NULL, NULL, application, NULL);
        HWND second = CreateWindowExW(0, L"OPWindowClass_mstscax", L"second",
            WS_OVERLAPPED, 0, 0, 10, 10, NULL, NULL, application, NULL);
        Check(first && second && first != second, "Could not create detached output windows");

        IMsRdpExInstance* firstInstance = acquire(first);
        IMsRdpExInstance* secondInstance = acquire(second);
        Check(firstInstance && secondInstance && firstInstance != secondInstance,
            "Each detached window must have its own registered instance");
        HWND outputWindow = NULL;
        void* rdpClient = first;
        Check(SUCCEEDED(firstInstance->GetOutputWindow(&outputWindow)) && outputWindow == first,
            "Detached instance did not attach its output window");
        Check(SUCCEEDED(firstInstance->GetRdpClient(&rdpClient)) && !rdpClient,
            "Detached instance unexpectedly has a client");
        Check(acquire(NULL) == NULL, "NULL matched a detached output window");

        CountedUnknown firstPlugin, secondPlugin;
        firstPlugin.AddRef();
        secondPlugin.AddRef();
        Check(SUCCEEDED(firstInstance->SetWTSPluginObject(&firstPlugin)) &&
            SUCCEEDED(secondInstance->SetWTSPluginObject(&secondPlugin)),
            "Could not attach lifetime sentinels");
        Check(DestroyWindow(first) != FALSE, "Could not destroy first window");
        Check(acquire(first) == NULL, "Destroyed window remained registered");
        IMsRdpExInstance* stillRegistered = acquire(second);
        Check(stillRegistered == secondInstance,
            "Destroying first window removed the second instance");
        stillRegistered->Release();
        Check(firstInstance->Release() == 0,
            "Detached instance retained its creator reference after removal");
        Check(firstPlugin.refs == 1 && secondPlugin.refs == 2,
            "Destroying first window did not release only its instance");

        Check(DestroyWindow(second) != FALSE, "Could not destroy second window");
        Check(acquire(second) == NULL, "Second destroyed window remained registered");
        Check(secondInstance->Release() == 0,
            "Second detached instance retained its creator reference after removal");
        Check(secondPlugin.refs == 1, "Second instance did not release its owned plugin");
        Check(firstPlugin.Release() == 0 && secondPlugin.Release() == 0,
            "Lifetime sentinel references not balanced");

        releaseManager();
        Check(UnregisterClassW(L"OPWindowClass_mstscax", application) != FALSE,
            "Could not unregister output window class");
        Check(FreeLibrary(module) != FALSE, "Could not unload test DLL");
        std::cout << "PASS detached instance registration and lifetime\n";
        return 0;
    } catch (const std::exception& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
