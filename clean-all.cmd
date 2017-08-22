@echo off

echo.
echo Accord.NET Framework - all project configurations cleaner
echo =========================================================
echo. 
echo This Windows batch file will use Visual Studio 2015 to
echo compile the Debug and Release versions of the framework.
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

del /q "bin\*.log"
echo.

echo.
echo  - Cleaning Debug configuration...
%DEVENV% Sources\Accord.NET.sln /Clean "Debug|x64" /out "Setup\bin\Build.Debug.x64.log"
%DEVENV% Sources\Accord.NET.sln /Clean "Debug|Any CPU" /out "Setup\bin\Build.Debug.Any.log"
timeout /T 10
echo.
echo  - Cleaning Mono configuration...
%DEVENV% Sources\Accord.NET.sln /Clean "Mono|Any CPU" /out "Setup\bin\Build.Mono.log"
timeout /T 10
echo.
echo  - Cleaning NET35 configuration...
%DEVENV% Sources\Accord.NET.sln /Clean "net35|x64" /out "Setup\bin\Build.net35.x64.log"
%DEVENV% Sources\Accord.NET.sln /Clean "net35|Any CPU" /out "Setup\bin\Build.net35.Any.log"
timeout /T 10
echo.
echo  - Cleaning NET40 configuration...
%DEVENV% Sources\Accord.NET.sln /Clean "net40|x64" /out "Setup\bin\Build.net40.x64.log"
%DEVENV% Sources\Accord.NET.sln /Clean "net40|Any CPU" /out "Setup\bin\Build.net40.Any.log"
timeout /T 10
echo.
echo  - Cleaning NET45 configuration...
%DEVENV% Sources\Accord.NET.sln /Clean "net45|x64" /out "Setup\bin\Build.net45.x64.log"
%DEVENV% Sources\Accord.NET.sln /Clean "net45|Any CPU" /out "Setup\bin\Build.net45.Any.log"
timeout /T 10
echo.
echo  - Cleaning NET46 configuration...
%DEVENV% Sources\Accord.NET.sln /Clean "net46|x64" /out "Setup\bin\Build.net46.x64.log"
%DEVENV% Sources\Accord.NET.sln /Clean "net46|Any CPU" /out "Setup\bin\Build.net46.Any.log"
timeout /T 10
echo.
echo  - Cleaning NETSTANDARD configuration...
%DEVENV% "Sources\Accord.NET (NETStandard).sln" /Clean "netstandard|Any CPU" /out "Setup\bin\Build.netstandard.Any.log"
timeout /T 10
echo.
echo  - Cleaning samples...
%DEVENV% Samples\Samples.sln /Clean Release /out "Setup\bin\Build.Samples.log"
timeout /T 10
echo.
timeout /T 10