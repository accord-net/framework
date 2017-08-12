@echo off

:: (enable termination from subroutine)
SETLOCAL
set ERROR_CODE=0
if "%selfWrapped%"=="" (
  set selfWrapped=true
  %ComSpec% /s /c ""%~0" %*"
  goto :eof
)


:: Automated builder for executable installer,
:: compressed archive and NuGet packages

echo.
echo =========================================================

if "%1"=="version" goto version
if "%1"=="build"   goto build
if "%1"=="archive" goto archive
if "%1"=="pack"    goto pack
if "%1"=="push"    goto push
if not "%1" == ""  goto error


:version
:: Update version numbers
cmd /c "update-version.cmd"
echo.
echo.


:build
:: Compile sources
cd ..
cmd /c "build-all.cmd"
if %errorlevel% neq 0 exit /b %errorlevel%
cd Setup
echo.
echo.


:archive
:: Build compressed archive
cd Archiver
cmd /c "package-framework.cmd"
if %errorlevel% neq 0 exit /b %errorlevel%
cmd /c "package-samples.cmd"
if %errorlevel% neq 0 exit /b %errorlevel%
cd ..
echo.
echo.


:: Build executable installer [disabled since v3.3.0]
:: cd Installer
:: cmd /c Build.cmd
:: cd ..
:: echo.
:: echo.


:pack
:: Build NuGet packages
cd NuGet
cmd /c "create-packages.cmd"
if %errorlevel% neq 0 exit /b %errorlevel%
cd ..
echo.
echo.
pause
goto :eof

:error
echo "Unknown command '%1'."
goto :eof

:push
:: push NuGet packages to nuget.com
cd NuGet
cmd /c push-packages.cmd
cd ..
goto :eof