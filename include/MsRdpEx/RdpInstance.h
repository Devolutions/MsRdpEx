#ifndef MSRDPEX_INSTANCE_H
#define MSRDPEX_INSTANCE_H

#include <MsRdpEx/MsRdpEx.h>

#include <comdef.h>

struct __declspec(novtable)
    IMsRdpExInstance : public IUnknown
{
public:
    virtual HRESULT __stdcall GetRdpClient(LPVOID* ppvObject) = 0;
};

class CMsRdpExInstance;
class CMsRdpClient;

#ifdef __cplusplus
extern "C" {
#endif

CMsRdpExInstance* CMsRdpExInstance_New(CMsRdpClient* pMsRdpClient);

#ifdef __cplusplus
}
#endif

#endif // MSRDPEX_INSTANCE_H
