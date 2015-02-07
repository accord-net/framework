@echo off


:: Automated builder for executable installer,
:: compressed archive and NuGet packages


:: Compile sources
cmd /c "Compile.cmd"


:: Build compressed archive
cd Archiver
cmd /c "Build.cmd"
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