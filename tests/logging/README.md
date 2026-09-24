# Native regression tests

These tests exercise runtime diagnostic logging, gateway behavior, and plugin
reference ownership without making an RDP connection.
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

The ten logging tests cover five scenarios, each with ActiveX hooks disabled
and enabled. CTest also runs gateway and plugin reference tests. Each case has
a 45-second timeout. Logging scenarios retain their temporary log files and
print the directory on success or failure.

## Reproduce against another DLL

The runner loads exactly the absolute path supplied to it:

```powershell
$dll = "C:/path/to/packaged/win-x64/native/MsRdpEx.dll"
./build-logging-x64/Release/MsRdpEx_LoggingTest.exe $dll late 0
./build-logging-x64/Release/MsRdpEx_LoggingTest.exe $dll startup 0
```

Use a runner and DLL with matching architectures. Against a DLL without this
fix, `late` fails because enabling logging after DLL load creates no file;
`startup` passes because the environment was configured before DLL load.

Supported scenarios are `late`, `startup`, `levels`, `concurrent`, and
`first-open`. The last argument is `0` or `1` to disable or enable ActiveX hooks
in that process.
The runner clears inherited logging settings before selecting each scenario.

The configuration sequence matches RoyalApps.Community.Rdp.WinForms 1.4.3:
disable logging, select level and path, then enable logging.

## Runtime behavior

Logging configuration is process-scoped. The diagnostic file is opened at most
once per process:

- Startup through `MSRDPEX_LOG_*` opens the file during DLL load and keeps the
  existing truncate behavior.
- Otherwise the first `SetLogEnabled(true)` opens the file in append mode, using
  the path and level selected beforehand.
- `SetLogFilePath` and `SetLogLevel` only select what that single open uses.
  Changing the path after the log is open has no effect; restart the host
  process to log to a different destination.
- `SetLogEnabled(false)` stops new records but intentionally keeps the handle
  open. Closing it while other session threads may be writing would not be safe
  without locking the write path.

Because the handle is published once and never replaced, concurrent RDP sessions
in the same process cannot observe a closed or swapped `FILE*`, and normal
logging needs no additional application lock. Path selection and the one-time
open share a configuration lock, so simultaneous first-use configuration cannot
tear the path; this lock is never acquired while writing records. Records already
past their level check may still be written just after a disable; suppression is
not synchronous.

The existing COM interface and managed API remain unchanged, and the setters
still return `void`.

## WTS plugin reference ownership

The `logging.plugin-reference.*` cases use a test-only factory in the native
fixture DLL to create a real `CMsRdpExInstance` without an RDP control. A counted
`IUnknown` verifies that `SetWTSPluginObject` consumes an owned reference and
releases it when replaced (including by another reference to the same object),
cleared, or destroyed. The plugin getter returns a borrowed pointer. The
fixture export is not included in the production DLL.

## Detached output-window lifetime

`logging.detached-instance.lifetime` registers an output window class through
the real output-window hook in the test-only fixture DLL. It creates two windows
without RDP controls and checks that each is registered by its non-NULL window
handle, that NULL matches neither instance, and that destroying one window
removes only its matching instance. A counted plugin and the final `Release`
verify that window destruction drops the manager's reference and leaves no
creator reference behind. The fixture exports are not shipped.
The same test also verifies that failed registration and subsequent removal of
an unregistered instance leave its creator reference intact.
