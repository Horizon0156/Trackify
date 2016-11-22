; The name of the installer 
Name "Booking Helper"

; The file to write
OutFile "BookingHelper0.8_Setup.exe"

; The default installation directory
InstallDir "$PROGRAMFILES\Booking Helper"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\BookingHelper" "Install_Dir"

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "Booking Helper (required)"

  SectionIn RO
  
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Put files there
  File "bin\Release\BookingHelper.exe"
  File "bin\Release\*.dll"
  File /r "bin\Release\x64"
  File /r "bin\Release\x86"
  
  ; Write the installation path into the registry
  WriteRegStr HKLM "Software\BookingHelper" "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BookingHelper" "DisplayName" "Booking Helper"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BookingHelper" "DisplayVersion" "0.8"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BookingHelper" "Publisher" "Horizon777"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BookingHelper" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BookingHelper" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BookingHelper" "NoRepair" 1
  WriteUninstaller "Uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\Booking Helper"
  CreateShortCut "$SMPROGRAMS\Booking Helper\Uninstall.lnk" "$INSTDIR\Uninstall.exe" "" "$INSTDIR\Uninstall.exe" 0
  CreateShortCut "$SMPROGRAMS\Booking Helper\Booking Helper.lnk" "$INSTDIR\BookingHelper.exe" "" "$INSTDIR\BookingHelper.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\BookingHelper"
  DeleteRegKey HKLM SOFTWARE\BookingHelper

  ; Remove files and uninstaller
  Delete "$INSTDIR\*.*"
  Delete "$INSTDIR\x64\*.*"
  Delete "$INSTDIR\x86\*.*"

  ; Remove shortcuts, if any
  Delete "$SMPROGRAMS\Booking Helper\*.*"

  ; Remove directories
  RMDir "$SMPROGRAMS\Booking Helper"
  RMDir "$INSTDIR\x64"
  RMDir "$INSTDIR\x86"
  RMDir "$INSTDIR"

SectionEnd
