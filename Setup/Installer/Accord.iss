; Accord.NET setup project

#define VERSION GetStringFileInfo("..\..\Release\net45\Accord.dll", "ProductVersion")
#pragma message "Creating package for Accord.NET " + VERSION
[Setup]
AppName=Accord.NET Framework
AppVersion={#VERSION}
AppVerName=Accord.NET Framework {#VERSION}
AppPublisher=Accord.NET
AppPublisherURL=http://accord-framework.net
AppSupportURL=http://accord-framework.net
AppUpdatesURL=http://accord-framework.net
AppCopyright=Copyright © Accord.NET authors, 2009-2017
VersionInfoVersion={#VERSION}
DefaultDirName={pf}\Accord.NET\Framework
DefaultGroupName=Accord.NET\Framework
AllowNoIcons=yes
OutputBaseFilename=Accord.NET-{#VERSION}-installer
OutputDir=..\bin

Compression=lzma2/ultra64
SolidCompression=yes
LicenseFile=..\..\License.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Types]
Name: "default"; Description: "Recommended settings"
Name: "compact"; Description: "Compact installation"
Name: "custom"; Description: "Advanced installation (see warning)"; Flags: iscustom


[Components]
Name: "libs";     Description: "Accord.NET Framework's libraries";  Types: default compact custom; Flags: fixed
Name: "libs/gpl"; Description: "Extra GPL-only extensions";         Types: custom;
Name: "libs/noc"; Description: "Noncommercial extensions";          Types: custom;
Name: "docs";     Description: "Documentation";                     Types: default custom
Name: "sources";  Description: "Sources";                           Types: default custom
Name: "samples";  Description: "Samples";                           Types: default custom

[Code]
function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if (CurPageID = wpSelectComponents) and  
    (IsComponentSelected('libs/gpl') or IsComponentSelected('libs/noc')) then
  begin
    Result := MsgBox('You have chosen to install the GPL-only or noncommercial-only library extensions. ' +
      'Please make sure you understand the implications of using those modules and accept their license ' +
      'before you include them in your project. Are you sure you want to continue?', mbConfirmation, MB_YESNO) = IDYES;
  end;
end;

[Files]
Source: "..\..\Copyright.txt";       DestDir: "{app}";               Components: libs
Source: "..\..\License.txt";         DestDir: "{app}";               Components: libs
Source: "..\..\Release notes.txt";   DestDir: "{app}";               Components: libs
Source: "..\..\Contributors.txt";    DestDir: "{app}";               Components: libs
Source: "..\..\Docs\*.chm";          DestDir: "{app}\Docs";          Components: docs;     Flags: skipifsourcedoesntexist; Excludes: "*.~*"
Source: "..\..\Sources\*";           DestDir: "{app}\Sources";       Components: sources;  Flags: recursesubdirs; Excludes: "*.~*,\TestResults,*\bin,*\obj,*.sdf,*.suo,*.user,*.vsp,*.shfbproj_*,*.pidb"
Source: "..\..\Unit Tests\*";        DestDir: "{app}\Unit Tests";    Components: sources;  Flags: recursesubdirs; Excludes: "*.~*,\TestResults,*\bin,*\obj,*.sdf,*.suo,*.user,*.vsp,*.shfbproj_*,*.pidb"
Source: "..\..\Samples\*";           DestDir: "{app}\Samples";       Components: samples;  Flags: recursesubdirs; Excludes: "*.~*,*\obj,*\bin\x64\,*\bin\Debug,*\bin\Release,*\bin\x86\Debug,*\bin\x86\Release 3.5,*.pdb,*.user,*.pidb,\packages"
Source: "..\..\Setup\*";             DestDir: "{app}\Setup";         Components: sources;  Flags: recursesubdirs; Excludes: "\bin,\obj,*.user,*.vsp"
Source: "..\..\Externals\*";         DestDir: "{app}\Externals";     Components: libs;     Flags: recursesubdirs; Excludes: "*.~*,*.pdb"

; Official, supported release:
Source: "..\..\Release\net45\*";     DestDir: "{app}\Release\net45"; Components: libs;                            Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml,SlimDX.pdb"
Source: "..\..\Release\net40\*";     DestDir: "{app}\Release\net40"; Components: libs;                            Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml,SlimDX.pdb"
Source: "..\..\Release\net35\*";     DestDir: "{app}\Release\net35"; Components: libs;                            Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml,SlimDX.pdb"

; Extra GPL-only libraries:
Source: "..\..\Release\net45\GPL\*"; DestDir: "{app}\Release\net45"; Components: libs/gpl; Flags: recursesubdirs; Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml"
Source: "..\..\Release\net40\GPL\*"; DestDir: "{app}\Release\net40"; Components: libs/gpl; Flags: recursesubdirs; Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml"
Source: "..\..\Release\net35\GPL\*"; DestDir: "{app}\Release\net35"; Components: libs/gpl; Flags: recursesubdirs; Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml"

; Extra Noncommercial libraries:
Source: "..\..\Release\net45\Noncommercial\*"; DestDir: "{app}\Release\net45\Noncommercial\"; Components: libs/noc; Flags: recursesubdirs; Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml"
Source: "..\..\Release\net40\Noncommercial\*"; DestDir: "{app}\Release\net40\Noncommercial\"; Components: libs/noc; Flags: recursesubdirs; Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml"
Source: "..\..\Release\net35\Noncommercial\*"; DestDir: "{app}\Release\net35\Noncommercial\"; Components: libs/noc; Flags: recursesubdirs; Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml"


[Registry]
Root: HKLM; Subkey: "Software\Microsoft\.NETFramework\v3.5\AssemblyFoldersEx\Accord.NET"; Flags: uninsdeletekey; ValueType: string; ValueData: "{app}\Release\net35"
Root: HKLM; Subkey: "Software\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Accord.NET"; Flags: uninsdeletekey; ValueType: string; ValueData: "{app}\Release\net40"
Root: HKLM; Subkey: "Software\Wow6432Node\Microsoft\.NETFramework\v3.5\AssemblyFoldersEx\Accord.NET"; Flags: uninsdeletekey; ValueType: string; ValueData: "{app}\Release\net35"
Root: HKLM; Subkey: "Software\Wow6432Node\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Accord.NET"; Flags: uninsdeletekey; ValueType: string; ValueData: "{app}\Release\net40"


[Icons]
Name: "{group}\Documentation"; Filename: "{app}\Docs\Accord.NET.chm"
Name: "{group}\Project Home"; Filename: "http://accord-framework.net"
Name: "{group}\Samples"; Filename: "{app}\Samples\"; Components: samples
Name: "{group}\Release Notes"; Filename: "{app}\Release notes.txt"
Name: "{group}\{cm:UninstallProgram,Accord.NET Framework}"; Filename: "{uninstallexe}"
