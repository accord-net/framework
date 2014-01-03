; Accord.NET setup project

#define VERSION GetStringFileInfo("..\..\Release\Accord.dll", "ProductVersion")
#pragma message "Creating package for Accord.NET " + VERSION
[Setup]
AppName=Accord.NET Framework
AppVersion={#VERSION}
AppVerName=Accord.NET Framework {#VERSION}
AppPublisher=Accord.NET
AppPublisherURL=http://accord-framework.net
AppSupportURL=http://accord-framework.net
AppUpdatesURL=http://accord-framework.net
AppCopyright=Copyright © César Souza, 2009-2014
VersionInfoVersion={#VERSION}
DefaultDirName={pf}\Accord.NET\Framework
DefaultGroupName=Accord.NET\Framework
AllowNoIcons=yes
OutputBaseFilename=Accord.NET Framework-{#VERSION}
OutputDir=..\bin

Compression=lzma2/ultra64
SolidCompression=yes
LicenseFile=..\..\License.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Components]
Name: "libs";     Description: "Accord.NET Framework's libraries";  Types: full compact custom; Flags: fixed
Name: "libs/gpl"; Description: "Extra GPL-only libraries (SMOreg)"; Types: full custom;
Name: "docs";     Description: "Documentation";                     Types: full custom
Name: "sources";  Description: "Sources";                           Types: full custom
Name: "samples";  Description: "Samples";                           Types: full custom


[Files]
Source: "..\..\Copyright.txt";     DestDir: "{app}";           Components: libs
Source: "..\..\License.txt";       DestDir: "{app}";           Components: libs
Source: "..\..\Release notes.txt"; DestDir: "{app}";           Components: libs
Source: "..\..\Release\*";         DestDir: "{app}\Release";   Components: libs;                            Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml,SlimDX.pdb"
Source: "..\..\Release\GPL\*";     DestDir: "{app}\Release";   Components: libs/gpl; Flags: recursesubdirs; Excludes: "*.~*,*.lastcodeanalysissucceeded,*.CodeAnalysisLog.xml"
Source: "..\..\Docs\*.chm";        DestDir: "{app}\Docs";      Components: docs;     Flags: skipifsourcedoesntexist; Excludes: "*.~*"
Source: "..\..\Sources\*";         DestDir: "{app}\Sources";   Components: sources;  Flags: recursesubdirs; Excludes: "*.~*,\TestResults,*\bin,*\obj,*.sdf,*.suo,*.user,*.vsp,*.shfbproj_*,*.pidb"
Source: "..\..\Samples\*";         DestDir: "{app}\Samples";   Components: samples;  Flags: recursesubdirs; Excludes: "*.~*,*\obj,*\bin\x64\,*\bin\Debug,*\bin\Release,*\bin\x86\Debug,*\bin\x86\Release 3.5,*.pdb,*.user,*.pidb"
Source: "..\..\Setup\*";           DestDir: "{app}\Setup";     Components: sources;  Flags: recursesubdirs; Excludes: "\bin,\obj,*.user,*.vsp"
Source: "..\..\Externals\*";       DestDir: "{app}\Externals"; Components: libs;     Flags: recursesubdirs; Excludes: "*.~*,*.pdb"


[Registry]
Root: HKLM; Subkey: "Software\Microsoft\.NETFramework\AssemblyFolders\Accord.NET"; Flags: uninsdeletekey; ValueType: string; ValueData: "{app}\Release"
Root: HKLM; Subkey: "Software\Microsoft\.NETFramework\v4.0.30319\AssemblyFoldersEx\Accord.NET"; Flags: uninsdeletekey; ValueType: string; ValueData: "{app}\Release"


[Icons]
Name: "{group}\Documentation"; Filename: "{app}\Docs\Accord.NET.chm"
Name: "{group}\Project Home"; Filename: "http://accord-framework.net"
Name: "{group}\Samples"; Filename: "{app}\Samples\"; Components: samples
Name: "{group}\Release Notes"; Filename: "{app}\Release notes.txt"
Name: "{group}\{cm:UninstallProgram,Accord.NET Framework}"; Filename: "{uninstallexe}"
