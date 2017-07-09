@echo off

:: (enable termination from subroutine)
SETLOCAL
set ERROR_CODE=0
if "%selfWrapped%"=="" (
  set selfWrapped=true
  %ComSpec% /s /c ""%~0" %*"
  goto :eof
)


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
call:MSBUILD "Sources\Accord.NET.sln", "Debug",   "x64",     "Rebuild"
call:MSBUILD "Sources\Accord.NET.sln", "Debug",   "Any CPU", "Build"
call:MSBUILD "Sources\Accord.NET.sln", "mono",    "Any CPU", "Rebuild"
call:MSBUILD "Sources\Accord.NET.sln", "net35",   "x64",     "Rebuild"
call:MSBUILD "Sources\Accord.NET.sln", "net35",   "Any CPU", "Build"
call:MSBUILD "Sources\Accord.NET.sln", "net40",   "x64",     "Rebuild"
call:MSBUILD "Sources\Accord.NET.sln", "net40",   "Any CPU", "Build"
call:MSBUILD "Sources\Accord.NET.sln", "net45",   "x64",     "Rebuild"
call:MSBUILD "Sources\Accord.NET.sln", "net45",   "Any CPU", "Build"
call:MSBUILD "Sources\Accord.NET.sln", "net46",   "x64",     "Rebuild"
call:MSBUILD "Sources\Accord.NET.sln", "net46",   "Any CPU", "Build"
call:MSBUILD "Sources\Accord.NET.sln", "net462",  "x64",     "Rebuild"
call:MSBUILD "Sources\Accord.NET.sln", "net462",  "Any CPU", "Build"
call:MSBUILD "Samples\Samples.sln",    "Release", "x86",     "Rebuild"

::: Building netstandard2.0 packages from the command line still doesn't work very well:
::call:DNBUILD "Sources\Accord.NET (NETStandard).sln","netstandard2.0"
::call:MSBUILD "Sources\Accord.NET (NETStandard).sln","netstandard2.0","Any CPU",Rebuild

exit /b %ERROR_CODE%
goto:eof


:DNBUILD
set SOLUTION=%~1
set CONFIGURATION=%~2
dotnet build %SOLUTION% --configuration %CONFIGURATION% --no-incremental
goto:eof


:MSBUILD
set SOLUTION=%~1
set CONFIGURATION=%~2
set PLATFORM=%~3
set TASK=%~4
echo.
echo  - Building %SOLUTION% in %CONFIGURATION% / %PLATFORM% configuration...
%MSBUILD% /m "%SOLUTION%" /t:%TASK% /property:Prefer32bit=false /p:Configuration=%CONFIGURATION% /p:Platform="%PLATFORM%" /fl /flp:logfile="Setup\bin\Build.%CONFIGURATION%.%PLATFORM%.log";verbosity=normal /consoleloggerparameters:ErrorsOnly;Summary /verbosity:minimal /nologo
set ERROR_CODE=%errorlevel%
if %ERROR_CODE% neq 0 (
	echo Exiting with %ERROR_CODE%
	exit /b %ERROR_CODE%
)
goto:eof

