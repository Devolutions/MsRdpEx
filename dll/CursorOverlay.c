#include "MsRdpEx.h"
#include "CursorOverlay.h"

struct _MsRdpEx_CursorOverlay
{
	HCURSOR sourceCursor;
	HCURSOR cursor;
	int32_t hotX;
	int32_t hotY;
	int32_t width;
	int32_t height;
	int32_t inputX;
	int32_t inputY;
	bool visible;
	ULONGLONG lastEmitTick;

	HDC saveDC;
	HBITMAP saveBitmap;
	HGDIOBJ saveOldObject;
	int32_t saveWidth;
	int32_t saveHeight;

	// Composite() draws the cursor and stashes the region it overwrote here; Restore() puts it back.
	// They run at separate times (e.g. RDM reads the shadow between LockShadowBitmap/UnlockShadowBitmap),
	// so the pending-restore state has to survive between the two calls.
	bool composited;
	int32_t restoreLeft;
	int32_t restoreTop;
	int32_t restoreWidth;
	int32_t restoreHeight;

	CRITICAL_SECTION lock;
};

static void MsRdpEx_CursorOverlay_FreeSaveSurface(MsRdpEx_CursorOverlay* ctx)
{
	if (!ctx->saveDC)
		return;

	SelectObject(ctx->saveDC, ctx->saveOldObject);
	DeleteObject(ctx->saveBitmap);
	DeleteDC(ctx->saveDC);

	ctx->saveDC = NULL;
	ctx->saveBitmap = NULL;
	ctx->saveOldObject = NULL;
	ctx->saveWidth = 0;
	ctx->saveHeight = 0;
}

static bool MsRdpEx_CursorOverlay_EnsureSaveSurface(
	MsRdpEx_CursorOverlay* ctx, HDC shadowDC, int32_t width, int32_t height)
{
	if (ctx->saveDC && (width <= ctx->saveWidth) && (height <= ctx->saveHeight))
		return true;

	HDC saveDC = CreateCompatibleDC(shadowDC);
	if (!saveDC)
		return false;

	HBITMAP saveBitmap = CreateCompatibleBitmap(shadowDC, width, height);
	if (!saveBitmap)
	{
		DeleteDC(saveDC);
		return false;
	}

	HGDIOBJ saveOldObject = SelectObject(saveDC, saveBitmap);
	if (!saveOldObject || (saveOldObject == HGDI_ERROR))
	{
		DeleteObject(saveBitmap);
		DeleteDC(saveDC);
		return false;
	}

	MsRdpEx_CursorOverlay_FreeSaveSurface(ctx);
	ctx->saveDC = saveDC;
	ctx->saveBitmap = saveBitmap;
	ctx->saveOldObject = saveOldObject;
	ctx->saveWidth = width;
	ctx->saveHeight = height;

	return true;
}

MsRdpEx_CursorOverlay* MsRdpEx_CursorOverlay_New()
{
	MsRdpEx_CursorOverlay* ctx = (MsRdpEx_CursorOverlay*)calloc(1, sizeof(MsRdpEx_CursorOverlay));
	if (!ctx)
		return NULL;

	InitializeCriticalSectionAndSpinCount(&ctx->lock, 4000);
	return ctx;
}

void MsRdpEx_CursorOverlay_Free(MsRdpEx_CursorOverlay* ctx)
{
	if (!ctx)
		return;

	EnterCriticalSection(&ctx->lock);
	MsRdpEx_CursorOverlay_FreeSaveSurface(ctx);

	if (ctx->cursor)
		DestroyIcon(ctx->cursor);

	LeaveCriticalSection(&ctx->lock);
	DeleteCriticalSection(&ctx->lock);
	free(ctx);
}

bool MsRdpEx_CursorOverlay_SetShape(MsRdpEx_CursorOverlay* ctx, HCURSOR cursor)
{
	bool changed = false;
	HCURSOR cursorCopy = NULL;
	ICONINFO iconInfo = { 0 };
	BITMAP bitmap = { 0 };
	int32_t hotX = 0;
	int32_t hotY = 0;
	int32_t width = 0;
	int32_t height = 0;

	EnterCriticalSection(&ctx->lock);
	bool unchanged = (cursor == ctx->sourceCursor);
	LeaveCriticalSection(&ctx->lock);

	if (unchanged)
		return false;

	if (cursor)
	{
		cursorCopy = (HCURSOR)CopyIcon(cursor);
		if (!cursorCopy || !GetIconInfo(cursorCopy, &iconInfo))
		{
			if (cursorCopy)
				DestroyIcon(cursorCopy);
			return false;
		}

		HBITMAP sizeBitmap = iconInfo.hbmColor ? iconInfo.hbmColor : iconInfo.hbmMask;
		if (!sizeBitmap || !GetObject(sizeBitmap, sizeof(BITMAP), &bitmap))
		{
			DeleteObject(iconInfo.hbmMask);
			if (iconInfo.hbmColor)
				DeleteObject(iconInfo.hbmColor);
			DestroyIcon(cursorCopy);
			return false;
		}

		hotX = (int32_t)iconInfo.xHotspot;
		hotY = (int32_t)iconInfo.yHotspot;
		width = bitmap.bmWidth;
		height = (!iconInfo.hbmColor && iconInfo.hbmMask) ? (bitmap.bmHeight / 2) : bitmap.bmHeight;
	}

	EnterCriticalSection(&ctx->lock);

	if (cursor != ctx->sourceCursor)
	{
		if (ctx->cursor)
			DestroyIcon(ctx->cursor);

		ctx->sourceCursor = cursor;
		ctx->cursor = cursorCopy;
		cursorCopy = NULL;
		changed = true;

		if (cursor)
		{
			ctx->hotX = hotX;
			ctx->hotY = hotY;
			ctx->width = width;
			ctx->height = height;
		}
		else
		{
			ctx->visible = false;
			ctx->width = 0;
			ctx->height = 0;
		}
	}

	LeaveCriticalSection(&ctx->lock);

	if (iconInfo.hbmMask)
		DeleteObject(iconInfo.hbmMask);

	if (iconInfo.hbmColor)
		DeleteObject(iconInfo.hbmColor);

	if (cursorCopy)
		DestroyIcon(cursorCopy);

	return changed;
}

bool MsRdpEx_CursorOverlay_SetPosition(MsRdpEx_CursorOverlay* ctx, int32_t x, int32_t y, bool visible)
{
	bool emit = false;
	ULONGLONG now = GetTickCount64();

	EnterCriticalSection(&ctx->lock);

	visible = visible && (ctx->cursor != NULL);
	bool visibilityChanged = (ctx->visible != visible);
	bool positionChanged = (ctx->inputX != x) || (ctx->inputY != y);

	ctx->inputX = x;
	ctx->inputY = y;
	ctx->visible = visible;

	if ((visibilityChanged || positionChanged) &&
		(visibilityChanged || ((now - ctx->lastEmitTick) >= 30)))
	{
		ctx->lastEmitTick = now;
		emit = true;
	}

	LeaveCriticalSection(&ctx->lock);
	return emit;
}

void MsRdpEx_CursorOverlay_Composite(
	MsRdpEx_CursorOverlay* ctx, MsRdpEx_OutputMirror* outputMirror,
	HWND inputWindow, HWND outputWindow)
{
	HDC shadowDC = MsRdpEx_OutputMirror_GetShadowDC(outputMirror);
	if (!ctx || !shadowDC)
		return;

	// Undo a prior composite that never got its Restore (e.g. RDM's read threw between
	// LockShadowBitmap/UnlockShadowBitmap) so we don't bake a stale cursor into the save surface.
	MsRdpEx_CursorOverlay_Restore(ctx, outputMirror);

	EnterCriticalSection(&ctx->lock);

	ctx->composited = false;

	if (!ctx->visible || !ctx->cursor || (ctx->width <= 0) || (ctx->height <= 0) ||
		!IsWindow(inputWindow) || !IsWindow(outputWindow))
	{
		LeaveCriticalSection(&ctx->lock);
		return;
	}

	POINT point = { ctx->inputX, ctx->inputY };
	RECT inputRect = { 0 };
	RECT outputRect = { 0 };

	if (!GetClientRect(inputWindow, &inputRect) || !PtInRect(&inputRect, point))
	{
		LeaveCriticalSection(&ctx->lock);
		return;
	}

	SetLastError(ERROR_SUCCESS);
	int mapResult = MapWindowPoints(inputWindow, outputWindow, &point, 1);

	if ((mapResult == 0) && (GetLastError() != ERROR_SUCCESS))
	{
		LeaveCriticalSection(&ctx->lock);
		return;
	}

	if (!GetClientRect(outputWindow, &outputRect) || !PtInRect(&outputRect, point))
	{
		LeaveCriticalSection(&ctx->lock);
		return;
	}

	uint32_t frameWidth = 0;
	uint32_t frameHeight = 0;
	MsRdpEx_OutputMirror_GetFrameSize(outputMirror, &frameWidth, &frameHeight);

	int32_t cursorX = point.x - ctx->hotX;
	int32_t cursorY = point.y - ctx->hotY;
	int32_t left = max(cursorX, 0);
	int32_t top = max(cursorY, 0);
	int32_t right = min(cursorX + ctx->width, (int32_t)frameWidth);
	int32_t bottom = min(cursorY + ctx->height, (int32_t)frameHeight);
	int32_t copyWidth = right - left;
	int32_t copyHeight = bottom - top;

	if ((copyWidth <= 0) || (copyHeight <= 0) ||
		!MsRdpEx_CursorOverlay_EnsureSaveSurface(ctx, shadowDC, copyWidth, copyHeight) ||
		!BitBlt(ctx->saveDC, 0, 0, copyWidth, copyHeight, shadowDC, left, top, SRCCOPY))
	{
		LeaveCriticalSection(&ctx->lock);
		return;
	}

	if (DrawIconEx(shadowDC, cursorX, cursorY, ctx->cursor, 0, 0, 0, NULL, DI_NORMAL))
	{
		GdiFlush();
		ctx->composited = true;
		ctx->restoreLeft = left;
		ctx->restoreTop = top;
		ctx->restoreWidth = copyWidth;
		ctx->restoreHeight = copyHeight;
	}

	LeaveCriticalSection(&ctx->lock);
}

void MsRdpEx_CursorOverlay_Restore(MsRdpEx_CursorOverlay* ctx, MsRdpEx_OutputMirror* outputMirror)
{
	HDC shadowDC = MsRdpEx_OutputMirror_GetShadowDC(outputMirror);
	if (!ctx || !shadowDC)
		return;

	EnterCriticalSection(&ctx->lock);

	if (ctx->composited)
	{
		BitBlt(shadowDC, ctx->restoreLeft, ctx->restoreTop, ctx->restoreWidth, ctx->restoreHeight,
			ctx->saveDC, 0, 0, SRCCOPY);
		GdiFlush();
		ctx->composited = false;
	}

	LeaveCriticalSection(&ctx->lock);
}

// Push path (MsRdpEx's own recorder): draw the cursor, encode the frame, put the pixels back.
void MsRdpEx_CursorOverlay_DumpFrame(
	MsRdpEx_CursorOverlay* ctx, MsRdpEx_OutputMirror* outputMirror,
	HWND inputWindow, HWND outputWindow)
{
	MsRdpEx_CursorOverlay_Composite(ctx, outputMirror, inputWindow, outputWindow);
	MsRdpEx_OutputMirror_DumpFrame(outputMirror);
	MsRdpEx_CursorOverlay_Restore(ctx, outputMirror);
}