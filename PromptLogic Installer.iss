; ------------------------------------------------------------
; PromptLogic Installer Script
; Version: 0.2
; Requires: .NET 9 Desktop Runtime
; INNO Setup Compiler Version 6.7.0
; ------------------------------------------------------------

[Setup]
AppName=PromptLogic
AppVersion=0.2
AppPublisher=Art LeDuc
DefaultDirName={autopf}\PromptLogic
DefaultGroupName=PromptLogic
OutputDir=.\installer
OutputBaseFilename=PromptLogic-0.2-Setup
Compression=lzma
SolidCompression=yes
DisableProgramGroupPage=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

; Require admin for proper installation
PrivilegesRequired=admin

; ------------------------------------------------------------
; .NET 9 Desktop Runtime Check
; ------------------------------------------------------------

[Code]
function IsDotNet9DesktopInstalled(): Boolean;
var
  keyPath: string;
  subkeys: TArrayOfString;
  i: Integer;
  desktopPath: string;
  FindRec: TFindRec;
begin
  Result := False;

  // --- 1. Registry check ---
  keyPath :=
    'SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedfx\Microsoft.WindowsDesktop.App';

  if RegGetSubkeyNames(HKLM, keyPath, subkeys) then
  begin
    for i := 0 to GetArrayLength(subkeys) - 1 do
    begin
      if Pos('9.', subkeys[i]) = 1 then
      begin
        Result := True;
        Exit;
      end;
    end;
  end;

  // --- 2. Filesystem fallback ---
  desktopPath := ExpandConstant('{pf}\dotnet\shared\Microsoft.WindowsDesktop.App');

  if DirExists(desktopPath) then
  begin
    // Use TFindRec, not a string array
    if FindFirst(desktopPath + '\9.*', FindRec) then
    begin
      Result := True;
      FindClose(FindRec);
      Exit;
    end;
  end;
end;

function InitializeSetup(): Boolean;
begin
  if not IsDotNet9DesktopInstalled() then
  begin
    MsgBox(
      'PromptLogic requires the .NET 9 Desktop Runtime.'#13#13 +
      'Please install it and run this installer again.',
      mbError, MB_OK);
    Result := False;
  end
  else
    Result := True;
end;

end.

; ------------------------------------------------------------
; Files
; ------------------------------------------------------------
[Files]

Source: "bin\Release\net9.0-windows\PromptLogic.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "bin\Release\net9.0-windows\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "docs\*"; DestDir: "{app}\docs"; Flags: ignoreversion recursesubdirs createallsubdirs

; ------------------------------------------------------------
; Shortcuts
; ------------------------------------------------------------

[Icons]
Name: "{group}\PromptLogic"; Filename: "{app}\PromptLogic.exe"
Name: "{commondesktop}\PromptLogic"; Filename: "{app}\PromptLogic.exe"; Tasks: desktopicon

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"

; ------------------------------------------------------------
; Uninstall
; ------------------------------------------------------------

[UninstallDelete]
Type: filesandordirs; Name: "{app}"
