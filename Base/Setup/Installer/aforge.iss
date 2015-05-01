; AForge.NET setup project

[Setup]
AppName=AForge.NET Framework
AppVerName=AForge.NET Framework 2.2.5
AppPublisher=AForge.NET
AppPublisherURL=http://www.aforgenet.com/framework/
AppSupportURL=http://www.aforgenet.com/framework/
AppUpdatesURL=http://www.aforgenet.com/framework/
DefaultDirName={pf}\AForge.NET\Framework
DefaultGroupName=AForge.NET\Framework
AllowNoIcons=yes
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes
LicenseFile=license.txt

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Components]
Name: "ext"; Description: "External Dependencies"; Types: full compact custom; Flags: fixed
Name: "libs"; Description: "AForge.NET Framework's libraries"; Types: full compact custom; Flags: fixed
Name: "docs"; Description: "Documentation"; Types: full custom
Name: "sources"; Description: "Sources"; Types: full custom
Name: "samples"; Description: "Samples"; Types: full custom
Name: "tests"; Description: "Unit Tests"; Types: full custom

[Files]
Source: "Files\Copyright.txt"; DestDir: "{app}"; Components: libs
Source: "Files\Release notes.txt"; DestDir: "{app}"; Components: libs
Source: "Files\lgpl-3.0.txt"; DestDir: "{app}"; Components: libs
Source: "Files\gpl-3.0.txt"; DestDir: "{app}"; Components: libs
Source: "Files\License.txt"; DestDir: "{app}"; Components: libs
Source: "Files\Docs\*"; DestDir: "{app}\Docs"; Components: docs
Source: "Files\Externals\*"; DestDir: "{app}\Externals"; Components: ext; Flags: recursesubdirs
Source: "Files\Release\*"; DestDir: "{app}\Release"; Components: libs
Source: "Files\Samples\*"; DestDir: "{app}\Samples"; Components: samples; Flags: recursesubdirs
Source: "Files\Sources\*"; DestDir: "{app}\Sources"; Components: sources; Flags: recursesubdirs
Source: "Files\Tools\*"; DestDir: "{app}\Tools"; Components: sources; Flags: recursesubdirs
Source: "Files\Unit Tests\*"; DestDir: "{app}\Unit Tests"; Components: tests; Flags: recursesubdirs

[Registry]
Root: HKLM; Subkey: "Software\Microsoft\.NETFramework\AssemblyFolders\AForge.NET"; Flags: uninsdeletekey; ValueType: string; ValueData: "{app}\Release"

[Icons]
Name: "{group}\Documentation"; Filename: "{app}\Docs\AForge.NET.chm"
Name: "{group}\Project Home"; Filename: "http://www.aforgenet.com/framework/"
Name: "{group}\Release Notes"; Filename: "{app}\Release notes.txt"

Name: "{group}\{cm:UninstallProgram,AForge.NET Framework}"; Filename: "{uninstallexe}"
