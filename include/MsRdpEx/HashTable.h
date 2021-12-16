
#ifndef MSRDPEX_HASHTABLE_H
#define MSRDPEX_HASHTABLE_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

#define MSRDPEX_HASHTABLE_INT_KEY(x) (void*)(size_t)(x)
#define MSRDPEX_HASHTABLE_INT_VALUE(x) (void*)(size_t)(x)

#define MSRDPEX_HASHTABLE_FLAGS_NONE                0x00
#define MSRDPEX_HASHTABLE_FLAGS_ACCEPT_NULL_KEYS    0x01
#define MSRDPEX_HASHTABLE_FLAGS_ACCEPT_NULL_VALUES  0x02

typedef uint32_t (*MSRDPEX_HASHTABLE_HASH_FN)(void* key);
typedef bool (*MSRDPEX_HASHTABLE_KEY_COMPARE_FN)(void* key1, void* key2);
typedef bool (*MSRDPEX_HASHTABLE_VALUE_COMPARE_FN)(void* value1, void* value2);
typedef void* (*MSRDPEX_HASHTABLE_KEY_CLONE_FN)(void* key);
typedef void* (*MSRDPEX_HASHTABLE_VALUE_CLONE_FN)(void* value);
typedef void (*MSRDPEX_HASHTABLE_KEY_FREE_FN)(void* key);
typedef void (*MSRDPEX_HASHTABLE_VALUE_FREE_FN)(void* value);

typedef struct msrdpex_key_value_pair MsRdpEx_KeyValuePair;
typedef struct msrdpex_hash_table MsRdpEx_HashTable;

void MsRdpEx_HashTable_SetHashFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_HASH_FN hash);
void MsRdpEx_HashTable_SetKeyCompareFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_KEY_COMPARE_FN keyCompare);
void MsRdpEx_HashTable_SetValueCompareFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_VALUE_COMPARE_FN valueCompare);
void MsRdpEx_HashTable_SetKeyCloneFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_KEY_CLONE_FN keyClone);
void MsRdpEx_HashTable_SetValueCloneFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_VALUE_CLONE_FN valueClone);
void MsRdpEx_HashTable_SetKeyFreeFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_KEY_FREE_FN keyFree);
void MsRdpEx_HashTable_SetValueFreeFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_VALUE_FREE_FN valueFree);

/**
 * Lock access to the hash table.
 */
void MsRdpEx_HashTable_Lock(MsRdpEx_HashTable* table);

/**
 * Unlock access to the hash table.
 */
void MsRdpEx_HashTable_Unlock(MsRdpEx_HashTable* table);

/**
 * Gets the number of key/value pairs contained in the HashTable.
 */
int MsRdpEx_HashTable_Count(MsRdpEx_HashTable* table);

/**
 * Adds an element with the specified key and value into the HashTable.
 */
int MsRdpEx_HashTable_Add(MsRdpEx_HashTable* table, void* key, void* value);

/**
 * Removes the element with the specified key from the HashTable.
 */
bool MsRdpEx_HashTable_Remove(MsRdpEx_HashTable* table, void* key);

bool MsRdpEx_HashTable_RemoveValue(MsRdpEx_HashTable* table, void* value);

/**
 * Removes all elements from the HashTable.
 */
void MsRdpEx_HashTable_Clear(MsRdpEx_HashTable* table);

/**
 * Determines whether the HashTable contains a specific key.
 */
bool MsRdpEx_HashTable_ContainsKey(MsRdpEx_HashTable* table, void* key);

/**
 * Determines whether the HashTable contains a specific value.
 */
bool MsRdpEx_HashTable_ContainsValue(MsRdpEx_HashTable* table, void* value);

/**
 * Get an item value using key.
 */
void* MsRdpEx_HashTable_GetItemValue(MsRdpEx_HashTable* table, void* key);

/**
 * Set an item value using key.
 */
bool MsRdpEx_HashTable_SetItemValue(MsRdpEx_HashTable* table, void* key, void* value);

/**
 * Gets the list of keys as an array.
 */
int MsRdpEx_HashTable_GetKeys(MsRdpEx_HashTable* table, ULONG_PTR** ppKeys);

uint32_t MsRdpEx_HashTable_PointerHash(void* pointer);
bool MsRdpEx_HashTable_PointerCompare(void* pointer1, void* pointer2);
uint32_t MsRdpEx_HashTable_StringHash(void* key);
bool MsRdpEx_HashTable_StringCompare(void* string1, void* string2);
void* MsRdpEx_HashTable_StringClone(void* str);
void MsRdpEx_HashTable_StringFree(void* str);
bool MsRdpEx_HashTable_UInt32Compare(void* v1, void* v2);
uint32_t MsRdpEx_HashTable_UInt32Hash(void* v);

MsRdpEx_HashTable* MsRdpEx_HashTable_New(bool synchronized, uint8_t flags);
void MsRdpEx_HashTable_Free(MsRdpEx_HashTable* table);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_HASHTABLE_H */