@echo off

set sources=C:\Projects\Accord.NET\framework\Sources
set unittest_extras=C:\Projects\Accord.NET\framework\Sources\Extras
set unitest=C:\Projects\Accord.NET\framework\Unit Tests
set notepad="C:\Program Files (x86)\Notepad++\notepad++.exe"
echo.

%notepad% "%unitest%\Accord.Tests.targets"

echo "%unitest%"
for /r "%unitest%" %%F in (Accord.Tests.*.csproj) do (
  echo %%F
  %notepad% %%F
)

echo "%unittest_extras%"
for /r "%unittest_extras%" %%F in (Accord.Tests.*.csproj) do (
  echo %%F
  %notepad% %%F
)

echo "%unitest%"
for /r "%unitest%" %%F in (Accord.Tests.*.vcxproj) do (
  echo %%F
  %notepad% %%F
)
