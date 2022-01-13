#ifndef MSRDPEX_TSOBJECTS_H
#define MSRDPEX_TSOBJECTS_H

#include "MsRdpEx.h"

#include <comdef.h>

#ifdef __cplusplus
extern "C" {
#endif

#pragma pack(push, 1)

#define TSPROPERTY_TYPE_ULONG       1
#define TSPROPERTY_TYPE_INT         2
#define TSPROPERTY_TYPE_BOOL        3
#define TSPROPERTY_TYPE_STRING      4
#define TSPROPERTY_TYPE_BINARY      5
#define TSPROPERTY_TYPE_SSTRING     6
#define TSPROPERTY_TYPE_IUNKNOWN    7

struct tagPROPERTY_ENTRY_EX
{
    const char* propName;
    uint8_t propType;
    uint8_t padding1[23];
    uint8_t padding2[32];
    uint64_t padding3;
};
typedef struct tagPROPERTY_ENTRY_EX PROPERTY_ENTRY_EX;

#pragma pack(pop)

typedef struct _ITSPropertySet ITSPropertySet;

typedef struct ITSPropertySetVtbl
{
    HRESULT(STDMETHODCALLTYPE* QueryInterface)(ITSPropertySet* This, REFIID riid, void** ppvObject);
    ULONG(STDMETHODCALLTYPE* AddRef)(ITSPropertySet* This);
    ULONG(STDMETHODCALLTYPE* Release)(ITSPropertySet* This);
    HRESULT(STDMETHODCALLTYPE* SetProperty)(ITSPropertySet* This, const char* propName, void* propValue);
    HRESULT(STDMETHODCALLTYPE* SetBoolProperty)(ITSPropertySet* This, const char* propName, VARIANT_BOOL propValue);
    HRESULT(STDMETHODCALLTYPE* SetIntProperty)(ITSPropertySet* This, const char* propName, int propValue);
    HRESULT(STDMETHODCALLTYPE* SetIUnknownProperty)(ITSPropertySet* This, const char* propName, void* propValue);
    HRESULT(STDMETHODCALLTYPE* SetStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* propValue);
    HRESULT(STDMETHODCALLTYPE* SetSecureStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* propValue);
    HRESULT(STDMETHODCALLTYPE* SetUlongPtrProperty)(ITSPropertySet* This, const char* propName, ULONG_PTR propValue);
    HRESULT(STDMETHODCALLTYPE* GetProperty1)(ITSPropertySet* This, const char* propName, WCHAR* a1, int a2);
    HRESULT(STDMETHODCALLTYPE* GetProperty2)(ITSPropertySet* This, const char* propName, uint32_t* a1);
    HRESULT(STDMETHODCALLTYPE* GetIntProperty)(ITSPropertySet* This, const char* propName, int* propValue);
    HRESULT(STDMETHODCALLTYPE* GetIUnknownProperty)(ITSPropertySet* This, const char* propName, IUnknown** propValue);
    HRESULT(STDMETHODCALLTYPE* GetBoolProperty)(ITSPropertySet* This, const char* propName, VARIANT_BOOL* propValue);
    HRESULT(STDMETHODCALLTYPE* GetStringProperty)(ITSPropertySet* This, const char* propName, WCHAR** propValue);
    HRESULT(STDMETHODCALLTYPE* GetSecureStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* a1, uint32_t* a2);
    HRESULT(STDMETHODCALLTYPE* GetUlongPtrProperty)(ITSPropertySet* This, const char* propName, ULONG_PTR* propValue);
    HRESULT(STDMETHODCALLTYPE* EnterReadLock)(ITSPropertySet* This);
    HRESULT(STDMETHODCALLTYPE* LeaveReadLock)(ITSPropertySet* This);
    HRESULT(STDMETHODCALLTYPE* EnterWriteLock)(ITSPropertySet* This);
    HRESULT(STDMETHODCALLTYPE* LeaveWriteLock)(ITSPropertySet* This);
    HRESULT(STDMETHODCALLTYPE* RevertToDefaults)(ITSPropertySet* This);
} ITSPropertySetVtbl;

struct _ITSPropertySet
{
    ITSPropertySetVtbl* vtbl;
    void* INonDelegatingUnknownVtbl;
    void* TSObjectVtbl;
    const char* name;
    uint32_t marker;
    uint32_t refCount;
    void* unknown1;
    void* unknown2;
    void* unknown3;
    PROPERTY_ENTRY_EX* propMap;
    uint32_t propCount;
};

typedef struct _ITSObjectBase ITSObjectBase;

struct _ITSObjectBase
{
    IUnknown* vtbl;
    IUnknown* INonDelegatingUnknownVtbl;
    IUnknown* TSObjectVtbl;
    const char* name;
    uint32_t marker;
    uint32_t refCount;
};

#define TSOBJECT_MARKER 0xDBCAABCD

const char* GetTSPropertyTypeName(uint8_t propType);

PROPERTY_ENTRY_EX* FindTSProperty(ITSPropertySet* pTSPropertySet, const char* name);
bool GetTSPropertyType(ITSPropertySet* pTSPropertySet, const char* name, uint8_t* pPropType);

void DumpTSPropertyMap(ITSPropertySet* pTSPropertySet, const char* name);

bool TsPropertyMap_IsCoreProps(ITSPropertySet* pTSPropertySet);
bool TsPropertyMap_IsBaseProps(ITSPropertySet* pTSPropertySet);
bool TsPropertyMap_IsTransportProps(ITSPropertySet* pTSPropertySet);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_TSOBJECTS_H */