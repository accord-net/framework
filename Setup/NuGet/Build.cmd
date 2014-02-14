@echo off

echo.
echo Accord.NET Framework NuGet packages builder
echo =========================================================
echo. 
echo This Windows batch file uses NuGet to automatically
echo build the NuGet packages versions of the framework.
echo. 

timeout /T 5

:: Set version info
set version=2.12.0.0
set output=..\bin\nupkg

:: Create output directory
IF NOT EXIST %output%\nul (
    mkdir %output%
)

:: Remove old files
forfiles /p %output% /m *.nupkg /c "cmd /c del @file"


echo.
echo Creating packages...

forfiles /m *.nuspec /c "cmd /c nuget.exe pack @File -Version %version% -OutputDirectory %output%"

:eof