# Native logging regression tests

These tests exercise runtime diagnostic logging without making an RDP connection.
They are opt-in and do not change normal builds or release packaging.

## Build and run

Use Visual Studio 2022 or newer with the C++ toolchain and ATL for the target
architecture. Run from the repository root:

```powershell
cmake -S . -B build-logging-x64 -G "Visual Studio 17 2022" -A x64 -DWITH_DOTNET=OFF -DMSRDPEX_BUILD_LOGGING_TESTS=ON
cmake --build build-logging-x64 --config Release
ctest --test-dir build-logging-x64 -C Release --output-on-failure
```

Use `-A ARM64` or `-A Win32` and a separate build directory for ARM64 or x86.
The test process must run on Windows with support for its target architecture.
If multiple Visual Studio installations exist, select the one with ATL using
`-DCMAKE_GENERATOR_INSTANCE="C:/Program Files/Microsoft Visual Studio/2022/Community"`.

There are 24 tests per architecture: ten isolated DLL scenarios, each with hooks
disabled and enabled, a deterministic reentrancy/lifecycle test, an abandoned-lock
test, and two recording teardown tests. CTest applies a
45-second timeout to each case, including concurrency and process shutdown.
DLL scenarios retain their temporary log files and print the directory on success
or failure; the shutdown case exits directly while writer threads are active.

## Reproduce against another DLL

The DLL integration runner loads exactly the absolute path supplied to it:

```powershell
$dll = "C:/path/to/packaged/win-x64/native/MsRdpEx.dll"
./build-logging-x64/Release/MsRdpEx_LoggingTest.exe $dll late 0
./build-logging-x64/Release/MsRdpEx_LoggingTest.exe $dll startup 0
```

Use a runner and DLL with matching architectures. Against Devolutions.MsRdpEx
2026.6.18, `late` fails because enabling logging after DLL load creates no file;
`startup` passes because the environment was configured before DLL load.

Supported scenarios are `late`, `startup`, `toggle`, `repeat`, `switch`,
`disabled-path`, `recovery`, `levels`, `concurrent`, and `shutdown`. The last
argument is `0` or `1` to disable or enable ActiveX hooks in that process.
The runner clears inherited logging settings before selecting each scenario.

The configuration sequence matches RoyalApps.Community.Rdp.WinForms 1.4.3:
disable logging, select level and path, then enable logging. Assertions check
actual native output, preservation of earlier entries, immediate suppression on
disable, file-handle release, UTF-8 destinations, failure recovery, and filtering.
Concurrency tests also check that emitted records are complete.

`MsRdpEx_LoggerReentrancyTest` links the production `Log.c` with a test file-open
adapter that calls back into the logger. This guarantees reentrancy coverage
independently of which hooks a particular Windows version invokes. It also
verifies hex-dump output and suppression, append behavior, and that identical
settings do not reopen the destination, including an empty path selecting the
default log. Default-path coverage also checks disable/re-enable and switching
between default and explicit destinations without losing entries.
It also verifies that closing and reopening
the logger preserves the enabled setting and prior entries, while an explicit
disable remains disabled across an open operation.

`logging.abandoned-lock` terminates a test thread inside the file-open adapter,
while that thread owns the production logger lock. It then exercises logger
entry points after preparing for process exit; none may wait on that lock.

`logging.recording-exit` and `logging.recording-unload` build a separate test DLL
from the production sources, with a fixture export that creates a recording owned
only by the instance manager. A parent process checks that process exit or explicit
DLL unload writes the recording manifest. The fixture adds no exports to the
production DLL and does not use an optional encoder; it validates recording
ownership and manifest finalization, not video encoding or remux output.

## Runtime behavior

An enable operation opens the selected destination; disable flushes and closes it.
Changing an enabled destination switches output without retaining the previous
file on failure. Runtime opens append; environment-enabled startup retains the
existing truncate behavior. Invalid paths and failed opens leave output inactive
and report a debugger message. A subsequent enable operation retries a valid
stored path; replacing an invalid path restores normal operation.

The existing COM interface and managed API remain unchanged. The setters still
return `void`; this patch does not introduce a managed configuration-status API.
At process termination, logging is suppressed on the exiting thread before the
existing DLL cleanup runs. This avoids acquiring logger or diagnostic stream
locks owned by terminated threads while preserving instance and recording cleanup.
Explicit DLL unloading retains normal logger close/flush behavior. Closing the
logger releases its file without changing the enabled configuration, allowing a
later native load/open to resume logging. Explicit disable still changes that
configuration and closes the file.
