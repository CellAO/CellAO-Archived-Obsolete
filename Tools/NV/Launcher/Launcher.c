/*
Copyright (c) 2005-2008, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#include <Windows.h>

#ifndef _DEBUG
#define DLLNAME "CellAOLauncher.dll"
#else
#define DLLNAME "CellAOLauncher.dbg.dll"
#endif

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow)
{
	STARTUPINFO			si;
	PROCESS_INFORMATION	pi;
	HANDLE				hKernel;
	void				*dllname;

	memset(&si, 0, sizeof(si));

	si.cb = sizeof(si);

	if (!CreateProcess("Anarchy.exe", NULL, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &si, &pi))
	{
		MessageBox(0, "Failed to load Anarchy.exe!\nMake sure CellAOLauncher.exe and CellAOLauncher.dll are both in the same directory as Anarchy.exe.", "CellAO Launcher", MB_ICONEXCLAMATION);

		return 1;
	}

	hKernel = GetModuleHandle("Kernel32.dll");

	dllname = VirtualAllocEx(pi.hProcess, NULL, sizeof(DLLNAME), MEM_RESERVE|MEM_COMMIT, PAGE_READWRITE);
	WriteProcessMemory(pi.hProcess, dllname, DLLNAME, sizeof(DLLNAME), NULL);

	if (CreateRemoteThread(pi.hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)GetProcAddress(hKernel, "LoadLibraryA"), dllname, 0, NULL) == NULL)
	{
		MessageBox(0, "Failed to create remote thread in Anarchy.exe.\nPlease check Anti-Virus software/etc that could be preventing this", "CellAO Launcher", MB_ICONEXCLAMATION);
	}

	CloseHandle(hKernel);

	ResumeThread(pi.hThread);

	CloseHandle(pi.hThread);
	CloseHandle(pi.hProcess);

	return 0;
}
