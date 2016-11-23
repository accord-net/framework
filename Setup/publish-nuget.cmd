@echo off

:: Build NuGet packages
cd NuGet
cmd /c push-packages.cmd
cd ..

echo.
echo.

pause