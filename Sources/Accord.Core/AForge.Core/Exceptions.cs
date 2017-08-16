// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © Andrew Kirillov, 2007-2009
// andrew.kirillov@aforgenet.com
//

namespace Accord
{
    using System;
    using System.Runtime.Serialization;
    using Accord.Compat;

    /// <summary>
    /// Connection failed exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// has failed.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class ConnectionFailedException : Exception
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="ConnectionFailedException"/> class.
        /// </summary>
        /// 
        public ConnectionFailedException()
            : base() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConnectionFailedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public ConnectionFailedException(string message)
            : base(message) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConnectionFailedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// 
        public ConnectionFailedException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    /// <summary>
    /// Connection lost exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// is lost. When the exception is caught, user may need to reconnect to the device.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class ConnectionLostException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionLostException"/> class.
        /// </summary>
        /// 
        public ConnectionLostException() :
            base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionLostException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public ConnectionLostException(string message) :
            base(message) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConnectionLostException"/> class.
        /// </summary>
        /// 
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// 
        public ConnectionLostException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="ConnectionLostException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// 
        public ConnectionLostException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Not connected exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if connection to device
    /// is not established, but user requests for its services.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class NotConnectedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// 
        public NotConnectedException() :
            base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public NotConnectedException(string message) :
            base(message) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// 
        public NotConnectedException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NotConnectedException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// 
        public NotConnectedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }

    /// <summary>
    /// Device busy exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if access to certain device
    /// is not available due to the fact that it is currently busy handling other request/connection.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class DeviceBusyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBusyException"/> class.
        /// </summary>
        /// 
        public DeviceBusyException() :
            base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBusyException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public DeviceBusyException(string message) :
            base(message) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DeviceBusyException"/> class.
        /// </summary>
        /// 
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// 
        public DeviceBusyException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DeviceBusyException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// 
        public DeviceBusyException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Device error exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case if some error happens with a device, which
    /// may need to be reported to user.</para></remarks>
    ///
    [Serializable]
    public class DeviceErrorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceErrorException"/> class.
        /// </summary>
        /// 
        public DeviceErrorException() :
            base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceErrorException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public DeviceErrorException(string message) :
            base(message) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DeviceErrorException"/> class.
        /// </summary>
        /// 
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        /// 
        public DeviceErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="DeviceErrorException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        /// 
        public DeviceErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
