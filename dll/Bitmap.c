
#include "MsRdpEx.h"

#include <MsRdpEx/MsRdpEx.h>

#pragma pack(push, 1)

struct _MSRDPEX_BITMAP_FILE_HEADER
{
	uint8_t bfType[2];
	uint32_t bfSize;
	uint16_t bfReserved1;
	uint16_t bfReserved2;
	uint32_t bfOffBits;
};
typedef struct _MSRDPEX_BITMAP_FILE_HEADER MSRDPEX_BITMAP_FILE_HEADER;

struct _MSRDPEX_BITMAP_INFO_HEADER
{
	uint32_t biSize;
	int32_t biWidth;
	int32_t biHeight;
	uint16_t biPlanes;
	uint16_t biBitCount;
	uint32_t biCompression;
	uint32_t biSizeImage;
	int32_t biXPelsPerMeter;
	int32_t biYPelsPerMeter;
	uint32_t biClrUsed;
	uint32_t biClrImportant;
};
typedef struct _MSRDPEX_BITMAP_INFO_HEADER MSRDPEX_BITMAP_INFO_HEADER;

struct _MSRDPEX_BITMAP_CORE_HEADER
{
	uint32_t bcSize;
	uint16_t bcWidth;
	uint16_t bcHeight;
	uint16_t bcPlanes;
	uint16_t bcBitCount;
};
typedef struct _MSRDPEX_BITMAP_CORE_HEADER MSRDPEX_BITMAP_CORE_HEADER;

#pragma pack(pop)

bool MsRdpEx_WriteBitmapFile(const char* filename, uint8_t* data, int width, int height, int bpp)
{
	FILE* fp;
	bool success = true;
	MSRDPEX_BITMAP_FILE_HEADER bf;
	MSRDPEX_BITMAP_INFO_HEADER bi;

	ZeroMemory(&bf, sizeof(MSRDPEX_BITMAP_FILE_HEADER));
	ZeroMemory(&bi, sizeof(MSRDPEX_BITMAP_INFO_HEADER));

	fp = fopen(filename, "w+b");

	if (!fp)
		return false;

	bf.bfType[0] = 'B';
	bf.bfType[1] = 'M';
	bf.bfReserved1 = 0;
	bf.bfReserved2 = 0;
	bf.bfOffBits = sizeof(MSRDPEX_BITMAP_FILE_HEADER) + sizeof(MSRDPEX_BITMAP_INFO_HEADER);
	bi.biSizeImage = width * height * (bpp / 8);
	bf.bfSize = bf.bfOffBits + bi.biSizeImage;

	bi.biWidth = width;
	bi.biHeight = -1 * height;
	bi.biPlanes = 1;
	bi.biBitCount = bpp;
	bi.biCompression = 0;
	bi.biXPelsPerMeter = 0;
	bi.biYPelsPerMeter = 0;
	bi.biClrUsed = 0;
	bi.biClrImportant = 0;
	bi.biSize = sizeof(MSRDPEX_BITMAP_INFO_HEADER);

	if (fwrite(&bf, sizeof(MSRDPEX_BITMAP_FILE_HEADER), 1, fp) != 1 ||
		fwrite(&bi, sizeof(MSRDPEX_BITMAP_INFO_HEADER), 1, fp) != 1 ||
		fwrite(data, bi.biSizeImage, 1, fp) != 1)
	{
		success = false;
	}

	fclose(fp);

	return success;
}

HBITMAP MsRdpEx_CreateDIBSection(HDC hDC, int width, int height, int bpp, uint8_t** ppPixelData)
{
    BITMAPINFO bitmapInfo;
    ZeroMemory(&bitmapInfo, sizeof(BITMAPINFO));
    bitmapInfo.bmiHeader.biSize = sizeof(BITMAPINFO);
    bitmapInfo.bmiHeader.biWidth = width;
	bitmapInfo.bmiHeader.biHeight = -1 * height;
    bitmapInfo.bmiHeader.biPlanes = 1;
    bitmapInfo.bmiHeader.biBitCount = bpp;
    bitmapInfo.bmiHeader.biCompression = BI_RGB;
    return CreateDIBSection(hDC, &bitmapInfo, DIB_RGB_COLORS, (void**) ppPixelData, NULL, 0);
}
