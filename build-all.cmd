@echo off

echo.
echo Accord.NET Framework - all project configurations builder
echo =========================================================
echo. 
echo This Windows batch file will use Visual Studio 2013 to
echo compile the Debug and Release versions of the framework.
echo. 


set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe"

del /q "bin\*.log"
echo.

echo.
echo  - Building Debug configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild Debug /out "bin\Build.Debug.log"
echo.
echo  - Building Mono configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild MONO /out "bin\Build.MONO.log"
echo.
echo  - Building NET35 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET35 /out "bin\Build.NET35.log"
echo.
echo  - Building NET40 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET40 /out "bin\Build.NET40.log"
echo.
echo  - Building NET45 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET45 /out "bin\Build.NET45.log"
echo.
echo  - Building NET46 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild NET46 /out "bin\Build.NET46.log"
echo.
echo  - Building samples...
%DEVENV% Samples\Samples.sln /Rebuild Release /out "bin\Build.Samples.log"
echo.
timeout /T 5