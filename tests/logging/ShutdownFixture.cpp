#include <MsRdpEx/RdpInstance.h>
#include <MsRdpEx/OutputMirror.h>

// Compiled only into the test DLL. Production exports and COM interfaces stay
// unchanged. Leave the manager as the sole owner so DLL teardown must finalize
// the recording. No RDP server or optional video encoder is required.
extern "C" bool PrepareRecording(const char* directory)
{
    auto mirror = MsRdpEx_OutputMirror_New();
    if (!mirror)
        return false;
    MsRdpEx_OutputMirror_SetRecordingPath(mirror, directory);
    MsRdpEx_OutputMirror_SetSessionId(mirror, "11111111-2222-3333-4444-555555555555");
    MsRdpEx_OutputMirror_SetFrameSize(mirror, 16, 16);
    MsRdpEx_OutputMirror_SetVideoRecordingEnabled(mirror, true);
    if (!MsRdpEx_OutputMirror_Init(mirror)) {
        MsRdpEx_OutputMirror_Free(mirror);
        return false;
    }
    auto native = CMsRdpExInstance_New(nullptr);
    if (!native) {
        MsRdpEx_OutputMirror_Free(mirror);
        return false;
    }
    auto instance = reinterpret_cast<IMsRdpExInstance*>(native);
    instance->SetOutputMirrorObject(mirror);
    const bool added = MsRdpEx_InstanceManager_Add(native);
    instance->Release();
    return added;
}
