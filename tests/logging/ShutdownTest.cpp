#include <windows.h>

#include <filesystem>
#include <fstream>
#include <iostream>
#include <stdexcept>
#include <string>

namespace fs = std::filesystem;

static void Require(bool condition, const char* message)
{
    if (!condition)
        throw std::runtime_error(message);
}

int wmain(int argc, wchar_t** argv)
{
    fs::path directory;
    try {
        Require(argc == 3 || argc == 4, "Usage: ShutdownTest <absolute-test-dll> <exit|unload|exit-gateway-lock> [child-directory]");
        Require(fs::path(argv[1]).is_absolute(), "Select an absolute test DLL path");
        const bool holdGatewayLock = std::wstring(argv[2]) == L"exit-gateway-lock";
        const bool processExit = holdGatewayLock || std::wstring(argv[2]) == L"exit";
        Require(processExit || std::wstring(argv[2]) == L"unload", "Unknown shutdown scenario");
        if (argc == 4) {
            directory = argv[3];
            SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", L"1");
            SetEnvironmentVariableW(L"MSRDPEX_PCAP_ENABLED", L"0");
            SetEnvironmentVariableW(L"MSRDPEX_LOG_LEVEL", L"TRACE");
            SetEnvironmentVariableW(L"MSRDPEX_LOG_FILE_PATH", (directory / L"diagnostics.log").c_str());
            SetEnvironmentVariableW(L"MSRDPEX_RECORDING_PATH", directory.c_str());
            // Keep this fixture independent of an installed optional encoder.
            SetEnvironmentVariableW(L"MSRDPEX_XMF_DLL", (directory / L"missing-xmf.dll").c_str());
            HMODULE module = LoadLibraryW(argv[1]);
            Require(module != nullptr, "Could not load shutdown fixture DLL");
            using Prepare = bool(*)(const char*);
            auto prepare = reinterpret_cast<Prepare>(GetProcAddress(module, "PrepareRecording"));
            Require(prepare && prepare(directory.u8string().c_str()), "Could not prepare a manager-owned recording");
            const auto manifest = directory / L"11111111-2222-3333-4444-555555555555" / L"recording.json";
            Require(!fs::exists(manifest), "Fixture finalized recording before DLL teardown");
            if (holdGatewayLock) {
                using Hold = bool(*)();
                auto hold = reinterpret_cast<Hold>(GetProcAddress(module, "HoldGatewayBindingLock"));
                Require(hold && hold(), "Could not hold the gateway binding lock");
            }
            if (processExit)
                ExitProcess(0);
            Require(FreeLibrary(module), "Explicit DLL unload failed");
            return 0;
        }

        directory = fs::temp_directory_path() /
            (L"MsRdpEx-ShutdownTest-" + std::to_wstring(GetCurrentProcessId()) + L"-" + std::to_wstring(GetTickCount64()));
        Require(fs::create_directory(directory), "Could not create temporary directory");
        wchar_t executable[32768] = { 0 };
        Require(GetModuleFileNameW(nullptr, executable, 32768) != 0, "Executable lookup failed");
        std::wstring command = L"\"" + std::wstring(executable) + L"\" \"" + argv[1] + L"\" " +
            argv[2] + L" \"" + directory.wstring() + L"\"";
        STARTUPINFOW startup = { sizeof(startup) };
        PROCESS_INFORMATION child = { 0 };
        Require(CreateProcessW(executable, command.data(), nullptr, nullptr, FALSE, CREATE_NO_WINDOW,
            nullptr, nullptr, &startup, &child), "Could not start shutdown child");
        const auto wait = WaitForSingleObject(child.hProcess, 20000);
        DWORD result = 1;
        GetExitCodeProcess(child.hProcess, &result);
        if (wait != WAIT_OBJECT_0) {
            TerminateProcess(child.hProcess, 1);
            WaitForSingleObject(child.hProcess, 5000);
        }
        CloseHandle(child.hThread);
        CloseHandle(child.hProcess);
        Require(wait == WAIT_OBJECT_0, "DLL teardown hung");
        Require(result == 0, "Shutdown child failed");
        std::ifstream stream(directory / L"11111111-2222-3333-4444-555555555555" / L"recording.json");
        Require(stream.good(), "DLL teardown did not finalize the recording manifest");
        const std::string manifest{ std::istreambuf_iterator<char>(stream), std::istreambuf_iterator<char>() };
        Require(manifest.find("11111111-2222-3333-4444-555555555555") != std::string::npos,
            "Recording manifest did not contain the expected session");
        std::wcout << L"PASS recording " << argv[2] << L"; files: " << directory << L'\n';
        return 0;
    }
    catch (const std::exception& error) {
        std::cerr << "FAIL: " << error.what() << '\n';
        std::wcerr << L"Files: " << directory << L'\n';
        return 1;
    }
}
