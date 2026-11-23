#define AppName "Insight Hot Key"
#define AppVersion "1.0.0"
#define AppPublisher "INSIGHT Software"
#define AppURL "https://github.com/insight-software/insight-hot-key"
#define AppExeName "InsightHotKey.exe"

[Setup]
; Unique GUID for Insight Hot Key
AppId={{8B1A2C3D-4E5F-6789-ABCD-EF0123456789}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={autopf}\INSIGHT Software\{#AppName}
DefaultGroupName={#AppName}
AllowNoIcons=yes
LicenseFile=embedded\License.rtf
SetupIconFile=embedded\App.ico
WizardImageFile=embedded\WizardImage0.bmp
WizardSmallImageFile=embedded\WizardSmallImage0.bmp
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
OutputDir=output
OutputBaseFilename=InsightHotKey_Setup_v{#AppVersion}_x64
ArchitecturesAllowed=x64os
ArchitecturesInstallIn64BitMode=x64os
UninstallDisplayIcon={app}\{#AppExeName}
PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startupicon"; Description: "Launch Insight Hot Key automatically when Windows starts"; GroupDescription: "Startup options:"; Flags: unchecked

[Files]
Source: "Portable\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "embedded\License.txt"; DestDir: "{app}"; DestName: "LICENSE.txt"; Flags: ignoreversion
Source: "embedded\License.rtf"; DestDir: "{app}"; DestName: "LICENSE.rtf"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{group}\{cm:UninstallProgram,{#AppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon
Name: "{autostartup}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: startupicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
