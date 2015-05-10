// AForge Controls Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2010
// andrew.kirillov@aforgenet.com
//

using System;
using System.Runtime.InteropServices;

namespace AForge.Controls
{
    internal static class JoystickAPI
    {
        [DllImport( "winmm.dll" )]
        public static extern int joyGetNumDevs( );

        [DllImport( "winmm.dll" )]
        public static extern ResultCode joyGetDevCapsW( int uJoyID,
            [In, Out, MarshalAs( UnmanagedType.LPStruct )] JOYCAPS pjc, int cbjc );

        [DllImport( "winmm.dll" )]
        public static extern ResultCode joyGetPos( int uJoyID, JOYINFO pji );

        [DllImport( "winmm.dll" )]
        public static extern ResultCode joyGetPosEx( int uJoyID, JOYINFOEX pji );

        [DllImport( "winmm.dll" )]
        public static extern ResultCode joyReleaseCapture( int uJoyID );

        [DllImport( "winmm.dll" )]
        public static extern ResultCode joySetCapture( IntPtr hwnd, int uJoyID, int uPeriod, bool fChanged );

        // Information about current state of joystick's axes and buttons
        [StructLayout( LayoutKind.Sequential )]
        public class JOYINFO
        {
            public int xPos;
            public int yPos;
            public int zPos;
            public int buttons;
        }

        // Extended information about current state of joystick's axes and buttons
        [StructLayout( LayoutKind.Sequential )]
        public class JOYINFOEX
        {
            public int size;
            public JoyPosFlags flags;
            public int xPos;
            public int yPos;
            public int zPos;
            public int rPos; 
            public int uPos; 
            public int vPos; 
            public int buttons; 
            public int buttonNumber; 
            public int pov; 
            public int reserved1; 
            public int reserved2; 
        }

        // Joystick capabilities
        [StructLayout( LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Unicode )]
        public class JOYCAPS
        {
            public short mid; 
            public short pid; 
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 32 )]
            public string name;
            public int  xMin; 
            public int  xMax; 
            public int  yMin; 
            public int  yMax; 
            public int  zMin; 
            public int  zMax; 
            public int  buttonsNumber; 
            public int  minPeriod; 
            public int  maxPeriod; 
            public int  rMin; 
            public int  rMax; 
            public int  uMin; 
            public int  uMax; 
            public int  vMin; 
            public int  vMax; 
            public int  caps; 
            public int  axesMax; 
            public int  axesNumber; 
            public int  buttonsMax; 
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 32 )]
            public string regKey;
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 260 )]
            public string oemVxD;
        }

        // Some result codes
        public enum ResultCode : uint
        {
            NoError      = 0,
            Error        = 1,
            BadDeviceID  = 2,
            NoDriver     = 6,
            InvalidParam = 11,

            JoystickInvalidParam = 165,
            JoystickRequestNotCompleted = 166,
            JoystickUnplugged    = 167
        }

        [Flags]
        public enum JoyPosFlags
        {
            ReturnX = 0x01,
            ReturnY = 0x02,
            ReturnZ = 0x04,
            ReturnR = 0x08,
            ReturnU = 0x10,
            ReturnV = 0x20,
            ReturnPov     = 0x40,
            ReturnButtons = 0x80,

            ReturnXY      = 0x03,
            ReturnXYZ     = 0x07,
            ReturnXYZR    = 0x0F,
            ReturnXYZRU   = 0x1F,
            ReturnXYZRUV  = 0x3F,

            ReturnAll     = 0xFF
        }

        [Flags]
        public enum JoyButtons
        {
            Button1 = 0x001,
            Button2 = 0x002,
            Button3 = 0x004,
            Button4 = 0x008,
            Button5 = 0x010,
            Button6 = 0x020,
            Button7 = 0x040,
            Button8 = 0x080
        }

        public const int MM_JOY1MOVE       = 0x3A0;
        public const int MM_JOY2MOVE       = 0x3A1;
        public const int MM_JOY1ZMOVE      = 0x3A2;
        public const int MM_JOY2ZMOVE      = 0x3A3;
        public const int MM_JOY1BUTTONDOWN = 0x3B5;
        public const int MM_JOY2BUTTONDOWN = 0x3B6;
        public const int MM_JOY1BUTTONUP   = 0x3B7;
        public const int MM_JOY2BUTTONUP   = 0x3B8;
    }
}
