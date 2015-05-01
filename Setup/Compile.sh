#!/bin/bash
###########

echo
echo Accord.NET Framework all projects configurations builder
echo =========================================================
echo 
echo This Linux bash script will use Mono's xbuild tool to
echo compile the Debug and Release versions of the framework.
echo 


echo  - Building NET40 configuration...
mdtool -v build -c:"NET40" ../Sources/Accord.NET.Mono.sln

# echo  - Building samples...
# mdtool -v build            ../Samples/Samples.sln

