#include <windows.h>
#include <filesystem>
#include <iostream>
#include <stdexcept>
#include <string>

static void Check(bool condition, const char* message)
{
    if (!condition) throw std::runtime_error(message);
}

int wmain(int argc, wchar_t** argv)
{
    try {
        Check(argc == 3 || argc == 4, "Expected DLL path, exit/unload/exit-held-lock mode and optional child event");
        Check(std::filesystem::path(argv[1]).is_absolute(), "Use an absolute DLL path");
        bool holdLock = wcscmp(argv[2], L"exit-held-lock") == 0;
        bool processExit = holdLock || wcscmp(argv[2], L"exit") == 0;
        Check(processExit || wcscmp(argv[2], L"unload") == 0, "Unknown shutdown mode");
        if (argc == 4) {
            SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", L"1");
            SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", L"0");
            SetEnvironmentVariableW(L"MSRDPEX_PCAP_ENABLED", L"0");
            HMODULE module = LoadLibraryW(argv[1]);
            Check(module != NULL, "Could not load shutdown fixture DLL");
            using Prepare = bool(*)(const wchar_t*, bool);
            auto prepare = (Prepare)GetProcAddress(module, "PrepareGatewayShutdown");
            Check(prepare && prepare(argv[3], holdLock), "Could not prepare gateway shutdown fixture");
            if (processExit) ExitProcess(0);
            Check(FreeLibrary(module) != FALSE, "DLL unload failed");
            return 0;
        }

        std::wstring eventName = L"Local\\MsRdpEx-GatewayShutdown-" +
            std::to_wstring(GetCurrentProcessId()) + L"-" + std::to_wstring(GetTickCount64());
        HANDLE completed = CreateEventW(NULL, TRUE, FALSE, eventName.c_str());
        Check(completed != NULL, "Could not create shutdown completion event");
        wchar_t executable[32768] = {};
        Check(GetModuleFileNameW(NULL, executable, 32768) != 0, "Could not resolve executable");
        std::wstring command = L"\"" + std::wstring(executable) + L"\" \"" + argv[1] + L"\" " +
            argv[2] + L" \"" + eventName + L"\"";
        STARTUPINFOW startup = { sizeof(startup) };
        PROCESS_INFORMATION child = {};
        if (!CreateProcessW(executable, command.data(), NULL, NULL, FALSE, CREATE_NO_WINDOW,
                NULL, NULL, &startup, &child)) {
            CloseHandle(completed);
            throw std::runtime_error("Could not start shutdown child");
        }
        DWORD wait = WaitForSingleObject(child.hProcess, 20000), exitCode = 1;
        GetExitCodeProcess(child.hProcess, &exitCode);
        if (wait != WAIT_OBJECT_0) {
            TerminateProcess(child.hProcess, 1);
            WaitForSingleObject(child.hProcess, 5000);
        }
        bool finished = WaitForSingleObject(completed, 0) == WAIT_OBJECT_0;
        CloseHandle(child.hThread); CloseHandle(child.hProcess); CloseHandle(completed);
        Check(wait == WAIT_OBJECT_0, "DLL teardown hung");
        Check(exitCode == 0, "Shutdown child failed");
        Check(finished, "Production DLL teardown did not complete");
        std::wcout << L"PASS gateway shutdown: " << argv[2] << L'\n';
        return 0;
    } catch (const std::exception& error) {
        std::cerr << error.what() << '\n';
        return 1;
    }
}
