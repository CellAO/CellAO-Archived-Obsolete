;---------------------------------------
; CellAO Server Installer NSIS Script
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

!execute 'SubWCRev ..\..\..\ CellAOCommon.nsh.template CellAOCommon.nsh'

!include "CellAOCommon.nsh"

!define MUI_ABORTWARNING
!define MUI_ICON "${INSTALL_ICON}"
!define MUI_UNICON "${UNINSTALL_ICON}"

!insertmacro MUI_PAGE_WELCOME

!insertmacro MUI_PAGE_LICENSE "Licence.txt"

!define MUI_PAGE_CUSTOMFUNCTION_PRE componentsPre
!define MUI_PAGE_CUSTOMFUNCTION_LEAVE componentsLeave
!insertmacro MUI_PAGE_COMPONENTS

!insertmacro MUI_PAGE_DIRECTORY

var ICONS_GROUP
!define MUI_STARTMENUPAGE_NODISABLE
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "CellAO Server"
!define MUI_STARTMENUPAGE_REGISTRY_ROOT "${UNINST_ROOT_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_KEY "${SERVER_UNINST_KEY}"
!define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "${STARTMENU_REGVAL}"
!insertmacro MUI_PAGE_STARTMENU "Application" $ICONS_GROUP

!insertmacro MUI_PAGE_INSTFILES

!define MUI_FINISHPAGE_NOAUTOCLOSE
;!define MUI_FINISHPAGE_RUN "$INSTDIR\CellAO.exe"
!insertmacro MUI_PAGE_FINISH

!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "English"

Name "${SERVER_NAME} ${SERVER_VERSION}"
OutFile "Built\CellAO-Server-${SERVER_VERSION}-Setup.exe"
InstallDir "$PROGRAMFILES\CellAO"
ShowInstDetails show
ShowUnInstDetails show
BrandingText "CellAO"
RequestExecutionLevel admin
CRCCheck on

var IS64BIT
var MYSQLPATH
var MYSQLVERSION
var HASMYSQL

var SERVERHOST
var MYSQLHOST
var MYSQLUSERNAME
var MYSQLPASSWORD
var MYSQLDATABASE
var ENABLEPASSWORD

Function .onInit
  !insertmacro MutexCheck
  
  ; XXXX TODO: NV - Have the installer prompt to change these from their default values during install
  StrCpy $SERVERHOST "127.0.0.1"
  StrCpy $MYSQLHOST "127.0.0.1"
  StrCpy $MYSQLUSERNAME "root"
  StrCpy $MYSQLPASSWORD ""
  StrCpy $MYSQLDATABASE "cellao"
  StrCpy $ENABLEPASSWORD "false"

  System::Call "kernel32::GetCurrentProcess() i .s"
  System::Call "kernel32::IsWow64Process(i s, *i .r0)"
  ${If} $0 != 0
    StrCpy $IS64BIT 1
  ${Else}
    StrCpy $IS64BIT 0
  ${EndIf}
  
  !insertmacro FindMySQL $MYSQLPATH $MYSQLVERSION
  
  ${If} $MYSQLVERSION != ""
    StrCpy $HASMYSQL 1
  ${Else}
    StrCpy $HASMYSQL 0
  ${EndIf}
  
  InitPluginsDir
  SetOutPath "$PLUGINSDIR"
  File "${NSISDIR}\Plugins\NSISdl.dll"
FunctionEnd

Function componentsPre
  ${If} $HASMYSQL == 1
    ; No point showing the components page if we have already found MySQL is installed
    ;Abort
  ${EndIf}
FunctionEnd

Function componentsLeave
  ; Just in case
  ${If} $HASMYSQL != 1
  ${EndIf}
FunctionEnd

Section "CellAO Server" SecCellAO
  SectionIn RO
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  
  ${If} $IS64BIT != 0
    DetailPrint "64-bit Operating System Detected"
  ${Else}
    DetailPrint "32-bit Operating System Detected"
  ${EndIf}
  
  DetailPrint "Checking if .NET Framework 4.0 is installed"
  ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4.0" "Install"
  ${If} $0 != 1
    DetailPrint ".NET Framework 4.0 not found. Installing"

    File "dotNetFx40_Full_setup.exe"
    
    ExecWait "$OUTDIR\dotNetFx35setup.exe /q /norestart" $1
    ; 3010 ERROR_SUCCESS_REBOOT_REQUIRED
    ${If} $1 == 3010
      SetRebootFlag true
    ${EndIf}
    
    Delete "dotNetFx40_Full_setup.exe"

    DetailPrint "Done installing .NET Framework 4.0."
  ${Else}
    DetailPrint ".NET Framework 4.0 found."
  ${EndIf}
  
  File "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\ZoneEngine.exe"
  File "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\LoginEngine.exe"
  File "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\ChatEngine.exe"
  
  File "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\*.dll"

  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\Scripts"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\SQLTables"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\XML Data"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\de"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\es"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\fi"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\fr"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\ja"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\locale"
  File /r "..\..\..\${SERVER_SVN_DIRECTORY}\Built\Release\zh-CN"

  SetShellVarContext all
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  CreateDirectory "$SMPROGRAMS\$ICONS_GROUP"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\CellAO ZoneEngine.lnk" "$INSTDIR\ZoneEngine.exe"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\CellAO LoginEngine.lnk" "$INSTDIR\LoginEngine.exe"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\CellAO ChatEngine.lnk" "$INSTDIR\ChatEngine.exe"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

var MYSQL_URL
var MYSQL_FILENAME
Section /o "MySQL Server" SecMySQL
  ${If} $IS64BIT != 0
    DetailPrint "Downloading 64-bit MySQL"
    StrCpy $MYSQL_URL ${MYSQL_URL_WIN64}
    StrCpy $MYSQL_FILENAME ${MYSQL_FILENAME_WIN64}
  ${Else}
    DetailPrint "Downloading 32-bit MySQL"
    StrCpy $MYSQL_URL ${MYSQL_URL_WIN32}
    StrCpy $MYSQL_FILENAME ${MYSQL_FILENAME_WIN32}
  ${EndIf}
  
  NSISdl::download /TIMEOUT=30000 "$MYSQL_URL" "$PLUGINSDIR\$MYSQL_FILENAME"
  Pop $0
  StrCmp "$0" "success" success
  DetailPrint "MySQL Download Failed: $0. Skipping MySQL Install."
  DetailPrint "Please install MySQL Manually if required."
  goto end
  success:
  DetailPrint "Installing MySQL"

  ExecWait 'MSIEXEC /passive /norestart /i "$PLUGINSDIR\$MYSQL_FILENAME"' $0
  ; 3010 ERROR_SUCCESS_REBOOT_REQUIRED
  ${If} $0 == 3010
    SetRebootFlag true
    goto installok
  ${EndIf}
  ${If} $0 == 0
    goto installok
  ${EndIf}
  ${If} $0 == ""
    goto installok
  ${EndIf}

  DetailPrint "MySQL Installation Failed. Skipping MySQL Install."
  DetailPrint "Please install MySQL Manually if required."
  goto end
  installok:
  !insertmacro FindMySQL $MYSQLPATH $MYSQLVERSION

  ${If} $MYSQLVERSION != ""
    DetailPrint "MySQL Successfully Installed"
    StrCpy $HASMYSQL 1
  ${Else}
    DetailPrint "DANGER WILL ROBINSON. This should not happen."
    DetailPrint "MySQL 'Successfully' Installed but unable to locate!"
    StrCpy $HASMYSQL 0
  ${EndIf}

  end:
SectionEnd

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SecCellAO} "CellAO core components."
  !insertmacro MUI_DESCRIPTION_TEXT ${SecMySQL} "MySQL server ${MYSQL_VERSION} (will be downloaded during installation)."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

Section -AdditionalIcons
  SetShellVarContext all
  !insertmacro MUI_STARTMENU_WRITE_BEGIN Application
  WriteIniStr "$INSTDIR\CellAO.url" "InternetShortcut" "URL" "${WEB_SITE}"
  CreateShortCut "$SMPROGRAMS\$ICONS_GROUP\CellAO Website.lnk" "$INSTDIR\CellAO.url"
  !insertmacro MUI_STARTMENU_WRITE_END
SectionEnd

Section -Post
  FileOpen $0 "$INSTDIR\Config.xml" w
  FileWrite $0 "<?xml version=$\"1.0$\" encoding=$\"utf-8$\"?>$\r$\n"
  FileWrite $0 "<Config>$\r$\n"
  FileWrite $0 "  <!-- Ports for the Game Engines -->$\r$\n"
  FileWrite $0 "  <ListenIP>0.0.0.0</ListenIP>$\r$\n"
  FileWrite $0 "  <ChatIP>127.0.0.1</ChatIP>$\r$\n"
  FileWrite $0 "  <ZoneIP>127.0.0.1</ZoneIP>$\r$\n"
  FileWrite $0 "  <ChatPort>7012</ChatPort>$\r$\n"
  FileWrite $0 "  <ZonePort>7501</ZonePort>$\r$\n"
  FileWrite $0 "  <LoginPort>7500</LoginPort>$\r$\n"
  FileWrite $0 "  <CommPort>6996</CommPort>$\r$\n"
  FileWrite $0 "  <!-- Log Area -->$\r$\n"
  FileWrite $0 "  <SqlLog>true</SqlLog>$\r$\n"
  FileWrite $0 "  <LogChat>false</LogChat>$\r$\n"
  FileWrite $0 "  <!-- Server MOTD -->$\r$\n"
  FileWrite $0 "  <Motd>Welcome to CellAO-Shining_Coiler 1.0.3.0, you Visit our Forums at http://www.aocell.info</Motd>$\r$\n"
  FileWrite $0 "    <!-- Set this to true if you want to use encrypted passwords with your server you will need to supply each user the launcher we have created -->$\r$\n"
  FileWrite $0 "  <UsePassword>false</UsePassword>$\r$\n"
  FileWrite $0 "  <!-- Choose your SQL Databse Type here, MySql, MsSql, PostgreSQL-->$\r$\n"
  FileWrite $0 "  <SQLType>MySql</SQLType>$\r$\n"
  FileWrite $0 "  <!-- Sql Connection Setup Area -->$\r$\n"
  FileWrite $0 "  <MysqlConnection>Database=cellao;Data Source=localhost;User ID=root;Password=root</MysqlConnection>$\r$\n"
  FileWrite $0 "  <MsSqlConnection>Data Source=localhost;Inital Catalog=cellao;UserID=Administrator;Password=password;Trusted_Connection=false</MsSqlConnection>$\r$\n"
  FileWrite $0 "  <PostgreConnection>Server=localhost;User Id=root;Password=root;Database=cellao</PostgreConnection>$\r$\n"
  FileWrite $0 "  <!-- This area is for Locale Settings, en = english, gr = german, more languages will be supported later, Default = en-->$\r$\n"
  FileWrite $0 "  <Locale>en</Locale>$\r$\n"
  FileWrite $0 "  <!-- This area will be used for Complit Checks for Grid Use -->$\r$\n"
  FileWrite $0 "  <WestAthen>0</WestAthen>$\r$\n"
  FileWrite $0 "  <OldAthen>0</OldAthen>$\r$\n"
  FileWrite $0 "  <Tir_Inside>0</Tir_Inside>$\r$\n"
  FileWrite $0 "  <Tir_Outside>0</Tir_Outside>$\r$\n"
  FileWrite $0 "  <Borealis>0</Borealis>$\r$\n"
  FileWrite $0 "  <Meetmedeere>0</Meetmedeere>$\r$\n"
  FileWrite $0 "  <Newland>0</Newland>$\r$\n"
  FileWrite $0 "  <Omni_One_Trade>0</Omni_One_Trade>$\r$\n"
  FileWrite $0 "  <Rome_Red>0</Rome_Red>$\r$\n"
  FileWrite $0 "  <Omni_One_Entertainment_North>0</Omni_One_Entertainment_North>$\r$\n"
  FileWrite $0 "  <Omni_One_Entertainment_South>0</Omni_One_Entertainment_South>$\r$\n"
  FileWrite $0 "  <Lush_Hills>0</Lush_Hills>$\r$\n"
  FileWrite $0 "  <Clondyke>0</Clondyke>$\r$\n"
  FileWrite $0 "  <Galway_County>0</Galway_County>$\r$\n"
  FileWrite $0 "  <Broken_Shores>0</Broken_Shores>$\r$\n"
  FileWrite $0 "  <four_holes>0</four_holes>$\r$\n"
  FileWrite $0 "  <twoho>0</twoho>$\r$\n"
  FileWrite $0 "  <Harrys>0</Harrys>$\r$\n"
  FileWrite $0 "  <Omni_One_HQ>0</Omni_One_HQ>$\r$\n"
  FileWrite $0 "  <Camelot>0</Camelot>$\r$\n"
  FileWrite $0 "  <Sentinels>0</Sentinels>$\r$\n"
  FileWrite $0 "</Config>$\r$\n"
  FileClose $0

  WriteUninstaller "$INSTDIR\CellAO-uninst.exe"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "UninstallString" "$INSTDIR\CellAO-uninst.exe"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "DisplayIcon" "$INSTDIR\ZoneEngine.exe"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "DisplayVersion" "${LAUNCHER_VERSION}"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "URLInfoAbout" "${WEB_SITE}"
  WriteRegStr ${UNINST_ROOT_KEY} "${LAUNCHER_UNINST_KEY}" "Publisher" "${PUBLISHER}"
SectionEnd


; Uninstaller Stuff
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
  Delete "$INSTDIR\CellAO-uninst.exe"
  Delete "$INSTDIR\CellAO.url"

  Delete "$INSTDIR\ZoneEngine.exe"
  Delete "$INSTDIR\LoginEngine.exe"
  Delete "$INSTDIR\ChatEngine.exe"

  Delete "$INSTDIR\AO.Core.dll"
  Delete "$INSTDIR\Cell.Core.dll"
  Delete "$INSTDIR\ISComm.dll"
  Delete "$INSTDIR\lua51.dll"
  Delete "$INSTDIR\LuaInterface.dll"
  Delete "$INSTDIR\MySql.Data.dll"
  Delete "$INSTDIR\NLog.dll"
  Delete "$INSTDIR\zlib.net.dll"
  Delete "$INSTDIR\Interop.NATUPNPLib.dll"

  RMDir /r "$INSTDIR\Scripts"
  RMDir /r "$INSTDIR\SQLTables"
  RMDir /r "$INSTDIR\Client_Database"
  RMDir /r "$INSTDIR\XML Data"

  MessageBox MB_YESNO|MB_ICONQUESTION "Do you wish to remove the CellAO Configuration file?" IDNO SkipRemove
    Delete "$INSTDIR\Config.xml"
    RMDir "$INSTDIR"
  SkipRemove:
  
  Delete "$SMPROGRAMS\$ICONS_GROUP\CellAO Website.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\CellAO ZoneEngine.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\CellAO LoginEngine.lnk"
  Delete "$SMPROGRAMS\$ICONS_GROUP\CellAO ChatEngine.lnk"

  RMDir "$SMPROGRAMS\$ICONS_GROUP"

  DeleteRegKey ${UNINST_ROOT_KEY} "${SERVER_UNINST_KEY}"
  SetAutoClose true
SectionEnd
