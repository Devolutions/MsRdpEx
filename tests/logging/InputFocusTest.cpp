#include <windows.h>

#include <iostream>
#include <stdexcept>
#include <string>
#include <vector>

struct InputMessage
{
    UINT message;
    WPARAM key;
    LPARAM flags;
};

static std::vector<InputMessage> forwarded;
static constexpr LRESULT ForwardedResult = 0x1234;

static void Check(bool condition, const char* message)
{
    if (!condition)
        throw std::runtime_error(message);
}

static LRESULT CALLBACK OriginalInputProc(HWND, UINT message, WPARAM key, LPARAM flags)
{
    forwarded.push_back({ message, key, flags });
    return ForwardedResult;
}

static void ExpectForwarded(WNDPROC inputProc, UINT message, WPARAM key, LPARAM flags)
{
    forwarded.clear();
    LRESULT result = CallWindowProcW(inputProc, NULL, message, key, flags);
    Check(result == ForwardedResult && forwarded.size() == 1,
        "Input message did not reach the original RDP procedure exactly once");
    const InputMessage& received = forwarded.front();
    Check(received.message == message && received.key == key && received.flags == flags,
        "Input message payload changed before reaching the original RDP procedure");
}

static void ExpectBlocked(WNDPROC inputProc, UINT message, LPARAM flags)
{
    forwarded.clear();
    Check(CallWindowProcW(inputProc, NULL, message, 0xFF, flags) == 0 && forwarded.empty(),
        "The reserved focus key press reached the original RDP procedure (RDMW-24578)");
}

static void CheckFocusPresses(WNDPROC inputProc)
{
    for (LPARAM flags : { LPARAM(1), LPARAM(0x20380001), LPARAM(0x40380002) })
    {
        ExpectBlocked(inputProc, WM_KEYDOWN, flags);
        ExpectBlocked(inputProc, WM_SYSKEYDOWN, flags);
    }
}

static void CheckFocusReleases(WNDPROC inputProc)
{
    for (LPARAM flags : { LPARAM(0xC0000001), LPARAM(0xE0380001), LPARAM(0xC1380001) })
    {
        ExpectForwarded(inputProc, WM_KEYUP, 0xFF, flags);
        ExpectForwarded(inputProc, WM_SYSKEYUP, 0xFF, flags);
    }
}

static void CheckOrdinaryKeys(WNDPROC inputProc)
{
    for (WPARAM key = 1; key < 0xFF; ++key)
    {
        ExpectForwarded(inputProc, WM_KEYDOWN, key, 1);
        ExpectForwarded(inputProc, WM_KEYUP, key, LPARAM(0xC0000001));
        ExpectForwarded(inputProc, WM_SYSKEYDOWN, key, LPARAM(0x20000001));
        ExpectForwarded(inputProc, WM_SYSKEYUP, key, LPARAM(0xE0000001));
    }

    ExpectForwarded(inputProc, WM_APP + 1, 0xFF, 42);
}

static void CheckRepeatedFocus(WNDPROC inputProc)
{
    for (int cycle = 0; cycle < 250; ++cycle)
    {
        ExpectForwarded(inputProc, WM_SETFOCUS, 0, 0);
        ExpectBlocked(inputProc, WM_KEYDOWN, 1);
        ExpectForwarded(inputProc, WM_KEYUP, 0xFF, LPARAM(0xC0000001));
        ExpectForwarded(inputProc, WM_SYSKEYDOWN, VK_MENU, LPARAM(0x20380001));
        ExpectForwarded(inputProc, WM_KILLFOCUS, 0, 0);
        ExpectForwarded(inputProc, WM_SETFOCUS, 0, 0);
        ExpectBlocked(inputProc, WM_SYSKEYDOWN, LPARAM(0x20000001));
        ExpectForwarded(inputProc, WM_SYSKEYUP, 0xFF, LPARAM(0xE0000001));
        ExpectForwarded(inputProc, WM_SYSKEYUP, VK_MENU, LPARAM(0xC0380001));
        ExpectForwarded(inputProc, WM_KEYDOWN, 'A', LPARAM(0x001E0001));
        ExpectForwarded(inputProc, WM_KEYUP, 'A', LPARAM(0xC01E0001));
    }
}

int wmain(int argc, wchar_t** argv)
{
    try
    {
        Check(argc == 3, "Expected DLL path and scenario");
        Check(SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", L"1") != FALSE,
            "Could not enable ActiveX hooks");
        Check(SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", L"0") != FALSE,
            "Could not disable logging");

        HMODULE module = LoadLibraryW(argv[1]);
        Check(module != NULL, "Could not load the selected native DLL");
        HINSTANCE application = GetModuleHandleW(NULL);
        WNDCLASSW inputClass = {};
        inputClass.hInstance = application;
        inputClass.lpfnWndProc = OriginalInputProc;
        inputClass.lpszClassName = L"IHWindowClass";
        Check(RegisterClassW(&inputClass) != 0, "Could not register the RDP input class");

        WNDCLASSW registered = {};
        Check(GetClassInfoW(application, L"IHWindowClass", &registered) != FALSE,
            "Could not find the registered RDP input class");
        Check(registered.lpfnWndProc != OriginalInputProc,
            "The production DLL did not install its RDP input hook");

        std::wstring scenario = argv[2];
        if (scenario == L"focus-presses")
            CheckFocusPresses(registered.lpfnWndProc);
        else if (scenario == L"focus-releases")
            CheckFocusReleases(registered.lpfnWndProc);
        else if (scenario == L"ordinary-keys")
            CheckOrdinaryKeys(registered.lpfnWndProc);
        else if (scenario == L"repeated-focus")
            CheckRepeatedFocus(registered.lpfnWndProc);
        else
            throw std::runtime_error("Unknown scenario");

        Check(UnregisterClassW(L"IHWindowClass", application) != FALSE,
            "Could not unregister the RDP input class");
        Check(FreeLibrary(module) != FALSE, "Could not unload the native DLL");
        std::wcout << L"PASS: " << scenario << L'\n';
        return 0;
    }
    catch (const std::exception& exception)
    {
        std::cerr << exception.what() << '\n';
        return 1;
    }
}
