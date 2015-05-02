// AForge XIMEA Video Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
//

namespace AForge.Video.Ximea
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Text;
    using AForge.Video.Ximea.Internal;

    /// <summary>
    /// The class provides access to XIMEA cameras.
    /// </summary>
    /// 
    /// <remarks><para>The class allows to perform image acquisition from <a href="http://www.ximea.com/">XIMEA</a> cameras.
    /// It wraps XIMEA'a xiAPI, which means that users of this class will also require <b>m3api.dll</b> and a correct
    /// TM file for the camera model connected to the system (both are provided with XIMEA API software package).</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// XimeaCamera camera = new XimeaCamera( );
    /// 
    /// // open camera and start data acquisition
    /// camera.Open( 0 );
    /// camera.StartAcquisition( );
    /// 
    /// // set exposure time to 10 milliseconds
    /// camera.SetParam( CameraParameter.Exposure, 10 * 1000 );
    /// 
    /// // get image from the camera
    /// Bitmap bitmap = camera.GetImage( );
    /// // process the image
    /// // ...
    /// 
    /// // dispose the image when it is no longer needed
    /// bitmap.Dispose( );
    /// 
    /// // stop data acquisition and close the camera
    /// camera.StopAcquisition( );
    /// camera.Close( );
    /// </code>
    /// </remarks>
    /// 
    /// <seealso cref="XimeaVideoSource"/>
    /// 
    public class XimeaCamera
    {
        private IntPtr deviceHandle = IntPtr.Zero;
        private bool isAcquisitionStarted = false;
        private int deviceID = 0;

        // dummy object to lock for synchronization
        private object sync = new object( );

        /// <summary>
        /// Get number of XIMEA camera connected to the system.
        /// </summary>
        public static int CamerasCount
        {
            get
            {
                int count;

                int errorCode = XimeaAPI.xiGetNumberDevices( out count );
                HandleError( errorCode );

                return count;
            }
        }

        /// <summary>
        /// Specifies if camera's data acquisition is currently active for the opened camera (if any).
        /// </summary>
        public bool IsAcquisitionStarted
        {
            get
            {
                lock ( sync )
                {
                    return isAcquisitionStarted;
                }
            }
        }

        /// <summary>
        /// Specifies if a camera is currently opened by the instance of the class.
        /// </summary>
        public bool IsDeviceOpen
        {
            get
            {
                lock ( sync )
                {
                    return ( deviceHandle != IntPtr.Zero );
                }
            }
        }

        /// <summary>
        /// ID of the the recently opened XIMEA camera.
        /// </summary>
        public int DeviceID
        {
            get { return deviceID; }
        }

        /// <summary>
        /// Open XIMEA camera.
        /// </summary>
        /// 
        /// <param name="deviceID">Camera ID to open.</param>
        /// 
        /// <remarks><para>Opens the specified XIMEA camera preparing it for starting video acquisition
        /// which is done using <see cref="StartAcquisition"/> method. The <see cref="IsDeviceOpen"/>
        /// property can be used at any time to find if a camera was opened or not.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        ///
        public void Open( int deviceID )
        {
            lock ( sync )
            {
                IntPtr deviceHandle;
                int errorCode = XimeaAPI.xiOpenDevice( deviceID, out deviceHandle );
                HandleError( errorCode );
                // save the device handle is everything is fine
                this.deviceHandle = deviceHandle;
                this.isAcquisitionStarted = false;
                this.deviceID = deviceID;
            }
        }

        /// <summary>
        /// Close opened camera (if any) and release allocated resources.
        /// </summary>
        /// 
        /// <remarks><para><note>The method also calls <see cref="StopAcquisition"/> method if it was not
        /// done by user.</note></para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        ///
        public void Close( )
        {
            lock ( sync )
            {
                if ( deviceHandle != IntPtr.Zero )
                {
                    if ( isAcquisitionStarted )
                    {
                        try
                        {
                            StopAcquisition( );
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        int errorCode = XimeaAPI.xiCloseDevice( deviceHandle );
                        HandleError( errorCode );
                    }
                    finally
                    {
                        deviceHandle = IntPtr.Zero;
                    }
                }
            }
        }

        /// <summary>
        /// Begin camera's work cycle and start data acquisition from it.
        /// </summary>
        /// 
        /// <remarks><para>The <see cref="IsAcquisitionStarted"/> property can be used at any time to find if the
        /// acquisition was started or not.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        /// 
        public void StartAcquisition( )
        {
            lock ( sync )
            {
                CheckConnection( );

                int errorCode = XimeaAPI.xiStartAcquisition( deviceHandle );
                HandleError( errorCode );

                isAcquisitionStarted = true;
            }
        }

        /// <summary>
        /// End camera's work cycle and stops data acquisition.
        /// </summary>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        /// 
        public void StopAcquisition( )
        {
            lock ( sync )
            {
                CheckConnection( );

                try
                {
                    int errorCode = XimeaAPI.xiStopAcquisition( deviceHandle );
                    HandleError( errorCode );
                }
                finally
                {
                    isAcquisitionStarted = false;
                }
            }
        }

        /// <summary>
        /// Set camera's parameter.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Integer parameter value.</param>
        /// 
        /// <remarks><para>The method allows to control different camera's parameters, like exposure time, gain value, etc.
        /// See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.
        /// </para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public void SetParam( string parameterName, int value )
        {
            lock ( sync )
            {
                CheckConnection( );

                int errorCode = XimeaAPI.xiSetParam( deviceHandle, parameterName, ref value, 4, ParameterType.Integer );
                HandleError( errorCode );
            }
        }

        /// <summary>
        /// Set camera's parameter.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="value">Float parameter value.</param>
        /// 
        /// <remarks><para>The method allows to control different camera's parameters, like exposure time, gain value, etc.
        /// See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.
        /// </para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public void SetParam( string parameterName, float value )
        {
            lock ( sync )
            {
                CheckConnection( );

                int errorCode = XimeaAPI.xiSetParam( deviceHandle, parameterName, ref value, 4, ParameterType.Float );
                HandleError( errorCode );
            }
        }

        /// <summary>
        /// Get camera's parameter as integer value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns integer value of the requested parameter.</returns>
        /// 
        /// <remarks><para>See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public int GetParamInt( string parameterName )
        {
            lock ( sync )
            {
                CheckConnection( );

                int value;
                int size;
                ParameterType type = ParameterType.Integer;

                int errorCode = XimeaAPI.xiGetParam( deviceHandle, parameterName, out value, out size, ref type );
                HandleError( errorCode );

                return value;
            }
        }

        /// <summary>
        /// Get camera's parameter as float value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns float value of the requested parameter.</returns>
        /// 
        /// <remarks><para>See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public float GetParamFloat( string parameterName )
        {
            lock ( sync )
            {
                CheckConnection( );

                float value;
                int size;
                ParameterType type = ParameterType.Float;

                int errorCode = XimeaAPI.xiGetParam( deviceHandle, parameterName, out value, out size, ref type );
                HandleError( errorCode );

                return value;
            }
        }

        /// <summary>
        /// Get camera's parameter as string value.
        /// </summary>
        /// 
        /// <param name="parameterName">Parameter name to get from camera.</param>
        /// 
        /// <returns>Returns string value of the requested parameter.</returns>
        /// 
        /// <remarks><para>See <see cref="CameraParameter"/> class for the list of some possible configuration parameters. See
        /// XIMEA documentation for the complete list of supported parameters.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        ///
        public string GetParamString( string parameterName )
        {
            lock ( sync )
            {
                CheckConnection( );

                byte[] bytes = new byte[260];
                int size = bytes.Length;
                ParameterType type = ParameterType.String;

                unsafe
                {
                    fixed ( byte* ptr = bytes )
                    {
                        int errorCode = XimeaAPI.xiGetParam( deviceHandle, parameterName, ptr, out size, ref type );
                        HandleError( errorCode );
                    }
                }

                return Encoding.ASCII.GetString( bytes, 0, size );
            }
        }

        /// <summary>
        /// Get image from the opened XIMEA camera.
        /// </summary>
        /// 
        /// <returns>Returns image retrieved from the camera.</returns>
        /// 
        /// <remarks><para>The method calls <see cref="GetImage(int)"/> method specifying 5000 as the timeout
        /// value.</para></remarks>
        ///
        public unsafe Bitmap GetImage( )
        {
            return GetImage( 5000 );
        }

        /// <summary>
        /// Get image from the opened XIMEA camera.
        /// </summary>
        /// 
        /// <param name="timeout">Maximum time to wait in milliseconds till image becomes available.</param>
        /// 
        /// <returns>Returns image retrieved from the camera.</returns>
        /// 
        /// <remarks><para>The method calls <see cref="GetImage(int,bool)"/> method specifying <see langword="true"/>
        /// the <b>makeCopy</b> parameter.</para></remarks>
        ///
        public unsafe Bitmap GetImage( int timeout )
        {
            return GetImage( timeout, true );
        }
        
        /// <summary>
        /// Get image from the opened XIMEA camera.
        /// </summary>
        /// 
        /// <param name="timeout">Maximum time to wait in milliseconds till image becomes available.</param>
        /// <param name="makeCopy">Make a copy of the camera's image or not.</param>
        /// 
        /// <returns>Returns image retrieved from the camera.</returns>
        /// 
        /// <remarks><para>If the <paramref name="makeCopy"/> is set to <see langword="true"/>, then the method
        /// creates a managed copy of the camera's image, so the managed image stays valid even when the camera
        /// is closed. However, setting this parameter to <see langword="false"/> creates a managed image which is
        /// just a wrapper around camera's unmanaged image. So if camera is closed and its resources are freed, the
        /// managed image becomes no longer valid and accessing it will generate an exception.</para></remarks>
        /// 
        /// <exception cref="VideoException">An error occurred while communicating with a camera. See error
        /// message for additional information.</exception>
        /// <exception cref="NotConnectedException">No camera was opened, so can not access its methods.</exception>
        /// <exception cref="TimeoutException">Time out value reached - no image is available within specified time value.</exception>
        ///
        public Bitmap GetImage( int timeout, bool makeCopy )
        {
            lock ( sync )
            {
                CheckConnection( );

                int errorCode;

                XimeaImage ximeaImage = new XimeaImage( );
                unsafe
                {
                    ximeaImage.StructSize = sizeof( XimeaImage );
                }

                // get image from XIMEA camera
                try
                {
                    errorCode = XimeaAPI.xiGetImage( deviceHandle, timeout, ref ximeaImage );
                }
                catch ( AccessViolationException )
                {
                    errorCode = 9;
                }

                // handle error if any
                HandleError( errorCode );

                // create managed bitmap for the unmanaged image provided by camera
                PixelFormat pixelFormat = PixelFormat.Undefined;
                int stride = 0;

                switch ( ximeaImage.PixelFormat )
                {
                    case ImageFormat.Grayscale8:
                        pixelFormat = PixelFormat.Format8bppIndexed;
                        stride = ximeaImage.Width;
                        break;

                    case ImageFormat.RGB24:
                        pixelFormat = PixelFormat.Format24bppRgb;
                        stride = ximeaImage.Width * 3;
                        break;

                    case ImageFormat.RGB32:
                        pixelFormat = PixelFormat.Format32bppRgb;
                        stride = ximeaImage.Width * 4;
                        break;

                    default:
                        throw new VideoException( "Unsupported pixel format." );
                }

                Bitmap bitmap = null;

                if ( !makeCopy )
                {
                    bitmap = new Bitmap( ximeaImage.Width, ximeaImage.Height, stride, pixelFormat, ximeaImage.BitmapData );
                }
                else
                {
                    bitmap = new Bitmap( ximeaImage.Width, ximeaImage.Height, pixelFormat );

                    // lock destination bitmap data
                    BitmapData bitmapData = bitmap.LockBits(
                        new Rectangle( 0, 0, ximeaImage.Width, ximeaImage.Height ),
                        ImageLockMode.ReadWrite, pixelFormat );

                    int dstStride = bitmapData.Stride;
                    int lineSize  = Math.Min( stride, dstStride );

                    unsafe
                    {
                        byte* dst = (byte*) bitmapData.Scan0.ToPointer( );
                        byte* src = (byte*) ximeaImage.BitmapData.ToPointer( );

                        if ( stride != dstStride )
                        {
                            // copy image
                            for ( int y = 0; y < ximeaImage.Height; y++ )
                            {
                                AForge.SystemTools.CopyUnmanagedMemory( dst, src, lineSize );
                                dst += dstStride;
                                src += stride;
                            }
                        }
                        else
                        {
                            AForge.SystemTools.CopyUnmanagedMemory( dst, src, stride * ximeaImage.Height );
                        }
                    }

                    // unlock destination images
                    bitmap.UnlockBits( bitmapData );
                }

                // set palette for grayscale image
                if ( ximeaImage.PixelFormat == ImageFormat.Grayscale8 )
                {
                    ColorPalette palette = bitmap.Palette;
                    for ( int i = 0; i < 256; i++ )
                    {
                        palette.Entries[i] = Color.FromArgb( i, i, i );
                    }
                    bitmap.Palette = palette;
                }

                return bitmap;
            }
        }


        // Handle errors from XIMEA API
        private static void HandleError( int errorCode )
        {
            if ( errorCode != 0 )
            {
                if ( errorCode == 10 )
                {
                    throw new TimeoutException( "Time out while waiting for camera response." ); 
                }

                string errorMessage = string.Empty;

                switch ( errorCode )
                {
                    case 1:
                        errorMessage = "Invalid handle";
                        break;

                    case 2:
                        errorMessage = "Register read error";
                        break;

                    case 3:
                        errorMessage = "Register write error";
                        break;

                    case 4:
                        errorMessage = "Freeing resiurces error";
                        break;

                    case 5:
                        errorMessage = "Freeing channel error";
                        break;

                    case 6:
                        errorMessage = "Freeing bandwith error";
                        break;

                    case 7:
                        errorMessage = "Read block error";
                        break;

                    case 8:
                        errorMessage = "Write block error";
                        break;

                    case 9:
                        errorMessage = "No image";
                        break;

                    case 11:
                        errorMessage = "Invalid arguments supplied";
                        break;

                    case 12:
                        errorMessage = "Not supported";
                        break;

                    case 13:
                        errorMessage = "Attach buffers error";
                        break;

                    case 14:
                        errorMessage = "Overlapped result";
                        break;

                    case 15:
                        errorMessage = "Memory allocation error";
                        break;

                    case 16:
                        errorMessage = "DLL context is NULL";
                        break;

                    case 17:
                        errorMessage = "DLL context is non zero";
                        break;

                    case 18:
                        errorMessage = "DLL context exists";
                        break;

                    case 19:
                        errorMessage = "Too many devices connected";
                        break;

                    case 20:
                        errorMessage = "Camera context error";
                        break;

                    case 21:
                        errorMessage = "Unknown hardware";
                        break;

                    case 22:
                        errorMessage = "Invalid TM file";
                        break;

                    case 23:
                        errorMessage = "Invalid TM tag";
                        break;

                    case 24:
                        errorMessage = "Incomplete TM";
                        break;

                    case 25:
                        errorMessage = "Bus reset error";
                        break;

                    case 26:
                        errorMessage = "Not implemented";
                        break;

                    case 27:
                        errorMessage = "Shading too bright";
                        break;

                    case 28:
                        errorMessage = "Shading too dark";
                        break;

                    case 29:
                        errorMessage = "Gain is too low";
                        break;

                    case 30:
                        errorMessage = "Invalid bad pixel list";
                        break;

                    case 31:
                        errorMessage = "Bad pixel list realloc error";
                        break;

                    case 32:
                        errorMessage = "Invalid pixel list";
                        break;

                    case 33:
                        errorMessage = "Invalid Flash File System";
                        break;

                    case 34:
                        errorMessage = "Invalid profile";
                        break;

                    case 35:
                        errorMessage = "Invalid calibration";
                        break;

                    case 36:
                        errorMessage = "Invalid buffer";
                        break;

                    case 38:
                        errorMessage = "Invalid data";
                        break;

                    case 39:
                        errorMessage = "Timing generator is busy";
                        break;

                    case 40:
                        errorMessage = "Wrong operation open/write/read/close";
                        break;

                    case 41:
                        errorMessage = "Acquisition already started";
                        break;

                    case 42:
                        errorMessage = "Old version of device driver installed to the system";
                        break;

                    case 44:
                        errorMessage = "Data can't be processed";
                        break;

                    case 45:
                        errorMessage = "Error occured and acquisition has been stoped or didn't start";
                        break;

                    case 46:
                        errorMessage = "Acquisition has been stoped with error";
                        break;

                    case 100:
                        errorMessage = "Unknown parameter";
                        break;

                    case 101:
                        errorMessage = "Wrong parameter value";
                        break;

                    case 103:
                        errorMessage = "Wrong parameter type";
                        break;

                    case 104:
                        errorMessage = "Wrong parameter size";
                        break;

                    case 105:
                        errorMessage = "Input buffer too small";
                        break;

                    case 106:
                        errorMessage = "Parameter info not supported";
                        break;

                    case 107:
                        errorMessage = "Parameter info not supported";
                        break;

                    case 108:
                        errorMessage = "Data format not supported";
                        break;

                    case 109:
                        errorMessage = "Read only parameter";
                        break;

                    case 110:
                        errorMessage = "No devices found";
                        break;
                }

                throw new VideoException( string.Format( "Error code: {0}, Message: {1}", errorCode, errorMessage ) );
            }
        }

        // Check if a camera is open or not
        private void CheckConnection( )
        {
            if ( deviceHandle == IntPtr.Zero )
            {
                throw new NotConnectedException( "No established connection to XIMEA camera." );
            }
        }
    }
}
