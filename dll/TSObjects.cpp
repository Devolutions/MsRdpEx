
#include <MsRdpEx/MsRdpEx.h>

#include <MsRdpEx/Memory.h>

#include "TSObjects.h"

const char* GetTSPropertyTypeName(uint8_t propType)
{
    const char* name = "none";

    switch (propType)
    {
        case 1: name = "ULONG"; break;
        case 2: name = "INT"; break;
        case 3: name = "BOOL"; break;
        case 4: name = "String"; break;
        case 5: name = "Binary"; break;
        case 6: name = "SecureString"; break;
        case 7: name = "IUnknown"; break;
    }

    return name;
}

PROPERTY_ENTRY_EX* FindTSProperty(ITSPropertySet* pTSPropertySet, const char* name)
{
    PROPERTY_ENTRY_EX* found = NULL;
    uint32_t propCount = pTSPropertySet->propCount;
    PROPERTY_ENTRY_EX* propMap = pTSPropertySet->propMap;

    for (int i = 0; i < propCount; i++)
    {
        PROPERTY_ENTRY_EX* prop = &propMap[i];

        if (MsRdpEx_StringEquals(name, prop->propName)) {
            found = prop;
            break;
        }
    }

    return found;
}

bool GetTSPropertyType(ITSPropertySet* pTSPropertySet, const char* name, uint8_t* pPropType)
{
    PROPERTY_ENTRY_EX* prop = FindTSProperty(pTSPropertySet, name);

    *pPropType = 0;

    if (prop) {
        *pPropType = prop->propType;
    }

    return prop ? true : false;
}

void DumpTSPropertyMap(ITSPropertySet* pTSPropertySet, const char* name)
{
    uint32_t propCount = pTSPropertySet->propCount;
    PROPERTY_ENTRY_EX* propMap = pTSPropertySet->propMap;

    MsRdpEx_Log("TSPropertySet: %s (%d)", name, propCount);

    for (int i = 0; i < propCount; i++)
    {
        PROPERTY_ENTRY_EX* prop = &propMap[i];
        MsRdpEx_Log("%s (%s)", prop->propName, GetTSPropertyTypeName(prop->propType));
    }
}

bool TsPropertyMap_IsCoreProps(ITSPropertySet* pTSPropertySet)
{
    uint32_t propCount;
    PROPERTY_ENTRY_EX* propMap;

    if (!MsRdpEx_CanReadUnsafePtr((void*)pTSPropertySet, sizeof(ITSPropertySet)))
        return false;
    
    propCount = pTSPropertySet->propCount;
    propMap = pTSPropertySet->propMap;

    if ((propCount < 175) || (propCount > 500))
        return false;

    if (!MsRdpEx_CanReadUnsafePtr((void*)propMap, propCount * sizeof(PROPERTY_ENTRY_EX)))
        return false;

    for (int i = 0; i < 10; i++)
    {
        PROPERTY_ENTRY_EX* prop = &propMap[i];
  
        if (MsRdpEx_StringIEqualsUnsafePtr(prop->propName, "ServerName")) {
            return true;
        }
    }

    return false;
}

bool TsPropertyMap_IsBaseProps(ITSPropertySet* pTSPropertySet)
{
    uint32_t propCount;
    PROPERTY_ENTRY_EX* propMap;

    if (!MsRdpEx_CanReadUnsafePtr((void*)pTSPropertySet, sizeof(ITSPropertySet)))
        return false;

    propCount = pTSPropertySet->propCount;
    propMap = pTSPropertySet->propMap;

    if ((propCount < 100) || (propCount > 500))
        return false;

    if (!MsRdpEx_CanReadUnsafePtr((void*)propMap, propCount * sizeof(PROPERTY_ENTRY_EX)))
        return false;

    for (int i = 0; i < 10; i++)
    {
        PROPERTY_ENTRY_EX* prop = &propMap[i];

        if (MsRdpEx_StringIEqualsUnsafePtr(prop->propName, "FullScreen")) {
            return true;
        }
    }

    return false;
}

bool TsPropertyMap_IsTransportProps(ITSPropertySet* pTSPropertySet)
{
    uint32_t propCount;
    PROPERTY_ENTRY_EX* propMap;

    if (!MsRdpEx_CanReadUnsafePtr((void*)pTSPropertySet, sizeof(ITSPropertySet)))
        return false;

    propCount = pTSPropertySet->propCount;
    propMap = pTSPropertySet->propMap;

    if ((propCount < 25) || (propCount > 500))
        return false;

    if (!MsRdpEx_CanReadUnsafePtr((void*)propMap, propCount * sizeof(PROPERTY_ENTRY_EX)))
        return false;

    for (int i = 0; i < 10; i++)
    {
        PROPERTY_ENTRY_EX* prop = &propMap[i];

        if (MsRdpEx_StringIEqualsUnsafePtr(prop->propName, "GatewayHostname")) {
            return true;
        }
    }

    return false;
}

typedef struct
{
    void* param1;
    void* param2;
    void* name;
    void* marker;
} COPWnd;

void CDECL MsRdpEx_OutputWindow_OnCreate(HWND hWnd, void* pUserData)
{
    COPWnd* pOPWnd = (COPWnd*) pUserData;

    MsRdpEx_Log("WindowCreate: %s name: %s hWnd: %p", pOPWnd->name, hWnd);
}
