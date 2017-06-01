@echo off

echo.
echo Accord.NET Framework - all project configurations builder
echo =========================================================
echo. 

:: Use Microsoft's vswhere.exe to locate the appropriate version of MSBuild:
for /f "usebackq tokens=1* delims=: " %%i in (`Tools\vswhere -latest -requires Microsoft.Component.MSBuild`) do (
  if /i "%%i"=="installationPath" set InstallDir=%%j
)
set MSBUILD="%InstallDir%\MSBuild\15.0\Bin\MSBuild.exe"

echo This Windows batch file will use MSBuild.exe from the path
echo %MSBUILD%
echo to compile the Debug and Release versions of the framework.
echo. 
::pause

del /q "Setup\bin\*.log"

echo.
call:BUILD "Sources\Accord.NET.sln","Debug","x64"
call:BUILD "Sources\Accord.NET.sln","Debug","Any CPU"
call:BUILD "Sources\Accord.NET.sln","Mono","Any CPU"
call:BUILD "Sources\Accord.NET.sln","NET35","x64"
call:BUILD "Sources\Accord.NET.sln","NET35","Any CPU"
call:BUILD "Sources\Accord.NET.sln","NET40","x64"
call:BUILD "Sources\Accord.NET.sln","NET40","Any CPU"
call:BUILD "Sources\Accord.NET.sln","NET45","x64"
call:BUILD "Sources\Accord.NET.sln","NET45","Any CPU"
call:BUILD "Sources\Accord.NET.sln","NET46","x64"
call:BUILD "Sources\Accord.NET.sln","NET46","Any CPU"
call:BUILD "Sources\Accord.NET.sln","NET462","x64"
call:BUILD "Sources\Accord.NET.sln","NET462","Any CPU"
call:BUILD "Sources\Accord.NET (NETStandard).sln","netstandard","Any CPU"
call:BUILD "Samples\Samples.sln","Release","x86"
goto:eof



:BUILD
set SOLUTION=%~1
set CONFIGURATION=%~2
set PLATFORM=%~3
echo.
echo  - Building %SOLUTION% in %CONFIGURATION% / %PLATFORM% configuration...
%MSBUILD% "%SOLUTION%" /t:Rebuild /p:Configuration=%CONFIGURATION% /p:Platform="%PLATFORM%" /fl /flp:logfile="Setup\bin\Build.%CONFIGURATION%.%PLATFORM%.log";verbosity=normal /consoleloggerparameters:ErrorsOnly;Summary /verbosity:minimal /nologo
goto:eof