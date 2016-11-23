@echo off

set sources="C:\Projects\Accord.NET\framework\Sources"
set unitest="C:\Projects\Accord.NET\framework\Unit Tests"
set notepad="C:\Program Files (x86)\Notepad++\notepad++.exe"
echo.

echo %sources%
for /r %sources% %%F in (*.csproj) do (
  echo %%F
  %notepad% %%F
)

echo %unitest%
for /r %unitest% %%F in (*.csproj) do (
  echo %%F
  %notepad% %%F
)