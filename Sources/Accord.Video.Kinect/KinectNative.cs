// AForge Kinect Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.Kinect
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Kinect's LED color options.
    /// </summary>
    public enum LedColorOption
    {
        /// <summary>
        /// The LED is off.
        /// </summary>
        Off = 0,
        /// <summary>
        /// The LED is on and has green color.
        /// </summary>
        Green = 1,
        /// <summary>
        /// The LED is on and has red color.
        /// </summary>
        Red = 2,
        /// <summary>
        /// The LED is on and has yellow color.
        /// </summary>
        Yellow = 3,
        /// <summary>
        /// The LED is blinking with green color.
        /// </summary>
        BlinkGreen = 4,
        /// <summary>
        /// The LED is blinking from red to yellow color.
        /// </summary>
        BlinkRedYellow = 6
    }

    /// <summary>
    /// Kinect's resolutions of video and depth cameras.
    /// </summary>
    public enum CameraResolution
    {
        /// <summary>
        /// Low resolution.
        /// </summary>
        Low,
        /// <summary>
        /// Medium resolution.
        /// </summary>
        Medium,
        /// <summary>
        /// Hight resolution.
        /// </summary>
        High,
    }

    internal static class KinectNative
    {
        // dummy object for synchronization
        // Note: it was noticed that if freenect_stop_depth() is called together with freenect_process_events()
        // in different threads, then the call may hang sometimes for about 30 seconds or more making impression
        // of dead lock or so. Putting lock around those calls seems to help.
        private static object sync = new object( );

        // Logging levels
        public enum LogLevelOptions
        {
            Fatal = 0,
            Error,
            Warning,
            Notice,
            Info,
            Debug,
            Spew,
            Flood,
        }

        // Video camera data formats
        public enum VideoCameraFormat
        {
            RGB = 0,
            Bayer = 1,
            IR8Bit = 2,
            IR10Bit = 3,
            IR10BitPacked = 4
        }

        // Depth camera data formats
        public enum DepthCameraFormat
        {
            Format11Bit = 0,
            Format10Bit = 1,
            FormatPacked11Bit = 2,
            FormatPacked10Bit = 3
        }

        // Different states the tilt motor can be in operation
        public enum TiltStatusOption
        {
            Stopped = 0x00,
            AtLimit = 0x01,
            Moving  = 0x04
        }

        // Device tilt state values. This holds stuff like accelerometer and tilt status.
        public struct TiltState
        {
            public Int16 AccelerometerX;
            public Int16 AccelerometerY;
            public Int16 AccelerometerZ;
            public SByte TiltAngle;
            public TiltStatusOption TiltStatus;

            public TiltState( Int16 x, Int16 y, Int16 z )
            {
                AccelerometerX = x;
                AccelerometerY = y;
                AccelerometerZ = z;
                TiltAngle = 0;
                TiltStatus = TiltStatusOption.Stopped;
            }
        }

        // Information about certain picture format
        [StructLayout( LayoutKind.Sequential )]
        public struct BitmapInfoHeader
        {
            public uint Reserved;
            public uint Resolution;
            public uint Format;
            // Total buffer size in bytes to hold a single frame of data
            public uint Bytes;
            // Width of the frame, in pixels
            public short Width;
            // Height of the frame, in pixels
            public short Height;
            // Number of bits of information needed for each pixel
            public byte DataBitsPerPixel;
            // Number of bits of padding for alignment used for each pixel
            public byte PaddingBitsPerPixel;
            // Approximate expected frame rate
            public byte Framerate;
            // If 0, this structure is invalid and does not describe a supported mode.  Otherwise, it is valid.
            public byte IsValid;
        }

        // BSD like time value
        [StructLayout( LayoutKind.Sequential )]
        internal class timeval
        {
            public int sec;
            public int usec;
        }

        // Native callback for freenect library logging
        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        public delegate void FreenectLogCallback( IntPtr device, LogLevelOptions logLevel, string message );

        // Native callback for depth data
        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        public delegate void FreenectDepthDataCallback( IntPtr device, IntPtr depthData, UInt32 timestamp );

        // Native callback for video image data
        [UnmanagedFunctionPointer( CallingConvention.Cdecl )]
        public delegate void FreenectVideoDataCallback( IntPtr device, IntPtr imageData, UInt32 timestamp );
	
        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_init( ref IntPtr context, IntPtr freenectUSBContext );

        [DllImport( "freenect", EntryPoint = "freenect_process_events", CallingConvention = CallingConvention.Cdecl )]
        private static extern int native_freenect_process_events( IntPtr context );

        public static int freenect_process_events( IntPtr context )
        {
            lock ( sync )
            {
                return native_freenect_process_events( context );
            }
        }

        [DllImport( "freenect", EntryPoint = "freenect_process_events_timeout", CallingConvention = CallingConvention.Cdecl )]
        private static extern int native_freenect_process_events_timeout( IntPtr context, [In] IntPtr timeout );

        public static int freenect_process_events_timeout0( IntPtr context )
        {
            lock ( sync )
            {
                return native_freenect_process_events_timeout( context, IntPtr.Zero );
            }
        }

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern void freenect_set_log_level( IntPtr context, LogLevelOptions level );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern void freenect_set_log_callback( IntPtr context, FreenectLogCallback callback );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_shutdown( IntPtr context );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_num_devices( IntPtr context );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_open_device( IntPtr context, ref IntPtr device, int index );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_close_device( IntPtr device );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        [return: MarshalAs( UnmanagedType.Struct )]
        public static extern BitmapInfoHeader freenect_find_video_mode( CameraResolution resolution, VideoCameraFormat videoFormat );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_set_video_mode( IntPtr device, [In, MarshalAs( UnmanagedType.Struct )] BitmapInfoHeader infoHeader );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_set_video_buffer( IntPtr device, IntPtr buffer );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern void freenect_set_video_callback( IntPtr device, FreenectVideoDataCallback callback );

        [DllImport( "freenect", EntryPoint = "freenect_start_video", CallingConvention = CallingConvention.Cdecl )]
        private static extern int native_freenect_start_video( IntPtr device );

        public static int freenect_start_video( IntPtr device )
        {
            lock ( sync )
            {
                return native_freenect_start_video( device );
            }
        }

        [DllImport( "freenect", EntryPoint = "freenect_stop_video", CallingConvention = CallingConvention.Cdecl )]
        private static extern int native_freenect_stop_video( IntPtr device );

        public static int freenect_stop_video( IntPtr device )
        {
            lock ( sync )
            {
                return native_freenect_stop_video( device );
            }
        }

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        [return: MarshalAs( UnmanagedType.Struct )]
        public static extern BitmapInfoHeader freenect_find_depth_mode( CameraResolution resolution, DepthCameraFormat depthFormat );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_set_depth_mode( IntPtr device, BitmapInfoHeader infoHeader );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_set_depth_buffer( IntPtr device, IntPtr buffer );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern void freenect_set_depth_callback( IntPtr device, FreenectDepthDataCallback callback );

        [DllImport( "freenect", EntryPoint = "freenect_start_depth", CallingConvention = CallingConvention.Cdecl )]
        private static extern int native_freenect_start_depth( IntPtr device );

        public static int freenect_start_depth( IntPtr device )
        {
            lock ( sync )
            {
                return native_freenect_start_depth( device );
            }
        }

        [DllImport( "freenect", EntryPoint = "freenect_stop_depth", CallingConvention = CallingConvention.Cdecl )]
        private static extern int native_freenect_stop_depth( IntPtr device );

        public static int freenect_stop_depth( IntPtr device )
        {
            lock ( sync )
            {
                return native_freenect_stop_depth( device );
            }
        }
        
        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_set_led( IntPtr device, LedColorOption option );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_update_tilt_state( IntPtr device );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern IntPtr freenect_get_tilt_state( IntPtr device );

        [DllImport( "freenect", CallingConvention = CallingConvention.Cdecl )]
        public static extern int freenect_set_tilt_degs( IntPtr device, double angle );

        // feenect context
        private static IntPtr freenectContext = IntPtr.Zero;
        // logging calback
        private static FreenectLogCallback logCallback = new FreenectLogCallback( LogCallback );

        // Gets a freenect context to work with.
        public static IntPtr Context
        {
            get
            {
                if ( freenectContext == IntPtr.Zero )
                {
                    KinectNative.InitializeContext( );
                }

                return freenectContext;
            }
        }

        // Initializes the freenect context
        private static void InitializeContext( )
        {
            int result = freenect_init( ref KinectNative.freenectContext, IntPtr.Zero );

            if ( result != 0 )
            {
                throw new ApplicationException( "Could not initialize freenect context. Error Code:" + result );
            }

            // set callback for logging
            KinectNative.freenect_set_log_level( freenectContext, LogLevelOptions.Error );
            KinectNative.freenect_set_log_callback( freenectContext, logCallback );
        }

        // Shuts down the context and closes any open devices
        public static void ShutdownContext( )
        {
            if ( freenectContext != IntPtr.Zero )
            {
                // shutdown context
                int result = KinectNative.freenect_shutdown( freenectContext );
                if ( result != 0 )
                {
                    throw new ApplicationException( "Could not shutdown freenect context. Error Code:" + result );
                }

                // Dispose pointer
                KinectNative.freenectContext = IntPtr.Zero;
            }
        }

        // Logging callback
        internal static void LogCallback( IntPtr device, LogLevelOptions logLevel, string message )
        {
            Console.WriteLine( string.Format( "[{0}] : {1}", logLevel, message ) );

            if ( OnError != null )
            {
                OnError( device );
            }
        }

        public delegate void ErrorHandler( IntPtr device );
        public static event ErrorHandler OnError;
    }
}
