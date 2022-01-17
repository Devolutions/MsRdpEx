
#include <MsRdpEx/HashTable.h>

struct msrdpex_key_value_pair
{
	void* key;
	void* value;
	MsRdpEx_KeyValuePair* next;
};

struct msrdpex_hash_table
{
	bool synchronized;
	bool acceptNullKeys;
	bool acceptNullValues;
	CRITICAL_SECTION lock;

	int numOfBuckets;
	int numOfElements;
	float idealRatio;
	float lowerRehashThreshold;
	float upperRehashThreshold;
	MsRdpEx_KeyValuePair** bucketArray;

	MSRDPEX_HASHTABLE_HASH_FN hash;
	MSRDPEX_HASHTABLE_KEY_COMPARE_FN keyCompare;
	MSRDPEX_HASHTABLE_VALUE_COMPARE_FN valueCompare;
	MSRDPEX_HASHTABLE_KEY_CLONE_FN keyClone;
	MSRDPEX_HASHTABLE_VALUE_CLONE_FN valueClone;
	MSRDPEX_HASHTABLE_KEY_FREE_FN keyFree;
	MSRDPEX_HASHTABLE_VALUE_FREE_FN valueFree;
};

bool MsRdpEx_HashTable_PointerCompare(void* pointer1, void* pointer2)
{
	return (pointer1 == pointer2);
}

uint32_t MsRdpEx_HashTable_PointerHash(void* pointer)
{
	return ((uint32_t) (UINT_PTR) pointer) >> 4;
}

bool MsRdpEx_HashTable_StringCompare(void* string1, void* string2)
{
	if (!string1 || !string2)
		return (string1 == string2);

	return (strcmp((char*) string1, (char*) string2) == 0);
}

uint32_t MsRdpEx_HashTable_StringHash(void* key)
{
	uint32_t c;
	uint32_t hash = 5381;
	uint8_t* str = (uint8_t*) key;

	/* djb2 algorithm */
	while ((c = *str++) != '\0')
		hash = (hash * 33) + c;

	return hash;
}

void* MsRdpEx_HashTable_StringClone(void* str)
{
	return _strdup((char*) str);
}

void MsRdpEx_HashTable_StringFree(void* str)
{
	free(str);
}

bool MsRdpEx_HashTable_UInt32Compare(void* v1, void* v2)
{
	return (v1 == v2);
}

uint32_t MsRdpEx_HashTable_UInt32Hash(void* v)
{
	return (uint32_t)(size_t) v;
}

static int MsRdpEx_HashTable_IsProbablePrime(int oddNumber)
{
	int i;

	for (i = 3; i < 51; i += 2)
	{
		if (oddNumber == i)
			return 1;
		else if (oddNumber % i == 0)
			return 0;
	}

	return 1; /* maybe */
}

static long MsRdpEx_HashTable_CalculateIdealNumOfBuckets(MsRdpEx_HashTable* table)
{
	int idealNumOfBuckets = table->numOfElements / ((int) table->idealRatio);

	if (idealNumOfBuckets < 5)
		idealNumOfBuckets = 5;
	else
		idealNumOfBuckets |= 0x01;

	while (!MsRdpEx_HashTable_IsProbablePrime(idealNumOfBuckets))
		idealNumOfBuckets += 2;

	return idealNumOfBuckets;
}

void MsRdpEx_HashTable_SetHashFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_HASH_FN hash)
{
	table->hash = hash;
}

void MsRdpEx_HashTable_SetKeyCompareFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_KEY_COMPARE_FN keyCompare)
{
	table->keyCompare = keyCompare;
}

void MsRdpEx_HashTable_SetValueCompareFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_VALUE_COMPARE_FN valueCompare)
{
	table->valueCompare = valueCompare;
}

void MsRdpEx_HashTable_SetKeyCloneFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_KEY_CLONE_FN keyClone)
{
	table->keyClone = keyClone;
}

void MsRdpEx_HashTable_SetValueCloneFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_VALUE_CLONE_FN valueClone)
{
	table->valueClone = valueClone;
}

void MsRdpEx_HashTable_SetKeyFreeFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_KEY_FREE_FN keyFree)
{
	table->keyFree = keyFree;
}

void MsRdpEx_HashTable_SetValueFreeFunction(MsRdpEx_HashTable* table, MSRDPEX_HASHTABLE_VALUE_FREE_FN valueFree)
{
	table->valueFree = valueFree;
}

void MsRdpEx_HashTable_Lock(MsRdpEx_HashTable* table)
{
	EnterCriticalSection(&table->lock);
}

void MsRdpEx_HashTable_Unlock(MsRdpEx_HashTable* table)
{
	LeaveCriticalSection(&table->lock);
}

void MsRdpEx_HashTable_Rehash(MsRdpEx_HashTable* table, int numOfBuckets)
{
	int index;
	uint32_t hashValue;
	MsRdpEx_KeyValuePair* pair;
	MsRdpEx_KeyValuePair* nextPair;
	MsRdpEx_KeyValuePair** newBucketArray;

	if (numOfBuckets == 0)
		numOfBuckets = MsRdpEx_HashTable_CalculateIdealNumOfBuckets(table);

	if (numOfBuckets == table->numOfBuckets)
		return; /* already the right size! */

	newBucketArray = (MsRdpEx_KeyValuePair**) calloc(numOfBuckets, sizeof(MsRdpEx_KeyValuePair*));

	if (!newBucketArray)
	{
		/*
		 * Couldn't allocate memory for the new array.
		 * This isn't a fatal error; we just can't perform the rehash.
		 */
		return;
	}

	for (index = 0; index < table->numOfBuckets; index++)
	{
		pair = table->bucketArray[index];

		while (pair)
		{
			nextPair = pair->next;
			hashValue = table->hash(pair->key) % numOfBuckets;
			pair->next = newBucketArray[hashValue];
			newBucketArray[hashValue] = pair;
			pair = nextPair;
		}
	}

	free(table->bucketArray);
	table->bucketArray = newBucketArray;
	table->numOfBuckets = numOfBuckets;
}

void MsRdpEx_HashTable_SetIdealRatio(MsRdpEx_HashTable* table, float idealRatio, float lowerRehashThreshold,
				float upperRehashThreshold)
{
	table->idealRatio = idealRatio;
	table->lowerRehashThreshold = lowerRehashThreshold;
	table->upperRehashThreshold = upperRehashThreshold;
}

MsRdpEx_KeyValuePair* MsRdpEx_HashTable_Get(MsRdpEx_HashTable* table, void* key)
{
	uint32_t hashValue;
	MsRdpEx_KeyValuePair* pair;

	hashValue = table->hash(key) % table->numOfBuckets;

	pair = table->bucketArray[hashValue];

	while (pair && !table->keyCompare(key, pair->key))
		pair = pair->next;

	return pair;
}

int MsRdpEx_HashTable_Count(MsRdpEx_HashTable* table)
{
	return table->numOfElements;
}

int MsRdpEx_HashTable_Add(MsRdpEx_HashTable* table, void* key, void* value)
{
	int status = 0;
	uint32_t hashValue;
	MsRdpEx_KeyValuePair* pair;
	MsRdpEx_KeyValuePair* newPair;

	if (!table->acceptNullKeys && !key)
		return -1;

	if (!table->acceptNullValues && !value)
		return -1;

	if (table->keyClone)
	{
		key = table->keyClone(key);

		if (!table->acceptNullKeys && !key)
			return -1;
	}

	if (table->valueClone)
	{
		value = table->valueClone(value);

		if (!table->acceptNullValues && !value)
			return -1;
	}

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	hashValue = table->hash(key) % table->numOfBuckets;
	pair = table->bucketArray[hashValue];

	while (pair && !table->keyCompare(key, pair->key))
		pair = pair->next;

	if (pair)
	{
		if (pair->key != key)
		{
			if (table->keyFree)
				table->keyFree(pair->key);
			pair->key = key;
		}

		if (pair->value != value)
		{
			if (table->valueFree)
				table->valueFree(pair->value);
			pair->value = value;
		}
	}
	else
	{
		newPair = (MsRdpEx_KeyValuePair*) malloc(sizeof(MsRdpEx_KeyValuePair));

		if (!newPair)
		{
			status = -1;
		}
		else
		{
			newPair->key = key;
			newPair->value = value;
			newPair->next = table->bucketArray[hashValue];
			table->bucketArray[hashValue] = newPair;
			table->numOfElements++;

			if (table->upperRehashThreshold > table->idealRatio)
			{
				float elementToBucketRatio = (float) table->numOfElements / (float) table->numOfBuckets;

				if (elementToBucketRatio > table->upperRehashThreshold)
					MsRdpEx_HashTable_Rehash(table, 0);
			}
		}
	}

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	return status;
}

bool MsRdpEx_HashTable_RemoveValue(MsRdpEx_HashTable* table, void* value)
{
	int index;
	bool status = false;
	MsRdpEx_KeyValuePair* pair;

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	for (index = 0; index < table->numOfBuckets; index++)
	{
		pair = table->bucketArray[index];

		while (pair)
		{
			if (table->valueCompare(value, pair->value))
			{
				status = true;
				MsRdpEx_HashTable_Remove(table, pair->key);
				break;
			}

			pair = pair->next;
		}

		if (status)
			break;
	}

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	return status;
}

bool MsRdpEx_HashTable_Remove(MsRdpEx_HashTable* table, void* key)
{
	uint32_t hashValue;
	bool status = true;
	MsRdpEx_KeyValuePair* pair = NULL;
	MsRdpEx_KeyValuePair* previousPair = NULL;

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	hashValue = table->hash(key) % table->numOfBuckets;

	pair = table->bucketArray[hashValue];

	while (pair && !table->keyCompare(key, pair->key))
	{
		previousPair = pair;
		pair = pair->next;
	}

	if (!pair)
	{
		status = false;
	}
	else
	{
		if (table->keyFree)
			table->keyFree(pair->key);

		if (table->valueFree)
			table->valueFree(pair->value);

		if (previousPair)
			previousPair->next = pair->next;
		else
			table->bucketArray[hashValue] = pair->next;

		free(pair);

		table->numOfElements--;

		if (table->lowerRehashThreshold > 0.0)
		{
			float elementToBucketRatio = (float) table->numOfElements / (float) table->numOfBuckets;

			if (elementToBucketRatio < table->lowerRehashThreshold)
				MsRdpEx_HashTable_Rehash(table, 0);
		}
	}

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	return status;
}

void* MsRdpEx_HashTable_GetItemValue(MsRdpEx_HashTable* table, void* key)
{
	void* value = NULL;
	MsRdpEx_KeyValuePair* pair;

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	pair = MsRdpEx_HashTable_Get(table, key);

	if (pair)
		value = pair->value;

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	return value;
}

bool MsRdpEx_HashTable_SetItemValue(MsRdpEx_HashTable* table, void* key, void* value)
{
	bool status = true;
	MsRdpEx_KeyValuePair* pair;

	if (table->valueClone)
	{
		if (!table->acceptNullValues && !value)
			return false;

		value = table->valueClone(value);

		if (!table->acceptNullValues && !value)
			return false;
	}

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	pair = MsRdpEx_HashTable_Get(table, key);

	if (!pair)
		status = false;
	else
		pair->value = value;

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	return status;
}

void MsRdpEx_HashTable_Clear(MsRdpEx_HashTable* table)
{
	int index;
	MsRdpEx_KeyValuePair* pair;
	MsRdpEx_KeyValuePair* nextPair;

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	for (index = 0; index < table->numOfBuckets; index++)
	{
		pair = table->bucketArray[index];

		while (pair)
		{
			nextPair = pair->next;

			if (table->keyFree)
				table->keyFree(pair->key);

			if (table->valueFree)
				table->valueFree(pair->value);

			free(pair);

			pair = nextPair;
		}

		table->bucketArray[index] = NULL;
	}

	table->numOfElements = 0;
	MsRdpEx_HashTable_Rehash(table, 5);

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);
}

int MsRdpEx_HashTable_GetKeys(MsRdpEx_HashTable* table, ULONG_PTR** ppKeys)
{
	int iKey;
	int count;
	int index;
	ULONG_PTR* pKeys;
	MsRdpEx_KeyValuePair* pair;
	MsRdpEx_KeyValuePair* nextPair;

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	iKey = 0;
	count = table->numOfElements;

	if (count < 1)
	{
		if (table->synchronized)
			LeaveCriticalSection(&table->lock);

		return 0;
	}

	pKeys = (ULONG_PTR*) calloc(count, sizeof(ULONG_PTR));

	if (!pKeys)
	{
		if (table->synchronized)
			LeaveCriticalSection(&table->lock);

		return -1;
	}

	for (index = 0; index < table->numOfBuckets; index++)
	{
		pair = table->bucketArray[index];

		while (pair)
		{
			nextPair = pair->next;

			pKeys[iKey++] = (ULONG_PTR) pair->key;

			pair = nextPair;
		}
	}

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	*ppKeys = pKeys;

	return count;
}

bool MsRdpEx_HashTable_ContainsKey(MsRdpEx_HashTable* table, void* key)
{
	bool status;

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	status = (MsRdpEx_HashTable_Get(table, key) != NULL) ? true : false;

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	return status;
}

bool MsRdpEx_HashTable_ContainsValue(MsRdpEx_HashTable* table, void* value)
{
	int index;
	bool status = false;
	MsRdpEx_KeyValuePair* pair;

	if (table->synchronized)
		EnterCriticalSection(&table->lock);

	for (index = 0; index < table->numOfBuckets; index++)
	{
		pair = table->bucketArray[index];

		while (pair)
		{
			if (table->valueCompare(value, pair->value))
			{
				status = true;
				break;
			}

			pair = pair->next;
		}

		if (status)
			break;
	}

	if (table->synchronized)
		LeaveCriticalSection(&table->lock);

	return status;
}

MsRdpEx_HashTable* MsRdpEx_HashTable_New(bool synchronized, uint8_t flags)
{
	MsRdpEx_HashTable* table;

	table = (MsRdpEx_HashTable*) calloc(1, sizeof(MsRdpEx_HashTable));

	if (!table)
		return NULL;

	table->synchronized = synchronized;
	table->acceptNullKeys = flags & MSRDPEX_HASHTABLE_FLAGS_ACCEPT_NULL_KEYS ? true : false;
	table->acceptNullValues = flags & MSRDPEX_HASHTABLE_FLAGS_ACCEPT_NULL_VALUES ? true : false;

	InitializeCriticalSectionAndSpinCount(&(table->lock), 4000);

	table->numOfBuckets = 64;
	table->numOfElements = 0;

	table->bucketArray = (MsRdpEx_KeyValuePair**) calloc(table->numOfBuckets, sizeof(MsRdpEx_KeyValuePair*));

	if (!table->bucketArray)
	{
		free(table);
		return NULL;
	}

	table->idealRatio = 3.0;
	table->lowerRehashThreshold = 0.0;
	table->upperRehashThreshold = 15.0;

	table->hash = MsRdpEx_HashTable_PointerHash;
	table->keyCompare = MsRdpEx_HashTable_PointerCompare;
	table->valueCompare = MsRdpEx_HashTable_PointerCompare;
	table->keyClone = NULL;
	table->valueClone = NULL;
	table->keyFree = NULL;
	table->valueFree = NULL;

	return table;
}

void MsRdpEx_HashTable_Free(MsRdpEx_HashTable* table)
{
	int index;
	MsRdpEx_KeyValuePair* pair;
	MsRdpEx_KeyValuePair* nextPair;

	if (!table)
		return;

	for (index = 0; index < table->numOfBuckets; index++)
	{
		pair = table->bucketArray[index];

		while (pair)
		{
			nextPair = pair->next;

			if (table->keyFree)
				table->keyFree(pair->key);

			if (table->valueFree)
				table->valueFree(pair->value);

			free(pair);

			pair = nextPair;
		}
	}

	DeleteCriticalSection(&(table->lock));

	free(table->bucketArray);
	free(table);
}