@echo off

set samples=C:\Projects\Accord.NET\framework\Samples
set notepad="C:\Program Files (x86)\Notepad++\notepad++.exe"
echo.

echo %samples%
for /r "%samples%" %%F in (*.csproj) do (
  echo %%F
  %notepad% %%F
)

