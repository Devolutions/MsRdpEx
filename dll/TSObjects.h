#ifndef MSRDPEX_TSOBJECTS_H
#define MSRDPEX_TSOBJECTS_H

#include "MsRdpEx.h"

#include <comdef.h>

#pragma warning (disable : 26812)

#include "mstscax.tlh"

using namespace MSTSCLib;

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

typedef HRESULT (__stdcall* ITSPropertySet_SetBoolProperty)(ITSPropertySet* This, const char* propName, int propValue);
typedef HRESULT (__stdcall* ITSPropertySet_GetBoolProperty)(ITSPropertySet* This, const char* propName, int* propValue);

typedef HRESULT (__stdcall* ITSPropertySet_SetIntProperty)(ITSPropertySet* This, const char* propName, int propValue);
typedef HRESULT (__stdcall* ITSPropertySet_GetIntProperty)(ITSPropertySet* This, const char* propName, int* propValue);

typedef HRESULT (__stdcall* ITSPropertySet_SetStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* propValue);
typedef HRESULT (__stdcall* ITSPropertySet_GetStringProperty)(ITSPropertySet* This, const char* propName, WCHAR** propValue);

typedef struct ITSPropertySetVtbl
{
    HRESULT(__stdcall* QueryInterface)(ITSPropertySet* This, REFIID riid, void** ppvObject);
    ULONG(__stdcall* AddRef)(ITSPropertySet* This);
    ULONG(__stdcall* Release)(ITSPropertySet* This);
    HRESULT(__stdcall* SetProperty)(ITSPropertySet* This, const char* propName, void* propValue);
    HRESULT(__stdcall* SetBoolProperty)(ITSPropertySet* This, const char* propName, int propValue);
    HRESULT(__stdcall* SetIntProperty)(ITSPropertySet* This, const char* propName, int propValue);
    HRESULT(__stdcall* SetIUnknownProperty)(ITSPropertySet* This, const char* propName, void* propValue);
    HRESULT(__stdcall* SetStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* propValue);
    HRESULT(__stdcall* SetSecureStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* propValue);
    HRESULT(__stdcall* SetUlongPtrProperty)(ITSPropertySet* This, const char* propName, ULONG_PTR propValue);
    HRESULT(__stdcall* GetProperty1)(ITSPropertySet* This, const char* propName, WCHAR* a1, int a2);
    HRESULT(__stdcall* GetProperty2)(ITSPropertySet* This, const char* propName, uint32_t* a1);
    HRESULT(__stdcall* GetIntProperty)(ITSPropertySet* This, const char* propName, int* propValue);
    HRESULT(__stdcall* GetIUnknownProperty)(ITSPropertySet* This, const char* propName, IUnknown** propValue);
    HRESULT(__stdcall* GetBoolProperty)(ITSPropertySet* This, const char* propName, int* propValue);
    HRESULT(__stdcall* GetStringProperty)(ITSPropertySet* This, const char* propName, WCHAR** propValue);
    HRESULT(__stdcall* GetSecureStringProperty)(ITSPropertySet* This, const char* propName, WCHAR* a1, uint32_t* a2);
    HRESULT(__stdcall* GetUlongPtrProperty)(ITSPropertySet* This, const char* propName, ULONG_PTR* propValue);
    HRESULT(__stdcall* EnterReadLock)(ITSPropertySet* This);
    HRESULT(__stdcall* LeaveReadLock)(ITSPropertySet* This);
    HRESULT(__stdcall* EnterWriteLock)(ITSPropertySet* This);
    HRESULT(__stdcall* LeaveWriteLock)(ITSPropertySet* This);
    HRESULT(__stdcall* RevertToDefaults)(ITSPropertySet* This);
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