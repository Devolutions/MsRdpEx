
#include <MsRdpEx/ArrayList.h>

struct msrdpex_array_list_it
{
	int index;
	int count;
	void** array;
	uint32_t flags;
	MsRdpEx_ArrayList* ctx;
};

struct msrdpex_array_list
{
	int capacity;
	int growthFactor;
	bool synchronized;

	int count;
	void** array;
	CRITICAL_SECTION lock;

	MsRdpEx_Object object;
	MsRdpEx_ArrayListIt it;
};

/**
 * Returns the number of elements in the iterator.
 */

int MsRdpEx_ArrayListIt_Count(MsRdpEx_ArrayListIt* it)
{
	return it->count;
}

/**
 * Set the iterator position to the beginning.
 */

void MsRdpEx_ArrayListIt_Reset(MsRdpEx_ArrayListIt* it)
{
	if (!it)
		return;

	if (it->flags & MSRDPEX_ITERATOR_FLAG_REVERSE)
		it->index = (it->count > 1) ? it->count - 1 : 0;
	else
		it->index = 0;
}

/**
 * Returns the current element in the iterator.
 */

void* MsRdpEx_ArrayListIt_Current(MsRdpEx_ArrayListIt* it)
{
	if (!it)
		return NULL;
	
	return it->array[it->index];
}

/**
 * Advance the iterator position.
 */

bool MsRdpEx_ArrayListIt_Move(MsRdpEx_ArrayListIt* it)
{
	if (!it)
		return false;

	if (it->flags & MSRDPEX_ITERATOR_FLAG_REVERSE)
	{
		if (it->index > 0)
		{
			it->index--;
			return true;
		}
	}
	else
	{
		if (it->index < it->count)
		{
			it->index++;
			return true;
		}
	}

	return false;
}

/**
 * Returns the current element and advance the iterator position.
 */

void* MsRdpEx_ArrayListIt_Next(MsRdpEx_ArrayListIt* it)
{
	if (!it)
		return NULL;

	if (it->flags & MSRDPEX_ITERATOR_FLAG_REVERSE)
	{
		if (it->index > 0)
			return it->array[it->index--];
	}
	else
	{
		if (it->index < it->count)
			return it->array[it->index++];
	}

	return NULL;
}

/**
 * Returns true if the iterator is at the end of the list.
 */

bool MsRdpEx_ArrayListIt_Done(MsRdpEx_ArrayListIt* it)
{
	bool done = false;

	if (!it)
		return false;

	if (it->flags & MSRDPEX_ITERATOR_FLAG_REVERSE)
		done = (it->index > 0) ? false : true;
	else
		done = (it->index < it->count) ? false : true;

	return done;
}

/**
 * Returns an iterator for the items contained in the ArrayList.
 */

MsRdpEx_ArrayListIt* MsRdpEx_ArrayList_It(MsRdpEx_ArrayList* ctx, uint32_t flags)
{
	size_t size;
	MsRdpEx_ArrayListIt* it;

	if (!(flags & MSRDPEX_ITERATOR_FLAG_EXCLUSIVE))
		flags |= MSRDPEX_ITERATOR_FLAG_DUPLICATE; /* non-exclusive implies duplicate */

	if ((flags & MSRDPEX_ITERATOR_FLAG_EXCLUSIVE) &&
		!(flags & MSRDPEX_ITERATOR_FLAG_DUPLICATE))
	{
		/* exclusive and non-duplicate makes no memory allocation */

		if (ctx->synchronized)
			EnterCriticalSection(&ctx->lock);

		it = &ctx->it;

		it->ctx = ctx;
		it->flags = flags;

		it->count = ctx->count;
		it->array = ctx->array;

		MsRdpEx_ArrayListIt_Reset(it);

		return it;
	}

	/* duplicate */

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	size = sizeof(MsRdpEx_ArrayListIt) + (ctx->count * sizeof(void*));

	it = (MsRdpEx_ArrayListIt*) calloc(1, size);

	if (!it)
	{
		if (ctx->synchronized)
			LeaveCriticalSection(&ctx->lock);

		return NULL;
	}

	it->ctx = ctx;
	it->flags = flags;

	it->count = ctx->count;
	it->array = (void**) &((uint8_t*) it)[sizeof(MsRdpEx_ArrayListIt)];
	CopyMemory(it->array, ctx->array, (ctx->count * sizeof(void*)));

	MsRdpEx_ArrayListIt_Reset(it);

	if (!(flags & MSRDPEX_ITERATOR_FLAG_EXCLUSIVE))
	{
		/* exclusive does not release lock until iterator finish */

		if (ctx->synchronized)
			LeaveCriticalSection(&ctx->lock);
	}

	return it;
}

/**
 * Needs to be called when done iterating to cleanup resources.
 */

void MsRdpEx_ArrayListIt_Finish(MsRdpEx_ArrayListIt* it)
{
	uint32_t flags;
	MsRdpEx_ArrayList* ctx;

	if (!it)
		return;

	ctx = it->ctx;
	flags = it->flags;

	if (flags & MSRDPEX_ITERATOR_FLAG_EXCLUSIVE)
	{
		if (!ctx)
			return;

		if (ctx->synchronized)
			LeaveCriticalSection(&ctx->lock);

		if (!(flags & MSRDPEX_ITERATOR_FLAG_DUPLICATE))
			return; /* exclusive and non-duplicate makes no memory allocation */
	}

	free(it); /* a single free is required since we pack the iterator + array in the same block */
}

/**
 * Properties
 */

/**
 * Gets or sets the number of elements that the ArrayList can contain.
 */

int MsRdpEx_ArrayList_Capacity(MsRdpEx_ArrayList* ctx)
{
	return ctx->capacity;
}

/**
 * Gets the number of elements actually contained in the ArrayList.
 */

int MsRdpEx_ArrayList_Count(MsRdpEx_ArrayList* ctx)
{
	return ctx->count;
}

/**
 * Check if the list is empty
 */

bool MsRdpEx_ArrayList_IsEmpty(MsRdpEx_ArrayList* ctx)
{
	return (ctx->count < 1);
}

/**
 * Gets the internal list of items contained in the ArrayList.
 */

int MsRdpEx_ArrayList_Items(MsRdpEx_ArrayList* ctx, uintptr_t** ppItems)
{
	*ppItems = (uintptr_t*) ctx->array;
	return ctx->count;
}

/**
 * Gets a value indicating whether access to the ArrayList is synchronized (thread safe).
 */

bool MsRdpEx_ArrayList_IsSynchronized(MsRdpEx_ArrayList* ctx)
{
	return ctx->synchronized;
}

/**
 * Lock access to the ArrayList
 */

void MsRdpEx_ArrayList_Lock(MsRdpEx_ArrayList* ctx)
{
	EnterCriticalSection(&ctx->lock);
}

/**
 * Unlock access to the ArrayList
 */

void MsRdpEx_ArrayList_Unlock(MsRdpEx_ArrayList* ctx)
{
	LeaveCriticalSection(&ctx->lock);
}

/**
 * Gets the element at the specified index.
 */

void* MsRdpEx_ArrayList_GetItem(MsRdpEx_ArrayList* ctx, int index)
{
	void* obj = NULL;

	if ((index >= 0) && (index < ctx->count))
	{
		obj = ctx->array[index];
	}

	return obj;
}

/**
 * Sets the element at the specified index.
 */

bool MsRdpEx_ArrayList_SetItem(MsRdpEx_ArrayList* ctx, int index, void* obj)
{
	if ((index >= 0) && (index < ctx->count))
	{
		ctx->array[index] = obj;
		return true;
	}

	return false;
}

/**
 * Methods
 */

 /**
 * Get head item.
 */

void* MsRdpEx_ArrayList_GetHead(MsRdpEx_ArrayList* ctx)
{
	int index;
	void* obj = NULL;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if (ctx->count > 1)
	{
		index = 0;
		obj = ctx->array[index];
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return obj;
}

/**
 * Get tail item.
 */

void* MsRdpEx_ArrayList_GetTail(MsRdpEx_ArrayList* ctx)
{
	int index;
	void* obj = NULL;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if (ctx->count > 1)
	{
		index = ctx->count - 1;
		obj = ctx->array[index];
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return obj;
}

/**
 * Find an item in the array list.
 */

void* MsRdpEx_ArrayList_Find(MsRdpEx_ArrayList* ctx, MSRDPEX_OBJECT_MATCH_FN fnMatch, void* param)
{
	void* obj = NULL;
	bool found = false;
	MsRdpEx_ArrayListIt* it = NULL;

	if (!fnMatch)
		return false;

	it = MsRdpEx_ArrayList_It(ctx, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

	while (!MsRdpEx_ArrayListIt_Done(it))
	{
		obj = MsRdpEx_ArrayListIt_Next(it);

		found = fnMatch(obj, param);

		if (found)
			break;
	}

	MsRdpEx_ArrayListIt_Finish(it);

	return found ? obj : NULL;
}

/**
 * Shift a section of the list.
 */

bool MsRdpEx_ArrayList_Shift(MsRdpEx_ArrayList* ctx, int index, int count)
{
	if (count > 0)
	{
		if (ctx->count + count > ctx->capacity)
		{
			void** newArray;
			int newCapacity = ctx->capacity * ctx->growthFactor;
			newArray = (void**) realloc(ctx->array, sizeof(void*) * newCapacity);

			if (!newArray)
				return false;

			ctx->array = newArray;
			ctx->capacity = newCapacity;
		}

		MoveMemory(&ctx->array[index + count], &ctx->array[index], (ctx->count - index) * sizeof(void*));
		ctx->count += count;
	}
	else if (count < 0)
	{
		int chunk = ctx->count - index + count;

		if (chunk > 0)
			MoveMemory(&ctx->array[index], &ctx->array[index - count], chunk * sizeof(void*));

		ctx->count += count;
	}

	return true;
}

MsRdpEx_Object* MsRdpEx_ArrayList_Object(MsRdpEx_ArrayList* ctx)
{
	return &ctx->object;
}

/**
 * Removes all elements from the ArrayList.
 */

void MsRdpEx_ArrayList_Clear(MsRdpEx_ArrayList* ctx, bool free)
{
	int index;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	for (index = 0; index < ctx->count; index++)
	{
		if (free && ctx->object.fnObjectFree)
			ctx->object.fnObjectFree(ctx->array[index]);

		ctx->array[index] = NULL;
	}

	ctx->count = 0;

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);
}

/**
 * Determines whether an element is in the ArrayList.
 */

bool MsRdpEx_ArrayList_Contains(MsRdpEx_ArrayList* ctx, void* obj)
{
	int index;
	bool ret = false;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	for (index = 0; index < ctx->count; index++)
	{
		ret = ctx->object.fnObjectEquals(ctx->array[index], obj);

		if (ret)
			break;
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return ret;
}

/**
 * Adds an object to the end of the ArrayList.
 */

int MsRdpEx_ArrayList_Add(MsRdpEx_ArrayList* ctx, void* obj)
{
	int index = -1;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if (ctx->count + 1 > ctx->capacity)
	{
		void** newArray;
		int newCapacity = ctx->capacity * ctx->growthFactor;
		newArray = (void**) realloc(ctx->array, sizeof(void*) * newCapacity);

		if (!newArray)
			goto out;

		ctx->array = newArray;
		ctx->capacity = newCapacity;
	}

	index = ctx->count++;
	ctx->array[index] = obj;
out:

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return index;
}

/*
 * Inserts an element into the ArrayList at the specified index.
 */

bool MsRdpEx_ArrayList_InsertAt(MsRdpEx_ArrayList* ctx, int index, void* obj)
{
	bool success = false;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if (index == ctx->count)
	{
		if (MsRdpEx_ArrayList_Add(ctx, obj) >= 0)
			success = true;
	}
	else
	{
		if ((index >= 0) && (index < ctx->count))
		{
			if (MsRdpEx_ArrayList_Shift(ctx, index, 1))
			{
				ctx->array[index] = obj;
				success = true;
			}
		}
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return success;
}

/*
 * Inserts an element into the ArrayList using the specified comparison function.
 */

int MsRdpEx_ArrayList_Insert(MsRdpEx_ArrayList* ctx, void* obj, MSRDPEX_OBJECT_COMPARE_FN fnCompare)
{
	int index = 0;
	void* curr = NULL;
	MsRdpEx_ArrayListIt* it = NULL;

	if (!fnCompare)
		return -1;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	it = MsRdpEx_ArrayList_It(ctx, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

	while (!MsRdpEx_ArrayListIt_Done(it))
	{
		curr = MsRdpEx_ArrayListIt_Next(it);

		if (fnCompare(obj, curr) < 0)
			break;

		index++;
	}

	MsRdpEx_ArrayListIt_Finish(it);

	if (!MsRdpEx_ArrayList_InsertAt(ctx, index, obj))
		index = -1;

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return index;
}

/**
 * Removes the first occurrence of a specific object from the ArrayList.
 */

void* MsRdpEx_ArrayList_Remove(MsRdpEx_ArrayList* ctx, void* obj, bool free)
{
	int index;
	bool found = false;
	void* ret = NULL;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	for (index = 0; index < ctx->count; index++)
	{
		if (ctx->object.fnObjectEquals(ctx->array[index], obj))
		{
			found = true;
			break;
		}
	}

	if (found)
	{
		ret = ctx->array[index];

		if (free && ctx->object.fnObjectFree)
			ctx->object.fnObjectFree(ctx->array[index]);

		MsRdpEx_ArrayList_Shift(ctx, index, -1);
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return ret;
}

/**
 * Removes the element at the specified index of the ArrayList.
 */

void* MsRdpEx_ArrayList_RemoveAt(MsRdpEx_ArrayList* ctx, int index, bool free)
{
	void* ret = NULL;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if ((index >= 0) && (index < ctx->count))
	{
		ret = ctx->array[index];

		if (free && ctx->object.fnObjectFree)
			ctx->object.fnObjectFree(ctx->array[index]);

		MsRdpEx_ArrayList_Shift(ctx, index, -1);
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return ret;
}

/**
 * Removes the head of the list.
 */

void* MsRdpEx_ArrayList_RemoveHead(MsRdpEx_ArrayList* ctx, bool free)
{
	int index = 0;
	void* obj = NULL;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if ((index >= 0) && (index < ctx->count))
	{
		obj = ctx->array[index];

		if (free && ctx->object.fnObjectFree)
			ctx->object.fnObjectFree(ctx->array[index]);

		MsRdpEx_ArrayList_Shift(ctx, index, -1);
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return obj;
}

/**
 * Removes the tail of the list.
 */

void* MsRdpEx_ArrayList_RemoveTail(MsRdpEx_ArrayList* ctx, bool free)
{
	int index = 0;
	void* obj = NULL;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	index = ctx->count - 1;

	if ((index >= 0) && (index < ctx->count))
	{
		obj = ctx->array[index];

		if (free && ctx->object.fnObjectFree)
			ctx->object.fnObjectFree(ctx->array[index]);

		MsRdpEx_ArrayList_Shift(ctx, index, -1);
	}

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return obj;
}

/**
 * Removes all elements that match a specific criteria from the list.
 */

int MsRdpEx_ArrayList_RemoveAll(MsRdpEx_ArrayList* ctx, MSRDPEX_OBJECT_MATCH_FN fnMatch, void* param, bool free)
{
	void* obj;
	bool match;
	int count = 0;
	MsRdpEx_ArrayListIt* it;

	if (!fnMatch)
		return false;

	it = MsRdpEx_ArrayList_It(ctx, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE | MSRDPEX_ITERATOR_FLAG_DUPLICATE);

	while (!MsRdpEx_ArrayListIt_Done(it))
	{
		obj = MsRdpEx_ArrayListIt_Next(it);

		match = fnMatch(obj, param);

		if (match)
		{
			MsRdpEx_ArrayList_Remove(ctx, obj, free);
			count++;
		}
	}

	MsRdpEx_ArrayListIt_Finish(it);

	return count;
}

/**
 * Removes all duplicate elements from the list.
 */

int MsRdpEx_ArrayList_RemoveDuplicates(MsRdpEx_ArrayList* ctx, MSRDPEX_OBJECT_EQUALS_FN fnEquals, bool free)
{
	bool found;
	void* obj1;
	void* obj2;
	int unique = 0;
	int duplicate = 0;
	MsRdpEx_ArrayListIt* it;

	if (ctx->count < 1)
		return 0;

	if (!fnEquals)
		fnEquals = ctx->object.fnObjectEquals;

	if (!fnEquals)
		return -1;

	/**
	 * The algorithm proceeds as follows:
	 * Insert each element of the original array in two arrays: unique or duplicate.
	 * We create a copy of the original array to iterate over it without interference,
	 * using the internal array as buffer for both the unique and duplicate arrays.
	 * Insertion in the unique array is done from the start of the array buffer,
	 * and insertion in the duplicate array is from the end of the array buffer.
	 * At the end, the internal array buffer should contain all unique elements
	 * at the beginning, and all duplicate elements at the end. We finish by
	 * removing all elements at the end to keep only the unique elements.
	 */

	it = MsRdpEx_ArrayList_It(ctx, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE | MSRDPEX_ITERATOR_FLAG_DUPLICATE);

	while (!MsRdpEx_ArrayListIt_Done(it))
	{
		obj1 = MsRdpEx_ArrayListIt_Next(it);

		/* search for current object in unique list */

		found = false;

		for (int k = 0; k < unique; k++)
		{
			obj2 = ctx->array[k];

			if (fnEquals(obj1, obj2))
			{
				found = true;
				break;
			}
		}

		if (!found)
			ctx->array[unique++] = obj1; /* add to unique list */
		else
			ctx->array[(ctx->count - 1) - duplicate++] = obj1; /* add to duplicate list */
	}

	while (ctx->count > unique)
	{
		/* remove duplicates from end of array list */

		obj1 = ctx->array[(ctx->count - 1)];
		ctx->count--;

		if (free && ctx->object.fnObjectFree)
			ctx->object.fnObjectFree(obj1);
	}

	MsRdpEx_ArrayListIt_Finish(it);

	return duplicate;
}

/**
 * Searches for the specified Object and returns the zero-based index of the first occurrence within the entire ArrayList.
 *
 * Searches for the specified Object and returns the zero-based index of the last occurrence within the range of elements
 * in the ArrayList that extends from the first element to the specified index.
 *
 * Searches for the specified Object and returns the zero-based index of the last occurrence within the range of elements
 * in the ArrayList that contains the specified number of elements and ends at the specified index.
 */

int MsRdpEx_ArrayList_IndexOf(MsRdpEx_ArrayList* ctx, void* obj, int startIndex, int count)
{
	int index;
	bool found = false;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if (startIndex < 0)
		startIndex = 0;

	if (count < 0)
		count = ctx->count;

	for (index = startIndex; index < startIndex + count; index++)
	{
		if (ctx->object.fnObjectEquals(ctx->array[index], obj))
		{
			found = true;
			break;
		}
	}

	if (!found)
		index = -1;

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return index;
}

/**
 * Searches for the specified Object and returns the zero-based index of the last occurrence within the entire ArrayList.
 *
 * Searches for the specified Object and returns the zero-based index of the last occurrence within the range of elements
 * in the ArrayList that extends from the first element to the specified index.
 *
 * Searches for the specified Object and returns the zero-based index of the last occurrence within the range of elements
 * in the ArrayList that contains the specified number of elements and ends at the specified index.
 */

int MsRdpEx_ArrayList_LastIndexOf(MsRdpEx_ArrayList* ctx, void* obj, int startIndex, int count)
{
	int index;
	bool found = false;

	if (ctx->synchronized)
		EnterCriticalSection(&ctx->lock);

	if (startIndex < 0)
		startIndex = 0;

	if (count < 0)
		count = ctx->count;

	for (index = startIndex + count - 1; index >= startIndex; index--)
	{
		if (ctx->object.fnObjectEquals(ctx->array[index], obj))
		{
			found = true;
			break;
		}
	}

	if (!found)
		index = -1;

	if (ctx->synchronized)
		LeaveCriticalSection(&ctx->lock);

	return index;
}

bool MsRdpEx_ArrayList_DefaultEquals(void* objA, void* objB)
{
	return objA == objB ? true : false;
}

/**
 * Construction, Destruction
 */

MsRdpEx_ArrayList* MsRdpEx_ArrayList_New(bool synchronized)
{
	MsRdpEx_ArrayList* ctx = NULL;

	ctx = (MsRdpEx_ArrayList*) calloc(1, sizeof(MsRdpEx_ArrayList));

	if (!ctx)
		return NULL;

	ctx->synchronized = synchronized;
	ctx->capacity = 32;
	ctx->growthFactor = 2;
	ctx->object.fnObjectEquals = MsRdpEx_ArrayList_DefaultEquals;
	ctx->array = (void**) calloc(ctx->capacity, sizeof(void*));

	if (!ctx->array)
		goto out_free;

	InitializeCriticalSectionAndSpinCount(&ctx->lock, 4000);
	return ctx;

out_free:
	free(ctx);
	return NULL;
}

void MsRdpEx_ArrayList_Free(MsRdpEx_ArrayList* ctx)
{
	if (!ctx)
		return;

	MsRdpEx_ArrayList_Clear(ctx, true);
	DeleteCriticalSection(&ctx->lock);
	free(ctx->array);
	free(ctx);
}
