#ifndef MSRDPEX_ARRAY_LIST_H
#define MSRDPEX_ARRAY_LIST_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

typedef void* (*MSRDPEX_OBJECT_NEW_FN)(void);
typedef void (*MSRDPEX_OBJECT_INIT_FN)(void* obj);
typedef void (*MSRDPEX_OBJECT_UNINIT_FN)(void* obj);
typedef void (*MSRDPEX_OBJECT_FREE_FN)(void* obj);
typedef bool (*MSRDPEX_OBJECT_EQUALS_FN)(void* objA, void* objB);

typedef bool (*MSRDPEX_OBJECT_MATCH_FN)(void* obj, void* param);
typedef int (*MSRDPEX_OBJECT_COMPARE_FN)(void* objA, void* objB);

struct msrdpex_object
{
	MSRDPEX_OBJECT_NEW_FN fnObjectNew;
	MSRDPEX_OBJECT_INIT_FN fnObjectInit;
	MSRDPEX_OBJECT_UNINIT_FN fnObjectUninit;
	MSRDPEX_OBJECT_FREE_FN fnObjectFree;
	MSRDPEX_OBJECT_EQUALS_FN fnObjectEquals;
};
typedef struct msrdpex_object MsRdpEx_Object;

typedef struct msrdpex_array_list MsRdpEx_ArrayList;
typedef struct msrdpex_array_list_it MsRdpEx_ArrayListIt;

#define MSRDPEX_ITERATOR_FLAG_EXCLUSIVE		0x00000001
#define MSRDPEX_ITERATOR_FLAG_DUPLICATE		0x00000002
#define MSRDPEX_ITERATOR_FLAG_REVERSE		0x00000004

int MsRdpEx_ArrayListIt_Count(MsRdpEx_ArrayListIt* it);
void MsRdpEx_ArrayListIt_Reset(MsRdpEx_ArrayListIt* it);
void* MsRdpEx_ArrayListIt_Current(MsRdpEx_ArrayListIt* it);
bool MsRdpEx_ArrayListIt_Move(MsRdpEx_ArrayListIt* it);
void* MsRdpEx_ArrayListIt_Next(MsRdpEx_ArrayListIt* it);
bool MsRdpEx_ArrayListIt_Done(MsRdpEx_ArrayListIt* it);

MsRdpEx_ArrayListIt* MsRdpEx_ArrayList_It(MsRdpEx_ArrayList* ctx, uint32_t flags);
void MsRdpEx_ArrayListIt_Finish(MsRdpEx_ArrayListIt* it);

int MsRdpEx_ArrayList_Capacity(MsRdpEx_ArrayList* ctx);
int MsRdpEx_ArrayList_Count(MsRdpEx_ArrayList* ctx);
bool MsRdpEx_ArrayList_IsEmpty(MsRdpEx_ArrayList* ctx);
int MsRdpEx_ArrayList_Items(MsRdpEx_ArrayList* ctx, uintptr_t** ppItems);
bool MsRdpEx_ArrayList_IsSynchronized(MsRdpEx_ArrayList* ctx);

void MsRdpEx_ArrayList_Lock(MsRdpEx_ArrayList* ctx);
void MsRdpEx_ArrayList_Unlock(MsRdpEx_ArrayList* ctx);

void* MsRdpEx_ArrayList_GetItem(MsRdpEx_ArrayList* ctx, int index);
bool MsRdpEx_ArrayList_SetItem(MsRdpEx_ArrayList* ctx, int index, void* obj);

void* MsRdpEx_ArrayList_GetHead(MsRdpEx_ArrayList* ctx);
void* MsRdpEx_ArrayList_GetTail(MsRdpEx_ArrayList* ctx);

bool MsRdpEx_ArrayList_DefaultEquals(void* objA, void* objB);

MsRdpEx_Object* MsRdpEx_ArrayList_Object(MsRdpEx_ArrayList* ctx);

void* MsRdpEx_ArrayList_Find(MsRdpEx_ArrayList* ctx, MSRDPEX_OBJECT_MATCH_FN fnMatch, void* param);

void MsRdpEx_ArrayList_Clear(MsRdpEx_ArrayList* ctx, bool free);
bool MsRdpEx_ArrayList_Contains(MsRdpEx_ArrayList* ctx, void* obj);

int MsRdpEx_ArrayList_Add(MsRdpEx_ArrayList* ctx, void* obj);
bool MsRdpEx_ArrayList_InsertAt(MsRdpEx_ArrayList* ctx, int index, void* obj);
int MsRdpEx_ArrayList_Insert(MsRdpEx_ArrayList* ctx, void* obj, MSRDPEX_OBJECT_COMPARE_FN fnCompare);

void* MsRdpEx_ArrayList_Remove(MsRdpEx_ArrayList* ctx, void* obj, bool free);
void* MsRdpEx_ArrayList_RemoveAt(MsRdpEx_ArrayList* ctx, int index, bool free);

void* MsRdpEx_ArrayList_RemoveHead(MsRdpEx_ArrayList* ctx, bool free);
void* MsRdpEx_ArrayList_RemoveTail(MsRdpEx_ArrayList* ctx, bool free);

int MsRdpEx_ArrayList_RemoveAll(MsRdpEx_ArrayList* ctx, MSRDPEX_OBJECT_MATCH_FN fnMatch, void* param, bool free);
int MsRdpEx_ArrayList_RemoveDuplicates(MsRdpEx_ArrayList* ctx, MSRDPEX_OBJECT_EQUALS_FN fnEquals, bool free);

int MsRdpEx_ArrayList_IndexOf(MsRdpEx_ArrayList* ctx, void* obj, int startIndex, int count);
int MsRdpEx_ArrayList_LastIndexOf(MsRdpEx_ArrayList* ctx, void* obj, int startIndex, int count);

MsRdpEx_ArrayList* MsRdpEx_ArrayList_New(bool synchronized);
void MsRdpEx_ArrayList_Free(MsRdpEx_ArrayList* ctx);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_ARRAY_LIST_H */
