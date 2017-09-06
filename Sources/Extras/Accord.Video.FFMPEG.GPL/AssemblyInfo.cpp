#include "stdafx.h"

using namespace System;
using namespace System::Reflection;
using namespace System::Runtime::CompilerServices;
using namespace System::Runtime::InteropServices;
using namespace System::Security::Permissions;

[assembly:ComVisible(false)];
[assembly:CLSCompliantAttribute(true)];

#if !NETSTANDARD
[assembly:AssemblyKeyFileAttribute("Accord.snk")];
[assembly:AssemblyDelaySignAttribute(true)];
#endif

[assembly:AssemblyTitleAttribute("Accord.Video.FFMPEG")];

