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
%DEVENV% Sources\Accord.NET.sln /Rebuild "Debug|x64" /out "Setup\bin\Build.Debug.x64.log"
%DEVENV% Sources\Accord.NET.sln /Rebuild "Debug|Any CPU" /out "Setup\bin\Build.Debug.Any.log"
timeout /T 10
echo.
echo  - Building Mono configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild "Mono|Any CPU" /out "Setup\bin\Build.Mono.log"
timeout /T 10
echo.
echo  - Building NET35 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild "net35|x64" /out "Setup\bin\Build.net35.x64.log"
%DEVENV% Sources\Accord.NET.sln /Rebuild "net35|Any CPU" /out "Setup\bin\Build.net35.Any.log"
timeout /T 10
echo.
echo  - Building NET40 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild "net40|x64" /out "Setup\bin\Build.net40.x64.log"
%DEVENV% Sources\Accord.NET.sln /Rebuild "net40|Any CPU" /out "Setup\bin\Build.net40.Any.log"
timeout /T 10
echo.
echo  - Building NET45 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild "net45|x64" /out "Setup\bin\Build.net45.x64.log"
%DEVENV% Sources\Accord.NET.sln /Rebuild "net45|Any CPU" /out "Setup\bin\Build.net45.Any.log"
timeout /T 10
echo.
echo  - Building NET46 configuration...
%DEVENV% Sources\Accord.NET.sln /Rebuild "net46|x64" /out "Setup\bin\Build.net46.x64.log"
%DEVENV% Sources\Accord.NET.sln /Rebuild "net46|Any CPU" /out "Setup\bin\Build.net46.Any.log"
timeout /T 10
echo.
echo  - Building samples...
%DEVENV% Samples\Samples.sln /Rebuild Release /out "Setup\bin\Build.Samples.log"
timeout /T 10
echo.
timeout /T 10