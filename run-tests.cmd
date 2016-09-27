@echo OFF
setlocal enabledelayedexpansion

set TESTS=Unit Tests\bin\Release\net45\
set NUNIT="Externals\NUnit\nunit-console-x86.exe" 

:: Run unit tests in the Mono solution
set LIST=%LIST% "%TESTS%Accord.Tests.Audio.dll"
set LIST=%LIST% "%TESTS%Accord.Tests.Controls.dll"
set LIST=%LIST% "%TESTS%Accord.Tests.Core.dll"
set LIST=%LIST% "%TESTS%Accord.Tests.Imaging.dll" 
set LIST=%LIST% "%TESTS%Accord.Tests.IO.dll"
set LIST=%LIST% "%TESTS%Accord.Tests.MachineLearning.dll"
set LIST=%LIST% "%TESTS%Accord.Tests.Math.dll"
set LIST=%LIST% "%TESTS%Accord.Tests.Neuro.dll" 
set LIST=%LIST% "%TESTS%Accord.Tests.Vision.dll" 
set LIST=%LIST% "%TESTS%Accord.Tests.Statistics.dll" 

%NUNIT% -noxml -nodots -labels /process=multiple %LIST% /framework:net-4.6

 
 

