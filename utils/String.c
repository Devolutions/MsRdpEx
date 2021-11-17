
#include "Utils.h"

int MsRdpEx_ConvertFromUnicode(UINT CodePage, DWORD dwFlags, LPCWSTR lpWideCharStr, int cchWideChar,
                       LPSTR* lpMultiByteStr, int cbMultiByte, LPCSTR lpDefaultChar,
                       LPBOOL lpUsedDefaultChar)
{
    int status;
    BOOL allocate = FALSE;

    if (!lpWideCharStr)
        return 0;

    if (!lpMultiByteStr)
        return 0;

    if (cchWideChar == -1)
        cchWideChar = (int)(wcslen(lpWideCharStr) + 1);

    if (cbMultiByte == 0)
    {
        cbMultiByte =
            WideCharToMultiByte(CodePage, dwFlags, lpWideCharStr, cchWideChar, NULL, 0, NULL, NULL);
        allocate = TRUE;
    }
    else if (!(*lpMultiByteStr))
        allocate = TRUE;

    if (cbMultiByte < 1)
        return 0;

    if (allocate)
    {
        *lpMultiByteStr = (LPSTR)calloc(1, cbMultiByte + 1);

        if (!(*lpMultiByteStr))
        {
            return 0;
        }
    }

    status = WideCharToMultiByte(CodePage, dwFlags, lpWideCharStr, cchWideChar, *lpMultiByteStr,
                                 cbMultiByte, lpDefaultChar, lpUsedDefaultChar);

    if ((status != cbMultiByte) && allocate)
    {
        status = 0;
    }

    if ((status <= 0) && allocate)
    {
        free(*lpMultiByteStr);
        *lpMultiByteStr = NULL;
    }

    return status;
}

int MsRdpEx_ConvertToUnicode(UINT CodePage, DWORD dwFlags, LPCSTR lpMultiByteStr, int cbMultiByte,
                     LPWSTR* lpWideCharStr, int cchWideChar)
{
    int status;
    BOOL allocate = FALSE;

    if (!lpMultiByteStr)
        return 0;

    if (!lpWideCharStr)
        return 0;

    if (cbMultiByte == -1)
    {
        size_t len = strnlen(lpMultiByteStr, INT_MAX);
        if (len >= INT_MAX)
            return 0;
        cbMultiByte = (int)(len + 1);
    }

    if (cchWideChar == 0)
    {
        cchWideChar = MultiByteToWideChar(CodePage, dwFlags, lpMultiByteStr, cbMultiByte, NULL, 0);
        allocate = TRUE;
    }
    else if (!(*lpWideCharStr))
        allocate = TRUE;

    if (cchWideChar < 1)
        return 0;

    if (allocate)
    {
        *lpWideCharStr = (LPWSTR)calloc(cchWideChar + 1, sizeof(WCHAR));

        if (!(*lpWideCharStr))
        {
            return 0;
        }
    }

    status = MultiByteToWideChar(CodePage, dwFlags, lpMultiByteStr, cbMultiByte, *lpWideCharStr, cchWideChar);

    if (status != cchWideChar)
    {
        if (allocate)
        {
            free(*lpWideCharStr);
            *lpWideCharStr = NULL;
            status = 0;
        }
    }

    return status;
}
