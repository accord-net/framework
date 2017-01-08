@echo off

:: Automated builder for executable installer,
:: compressed archive and NuGet packages

echo.
echo =========================================================


:: Update version numbers
cd Scripts
cmd /c "update-version.cmd"
cd ..

:: Compile sources
cd ..
cmd /c "build-all.cmd"
cd Setup

:: Build compressed archive
cd Archiver
cmd /c "package-framework.cmd"
cmd /c "package-samples.cmd"
cd ..

echo.
echo.

:: Build executable installer [disabled since v3.3.0]
:: cd Installer
:: cmd /c Build.cmd
:: cd ..
:: 
:: echo.
:: echo.

:: Build NuGet packages
cd NuGet
cmd /c "create-packages.cmd"
cd ..

echo.
echo.

pause