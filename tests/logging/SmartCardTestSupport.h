#pragma once

#include <MsRdpEx/RdpSettings.h>
#include <MsRdpEx/RdpInstance.h>
#include <stdexcept>

extern "C" IMsRdpExInstance* CreatePluginReferenceInstance();
extern "C" bool RegisterPluginReferenceInstance(IMsRdpExInstance* instance);
extern "C" bool UnregisterPluginReferenceInstance(IMsRdpExInstance* instance);

inline void SmartCardCheck(bool condition, const char* message)
{
    if (!condition)
        throw std::runtime_error(message);
}

class SmartCardTestUnknown : public IUnknown
{
public:
    HRESULT STDMETHODCALLTYPE QueryInterface(REFIID iid, void** object) override
    {
        *object = NULL;
        if (iid != IID_IUnknown)
            return E_NOINTERFACE;
        *object = static_cast<IUnknown*>(this);
        AddRef();
        return S_OK;
    }
    ULONG STDMETHODCALLTYPE AddRef() override { return 2; }
    ULONG STDMETHODCALLTYPE Release() override { return 1; }
};
