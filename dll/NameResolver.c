
#include <MsRdpEx/NameResolver.h>

#include <MsRdpEx/HashTable.h>

struct _MsRdpEx_NameResolver
{
	MsRdpEx_HashTable* renaming;
};

void MsRdpEx_NameResolver_Free(MsRdpEx_NameResolver* ctx);

static int g_RefCount = 0;
static MsRdpEx_NameResolver* g_NameResolver = NULL;

bool MsRdpEx_NameResolver_GetMapping(const char* oldName, char** pNewName)
{
	MsRdpEx_NameResolver* ctx = g_NameResolver;

	if (!ctx)
		return false;

	char* newName = MsRdpEx_HashTable_GetItemValue(ctx->renaming, (void*) oldName);

	if (newName) {
		*pNewName = _strdup(newName);

		if (*pNewName != NULL) {
			return true;
		}
	}

	return false;
}

bool MsRdpEx_NameResolver_RemapName(const char* oldName, const char* newName)
{
    MsRdpEx_NameResolver* ctx = g_NameResolver;

    if (!ctx)
        return false;

	MsRdpEx_LogPrint(DEBUG, "RemapName: %s -> %s", oldName, newName);

    MsRdpEx_HashTable_Add(ctx->renaming, (void*) oldName, (void*) newName);

    return true;
}

bool MsRdpEx_NameResolver_UnmapName(const char* name)
{
    MsRdpEx_NameResolver* ctx = g_NameResolver;

    if (!ctx)
        return false;

    return MsRdpEx_HashTable_Remove(ctx->renaming, (void*) name);
}

MsRdpEx_NameResolver* MsRdpEx_NameResolver_New()
{
	MsRdpEx_NameResolver* ctx;

	ctx = (MsRdpEx_NameResolver*) calloc(1, sizeof(MsRdpEx_NameResolver));

	if (!ctx)
		return NULL;

	ctx->renaming = MsRdpEx_HashTable_New(true, MSRDPEX_HASHTABLE_FLAGS_NONE);

	if (!ctx->renaming)
		goto error;

    MsRdpEx_HashTable_SetHashFunction(ctx->renaming, MsRdpEx_HashTable_StringHash);
    MsRdpEx_HashTable_SetKeyCompareFunction(ctx->renaming, MsRdpEx_StringIEquals);
    MsRdpEx_HashTable_SetKeyCloneFunction(ctx->renaming, MsRdpEx_HashTable_StringClone);
    MsRdpEx_HashTable_SetKeyFreeFunction(ctx->renaming, MsRdpEx_HashTable_StringFree);
	MsRdpEx_HashTable_SetValueCompareFunction(ctx->renaming, MsRdpEx_StringIEquals);
	MsRdpEx_HashTable_SetValueCloneFunction(ctx->renaming, MsRdpEx_HashTable_StringClone);
	MsRdpEx_HashTable_SetValueFreeFunction(ctx->renaming, MsRdpEx_HashTable_StringFree);

	return ctx;
error:
	MsRdpEx_NameResolver_Free(ctx);
	return NULL;
}

void MsRdpEx_NameResolver_Free(MsRdpEx_NameResolver* ctx)
{
	if (!ctx)
		return;

	if (ctx->renaming) {
		MsRdpEx_HashTable_Free(ctx->renaming);
		ctx->renaming = NULL;
	}
}

MsRdpEx_NameResolver* MsRdpEx_NameResolver_Get()
{
	if (!g_NameResolver)
		g_NameResolver = MsRdpEx_NameResolver_New(true);

	g_RefCount++;

	return g_NameResolver;
}

void MsRdpEx_NameResolver_Release()
{
	g_RefCount--;

	if (g_RefCount < 0)
		g_RefCount = 0;

	if (g_NameResolver && (g_RefCount < 1))
	{
		MsRdpEx_NameResolver_Free(g_NameResolver);
		g_NameResolver = NULL;
	}
}
