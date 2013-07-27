@echo off

echo.
echo Accord.NET Framework executable installer builder
echo =========================================================
echo. 
echo This Windows batch file uses InnoSetup to automatically
echo build the executable installer version of the framework.
echo. 

timeout /T 5

"C:\Program Files (x86)\Inno Setup 5\Compil32.exe" /cc Accord.iss

