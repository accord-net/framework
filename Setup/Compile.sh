#!/bin/bash
###########

echo
echo Accord.NET Framework all projects configurations builder
echo =========================================================
echo 
echo This Linux bash script will use Mono's xbuild tool to
echo compile the Debug and Release versions of the framework.
echo 


echo  - Building Debug configuration...
xbuild /target:"Clean;Build" /p:Configuration=Debug ../Sources/Accord.NET.Mono.sln

echo  - Building NET35 configuration...
xbuild /target:"Clean;Build" /p:Configuration=NET35 ../Sources/Accord.NET.Mono.sln

echo  - Building NET40 configuration...
xbuild /target:"Clean;Build" /p:Configuration=NET40 ../Sources/Accord.NET.Mono.sln

echo  - Building NET45 configuration...
xbuild /target:"Clean;Build" /p:Configuration=NET45 ../Sources/Accord.NET.Mono.sln

echo  - Building samples...
xbuild /target:"Clean;Build" /p:Configuration=NET45 ../Samples/Samples.sln

