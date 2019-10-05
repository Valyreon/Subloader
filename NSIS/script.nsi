!include "MUI2.nsh"
Name "Subloader Installer"
OutFile "installer.exe"
InstallDir "$PROGRAMFILES\Subloader"

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
  File "SubtitleSuppliers.dll"
  File "Newtonsoft.Json.dll"
   
  ;create start-menu items
  CreateDirectory "$SMPROGRAMS\Subloader"
  CreateShortCut "$SMPROGRAMS\Subloader\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Subloader\Subloader.lnk" "$INSTDIR\SubLoad.exe" "" "$INSTDIR\SubLoad.exe" 0
   
  ;write uninstall information to the registry
  WriteRegStr HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader" "DisplayName" "Subloader (remove only)"
  WriteRegStr HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader" "UninstallString" "$INSTDIR\Uninstall.exe"
  
  ;CONTEXT REGISTRY COMMANDS
  Var /GLOBAL defaultAvi
  Var /GLOBAL defaultMp4
  Var /GLOBAL defaultMkv
  
  ReadRegStr $defaultAvi HKEY_CLASSES_ROOT .avi ""
  ReadRegStr $defaultMp4 HKEY_CLASSES_ROOT .mp4 ""
  ReadRegStr $defaultMkv HKEY_CLASSES_ROOT .mkv ""
  
  ;HKEY_CURRENT_USER\Software\Classes\jpegfile\shell
  
  DetailPrint "Writing registry keys for AVI context menu"
  ;AVI
  WriteRegStr HKEY_CLASSES_ROOT "$defaultAvi\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_CLASSES_ROOT "$defaultAvi\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_CLASSES_ROOT "$defaultAvi\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'
  ;IN HKCU
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultAvi\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultAvi\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultAvi\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'
  
  DetailPrint "Writing registry keys for MP4 context menu"
  ;for mp4
  WriteRegStr HKEY_CLASSES_ROOT "$defaultMp4\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_CLASSES_ROOT "$defaultMp4\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_CLASSES_ROOT "$defaultMp4\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'
  ;IN HKCU
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultMp4\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultMp4\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultMp4\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'
  
  DetailPrint "Writing registry keys for MKV context menu"
  ;for mkv
  WriteRegStr HKEY_CLASSES_ROOT "$defaultMkv\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_CLASSES_ROOT "$defaultMkv\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_CLASSES_ROOT "$defaultMkv\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'
  ;IN HKCU
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultMkv\shell\Subloader" "" "Find subtitles"
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultMkv\shell\Subloader" "Icon" '"$INSTDIR\SubLoad.exe"'
  WriteRegStr HKEY_LOCAL_MACHINE "Software\Classes\$defaultMkv\shell\Subloader\command" "" '"$INSTDIR\SubLoad.exe" "%1"'
  
  ;memorize for deletion
  WriteRegStr HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "aviCR" "$defaultAvi"
  WriteRegStr HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "mp4CR" "$defaultMp4"
  WriteRegStr HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "mkvCR" "$defaultMkv"
  
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
	
	ReadRegStr $crVar HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "aviCR"
	DetailPrint "Deleting registry key: HKCR\$crVar\shell\Subloader"
	DeleteRegKey HKEY_CLASSES_ROOT "$crVar\shell\Subloader"
	DetailPrint "Deleting registry key: HKLM\Software\Classes\$crVar\shell\Subloader"
	DeleteRegKey HKEY_LOCAL_MACHINE "Software\Classes\$crVar\shell\Subloader"
	
	ReadRegStr $crVar HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "mp4CR"
	DetailPrint "Deleting registry key: HKCR\$crVar\shell\Subloader"
	DeleteRegKey HKEY_CLASSES_ROOT "$crVar\shell\Subloader"
	DetailPrint "Deleting registry key: HKLM\Software\Classes\$crVar\shell\Subloader"
	DeleteRegKey HKEY_LOCAL_MACHINE "Software\Classes\$crVar\shell\Subloader"
	
	ReadRegStr $crVar HKEY_LOCAL_MACHINE "SOFTWARE\Subloader" "mkvCR"
	DetailPrint "Deleting registry key: HKLM\$crVar\shell\Subloader"
	DeleteRegKey HKEY_CLASSES_ROOT "$crVar\shell\Subloader"
	DetailPrint "Deleting registry key: HKLM\Software\Classes\$crVar\shell\Subloader"
	DeleteRegKey HKEY_LOCAL_MACHINE "Software\Classes\$crVar\shell\Subloader"
	 
	;Delete Start Menu Shortcuts
	Delete "$SMPROGRAMS\Subloader\*.*"
	RmDir  "$SMPROGRAMS\Subloader"

	;Delete Uninstaller And Unistall Registry Entries
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Subloader"
	DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader"
 
SectionEnd