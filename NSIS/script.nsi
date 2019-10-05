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
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Subloader" "DisplayName" "Subloader (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Subloader" "UninstallString" "$INSTDIR\Uninstall.exe"
   
  WriteUninstaller "$INSTDIR\Uninstall.exe"
 
SectionEnd

;--------------------------------    
;Uninstaller Section  
Section "Uninstall"

;Delete Files 
RMDir /r "$INSTDIR\*.*"    
 
;Remove the installation directory
RMDir "$INSTDIR"
 
;Delete Start Menu Shortcuts
Delete "$SMPROGRAMS\Subloader\*.*"
RmDir  "$SMPROGRAMS\Subloader"
 
;Delete Uninstaller And Unistall Registry Entries
DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Subloader"
DeleteRegKey HKEY_LOCAL_MACHINE "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Subloader"  
 
SectionEnd
