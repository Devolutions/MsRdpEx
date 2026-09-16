#include <MsRdpEx/RdpCoreApi.h>

#include <atomic>
#include <filesystem>
#include <fstream>
#include <iostream>
#include <stdexcept>
#include <string>
#include <thread>
#include <vector>

namespace fs = std::filesystem;

static void Require(bool condition, const char* message)
{
    if (!condition)
        throw std::runtime_error(message);
}

static std::string Read(const fs::path& path)
{
    if (!fs::exists(path))
        return {};
    std::ifstream stream(path, std::ios::binary);
    Require(stream.good(), "Could not read log file");
    return { std::istreambuf_iterator<char>(stream), std::istreambuf_iterator<char>() };
}

static size_t CountLoads(const fs::path& path)
{
    const auto text = Read(path);
    const std::string marker = " - CMsRdpExCoreApi::Load\n";
    size_t count = 0;
    size_t position = 0;
    while ((position = text.find(marker, position)) != std::string::npos) {
        ++count;
        position += marker.size();
    }
    return count;
}

class Core
{
public:
    explicit Core(const wchar_t* dll)
    {
        module = LoadLibraryW(dll);
        Require(module != nullptr, "Could not load the explicitly selected DLL");
        using CreateInstance = HRESULT(__stdcall*)(REFIID, LPVOID*);
        auto create = reinterpret_cast<CreateInstance>(GetProcAddress(module, "MsRdpEx_CreateInstance"));
        Require(create != nullptr, "MsRdpEx_CreateInstance export missing");
        Require(SUCCEEDED(create(__uuidof(IMsRdpExCoreApi), reinterpret_cast<void**>(&api))),
            "Could not create the native core API");
    }

    ~Core()
    {
        api->SetLogEnabled(false);
        api->SetAxHookEnabled(false);
        api->Release();
        FreeLibrary(module);
    }

    Core(const Core&) = delete;
    Core& operator=(const Core&) = delete;
    IMsRdpExCoreApi* operator->() const { return api; }

    void Path(const fs::path& path) const { api->SetLogFilePath(path.u8string().c_str()); }

    void Configure(const fs::path& path) const
    {
        // RoyalApps.Community.Rdp.WinForms 1.4.3 ApplyLogging sequence.
        api->SetLogEnabled(false);
        api->SetLogLevel(MSRDPEX_LOG_TRACE);
        Path(path);
        api->SetLogEnabled(true);
    }

private:
    HMODULE module = nullptr;
    IMsRdpExCoreApi* api = nullptr;
};

static void Test(const std::wstring& scenario, Core& core, const fs::path& directory)
{
    const auto first = directory / L"initial.log";
    // Exercise the existing UTF-8 native path contract as well.
    const auto second = directory / L"second-\u00e4-\u4e2d.log";

    if (scenario == L"startup") {
        core->Load();
        Require(CountLoads(first) == 1, "Startup logging did not write to the environment path");
        Require(Read(first).find("old contents") == std::string::npos,
            "Startup logging no longer truncates a previous process's log");
        return;
    }

    // Reproduce the wrapper's initialization before its first configuration call.
    core->SetLogEnabled(false);
    core->SetPcapEnabled(false);
    core->Load();
    core.Configure(first);
    core->Load();
    Require(CountLoads(first) == 1, "Enabling diagnostics after DLL load did not write a native entry");

    if (scenario == L"late")
        return;

    if (scenario == L"toggle") {
        core->SetLogEnabled(false);
        HANDLE exclusive = CreateFileW(first.c_str(), GENERIC_READ | GENERIC_WRITE, 0, nullptr,
            OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, nullptr);
        Require(exclusive != INVALID_HANDLE_VALUE, "Disabling logging did not close its file handle");
        CloseHandle(exclusive);
        core->Load();
        Require(CountLoads(first) == 1, "Disabled logging still wrote an entry");
        core->SetLogEnabled(true);
        core->Load();
        Require(CountLoads(first) == 2, "Re-enabling logging lost prior entries");
    }
    else if (scenario == L"repeat") {
        DWORD handlesBefore = 0;
        DWORD handlesAfter = 0;
        Require(GetProcessHandleCount(GetCurrentProcess(), &handlesBefore), "Handle count failed");
        for (int i = 0; i < 100; ++i) {
            core.Path(first);
            core->SetLogEnabled(true);
            core->Load();
            core.Configure(first);
            core->Load();
        }
        Require(GetProcessHandleCount(GetCurrentProcess(), &handlesAfter), "Handle count failed");
        Require(handlesAfter == handlesBefore, "Repeated configuration leaked file handles");
        Require(CountLoads(first) == 201, "Repeated configuration truncated or lost diagnostic entries");
    }
    else if (scenario == L"switch") {
        core.Path(second);
        core->Load();
        Require(CountLoads(first) == 1 && CountLoads(second) == 1,
            "Changing the enabled destination did not redirect output");
        core.Path(first);
        core->Load();
        Require(CountLoads(first) == 2 && CountLoads(second) == 1,
            "Switching back truncated a log or wrote to the wrong file");
    }
    else if (scenario == L"disabled-path") {
        core->SetLogEnabled(false);
        core.Path(second);
        core->Load();
        Require(!fs::exists(second), "Setting a path while disabled created a log file");
        core->SetLogEnabled(true);
        core->Load();
        Require(CountLoads(first) == 1 && CountLoads(second) == 1,
            "Enabling did not use the path selected while disabled");
    }
    else if (scenario == L"recovery") {
        const auto missing = directory / L"missing";
        const auto retry = missing / L"retry.log";
        core.Path(retry);
        core->Load();
        Require(CountLoads(first) == 1 && !fs::exists(retry), "Failed open fell back to the previous log");
        fs::create_directory(missing);
        core->SetLogEnabled(true);
        core->Load();
        Require(CountLoads(retry) == 1, "Enabling did not retry the retained destination");

        // A directory is a deterministic unwritable file destination on Windows.
        core.Path(directory);
        core->Load();
        Require(CountLoads(retry) == 1, "Unwritable destination retained the old file handle");
        core->SetLogFilePath(std::string(MSRDPEX_MAX_PATH, 'x').c_str());
        core->SetLogEnabled(true);
        core->Load();
        core->SetLogFilePath(nullptr);
        core->SetLogEnabled(true);
        core->Load();
        Require(CountLoads(retry) == 1, "Invalid path fell back to the previous log");
        core.Path(second);
        core->Load();
        Require(CountLoads(second) == 1, "Valid path did not recover logging after invalid paths");
    }
    else if (scenario == L"levels") {
        for (uint32_t level : std::vector<uint32_t>{ MSRDPEX_LOG_INFO, MSRDPEX_LOG_OFF, UINT32_MAX }) {
            core->SetLogLevel(level);
            core->Load();
        }
        Require(CountLoads(first) == 1, "Level filtering did not suppress DEBUG entries");
        core->SetLogLevel(MSRDPEX_LOG_DEBUG);
        core->Load();
        Require(CountLoads(first) == 2, "Restoring DEBUG did not resume logging");
    }
    else if (scenario == L"concurrent" || scenario == L"shutdown") {
        std::atomic<bool> stop{ false };
        std::atomic<unsigned int> attempts{ 0 };
        std::vector<std::thread> writers;
        for (int i = 0; i < 4; ++i) {
            writers.emplace_back([&]() {
                while (!stop.load()) {
                    core->Load();
                    ++attempts;
                    std::this_thread::yield();
                }
            });
        }
        if (scenario == L"shutdown") {
            while (attempts.load() < 100)
                std::this_thread::yield();
            // ExitProcess terminates other threads before calling DllMain.
            // They may still own the logger lock; CTest's timeout detects a hang.
            ExitProcess(0);
        }
        bool wroteWhileDisabled = false;
        try {
            for (int i = 0; i < 100; ++i) {
                core->SetLogEnabled(false);
                const auto count = CountLoads(first) + CountLoads(second);
                const auto before = attempts.load();
                while (attempts.load() - before < 20)
                    std::this_thread::yield();
                wroteWhileDisabled |= count != CountLoads(first) + CountLoads(second);
                core.Path((i % 2) ? first : second);
                core->SetLogEnabled(true);
                core->Load();
            }
        }
        catch (...) {
            stop = true;
            for (auto& writer : writers)
                writer.join();
            throw;
        }
        stop = true;
        for (auto& writer : writers)
            writer.join();
        core->SetLogEnabled(false);
        Require(!wroteWhileDisabled, "A concurrent writer wrote after disabling returned");
        Require(CountLoads(first) > 1 && CountLoads(second) > 0, "Concurrent destination changes lost output");
        for (const auto& path : { first, second }) {
            std::ifstream stream(path);
            std::string line;
            while (std::getline(stream, line)) {
                Require(line.rfind("[", 0) == 0 && line.find(" PID:") != std::string::npos &&
                    line.find(" TID:") != std::string::npos && line.find(" - ") != std::string::npos,
                    "Concurrent writes produced a malformed log record");
            }
        }
    }
    else {
        throw std::runtime_error("Unknown logging test scenario");
    }
}

int wmain(int argc, wchar_t** argv)
{
    if (argc != 4) {
        std::cerr << "Usage: MsRdpEx_LoggingTest <absolute-dll-path> <scenario> <hooks:0|1>\n";
        return 2;
    }

    fs::path directory;
    try {
        Require(fs::path(argv[1]).is_absolute(), "The DLL path must be absolute");
        directory = fs::temp_directory_path() /
            (L"MsRdpEx-LoggingTest-" + std::to_wstring(GetCurrentProcessId()) + L"-" + std::to_wstring(GetTickCount64()));
        Require(fs::create_directory(directory), "Could not create an isolated test directory");
        SetEnvironmentVariableW(L"MSRDPEX_LOG_LEVEL", nullptr);
        SetEnvironmentVariableW(L"MSRDPEX_LOG_FILE_PATH", nullptr);
        SetEnvironmentVariableW(L"MSRDPEX_LOG_ENABLED", nullptr);
        SetEnvironmentVariableW(L"MSRDPEX_PCAP_ENABLED", L"0");
        SetEnvironmentVariableW(L"MSRDPEX_HOOK_ENABLED", argv[3]);

        const std::wstring scenario = argv[2];
        if (scenario == L"startup") {
            const auto path = directory / L"initial.log";
            std::ofstream(path) << "old contents\n";
            SetEnvironmentVariableW(L"MSRDPEX_LOG_LEVEL", L"TRACE");
            SetEnvironmentVariableW(L"MSRDPEX_LOG_FILE_PATH", path.c_str());
        }
        Core core(argv[1]);
        Test(scenario, core, directory);
        std::wcout << L"PASS " << scenario << L" hooks=" << argv[3] << L"; logs: " << directory << L'\n';
        return 0;
    }
    catch (const std::exception& error) {
        std::cerr << "FAIL: " << error.what() << '\n';
        std::wcerr << L"Logs: " << directory << L'\n';
        return 1;
    }
}
