@echo off

echo.
echo Accord.NET Framework - all project configurations builder
echo =========================================================
echo. 

:: Using devenv.com instead of .exe makes the console window wait until the completion
set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio\Preview\Professional\Common7\IDE\devenv.com"
set DEVVER=2017 Professional Preview
if not exist %DEVENV% (
	set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.com"
	set DEVVER=2017 Community
	if not exist %DEVENV% (
		set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.com"
		set DEVVER=2015
		if not exist %DEVENV% (
			set DEVENV="C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.com"
			set DEVVER=2013
			if not exist %DEVENV% (
				echo "Error: Could not find Visual Studio's devenv.com"
				exit
			)
		)
	)
) 

echo This Windows batch file will use Visual Studio %DEVVER% 
echo to compile the Debug and Release versions of the framework.
echo. 

del /q "bin\*.log"
echo.

echo.
echo  - Building Debug configuration...
%DEVENV% Sources\Accord.NET.sln /Build "Debug|x64" /out "Setup\bin\Build.Debug.x64.log"
%DEVENV% Sources\Accord.NET.sln /Build "Debug|Any CPU" /out "Setup\bin\Build.Debug.Any.log"
timeout /T 10
echo.
echo  - Building Mono configuration...
%DEVENV% Sources\Accord.NET.sln /Build "Mono|Any CPU" /out "Setup\bin\Build.Mono.log"
timeout /T 10
echo.
echo  - Building NET35 configuration...
%DEVENV% Sources\Accord.NET.sln /Build "net35|x64" /out "Setup\bin\Build.net35.x64.log"
%DEVENV% Sources\Accord.NET.sln /Build "net35|Any CPU" /out "Setup\bin\Build.net35.Any.log"
timeout /T 10
echo.
echo  - Building NET40 configuration...
%DEVENV% Sources\Accord.NET.sln /Build "net40|x64" /out "Setup\bin\Build.net40.x64.log"
%DEVENV% Sources\Accord.NET.sln /Build "net40|Any CPU" /out "Setup\bin\Build.net40.Any.log"
timeout /T 10
echo.
echo  - Building NET45 configuration...
%DEVENV% Sources\Accord.NET.sln /Build "net45|x64" /out "Setup\bin\Build.net45.x64.log"
%DEVENV% Sources\Accord.NET.sln /Build "net45|Any CPU" /out "Setup\bin\Build.net45.Any.log"
timeout /T 10
echo.
echo  - Building NET46 configuration...
%DEVENV% Sources\Accord.NET.sln /Build "net46|x64" /out "Setup\bin\Build.net46.x64.log"
%DEVENV% Sources\Accord.NET.sln /Build "net46|Any CPU" /out "Setup\bin\Build.net46.Any.log"
timeout /T 10
echo.
echo  - Building samples...
%DEVENV% Samples\Samples.sln /Build Release /out "Setup\bin\Build.Samples.log"
timeout /T 10
echo.
timeout /T 10