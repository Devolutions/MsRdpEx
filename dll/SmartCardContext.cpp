#include <MsRdpEx/MsRdpEx.h>
#include <MsRdpEx/Environment.h>
#include <MsRdpEx/SmartCardContext.h>
#include <wincred.h>
#include <wincrypt.h>
#include <ncrypt.h>
#include <winscard.h>
#include <vector>
#include <string>
#include <new>

namespace
{
struct CardResources
{
    SCARDCONTEXT cards = 0;
    HCERTSTORE store = NULL;
    PCCERT_CONTEXT certificate = NULL;
    HCRYPTPROV capi = 0;
    HCRYPTKEY capiKey = 0;
    NCRYPT_PROV_HANDLE cng = 0;
    NCRYPT_KEY_HANDLE cngKey = 0;

    ~CardResources()
    {
        if (cngKey) NCryptFreeObject(cngKey);
        if (cng) NCryptFreeObject(cng);
        if (capiKey) CryptDestroyKey(capiKey);
        if (capi) CryptReleaseContext(capi, 0);
        if (certificate) CertFreeCertificateContext(certificate);
        if (store) CertCloseStore(store, 0);
        if (cards) SCardReleaseContext(cards);
    }
};

static void Fail(MsRdpEx_SmartCardContext& result, MsRdpEx_SmartCardContextStatus status, DWORD error)
{
    result.status = status;
    result.error = error;
}

template<size_t N>
static bool CopyName(WCHAR (&output)[N], const WCHAR* input)
{
    if (!input || !input[0] || wcsnlen(input, N) == N)
        return false;
    return wcscpy_s(output, input) == 0;
}

static bool SameCard(const SCARD_READERSTATEW& first, const SCARD_READERSTATEW& second)
{
    // Include the insertion/removal event counter, not just ATR (two cards may have the same ATR).
    const DWORD identity = 0xFFFF0000 | SCARD_STATE_PRESENT | SCARD_STATE_EMPTY;
    return (first.dwEventState & identity) == (second.dwEventState & identity) &&
        first.cbAtr == second.cbAtr && memcmp(first.rgbAtr, second.rgbAtr, first.cbAtr) == 0;
}

static bool BindingName(const std::vector<BYTE>& binding, const WCHAR* value)
{
    const ULONG_PTR base = reinterpret_cast<ULONG_PTR>(binding.data());
    const ULONG_PTR address = reinterpret_cast<ULONG_PTR>(value);
    if (address < base || address - base < sizeof(CRYPT_KEY_PROV_INFO) ||
        address - base >= binding.size() || address % sizeof(WCHAR))
        return false;
    const size_t available = (binding.size() - (address - base)) / sizeof(WCHAR);
    return available && wcsnlen(value, available) < available;
}

static bool ReadSingleCard(SCARDCONTEXT cards, MsRdpEx_SmartCardContext& result,
    SCARD_READERSTATEW& selected)
{
    DWORD chars = 0;
    LONG status = SCardListReadersW(cards, NULL, NULL, &chars);
    if (status == SCARD_E_NO_READERS_AVAILABLE) {
        Fail(result, MsRdpEx_SmartCardContextStatus::NoMatch, status);
        return false;
    }
    if (status != SCARD_S_SUCCESS || chars < 2 || chars > 32768) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error,
            status != SCARD_S_SUCCESS ? status : ERROR_INVALID_DATA);
        return false;
    }
    std::vector<WCHAR> names(chars);
    status = SCardListReadersW(cards, NULL, names.data(), &chars);
    if (status != SCARD_S_SUCCESS || chars > names.size() || chars < 2 ||
        names[chars - 1] != L'\0' || names[chars - 2] != L'\0') {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error,
            status != SCARD_S_SUCCESS ? status : ERROR_INVALID_DATA);
        return false;
    }
    unsigned present = 0;
    for (size_t offset = 0; offset < chars && names[offset]; ) {
        const size_t length = wcsnlen(names.data() + offset, chars - offset);
        if (length == chars - offset) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
            return false;
        }
        SCARD_READERSTATEW state = {};
        state.szReader = names.data() + offset;
        status = SCardGetStatusChangeW(cards, 0, &state, 1);
        if (status != SCARD_S_SUCCESS ||
            (state.dwEventState & (SCARD_STATE_UNKNOWN | SCARD_STATE_UNAVAILABLE | SCARD_STATE_MUTE |
                SCARD_STATE_UNPOWERED | SCARD_STATE_EXCLUSIVE)) || state.cbAtr > sizeof(state.rgbAtr)) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error,
                status != SCARD_S_SUCCESS ? status : ERROR_NOT_READY);
            return false;
        }
        if (state.dwEventState & SCARD_STATE_PRESENT) {
            if (++present > 1) {
                Fail(result, MsRdpEx_SmartCardContextStatus::Ambiguous, ERROR_MORE_DATA);
                return false;
            }
            if (!CopyName(result.reader, state.szReader) || !state.cbAtr) {
                Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
                return false;
            }
            selected = state;
        }
        offset += length + 1;
    }
    if (!present) {
        Fail(result, MsRdpEx_SmartCardContextStatus::NoMatch, ERROR_NOT_FOUND);
        return false;
    }
    selected.szReader = result.reader;
    return true;
}

static void Resolve(const WCHAR* userName, MsRdpEx_SmartCardContext& result)
{
    result.stage = "certificate-reference";
    if (!MsRdpEx_GetSmartCardCertificateHash(userName, result.certificateHash)) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Ineligible, ERROR_INVALID_DATA);
        return;
    }
    result.stage = "emulated-reader";
    if (MsRdpEx_EnvExists("MSRDPEX_WINSCARD_DLL") ||
        MsRdpEx_EnvExists("WINSCARD_SMARTCARD_READER_NAME") ||
        MsRdpEx_EnvExists("WINSCARD_SMARTCARD_CONTAINER_NAME")) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Unsupported, ERROR_NOT_SUPPORTED);
        return;
    }

    CardResources resources;
    result.stage = "certificate-store";
    resources.store = CertOpenStore(CERT_STORE_PROV_SYSTEM_W, 0, 0,
        CERT_SYSTEM_STORE_CURRENT_USER | CERT_STORE_OPEN_EXISTING_FLAG | CERT_STORE_READONLY_FLAG, L"MY");
    if (!resources.store) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, GetLastError());
        return;
    }
    CRYPT_HASH_BLOB hash = { sizeof(result.certificateHash), result.certificateHash };
    resources.certificate = CertFindCertificateInStore(resources.store, X509_ASN_ENCODING,
        0, CERT_FIND_SHA1_HASH, &hash, NULL);
    if (!resources.certificate) {
        Fail(result, MsRdpEx_SmartCardContextStatus::NoMatch, GetLastError());
        return;
    }
    // Linked/collection stores must not silently choose one of several bindings for the same certificate.
    PCCERT_CONTEXT duplicate = CertDuplicateCertificateContext(resources.certificate);
    if (!duplicate) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, GetLastError());
        return;
    }
    duplicate = CertFindCertificateInStore(resources.store, X509_ASN_ENCODING, 0,
        CERT_FIND_SHA1_HASH, &hash, duplicate);
    if (duplicate) {
        CertFreeCertificateContext(duplicate);
        Fail(result, MsRdpEx_SmartCardContextStatus::Ambiguous, ERROR_MORE_DATA);
        return;
    }
    if (GetLastError() != CRYPT_E_NOT_FOUND) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, GetLastError());
        return;
    }
    result.stage = "provider-binding";
    DWORD size = 0;
    if (!CertGetCertificateContextProperty(resources.certificate, CERT_KEY_PROV_INFO_PROP_ID, NULL, &size)) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, GetLastError());
        return;
    }
    if (size < sizeof(CRYPT_KEY_PROV_INFO) || size > 65536) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
        return;
    }
    std::vector<BYTE> binding(size);
    if (!CertGetCertificateContextProperty(resources.certificate, CERT_KEY_PROV_INFO_PROP_ID,
        binding.data(), &size)) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, GetLastError());
        return;
    }
    if (size < sizeof(CRYPT_KEY_PROV_INFO) || size > binding.size()) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
        return;
    }
    binding.resize(size);
    const auto* info = reinterpret_cast<const CRYPT_KEY_PROV_INFO*>(binding.data());
    if (!BindingName(binding, info->pwszContainerName) || !BindingName(binding, info->pwszProvName) ||
        !CopyName(result.container, info->pwszContainerName) ||
        !CopyName(result.provider, info->pwszProvName)) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
        return;
    }
    const bool cng = wcscmp(result.provider, MS_SMART_CARD_KEY_STORAGE_PROVIDER) == 0 &&
        info->dwProvType == 0 &&
        (info->dwKeySpec == 0 || info->dwKeySpec == AT_KEYEXCHANGE || info->dwKeySpec == AT_SIGNATURE);
    const bool capi = wcscmp(result.provider, MS_SCARD_PROV_W) == 0 &&
        info->dwProvType == PROV_RSA_FULL &&
        (info->dwKeySpec == AT_KEYEXCHANGE || info->dwKeySpec == AT_SIGNATURE);
    const DWORD supportedFlags = CRYPT_SILENT | CERT_SET_KEY_PROV_HANDLE_PROP_ID | CERT_SET_KEY_CONTEXT_PROP_ID;
    if ((!cng && !capi) || (info->dwFlags & ~supportedFlags) || info->cProvParam ||
        wcsncmp(result.container, L"\\\\", 2) == 0) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Unsupported, ERROR_NOT_SUPPORTED);
        return;
    }
    result.keySpec = info->dwKeySpec;
    result.stage = "readers";
    LONG status = SCardEstablishContext(SCARD_SCOPE_USER, NULL, NULL, &resources.cards);
    if (status != SCARD_S_SUCCESS) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, status);
        return;
    }
    SCARD_READERSTATEW before = {};
    if (!ReadSingleCard(resources.cards, result, before))
        return;

    result.stage = "card-name";
    WCHAR cardNames[2048] = {};
    DWORD chars = ARRAYSIZE(cardNames);
    status = SCardListCardsW(resources.cards, before.rgbAtr, NULL, 0, cardNames, &chars);
    if (status != SCARD_S_SUCCESS || chars > ARRAYSIZE(cardNames) || chars < 2 ||
        cardNames[chars - 1] != L'\0' || !cardNames[0]) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error,
            status != SCARD_S_SUCCESS ? status : ERROR_INVALID_DATA);
        return;
    }
    const size_t firstLength = wcsnlen(cardNames, chars);
    if (firstLength + 1 >= chars || cardNames[firstLength + 1] != L'\0') {
        Fail(result, MsRdpEx_SmartCardContextStatus::Ambiguous, ERROR_MORE_DATA);
        return;
    }
    if (!CopyName(result.card, cardNames)) {
        Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
        return;
    }

    // Qualify the existing binding with the sole present reader. Never use a default container,
    // substitute a provider, supply a PIN, sign, decrypt, or retry a silent-context failure.
    const std::wstring scoped = std::wstring(L"\\\\.\\") + result.reader + L"\\" + result.container;
    result.stage = "reader-scoped-key";
    std::vector<BYTE> certificate;
    if (cng) {
        SECURITY_STATUS error = NCryptOpenStorageProvider(&resources.cng, result.provider, 0);
        if (error == ERROR_SUCCESS)
            error = NCryptOpenKey(resources.cng, &resources.cngKey, scoped.c_str(), result.keySpec, NCRYPT_SILENT_FLAG);
        if (error == ERROR_SUCCESS)
            error = NCryptGetProperty(resources.cngKey, NCRYPT_CERTIFICATE_PROPERTY, NULL, 0, &size, NCRYPT_SILENT_FLAG);
        if (error != ERROR_SUCCESS) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error, error);
            return;
        }
        if (!size || size > 65536) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
            return;
        }
        certificate.resize(size);
        error = NCryptGetProperty(resources.cngKey, NCRYPT_CERTIFICATE_PROPERTY,
            certificate.data(), size, &size, NCRYPT_SILENT_FLAG);
        if (error != ERROR_SUCCESS) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error, error);
            return;
        }
    } else {
        if (!CryptAcquireContextW(&resources.capi, scoped.c_str(), result.provider, PROV_RSA_FULL, CRYPT_SILENT) ||
            !CryptGetUserKey(resources.capi, result.keySpec, &resources.capiKey) ||
            !CryptGetKeyParam(resources.capiKey, KP_CERTIFICATE, NULL, &size, 0)) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error, GetLastError());
            return;
        }
        if (!size || size > 65536) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error, ERROR_INVALID_DATA);
            return;
        }
        certificate.resize(size);
        if (!CryptGetKeyParam(resources.capiKey, KP_CERTIFICATE, certificate.data(), &size, 0)) {
            Fail(result, MsRdpEx_SmartCardContextStatus::Error, GetLastError());
            return;
        }
    }
    result.stage = "key-certificate";
    if (size > certificate.size() || size != resources.certificate->cbCertEncoded ||
        memcmp(certificate.data(), resources.certificate->pbCertEncoded, size) != 0) {
        Fail(result, MsRdpEx_SmartCardContextStatus::NoMatch, ERROR_INVALID_DATA);
        return;
    }
    result.stage = "card-stability";
    MsRdpEx_SmartCardContext afterContext;
    SCARD_READERSTATEW after = {};
    if (!ReadSingleCard(resources.cards, afterContext, after)) {
        Fail(result, afterContext.status, afterContext.error);
        return;
    }
    if (wcscmp(result.reader, afterContext.reader) != 0 || !SameCard(before, after)) {
        Fail(result, MsRdpEx_SmartCardContextStatus::NoMatch, ERROR_NOT_READY);
        return;
    }
    result.stage = "verified";
    result.status = MsRdpEx_SmartCardContextStatus::Ready;
}
}

MsRdpEx_SmartCardContextMode MsRdpEx_GetSmartCardContextMode()
{
    WCHAR value[16] = {};
    SetLastError(ERROR_SUCCESS);
    DWORD length = GetEnvironmentVariableW(L"MSRDPEX_SSPI_SMARTCARD_CONTEXT", value, ARRAYSIZE(value));
    if (!length && (GetLastError() == ERROR_ENVVAR_NOT_FOUND || GetLastError() == ERROR_SUCCESS))
        return MsRdpEx_SmartCardContextMode::Off;
    if (length < ARRAYSIZE(value)) {
        if (_wcsicmp(value, L"off") == 0) return MsRdpEx_SmartCardContextMode::Off;
        if (_wcsicmp(value, L"observe") == 0) return MsRdpEx_SmartCardContextMode::Observe;
        if (_wcsicmp(value, L"apply") == 0) return MsRdpEx_SmartCardContextMode::Apply;
    }
    MsRdpEx_LogPrint(WARN, "Smart-card context: invalid mode; candidate not applied (expected off, observe, apply)");
    return MsRdpEx_SmartCardContextMode::Off;
}

bool MsRdpEx_GetSmartCardCertificateHash(const WCHAR* userName, BYTE (&hash)[20])
{
    CRED_MARSHAL_TYPE type;
    void* credential = NULL;
    if (!userName || !CredUnmarshalCredentialW(userName, &type, &credential))
        return false;
    const auto* certificate = static_cast<const CERT_CREDENTIAL_INFO*>(credential);
    const bool valid = type == CertCredential && certificate &&
        certificate->cbSize == sizeof(CERT_CREDENTIAL_INFO);
    if (valid)
        CopyMemory(hash, certificate->rgbHashOfCert, sizeof(hash));
    CredFree(credential);
    return valid;
}

void MsRdpEx_ResolveSmartCardContext(const WCHAR* userName, MsRdpEx_SmartCardContext* context)
{
    const auto mode = context->mode;
    *context = MsRdpEx_SmartCardContext();
    context->mode = mode;
    if (mode == MsRdpEx_SmartCardContextMode::Off)
        return;
    try {
        Resolve(userName, *context);
    } catch (const std::bad_alloc&) {
        Fail(*context, MsRdpEx_SmartCardContextStatus::Error, ERROR_NOT_ENOUGH_MEMORY);
    }
}

void MsRdpEx_LogSmartCardContext(const MsRdpEx_SmartCardContext& context)
{
    const char* status = "ineligible";
    switch (context.status) {
        case MsRdpEx_SmartCardContextStatus::Ready: status = "ready"; break;
        case MsRdpEx_SmartCardContextStatus::NoMatch: status = "no-match"; break;
        case MsRdpEx_SmartCardContextStatus::Ambiguous: status = "ambiguous"; break;
        case MsRdpEx_SmartCardContextStatus::Unsupported: status = "unsupported"; break;
        case MsRdpEx_SmartCardContextStatus::Error: status = "error"; break;
        default: break;
    }
    const uint32_t level = context.status == MsRdpEx_SmartCardContextStatus::Ready ? MSRDPEX_LOG_DEBUG : MSRDPEX_LOG_WARN;
    if (!MsRdpEx_IsLogLevelActive(level))
        return;
    MsRdpEx_Log(level,
        "Smart-card context: mode=%s status=%s stage=%s error=0x%08X; %s",
        context.mode == MsRdpEx_SmartCardContextMode::Apply ? "apply" : "observe", status,
        context.stage, context.error,
        context.status == MsRdpEx_SmartCardContextStatus::Ready ?
            "snapshot prepared (not yet applied)" : "candidate not applied");
}
