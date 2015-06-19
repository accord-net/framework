#!/bin/bash
###########

echo "                                                         "
echo "Accord.NET Framework all projects configurations builder "
echo "========================================================="
echo "                                                         "
echo "This Linux bash script will use Mono's xbuild tool to    " 
echo "compile the Debug and Release versions of the framework. "
echo "                                                         " 



if [ $# -eq 0 ] || [ "$1" == "framework" ]; then
	echo ""
	echo "  - Building NET40 configuration..."
	echo ""
	xbuild /p:Configuration=NET40 Sources/Accord.NET.Mono.sln 
fi

if [ $# -eq 0 ] || [ "$1" == "samples" ]; then
	echo ""
	echo "  - Building samples..."
	echo ""
	xbuild /p:Configuration=Mono /p:Platform=x86 Samples/Samples.sln
fi
