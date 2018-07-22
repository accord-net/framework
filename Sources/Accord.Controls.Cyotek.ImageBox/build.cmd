@ECHO OFF

SETLOCAL

CALL ..\..\..\build\set35vars.bat

%msbuildexe% Cyotek.Windows.Forms.ImageBox.sln /p:Configuration=Release /verbosity:minimal /nologo /t:Clean,Build
CALL dualsigncmd Cyotek.Windows.Forms.ImageBox\bin\Release\Cyotek.Windows.Forms.ImageBox.dll

PUSHD
IF NOT EXIST nuget MKDIR nuget
CD nuget
%nugetexe% pack ..\Cyotek.Windows.Forms.ImageBox\Cyotek.Windows.Forms.ImageBox.csproj -Prop Configuration=Release
%zipexe% a -bd -tZip  Cyotek.Windows.Forms.ImageBox.x.x.x.x.zip ..\Cyotek.Windows.Forms.ImageBox\bin\Release\Cyotek.Windows.Forms.ImageBox.*
POPD

ENDLOCAL
