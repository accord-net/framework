@echo off

set sources=C:\Projects\Accord.NET\framework\Sources
set unitest=C:\Projects\Accord.NET\framework\Unit Tests
set notepad="C:\Program Files (x86)\Notepad++\notepad++.exe"
echo.

%notepad% "%sources%\Accord.NET.sln"
%notepad% "%sources%\Accord.NET.targets"
%notepad% "%unitest%\Accord.Tests.targets"

echo %sources%
for /r "%sources%" %%F in (*Standard^).csproj) do (
  echo %%F
  %notepad% %%F
)

for /r "%sources%" %%F in (*Standard^).vcxproj) do (
  echo %%F
  %notepad% %%F
)

echo "%unitest%"
for /r "%unitest%" %%F in (*Standard^).csproj) do (
  echo %%F
  %notepad% %%F
)

echo "%unitest%"
for /r "%unitest%" %%F in (*Standard^).vcxproj) do (
  echo %%F
  %notepad% %%F
)
