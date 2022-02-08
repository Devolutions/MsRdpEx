
#include <MsRdpEx/Memory.h>

bool MsRdpEx_IsBadReadPtr(void* ptr, size_t size)
{
    bool ok;
    DWORD mask;
    BYTE* p = (BYTE*) ptr;
    BYTE* maxPtr = p + size;
    BYTE* regionEnd = NULL;
    MEMORY_BASIC_INFORMATION mbi;

    if (size < 0)
        return false;

    if (!p)
        return true;

    mask = PAGE_READONLY | PAGE_READWRITE | PAGE_WRITECOPY | PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE | PAGE_EXECUTE_WRITECOPY;

    do
    {
        if (p == ptr || p == regionEnd)
        {
            if (VirtualQuery((LPCVOID)p, &mbi, sizeof(mbi)) == 0)
            {
                return true;
            }
            else
            {
                regionEnd = ((BYTE*) mbi.BaseAddress + mbi.RegionSize);
            }
        }

        ok = (mbi.Protect & mask) != 0;

        if (mbi.Protect & (PAGE_GUARD | PAGE_NOACCESS))
        {
            ok = false;
        }

        if (!ok)
        {
            return true;
        }

        if (maxPtr <= regionEnd) /* the whole address range is inside the current memory region */
        {
            return false;
        }
        else if (maxPtr > regionEnd) /* this region is a part of (or overlaps with) the address range we are checking */
        {
            p = regionEnd; /* lets move to the next memory region */
        }
    } while (p < maxPtr);

    return false;
}

bool MsRdpEx_CanReadUnsafePtr(void* ptr, size_t size)
{
    return MsRdpEx_IsBadReadPtr(ptr, size) ? false : true;
}

bool MsRdpEx_StringEqualsUnsafePtr(const char* ptr, const char* str)
{
    size_t length = strlen(str);

    if (length < 1)
        return false;

    if (!MsRdpEx_CanReadUnsafePtr((void*) ptr, length + 1))
        return false;

    return (strcmp(ptr, str) == 0) ? true : false;
}

bool MsRdpEx_StringIEqualsUnsafePtr(const char* ptr, const char* str)
{
    size_t length = strlen(str);

    if (length < 1)
        return false;

    if (!MsRdpEx_CanReadUnsafePtr((void*)ptr, length + 1))
        return false;

    return (_stricmp(ptr, str) == 0) ? true : false;
}
