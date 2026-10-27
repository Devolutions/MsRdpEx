#include "SmartCardTestSupport.h"
#include <MsRdpEx/Sspi.h>
#include <ntsecapi.h>
#include <wincrypt.h>
#include <ncrypt.h>
#include <string>
#include <vector>
#include <thread>
#include <map>
#include <climits>

static BOOL WINAPI TestPack(DWORD, LPWSTR, LPWSTR, PBYTE, DWORD*);
static BOOL WINAPI TestUnpack(DWORD, PVOID, DWORD, LPWSTR, DWORD*, LPWSTR, DWORD*, LPWSTR, DWORD*);
static HLOCAL WINAPI TestLocalAlloc(UINT, SIZE_T);
static HLOCAL WINAPI TestLocalFree(HLOCAL);

// Replace only the OS packing/allocation boundary in this private fixture.
#define CredPackAuthenticationBufferW TestPack
#define CredUnPackAuthenticationBufferW TestUnpack
#define LocalAlloc TestLocalAlloc
#define LocalFree TestLocalFree
#include "../../dll/Sspi.cpp"
#undef LocalFree
#undef LocalAlloc
#undef CredUnPackAuthenticationBufferW
#undef CredPackAuthenticationBufferW

enum class PackResult { Success, ProbeFailure, SmallProbe, AllocationFailure, PackFailure, WrongType, Oversize, Shrink };
static PackResult packResult = PackResult::Success;
static int packCalls = 0;
static int unpackCalls = 0;
static HLOCAL allocation = NULL;
static SIZE_T allocationSize = 0;
static bool scrubbed = false;
static std::map<HLOCAL, SIZE_T> allocations;
static bool allScrubbed = true;
static std::wstring unpackUser;
static std::wstring unpackPin = L"fixture-pin";
static bool protectedInput = false;
static std::string failure;
static bool contextPayload = false;
static std::vector<BYTE> MakeContextPayload(DWORD spec);

static HLOCAL WINAPI TestLocalAlloc(UINT flags, SIZE_T size)
{
    if (packResult == PackResult::AllocationFailure) {
        SetLastError(ERROR_NOT_ENOUGH_MEMORY);
        return NULL;
    }
    allocation = LocalAlloc(flags, size);
    allocationSize = size;
    scrubbed = false;
    if (allocation)
        allocations[allocation] = size;
    return allocation;
}

static HLOCAL WINAPI TestLocalFree(HLOCAL memory)
{
    const auto found = allocations.find(memory);
    if (found != allocations.end()) {
        scrubbed = true;
        for (SIZE_T i = 0; i < found->second; ++i)
            if (static_cast<BYTE*>(memory)[i] != 0)
                scrubbed = false;
        allScrubbed = allScrubbed && scrubbed;
        allocations.erase(found);
        if (memory == allocation)
            allocation = NULL;
    }
    return LocalFree(memory);
}

static BOOL WINAPI TestPack(DWORD flags, LPWSTR, LPWSTR pin, PBYTE buffer, DWORD* size)
{
    ++packCalls;
    SmartCardCheck(flags == 0, "Packing flags changed");
    if (contextPayload) {
        const auto payload = MakeContextPayload(1);
        const DWORD capacity = *size;
        *size = static_cast<DWORD>(payload.size());
        if (!buffer) {
            SetLastError(ERROR_INSUFFICIENT_BUFFER);
            return FALSE;
        }
        SmartCardCheck(capacity >= payload.size(), "Short realistic packing allocation");
        CopyMemory(buffer, payload.data(), payload.size());
        return TRUE;
    }
    const DWORD required = sizeof(KERB_CERTIFICATE_LOGON) + 128;
    if (!buffer) {
        *size = packResult == PackResult::SmallProbe ? sizeof(DWORD) : required;
        SetLastError(packResult == PackResult::ProbeFailure ? ERROR_INVALID_DATA : ERROR_INSUFFICIENT_BUFFER);
        return FALSE;
    }
    SmartCardCheck(*size == required, "Unexpected packing allocation size");
    auto* logon = reinterpret_cast<KERB_CERTIFICATE_LOGON*>(buffer);
    logon->MessageType = packResult == PackResult::WrongType ? KerbInteractiveLogon : KerbCertificateLogon;
    const size_t bytes = (wcslen(pin) + 1) * sizeof(WCHAR);
    SmartCardCheck(bytes <= 128, "Fixture PIN too long");
    CopyMemory(buffer + sizeof(*logon), pin, bytes);
    if (packResult == PackResult::PackFailure) {
        *size = required * 2;
        SetLastError(ERROR_INSUFFICIENT_BUFFER);
        return FALSE;
    }
    if (packResult == PackResult::Oversize)
        *size = required + 1;
    if (packResult == PackResult::Shrink)
        *size = sizeof(*logon);
    return TRUE;
}

static BOOL WINAPI TestUnpack(DWORD flags, PVOID, DWORD, LPWSTR user, DWORD* userSize,
    LPWSTR domain, DWORD* domainSize, LPWSTR pin, DWORD* pinSize)
{
    ++unpackCalls;
    if (protectedInput && flags != CRED_PACK_PROTECTED_CREDENTIALS) {
        SetLastError(ERROR_NOT_SUPPORTED);
        return FALSE;
    }
    const DWORD requiredUser = static_cast<DWORD>(unpackUser.size() + 1);
    const DWORD requiredPin = static_cast<DWORD>(unpackPin.size() + 1);
    if (!user || !domain || !pin) {
        *userSize = requiredUser;
        *domainSize = 1;
        *pinSize = requiredPin;
        SetLastError(ERROR_INSUFFICIENT_BUFFER);
        return FALSE;
    }
    SmartCardCheck(*userSize >= requiredUser && *domainSize >= 1 && *pinSize >= requiredPin,
        "Unpack capacities changed");
    wcscpy_s(user, *userSize, unpackUser.c_str());
    domain[0] = L'\0';
    wcscpy_s(pin, *pinSize, unpackPin.c_str());
    return TRUE;
}

static void SetEnabled(CMsRdpExtendedSettings& settings, bool enabled)
{
    VARIANT value;
    VariantInit(&value);
    value.vt = VT_BOOL;
    value.boolVal = enabled ? VARIANT_TRUE : VARIANT_FALSE;
    SmartCardCheck(settings.put_Property(bstr_t(L"KerbCertificateLogon"), &value) == S_OK, "Cannot set opt-in");
}

static void CheckPreserved(void* auth, CMsRdpExtendedSettings* settings)
{
    SspiCertificateLogonRewrite rewrite = {};
    const char* builder = NULL;
    const int priorPack = packCalls;
    const int priorUnpack = unpackCalls;
    SmartCardCheck(!sspi_TryBuildCertificateLogon(auth, settings, &rewrite, &builder), "Unexpected rewrite");
    SmartCardCheck(!rewrite.active && !rewrite.pAuthData && !rewrite.packedCredentials,
        "Pass-through owns or substitutes caller data");
    SmartCardCheck(packCalls == priorPack && unpackCalls == priorUnpack, "Pass-through attempted packing");
    SmartCardCheck(builder && !builder[0], "Pass-through reported a builder");
    sspi_FreeCertificateLogonRewrite(&rewrite);
}

static void TestPreservation(CMsRdpExtendedSettings& settings)
{
    std::vector<BYTE> payload(sizeof(KERB_CERTIFICATE_LOGON) + 160, 0x5A);
    auto* logon = reinterpret_cast<KERB_CERTIFICATE_LOGON*>(payload.data());
    ZeroMemory(logon, sizeof(*logon));
    logon->MessageType = KerbCertificateLogon;
    logon->Flags = 0x1234;
    logon->CspDataLength = 160;
    logon->CspData = reinterpret_cast<PUCHAR>(sizeof(*logon));
    const auto original = payload;
    int schannel = 1;
    settings.SetCapturedPin(L"fixture-pin");
    for (auto type : { CredsspCertificateCreds, CredsspSubmitBufferBoth, CredsspSubmitBufferBothOld }) {
        CREDSSP_CRED simple = { type, &schannel, payload.data() };
        CREDSSP_CRED originalSimple = simple;
        CheckPreserved(&simple, &settings);
        SmartCardCheck(memcmp(&simple, &originalSimple, sizeof(simple)) == 0, "Simple wrapper modified");

        CREDSSP_CRED_EX extended = {};
        extended.Type = CredsspCredEx;
        extended.Version = CREDSSP_CRED_EX_VERSION;
        extended.Flags = 0xA5;
        extended.Reserved = 0x5A;
        extended.Cred = simple;
        CREDSSP_CRED_EX originalExtended = extended;
        CheckPreserved(&extended, &settings);
        SmartCardCheck(memcmp(&extended, &originalExtended, sizeof(extended)) == 0, "Extended wrapper modified");
        SmartCardCheck(payload == original, "Certificate metadata modified");
    }
}

static void TestUnsupported(CMsRdpExtendedSettings& settings)
{
    CheckPreserved(NULL, &settings);
    CREDSSP_CRED unknown = { static_cast<CREDSPP_SUBMIT_TYPE>(999), NULL, NULL };
    CheckPreserved(&unknown, &settings);
    CREDSSP_CRED_EX extended = {};
    extended.Type = CredsspCredEx;
    extended.Version = CREDSSP_CRED_EX_VERSION + 1;
    extended.Cred.Type = CredsspSubmitBufferBoth;
    CheckPreserved(&extended, &settings);

    SYSTEM_INFO systemInfo;
    GetSystemInfo(&systemInfo);
    auto* pages = static_cast<BYTE*>(VirtualAlloc(NULL, systemInfo.dwPageSize * 2,
        MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE));
    SmartCardCheck(pages != NULL, "Cannot allocate guarded fixture");
    DWORD oldProtection;
    SmartCardCheck(VirtualProtect(pages + systemInfo.dwPageSize, systemInfo.dwPageSize,
        PAGE_NOACCESS, &oldProtection) != FALSE, "Cannot protect fixture page");
    CheckPreserved(pages + systemInfo.dwPageSize, &settings);
    auto* truncated = reinterpret_cast<DWORD*>(pages + systemInfo.dwPageSize - sizeof(DWORD));
    *truncated = CredsspSubmitBufferBoth;
    CheckPreserved(truncated, &settings);
    *truncated = KerbCertificateLogon;
    CREDSSP_CRED simple = { CredsspSubmitBufferBoth, NULL, truncated };
    CheckPreserved(&simple, &settings);
    simple.pSpnegoCred = pages + systemInfo.dwPageSize;
    CheckPreserved(&simple, &settings);
    VirtualFree(pages, 0, MEM_RELEASE);
}

static void TestRewrite(CMsRdpExtendedSettings& settings, bool extendedWrapper,
    CREDSPP_SUBMIT_TYPE inputType = CredsspSubmitBufferBoth)
{
    HLOCAL input = LocalAlloc(LMEM_FIXED | LMEM_ZEROINIT, 128);
    SmartCardCheck(input != NULL, "Cannot allocate fixture input");
    *static_cast<DWORD*>(input) = KerbInteractiveLogon;
    int schannel = 1;
    CREDSSP_CRED_EX extended = {};
    extended.Type = CredsspCredEx;
    extended.Version = CREDSSP_CRED_EX_VERSION;
    extended.Flags = 0xA5;
    extended.Reserved = 0x5A;
    extended.Cred = { inputType, &schannel, input };
    const CREDSSP_CRED_EX original = extended;
    void* auth = extendedWrapper ? static_cast<void*>(&extended) : &extended.Cred;
    SspiCertificateLogonRewrite rewrite = {};
    const char* builder = NULL;
    SmartCardCheck(sspi_TryBuildCertificateLogon(auth, &settings, &rewrite, &builder), "Incoming rewrite failed");
    SmartCardCheck(strcmp(builder, "incoming-auth-data") == 0, "Unexpected builder");
    SmartCardCheck(rewrite.cred.Type == CredsspSubmitBufferBoth &&
        rewrite.cred.pSchannelCred == &schannel && rewrite.cred.pSpnegoCred == allocation,
        "Rewritten envelope lost Schannel or certificate");
    SmartCardCheck(rewrite.pAuthData == (extendedWrapper ? static_cast<void*>(&rewrite.credEx) : &rewrite.cred),
        "Wrong rewritten wrapper pointer");
    if (extendedWrapper) {
        CREDSSP_CRED_EX expected = original;
        expected.Cred = rewrite.cred;
        SmartCardCheck(memcmp(&expected, &rewrite.credEx, sizeof(expected)) == 0, "Extended fields lost");
    }
    SmartCardCheck(memcmp(&extended, &original, sizeof(extended)) == 0, "Caller wrapper changed");
    sspi_FreeCertificateLogonRewrite(&rewrite);
    SmartCardCheck(scrubbed && !allocation && !rewrite.active, "Owned credential not scrubbed and freed");
    SmartCardCheck(*static_cast<DWORD*>(input) == KerbInteractiveLogon, "Caller credential changed");
    LocalFree(input);
}

static void TestEligibility(CMsRdpExtendedSettings& settings)
{
    HLOCAL input = LocalAlloc(LMEM_FIXED | LMEM_ZEROINIT, 128);
    SmartCardCheck(input != NULL, "Cannot allocate fixture input");
    *static_cast<DWORD*>(input) = KerbInteractiveLogon;
    CREDSSP_CRED simple = { CredsspSubmitBufferBoth, NULL, input };
    CheckPreserved(&simple, NULL);
    SetEnabled(settings, false);
    CheckPreserved(&simple, &settings);
    SetEnabled(settings, true);

    for (const auto* pin : { L"", L"fixture-pin" }) {
        unpackPin = pin;
        if (unpackPin.size())
            unpackUser = L"ordinary-user";
        SspiCertificateLogonRewrite rewrite = {};
        const char* builder = NULL;
        const int priorPack = packCalls;
        SmartCardCheck(!sspi_TryBuildCertificateLogon(&simple, &settings, &rewrite, &builder),
            "Empty PIN or ordinary password rewritten");
        SmartCardCheck(packCalls == priorPack && !rewrite.active, "Ineligible credential reached packer");
    }
    LocalFree(input);
}

static void TestPacking()
{
    for (auto result : { PackResult::ProbeFailure, PackResult::SmallProbe, PackResult::AllocationFailure,
            PackResult::PackFailure, PackResult::WrongType, PackResult::Oversize, PackResult::Shrink }) {
        packResult = result;
        HLOCAL packed = NULL;
        DWORD size = 0;
        WCHAR user[] = L"fixture-certificate";
        WCHAR pin[] = L"fixture-pin";
        bool built = sspi_CreatePackedCertificateLogon(user, pin, &packed, &size);
        if (result == PackResult::Shrink) {
            SmartCardCheck(built && size == allocationSize, "Cleanup lost the full allocation extent");
            SspiCertificateLogonRewrite rewrite = {};
            rewrite.packedCredentials = packed;
            rewrite.cbPackedCredentials = size;
            sspi_FreeCertificateLogonRewrite(&rewrite);
        } else {
            SmartCardCheck(!built && !packed && !size, "Failed pack reported success");
        }
        if (result == PackResult::PackFailure || result == PackResult::WrongType ||
            result == PackResult::Oversize || result == PackResult::Shrink)
            SmartCardCheck(scrubbed && !allocation, "Packing failure leaked or failed to scrub its buffer");
    }
}

static void* acquiredAuth = NULL;
static bool expectReplacement = false;
static SECURITY_STATUS acquireResult = SEC_E_OK;

static SECURITY_STATUS SEC_ENTRY TestAcquire(SEC_WCHAR*, SEC_WCHAR*, ULONG, void*, void* auth,
    SEC_GET_KEY_FN, void*, PCredHandle, PTimeStamp)
{
    acquiredAuth = auth;
    if (expectReplacement) {
        auto* cred = static_cast<CREDSSP_CRED*>(auth);
        SmartCardCheck(cred && cred->Type == CredsspSubmitBufferBoth &&
            cred->pSpnegoCred == allocation && allocation, "Acquisition did not receive a live packed credential");
        SmartCardCheck(static_cast<KERB_CERTIFICATE_LOGON*>(allocation)->MessageType == KerbCertificateLogon,
            "Acquisition received the wrong credential");
    }
    return acquireResult;
}

static void TestAcquisition(CREDSPP_SUBMIT_TYPE inputType = CredsspSubmitBufferBoth)
{
    SmartCardTestUnknown unknown;
    IMsRdpExInstance* instance = CreatePluginReferenceInstance();
    SmartCardCheck(instance != NULL, "Cannot create fixture instance");
    GUID session = {};
    instance->GetSessionId(&session);
    auto* settings = new CMsRdpExtendedSettings(&unknown, &session);
    SetEnabled(*settings, true);
    instance->AttachExtendedSettings(settings);
    settings->Release();
    SmartCardCheck(RegisterPluginReferenceInstance(instance), "Cannot register fixture instance");
    MsRdpEx_Sspi_BeginSession(&session);
    Real_AcquireCredentialsHandleW = TestAcquire;

    HLOCAL input = LocalAlloc(LMEM_FIXED | LMEM_ZEROINIT, sizeof(KERB_CERTIFICATE_LOGON));
    SmartCardCheck(input != NULL, "Cannot allocate acquisition input");
    int schannel = 1;
    CREDSSP_CRED cred = { inputType, &schannel, input };
    WCHAR package[] = L"CREDSSP";
    CredHandle handle = {};
    for (auto status : { SEC_E_OK, SEC_E_INVALID_TOKEN }) {
        acquireResult = status;
        *static_cast<DWORD*>(input) = KerbCertificateLogon;
        expectReplacement = false;
        const int priorPack = packCalls;
        SmartCardCheck(sspi_AcquireCredentialsHandleW(NULL, package, SECPKG_CRED_OUTBOUND, NULL,
            &cred, NULL, NULL, &handle, NULL) == status, "Pass-through changed acquisition status");
        SmartCardCheck(acquiredAuth == &cred && packCalls == priorPack, "Acquisition changed caller pointer");

        *static_cast<DWORD*>(input) = KerbInteractiveLogon;
        expectReplacement = true;
        SmartCardCheck(sspi_AcquireCredentialsHandleW(NULL, package, SECPKG_CRED_OUTBOUND, NULL,
            &cred, NULL, NULL, &handle, NULL) == status, "Rewrite changed acquisition status");
        SmartCardCheck(acquiredAuth != &cred && !allocation && scrubbed,
            "Acquisition did not release and scrub its temporary credential");
        SmartCardCheck(cred.pSpnegoCred == input && cred.pSchannelCred == &schannel &&
            *static_cast<DWORD*>(input) == KerbInteractiveLogon, "Acquisition changed caller-owned data");
    }
    LocalFree(input);
    MsRdpEx_Sspi_EndSession(&session);
    SmartCardCheck(UnregisterPluginReferenceInstance(instance), "Cannot unregister fixture instance");
    instance->Release();
}

static void TestLegacyWrapper(CMsRdpExtendedSettings& settings)
{
    // RDM's baseline supplies type 51 with an opaque non-certificate payload.
    DWORD opaqueMarker = 0x12345678;
    int schannel = 1;
    CREDSSP_CRED original = { CredsspSubmitBufferBothOld, &schannel, &opaqueMarker };
    CREDSSP_CRED* input = NULL;
    CREDSSP_CRED_EX* extended = NULL;
    SmartCardCheck(sspi_GetCertificateLogonRewriteInput(&original, &input, &extended) &&
        input == &original && !extended, "Baseline type-51 wrapper incorrectly excluded from synthesis");
    TestRewrite(settings, false, CredsspSubmitBufferBothOld);
    TestRewrite(settings, true, CredsspSubmitBufferBothOld);
    TestAcquisition(CredsspSubmitBufferBothOld);
}

static void TestSessionScopes()
{
    GUID first = { 1 }, second = { 2 }, resolved = {};
    MsRdpEx_SspiSessionResolution resolution;
    MsRdpEx_Sspi_BeginSession(&first);
    MsRdpEx_Sspi_BeginSession(&second);
    SmartCardCheck(sspi_GetCurrentSessionId(&resolved, &resolution) && IsEqualGUID(resolved, second),
        "Nested scope selected wrong session");
    MsRdpEx_Sspi_EndSession(&first);
    SmartCardCheck(sspi_GetCurrentSessionId(&resolved, &resolution) && IsEqualGUID(resolved, second),
        "Out-of-order end discarded live scope");
    bool bound = false;
    std::thread worker([&] {
        MsRdpEx_Sspi_BindCurrentThreadToSession(&second);
        GUID workerId = {};
        bound = sspi_GetCurrentSessionId(&workerId, NULL) && IsEqualGUID(workerId, second);
    });
    worker.join();
    SmartCardCheck(bound, "Worker binding failed");
    MsRdpEx_Sspi_EndSession(&second);
    SmartCardCheck(!sspi_GetCurrentSessionId(&resolved, NULL), "Ended session retained a binding");
}

static std::vector<BYTE> MakeContextPayload(DWORD spec)
{
    const WCHAR* names[] = { L"", L"", L"fixture-container", spec ? MS_SCARD_PROV_W : MS_SMART_CARD_KEY_STORAGE_PROVIDER };
    const DWORD cspOffset = sizeof(KERB_CERTIFICATE_LOGON) + 32;
    DWORD cspSize = MSRDPEX_CSP_INFO_HEADER_SIZE;
    for (const auto* name : names)
        cspSize += static_cast<DWORD>((wcslen(name) + 1) * sizeof(WCHAR));
    std::vector<BYTE> payload(cspOffset + cspSize);
    BYTE* memory = payload.data();
    auto* logon = reinterpret_cast<KERB_CERTIFICATE_LOGON*>(memory);
    logon->MessageType = KerbCertificateLogon;
    logon->DomainName = { 2, 4, reinterpret_cast<PWSTR>(sizeof(*logon)) };
    logon->UserName = { 2, 4, reinterpret_cast<PWSTR>(sizeof(*logon) + 4) };
    logon->Pin = { 12, 14, reinterpret_cast<PWSTR>(sizeof(*logon) + 8) };
    CopyMemory((BYTE*)memory + sizeof(*logon), L"D\0U\0secret", sizeof(L"D\0U\0secret"));
    logon->CspData = reinterpret_cast<PUCHAR>((ULONG_PTR)cspOffset);
    logon->CspDataLength = cspSize;
    auto* csp = reinterpret_cast<MsRdpEx_KerbSmartCardCspInfo*>((BYTE*)memory + cspOffset);
    csp->dwCspInfoLen = cspSize;
    csp->MessageType = 1;
    csp->KeySpec = spec;
    ULONG* offsets[] = { &csp->nCardNameOffset, &csp->nReaderNameOffset,
        &csp->nContainerNameOffset, &csp->nCSPNameOffset };
    DWORD offset = 0;
    for (int i = 0; i < 4; ++i) {
        *offsets[i] = offset / sizeof(WCHAR);
        const DWORD bytes = static_cast<DWORD>((wcslen(names[i]) + 1) * sizeof(WCHAR));
        CopyMemory((BYTE*)&csp->bBuffer + offset, names[i], bytes);
        offset += bytes;
    }
    return payload;
}

static void TestContextPacking()
{
    for (DWORD spec : { 0UL, 1UL, 2UL }) {
        MsRdpEx_SmartCardContext context;
        context.mode = MsRdpEx_SmartCardContextMode::Apply;
        context.status = MsRdpEx_SmartCardContextStatus::Ready;
        context.keySpec = spec;
        SmartCardCheck(MsRdpEx_GetSmartCardCertificateHash(unpackUser.c_str(), context.certificateHash),
            "Cannot decode fixture certificate");
        wcscpy_s(context.card, L"fixture-card");
        wcscpy_s(context.reader, L"fixture-reader");
        wcscpy_s(context.container, L"fixture-container");
        wcscpy_s(context.provider, spec ? MS_SCARD_PROV_W : MS_SMART_CARD_KEY_STORAGE_PROVIDER);
        for (int test = 0; test < 16; ++test) {
            packResult = PackResult::Success;
            auto payload = MakeContextPayload(spec);
            if (test == 15) {
                payload.resize(payload.size() + 4);
                auto* header = reinterpret_cast<KERB_CERTIFICATE_LOGON*>(payload.data());
                header->CspDataLength += 4;
                auto* extension = reinterpret_cast<MsRdpEx_KerbSmartCardCspInfo*>(
                    payload.data() + (ULONG_PTR)header->CspData);
                extension->dwCspInfoLen += 4;
                payload.back() = 0xA5;
            }
            DWORD size = static_cast<DWORD>(payload.size());
            HLOCAL packed = TestLocalAlloc(LMEM_FIXED | LMEM_ZEROINIT, size);
            SmartCardCheck(packed != NULL, "Cannot allocate context fixture");
            CopyMemory(packed, payload.data(), size);
            auto candidate = context;
            auto* logon = static_cast<KERB_CERTIFICATE_LOGON*>(packed);
            auto* csp = reinterpret_cast<MsRdpEx_KerbSmartCardCspInfo*>((BYTE*)packed + (ULONG_PTR)logon->CspData);
            switch (test) {
                case 1: candidate.mode = MsRdpEx_SmartCardContextMode::Observe; break;
                case 2: candidate.mode = MsRdpEx_SmartCardContextMode::Off; break;
                case 3: candidate.status = MsRdpEx_SmartCardContextStatus::Ambiguous; break;
                case 4: candidate.certificateHash[0] ^= 0xFF; break;
                case 5: candidate.keySpec ^= 3; break;
                case 6: candidate.container[0] = L'x'; break;
                case 7: logon->Pin.Buffer = (PWSTR)logon; break;
                case 8: csp->nReaderNameOffset = MAXDWORD; break;
                case 9: csp->flags = 1; break;
                case 10: packResult = PackResult::AllocationFailure; break;
                case 11: ((BYTE*)packed)[size - 1] = 1; break; // Missing provider terminator.
                case 12: logon->CspData = reinterpret_cast<PUCHAR>((ULONG_PTR)size); break;
                case 13: logon->Pin.MaximumLength = USHRT_MAX; break;
                case 14: logon->Flags = 2; break;
            }
            const std::vector<BYTE> original((BYTE*)packed, (BYTE*)packed + size);
            const HLOCAL old = packed;
            const DWORD oldSize = size;
            const bool applied = sspi_ApplySmartCardContext(candidate, unpackUser.c_str(), &packed, &size);
            SmartCardCheck(applied == (test == 0), "Wrong candidate application outcome");
            if (applied) {
                SmartCardCheck(packed != old && size > oldSize && allScrubbed, "Old packed credential not scrubbed");
                auto expectedHeader = *reinterpret_cast<const KERB_CERTIFICATE_LOGON*>(original.data());
                const auto* actual = static_cast<KERB_CERTIFICATE_LOGON*>(packed);
                expectedHeader.CspData = actual->CspData;
                expectedHeader.CspDataLength = actual->CspDataLength;
                SmartCardCheck(memcmp(&expectedHeader, actual, sizeof(expectedHeader)) == 0 &&
                    memcmp((BYTE*)packed + sizeof(expectedHeader), original.data() + sizeof(expectedHeader),
                        oldSize - sizeof(expectedHeader)) == 0, "Candidate changed PIN, hints, flags or original payload");
                const auto* newCsp = reinterpret_cast<const MsRdpEx_KerbSmartCardCspInfo*>(
                    (BYTE*)packed + (ULONG_PTR)actual->CspData);
                const WCHAR* reader = NULL;
                DWORD chars = 0;
                SmartCardCheck(newCsp->dwCspInfoLen == actual->CspDataLength &&
                    newCsp->KeySpec == spec &&
                    sspi_GetCspInfoName(newCsp, actual->CspDataLength, newCsp->nReaderNameOffset, &reader, &chars) &&
                    wcscmp(reader, context.reader) == 0, "Candidate CSP metadata invalid");
            } else {
                SmartCardCheck(packed == old && size == oldSize && memcmp(packed, original.data(), size) == 0,
                    "Skipped candidate changed original synthesis");
            }
            SecureZeroMemory(packed, size);
            TestLocalFree(packed);
            SmartCardCheck(allocations.empty() && allScrubbed, "Context experiment leaked credential bytes");
        }
    }
    packResult = PackResult::Success;
    contextPayload = true;
    MsRdpEx_SmartCardContext context;
    context.mode = MsRdpEx_SmartCardContextMode::Apply;
    context.status = MsRdpEx_SmartCardContextStatus::Ready;
    context.keySpec = 1;
    MsRdpEx_GetSmartCardCertificateHash(unpackUser.c_str(), context.certificateHash);
    wcscpy_s(context.card, L"fixture-card");
    wcscpy_s(context.reader, L"fixture-reader");
    wcscpy_s(context.container, L"fixture-container");
    wcscpy_s(context.provider, MS_SCARD_PROV_W);
    HLOCAL packed = NULL;
    DWORD size = 0;
    WCHAR pin[] = L"secret";
    SmartCardCheck(sspi_CreatePackedCertificateLogon(&unpackUser[0], pin, &packed, &size, &context),
        "Shared packing boundary rejected candidate");
    SmartCardCheck(size > MakeContextPayload(1).size() &&
        static_cast<KERB_CERTIFICATE_LOGON*>(packed)->CspDataLength > 40,
        "Shared packer did not apply snapshot");
    SspiCertificateLogonRewrite rewrite = {};
    rewrite.packedCredentials = packed;
    rewrite.cbPackedCredentials = size;
    sspi_FreeCertificateLogonRewrite(&rewrite);
    SmartCardCheck(allocations.empty() && allScrubbed, "Shared candidate packing leaked credentials");
    contextPayload = false;
}

extern "C" const char* RunSmartCardSspiTest(const char* scenario)
{
    try {
        SmartCardTestUnknown unknown;
        GUID session = {};
        CMsRdpExtendedSettings settings(&unknown, &session);
        SetEnabled(settings, true);
        CERT_CREDENTIAL_INFO certificate = {};
        certificate.cbSize = sizeof(certificate);
        certificate.rgbHashOfCert[0] = 1;
        LPWSTR marshaled = NULL;
        SmartCardCheck(CredMarshalCredentialW(CertCredential, &certificate, &marshaled) != FALSE,
            "Cannot marshal fixture certificate reference");
        unpackUser = marshaled;
        CredFree(marshaled);
        if (strcmp(scenario, "preservation") == 0) TestPreservation(settings);
        else if (strcmp(scenario, "unsupported") == 0) TestUnsupported(settings);
        else if (strcmp(scenario, "rewrite") == 0) {
            TestRewrite(settings, false);
            protectedInput = true;
            TestRewrite(settings, true);
        }
        else if (strcmp(scenario, "eligibility") == 0) TestEligibility(settings);
        else if (strcmp(scenario, "packing") == 0) TestPacking();
        else if (strcmp(scenario, "acquisition") == 0) TestAcquisition();
        else if (strcmp(scenario, "legacy-wrapper") == 0) TestLegacyWrapper(settings);
        else if (strcmp(scenario, "sessions") == 0) TestSessionScopes();
        else if (strcmp(scenario, "context-packing") == 0) TestContextPacking();
        else throw std::runtime_error("Unknown smart-card SSPI scenario");
        return NULL;
    } catch (const std::exception& error) {
        failure = error.what();
        return failure.c_str();
    }
}
