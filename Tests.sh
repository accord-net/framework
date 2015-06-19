#!/bin/bash
###########

TESTS="Unit\ Tests/bin/Release/net45/"
NUNIT="Externals/NUnit/nunit-console-x86.exe" 

# Run unit tests in the Mono solution
LIST="$LIST '%TESTS%Accord.Tests.Audio.dll"
LIST="$LIST '%TESTS%Accord.Tests.Controls.dll"
LIST="$LIST '%TESTS%Accord.Tests.Core.dll"
LIST="$LIST '%TESTS%Accord.Tests.Imaging.dll" 
LIST="$LIST '%TESTS%Accord.Tests.IO.dll"
LIST="$LIST '%TESTS%Accord.Tests.MachineLearning.dll"
LIST="$LIST '%TESTS%Accord.Tests.Math.dll"
LIST="$LIST '%TESTS%Accord.Tests.Neuro.dll" 
LIST="$LIST '%TESTS%Accord.Tests.Vision.dll" 
LIST="$LIST '%TESTS%Accord.Tests.Statistics.dll" 

mono --runtime=v4.0 $NUNIT -noxml -nodots -labels -stoponerror /process=multiple $LIST /framework:net-4.0