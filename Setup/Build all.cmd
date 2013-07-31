@echo off

:: Automated builder for executable installer,
:: compressed archive and NuGet packages

:: Build compressed archive
cd Archiver
cmd /c "Build (v4.0).cmd"
cmd /c "Build (v3.5).cmd"
cmd /c "Samples.cmd"
cd ..

echo.
echo.

:: Build executable installer
cd Installer
cmd /c Build.cmd
cd ..

echo.
echo.

:: Build NuGet packages
cd NuGet
cmd /c Build.cmd
cd ..

echo.
echo.

pause