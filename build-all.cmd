@echo off

echo.
echo Accord.NET Framework - all project configurations builder
echo =========================================================
echo. 
echo This Windows batch file will use Visual Studio 2015 to
echo compile the Debug and Release versions of the framework.
echo. 

:: Using devenv.com instead of .exe makes the console window wait until the completion
:: set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.com"
set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.com"
:: set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.com"

del /q "bin\*.log"
echo.

echo.
echo  - Building Debug configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild Debug /out "Setup\bin\Build.Debug.log"
timeout /T 30
echo.
echo  - Building Mono configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild MONO /out "Setup\bin\Build.MONO.log"
timeout /T 30
echo.
echo  - Building NET35 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET35 /out "Setup\bin\Build.NET35.log"
timeout /T 30
echo.
echo  - Building NET40 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET40 /out "Setup\bin\Build.NET40.log"
timeout /T 30
echo.
echo  - Building NET45 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET45 /out "Setup\bin\Build.NET45.log"
timeout /T 30
echo.
echo  - Building NET46 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET46 /out "Setup\bin\Build.NET46.log"
timeout /T 30
echo.
echo  - Building samples...
%DEVENV% Samples\Samples.sln /Rebuild Release /out "Setup\bin\Build.Samples.log"
timeout /T 30
echo.
timeout /T 10