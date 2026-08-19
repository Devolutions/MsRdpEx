#ifndef MSRDPEX_OUTPUT_MIRROR_CAPTURE_H
#define MSRDPEX_OUTPUT_MIRROR_CAPTURE_H

#include "RdpInstanceInternal.h"

#include <MsRdpEx/OutputMirror.h>
#include <MsRdpEx/RdpSettings.h>

MsRdpEx_OutputMirror* MsRdpEx_OutputMirror_GetOrCreate(
    IMsRdpExInstance* instance,
    CMsRdpExtendedSettings* extendedSettings);

bool MsRdpEx_OutputMirror_CapturePixels(
    IMsRdpExInstance* instance,
    const uint8_t* pixels,
    uint32_t width,
    uint32_t height,
    uint32_t sourceStride);

#endif /* MSRDPEX_OUTPUT_MIRROR_CAPTURE_H */
