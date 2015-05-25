@echo off

:: Build NuGet packages
cd NuGet
cmd /c Push.cmd
cd ..

echo.
echo.

pause