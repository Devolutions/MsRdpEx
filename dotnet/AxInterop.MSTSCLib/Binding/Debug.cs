using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace MsRdpEx.Interop
{
    [GeneratedComInterface]
    [Guid("209D0EB9-6254-47B1-9033-A98DAE55BB27")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public unsafe partial interface IMsTscDebug : IDispatch
    {
        void SetHatchBitmapPDU(int phatchBitmapPDU);
        int GetHatchBitmapPDU();
        void SetHatchSSBOrder(int phatchSSBOrder);
        int GetHatchSSBOrder();
        void SetHatchMembltOrder(int phatchMembltOrder);
        int GetHatchMembltOrder();
        void SetHatchIndexPDU(int phatchIndexPDU);
        int GetHatchIndexPDU();
        void SetLabelMemblt(int plabelMemblt);
        int GetLabelMemblt();
        void SetBitmapCacheMonitor(int pbitmapCacheMonitor);
        int GetBitmapCacheMonitor();
        void SetMallocFailuresPercent(int pmallocFailuresPercent);
        int GetMallocFailuresPercent();
        void SetMallocHugeFailuresPercent(int pmallocHugeFailuresPercent);
        int GetMallocHugeFailuresPercent();
        void SetNetThroughput(int NetThroughput);
        int GetNetThroughput();
        void SetCLXCmdLine(ReadOnlyBinaryStringRef pCLXCmdLine);
        BinaryString GetCLXCmdLine();
        void SetCLXDll(ReadOnlyBinaryStringRef pCLXDll);
        BinaryString GetCLXDll();
        void SetRemoteProgramsHatchVisibleRegion(int pcbHatch);
        int GetRemoteProgramsHatchVisibleRegion();
        void SetRemoteProgramsHatchVisibleNoDataRegion(int pcbHatch);
        int GetRemoteProgramsHatchVisibleNoDataRegion();
        void SetRemoteProgramsHatchNonVisibleRegion(int pcbHatch);
        int GetRemoteProgramsHatchNonVisibleRegion();
        void SetRemoteProgramsHatchWindow(int pcbHatch);
        int GetRemoteProgramsHatchWindow();
        void SetRemoteProgramsStayConnectOnBadCaps(int pcbStayConnected);
        int GetRemoteProgramsStayConnectOnBadCaps();
        uint GetControlType();
        void SetDecodeGfx(VariantBool rhs);
    }
}
