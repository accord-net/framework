@echo off
setlocal enabledelayedexpansion

echo.
echo Accord.NET Framework sample applications archive builder
echo =========================================================
echo. 
echo This Windows batch file will use WinRAR to automatically
echo build the compressed archives of the sample applications.
echo. 


:: Settings for complete and (libs-only) package creation
:: ---------------------------------------------------------
set rar="C:\Program Files\WinRAR\winrar"
set opts=a -m5 -s -afzip
set sampleDir=..\..\Samples\
set outputDir=..\bin\samples\
set sampleBin=\bin\x86\Release\

:: Get absolute sample dir
pushd .
cd %sampleDir%
set sampleDir=%CD%
popd

:: Get absolute output dir
pushd .
:: Create output directory
IF NOT EXIST %outputDir%\nul (
   mkdir %outputDir%
)
cd %outputDir%
set outputDir=%CD%
popd

echo.
echo  - Full sample path: %sampleDir%
echo  - Full output path: %outputDir%
echo.
echo  - WinRAR Command: %rar%
echo  - WinRAR Options: "%opts%"
echo.

timeout /T 10

echo.
echo.
echo Packaging Accord.NET sample applications...
echo ---------------------------------------------------------


:: Remove old files
forfiles /p %outputDir% /m *.zip /c "cmd /c del @file"

:: For each sample application project
for /r %sampleDir% %%f in (*.csproj) do (
   
   :: Create the base filename (Accord-Assembly-ProjectName)
   set cur=%%~dpf
   set fileName=Accord!cur:%sampleDir%=!
   set fileName=!fileName:~0,-1!
   set fileName=!fileName:\=-!
   
   
   :: Convert the filename to lowercase
   for %%c in ("A=a" "B=b" "C=c" "D=d" "E=e" "F=f" "G=g" "H=h" "I=i" "J=j" "K=k" "L=l" "M=m" "N=n" "O=o" "P=p" "Q=q" "R=r" "S=s" "T=t" "U=u" "V=v" "W=w" "X=x" "Y=y" "Z=z" " =-") do set fileName=!fileName:%%~c!
   
   :: Create the binary package
   set binFileName=!fileName!.zip
   
   :: Get the binary files folder
   set binFolder=!cur!%sampleBin%
   
   :: Compress the files
   echo - Processing !binFileName!
   pushd .
   cd !binFolder!
   %rar% %opts% -r "%outputDir%\!binFileName!" *.* -x*.pdb -x*.xml
   popd
   
   :: Create the sources package
   set srcFileName=!fileName!.zip
   
   :: Get the binary files folder
   set srcFolder=!cur!
   
   :: Compress the files
   echo - Processing !srcFileName!
   pushd .
   cd !srcFolder!   
   %rar% %opts% -apsources -r "%outputDir%\!srcFileName!" *.* -x*\.svn* -x*\obj -x*\bin -x*.suo -x*.user -x"*\bin\x86\Release 3.5"
   popd
)


echo.
echo ---------------------------------------------------------
echo All sample applications have finished processing. 
echo ---------------------------------------------------------
echo.

timeout /T 10