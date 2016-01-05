#include "stdafx.h"
#include <Shlwapi.h>

#pragma comment (lib, "Shlwapi.lib")
#pragma unmanaged

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
	)
{
	WCHAR path[MAX_PATH];
	GetModuleFileName(NULL, path, MAX_PATH);
	PathRemoveFileSpec(path);
	wcscat(path, L"\\ffmpeg");

#if defined(_WIN64)
	BOOL Is64BitProcess = TRUE;   // 64-bit program
#else
	BOOL Is64BitProcess = FALSE;
#endif

	wcscat(path, Is64BitProcess ? L"\\x64" : L"\\x86");
	SetDllDirectory(path);
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}
	return TRUE;
}