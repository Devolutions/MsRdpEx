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

    if (scenario == L"startup") {
        core->Load();
        Require(CountLoads(first) == 1, "Startup logging did not write to the environment path");
        Require(Read(first).find("old contents") == std::string::npos,
            "Startup logging no longer truncates a previous process's log");
        return;
    }

    if (scenario == L"first-open") {
        // Race independent session configuration calls before any destination
        // has been selected. Exactly one complete path must win.
        constexpr int threadCount = 8;
        std::atomic<bool> start{ false };
        std::vector<std::thread> sessions;
        std::vector<fs::path> paths;

        core->SetLogEnabled(false);
        for (int i = 0; i < threadCount; ++i)
            paths.push_back(directory / (L"session-" + std::to_wstring(i) + L".log"));

        for (int i = 0; i < threadCount; ++i) {
            sessions.emplace_back([&, i]() {
                while (!start.load())
                    std::this_thread::yield();
                core.Path(paths[i]);
                core->SetLogEnabled(true);
            });
        }

        start = true;
        for (auto& session : sessions)
            session.join();

        core->SetLogLevel(MSRDPEX_LOG_DEBUG);
        core->Load();

        size_t populated = 0;
        for (const auto& path : paths)
            populated += CountLoads(path) == 1 ? 1 : 0;
        Require(populated == 1, "Concurrent first-open configuration selected an invalid destination");
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

    if (scenario == L"levels") {
        for (uint32_t level : std::vector<uint32_t>{ MSRDPEX_LOG_INFO, MSRDPEX_LOG_OFF, UINT32_MAX }) {
            core->SetLogLevel(level);
            core->Load();
        }
        Require(CountLoads(first) == 1, "Level filtering did not suppress DEBUG entries");
        core->SetLogLevel(MSRDPEX_LOG_DEBUG);
        core->Load();
        Require(CountLoads(first) == 2, "Restoring DEBUG did not resume logging");
    }
    else if (scenario == L"concurrent") {
        // Several RDP sessions log from the same process while diagnostics are
        // toggled. The destination is fixed once opened, so this checks record
        // integrity and immediate suppression, not destination switching.
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
        try {
            for (int i = 0; i < 100; ++i) {
                core->SetLogEnabled(false);
                std::this_thread::yield();
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
        Require(CountLoads(first) > 1, "Concurrent logging lost output");
        std::ifstream stream(first);
        std::string line;
        while (std::getline(stream, line)) {
            Require(line.rfind("[", 0) == 0 && line.find(" PID:") != std::string::npos &&
                line.find(" TID:") != std::string::npos && line.find(" - ") != std::string::npos,
                "Concurrent writes produced a malformed log record");
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
