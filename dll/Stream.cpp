
#include <MsRdpEx/Stream.h>

bool MsRdpEx_Stream_Find(MsRdpEx_Stream* s, const uint8_t* pattern, size_t pattern_length, size_t* offset)
{
	MsRdpEx_Stream* _s = NULL;
	bool result = false;

	if (MsRdpEx_Stream_GetRemainingLength(s) < pattern_length)
		return false;

	_s = MsRdpEx_Stream_New(s->pointer, MsRdpEx_Stream_GetRemainingLength(s), false);

	while (MsRdpEx_Stream_CheckSafeRead(_s, pattern_length))
	{
		if (memcmp(_s->pointer, pattern, pattern_length) == 0)
		{
			result = true;
			*offset = _s->pointer - s->pointer;
			goto exit;
		}

		MsRdpEx_StreamSeek(_s, 1);
	}

exit:
	MsRdpEx_Stream_Free(_s);

	return result;
}

bool MsRdpEx_Stream_EnsureCapacity(MsRdpEx_Stream* s, size_t size)
{
	if (!s->capacity)
		return false;

	if (s->capacity < size)
	{
		uint8_t* new_buf;
		size_t position;
		size_t old_capacity;
		size_t new_capacity;

		old_capacity = s->capacity;
		new_capacity = old_capacity;

		do
		{
			new_capacity *= 2;
		}
		while (new_capacity < size);

		position = MsRdpEx_Stream_GetPosition(s);

		if (s->owner)
		{
			new_buf = (uint8_t*) realloc(s->buffer, new_capacity);

			if (!new_buf)
				return false;
		}
		else
		{
			new_buf = (uint8_t*) malloc(new_capacity);

			if (!new_buf)
				return false;

			CopyMemory(new_buf, s->buffer, old_capacity);

			s->owner = true;
		}

		s->buffer = new_buf;
		s->capacity = new_capacity;
		s->length = new_capacity;
		ZeroMemory(&s->buffer[old_capacity], s->capacity - old_capacity);

		MsRdpEx_Stream_SetPosition(s, position);
	}

	return true;
}

bool MsRdpEx_Stream_EnsureRemainingCapacity(MsRdpEx_Stream* s, size_t size)
{
	if ((MsRdpEx_Stream_GetPosition(s) + size) > MsRdpEx_Stream_Capacity(s))
		return MsRdpEx_Stream_EnsureCapacity(s, MsRdpEx_Stream_Capacity(s) + size);

	return true;
}

bool MsRdpEx_Stream_CheckSafeRead(MsRdpEx_Stream* s, size_t size)
{
	size_t offset;

	if (s->buffer > s->pointer)
		return false;

	offset = s->pointer - s->buffer;

	if (s->length < offset)
		return false;

	int remaining_length = s->length - (s->pointer - s->buffer);

	if (remaining_length < size)
		return false;

	return true;
}

bool MsRdpEx_Stream_CheckSafeWrite(MsRdpEx_Stream* s, size_t size)
{
	if ((MsRdpEx_Stream_GetPosition(s) + size) > MsRdpEx_Stream_Capacity(s))
	{
		return MsRdpEx_Stream_EnsureCapacity(s, MsRdpEx_Stream_Capacity(s) + size);
	}

	return true;
}

int MsRdpEx_Stream_Format(MsRdpEx_Stream* s, const char* format, ...)
{
	int status;
	size_t size;
	va_list args;

	/* vsnprintf: http://www.cplusplus.com/reference/cstdio/vsnprintf/ */

	va_start(args, format);
	size = s->capacity - MsRdpEx_Stream_GetPosition(s);
	status = vsnprintf((char*) s->pointer, size, format, args);
	va_end(args);

	if (status < 0)
		return -1;

	if (status < size)
	{
		MsRdpEx_StreamSeek(s, (size_t) status);
	}
	else
	{
		if (!MsRdpEx_Stream_CheckSafeWrite(s, status + 1))
			return -1;

		va_start(args, format);
		size = s->capacity - MsRdpEx_Stream_GetPosition(s);
		status = vsnprintf((char*) s->pointer, size, format, args);
		va_end(args);

		if ((status < 0) || (status >= size))
			return -1;

		MsRdpEx_StreamSeek(s, (size_t) status);
	}

	return status;
}

MsRdpEx_Stream* MsRdpEx_Stream_Init(MsRdpEx_Stream* s, uint8_t* buffer, size_t size, bool owner)
{
	if (!s)
		return NULL;

	s->owner = owner;

	if (buffer)
	{
		s->buffer = buffer;
	}
	else
	{
		s->buffer = (uint8_t*) malloc(size);
		s->owner = true;
	}

	if (!s->buffer)
		return NULL;

	s->pointer = s->buffer;
	s->capacity = size;
	s->length = size;

	return s;
}

void MsRdpEx_Stream_Uninit(MsRdpEx_Stream* s)
{
	if (!s)
		return;

	if (s->owner)
	{
		free(s->buffer);
		s->buffer = NULL;
	}

	s->owner = false;
	s->pointer = NULL;
	s->capacity = 0;
	s->length = 0;
}

MsRdpEx_Stream* MsRdpEx_Stream_New(uint8_t* buffer, size_t size, bool owner)
{
	MsRdpEx_Stream* s;

	s = (MsRdpEx_Stream*) malloc(sizeof(MsRdpEx_Stream));

	if (!s)
		return NULL;

	if (!MsRdpEx_Stream_Init(s, buffer, size, owner))
	{
		free(s);
		return NULL;
	}

	return s;
}

void MsRdpEx_Stream_Free(MsRdpEx_Stream* s)
{
	if (!s)
		return;

	MsRdpEx_Stream_Uninit(s);

	free(s);
}
