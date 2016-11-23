@echo off

echo.
echo Accord.NET Framework executable installer builder
echo =========================================================
echo. 
echo This Windows batch file uses InnoSetup to automatically
echo build the executable installer version of the framework.
echo. 

call ..\version.cmd

set inno="C:\Program Files (x86)\Inno Setup 5\Compil32.exe"

rem set rar="C:\Program Files\WinRAR\rar"
rem set opts=a -m0 -s

timeout /T 5

%inno% /cc Accord.iss
rem %rar% %opts% ..\bin\Accord.NET-%version%-installer.rar ..\bin\Accord.NET-%version%-installer.exe