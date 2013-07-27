@echo off

:: Make sure the nuget executable is writable
attrib -R NuGet.exe

echo.
echo Updating NuGet...
cmd /c nuget.exe update -Self

:eof