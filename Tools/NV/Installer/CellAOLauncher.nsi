;---------------------------------------
; CellAO Launcher Installer NSIS Script
;
; Copyright (c) 2005-2008, CellAO Team
;
; All rights reserved.
;
; Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
;
;     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
;     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
;     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
;
; THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
; "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
; LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
; A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
; CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
; EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
; PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
; PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
; LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
; NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
; SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
;---------------------------------------

SetCompressor /FINAL /SOLID lzma

!include "LogicLib.nsh"
!include "nsDialogs.nsh"
!include "MUI2.nsh"

!include "CellAOCommon.nsh"

!define MUI_ABORTWARNING
!define MUI_ICON "${INSTALL_ICON}"
!define MUI_UNICON "${UNINSTALL_ICON}"

!insertmacro MUI_PAGE_WELCOME

!insertmacro MUI_PAGE_LICENSE "Licence.txt"

var AOVER
var SERVERIP
var SERVERPORT
var COPYAO
!define MUI_DIRECTORYPAGE_TEXT_TOP "Setup will install $(^Name) in the following folder. $\r$\n$\r$\nIf this folder already contains a copy of the Anarchy Online client, it will be modified to use CellAO. If it does not contain a copy of the Anarchy Online client, you will be prompted to specify the location of an Anarchy Online client to copy to the installation folder. $\r$\n$\r$\nTo install in a different folder, click Browse and select another folder. Click Next to continue."
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE directoryLeave
!insertmacro MUI_PAGE_DIRECTORY

var AODIR
!define MUI_DIRECTORYPAGE_TEXT_TOP "Please specify the location of your Anarchy Online folder to copy for use with CellAO. Click Next to continue."
!define MUI_DIRECTORYPAGE_TEXT_DESTINATION "Anarchy Online Folder"
!define MUI_DIRECTORYPAGE_VARIABLE "$AODIR"
!define MUI_PAGE_CUSTOMFUNCTION_PRE anarchyDirectoryPre
!define MUI_PAGE_CUSTOMFUNCTION_SHOW anarchyDirectoryShow
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE anarchyDirectoryLeave
!insertmacro MUI_PAGE_DIRECTORY

Page custom ServerDetailsCreate ServerDetailsLeave

var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "CellAO Client"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${LAUNCHER_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU "Application" $ICONS_GROUP

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_NOREBOOTSUPPORT
!define MUI_FINISHPAGE_NOAUTOCLOSE
!define MUI_FINISHPAGE_RUN "$INSTDIR\CellAOLauncher.exe"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Name "${LAUNCHER_NAME} ${LAUNCHER_VERSION}"
OutFile "Built\CellAO-Client-${LAUNCHER_VERSION}-Setup.exe"
InstallDir "$PROGRAMFILES\CellAO Client"
InstallDirRegKey HKLM "${LAUNCHER_DIR_REGKEY}" ""
ShowInstDetails show
ShowUnInstDetails show
BrandingText "CellAO"
RequestExecutionLevel admin
CRCCheck on

Function .onInit
  !insertmacro MutexCheck
  
  ; XXXX NV TODO: Make it scan the registry to find the Uninstall Information for AO and get the directory from there before using this fallback default
  StrCpy $AODIR "$PROGRAMFILES\Funcom\Anarchy Online"
  StrCpy $SERVERIP "127.0.0.1"
  StrCpy $SERVERPORT "7500"
FunctionEnd

Function directoryLeave
  !insertmacro CheckNoAo

  FileOpen $0 "$INSTDIR\version.id" "r"
  FileRead $0 $AOVER
  FileClose $0
  StrCpy $AOVER $AOVER -2
  StrCpy $1 $AOVER "" -4
  StrCmp $1 "_EP1" "" warnFroob
  StrCpy $AOVER $AOVER -4
  Goto next
  warnFroob:
  MessageBox MB_YESNO|MB_ICONINFORMATION "Small client detected in Installation Directory. It is reccomended you use a Full client with CellAO. Do you wish to continue?" IDYES next
    Abort
  next:

  StrCpy $COPYAO 0
  Goto end
  noao:
  StrCpy $COPYAO 1
  end:
FunctionEnd

Function anarchyDirectoryPre
  StrCmp $COPYAO 0 abort

  Goto end
  abort:
  Abort
  end:
FunctionEnd

Function anarchyDirectoryShow
  FindWindow $0 "#32770" "" $HWNDPARENT
  GetDlgItem $1 $0 1023 ; 1023 = IDC_SPACEREQUIRED
  ShowWindow $1 0
  GetDlgItem $1 $0 1024 ; 1024 = IDC_SPACEAVAILABLE
  ShowWindow $1 0
FunctionEnd

Function anarchyDirectoryLeave
  !insertmacro CheckNoAo

  FileOpen $0 "$AODIR\version.id" "r"
  FileRead $0 $AOVER
  FileClose $0
  StrCpy $AOVER $AOVER -2
  StrCpy $1 $AOVER "" -4
  StrCmp $1 "_EP1" "" warnFroob
  StrCpy $AOVER $AOVER -4
  Goto next
  warnFroob:
  MessageBox MB_YESNO|MB_ICONINFORMATION "Small client detected. It is reccomended you use a Full client with CellAO. Do you wish to continue?" IDYES next
    Abort
  next:

  Goto end
  noao:
  MessageBox MB_OK|MB_ICONEXCLAMATION "'$AODIR' does not contain a valid Anarchy Online installation."
  Abort
  end:
FunctionEnd

var SERVERDETAILSDLG
var SERVERDETAILSDLG_IP
var SERVERDETAILSDLG_PORT
Function ServerDetailsCreate
  nsDialogs::Create /NOUNLOAD 1018
  Pop $SERVERDETAILSDLG

  !insertmacro MUI_HEADER_TEXT "Server Details" "Specify server details to connect to."
  
  ${NSD_CreateLabel} 0 0 100% 12u "Please specify the Host/IP and Port of the CellAO server you wish to connect to."
  Pop $0

  ${NSD_CreateGroupBox} 0 40u 100% 27u "Server Host/IP"
  Pop $0
  ${NSD_CreateText} 5u 50u -10u 12u "$SERVERIP"
  Pop $SERVERDETAILSDLG_IP

  ${NSD_CreateGroupBox} 0u 80u 100% 27u "Server Port"
  Pop $0
  ${NSD_CreateText} 5u 90u -10u 12u "$SERVERPORT"
  Pop $SERVERDETAILSDLG_PORT

  nsDialogs::Show
FunctionEnd

Function ServerDetailsLeave
  ${NSD_GetText} $SERVERDETAILSDLG_IP $SERVERIP
  ${NSD_GetText} $SERVERDETAILSDLG_PORT $SERVERPORT
FunctionEnd

Section "CellAO Launcher" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  
  DetailPrint "Detected Anarchy Online version $AOVER"
  StrCmp $COPYAO 0 SkipAOCopy
  DetailPrint "Copying Anarchy Online Installation..."
  CopyFiles /SILENT "$AODIR\cd_image" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\*.dll" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\setupf" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\Anarchy.exe" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\AnarchyPatcher.exe" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\Client.exe" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\*.dat" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\version.id" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\anarchy.cfg" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\commands.txt" "$INSTDIR"
  CopyFiles /SILENT "$AODIR\mapid.txt" "$INSTDIR"
  CreateDirectory "$INSTDIR\Scripts"
  CreateDirectory "$INSTDIR\Prefs"
  SetOutPath "$INSTDIR\Prefs"
  File "Prefs.xml"
  SetOutPath "$INSTDIR"
  SkipAOCopy:

  IfFileExists "$INSTDIR\cd_image\data\launcher\DimensionServer.url" "" +1
  Rename "$INSTDIR\cd_image\data\launcher\DimensionServer.url" "$INSTDIR\cd_image\data\launcher\DimensionServer.url.bak"

  IfFileExists "$INSTDIR\cd_image\data\launcher\AnarchyLauncher.url" "" +1
  Rename "$INSTDIR\cd_image\data\launcher\AnarchyLauncher.url" "$INSTDIR\cd_image\data\launcher\AnarchyLauncher.url.bak"

  IfFileExists "$INSTDIR\cd_image\data\launcher\dimensions.txt" "" +1
  Rename "$INSTDIR\cd_image\data\launcher\dimensions.txt" "$INSTDIR\cd_image\data\launcher\dimensions.txt.bak"
  
  SetOutPath "$INSTDIR\cd_image\data\launcher"
  File "AnarchyLauncher.url"
  SetOutPath "$INSTDIR"

  FileOpen $0 "$INSTDIR\cd_image\data\launcher\dimensions.txt" "w"
  FileWrite $0 "# This file holds all the timelines available at this time.$\r$\n# All lines starting with # is ignored by parser...$\r$\n#$\r$\n$\r$\n"
  FileWrite $0 "STARTINFO$\r$\n"
  FileWrite $0 "description     =       CellAO$\r$\n"
  FileWrite $0 "displayname     =       CellAO$\r$\n"
  FileWrite $0 "connect         =       $SERVERIP$\r$\n"
  FileWrite $0 "ports           =       $SERVERPORT$\r$\n"
  FileWrite $0 "url             =       http://www.aocell.info/news.php$\r$\n"
  FileWrite $0 "version         =       $AOVER$\r$\n"
  FileWrite $0 "ENDINFO$\r$\n"
  FileClose $0

  File "..\Launcher\CellAOLauncher.exe"
  File "..\Launcher\CellAOLauncher.dll"
  
  SetShellVarContext all
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\CellAO Launcher.lnk" "$INSTDIR\CellAOLauncher.exe"
  CreateShortCut "$DESKTOP\CellAO Launcher.lnk" "$INSTDIR\CellAOLauncher.exe"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -AdditionalIcons
  SetShellVarContext all
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  WriteIniStr "$INSTDIR\CellAO.url" "InternetShortcut" "URL" "${WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\CellAO Website.lnk" "$INSTDIR\CellAO.url"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -Post
  WriteUninstaller "$INSTDIR\CellAOLauncher-uninst.exe"
  WriteRegStr HKLM "${LAUNCHER_DIR_REGKEY}" "" "$INSTDIR\CellAOLauncher.exe"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "UninstallString" "$INSTDIR\CellAOLauncher-uninst.exe"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "DisplayIcon" "$INSTDIR\CellAOLauncher.exe"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "DisplayVersion" "${LAUNCHER_VERSION}"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "URLInfoAbout" "${WEB_SITE}"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "Publisher" "${PUBLISHER}"
SectionEnd


Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  SetShellVarContext all
  !insertmacro MUI_STARTMENU_GETFOLDER "Application" $ICONS_GROUP
  Delete "$INSTDIR\CellAOLauncher-uninst.exe"
  Delete "$INSTDIR\CellAO.url"
  Delete "$INSTDIR\CellAOLauncher.dll"
  Delete "$INSTDIR\CellAOLauncher.exe"

  IfFileExists "$INSTDIR\cd_image\data\launcher\DimensionServer.url.bak" "" +2
  Delete "$INSTDIR\cd_image\data\launcher\DimensionServer.url"
  Rename "$INSTDIR\cd_image\data\launcher\DimensionServer.url.bak" "$INSTDIR\cd_image\data\launcher\DimensionServer.url"

  IfFileExists "$INSTDIR\cd_image\data\launcher\AnarchyLauncher.url.bak" "" +2
  Delete "$INSTDIR\cd_image\data\launcher\AnarchyLauncher.url"
  Rename "$INSTDIR\cd_image\data\launcher\AnarchyLauncher.url.bak" "$INSTDIR\cd_image\data\launcher\AnarchyLauncher.url"

  IfFileExists "$INSTDIR\cd_image\data\launcher\dimensions.txt.bak" "" +2
  Delete "$INSTDIR\cd_image\data\launcher\dimensions.txt"
  Rename "$INSTDIR\cd_image\data\launcher\dimensions.txt.bak" "$INSTDIR\cd_image\data\launcher\dimensions.txt"

  Delete "$SMPROGRAMS\$ICONS_GROUP\CellAO Website.lnk"
  Delete "$DESKTOP\CellAO Launcher.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\CellAO Launcher.lnk"

  RMDir "$SMPROGRAMS\$ICONS_GROUP"
  
  MessageBox MB_YESNO|MB_ICONQUESTION "Do you wish to remove the Anarchy Online client from the CellAO Launcher directory?" IDNO SkipRemove
    RMDir /r "$INSTDIR"
  SkipRemove:

  DeleteRegKey ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}"
  DeleteRegKey HKLM "${LAUNCHER_DIR_REGKEY}"
  SetAutoClose true
SectionEnd
