!include "MUI2.nsh"
Unicode True
Name "Subloader Installer"
OutFile "installer.exe"
InstallDir "$PROGRAMFILES64\Subloader"

;--------------------------------
;defines

!define MUI_ICON "icon.ico"

;--------------------------------
;PAGES

!insertmacro MUI_PAGE_LICENSE "licence.txt"
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_LANGUAGE "English"

;--------------------------------
;Installer Sections
Section "Install"

  ;Add files
  SetOutPath "$INSTDIR"

  File "SubLoad.exe"

  Call UninstallPrevious

  ;create start-menu items
  CreateDirectory "$SMPROGRAMS\Subloader"
  CreateShortCut "$SMPROGRAMS\Subloader\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Subloader\Subloader.lnk" "$INSTDIR\SubLoad.exe" "" "$INSTDIR\SubLoad.exe" 0

  ;write uninstall information to the registry
  WriteRegStr HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader" "DisplayName" "Subloader (remove only)"
  WriteRegStr HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader" "UninstallString" "$INSTDIR\Uninstall.exe"

  ;CONTEXT REGISTRY COMMANDS
  Var /GLOBAL extPath

  StrCpy $extPath "SystemFileAssociations\.avi"
  DetailPrint "Writing registry keys for AVI context menu"
  ;AVI
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'

  StrCpy $extPath "SystemFileAssociations\.mp4"
  DetailPrint "Writing registry keys for MP4 context menu"
  ;for mp4
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'

  StrCpy $extPath "SystemFileAssociations\.mkv"
  DetailPrint "Writing registry keys for MKV context menu"
  ;for mkv
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_CLASSES_ROOT "$extPath\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'

  ;-------------------------

  WriteUninstaller "$INSTDIR\Uninstall.exe"

SectionEnd

;--------------------------------
;Uninstaller Section
Section "Uninstall"
	;Delete Files
	RMDir /r "$INSTDIR\*.*"

	;Remove the installation directory
	RMDir "$INSTDIR"

	Var /GLOBAL crVar

  StrCpy $crVar "SystemFileAssociations\.avi"
	;ReadRegStr $crVar HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "aviCR"
	DetailPrint "Deleting registry key: HKCR\$crVar\shell\Subloader"
	DeleteRegKey HKEY_CLASSES_ROOT "$crVar\shell\Subloader"

  StrCpy $crVar "SystemFileAssociations\.mp4"
	;ReadRegStr $crVar HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "mp4CR"
	DetailPrint "Deleting registry key: HKCR\$crVar\shell\Subloader"
	DeleteRegKey HKEY_CLASSES_ROOT "$crVar\shell\Subloader"

  StrCpy $crVar "SystemFileAssociations\.mkv"
	;ReadRegStr $crVar HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "mkvCR"
	DetailPrint "Deleting registry key: HKLM\$crVar\shell\Subloader"
	DeleteRegKey HKEY_CLASSES_ROOT "$crVar\shell\Subloader"

	;Delete Start Menu Shortcuts
	Delete "$SMPROGRAMS\Subloader\*.*"
	RmDir  "$SMPROGRAMS\Subloader"

	;Delete Uninstaller And Unistall Registry Entries
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Subloader"
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader"

SectionEnd

Function UninstallPrevious

    ; Check for uninstaller.
    ReadRegStr $R0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader" "UninstallString"

    ${If} $R0 == ""
        Goto Done
    ${EndIf}

    DetailPrint "Removing previous installation."

    ; Run the uninstaller silently.
    ExecWait '"$R0 /S"'

    Done:

FunctionEnd