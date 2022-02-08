#ifndef MSRDPEX_STREAM_H
#define MSRDPEX_STREAM_H

#include <MsRdpEx/MsRdpEx.h>

#ifdef __cplusplus
extern "C" {
#endif

struct _MsRdpEx_Stream
{
	uint8_t* buffer;
	uint8_t* pointer;
	size_t length;
	size_t capacity;
	bool owner;
};
typedef struct _MsRdpEx_Stream MsRdpEx_Stream;

#define msrdpex_stream_read_n8(_t, _s, _v, _p) do { _v = \
	(_t)(*_s->pointer); \
	_s->pointer += _p; } while (0)

#define msrdpex_stream_read_n16_le(_t, _s, _v, _p) do { _v = \
	(_t)(*_s->pointer) + \
	(((_t)(*(_s->pointer + 1))) << 8); \
	if (_p) _s->pointer += 2; } while (0)

#define msrdpex_stream_read_n16_be(_t, _s, _v, _p) do { _v = \
	(((_t)(*_s->pointer)) << 8) + \
	(_t)(*(_s->pointer + 1)); \
	if (_p) _s->pointer += 2; } while (0)

#define msrdpex_stream_read_n32_le(_t, _s, _v, _p) do { _v = \
	(_t)(*_s->pointer) + \
	(((_t)(*(_s->pointer + 1))) << 8) + \
	(((_t)(*(_s->pointer + 2))) << 16) + \
	(((_t)(*(_s->pointer + 3))) << 24); \
	if (_p) _s->pointer += 4; } while (0)

#define msrdpex_stream_read_n32_be(_t, _s, _v, _p) do { _v = \
	(((_t)(*(_s->pointer))) << 24) + \
	(((_t)(*(_s->pointer + 1))) << 16) + \
	(((_t)(*(_s->pointer + 2))) << 8) + \
	(((_t)(*(_s->pointer + 3)))); \
	if (_p) _s->pointer += 4; } while (0)

#define msrdpex_stream_read_n64_le(_t, _s, _v, _p) do { _v = \
	(_t)(*_s->pointer) + \
	(((_t)(*(_s->pointer + 1))) << 8) + \
	(((_t)(*(_s->pointer + 2))) << 16) + \
	(((_t)(*(_s->pointer + 3))) << 24) + \
	(((_t)(*(_s->pointer + 4))) << 32) + \
	(((_t)(*(_s->pointer + 5))) << 40) + \
	(((_t)(*(_s->pointer + 6))) << 48) + \
	(((_t)(*(_s->pointer + 7))) << 56); \
	if (_p) _s->pointer += 8; } while (0)

#define msrdpex_stream_read_n64_be(_t, _s, _v, _p) do { _v = \
	(((_t)(*(_s->pointer))) << 56) + \
	(((_t)(*(_s->pointer + 1))) << 48) + \
	(((_t)(*(_s->pointer + 2))) << 40) + \
	(((_t)(*(_s->pointer + 3))) << 32) + \
	(((_t)(*(_s->pointer + 4))) << 24) + \
	(((_t)(*(_s->pointer + 5))) << 16) + \
	(((_t)(*(_s->pointer + 6))) << 8) + \
	(((_t)(*(_s->pointer + 7)))); \
	if (_p) _s->pointer += 8; } while (0)


#define MsRdpEx_StreamRead_UINT8(_s, _v) msrdpex_stream_read_n8(UINT8, _s, _v, 1)
#define MsRdpEx_StreamRead_INT8(_s, _v) msrdpex_stream_read_n8(INT8, _s, _v, 1)

#define MsRdpEx_StreamRead_UINT16(_s, _v) msrdpex_stream_read_n16_le(UINT16, _s, _v, 1)
#define MsRdpEx_StreamRead_INT16(_s, _v) msrdpex_stream_read_n16_le(INT16, _s, _v, 1)

#define MsRdpEx_StreamRead_UINT16_BE(_s, _v) msrdpex_stream_read_n16_be(UINT16, _s, _v, 1)
#define MsRdpEx_StreamRead_INT16_BE(_s, _v) msrdpex_stream_read_n16_be(INT16, _s, _v, 1)

#define MsRdpEx_StreamRead_UINT32(_s, _v) msrdpex_stream_read_n32_le(UINT32, _s, _v, 1)
#define MsRdpEx_StreamRead_INT32(_s, _v) msrdpex_stream_read_n32_le(INT32, _s, _v, 1)

#define MsRdpEx_StreamRead_UINT32_BE(_s, _v) msrdpex_stream_read_n32_be(UINT32, _s, _v, 1)
#define MsRdpEx_StreamRead_INT32_BE(_s, _v) msrdpex_stream_read_n32_be(INT32, _s, _v, 1)

#define MsRdpEx_StreamRead_UINT64(_s, _v) msrdpex_stream_read_n64_le(UINT64, _s, _v, 1)
#define MsRdpEx_StreamRead_INT64(_s, _v) msrdpex_stream_read_n64_le(INT64, _s, _v, 1)

#define MsRdpEx_StreamRead_UINT64_BE(_s, _v) msrdpex_stream_read_n64_be(UINT64, _s, _v, 1)
#define MsRdpEx_StreamRead_INT64_BE(_s, _v) msrdpex_stream_read_n64_be(INT64, _s, _v, 1)

#define MsRdpEx_StreamRead(_s, _b, _n) do { \
	memcpy(_b, (_s->pointer), (_n)); \
	_s->pointer += (_n); \
	} while (0)


#define MsRdpEx_StreamPeek_UINT8(_s, _v) msrdpex_stream_read_n8(UINT8, _s, _v, 0)
#define MsRdpEx_StreamPeek_INT8(_s, _v) msrdpex_stream_read_n8(INT8, _s, _v, 0)

#define MsRdpEx_StreamPeek_UINT16(_s, _v) msrdpex_stream_read_n16_le(UINT16, _s, _v, 0)
#define MsRdpEx_StreamPeek_INT16(_s, _v) msrdpex_stream_read_n16_le(INT16, _s, _v, 0)

#define MsRdpEx_StreamPeek_UINT16_BE(_s, _v) msrdpex_stream_read_n16_be(UINT16, _s, _v, 0)
#define MsRdpEx_StreamPeek_INT16_BE(_s, _v) msrdpex_stream_read_n16_be(INT16, _s, _v, 0)

#define MsRdpEx_StreamPeek_UINT32(_s, _v) msrdpex_stream_read_n32_le(UINT32, _s, _v, 0)
#define MsRdpEx_StreamPeek_INT32(_s, _v) msrdpex_stream_read_n32_le(INT32, _s, _v, 0)

#define MsRdpEx_StreamPeek_UINT32_BE(_s, _v) msrdpex_stream_read_n32_be(UINT32, _s, _v, 0)
#define MsRdpEx_StreamPeek_INT32_BE(_s, _v) msrdpex_stream_read_n32_be(INT32, _s, _v, 0)

#define MsRdpEx_StreamPeek_UINT64(_s, _v) msrdpex_stream_read_n64_le(UINT64, _s, _v, 0)
#define MsRdpEx_StreamPeek_INT64(_s, _v) msrdpex_stream_read_n64_le(INT64, _s, _v, 0)

#define MsRdpEx_StreamPeek_UINT64_BE(_s, _v) msrdpex_stream_read_n64_be(UINT64, _s, _v, 0)
#define MsRdpEx_StreamPeek_INT64_BE(_s, _v) msrdpex_stream_read_n64_be(INT64, _s, _v, 0)

#define MsRdpEx_StreamPeek(_s, _b, _n) do { \
	memcpy(_b, (_s->pointer), (_n)); \
	} while (0)


#define MsRdpEx_StreamWrite_UINT8(_s, _v) do { \
	*_s->pointer++ = (UINT8)(_v); } while (0)
#define MsRdpEx_StreamWrite_INT8 MsRdpEx_StreamWrite_UINT8

#define MsRdpEx_StreamWrite_UINT16(_s, _v) do { \
	*_s->pointer++ = (_v) & 0xFF; \
	*_s->pointer++ = ((_v) >> 8) & 0xFF; } while (0)
#define MsRdpEx_StreamWrite_INT16 MsRdpEx_StreamWrite_UINT16

#define MsRdpEx_StreamWrite_UINT16_BE(_s, _v) do { \
	*_s->pointer++ = ((_v) >> 8) & 0xFF; \
	*_s->pointer++ = (_v) & 0xFF; } while (0)
#define MsRdpEx_StreamWrite_INT16_BE MsRdpEx_StreamWrite_UINT16_BE

#define MsRdpEx_StreamWrite_UINT32(_s, _v) do { \
	*_s->pointer++ = (_v) & 0xFF; \
	*_s->pointer++ = ((_v) >> 8) & 0xFF; \
	*_s->pointer++ = ((_v) >> 16) & 0xFF; \
	*_s->pointer++ = ((_v) >> 24) & 0xFF; } while (0)
#define MsRdpEx_StreamWrite_INT32 MsRdpEx_StreamWrite_UINT32

#define MsRdpEx_StreamWrite_UINT32_BE(_s, _v) do { \
		MsRdpEx_StreamWrite_UINT16_BE(_s, ((_v) >> 16 & 0xFFFF)); \
		MsRdpEx_StreamWrite_UINT16_BE(_s, ((_v) & 0xFFFF)); \
	} while (0)
#define MsRdpEx_StreamWrite_INT32_BE MsRdpEx_StreamWrite_UINT32_BE

#define MsRdpEx_StreamWrite_UINT64(_s, _v) do { \
	*_s->pointer++ = (UINT64)(_v) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 8) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 16) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 24) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 32) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 40) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 48) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 56) & 0xFF; } while (0)
#define MsRdpEx_StreamWrite_INT64 MsRdpEx_StreamWrite_UINT64

#define MsRdpEx_StreamWrite_UINT64_BE(_s, _v) do { \
	*_s->pointer++ = ((UINT64)(_v) >> 56) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 48) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 40) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 32) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 24) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 16) & 0xFF; \
	*_s->pointer++ = ((UINT64)(_v) >> 8) & 0xFF; \
	*_s->pointer++ = (UINT64)(_v) & 0xFF; } while (0)
#define MsRdpEx_StreamWrite_INT64_BE MsRdpEx_StreamWrite_UINT64_BE
	
#define MsRdpEx_StreamWrite(_s, _b, _n) do { \
	memcpy(_s->pointer, (_b), (_n)); \
	_s->pointer += (_n); \
	} while (0)


#define MsRdpEx_StreamSeek(_s,_offset)		_s->pointer += (_offset)
#define MsRdpEx_StreamRewind(_s,_offset)		_s->pointer -= (_offset)

#define MsRdpEx_StreamSeek_UINT8(_s)		MsRdpEx_StreamSeek(_s, 1)
#define MsRdpEx_StreamSeek_UINT16(_s)		MsRdpEx_StreamSeek(_s, 2)
#define MsRdpEx_StreamSeek_UINT32(_s)		MsRdpEx_StreamSeek(_s, 4)
#define MsRdpEx_StreamSeek_UINT64(_s)		MsRdpEx_StreamSeek(_s, 8)

#define MsRdpEx_StreamRewind_UINT8(_s)		MsRdpEx_StreamRewind(_s, 1)
#define MsRdpEx_StreamRewind_UINT16(_s)		MsRdpEx_StreamRewind(_s, 2)
#define MsRdpEx_StreamRewind_UINT32(_s)		MsRdpEx_StreamRewind(_s, 4)
#define MsRdpEx_StreamRewind_UINT64(_s)		MsRdpEx_StreamRewind(_s, 8)

#define MsRdpEx_StreamZero(_s, _n) do { \
	memset(_s->pointer, 0, (_n)); \
	_s->pointer += (_n); \
	} while (0)

#define MsRdpEx_StreamFill(_s, _v, _n) do { \
	memset(_s->pointer, _v, (_n)); \
	_s->pointer += (_n); \
	} while (0)

#define MsRdpEx_StreamCopy(_dst, _src, _n) do { \
	memcpy(_dst->pointer, _src->pointer, _n); \
	_dst->pointer += _n; \
	_src->pointer += _n; \
	} while (0)

#define MsRdpEx_Stream_Buffer(_s)			_s->buffer
#define MsRdpEx_Stream_GetBuffer(_s, _b)		_b = _s->buffer
#define MsRdpEx_Stream_SetBuffer(_s, _b)		_s->buffer = _b

#define MsRdpEx_Stream_Pointer(_s)			_s->pointer
#define MsRdpEx_Stream_GetPointer(_s, _p)		_p = _s->pointer
#define MsRdpEx_Stream_SetPointer(_s, _p)		_s->pointer = _p

#define MsRdpEx_Stream_Length(_s)			_s->length
#define MsRdpEx_Stream_GetLength(_s, _l)		_l = _s->length
#define MsRdpEx_Stream_SetLength(_s, _l)		_s->length = _l

#define MsRdpEx_Stream_Capacity(_s)			_s->capacity
#define MsRdpEx_Stream_GetCapacity(_s, _c)		_c = _s->capacity
#define MsRdpEx_Stream_SetCapacity(_s, _c)		_s->capacity = _c

#define MsRdpEx_Stream_GetPosition(_s)		(_s->pointer - _s->buffer)
#define MsRdpEx_Stream_SetPosition(_s, _p)		_s->pointer = _s->buffer + (_p)

#define MsRdpEx_Stream_SealLength(_s)		_s->length = (_s->pointer - _s->buffer)
#define MsRdpEx_Stream_GetRemainingLength(_s)	(_s->length - (_s->pointer - _s->buffer))
#define MsRdpEx_Stream_GetRemainingCapacity(_s)	(_s->capacity - (_s->pointer - _s->buffer))

#define MsRdpEx_Stream_Clear(_s)			memset(_s->buffer, 0, _s->capacity)

static inline bool MsRdpEx_Stream_SafeSeek(MsRdpEx_Stream* s, size_t size)
{
	if (MsRdpEx_Stream_GetRemainingLength(s) < size)
		return false;

	MsRdpEx_StreamSeek(s, size);
	return true;
}

bool MsRdpEx_Stream_EnsureCapacity(MsRdpEx_Stream* s, size_t size);
bool MsRdpEx_Stream_EnsureRemainingCapacity(MsRdpEx_Stream* s, size_t size);

bool MsRdpEx_Stream_CheckSafeRead(MsRdpEx_Stream* s, size_t size);
bool MsRdpEx_Stream_CheckSafeWrite(MsRdpEx_Stream* s, size_t size);

int MsRdpEx_Stream_Format(MsRdpEx_Stream* s, const char* format, ...);

/**
 * Search for |pattern| from the current position in the stream. If pattern is found, |offset|
 * contains the offset from the current position where the |pattern| begins.
 */
bool MsRdpEx_Stream_Find(MsRdpEx_Stream* s, const uint8_t* pattern, size_t pattern_length, size_t* offset);

MsRdpEx_Stream* MsRdpEx_Stream_Init(MsRdpEx_Stream* s, uint8_t* buffer, size_t size, bool owner);
void MsRdpEx_Stream_Uninit(MsRdpEx_Stream* s);

MsRdpEx_Stream* MsRdpEx_Stream_New(uint8_t* buffer, size_t size, bool owner);
void MsRdpEx_Stream_Free(MsRdpEx_Stream* s);

#ifdef __cplusplus
}
#endif

#endif /* MSRDPEX_STREAM_H */
