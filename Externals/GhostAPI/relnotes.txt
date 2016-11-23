
Welcome to the LEGO cross-platform layered communication package ("Ghost")! 
Please read carefully these release notes to find out what this package contains, how to 
install it, what is changed from the previous releases and so on.

All the contents of this package are Copyright (C) 1998-2001 The LEGO Company 

Enjoy!
The folks at LEGO Technology Center - Billund, Denmark

----------------------------

Contents
========

	What's in the package
	Installation
		Windows
		MacOS
	Version History


What's in the package
=====================

The package contains:

	o	USB Tower drivers and API for Windows 98/ME/2000/MacOS9
	o	Ghost communication stack for Windows/MacOS (Intel/PowerPC)
	o	Ghost API w/ for Windows/MacOS (Intel/PowerPC)
			Header files
			Libraries
	o	Samples for for Windows/MacOS (Intel/PowerPC)
	o	Documentation
			USB Tower interface documentation
			Ghost API documentation

Installation
============


Windows
-------

	1.	IF YOU HAVE A LEGO USB TOWER:
		Plug in your tower. When Windows prompts you, insert this CD in the CD reader and continue. 
		If you downloaded this package from the internet, you may have to specify the actual directory
		where the USB driver files are (<your base directory>\WIN32\DRIVER).

	2.	Launch setup.exe from the CD's root directory. 


MacOS
-----

	1.	Copy the LEGO tower drivers from folder "Put into Extensionfolder" to your Mac's system extensions folder.
		The shared libraries in "Binary" can be put into extensions folder, or placed together with the application.

	2.  Copy the LEGO Tower control panel into the control panel folder inside the Mac's system folder.

	3.	Copy the Lego Tower Help folder to the Help folder inside the Mac's system folder.

	3.	Just grab the whole GhostAPI folder and copy it anywhere onto your hard drive.

	4.	Plug in your LEGO USB TOWER.


Version History
===============

v. 1.0.0.102 (November 15 2001)
---------------------------
* [GhostAPI] The GhostAPI now refuses to create a USB communication stack if it's running on Win95 (or earlier)
  or WInNT4 (or earlier), as those OSes do not support USB (Win95 OSR 2.1 does but it's usage is deprecated by 
  Microsoft and the USB tower driver does not support it). GhCreateStack will return PBK_ERR_NOTFOUND. 


v. 1.0.0.101 (July 24 2001)
---------------------------
* Changed major version number to 1 as this API becomes an official release. Build number remains the same.

v. 0.1.0.101 (June 19 2001)
---------------------------
* Increased error checking in GhCreateStack()
* [Mac] New driver with control panel and help
* [Win] Fixed driver installation script (LTOWER.INF) to prevent Windows 98 from asking for the ROBOLAB CD.


v. 0.1.0.100 (June 8 2001)
--------------------------
* [Mac] New driver with full vendor request support.
* [Mac] Final version of the serial port support 


v. 0.1.0.99 (May 18 2001)
-------------------------
* [Win] Sample Windows program supports unlock firmware and motors off/on

* [Mac only] New Mac USB Driver supporting exclusive access to the port and some settings functions
* [Mac only] Support for SetTimeout and GetTimeout in the USB port layer. USB timeouts are not fixed 
   any more, but proportional to the number of bytes requested for.
* [Mac only] Preliminary version of the serial port available.


v. 0.1.0.98 (May 18 2001)
-------------------------
* [Mac only] New Mac USB Driver supporting exclusive access to the port and some settings functions
* [Mac only] Support for SetTimeout and GetTimeout in the USB port layer. USB timeouts are not fixed 
   any more, but proportional to the number of bytes requested for.
* [Mac only] Preliminary version of the serial port available.

v. 0.1.0.97 (May 15 2001)
-------------------------
* [Win only] Now the GhostAPI doesn't load the pbkusbport and pbkcomm32 dlls untile they are needed
* [Win only] Changed the GhostAPI DLL's base address from default to 0x23000000 (this will speed up 
   loading in most cases).

v. 0.1.0.96 (May 10 2001)
-------------------------
* [Mac only] Added support for serial towers

v. 0.1.0.95 (Apr 24 2001)
-------------------------
* Added functions to the GhostAPI: GhDiagSelectFirstPort, GhDiagSelectNextPort, GhDiagSelectSpecificDevice
* [Win only] Added PbkMouse.exe to Windows binary deliverables for correct functioning of Serial port when a serial mouse is attached
  to the PC. 


v. 0.1.0.94 (Apr 20 2001)
-------------------------
* [Mac only] Fixed a bug where an attempt of creating a serial tower stack on the Macintosh would crash. 
* [Mac only] GhostAPI binary and stub renamed from "GhostAPI Classic" to just "GhostAPI" in order 
  to be consistent with the Windows Version
* [Mac only] The GhostAPI functions are no longer mangled by the compiler 
* [Win only] The Windows version now comes with a spiffy installation program 
* Added function to Windows and Mac sample programs to download a minimal RCX program

v. 0.1.0.93 (Apr 18 2001)
-------------------------
* Added new functions GhSetQueueContext and GhGetQueueContext, which give support to 
  notification context values. 
* Windows sample program updated to show usage of GhSetQueueContext.
* Corrected bug in the Windows sample program - it was trying to destroy a queue both 
  in the notification function
  and after calling GhExecute (if it returned PBOK).

v. 0.1.0.92 (April 6 2001)
--------------------------
* First Mac version released.
* GhGetRetries and GhSetRetries: changed parameter types from int and int* to int32 and int32*

v. 0.1.0.91 (Mar 21 2001)
---------------------------
* Removed C++ mangling from exported function names

v. 0.1.0.90 (Mar 6 2001)
---------------------------
* First released version (Win32 only). Code complete.
