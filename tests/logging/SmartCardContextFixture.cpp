#include "SmartCardTestSupport.h"
#include <wincred.h>
#include <wincrypt.h>
#include <ncrypt.h>
#include <winscard.h>
#include <string>

namespace
{
struct Fixture
{
    bool cng = false;
    bool missingCertificate = false;
    bool duplicateCertificate = false;
    bool unsupported = false;
    bool mismatch = false;
    bool cardChanged = false;
    bool multipleCardNames = false;
    bool malformedBinding = false;
    bool sharedState = false;
    DWORD bindingFlags = 0;
    DWORD keySpec = AT_KEYEXCHANGE;
    LONG readerError = SCARD_S_SUCCESS;
    SECURITY_STATUS keyError = ERROR_SUCCESS;
    unsigned present = 1;
    unsigned reads = 0;
    unsigned opens = 0;
    unsigned calls = 0;
    int resources = 0;
} fixture;

BYTE encodedCertificate[] = { 1, 2, 3 };
CERT_CONTEXT fixtureCertificate = { X509_ASN_ENCODING, encodedCertificate, sizeof(encodedCertificate) };
const WCHAR readerList[] = L"reader-one\0reader-two\0";

BOOL WINAPI TestCertFree(PCCERT_CONTEXT)
{
    --fixture.resources;
    return TRUE;
}

HCERTSTORE WINAPI TestCertOpen(LPCSTR, DWORD, HCRYPTPROV_LEGACY, DWORD flags, const void* name)
{
    ++fixture.calls;
    SmartCardCheck((flags & CERT_STORE_READONLY_FLAG) && wcscmp((LPCWSTR)name, L"MY") == 0,
        "Certificate store was not read-only MY");
    ++fixture.resources;
    return reinterpret_cast<HCERTSTORE>(1);
}

BOOL WINAPI TestCertClose(HCERTSTORE, DWORD) { --fixture.resources; return TRUE; }
PCCERT_CONTEXT WINAPI TestCertDuplicate(PCCERT_CONTEXT cert) { ++fixture.resources; return cert; }
PCCERT_CONTEXT WINAPI TestCertFind(HCERTSTORE, DWORD, DWORD, DWORD type, const void* value, PCCERT_CONTEXT previous)
{
    SmartCardCheck(type == CERT_FIND_SHA1_HASH && static_cast<const CRYPT_HASH_BLOB*>(value)->cbData == 20,
        "Lookup did not use selected certificate hash");
    if (previous) TestCertFree(previous);
    if (fixture.missingCertificate || (previous && !fixture.duplicateCertificate)) {
        SetLastError(static_cast<DWORD>(CRYPT_E_NOT_FOUND));
        return NULL;
    }
    ++fixture.resources;
    return &fixtureCertificate;
}

BOOL WINAPI TestCertProperty(PCCERT_CONTEXT, DWORD property, void* buffer, DWORD* size)
{
    SmartCardCheck(property == CERT_KEY_PROV_INFO_PROP_ID, "Unexpected certificate property");
    const WCHAR* provider = fixture.unsupported ? L"unverified-vendor" :
        (fixture.cng ? MS_SMART_CARD_KEY_STORAGE_PROVIDER : MS_SCARD_PROV_W);
    const WCHAR* container = L"fixture-container";
    const DWORD required = sizeof(CRYPT_KEY_PROV_INFO) +
        static_cast<DWORD>((wcslen(provider) + wcslen(container) + 2) * sizeof(WCHAR));
    if (buffer) {
        SmartCardCheck(*size >= required, "Short provider binding buffer");
        auto* info = static_cast<CRYPT_KEY_PROV_INFO*>(buffer);
        ZeroMemory(info, required);
        info->pwszContainerName = reinterpret_cast<WCHAR*>(info + 1);
        wcscpy_s(info->pwszContainerName, wcslen(container) + 1, container);
        info->pwszProvName = info->pwszContainerName + wcslen(container) + 1;
        wcscpy_s(info->pwszProvName, wcslen(provider) + 1, provider);
        info->dwProvType = fixture.cng ? 0 : PROV_RSA_FULL;
        info->dwKeySpec = fixture.keySpec;
        info->dwFlags = fixture.bindingFlags;
        if (fixture.malformedBinding)
            info->pwszProvName = reinterpret_cast<LPWSTR>(1);
    }
    *size = required;
    return TRUE;
}

LONG WINAPI TestEstablish(DWORD scope, LPCVOID, LPCVOID, LPSCARDCONTEXT context)
{
    SmartCardCheck(scope == SCARD_SCOPE_USER, "Wrong smart-card scope");
    ++fixture.resources;
    *context = 1;
    return SCARD_S_SUCCESS;
}
LONG WINAPI TestRelease(SCARDCONTEXT) { --fixture.resources; return SCARD_S_SUCCESS; }
LONG WINAPI TestListReaders(SCARDCONTEXT, LPCWSTR, LPWSTR readers, LPDWORD size)
{
    if (fixture.readerError != SCARD_S_SUCCESS) return fixture.readerError;
    if (readers) {
        SmartCardCheck(*size >= ARRAYSIZE(readerList), "Short reader buffer");
        CopyMemory(readers, readerList, sizeof(readerList));
    }
    *size = ARRAYSIZE(readerList);
    return SCARD_S_SUCCESS;
}
LONG WINAPI TestCardStatus(SCARDCONTEXT, DWORD timeout, LPSCARD_READERSTATEW state, DWORD count)
{
    SmartCardCheck(timeout == 0 && count == 1, "Resolver waited for a card");
    const bool present = fixture.present > 0 &&
        (fixture.present > 1 || wcscmp(state->szReader, L"reader-one") == 0);
    state->dwEventState = SCARD_STATE_CHANGED | (present ? SCARD_STATE_PRESENT : SCARD_STATE_EMPTY);
    if (fixture.cardChanged && fixture.reads >= 2)
        state->dwEventState |= 0x10000;
    if (fixture.sharedState && fixture.reads >= 2)
        state->dwEventState |= SCARD_STATE_INUSE;
    state->cbAtr = present ? 1 : 0;
    state->rgbAtr[0] = 0x3B;
    ++fixture.reads;
    return SCARD_S_SUCCESS;
}
LONG WINAPI TestListCards(SCARDCONTEXT, LPCBYTE, LPCGUID, DWORD, LPWSTR names, LPDWORD size)
{
    const WCHAR* list = fixture.multipleCardNames ? L"card-one\0card-two\0" : L"card-one\0";
    const DWORD chars = fixture.multipleCardNames ? 19 : 10;
    SmartCardCheck(*size >= chars, "Short card names buffer");
    CopyMemory(names, list, chars * sizeof(WCHAR));
    *size = chars;
    return SCARD_S_SUCCESS;
}

void CheckScopedName(LPCWSTR name)
{
    SmartCardCheck(wcscmp(name, L"\\\\.\\reader-one\\fixture-container") == 0,
        "Key acquisition was not reader and container scoped");
    ++fixture.opens;
}
BOOL WINAPI TestAcquire(HCRYPTPROV* handle, LPCWSTR container, LPCWSTR provider, DWORD type, DWORD flags)
{
    CheckScopedName(container);
    SmartCardCheck(wcscmp(provider, MS_SCARD_PROV_W) == 0 &&
        type == PROV_RSA_FULL && flags == CRYPT_SILENT, "CAPI was not silent or changed provider");
    if (fixture.keyError) { SetLastError(fixture.keyError); return FALSE; }
    ++fixture.resources;
    *handle = 2;
    return TRUE;
}
BOOL WINAPI TestReleaseProvider(HCRYPTPROV, DWORD) { --fixture.resources; return TRUE; }
BOOL WINAPI TestUserKey(HCRYPTPROV, DWORD spec, HCRYPTKEY* key)
{
    SmartCardCheck(spec == fixture.keySpec, "Changed legacy key specification");
    ++fixture.resources;
    *key = 3;
    return TRUE;
}
BOOL WINAPI TestDestroyKey(HCRYPTKEY) { --fixture.resources; return TRUE; }
BOOL ReadCertificate(BYTE* buffer, DWORD* size)
{
    if (buffer) {
        SmartCardCheck(*size >= sizeof(encodedCertificate), "Short certificate buffer");
        CopyMemory(buffer, encodedCertificate, sizeof(encodedCertificate));
        if (fixture.mismatch) buffer[0] ^= 0xFF;
    }
    *size = sizeof(encodedCertificate);
    return TRUE;
}
BOOL WINAPI TestKeyParam(HCRYPTKEY, DWORD property, BYTE* buffer, DWORD* size, DWORD flags)
{
    SmartCardCheck(property == KP_CERTIFICATE && !flags, "Unexpected CAPI operation");
    return ReadCertificate(buffer, size);
}
SECURITY_STATUS WINAPI TestOpenProvider(NCRYPT_PROV_HANDLE* provider, LPCWSTR name, DWORD flags)
{
    SmartCardCheck(wcscmp(name, MS_SMART_CARD_KEY_STORAGE_PROVIDER) == 0 && !flags,
        "Changed KSP provider");
    ++fixture.resources;
    *provider = 4;
    return ERROR_SUCCESS;
}
SECURITY_STATUS WINAPI TestOpenKey(NCRYPT_PROV_HANDLE, NCRYPT_KEY_HANDLE* key, LPCWSTR name,
    DWORD spec, DWORD flags)
{
    CheckScopedName(name);
    SmartCardCheck(spec == fixture.keySpec && flags == NCRYPT_SILENT_FLAG, "CNG key was not opened silently");
    if (fixture.keyError) return fixture.keyError;
    ++fixture.resources;
    *key = 5;
    return ERROR_SUCCESS;
}
SECURITY_STATUS WINAPI TestGetProperty(NCRYPT_HANDLE, LPCWSTR name, PBYTE buffer, DWORD capacity,
    DWORD* size, DWORD flags)
{
    SmartCardCheck(wcscmp(name, NCRYPT_CERTIFICATE_PROPERTY) == 0 && flags == NCRYPT_SILENT_FLAG,
        "Unexpected CNG operation");
    *size = capacity;
    ReadCertificate(buffer, size);
    return ERROR_SUCCESS;
}
SECURITY_STATUS WINAPI TestFreeObject(NCRYPT_HANDLE) { --fixture.resources; return ERROR_SUCCESS; }
}

// All device/store/provider boundaries are replaced; these tests cannot access a physical card.
#define CertOpenStore TestCertOpen
#define CertCloseStore TestCertClose
#define CertFindCertificateInStore TestCertFind
#define CertDuplicateCertificateContext TestCertDuplicate
#define CertFreeCertificateContext TestCertFree
#define CertGetCertificateContextProperty TestCertProperty
#define SCardEstablishContext TestEstablish
#define SCardReleaseContext TestRelease
#define SCardListReadersW TestListReaders
#define SCardGetStatusChangeW TestCardStatus
#define SCardListCardsW TestListCards
#define CryptAcquireContextW TestAcquire
#define CryptReleaseContext TestReleaseProvider
#define CryptGetUserKey TestUserKey
#define CryptDestroyKey TestDestroyKey
#define CryptGetKeyParam TestKeyParam
#define NCryptOpenStorageProvider TestOpenProvider
#define NCryptOpenKey TestOpenKey
#define NCryptGetProperty TestGetProperty
#define NCryptFreeObject TestFreeObject
#include "../../dll/SmartCardContext.cpp"

static MsRdpEx_SmartCardContext RunResolve(const WCHAR* user, MsRdpEx_SmartCardContextMode mode)
{
    MsRdpEx_SmartCardContext context;
    context.mode = mode;
    MsRdpEx_ResolveSmartCardContext(user, &context);
    SmartCardCheck(fixture.resources == 0, "Resolver leaked an OS resource");
    return context;
}

extern "C" const char* RunSmartCardContextTest(const char* scenario)
{
    static std::string failure;
    try {
        if (strcmp(scenario, "context-modes") == 0) {
            for (const WCHAR* value : { L"", L"off", L"OFF", L"observe", L"apply", L"invalid",
                    L"far-too-long-to-be-a-mode" }) {
                SetEnvironmentVariableW(L"MSRDPEX_SSPI_SMARTCARD_CONTEXT", value);
                const auto expected = wcscmp(value, L"observe") == 0 ? MsRdpEx_SmartCardContextMode::Observe :
                    wcscmp(value, L"apply") == 0 ? MsRdpEx_SmartCardContextMode::Apply : MsRdpEx_SmartCardContextMode::Off;
                SmartCardCheck(MsRdpEx_GetSmartCardContextMode() == expected, "Wrong mode parsing");
            }
            SetEnvironmentVariableW(L"MSRDPEX_SSPI_SMARTCARD_CONTEXT", NULL);
            SmartCardCheck(MsRdpEx_GetSmartCardContextMode() == MsRdpEx_SmartCardContextMode::Off, "Unset mode active");
            RunResolve(L"not-a-certificate", MsRdpEx_SmartCardContextMode::Off);
            SmartCardCheck(fixture.calls == 0, "Off mode touched certificate store");
            return NULL;
        }
        for (const char* name : { "MSRDPEX_WINSCARD_DLL", "WINSCARD_SMARTCARD_READER_NAME",
                "WINSCARD_SMARTCARD_CONTAINER_NAME" })
            SetEnvironmentVariableA(name, NULL);
        CERT_CREDENTIAL_INFO reference = {};
        reference.cbSize = sizeof(reference);
        reference.rgbHashOfCert[0] = 42;
        LPWSTR marshaled = NULL;
        SmartCardCheck(CredMarshalCredentialW(CertCredential, &reference, &marshaled) != FALSE,
            "Cannot marshal certificate reference");
        const std::wstring user = marshaled;
        CredFree(marshaled);
        for (bool cng : { false, true }) {
            for (auto mode : { MsRdpEx_SmartCardContextMode::Observe, MsRdpEx_SmartCardContextMode::Apply }) {
                fixture = Fixture();
                fixture.cng = cng;
                fixture.keySpec = cng ? 0UL : AT_KEYEXCHANGE;
                const auto result = RunResolve(user.c_str(), mode);
                SmartCardCheck(result.status == MsRdpEx_SmartCardContextStatus::Ready &&
                    result.keySpec == fixture.keySpec &&
                    result.certificateHash[0] == 42 && wcscmp(result.reader, L"reader-one") == 0 &&
                    wcscmp(result.card, L"card-one") == 0 && fixture.opens == 1, "Verified context not resolved");
            }
        }
        for (int test = 0; test < 14; ++test) {
            fixture = Fixture();
            auto expected = MsRdpEx_SmartCardContextStatus::Error;
            switch (test) {
                case 0: fixture.missingCertificate = true; expected = MsRdpEx_SmartCardContextStatus::NoMatch; break;
                case 1: fixture.duplicateCertificate = true; expected = MsRdpEx_SmartCardContextStatus::Ambiguous; break;
                case 2: fixture.unsupported = true; expected = MsRdpEx_SmartCardContextStatus::Unsupported; break;
                case 3: fixture.present = 0; expected = MsRdpEx_SmartCardContextStatus::NoMatch; break;
                case 4: fixture.present = 2; expected = MsRdpEx_SmartCardContextStatus::Ambiguous; break;
                case 5: fixture.multipleCardNames = true; expected = MsRdpEx_SmartCardContextStatus::Ambiguous; break;
                case 6: fixture.mismatch = true; expected = MsRdpEx_SmartCardContextStatus::NoMatch; break;
                case 7: fixture.cardChanged = true; expected = MsRdpEx_SmartCardContextStatus::NoMatch; break;
                case 8: fixture.keyError = NTE_SILENT_CONTEXT; break;
                case 9: fixture.cng = true; fixture.keyError = NTE_SILENT_CONTEXT; break;
                case 10: fixture.readerError = SCARD_E_NO_SERVICE; break;
                case 11: fixture.bindingFlags = CRYPT_MACHINE_KEYSET; expected = MsRdpEx_SmartCardContextStatus::Unsupported; break;
                case 12: fixture.malformedBinding = true; break;
                case 13: fixture.cng = true; fixture.keySpec = CERT_NCRYPT_KEY_SPEC; expected = MsRdpEx_SmartCardContextStatus::Unsupported; break;
            }
            const auto result = RunResolve(user.c_str(), MsRdpEx_SmartCardContextMode::Apply);
            SmartCardCheck(result.status == expected, "Resolver misclassified failure/ambiguity");
            if (test < 6 || test >= 10)
                SmartCardCheck(fixture.opens == 0, "Ineligible context opened a key");
        }
        for (bool cng : { false, true }) {
            fixture = Fixture();
            fixture.cng = cng;
            fixture.sharedState = true;
            fixture.keySpec = AT_SIGNATURE;
            fixture.bindingFlags = CRYPT_SILENT | CERT_SET_KEY_CONTEXT_PROP_ID;
            const auto result = RunResolve(user.c_str(), MsRdpEx_SmartCardContextMode::Apply);
            SmartCardCheck(result.status == MsRdpEx_SmartCardContextStatus::Ready && result.keySpec == AT_SIGNATURE,
                "Reader shared-use state or valid legacy KeySpec rejected");
        }
        fixture = Fixture();
        SetEnvironmentVariableW(L"MSRDPEX_WINSCARD_DLL", L"fixture-shim");
        SmartCardCheck(RunResolve(user.c_str(), MsRdpEx_SmartCardContextMode::Apply).status ==
            MsRdpEx_SmartCardContextStatus::Unsupported && !fixture.calls, "Emulated reader was probed");
        SetEnvironmentVariableW(L"MSRDPEX_WINSCARD_DLL", NULL);
        SmartCardCheck(RunResolve(L"ordinary-user", MsRdpEx_SmartCardContextMode::Apply).status ==
            MsRdpEx_SmartCardContextStatus::Ineligible && !fixture.calls, "Ordinary user was probed");
        return NULL;
    } catch (const std::exception& error) {
        failure = error.what();
        return failure.c_str();
    }
}
