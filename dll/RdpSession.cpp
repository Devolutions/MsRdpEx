
#include <MsRdpEx/RdpSession.h>

#include <MsRdpEx/ArrayList.h>

#include "TSObjects.h"

struct _MsRdpEx_RdpSession
{
	HWND hOutputPresenterWnd;
	MsRdpEx_OutputMirror* outputMirror;
};

MsRdpEx_RdpSession* MsRdpEx_RdpSession_New()
{
	MsRdpEx_RdpSession* session;
	
	session = (MsRdpEx_RdpSession*) calloc(1, sizeof(MsRdpEx_RdpSession));

	if (!session)
		return NULL;

	return session;
}

MsRdpEx_OutputMirror* MsRdpEx_RdpSession_GetOutputMirror(MsRdpEx_RdpSession* session)
{
	return session->outputMirror;
}

void MsRdpEx_RdpSession_SetOutputMirror(MsRdpEx_RdpSession* session, MsRdpEx_OutputMirror* outputMirror)
{
	session->outputMirror = outputMirror;
}

void MsRdpEx_RdpSession_AttachWindow(MsRdpEx_RdpSession* session, HWND hWnd, void* pUserData)
{
	if (!session)
		return;

	return;
}

void MsRdpEx_RdpSession_Free(MsRdpEx_RdpSession* session)
{
	if (!session)
		return;

	if (session->outputMirror) {
		MsRdpEx_OutputMirror_Free(session->outputMirror);
		session->outputMirror = NULL;
	}
	
	free(session);
}

struct _MsRdpEx_SessionManager
{
	MsRdpEx_ArrayList* sessions;
};

void MsRdpEx_SessionManager_Free(MsRdpEx_SessionManager* ctx);

static int g_RefCount = 0;
static MsRdpEx_SessionManager* g_SessionManager = NULL;

bool MsRdpEx_SessionManager_Add(MsRdpEx_RdpSession* session)
{
	MsRdpEx_SessionManager* ctx = g_SessionManager;

	if (!ctx)
		return false;

	MsRdpEx_ArrayList_Add(ctx->sessions, session);

	return true;
}

bool MsRdpEx_SessionManager_Remove(MsRdpEx_RdpSession* session, bool free)
{
	MsRdpEx_SessionManager* ctx = g_SessionManager;

	if (!ctx)
		return false;

	MsRdpEx_ArrayList_Remove(ctx->sessions, session, free);

	return true;
}

MsRdpEx_RdpSession* MsRdpEx_SessionManager_FindByOutputPresenterHwnd(HWND hWnd)
{
	MsRdpEx_SessionManager* ctx = g_SessionManager;

	if (!ctx)
		return NULL;

	bool found = false;
	MsRdpEx_RdpSession* obj = NULL;
	MsRdpEx_ArrayListIt* it = NULL;

	it = MsRdpEx_ArrayList_It(ctx->sessions, MSRDPEX_ITERATOR_FLAG_EXCLUSIVE);

	while (!MsRdpEx_ArrayListIt_Done(it))
	{
		obj = (MsRdpEx_RdpSession*) MsRdpEx_ArrayListIt_Next(it);

		found = (obj->hOutputPresenterWnd == hWnd) ? true : false;

		if (found)
			break;
	}

	MsRdpEx_ArrayListIt_Finish(it);

	return found ? obj : NULL;
}

MsRdpEx_SessionManager* MsRdpEx_SessionManager_New()
{
	MsRdpEx_SessionManager* ctx;

	ctx = (MsRdpEx_SessionManager*) calloc(1, sizeof(MsRdpEx_SessionManager));

	if (!ctx)
		return NULL;

	ctx->sessions = MsRdpEx_ArrayList_New(true);

	if (!ctx->sessions)
		goto error;

	MsRdpEx_ArrayList_Object(ctx->sessions)->fnObjectFree =
		(MSRDPEX_OBJECT_FREE_FN) MsRdpEx_RdpSession_Free;

	return ctx;
error:
	MsRdpEx_SessionManager_Free(ctx);
	return NULL;
}

void MsRdpEx_SessionManager_Free(MsRdpEx_SessionManager* ctx)
{
	if (!ctx)
		return;

	if (ctx->sessions) {
		MsRdpEx_ArrayList_Free(ctx->sessions);
		ctx->sessions = NULL;
	}
}

MsRdpEx_SessionManager* MsRdpEx_SessionManager_Get()
{
	if (!g_SessionManager)
		g_SessionManager = MsRdpEx_SessionManager_New();

	g_RefCount++;

	return g_SessionManager;
}

void MsRdpEx_SessionManager_Release()
{
	g_RefCount--;

	if (g_RefCount < 0)
		g_RefCount = 0;

	if (g_SessionManager && (g_RefCount < 1))
	{
		MsRdpEx_SessionManager_Free(g_SessionManager);
		g_SessionManager = NULL;
	}
}
