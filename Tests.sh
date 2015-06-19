#!/bin/bash
###########

TESTS="Unit Tests/bin/Release/net40/"
NUNIT="Externals/NUnit/nunit-console-x86.exe"
LIST=()

# Run unit tests in the Mono solution
LIST+=("${TESTS}Accord.Tests.Audio.dll")
LIST+=("${TESTS}Accord.Tests.Controls.dll")
LIST+=("${TESTS}Accord.Tests.Core.dll")
#LIST="$LIST ${TESTS}Accord.Tests.Imaging.dll
#LIST="$LIST ${TESTS}Accord.Tests.IO.dll
#LIST="$LIST ${TESTS}Accord.Tests.MachineLearning.dll
#LIST="$LIST ${TESTS}Accord.Tests.Math.dll
#LIST="$LIST ${TESTS}Accord.Tests.Neuro.dll
#LIST="$LIST ${TESTS}Accord.Tests.Vision.dll
#LIST="$LIST ${TESTS}Accord.Tests.Statistics.dll


mono --runtime=v4.0 ${NUNIT} -noxml -nodots -labels -process=multiple "${LIST[@]/#/}"
