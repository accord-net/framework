@echo off

set sources=C:\Projects\Accord.NET\framework\Sources
set unitest=C:\Projects\Accord.NET\framework\Unit Tests
set notepad="C:\Program Files (x86)\Notepad++\notepad++.exe"
echo.

%notepad% "%sources%\Accord.NET.sln"
%notepad% "%sources%\Accord.NET.targets"
%notepad% "%unitest%\Accord.Tests.targets"

echo %sources%
for /r "%sources%" %%F in (*.csproj) do (
  echo %%F
  %notepad% %%F
)

for /r "%sources%" %%F in (*.vcxproj) do (
  echo %%F
  %notepad% %%F
)

echo "%unitest%"
for /r "%unitest%" %%F in (*.csproj) do (
  echo %%F
  %notepad% %%F
)

echo "%unitest%"
for /r "%unitest%" %%F in (*.vcxproj) do (
  echo %%F
  %notepad% %%F
)
