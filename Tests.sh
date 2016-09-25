#!/bin/bash
###########

TESTS="Unit Tests/bin/Release/mono/"
NUNIT="Externals/NUnit/nunit-console-x86.exe"
LIST=()

# Run unit tests in the Mono solution
#LIST+=("${TESTS}Accord.Tests.Audio.dll")
LIST+=("${TESTS}Accord.Tests.Controls.dll")
LIST+=("${TESTS}Accord.Tests.Core.dll")
LIST+=("${TESTS}Accord.Tests.Imaging.dll")
LIST+=("${TESTS}Accord.Tests.IO.dll")
LIST+=("${TESTS}Accord.Tests.MachineLearning.dll")
LIST+=("${TESTS}Accord.Tests.Math.dll")
LIST+=("${TESTS}Accord.Tests.Neuro.dll")
LIST+=("${TESTS}Accord.Tests.Vision.dll")
LIST+=("${TESTS}Accord.Tests.Statistics.dll")


mono --runtime=v4.0 ${NUNIT} -noxml -nodots -labels -exclude:Serialization,Intensive,WinForms,Office,MonoNotSupported -process=single "${LIST[@]/#/}"
