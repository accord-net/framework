@echo off

set parent=C:\Projects\Accord.NET\framework\Sources
echo.
echo %parent%

for /r %parent% %%F in (*.csproj) do (
  echo %%F
  type "%%F"|repl "<OutputPath>..\..\Release\" "<OutputPath>$(SolutionDir)..\Release\" LI >"%%F.new"
  move /y "%%F.new" "%%F" >nul
)