#include <MsRdpEx/MsRdpEx.h>
#include <MsRdpEx/Environment.h>
#include "../../dll/Log.h"

#include <fstream>
#include <iostream>
#include <stdexcept>
#include <string>

static unsigned int openCalls = 0;
static bool nestedLevelActive = false;
static bool abandonLock = false;
static std::string appDataPath = ".";

// Link the production Log.c with a file-open adapter which deliberately calls
// back into the logger. The DLL integration tests cover real environment/path IO.
FILE* MsRdpEx_FileOpen(const char* path, const char* mode)
{
    if (abandonLock)
        ExitThread(0); // Deterministically model a terminated logger-lock owner.
    ++openCalls;
    nestedLevelActive |= MsRdpEx_IsLogLevelActive(MSRDPEX_LOG_DEBUG);
    MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "nested callback");
    const uint8_t data[] = { 'N', 'E', 'S', 'T', 'E', 'D' };
    MsRdpEx_LogHexDump(data, sizeof(data));
    return fopen(path, mode);
}

bool MsRdpEx_EnvExists(const char*) { return false; }
char* MsRdpEx_GetEnv(const char*) { return nullptr; }
const char* MsRdpEx_GetPath(uint32_t) { return appDataPath.c_str(); }
bool MsRdpEx_StringIEquals(const char* left, const char* right) { return _stricmp(left, right) == 0; }

static void Require(bool condition, const char* message)
{
    if (!condition)
        throw std::runtime_error(message);
}

static std::string Read(const char* path)
{
    std::ifstream stream(path, std::ios::binary);
    Require(stream.good(), "Could not read the log file");
    return { std::istreambuf_iterator<char>(stream), std::istreambuf_iterator<char>() };
}

int main(int argc, char** argv)
{
    char directory[MAX_PATH] = { 0 };
    char first[MAX_PATH] = { 0 };
    char second[MAX_PATH] = { 0 };
    try {
        Require(GetTempPathA(MAX_PATH, directory) != 0, "Temporary directory lookup failed");
        Require(GetTempFileNameA(directory, "rdp", 0, first) != 0, "Temporary file creation failed");
        Require(GetTempFileNameA(directory, "rdp", 0, second) != 0, "Temporary file creation failed");

        MsRdpEx_LogOpen();
        MsRdpEx_SetLogLevel(MSRDPEX_LOG_TRACE);
        MsRdpEx_SetLogFilePath(first);
        if (argc == 2 && strcmp(argv[1], "shutdown") == 0) {
            abandonLock = true;
            HANDLE writer = CreateThread(nullptr, 0, [](LPVOID) -> DWORD {
                MsRdpEx_SetLogEnabled(true);
                return 1; // The file-open adapter must terminate this thread.
            }, nullptr, 0, nullptr);
            Require(writer != nullptr, "Could not create the logger-lock owner");
            Require(WaitForSingleObject(writer, 5000) == WAIT_OBJECT_0, "Writer did not terminate");
            DWORD result = 1;
            Require(GetExitCodeThread(writer, &result) && result == 0, "Lock was not abandoned");
            CloseHandle(writer);
            MsRdpEx_LogPrepareForProcessExit();
            Require(!MsRdpEx_IsLogLevelActive(MSRDPEX_LOG_DEBUG), "Shutdown logging remained active");
            MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "shutdown record");
            const uint8_t data[] = { 'A', 'B' };
            MsRdpEx_LogHexDump(data, sizeof(data));
            MsRdpEx_SetLogEnabled(false);
            MsRdpEx_SetLogEnabled(true);
            MsRdpEx_SetLogLevel(MSRDPEX_LOG_TRACE);
            MsRdpEx_SetLogFilePath(second);
            MsRdpEx_LogOpen();
            MsRdpEx_LogClose();
            Require(openCalls == 0 && Read(first).empty() && Read(second).empty(),
                "Shutdown touched diagnostic files");
            DeleteFileA(first);
            DeleteFileA(second);
            std::cout << "PASS shutdown with an abandoned logger lock\n";
            return 0;
        }
        MsRdpEx_SetLogEnabled(true);
        Require(MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "first record"), "Initial write failed");
        const uint8_t data[] = { 'A', 'B' };
        MsRdpEx_LogHexDump(data, sizeof(data));
        MsRdpEx_SetLogEnabled(true);
        MsRdpEx_SetLogFilePath(first);
        Require(openCalls == 1, "Identical settings reopened the log file");
        MsRdpEx_SetLogEnabled(false);
        const auto disabled = Read(first);
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "disabled record");
        MsRdpEx_LogHexDump(data, sizeof(data));
        Require(Read(first) == disabled, "Disabled normal or hex-dump output reached the file");

        MsRdpEx_SetLogEnabled(true);
        MsRdpEx_SetLogFilePath(second);
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "second record");
        MsRdpEx_SetLogFilePath(first);
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "last record");
        MsRdpEx_LogClose();

        const auto closed = Read(first);
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "closed record");
        Require(Read(first) == closed, "Closing did not stop writes");
        MsRdpEx_LogOpen();
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "reopened record");
        MsRdpEx_LogClose();
        Require(Read(first).find("reopened record") != std::string::npos,
            "Close/open lost the enabled configuration");
        MsRdpEx_SetLogEnabled(false);
        MsRdpEx_LogOpen();
        Require(!MsRdpEx_IsLogLevelActive(MSRDPEX_LOG_DEBUG), "Open re-enabled explicitly disabled logging");
        MsRdpEx_LogClose();

        const auto firstText = Read(first);
        const auto secondText = Read(second);
        Require(openCalls == 5, "Unexpected number of file opens");
        Require(!nestedLevelActive, "A reentrant level check was active");
        Require(firstText.find("nested") == std::string::npos && secondText.find("nested") == std::string::npos &&
            firstText.find("NESTED") == std::string::npos && secondText.find("NESTED") == std::string::npos,
            "A reentrant callback wrote to a log");
        Require(firstText.find("first record") != std::string::npos &&
            firstText.find("4142") != std::string::npos && firstText.find("last record") != std::string::npos,
            "Reconfiguration lost normal or hex-dump output");
        Require(firstText.find("second record") == std::string::npos &&
            secondText.find("second record") != std::string::npos,
            "Output reached the wrong destination");

        appDataPath = std::string(first) + "-default";
        Require(CreateDirectoryA(appDataPath.c_str(), nullptr), "Default-path test directory creation failed");
        const auto defaultPath = appDataPath + "\\MsRdpEx.log";
        MsRdpEx_SetLogFilePath("");
        Require(openCalls == 5, "Selecting the default path while disabled opened a file");
        MsRdpEx_SetLogEnabled(true);
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "default first record");
        for (int i = 0; i < 3; ++i) {
            MsRdpEx_SetLogFilePath("");
            MsRdpEx_SetLogEnabled(true);
        }
        Require(openCalls == 6, "Identical default-path settings reopened the log file");
        MsRdpEx_SetLogEnabled(false);
        MsRdpEx_SetLogFilePath("");
        Require(openCalls == 6, "Reapplying the disabled default path opened a file");
        MsRdpEx_SetLogEnabled(true);
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "default resumed record");
        MsRdpEx_SetLogFilePath(second);
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "explicit destination record");
        MsRdpEx_SetLogFilePath("");
        MsRdpEx_SetLogFilePath("");
        MsRdpEx_Log(MSRDPEX_LOG_DEBUG, "default returned record");
        MsRdpEx_SetLogEnabled(false);
        Require(openCalls == 9, "Unexpected file opens while switching default and explicit paths");
        const auto defaultText = Read(defaultPath.c_str());
        Require(defaultText.find("default first record") != std::string::npos &&
            defaultText.find("default resumed record") != std::string::npos &&
            defaultText.find("default returned record") != std::string::npos,
            "Default-path configuration lost prior entries");
        Require(defaultText.find("explicit destination record") == std::string::npos &&
            Read(second).find("explicit destination record") != std::string::npos,
            "Switching between default and explicit paths routed output incorrectly");
        DeleteFileA(defaultPath.c_str());
        RemoveDirectoryA(appDataPath.c_str());
        DeleteFileA(first);
        DeleteFileA(second);
        std::cout << "PASS deterministic reentrancy and hex-dump lifecycle\n";
        return 0;
    }
    catch (const std::exception& error) {
        MsRdpEx_LogClose();
        std::cerr << "FAIL: " << error.what() << '\n';
        return 1;
    }
}
