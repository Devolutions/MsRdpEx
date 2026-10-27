# Native regression tests

These tests exercise runtime diagnostic logging, gateway behavior, plugin
reference ownership, and smart-card credential handling without making an RDP connection.
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

## DVC class factory lifetime

`logging.dvc-factory.*` obtains a class factory through the production
`DllGetClassObject` route without connecting RDP. The cases cover use after
session removal, plugin replacement and clearing, failed plugin queries,
reentrant plugin `AddRef`/`Release`, and manager shutdown during an in-flight
query. A test-only allocation failure checks that the native setter leaves the
caller's reference untouched on failure, including same-pointer replacement.
During instance destruction, reentrant getters observe an empty slot and
setters reject new registrations without consuming their input reference.
The fixture-only exports and allocator override are not included in the DLL.

## Smart-card credential handling

The `smartcard.*` cases compile the production SSPI, settings and card-context code
into the existing private fixture DLL. The packing/unpacking/allocation boundary
and the context resolver's certificate-store, WinSCard and CSP/KSP APIs are
substituted; no test exports or API substitutions enter the shipped DLL. The tests
use a synthetic certificate reference, not a real card, certificate private key,
or user PIN.

```powershell
ctest --test-dir build-logging-x64 -C Release -R "^smartcard\." --output-on-failure
```

Coverage includes preservation of Windows certificate-logon buffers and wrappers,
unsupported/NULL/guarded inputs, inherited Schannel and extended fields, protected
credential unpacking, opt-in and empty-PIN behavior, ordinary-password pass-through,
packed-buffer scrubbing (including partial failures and changed output sizes),
session scope/worker binding, and Boolean normalization/error propagation for
`PasswordContainsSCardPin`.

The `smartcard.legacy-wrapper` case covers RDM's observed type-51 input wrapper:
it remains eligible for synthesis, and a replacement uses type 50 while keeping
Schannel, extended-wrapper fields, caller memory, and acquisition return status intact.

The `smartcard.context-*` cases cover:

- Mode parsing (including invalid values), no I/O when off/ineligible, and identical
  resolution in observe/apply. A production-DLL load check ensures the experiment
  does not preload WinSCard/NCrypt ahead of the emulated-reader hooks.
- Reader-scoped silent CAPI/CNG opens, exact public-certificate matching, handle
  cleanup, absent/duplicate certificates, multiple cards/card names, card changes,
  silent-context failures, unsupported providers and emulated-reader exclusion.
- Per-connection snapshot invalidation, no-PIN/disabled-workaround gating, and
  rejection of a snapshot invalidated during preparation.
- Context application preserving the original PIN/account-field bytes, offsets,
  provider/container/key selection and original payload; observe/failure paths
  preserve the original allocation. Both allocations are scrubbed when replaced.

These are local regression checks, not proof of successful CredSSP delegation or
ActivClient remote logon. Hardware validation must reach the intended user's
desktop without a second prompt, rather than stopping at NLA success.

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
