; The name of the installer 
Name "Trackify"

; The file to write
OutFile "Trackify Setup 1.0.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\Trackify"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Trackify" "Install_Dir"

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "Trackify (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put files there
  File "bin\Production\Trackify.exe"
  File "bin\Production\Trackify.exe.config"
  File "bin\Production\*.dll"
  File /r "bin\Production\x64"
  File /r "bin\Production\x86"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM "Software\Trackify" "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Trackify" "DisplayName" "Trackify"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Trackify" "DisplayVersion" "1.0"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Trackify" "Publisher" "Horizon777"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Trackify" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Trackify" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Trackify" "NoRepair" 1
  WriteUninstaller "Uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Trackify"
  CreateShortCut "$SMPROGRAMS\Trackify\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Trackify\Trackify.lnk" "$INSTDIR\Trackify.exe" "" "$INSTDIR\Trackify.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Trackify"
  DeleteRegKey HKLM SOFTWARE\Trackify

  ; Remove files and uninstaller
  Delete "$INSTDIR\*.*"
  Delete "$INSTDIR\x64\*.*"
  Delete "$INSTDIR\x86\*.*"

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Trackify\*.*"

  ; Remove directories
  RMDir "$SMPROGRAMS\Trackify"
  RMDir "$INSTDIR\x64"
  RMDir "$INSTDIR\x86"
  RMDir "$INSTDIR"

SectionEnd
