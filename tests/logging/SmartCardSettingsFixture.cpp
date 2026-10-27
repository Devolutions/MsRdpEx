#include "SmartCardTestSupport.h"
#include <string>

static void TestPrepareContext(const WCHAR*, MsRdpEx_SmartCardContext*);
#define MsRdpEx_ResolveSmartCardContext TestPrepareContext
#include "../../dll/RdpSettings.cpp"
#undef MsRdpEx_ResolveSmartCardContext

static int contextResolutions = 0;
static CMsRdpExtendedSettings* changeDuringResolution = NULL;

static void TestPrepareContext(const WCHAR*, MsRdpEx_SmartCardContext* context)
{
    ++contextResolutions;
    context->status = MsRdpEx_SmartCardContextStatus::Ready;
    context->stage = "fixture";
    if (changeDuringResolution)
        changeDuringResolution->ClearSmartCardContext();
}

static void TestContextSettings()
{
    SmartCardTestUnknown unknown;
    GUID session = {};
    CMsRdpExtendedSettings settings(&unknown, &session);
    VARIANT enabled;
    VariantInit(&enabled);
    enabled.vt = VT_BOOL;
    enabled.boolVal = VARIANT_TRUE;
    settings.put_Property(bstr_t(L"KerbCertificateLogon"), &enabled);
    settings.SetCapturedPin(L"fixture-pin");
    settings.PrepareSmartCardContext();
    SmartCardCheck(contextResolutions == 0 &&
        settings.GetSmartCardContext().mode == MsRdpEx_SmartCardContextMode::Off, "Default mode resolved a card");
    for (const WCHAR* mode : { L"observe", L"apply" }) {
        SetEnvironmentVariableW(L"MSRDPEX_SSPI_SMARTCARD_CONTEXT", mode);
        const int before = contextResolutions;
        settings.PrepareSmartCardContext();
        SmartCardCheck(contextResolutions == before + 1 &&
            settings.GetSmartCardContext().status == MsRdpEx_SmartCardContextStatus::Ready,
            "Eligible connection did not prepare a snapshot");
        settings.ClearSmartCardContext();
        SmartCardCheck(settings.GetSmartCardContext().mode == MsRdpEx_SmartCardContextMode::Off,
            "Disconnect retained experiment state");
    }
    changeDuringResolution = &settings;
    settings.PrepareSmartCardContext();
    changeDuringResolution = NULL;
    SmartCardCheck(settings.GetSmartCardContext().mode == MsRdpEx_SmartCardContextMode::Off,
        "Racing invalidation published a stale snapshot");
    settings.PrepareSmartCardContext();
    settings.SetCapturedPin(L"replacement-fixture-pin");
    SmartCardCheck(settings.GetSmartCardContext().mode == MsRdpEx_SmartCardContextMode::Off,
        "PIN replacement retained old context");
    settings.PrepareSmartCardContext();
    enabled.boolVal = VARIANT_FALSE;
    settings.put_Property(bstr_t(L"KerbCertificateLogon"), &enabled);
    SmartCardCheck(settings.GetSmartCardContext().mode == MsRdpEx_SmartCardContextMode::Off,
        "Disabling workaround retained context");
    const int before = contextResolutions;
    settings.PrepareSmartCardContext();
    SmartCardCheck(contextResolutions == before, "Disabled workaround resolved a card");
    enabled.boolVal = VARIANT_TRUE;
    settings.put_Property(bstr_t(L"KerbCertificateLogon"), &enabled);
    settings.ClearCapturedPin();
    settings.PrepareSmartCardContext();
    SmartCardCheck(contextResolutions == before &&
        settings.GetSmartCardContext().status == MsRdpEx_SmartCardContextStatus::Ineligible,
        "No-PIN path resolved a card");
    SetEnvironmentVariableW(L"MSRDPEX_SSPI_SMARTCARD_CONTEXT", NULL);
}

class SmartCardCoreProperties : public IMsRdpExtendedSettings
{
public:
    HRESULT status = S_OK;
    int writes = 0;
    VARIANT_BOOL lastValue = VARIANT_FALSE;

    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID, void** object) override
    {
        *object = NULL;
        return E_NOINTERFACE;
    }
    ULONG STDMETHODCALLTYPE AddRef() override { return 2; }
    ULONG STDMETHODCALLTYPE Release() override { return 1; }
    HRESULT STDMETHODCALLTYPE put_Property(BSTR name, VARIANT* value) override
    {
        SmartCardCheck(wcscmp(name, L"PasswordContainsSCardPin") == 0, "Wrong core property");
        SmartCardCheck(value->vt == VT_BOOL, "Smart-card property used the integer setter");
        ++writes;
        lastValue = value->boolVal;
        return status;
    }
    HRESULT STDMETHODCALLTYPE get_Property(BSTR, VARIANT*) override { return E_NOTIMPL; }
};

extern "C" const char* RunSmartCardSettingsTest(const char* scenario)
{
    static std::string failure;
    try {
        if (strcmp(scenario, "context-settings") == 0) {
            TestContextSettings();
            return NULL;
        }
        SmartCardCoreProperties core;
        bool applied = false;
        for (auto type : { VT_BOOL, VT_I4, VT_UI4 }) {
            for (LONG value : { 0L, 1L, -1L }) {
                VARIANT input;
                VariantInit(&input);
                input.vt = static_cast<VARTYPE>(type);
                if (type == VT_BOOL) input.boolVal = static_cast<VARIANT_BOOL>(value);
                else if (type == VT_I4) input.lVal = value;
                else input.ulVal = static_cast<ULONG>(value);
                SmartCardCheck(RdpSettings_SetPasswordContainsSCardPin(&core, &input, &applied) == S_OK,
                    "Valid smart-card property rejected");
                SmartCardCheck(applied == (value != 0) &&
                    core.lastValue == (value ? VARIANT_TRUE : VARIANT_FALSE), "Boolean normalization failed");
            }
        }
        VARIANT input;
        VariantInit(&input);
        input.vt = VT_BOOL;
        input.boolVal = VARIANT_FALSE;
        applied = true;
        core.status = E_ACCESSDENIED;
        SmartCardCheck(RdpSettings_SetPasswordContainsSCardPin(&core, &input, &applied) == E_ACCESSDENIED &&
            applied, "Failed write changed state or hid error");
        SmartCardCheck(RdpSettings_SetPasswordContainsSCardPin(NULL, &input, &applied) == E_UNEXPECTED &&
            applied, "Missing core reported success");
        const int priorWrites = core.writes;
        input.vt = VT_EMPTY;
        SmartCardCheck(RdpSettings_SetPasswordContainsSCardPin(&core, &input, &applied) == E_INVALIDARG &&
            applied && core.writes == priorWrites, "Invalid variant changed state");

        SmartCardTestUnknown unknown;
        GUID session = {};
        CMsRdpExtendedSettings settings(&unknown, &session);
        input.vt = VT_I4;
        input.lVal = 1;
        SmartCardCheck(settings.put_Property(bstr_t(L"PasswordContainsSCardPin"), &input) == E_UNEXPECTED &&
            !settings.GetPasswordContainsSCardPin(), "Extended setter hid missing core");
        MsRdpEx_RdpFile* file = MsRdpEx_RdpFile_New();
        SmartCardCheck(file && MsRdpEx_RdpFile_LoadText(file, "PasswordContainsSCardPin:i:1\r\n"),
            "Cannot create RDP fixture");
        const HRESULT result = settings.ApplyRdpFile(file);
        MsRdpEx_RdpFile_Free(file);
        SmartCardCheck(result == E_UNEXPECTED && !settings.GetPasswordContainsSCardPin(),
            "RDP file did not share setter failure behavior");
        return NULL;
    } catch (const std::exception& error) {
        failure = error.what();
        return failure.c_str();
    }
}
