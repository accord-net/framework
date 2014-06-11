@echo off

set parent=D:\Projects\Accord.NET\GitHub\framework\Sources
echo.
echo %parent%

for /r %parent% %%F in (*.csproj) do (
  echo %%F
  type "%%F"|repl "Release 3.5|AnyCPU" "Release (3.5)|AnyCPU" LI >"%%F.new"
  move /y "%%F.new" "%%F" >nul
)