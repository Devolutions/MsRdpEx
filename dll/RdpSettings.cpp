
#include <MsRdpEx/RdpSettings.h>

#include <MsRdpEx/Memory.h>
#include <MsRdpEx/RdpFile.h>
#include <MsRdpEx/RdpInstance.h>
#include <MsRdpEx/Environment.h>
#include <MsRdpEx/NameResolver.h>
#include <MsRdpEx/Detours.h>

#include <intrin.h>

#include "MsRdpEx.h"
#include "TSObjects.h"
#include "ComHelpers.h"

extern "C" const GUID IID_ITSPropertySet;

extern MsRdpEx_mstscax g_mstscax;
extern MsRdpEx_rdclientax g_rdclientax;

static bool g_TSPropertySet_Hooked = false;

static ITSPropertySet_SetBoolProperty Real_ITSPropertySet_SetBoolProperty = NULL;
static ITSPropertySet_GetBoolProperty Real_ITSPropertySet_GetBoolProperty = NULL;
static ITSPropertySet_SetIntProperty Real_ITSPropertySet_SetIntProperty = NULL;
static ITSPropertySet_GetIntProperty Real_ITSPropertySet_GetIntProperty = NULL;
static ITSPropertySet_SetStringProperty Real_ITSPropertySet_SetStringProperty = NULL;
static ITSPropertySet_GetStringProperty Real_ITSPropertySet_GetStringProperty = NULL;
static ITSPropertySet_SetSecureStringProperty Real_ITSPropertySet_SetSecureStringProperty = NULL;
static ITSPropertySet_GetSecureStringProperty Real_ITSPropertySet_GetSecureStringProperty = NULL;

static HRESULT Hook_ITSPropertySet_SetBoolProperty(ITSPropertySet* This, const char* propName, int propValue)
{
    HRESULT hr;

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetBoolProperty(%s, %d)", propName, propValue);

    if (MsRdpEx_StringIEquals(propName, "UsingSavedCreds")) {
        // Workaround for "Always prompt for password upon connection" GPO":
        // The RDP ActiveX sets the "UsingSavedCreds" to true if the password is set.
        // This is obviously a problem for credential injection, so force it to false.
        // https://theitbros.com/enable-saved-credentials-usage-rdp/
        // https://twitter.com/awakecoding/status/1367953137210957826
        // HKLM:\SOFTWARE\Policies\Microsoft\Windows NT\Terminal Services\fPromptForPassword = 1
        propValue = 0;
    }

    if (MsRdpEx_StringIEquals(propName, "ShellMarkRdpSecure")) {
        // Workaround for RDP file signature requirement.
        // Always mark RDP files as "signed" even when they're not
        propValue = 1;
    }

    if (MsRdpEx_StringIEquals(propName, "ShowRedirectionWarningDialog")) {
        // Workaround for RDP file signature requirement.
        // Disable the redirection warning dialog to avoid some checks.
        propValue = 0;
    }

    hr = Real_ITSPropertySet_SetBoolProperty(This, propName, propValue);

    return hr;
}

static int g_InitializeKDCProxyClient = 0;

static HRESULT Hook_ITSPropertySet_GetBoolProperty(ITSPropertySet* This, const char* propName, int* propValue)
{
    HRESULT hr;

    hr = Real_ITSPropertySet_GetBoolProperty(This, propName, propValue);

    // KDC proxy client hack: oh, the things we wouldn't do for Kerberos!
    // CTscSslFilter::InitializeKDCProxyClient doesn't set KDCProxyName unless TSGTransportIsUsed is true
    // CTsConnectionInfoDlg::GetExpandedInfoString crashes if we set TSGTransportIsUsed true when it's not
    // CTscSslFilter::OnConnected checks IgnoreAuthenticationLevel, NegotiateSecurityLayer right before calling
    // CTscSslFilter::InitializeKDCProxyClient, so use this to our advantage to filter out undesired call sites.
    // We use a basic g_InitializeKDCProxyClient state machine, checking for caller DLLs, and hope for the best.
    if (MsRdpEx_StringIEquals(propName, "IgnoreAuthenticationLevel") &&
        MsRdpEx_IsAddressInRdpAxModule(_ReturnAddress())) {
        if (g_InitializeKDCProxyClient == 0) {
            g_InitializeKDCProxyClient = 1;
        }
    }
    else if (MsRdpEx_StringIEquals(propName, "NegotiateSecurityLayer") &&
        MsRdpEx_IsAddressInRdpAxModule(_ReturnAddress())) {
        if (g_InitializeKDCProxyClient == 1) {
            g_InitializeKDCProxyClient = 2;
        }
    }
    else if (MsRdpEx_StringIEquals(propName, "TSGTransportIsUsed") &&
        MsRdpEx_IsAddressInRdpAxModule(_ReturnAddress())) {
        if (g_InitializeKDCProxyClient == 2) {
            g_InitializeKDCProxyClient = 0;
            *propValue = 1; // bypass if (TSGTransportIsUsed) { /* break Kerberos */ }
            MsRdpEx_LogPrint(TRACE, "TSGTransportIsUsed is a lie!");
        }
    }

    if (MsRdpEx_StringIEquals(propName, "UseNewOutput")) {
        // Workaround to prevent disconnect error code 3334 with latest MSRDC
        *propValue = 1;
    }

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetBoolProperty(%s, %d)", propName, *propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_SetIntProperty(ITSPropertySet* This, const char* propName, int propValue)
{
    HRESULT hr;

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetIntProperty(%s, %d)", propName, propValue);

    hr = Real_ITSPropertySet_SetIntProperty(This, propName, propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_GetIntProperty(ITSPropertySet* This, const char* propName, int* propValue)
{
    HRESULT hr;

    hr = Real_ITSPropertySet_GetIntProperty(This, propName, propValue);

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetIntProperty(%s, %d)", propName, *propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_SetStringProperty(ITSPropertySet* This, const char* propName, WCHAR* propValue)
{
    HRESULT hr;

    char* propValueA = _com_util::ConvertBSTRToString((BSTR) propValue);
    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetStringProperty(%s, \"%s\")", propName, propValueA);
    delete[] propValueA;

    hr = Real_ITSPropertySet_SetStringProperty(This, propName, propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_GetStringProperty(ITSPropertySet* This, const char* propName, WCHAR** propValue)
{
    HRESULT hr;

    hr = Real_ITSPropertySet_GetStringProperty(This, propName, propValue);

    if (SUCCEEDED(hr)) {
        char* propValueA = _com_util::ConvertBSTRToString((BSTR)*propValue);
        MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetStringProperty(%s, \"%s\")", propName, propValueA ? propValueA : "");
        delete[] propValueA;
    }
    else {
        MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetStringProperty(%s), hr = 0x%08X", propName, hr);
    }

    return hr;
}

static HRESULT Hook_ITSPropertySet_SetSecureStringProperty(ITSPropertySet* This, const char* propName, WCHAR* propValue)
{
    HRESULT hr;

    //char* propValueA = _com_util::ConvertBSTRToString((BSTR)propValue);
    //MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetSecureStringProperty(%s, \"%s\")", propName, propValueA);

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::SetSecureStringProperty(%s, \"%s\")", propName, "*omitted*");

    hr = Real_ITSPropertySet_SetSecureStringProperty(This, propName, propValue);

    return hr;
}

static HRESULT Hook_ITSPropertySet_GetSecureStringProperty(ITSPropertySet* This, const char* propName, WCHAR** propValue, uint32_t* propLength)
{
    HRESULT hr;

    hr = Real_ITSPropertySet_GetSecureStringProperty(This, propName, propValue, propLength);

    MsRdpEx_LogPrint(TRACE, "ITSPropertySet::GetSecureStringProperty(%s)", propName);

    return hr;
}

static bool TSPropertySet_Hook(ITSPropertySet* pTSPropertySet, ITSPropertySetVtbl30* vtbl30, ITSPropertySetVtbl32* vtbl32)
{
    LONG error;

    if (!pTSPropertySet || (!vtbl30 && !vtbl32))
        return false;

    DetourRestoreAfterWith();
    DetourTransactionBegin();
    DetourUpdateThread(GetCurrentThread());

    if (vtbl32)
    {
        Real_ITSPropertySet_SetBoolProperty = vtbl32->SetBoolProperty;
        Real_ITSPropertySet_GetBoolProperty = vtbl32->GetBoolProperty;
        Real_ITSPropertySet_SetIntProperty = vtbl32->SetIntProperty;
        Real_ITSPropertySet_GetIntProperty = vtbl32->GetIntProperty;
        Real_ITSPropertySet_SetStringProperty = vtbl32->SetStringProperty;
        Real_ITSPropertySet_GetStringProperty = vtbl32->GetStringProperty;
        Real_ITSPropertySet_SetSecureStringProperty = vtbl32->SetSecureStringProperty;
        Real_ITSPropertySet_GetSecureStringProperty = vtbl32->GetSecureStringProperty;
    }
    else if (vtbl30)
    {
        Real_ITSPropertySet_SetBoolProperty = vtbl30->SetBoolProperty;
        Real_ITSPropertySet_GetBoolProperty = vtbl30->GetBoolProperty;
        Real_ITSPropertySet_SetIntProperty = vtbl30->SetIntProperty;
        Real_ITSPropertySet_GetIntProperty = vtbl30->GetIntProperty;
        Real_ITSPropertySet_SetStringProperty = vtbl30->SetStringProperty;
        Real_ITSPropertySet_GetStringProperty = vtbl30->GetStringProperty;
        Real_ITSPropertySet_SetSecureStringProperty = vtbl30->SetSecureStringProperty;
        Real_ITSPropertySet_GetSecureStringProperty = vtbl30->GetSecureStringProperty;
    }

    DetourAttach((PVOID*)(&Real_ITSPropertySet_SetBoolProperty), Hook_ITSPropertySet_SetBoolProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_GetBoolProperty), Hook_ITSPropertySet_GetBoolProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_SetIntProperty), Hook_ITSPropertySet_SetIntProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_GetIntProperty), Hook_ITSPropertySet_GetIntProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_SetStringProperty), Hook_ITSPropertySet_SetStringProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_GetStringProperty), Hook_ITSPropertySet_GetStringProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_SetSecureStringProperty), Hook_ITSPropertySet_SetSecureStringProperty);
    DetourAttach((PVOID*)(&Real_ITSPropertySet_GetSecureStringProperty), Hook_ITSPropertySet_GetSecureStringProperty);

    error = DetourTransactionCommit();
    return true;
}

static bool IsLikelyVoidMethodNoArgs(void* fn)
{
    if (!fn)
        return false;

    MEMORY_BASIC_INFORMATION mbi = { 0 };
    if (!VirtualQuery(fn, &mbi, sizeof(mbi)))
        return false;

    DWORD prot = mbi.Protect;

    bool isExecutable =
        (prot & PAGE_EXECUTE) ||
        (prot & PAGE_EXECUTE_READ) ||
        (prot & PAGE_EXECUTE_READWRITE) ||
        (prot & PAGE_EXECUTE_WRITECOPY);

    if (!isExecutable || mbi.State != MEM_COMMIT || (prot & PAGE_GUARD))
        return false;

#if defined(_M_AMD64) || defined(__x86_64__)
    uint8_t* code = (uint8_t*)fn;

    size_t available = mbi.RegionSize - ((uintptr_t)fn - (uintptr_t)mbi.BaseAddress);
    if (available < 8)
        return false;

    // Pattern 1: sub rsp, 0x28
    if (code[0] == 0x48 && code[1] == 0x83 && code[2] == 0xEC)
        return true;

    // Pattern 2: push rbp; mov rbp, rsp
    if (code[0] == 0x55 && code[1] == 0x48 && code[2] == 0x89 && code[3] == 0xE5)
        return true;

    // Pattern 3: add rcx, 0x50; jmp rel32 � lock method stubs
    if (code[0] == 0x48 && code[1] == 0x83 && code[2] == 0xC1 && code[3] == 0x50 &&
        code[4] == 0xE9)
        return true;

#elif defined(_M_IX86) || defined(__i386__)
    uint8_t* code = (uint8_t*)fn;

    if ((mbi.RegionSize - ((uintptr_t)fn - (uintptr_t)mbi.BaseAddress)) < 3)
        return false;

    // Typical prologue: push ebp; mov ebp, esp
    if (code[0] == 0x55 && code[1] == 0x8B && code[2] == 0xEC)
        return true;

    // Alternate: sub esp, imm8
    if (code[0] == 0x83 && code[1] == 0xEC)
        return true;

#elif defined(_M_ARM64) || defined(__aarch64__)
    uint32_t* ins = (uint32_t*)fn;

    size_t available = mbi.RegionSize - ((uintptr_t)fn - (uintptr_t)mbi.BaseAddress);
    if (available < 8)
        return false;

    uint32_t instr0 = ins[0];
    uint32_t instr1 = ins[1];

    // Pattern 1: stp x29, x30, [sp, #-16]! ; mov x29, sp
    if (instr0 == 0xA9BF7BF0 && instr1 == 0x910003FD)
        return true;

    // Pattern 2: ret
    if (instr0 == 0xD65F03C0)
        return true;

    // Pattern 3: add x0, x0, #imm ; b target
    if ((instr0 & 0xFFC003FF) == 0x91000000 && // ADD x0, x0, #imm
        (instr1 & 0xFC000000) == 0x14000000)   // B <imm>
        return true;
#endif

    return false;
}

static int TSPropertySet_FindLockFunctionsInVtbl(void** vtbl)
{
    const int blockSize = 4;
    const int maxEntries = 30;

    for (int i = 0; i <= maxEntries - blockSize; i++)
    {
        bool allMatch = true;

        for (int j = 0; j < blockSize; j++) {
            void* fn = vtbl[i + j];

            if (!IsLikelyVoidMethodNoArgs(fn)) {
                allMatch = false;
                break;
            }
        }

        if (allMatch)
            return i; // found the start of a 4-function block
    }

    return -1; // not found
}

static int TSPropertySet_DetectVtblVersion(void** vtbl)
{
    int vtblVersion = -1;
    DWORD ctlVersion = 0;

    if (MsRdpEx_IsAddressInRdclientAxModule(vtbl))
    {
        ctlVersion = g_rdclientax.tscCtlVer;

        if (ctlVersion >= 5326) {
            vtblVersion = 32;
        }
        else {
            vtblVersion = 30;
        }
    }
    else
    {
        ctlVersion = g_mstscax.tscCtlVer;

        if (ctlVersion >= 27842) {
            // First seen in Windows 11 Insider Preview Build 27842
            vtblVersion = 32;
        }
        else {
            vtblVersion = 30;
        }
    }

    /**
     * The vtable contains a block of 4 functions that take void and return void.
     * Detect common assembly pattern matching that simple function prototype,
     * and leverage this to find the offset where those functions begin:
     * 
     * CTSPropertySet::EnterReadLock(void)
     * CTSPropertySet::LeaveReadLock(void)
     * CTSPropertySet::EnterWriteLock(void)
     * CTSPropertySet::LeaveWriteLock(void)
     * 
     * Once we know the offset, use it to detect which vtable we're dealing with
     */

    int lockFunctionsOffset = TSPropertySet_FindLockFunctionsInVtbl(vtbl);

    if (lockFunctionsOffset > 0) {
        MsRdpEx_LogPrint(DEBUG, "Found TSPropertySet lock functions at vtable offset %d", lockFunctionsOffset);

        if (lockFunctionsOffset == 20) {
            vtblVersion = 32;
        }
        else if (lockFunctionsOffset == 18) {
            vtblVersion = 30;
        }
        else {
            MsRdpEx_LogPrint(WARN, "Unknown TSPropertySet lock functions vtable offset: %d", lockFunctionsOffset);
        }
    }

    MsRdpEx_LogPrint(DEBUG, "TSPropertySet ctlVersion: %d vtblVersion: %d", ctlVersion, vtblVersion);

    return vtblVersion;
}

class CMsRdpPropertySet : public IMsRdpExtendedSettings
{
public:
    CMsRdpPropertySet(IUnknown* pUnknown)
    {
        m_refCount = 0;
        m_pUnknown = pUnknown;
        pUnknown->QueryInterface(IID_ITSPropertySet, (LPVOID*)&m_pTSPropertySet);

        if (m_pTSPropertySet)
        {
            int vtblVersion = TSPropertySet_DetectVtblVersion(((void**)m_pTSPropertySet->vtbl));

            if (vtblVersion == 32) {
                m_vtbl32 = (ITSPropertySetVtbl32*)m_pTSPropertySet->vtbl;
                m_vtbl30 = NULL;
            }
            else if (vtblVersion == 30) {
                m_vtbl32 = NULL;
                m_vtbl30 = (ITSPropertySetVtbl30*)m_pTSPropertySet->vtbl;
            }
            else {
                MsRdpEx_LogPrint(ERROR, "Unknown TSPropertySet vtable version: %d", vtblVersion);
            }

            if (!g_TSPropertySet_Hooked) {
                TSPropertySet_Hook(m_pTSPropertySet, m_vtbl30, m_vtbl32);
                g_TSPropertySet_Hooked = true;
            }
        }
    }

    ~CMsRdpPropertySet()
    {
        m_pUnknown->Release();
        if (m_pTSPropertySet) {
            m_pTSPropertySet->vtbl->Release(m_pTSPropertySet);
            m_pTSPropertySet = NULL;
        }
    }

    // IUnknown interface
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(
        REFIID riid,
        LPVOID* ppvObject
    )
    {
        HRESULT hr = E_NOINTERFACE;
        ULONG refCount = m_refCount;
        char iid[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::QueryInterface");

        if (riid == IID_IUnknown)
        {
            *ppvObject = (LPVOID)((IUnknown*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else if ((riid == IID_IMsRdpExtendedSettings) && m_pTSPropertySet)
        {
            *ppvObject = (LPVOID)((IMsRdpExtendedSettings*)this);
            refCount = InterlockedIncrement(&m_refCount);
            hr = S_OK;
        }
        else
        {
            hr = m_pUnknown->QueryInterface(riid, ppvObject);
        }

        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);

        return hr;
    }

    ULONG STDMETHODCALLTYPE AddRef()
    {
        ULONG refCount = InterlockedIncrement(&m_refCount);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::AddRef() = %d", refCount);
        return refCount;
    }

    ULONG STDMETHODCALLTYPE Release()
    {
        ULONG refCount = InterlockedDecrement(&m_refCount);

        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::Release() = %d", refCount);

        if (refCount == 0)
        {
            delete this;
            return 0;
        }

        return refCount;
    }

    // IMsRdpExtendedSettings
public:
    HRESULT __stdcall put_Property(BSTR bstrPropertyName, VARIANT* pValue) {
        char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::put_Property(%s, vt: %d)", propName, pValue->vt);
        
        if (pValue->vt == VT_BOOL)
        {
            return SetVBoolProperty(propName, pValue->boolVal);
        }
        else if (pValue->vt == VT_I4)
        {
            return SetIntProperty(propName, pValue->intVal);
        }
        else if (pValue->vt == VT_UI4)
        {
            return SetIntProperty(propName, pValue->uintVal);
        }
        else if (pValue->vt == VT_BSTR)
        {
            return SetBStrProperty(propName, pValue->bstrVal);
        }

        delete[] propName;

        return E_INVALIDARG;
    }

    HRESULT __stdcall get_Property(BSTR bstrPropertyName, VARIANT* pValue) {
        HRESULT hr = E_INVALIDARG;
        uint8_t propType = 0;
        char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
        MsRdpEx_LogPrint(DEBUG, "CMsRdpPropertySet::get_Property(%s)", propName);

        VariantInit(pValue);

        if (GetTSPropertyType(m_pTSPropertySet, propName, &propType)) {
            if (propType == TSPROPERTY_TYPE_STRING) {
                hr = GetBStrProperty(propName, &pValue->bstrVal);

                if (hr == S_OK) {
                    pValue->vt = VT_BSTR;
                }
            }
            else if (propType == TSPROPERTY_TYPE_BOOL) {
                hr = GetVBoolProperty(propName, &pValue->boolVal);

                if (hr == S_OK) {
                    pValue->vt = VT_BOOL;
                }
            }
        }

        delete[] propName;

        return hr;
    }

    HRESULT __stdcall SetVBoolProperty(const char* propName, VARIANT_BOOL propValue) {
        return m_pTSPropertySet->vtbl->SetBoolProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall SetIntProperty(const char* propName, uint32_t propValue) {
        return m_pTSPropertySet->vtbl->SetIntProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall SetBStrProperty(const char* propName, BSTR propValue) {
        return m_pTSPropertySet->vtbl->SetStringProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall SetSecureStringProperty(const char* propName, BSTR propValue) {
        return m_pTSPropertySet->vtbl->SetSecureStringProperty(m_pTSPropertySet, propName, propValue);
    }

    HRESULT __stdcall GetVBoolProperty(const char* propName, VARIANT_BOOL* propValue) {
        HRESULT hr = E_FAIL;
        int iVal = 0;

        if (m_vtbl32) {
            hr = m_vtbl32->GetBoolProperty(m_pTSPropertySet, propName, &iVal);
        } else if (m_vtbl30) {
            hr = m_vtbl30->GetBoolProperty(m_pTSPropertySet, propName, &iVal);
        }

        *propValue = iVal ? VARIANT_TRUE : VARIANT_FALSE;
        return hr;
    }

    HRESULT __stdcall GetBStrProperty(const char* propName, BSTR* propValue) {
        HRESULT hr = E_FAIL;
        WCHAR* wstrVal = NULL;

        if (m_vtbl32) {
            hr = m_vtbl32->GetStringProperty(m_pTSPropertySet, propName, &wstrVal);
        } else if (m_vtbl30) {
            hr = m_vtbl30->GetStringProperty(m_pTSPropertySet, propName, &wstrVal);
        }

        if (hr != S_OK)
            return hr;

        *propValue = SysAllocString(wstrVal);

        return hr;
    }

private:
    ULONG m_refCount;
    IUnknown* m_pUnknown;
    ITSPropertySet* m_pTSPropertySet;
    ITSPropertySetVtbl30* m_vtbl30 = NULL;
    ITSPropertySetVtbl32* m_vtbl32 = NULL;
};

CMsRdpExtendedSettings::CMsRdpExtendedSettings(IUnknown* pUnknown, GUID* pSessionId)
{
    HRESULT hr;

    m_refCount = 0;
    m_pUnknown = pUnknown;

    MsRdpEx_GuidCopy(&m_sessionId, pSessionId);

    strncpy_s(m_KeyboardHookToggleShortcutKey, "Space", sizeof(m_KeyboardHookToggleShortcutKey) - 1);

    hr = pUnknown->QueryInterface(IID_IMsRdpClient7, (LPVOID*)&m_pMsRdpClient7);

    if (SUCCEEDED(hr) && m_pMsRdpClient7)
        m_pMsRdpClient7->AddRef();

    hr = pUnknown->QueryInterface(IID_IMsRdpExtendedSettings, (LPVOID*)&m_pMsRdpExtendedSettings);

    if (SUCCEEDED(hr) && m_pMsRdpExtendedSettings)
        m_pMsRdpExtendedSettings->AddRef();

    if (m_pMsRdpClient7)
    {
        m_pMsRdpClient7->get_TransportSettings2(&m_pMsRdpClientTransportSettings2);

        if (m_pMsRdpClientTransportSettings2)
            m_pMsRdpClientTransportSettings2->AddRef();
    }
}

CMsRdpExtendedSettings::~CMsRdpExtendedSettings()
{
    this->SetKdcProxyUrl(NULL);
    this->SetRecordingPath(NULL);
    this->SetRecordingPipeName(NULL);

    if (m_pMsRdpExtendedSettings)
        m_pMsRdpExtendedSettings->Release();

    if (m_pMsRdpClientTransportSettings2)
        m_pMsRdpClientTransportSettings2->Release();

    if (m_pMsRdpClient7)
        m_pMsRdpClient7->Release();
}

HRESULT STDMETHODCALLTYPE CMsRdpExtendedSettings::QueryInterface(
    REFIID riid,
    LPVOID* ppvObject
)
{
    HRESULT hr = E_NOINTERFACE;
    ULONG refCount = m_refCount;
    char iid[MSRDPEX_GUID_STRING_SIZE];
    MsRdpEx_GuidBinToStr((GUID*)&riid, iid, 0);

    if (riid == IID_IUnknown)
    {
        *ppvObject = (LPVOID)((IUnknown*)this);
        refCount = InterlockedIncrement(&m_refCount);
        hr = S_OK;
    }
    else if ((riid == IID_IMsRdpExtendedSettings) && m_pMsRdpExtendedSettings)
    {
        *ppvObject = (LPVOID)((IMsRdpExtendedSettings*)this);
        refCount = InterlockedIncrement(&m_refCount);
        hr = S_OK;
    }
    else
    {
        hr = m_pUnknown->QueryInterface(riid, ppvObject);
    }

    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::QueryInterface(%s) = 0x%08X, %d", iid, hr, refCount);

    return hr;
}

ULONG STDMETHODCALLTYPE CMsRdpExtendedSettings::AddRef()
{
    ULONG refCount = InterlockedIncrement(&m_refCount);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::AddRef() = %d", refCount);
    return refCount;
}

ULONG STDMETHODCALLTYPE CMsRdpExtendedSettings::Release()
{
    ULONG refCount = InterlockedDecrement(&m_refCount);

    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::Release() = %d", refCount);

    if (refCount == 0)
    {
        delete this;
        return 0;
    }

    return refCount;
}

HRESULT __stdcall CMsRdpExtendedSettings::put_Property(BSTR bstrPropertyName, VARIANT* pValue) {
    HRESULT hr = E_INVALIDARG;
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::put_Property(%s)", propName);

    if (MsRdpEx_StringEquals(propName, "KDCProxyURL"))
    {
        if (pValue->vt != VT_BSTR)
            goto end;

        char* propValue = _com_util::ConvertBSTRToString(pValue->bstrVal);

        if (propValue) {
            hr = this->SetKdcProxyUrl(propValue);
        }

        delete[] propValue;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "EnableMouseJiggler"))
    {
        if (pValue->vt != VT_BOOL)
            goto end;

        m_MouseJigglerEnabled = pValue->boolVal ? true : false;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "MouseJigglerInterval"))
    {
        if ((pValue->vt != VT_UI4) && (pValue->vt != VT_I4))
            goto end;

        m_MouseJigglerInterval = (uint32_t) pValue->uintVal;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "MouseJigglerMethod"))
    {
        if ((pValue->vt != VT_UI4) && (pValue->vt != VT_I4))
            goto end;

        m_MouseJigglerMethod = (uint32_t) pValue->uintVal;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "KeyboardHookToggleShortcutEnabled"))
    {
        if (pValue->vt != VT_BOOL)
            goto end;

        m_KeyboardHookToggleShortcutEnabled = pValue->boolVal ? true : false;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "KeyboardHookToggleShortcutKey"))
    {
        if (pValue->vt != VT_BSTR)
            goto end;

        char* propValueA = _com_util::ConvertBSTRToString((BSTR)pValue->bstrVal);
        strncpy_s(m_KeyboardHookToggleShortcutKey, propValueA, sizeof(m_KeyboardHookToggleShortcutKey) - 1);
        delete[] propValueA;

        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "OutputMirrorEnabled"))
    {
        if (pValue->vt != VT_BOOL)
            goto end;

        m_OutputMirrorEnabled = pValue->boolVal ? true : false;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "VideoRecordingEnabled"))
    {
        if (pValue->vt != VT_BOOL)
            goto end;

        m_VideoRecordingEnabled = pValue->boolVal ? true : false;

        if (m_VideoRecordingEnabled)
            m_OutputMirrorEnabled = true;

        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "VideoRecordingQuality"))
    {
        if ((pValue->vt != VT_UI4) && (pValue->vt != VT_I4))
            goto end;

        m_VideoRecordingQuality = (uint32_t)pValue->uintVal;

        if (m_VideoRecordingQuality < 1)
            m_VideoRecordingQuality = 1;

        if (m_VideoRecordingQuality > 10)
            m_VideoRecordingQuality = 10;

        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "RecordingPath"))
    {
        if (pValue->vt != VT_BSTR)
            goto end;

        char* propValue = _com_util::ConvertBSTRToString(pValue->bstrVal);

        if (propValue) {
            hr = this->SetRecordingPath(propValue);
        }

        free(propValue);
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "RecordingPipeName"))
    {
        if (pValue->vt != VT_BSTR)
            goto end;

        char* propValue = _com_util::ConvertBSTRToString(pValue->bstrVal);

        if (propValue) {
            hr = this->SetRecordingPipeName(propValue);
        }

        free(propValue);
        hr = S_OK;
        }
    else if (MsRdpEx_StringEquals(propName, "DumpBitmapUpdates"))
    {
        if (pValue->vt != VT_BOOL)
            goto end;

        m_DumpBitmapUpdates = pValue->boolVal ? true : false;

        if (m_DumpBitmapUpdates)
            m_OutputMirrorEnabled = true;

        hr = S_OK;
    }
    else
    {
        if (pValue->vt == VT_BSTR) {
            char* propValueA = _com_util::ConvertBSTRToString((BSTR)pValue->bstrVal);
            MsRdpEx_LogPrint(TRACE, "CMsRdpExtendedSettings::put_Property(%s, \"%s\")", propName, propValueA);
            delete[] propValueA;
        }

        hr = m_pMsRdpExtendedSettings->put_Property(bstrPropertyName, pValue);
    }

end:
    delete[] propName;
    return hr;
}

HRESULT __stdcall CMsRdpExtendedSettings::get_Property(BSTR bstrPropertyName, VARIANT* pValue) {
    HRESULT hr = E_INVALIDARG;
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::get_Property(%s)", propName);

    VariantInit(pValue);

    if (MsRdpEx_StringEquals(propName, "CoreProperties")) {
        if (!m_CoreProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        hr = m_CoreProps->QueryInterface(IID_IUnknown, (LPVOID*) &pValue->punkVal);

        if (pValue->punkVal) {
            pValue->punkVal->AddRef();
        }
    }
    else if (MsRdpEx_StringEquals(propName, "BaseProperties")) {
        if (!m_BaseProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        hr = m_BaseProps->QueryInterface(IID_IUnknown, (LPVOID*)&pValue->punkVal);

        if (pValue->punkVal) {
            pValue->punkVal->AddRef();
        }
    }
    else if (MsRdpEx_StringEquals(propName, "TransportProperties")) {
        if (!m_TransportProps) {
            return E_INVALIDARG;
        }

        pValue->vt = VT_UNKNOWN;
        pValue->punkVal = NULL;
        hr = m_TransportProps->QueryInterface(IID_IUnknown, (LPVOID*)&pValue->punkVal);

        if (pValue->punkVal) {
            pValue->punkVal->AddRef();
        }
    }
    else if (MsRdpEx_StringEquals(propName, "KDCProxyURL")) {
        pValue->vt = VT_BSTR;
        const char* kdcProxyUrl = m_KdcProxyUrl ? m_KdcProxyUrl : "";
        pValue->bstrVal = _com_util::ConvertStringToBSTR(kdcProxyUrl);
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "EnableMouseJiggler")) {
        pValue->vt = VT_BOOL;
        pValue->boolVal = m_MouseJigglerEnabled ? VARIANT_TRUE : VARIANT_FALSE;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "MouseJigglerInterval")) {
        pValue->vt = VT_I4;
        pValue->intVal = (INT) m_MouseJigglerInterval;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "MouseJigglerMethod")) {
        pValue->vt = VT_I4;
        pValue->intVal = (INT) m_MouseJigglerMethod;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "KeyboardHookToggleShortcutEnabled")) {
        pValue->vt = VT_BOOL;
        pValue->boolVal = m_KeyboardHookToggleShortcutEnabled ? VARIANT_TRUE : VARIANT_FALSE;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "KeyboardHookToggleShortcutKey")) {
        pValue->vt = VT_BSTR;
        pValue->bstrVal = _com_util::ConvertStringToBSTR(m_KeyboardHookToggleShortcutKey);
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "VideoRecordingEnabled")) {
        pValue->vt = VT_BOOL;
        pValue->boolVal = m_VideoRecordingEnabled ? VARIANT_TRUE : VARIANT_FALSE;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "VideoRecordingQuality")) {
        pValue->vt = VT_I4;
        pValue->intVal = (INT)m_VideoRecordingQuality;
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "RecordingPath")) {
        pValue->vt = VT_BSTR;
        const char* recordingPath = m_RecordingPath ? m_RecordingPath : "";
        pValue->bstrVal = _com_util::ConvertStringToBSTR(recordingPath);
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "RecordingPipeName")) {
        pValue->vt = VT_BSTR;
        const char* recordingPipeName = m_RecordingPipeName ? m_RecordingPipeName : "";
        pValue->bstrVal = _com_util::ConvertStringToBSTR(recordingPipeName);
        hr = S_OK;
    }
    else if (MsRdpEx_StringEquals(propName, "MsRdpEx_SessionId")) {
        pValue->vt = VT_BSTR;
        char sessionId[MSRDPEX_GUID_STRING_SIZE];
        MsRdpEx_GuidBinToStr((GUID*)&m_sessionId, sessionId, 0);
        pValue->bstrVal = _com_util::ConvertStringToBSTR(sessionId);
        hr = S_OK;
    }
    else {
        hr = m_pMsRdpExtendedSettings->get_Property(bstrPropertyName, pValue);
    }

    delete[] propName;

    return hr;
}

// additional functions

HRESULT __stdcall CMsRdpExtendedSettings::put_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::put_CoreProperty(%s)", propName);
    delete[] propName;

    if (!m_CoreProps)
        return E_INVALIDARG;

    return m_CoreProps->put_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::get_CoreProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::get_CoreProperty(%s)", propName);
    delete[] propName;

    if (!m_CoreProps)
        return E_INVALIDARG;

    return m_CoreProps->get_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::put_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::put_BaseProperty(%s)", propName);
    delete[] propName;

    if (!m_BaseProps)
        return E_INVALIDARG;

    return m_BaseProps->put_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::get_BaseProperty(BSTR bstrPropertyName, VARIANT* pValue) {
    char* propName = _com_util::ConvertBSTRToString(bstrPropertyName);
    MsRdpEx_LogPrint(DEBUG, "CMsRdpExtendedSettings::get_BaseProperty(%s)", propName);
    delete[] propName;

    if (!m_BaseProps)
        return E_INVALIDARG;

    return m_BaseProps->get_Property(bstrPropertyName, pValue);
}

HRESULT __stdcall CMsRdpExtendedSettings::SetTargetPassword(const char* password) {
    if (!m_CoreProps)
        return E_INVALIDARG;

    bstr_t propValue = _com_util::ConvertStringToBSTR(password);
    m_CoreProps->SetSecureStringProperty("Password", propValue);
    SecureZeroMemory(propValue.GetBSTR(), wcslen(propValue.GetBSTR()) * sizeof(WCHAR));

    return S_OK;
}

HRESULT __stdcall CMsRdpExtendedSettings::SetGatewayPassword(const char* password) {
    if (!m_pMsRdpClientTransportSettings2)
        return E_INVALIDARG;

    bstr_t propValue = _com_util::ConvertStringToBSTR(password);
    m_pMsRdpClientTransportSettings2->put_GatewayPassword(propValue);
    SecureZeroMemory(propValue.GetBSTR(), wcslen(propValue.GetBSTR()) * sizeof(WCHAR));

    return S_OK;
}

HRESULT __stdcall CMsRdpExtendedSettings::SetKdcProxyUrl(const char* kdcProxyUrl) {
    free(m_KdcProxyUrl);
    m_KdcProxyUrl = NULL;

    if (kdcProxyUrl) {
        m_KdcProxyUrl = _strdup(kdcProxyUrl);
    }
    return S_OK;
}

HRESULT __stdcall CMsRdpExtendedSettings::SetRecordingPath(const char* recordingPath) {
    free(m_RecordingPath);
    m_RecordingPath = NULL;

    if (recordingPath) {
        m_RecordingPath = _strdup(recordingPath);
    }
    return S_OK;
}

HRESULT __stdcall CMsRdpExtendedSettings::SetRecordingPipeName(const char* recordingPipeName) {
    free(m_RecordingPipeName);
    m_RecordingPipeName = NULL;

    if (recordingPipeName) {
        m_RecordingPipeName = _strdup(recordingPipeName);
    }
    return S_OK;
}

HRESULT CMsRdpExtendedSettings::AttachRdpClient(IMsTscAx* pMsTscAx)
{
    m_pMsTscAx = pMsTscAx;

    ITSObjectBase* pTSWin32CoreApi = NULL;
    ITSPropertySet* pTSCoreProps = NULL;
    ITSPropertySet* pTSBaseProps = NULL;
    ITSPropertySet* pTSTransportProps = NULL;

    for (int i = 0; i < 500; i++) {
        ITSObjectBase** ppTSObject = (ITSObjectBase**)&((size_t*)m_pMsTscAx)[i];
        if (MsRdpEx_CanReadUnsafePtr(ppTSObject, 8)) {
            ITSObjectBase* pTSObject = *ppTSObject;
            if (MsRdpEx_CanReadUnsafePtr(pTSObject, sizeof(ITSObjectBase))) {
                if (pTSObject->marker == TSOBJECT_MARKER) {
                    MsRdpEx_LogPrint(DEBUG, "MsTscAx(%d): 0x%08X name: %s refCount: %d",
                        i, (size_t)pTSObject, pTSObject->name, pTSObject->refCount);

                    if (MsRdpEx_StringEqualsUnsafePtr(pTSObject->name, "CTSPropertySet")) {
                        ITSPropertySet* pTSProps = (ITSPropertySet*)pTSObject;

                        if (!pTSCoreProps && TsPropertyMap_IsCoreProps(pTSProps)) {
                            pTSCoreProps = pTSProps;
                        }
                        else if (!pTSBaseProps && TsPropertyMap_IsBaseProps(pTSProps)) {
                            pTSBaseProps = pTSProps;
                        }
                    }
                    else if (MsRdpEx_StringEqualsUnsafePtr(pTSObject->name, "CTSWin32CoreApi")) {
                        pTSWin32CoreApi = pTSObject;
                    }
                }
            }
        }
    }

    MsRdpEx_LogPrint(DEBUG, "pTSCoreProps: %p", pTSCoreProps);
    MsRdpEx_LogPrint(DEBUG, "pTSBaseProps: %p", pTSBaseProps);
    MsRdpEx_LogPrint(DEBUG, "CTSWin32CoreApi: %p", pTSWin32CoreApi);

    if (pTSCoreProps)
    {
        m_pCorePropsRaw = pTSCoreProps;
        m_CoreProps = new CMsRdpPropertySet((IUnknown*)pTSCoreProps);
        //DumpTSPropertyMap(pTSCoreProps, "Core");
    }

    if (pTSBaseProps)
    {
        m_BaseProps = new CMsRdpPropertySet((IUnknown*)pTSBaseProps);
        //DumpTSPropertyMap(pTSBaseProps, "Base");
    }

    if (pTSTransportProps)
    {
        m_TransportProps = new CMsRdpPropertySet((IUnknown*)pTSTransportProps);
        //DumpTSPropertyMap(pTSTransportProps, "Transport");
    }

    return S_OK;
}

HRESULT CMsRdpExtendedSettings::ApplyRdpFile(void* rdpFilePtr)
{
    MsRdpEx_ArrayListIt* it = NULL;
    MsRdpEx_RdpFileEntry* entry = NULL;
    CMsRdpExtendedSettings* pMsRdpExtendedSettings = this;
    MsRdpEx_RdpFile* rdpFile = (MsRdpEx_RdpFile*) rdpFilePtr;

    it = MsRdpEx_ArrayList_It(rdpFile->entries, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

    while (!MsRdpEx_ArrayListIt_Done(it))
    {
        VARIANT value;
        VariantInit(&value);

        entry = (MsRdpEx_RdpFileEntry*)MsRdpEx_ArrayListIt_Next(it);

        if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "DisableCredentialsDelegation")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->PutProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "RedirectedAuthentication")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->PutProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "RestrictedLogon")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->PutProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "AuthenticationServiceClass")) {
            bstr_t propName = _com_util::ConvertStringToBSTR("AuthServiceClass");
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "UserSpecifiedServerName")) {
            bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ClientDeviceName")) {
            bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ConnectionBarText")) {
            bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_BaseProperty(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "DisableUDPTransport")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "AutoReconnectEnabled")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "MaxReconnectAttempts")) {
            if (MsRdpEx_RdpFileEntry_GetIntValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR("MaxAutoReconnectAttempts");
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "ConnectToChildSession")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "EnableFrameBufferRedirection")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "EnableHardwareMode")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "AllowBackgroundInput")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_BaseProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "EnableRelativeMouse")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_BaseProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "EnableMouseJiggler")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "MouseJigglerInterval")) {
            if (MsRdpEx_RdpFileEntry_GetIntValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "MouseJigglerMethod")) {
            if (MsRdpEx_RdpFileEntry_GetIntValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "KeyboardHookToggleShortcutEnabled")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "KeyboardHookToggleShortcutKey")) {
            bstr_t propName = _com_util::ConvertStringToBSTR("KeyboardHookToggleShortcutKey");
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_Property(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "TargetUserName")) {
            bstr_t propName = _com_util::ConvertStringToBSTR("UserName");
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "TargetDomain")) {
            bstr_t propName = _com_util::ConvertStringToBSTR("Domain");
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "ClearTextPassword")) {
            pMsRdpExtendedSettings->SetTargetPassword(entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "PasswordContainsSCardPin")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_CoreProperty(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "GatewayUserName")) {
            if (m_pMsRdpClientTransportSettings2) {
                bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
                m_pMsRdpClientTransportSettings2->put_GatewayUsername(propValue);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "GatewayDomain")) {
            if (m_pMsRdpClientTransportSettings2) {
                bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
                m_pMsRdpClientTransportSettings2->put_GatewayDomain(propValue);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "GatewayPassword")) {
            pMsRdpExtendedSettings->SetGatewayPassword(entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "KDCProxyURL")) {
            pMsRdpExtendedSettings->SetKdcProxyUrl(entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "WinSCardCertificateFilePath")) {
            MsRdpEx_SetEnv("WINSCARD_CERTIFICATE_FILE_PATH", entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "WinSCardCertificateFileData")) {
            MsRdpEx_SetEnv("WINSCARD_CERTIFICATE_FILE_DATA", entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "WinSCardPrivateKeyFilePath")) {
            MsRdpEx_SetEnv("WINSCARD_PRIVATE_KEY_FILE_PATH", entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "WinSCardPrivateKeyFileData")) {
            MsRdpEx_SetEnv("WINSCARD_PRIVATE_KEY_FILE_DATA", entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "WinSCardSmartcardContainerName")) {
            MsRdpEx_SetEnv("WINSCARD_SMARTCARD_CONTAINER_NAME", entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "WinSCardSmartcardReaderName")) {
            MsRdpEx_SetEnv("WINSCARD_SMARTCARD_READER_NAME", entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "WinSCardSmartcardPin")) {
            MsRdpEx_SetEnv("WINSCARD_SMARTCARD_PIN", entry->value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "VideoRecordingEnabled")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "VideoRecordingQuality")) {
            if (MsRdpEx_RdpFileEntry_GetIntValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "RecordingPath")) {
            bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_Property(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 's', "RecordingPipeName")) {
            bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
            bstr_t propValue = _com_util::ConvertStringToBSTR(entry->value);
            value.bstrVal = propValue;
            value.vt = VT_BSTR;
            pMsRdpExtendedSettings->put_Property(propName, &value);
        }
        else if (MsRdpEx_RdpFileEntry_IsMatch(entry, 'i', "DumpBitmapUpdates")) {
            if (MsRdpEx_RdpFileEntry_GetVBoolValue(entry, &value)) {
                bstr_t propName = _com_util::ConvertStringToBSTR(entry->name);
                pMsRdpExtendedSettings->put_Property(propName, &value);
            }
        }
    }

    MsRdpEx_ArrayListIt_Finish(it);

    return S_OK;
}

HRESULT CMsRdpExtendedSettings::LoadRdpFile(const char* rdpFileName)
{
    char* filename;

    if (rdpFileName)
        filename = _strdup(rdpFileName);
    else
        filename = MsRdpEx_GetRdpFilenameFromCommandLine();

    if (!filename)
        return E_UNEXPECTED;

    MsRdpEx_LogPrint(DEBUG, "Loading RDP file: %s", filename);
    MsRdpEx_RdpFile* rdpFile = MsRdpEx_RdpFile_New();

    if (MsRdpEx_RdpFile_Load(rdpFile, filename))
    {
        this->ApplyRdpFile(rdpFile);
    }

    MsRdpEx_RdpFile_Free(rdpFile);
    free(filename);
    return S_OK;
}

HRESULT CMsRdpExtendedSettings::LoadRdpFileFromNamedPipe(const char* pipeName)
{
    char* pipeNameEnv = MsRdpEx_GetEnv("MSRDPEX_SECURE_PIPE_NAME");

    if (pipeNameEnv)
        pipeName = pipeNameEnv;

    if (!pipeName)
        return S_OK; // no named pipe to read from

    MsRdpEx_LogPrint(DEBUG, "Loading RDP file from named pipe: %s", pipeName);
    MsRdpEx_RdpFile* rdpFile = MsRdpEx_RdpFile_New();

    char* pipeText = MsRdpEx_ReadTextFromNamedPipe(pipeName);

    if (pipeText && MsRdpEx_RdpFile_LoadText(rdpFile, pipeText))
    {
        this->ApplyRdpFile(rdpFile);
    }

    MsRdpEx_RdpFile_Free(rdpFile);

    free(pipeNameEnv);

    if (pipeText) {
        SecureZeroMemory(pipeText, strlen(pipeText));
        free(pipeText);
    }

    return S_OK;
}

HRESULT CMsRdpExtendedSettings::GetCorePropsRawPtr(LPVOID* ppCorePropsRaw)
{
    *ppCorePropsRaw = m_pCorePropsRaw;
    return S_OK;
}

HRESULT CMsRdpExtendedSettings::PrepareSspiSessionIdHack()
{
    HRESULT hr = S_OK;
    char fakeKdcProxyName[256];
    char sessionId[MSRDPEX_GUID_STRING_SIZE];

    if (!m_CoreProps) {
        MsRdpEx_LogPrint(ERROR, "PrepareSspiSessionIdHack - m_CoreProps is NULL!");
        return E_UNEXPECTED;
    }

    MsRdpEx_GuidBinToStr((GUID*)&m_sessionId, sessionId, 0);
    sprintf_s(fakeKdcProxyName, sizeof(fakeKdcProxyName) - 1, "MsRdpEx/%s", sessionId);

    BSTR fakeKdcProxyNameB = _com_util::ConvertStringToBSTR(fakeKdcProxyName);
    m_CoreProps->SetBStrProperty("KDCProxyName", fakeKdcProxyNameB);
    SysFreeString(fakeKdcProxyNameB);

    return hr;
}

HRESULT CMsRdpExtendedSettings::PrepareMouseJiggler()
{
    HRESULT hr = S_OK;
    VARIANT enableMouseJiggler;
    VARIANT allowBackgroundInput;
    bstr_t enableMouseJigglerName = _com_util::ConvertStringToBSTR("EnableMouseJiggler");
    bstr_t allowBackgroundInputName = _com_util::ConvertStringToBSTR("AllowBackgroundInput");

    if (!m_BaseProps) {
        MsRdpEx_LogPrint(ERROR, "PrepareMouseJiggler - m_BaseProps is NULL!");
        return E_UNEXPECTED;
    }

    VariantInit(&enableMouseJiggler);
    VariantInit(&allowBackgroundInput);
    hr = this->get_Property(enableMouseJigglerName, &enableMouseJiggler);
    hr = this->get_BaseProperty(allowBackgroundInputName, &allowBackgroundInput);

    if (enableMouseJiggler.boolVal == VARIANT_TRUE) {
        allowBackgroundInput.boolVal = VARIANT_TRUE;
        m_BaseProps->put_Property(allowBackgroundInputName, &allowBackgroundInput);
    }

    return hr;
}

HRESULT CMsRdpExtendedSettings::PrepareVideoRecorder()
{
    HRESULT hr = S_OK;
    IMsRdpExInstance* instance;
    bool outputMirrorEnabled = false;

    instance = (IMsRdpExInstance*) MsRdpEx_InstanceManager_FindBySessionId(&m_sessionId);

    if (!instance)
    {
        MsRdpEx_LogPrint(ERROR, "PrepareVideoRecorder - cannot find instance!");
        return E_UNEXPECTED;
    }

    instance->GetOutputMirrorEnabled(&outputMirrorEnabled);

    if (outputMirrorEnabled) {
        VARIANT enableHardwareMode;
        bstr_t enableHardwareModeName = _com_util::ConvertStringToBSTR("EnableHardwareMode");
        VariantInitBool(&enableHardwareMode, false);
        this->put_Property(enableHardwareModeName, &enableHardwareMode);
    }

    return hr;
}

HRESULT CMsRdpExtendedSettings::PrepareExtraSystemMenu()
{
    HRESULT hr = S_OK;
    VARIANT allowBackgroundInput;
    bstr_t allowBackgroundInputName = _com_util::ConvertStringToBSTR("AllowBackgroundInput");

    if (!m_BaseProps) {
        MsRdpEx_LogPrint(ERROR, "PrepareExtraSystemMenu - m_BaseProps is NULL!");
        return E_UNEXPECTED;
    }

    VariantInit(&allowBackgroundInput);
    hr = this->get_BaseProperty(allowBackgroundInputName, &allowBackgroundInput);

    if (m_ExtraSystemMenuEnabled) {
        allowBackgroundInput.boolVal = VARIANT_TRUE;
        m_BaseProps->put_Property(allowBackgroundInputName, &allowBackgroundInput);
    }

    return hr;
}

char* CMsRdpExtendedSettings::GetKdcProxyUrl()
{
    if (m_KdcProxyUrl)
        return _strdup(m_KdcProxyUrl);

    return NULL;
}

char* CMsRdpExtendedSettings::GetKdcProxyName()
{
    return MsRdpEx_KdcProxyUrlToName(m_KdcProxyUrl);
}

bool CMsRdpExtendedSettings::GetMouseJigglerEnabled()
{
    return m_MouseJigglerEnabled;
}

uint32_t CMsRdpExtendedSettings::GetMouseJigglerInterval()
{
    return m_MouseJigglerInterval;
}

uint32_t CMsRdpExtendedSettings::GetMouseJigglerMethod()
{
    return m_MouseJigglerMethod;
}

bool CMsRdpExtendedSettings::GetKeyboardHookToggleShortcutEnabled()
{
    return m_KeyboardHookToggleShortcutEnabled;
}

const char* CMsRdpExtendedSettings::GetKeyboardHookToggleShortcutKey()
{
    return (const char*) m_KeyboardHookToggleShortcutKey;
}

const char* CMsRdpExtendedSettings::GetSessionId()
{
    MsRdpEx_GuidBinToStr((GUID*)&m_sessionId, m_sessionIdStr, 0);
    return m_sessionIdStr;
}

bool CMsRdpExtendedSettings::GetOutputMirrorEnabled()
{
    return m_OutputMirrorEnabled;
}

bool CMsRdpExtendedSettings::GetVideoRecordingEnabled()
{
    return m_VideoRecordingEnabled;
}

uint32_t CMsRdpExtendedSettings::GetVideoRecordingQuality()
{
    return m_VideoRecordingQuality;
}

char* CMsRdpExtendedSettings::GetRecordingPath()
{
    if (m_RecordingPath)
        return _strdup(m_RecordingPath);

    return NULL;
}

char* CMsRdpExtendedSettings::GetRecordingPipeName()
{
    if (m_RecordingPipeName)
        return _strdup(m_RecordingPipeName);

    return NULL;
}

bool CMsRdpExtendedSettings::GetDumpBitmapUpdates()
{
    return m_DumpBitmapUpdates;
}

bool CMsRdpExtendedSettings::GetExtraSystemMenuEnabled()
{
    return m_ExtraSystemMenuEnabled;
}

CMsRdpExtendedSettings* CMsRdpExtendedSettings_New(IUnknown* pUnknown, IUnknown* pMsTscAx, GUID* pSessionId)
{
    CMsRdpExtendedSettings* settings = new CMsRdpExtendedSettings(pUnknown, pSessionId);
    settings->AttachRdpClient((IMsTscAx*) pMsTscAx);
    return settings;
}

char* MsRdpEx_KdcProxyUrlToName(const char* kdcProxyUrl)
{
    char* path = NULL;
    const char* host = NULL;
    char* kdcProxyName = NULL;

    // https://<host>[:<port>][/path]
    // <host>:[:<port>][:<path>]

    if (!kdcProxyUrl)
        return NULL;

    host = strstr(kdcProxyUrl, "://");

    if (!host)
        return NULL;

    host = &host[3];

    kdcProxyName = _strdup(host);

    if (!kdcProxyName)
        return NULL;

    path = (char*)strchr(kdcProxyName, '/');

    if (path)
        *path = ':';

    return kdcProxyName;
}
