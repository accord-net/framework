// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace AForge
{
    using System;

    /// <summary>
    /// Connection failed exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// has failed.</para>
    /// </remarks>
    /// 
    public class ConnectionFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionFailedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public ConnectionFailedException( string message ) :
            base( message ) { }
    }

    /// <summary>
    /// Connection lost exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// is lost. When the exception is caught, user may need to reconnect to the device.</para>
    /// </remarks>
    /// 
    public class ConnectionLostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionLostException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public ConnectionLostException( string message ) :
            base( message ) { }
    }

    /// <summary>
    /// Not connected exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// is not established, but user requests for its services.</para>
    /// </remarks>
    /// 
    public class NotConnectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public NotConnectedException( string message ) :
            base( message ) { }
    }

    /// <summary>
    /// Device busy exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if access to certain device
    /// is not available due to the fact that it is currently busy handling other request/connection.</para>
    /// </remarks>
    /// 
    public class DeviceBusyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBusyException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public DeviceBusyException( string message ) :
            base( message ) { }
    }

    /// <summary>
    /// Device error exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if some error happens with a device, which
    /// may need to be reported to user.</para></remarks>
    ///
    public class DeviceErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceErrorException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public DeviceErrorException( string message ) :
            base( message ) { }
    }
}
