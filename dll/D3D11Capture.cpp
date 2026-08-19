#include "MsRdpEx.h"
#include "D3D11Capture.h"
#include "OutputMirrorCapture.h"

#include <MsRdpEx/Detours.h>

#include <d3d11.h>
#include <dxgi1_2.h>

#include <vector>

typedef HRESULT(WINAPI* D3D11CreateDeviceFn)(
    IDXGIAdapter* adapter,
    D3D_DRIVER_TYPE driverType,
    HMODULE software,
    UINT flags,
    const D3D_FEATURE_LEVEL* featureLevels,
    UINT featureLevelsCount,
    UINT sdkVersion,
    ID3D11Device** device,
    D3D_FEATURE_LEVEL* featureLevel,
    ID3D11DeviceContext** immediateContext);

typedef HRESULT(STDMETHODCALLTYPE* CreateCompositionSwapChainFn)(
    IDXGIFactory2* factory,
    IUnknown* device,
    const DXGI_SWAP_CHAIN_DESC1* description,
    IDXGIOutput* restrictToOutput,
    IDXGISwapChain1** swapChain);

typedef HRESULT(STDMETHODCALLTYPE* CreateSwapChainFn)(
    IDXGIFactory* factory,
    IUnknown* device,
    DXGI_SWAP_CHAIN_DESC* description,
    IDXGISwapChain** swapChain);

typedef HRESULT(STDMETHODCALLTYPE* CreateSwapChainForHwndFn)(
    IDXGIFactory2* factory,
    IUnknown* device,
    HWND window,
    const DXGI_SWAP_CHAIN_DESC1* description,
    const DXGI_SWAP_CHAIN_FULLSCREEN_DESC* fullscreenDescription,
    IDXGIOutput* restrictToOutput,
    IDXGISwapChain1** swapChain);

typedef HRESULT(STDMETHODCALLTYPE* SwapChainPresentFn)(
    IDXGISwapChain* swapChain,
    UINT syncInterval,
    UINT flags);

typedef ULONG(STDMETHODCALLTYPE* SwapChainReleaseFn)(IDXGISwapChain* swapChain);

struct MsRdpEx_D3D11CaptureDevice
{
    ID3D11Device* device;
    IMsRdpExInstance* instance;
};

struct MsRdpEx_D3D11CaptureSwapChain
{
    IDXGISwapChain* swapChain;
    ID3D11Device* device;
    ID3D11DeviceContext* context;
    ID3D11Texture2D* stagingTexture;
    IMsRdpExInstance* instance;
    UINT width;
    UINT height;
    DXGI_FORMAT format;
    bool unavailable;
    bool captureStarted;
};

static SRWLOCK g_D3D11CaptureLock = SRWLOCK_INIT;
static std::vector<IMsRdpExInstance*> g_PendingInstances;
static std::vector<MsRdpEx_D3D11CaptureDevice> g_Devices;
static std::vector<MsRdpEx_D3D11CaptureSwapChain> g_SwapChains;

static D3D11CreateDeviceFn Real_D3D11CreateDevice = D3D11CreateDevice;
static CreateCompositionSwapChainFn Real_CreateCompositionSwapChain = NULL;
static CreateSwapChainFn Real_CreateSwapChain = NULL;
static CreateSwapChainForHwndFn Real_CreateSwapChainForHwnd = NULL;
static SwapChainPresentFn Real_SwapChainPresent = NULL;
static SwapChainReleaseFn Real_SwapChainRelease = NULL;

static bool g_CreateCompositionSwapChainHookAttached = false;
static bool g_CreateSwapChainHookAttached = false;
static bool g_CreateSwapChainForHwndHookAttached = false;
static bool g_SwapChainPresentHookAttached = false;
static bool g_SwapChainReleaseHookAttached = false;
static PVOID g_CreateCompositionSwapChainTarget = NULL;
static PVOID g_CreateSwapChainTarget = NULL;
static PVOID g_CreateSwapChainForHwndTarget = NULL;
static PVOID g_SwapChainPresentTarget = NULL;
static PVOID g_SwapChainReleaseTarget = NULL;
static thread_local LONG g_D3D11CaptureDepth = 0;

static HRESULT STDMETHODCALLTYPE Hook_CreateCompositionSwapChain(
    IDXGIFactory2* factory,
    IUnknown* device,
    const DXGI_SWAP_CHAIN_DESC1* description,
    IDXGIOutput* restrictToOutput,
    IDXGISwapChain1** swapChain);
static HRESULT STDMETHODCALLTYPE Hook_CreateSwapChain(
    IDXGIFactory* factory,
    IUnknown* device,
    DXGI_SWAP_CHAIN_DESC* description,
    IDXGISwapChain** swapChain);
static HRESULT STDMETHODCALLTYPE Hook_CreateSwapChainForHwnd(
    IDXGIFactory2* factory,
    IUnknown* device,
    HWND window,
    const DXGI_SWAP_CHAIN_DESC1* description,
    const DXGI_SWAP_CHAIN_FULLSCREEN_DESC* fullscreenDescription,
    IDXGIOutput* restrictToOutput,
    IDXGISwapChain1** swapChain);

static HRESULT STDMETHODCALLTYPE Hook_SwapChainPresent(
    IDXGISwapChain* swapChain,
    UINT syncInterval,
    UINT flags);

static ULONG STDMETHODCALLTYPE Hook_SwapChainRelease(IDXGISwapChain* swapChain);
static void MsRdpEx_D3D11Capture_DisableHardwareMode(
    IMsRdpExInstance* instance,
    const char* operation,
    HRESULT error);

static void MsRdpEx_D3D11Capture_ReleaseSwapChain(
    MsRdpEx_D3D11CaptureSwapChain* swapChain)
{
    if (swapChain->stagingTexture)
        swapChain->stagingTexture->Release();
    if (swapChain->context)
        swapChain->context->Release();
    if (swapChain->device)
        swapChain->device->Release();
    if (swapChain->instance)
        swapChain->instance->Release();
}

static void MsRdpEx_D3D11Capture_RemoveSwapChain(IDXGISwapChain* swapChain)
{
    AcquireSRWLockExclusive(&g_D3D11CaptureLock);
    for (size_t i = 0; i < g_SwapChains.size(); i++)
    {
        if (g_SwapChains[i].swapChain == swapChain)
        {
            MsRdpEx_D3D11Capture_ReleaseSwapChain(&g_SwapChains[i]);
            g_SwapChains.erase(g_SwapChains.begin() + i);
            break;
        }
    }
    ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
}

static IMsRdpExInstance* MsRdpEx_D3D11Capture_FindInstanceForDevice(IUnknown* device)
{
    IMsRdpExInstance* instance = NULL;
    ID3D11Device* d3dDevice = NULL;

    if (!device ||
        FAILED(device->QueryInterface(__uuidof(ID3D11Device), (void**)&d3dDevice)))
        return NULL;

    AcquireSRWLockShared(&g_D3D11CaptureLock);
    for (const MsRdpEx_D3D11CaptureDevice& entry : g_Devices)
    {
        if (entry.device == d3dDevice)
        {
            instance = entry.instance;
            instance->AddRef();
            break;
        }
    }
    ReleaseSRWLockShared(&g_D3D11CaptureLock);
    d3dDevice->Release();

    return instance;
}

static IMsRdpExInstance* MsRdpEx_D3D11Capture_FindPendingInstance()
{
    IMsRdpExInstance* instance = NULL;

    AcquireSRWLockShared(&g_D3D11CaptureLock);
    if (g_PendingInstances.size() == 1)
    {
        instance = g_PendingInstances.front();
        instance->AddRef();
    }
    ReleaseSRWLockShared(&g_D3D11CaptureLock);

    return instance;
}

static bool MsRdpEx_D3D11Capture_AttachDynamicHook(
    PVOID* realFunction,
    PVOID hookFunction,
    bool* attached)
{
    LONG error;

    if (*attached)
        return true;

    error = DetourTransactionBegin();
    if (error != NO_ERROR)
        return false;

    DetourUpdateThread(GetCurrentThread());
    error = DetourAttach(realFunction, hookFunction);
    if (error == NO_ERROR)
        error = DetourTransactionCommit();
    else
        DetourTransactionAbort();

    if (error == NO_ERROR)
        *attached = true;

    return error == NO_ERROR;
}

static bool MsRdpEx_D3D11Capture_AttachSwapChainHooks(IDXGISwapChain* swapChain)
{
    void** vtable = *(void***)swapChain;

    AcquireSRWLockExclusive(&g_D3D11CaptureLock);

    if (!g_SwapChainPresentHookAttached)
    {
        Real_SwapChainPresent = (SwapChainPresentFn)vtable[8];
        g_SwapChainPresentTarget = (PVOID)Real_SwapChainPresent;
        if (!MsRdpEx_D3D11Capture_AttachDynamicHook(
            (PVOID*)&Real_SwapChainPresent, (PVOID)Hook_SwapChainPresent,
            &g_SwapChainPresentHookAttached))
        {
            Real_SwapChainPresent = NULL;
            ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
            return false;
        }
    }
    else if (vtable[8] != g_SwapChainPresentTarget)
    {
        ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
        return false;
    }

    if (!g_SwapChainReleaseHookAttached)
    {
        Real_SwapChainRelease = (SwapChainReleaseFn)vtable[2];
        g_SwapChainReleaseTarget = (PVOID)Real_SwapChainRelease;
        if (!MsRdpEx_D3D11Capture_AttachDynamicHook(
            (PVOID*)&Real_SwapChainRelease, (PVOID)Hook_SwapChainRelease,
            &g_SwapChainReleaseHookAttached))
        {
            Real_SwapChainRelease = NULL;
            ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
            return false;
        }
    }
    else if (vtable[2] != g_SwapChainReleaseTarget)
    {
        ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
        return false;
    }

    ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
    return true;
}

static bool MsRdpEx_D3D11Capture_AttachFactoryHook(
    void** vtable,
    size_t slot,
    PVOID* realFunction,
    PVOID hookFunction,
    bool* attached,
    PVOID* target)
{
    if (*attached)
        return vtable[slot] == *target;

    *realFunction = vtable[slot];
    *target = *realFunction;
    if (MsRdpEx_D3D11Capture_AttachDynamicHook(
            realFunction, hookFunction, attached))
    {
        return true;
    }

    *realFunction = NULL;
    *target = NULL;
    return false;
}

static void MsRdpEx_D3D11Capture_TrackSwapChain(
    IUnknown* device,
    IDXGISwapChain* swapChain)
{
    IMsRdpExInstance* instance = MsRdpEx_D3D11Capture_FindInstanceForDevice(device);
    if (!instance)
    {
        instance = MsRdpEx_D3D11Capture_FindPendingInstance();
        if (instance)
        {
            MsRdpEx_D3D11Capture_DisableHardwareMode(
                instance, "attribute DXGI swap chain to RDP device", E_FAIL);
            instance->Release();
        }
        return;
    }

    ID3D11Device* d3dDevice = NULL;
    ID3D11DeviceContext* context = NULL;

    HRESULT hr = swapChain->GetDevice(__uuidof(ID3D11Device), (void**)&d3dDevice);
    if (FAILED(hr))
    {
        MsRdpEx_D3D11Capture_DisableHardwareMode(
            instance, "query DXGI swap-chain device", hr);
        instance->Release();
        return;
    }

    d3dDevice->GetImmediateContext(&context);
    if (!context || !MsRdpEx_D3D11Capture_AttachSwapChainHooks(swapChain))
    {
        MsRdpEx_D3D11Capture_DisableHardwareMode(
            instance, "hook RDP DXGI swap chain", E_FAIL);
        if (context)
            context->Release();
        d3dDevice->Release();
        instance->Release();
        return;
    }

    MsRdpEx_D3D11CaptureSwapChain entry = {};
    entry.swapChain = swapChain;
    entry.device = d3dDevice;
    entry.context = context;
    entry.instance = instance;

    AcquireSRWLockExclusive(&g_D3D11CaptureLock);
    g_SwapChains.push_back(entry);
    ReleaseSRWLockExclusive(&g_D3D11CaptureLock);

    MsRdpEx_LogPrint(DEBUG,
        "D3D11 capture tracking RDP DXGI swap chain: %p", swapChain);

    if (!MsRdpEx_Instance_ArmHardwareCaptureWatchdog(instance))
    {
        MsRdpEx_D3D11Capture_DisableHardwareMode(
            instance, "arm hardware capture watchdog",
            HRESULT_FROM_WIN32(GetLastError()));
    }
}

static void MsRdpEx_D3D11Capture_DisableHardwareMode(
    IMsRdpExInstance* instance,
    const char* operation,
    HRESULT error)
{
    MsRdpEx_LogPrint(ERROR,
        "D3D11 capture %s failed: 0x%08X; reconnecting with GDI presentation",
        operation, error);

    if (!MsRdpEx_Instance_RequestGdiReconnect(instance))
    {
        MsRdpEx_LogPrint(ERROR,
            "D3D11 capture could not schedule the GDI fallback reconnect");
    }
}

static void MsRdpEx_D3D11Capture_MarkUnavailable(
    MsRdpEx_D3D11CaptureSwapChain* swapChain,
    const char* operation,
    HRESULT error)
{
    if (swapChain->unavailable)
        return;

    swapChain->unavailable = true;
    MsRdpEx_D3D11Capture_DisableHardwareMode(
        swapChain->instance, operation, error);
}

static bool MsRdpEx_D3D11Capture_CopySwapChainPixels(
    MsRdpEx_D3D11CaptureSwapChain* swapChain,
    std::vector<uint8_t>* pixels,
    UINT* width,
    UINT* height)
{
    ID3D11Texture2D* backBuffer = NULL;
    D3D11_TEXTURE2D_DESC description = {};
    D3D11_MAPPED_SUBRESOURCE mapped = {};
    HRESULT hr;

    if (swapChain->unavailable)
        return false;

    hr = swapChain->swapChain->GetBuffer(
        0, __uuidof(ID3D11Texture2D), (void**)&backBuffer);
    if (FAILED(hr))
    {
        MsRdpEx_D3D11Capture_MarkUnavailable(swapChain, "GetBuffer", hr);
        return false;
    }

    backBuffer->GetDesc(&description);
    if ((description.Format != DXGI_FORMAT_B8G8R8A8_UNORM &&
         description.Format != DXGI_FORMAT_B8G8R8A8_UNORM_SRGB &&
         description.Format != DXGI_FORMAT_B8G8R8X8_UNORM &&
         description.Format != DXGI_FORMAT_B8G8R8X8_UNORM_SRGB) ||
        description.SampleDesc.Count != 1)
    {
        MsRdpEx_D3D11Capture_MarkUnavailable(
            swapChain, "unsupported backbuffer format", E_NOTIMPL);
        backBuffer->Release();
        return false;
    }

    if (!swapChain->stagingTexture ||
        swapChain->width != description.Width ||
        swapChain->height != description.Height ||
        swapChain->format != description.Format)
    {
        if (swapChain->stagingTexture)
        {
            swapChain->stagingTexture->Release();
            swapChain->stagingTexture = NULL;
        }

        description.Usage = D3D11_USAGE_STAGING;
        description.BindFlags = 0;
        description.CPUAccessFlags = D3D11_CPU_ACCESS_READ;
        description.MiscFlags = 0;

        hr = swapChain->device->CreateTexture2D(
            &description, NULL, &swapChain->stagingTexture);
        if (FAILED(hr))
        {
            MsRdpEx_D3D11Capture_MarkUnavailable(swapChain, "CreateTexture2D", hr);
            backBuffer->Release();
            return false;
        }

        swapChain->width = description.Width;
        swapChain->height = description.Height;
        swapChain->format = description.Format;
    }

    swapChain->context->CopyResource(swapChain->stagingTexture, backBuffer);
    hr = swapChain->context->Map(
        swapChain->stagingTexture, 0, D3D11_MAP_READ, 0, &mapped);
    if (FAILED(hr))
    {
        MsRdpEx_D3D11Capture_MarkUnavailable(swapChain, "Map", hr);
        backBuffer->Release();
        return false;
    }

    pixels->resize((size_t)swapChain->width * swapChain->height * 4);
    for (UINT y = 0; y < swapChain->height; y++)
    {
        memcpy(pixels->data() + (size_t)y * swapChain->width * 4,
            (const uint8_t*)mapped.pData + (size_t)y * mapped.RowPitch,
            (size_t)swapChain->width * 4);
    }

    swapChain->context->Unmap(swapChain->stagingTexture, 0);
    backBuffer->Release();
    *width = swapChain->width;
    *height = swapChain->height;
    return true;
}

static HRESULT STDMETHODCALLTYPE Hook_CreateCompositionSwapChain(
    IDXGIFactory2* factory,
    IUnknown* device,
    const DXGI_SWAP_CHAIN_DESC1* description,
    IDXGIOutput* restrictToOutput,
    IDXGISwapChain1** swapChain)
{
    HRESULT hr = Real_CreateCompositionSwapChain(
        factory, device, description, restrictToOutput, swapChain);

    // DirectComposition can invoke the factory from outside mstscax.dll.
    // Device attribution below keeps this limited to an RDP-owned device.
    if (SUCCEEDED(hr) && swapChain && *swapChain)
    {
        MsRdpEx_D3D11Capture_TrackSwapChain(device, *swapChain);
    }

    return hr;
}

static HRESULT STDMETHODCALLTYPE Hook_CreateSwapChain(
    IDXGIFactory* factory,
    IUnknown* device,
    DXGI_SWAP_CHAIN_DESC* description,
    IDXGISwapChain** swapChain)
{
    HRESULT hr = Real_CreateSwapChain(factory, device, description, swapChain);

    if (SUCCEEDED(hr) && swapChain && *swapChain)
        MsRdpEx_D3D11Capture_TrackSwapChain(device, *swapChain);

    return hr;
}

static HRESULT STDMETHODCALLTYPE Hook_CreateSwapChainForHwnd(
    IDXGIFactory2* factory,
    IUnknown* device,
    HWND window,
    const DXGI_SWAP_CHAIN_DESC1* description,
    const DXGI_SWAP_CHAIN_FULLSCREEN_DESC* fullscreenDescription,
    IDXGIOutput* restrictToOutput,
    IDXGISwapChain1** swapChain)
{
    HRESULT hr = Real_CreateSwapChainForHwnd(
        factory, device, window, description, fullscreenDescription,
        restrictToOutput, swapChain);

    if (SUCCEEDED(hr) && swapChain && *swapChain)
        MsRdpEx_D3D11Capture_TrackSwapChain(device, *swapChain);

    return hr;
}

static HRESULT STDMETHODCALLTYPE Hook_SwapChainPresent(
    IDXGISwapChain* swapChain,
    UINT syncInterval,
    UINT flags)
{
    if (g_D3D11CaptureDepth == 0)
    {
        g_D3D11CaptureDepth++;
        std::vector<uint8_t> pixels;
        IMsRdpExInstance* instance = NULL;
        UINT width = 0;
        UINT height = 0;
        bool copied = false;

        AcquireSRWLockExclusive(&g_D3D11CaptureLock);
        for (MsRdpEx_D3D11CaptureSwapChain& entry : g_SwapChains)
        {
            if (entry.swapChain == swapChain)
            {
                entry.instance->AddRef();
                instance = entry.instance;
                copied = MsRdpEx_D3D11Capture_CopySwapChainPixels(
                    &entry, &pixels, &width, &height);
                break;
            }
        }
        ReleaseSRWLockExclusive(&g_D3D11CaptureLock);

        if (copied)
        {
            bool outputMirrorEnabled = false;
            const HRESULT enabledHr =
                instance->GetOutputMirrorEnabled(&outputMirrorEnabled);
            const bool captured = SUCCEEDED(enabledHr) && outputMirrorEnabled &&
                MsRdpEx_OutputMirror_CapturePixels(
                    instance, pixels.data(), width, height, width * 4);

            if (captured)
            {
                bool firstCapture = false;
                AcquireSRWLockExclusive(&g_D3D11CaptureLock);
                for (MsRdpEx_D3D11CaptureSwapChain& entry : g_SwapChains)
                {
                    if (entry.swapChain == swapChain && !entry.captureStarted)
                    {
                        entry.captureStarted = true;
                        firstCapture = true;
                        break;
                    }
                }
                ReleaseSRWLockExclusive(&g_D3D11CaptureLock);

                if (firstCapture)
                {
                    MsRdpEx_LogPrint(DEBUG,
                        "D3D11 capture received the first hardware frame");
                }
                MsRdpEx_Instance_NotifyOutputFrame(instance);
            }
            else if (SUCCEEDED(enabledHr) && outputMirrorEnabled)
            {
                AcquireSRWLockExclusive(&g_D3D11CaptureLock);
                for (MsRdpEx_D3D11CaptureSwapChain& entry : g_SwapChains)
                {
                    if (entry.swapChain == swapChain)
                    {
                        MsRdpEx_D3D11Capture_MarkUnavailable(
                            &entry, "upload output mirror pixels", E_FAIL);
                        break;
                    }
                }
                ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
            }
        }

        if (instance)
            instance->Release();

        g_D3D11CaptureDepth--;
    }

    return Real_SwapChainPresent(swapChain, syncInterval, flags);
}

static ULONG STDMETHODCALLTYPE Hook_SwapChainRelease(IDXGISwapChain* swapChain)
{
    swapChain->AddRef();
    const ULONG refCount = Real_SwapChainRelease(swapChain);
    if (refCount == 1)
        MsRdpEx_D3D11Capture_RemoveSwapChain(swapChain);
    return Real_SwapChainRelease(swapChain);
}

static void MsRdpEx_D3D11Capture_TrackDevice(ID3D11Device* device)
{
    IMsRdpExInstance* instance = MsRdpEx_D3D11Capture_FindPendingInstance();
    if (!instance)
        return;

    AcquireSRWLockExclusive(&g_D3D11CaptureLock);
    for (const MsRdpEx_D3D11CaptureDevice& entry : g_Devices)
    {
        if (entry.device == device)
        {
            ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
            instance->Release();
            return;
        }
    }

    device->AddRef();
    instance->AddRef();
    g_Devices.push_back({ device, instance });
    ReleaseSRWLockExclusive(&g_D3D11CaptureLock);

    MsRdpEx_LogPrint(DEBUG, "D3D11 capture tracking RDP device: %p", device);

    IDXGIDevice* dxgiDevice = NULL;
    IDXGIAdapter* adapter = NULL;
    IDXGIFactory2* factory = NULL;

    HRESULT hr = device->QueryInterface(__uuidof(IDXGIDevice), (void**)&dxgiDevice);
    if (SUCCEEDED(hr))
        hr = dxgiDevice->GetAdapter(&adapter);
    if (SUCCEEDED(hr))
        hr = adapter->GetParent(__uuidof(IDXGIFactory2), (void**)&factory);

    if (SUCCEEDED(hr))
    {
        void** vtable = *(void***)factory;

        AcquireSRWLockExclusive(&g_D3D11CaptureLock);
        const bool compositionHooked = MsRdpEx_D3D11Capture_AttachFactoryHook(
            vtable, 24, (PVOID*)&Real_CreateCompositionSwapChain,
            (PVOID)Hook_CreateCompositionSwapChain,
            &g_CreateCompositionSwapChainHookAttached,
            &g_CreateCompositionSwapChainTarget);
        const bool legacyHooked = MsRdpEx_D3D11Capture_AttachFactoryHook(
            vtable, 10, (PVOID*)&Real_CreateSwapChain,
            (PVOID)Hook_CreateSwapChain,
            &g_CreateSwapChainHookAttached,
            &g_CreateSwapChainTarget);
        const bool hwndHooked = MsRdpEx_D3D11Capture_AttachFactoryHook(
            vtable, 15, (PVOID*)&Real_CreateSwapChainForHwnd,
            (PVOID)Hook_CreateSwapChainForHwnd,
            &g_CreateSwapChainForHwndHookAttached,
            &g_CreateSwapChainForHwndTarget);
        ReleaseSRWLockExclusive(&g_D3D11CaptureLock);

        if (!compositionHooked && !legacyHooked && !hwndHooked)
        {
            MsRdpEx_D3D11Capture_DisableHardwareMode(
                instance, "hook RDP DXGI factory swap-chain methods", E_FAIL);
        }
    }
    else
    {
        MsRdpEx_D3D11Capture_DisableHardwareMode(
            instance, "locate the RDP composition factory", hr);
    }

    if (factory)
        factory->Release();
    if (adapter)
        adapter->Release();
    if (dxgiDevice)
        dxgiDevice->Release();
    instance->Release();
}

static HRESULT WINAPI Hook_D3D11CreateDevice(
    IDXGIAdapter* adapter,
    D3D_DRIVER_TYPE driverType,
    HMODULE software,
    UINT flags,
    const D3D_FEATURE_LEVEL* featureLevels,
    UINT featureLevelsCount,
    UINT sdkVersion,
    ID3D11Device** device,
    D3D_FEATURE_LEVEL* featureLevel,
    ID3D11DeviceContext** immediateContext)
{
    const HRESULT hr = Real_D3D11CreateDevice(
        adapter, driverType, software, flags, featureLevels, featureLevelsCount,
        sdkVersion, device, featureLevel, immediateContext);

    if (SUCCEEDED(hr) && device && *device &&
        MsRdpEx_IsAddressInRdpAxModule(_ReturnAddress()))
    {
        MsRdpEx_LogPrint(DEBUG,
            "D3D11CreateDevice returned for RDP ActiveX: %p", *device);
        MsRdpEx_D3D11Capture_TrackDevice(*device);
    }

    return hr;
}

void MsRdpEx_D3D11Capture_RegisterPendingInstance(IMsRdpExInstance* instance)
{
    if (!instance)
        return;

    bool anotherInstanceIsTracked = false;

    AcquireSRWLockExclusive(&g_D3D11CaptureLock);
    for (IMsRdpExInstance* pendingInstance : g_PendingInstances)
    {
        if (pendingInstance == instance)
        {
            ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
            return;
        }

        anotherInstanceIsTracked = true;
    }

    for (const MsRdpEx_D3D11CaptureDevice& device : g_Devices)
    {
        if (device.instance != instance)
        {
            anotherInstanceIsTracked = true;
            break;
        }
    }

    for (const MsRdpEx_D3D11CaptureSwapChain& swapChain : g_SwapChains)
    {
        if (swapChain.instance != instance)
        {
            anotherInstanceIsTracked = true;
            break;
        }
    }

    if (!anotherInstanceIsTracked)
    {
        instance->AddRef();
        g_PendingInstances.push_back(instance);
    }
    ReleaseSRWLockExclusive(&g_D3D11CaptureLock);

    if (anotherInstanceIsTracked)
    {
        MsRdpEx_D3D11Capture_DisableHardwareMode(
            instance, "multiple simultaneous hardware capture sessions", E_NOTIMPL);
    }
}

void MsRdpEx_D3D11Capture_ReleaseInstance(IMsRdpExInstance* instance)
{
    if (!instance)
        return;

    MsRdpEx_Instance_DisarmHardwareCaptureWatchdog(instance);

    AcquireSRWLockExclusive(&g_D3D11CaptureLock);

    for (size_t i = g_SwapChains.size(); i > 0; i--)
    {
        const size_t index = i - 1;
        if (g_SwapChains[index].instance == instance)
        {
            MsRdpEx_D3D11Capture_ReleaseSwapChain(&g_SwapChains[index]);
            g_SwapChains.erase(g_SwapChains.begin() + index);
        }
    }

    for (size_t i = g_Devices.size(); i > 0; i--)
    {
        const size_t index = i - 1;
        if (g_Devices[index].instance == instance)
        {
            g_Devices[index].device->Release();
            g_Devices[index].instance->Release();
            g_Devices.erase(g_Devices.begin() + index);
        }
    }

    for (size_t i = 0; i < g_PendingInstances.size(); i++)
    {
        if (g_PendingInstances[i] == instance)
        {
            g_PendingInstances[i]->Release();
            g_PendingInstances.erase(g_PendingInstances.begin() + i);
            break;
        }
    }
    ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
}

void MsRdpEx_D3D11Capture_AttachHooks()
{
    MSRDPEX_DETOUR_ATTACH(Real_D3D11CreateDevice, Hook_D3D11CreateDevice);
}

void MsRdpEx_D3D11Capture_DetachHooks()
{
    MSRDPEX_DETOUR_DETACH(Real_D3D11CreateDevice, Hook_D3D11CreateDevice);
    MSRDPEX_DETOUR_DETACH(Real_CreateCompositionSwapChain, Hook_CreateCompositionSwapChain);
    MSRDPEX_DETOUR_DETACH(Real_CreateSwapChain, Hook_CreateSwapChain);
    MSRDPEX_DETOUR_DETACH(Real_CreateSwapChainForHwnd, Hook_CreateSwapChainForHwnd);
    MSRDPEX_DETOUR_DETACH(Real_SwapChainPresent, Hook_SwapChainPresent);
    MSRDPEX_DETOUR_DETACH(Real_SwapChainRelease, Hook_SwapChainRelease);
}

void MsRdpEx_D3D11Capture_Shutdown()
{
    AcquireSRWLockExclusive(&g_D3D11CaptureLock);

    for (MsRdpEx_D3D11CaptureSwapChain& swapChain : g_SwapChains)
        MsRdpEx_D3D11Capture_ReleaseSwapChain(&swapChain);
    g_SwapChains.clear();

    for (MsRdpEx_D3D11CaptureDevice& device : g_Devices)
    {
        device.device->Release();
        device.instance->Release();
    }
    g_Devices.clear();

    for (IMsRdpExInstance* instance : g_PendingInstances)
    {
        MsRdpEx_Instance_DisarmHardwareCaptureWatchdog(instance);
        instance->Release();
    }
    g_PendingInstances.clear();

    g_CreateCompositionSwapChainHookAttached = false;
    g_CreateSwapChainHookAttached = false;
    g_CreateSwapChainForHwndHookAttached = false;
    g_SwapChainPresentHookAttached = false;
    g_SwapChainReleaseHookAttached = false;
    g_CreateCompositionSwapChainTarget = NULL;
    g_CreateSwapChainTarget = NULL;
    g_CreateSwapChainForHwndTarget = NULL;
    g_SwapChainPresentTarget = NULL;
    g_SwapChainReleaseTarget = NULL;
    Real_CreateCompositionSwapChain = NULL;
    Real_CreateSwapChain = NULL;
    Real_CreateSwapChainForHwnd = NULL;
    Real_SwapChainPresent = NULL;
    Real_SwapChainRelease = NULL;

    ReleaseSRWLockExclusive(&g_D3D11CaptureLock);
}
