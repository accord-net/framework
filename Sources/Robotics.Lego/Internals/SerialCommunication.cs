// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego.Internals
{
    using System;
    using System.IO;
    using System.IO.Ports;

    /// <summary>
    /// Implementation of serial communication interface with LEGO Mindstorm NXT brick.
    /// </summary>
    /// 
    internal class SerialCommunication : INXTCommunicationInterface
    {
        // serial port for communication with NXT brick
        SerialPort port = null;

        /// <summary>
        /// Maximum message size, which can be sent over this communication interface to NXT
        /// brick.
        /// </summary>
        /// 
        public const int MaxMessageSize = 64;

        /// <summary>
        /// Serial port name used for communication.
        /// </summary>
        /// 
        public string PortName
        {
            get { return port.PortName; }
        }

        /// <summary>
        /// Get connection status.
        /// </summary>
        /// 
        public bool IsConnected
        {
            get { return port.IsOpen; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialCommunication"/> class.
        /// </summary>
        /// 
        /// <param name="portName">Serial port name to use for communication.</param>
        /// 
        /// <remarks>This constructor initializes serial port with default write and read
        /// timeout values, which are 1000 milliseconds.</remarks>
        /// 
        public SerialCommunication( string portName ) :
            this( portName, 1000, 1000 )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerialCommunication"/> class.
        /// </summary>
        /// 
        /// <param name="portName">Serial port name to use for communication.</param>
        /// <param name="writeTimeout">Timeout value used for write operations.</param>
        /// <param name="readTimeout">Timeout value used for read operations.</param>
        /// 
        public SerialCommunication( string portName, int writeTimeout, int readTimeout )
        {
            this.port = new SerialPort( portName );

            this.port.WriteTimeout = writeTimeout;
            this.port.ReadTimeout  = readTimeout;

            this.port.BaudRate  = 115200;
            this.port.DataBits  = 8;
            this.port.StopBits  = StopBits.One;
            this.port.Parity    = Parity.None;
            this.port.RtsEnable = true;
            this.port.DtrEnable = true;
        }

        /// <summary>
        /// Connect to NXT brick.
        /// </summary>
        /// 
        /// <returns>Returns <b>true</b> if connection was established successfully or <b>false</b>
        /// otherwise.</returns>
        /// 
        /// <remarks>If communication interface was connected before the call, existing connection will be reused.
        /// If it is required to force reconnection, then <see cref="Disconnect"/> method should be called before.
        /// </remarks>
        /// 
        public bool Connect( )
        {
            if ( !port.IsOpen )
            {
                // try to connect 
                try
                {
                    port.Open( );
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Disconnect from NXT brick.
        /// </summary>
        public void Disconnect( )
        {
            if ( port.IsOpen )
            {
                port.Close( );
            }
        }

        /// <summary>
        /// Send message to NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="message">Buffer containing the message to send.</param>
        /// 
        /// <returns>Returns <b>true</b> if message was sent successfully or <b>false</b>
        /// otherwise.</returns>
        /// 
        /// <remarks>This method assumes that message starts from the start of the
        /// specified buffer and occupies entire buffer.</remarks>
        /// 
        public bool SendMessage( byte[] message )
        {
            return SendMessage( message, 0, message.Length );
        }

        /// <summary>
        /// Send message to NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="message">Buffer containing the message to send.</param>
        /// <param name="length">Length of the message to send.</param>
        /// 
        /// <returns>Returns <b>true</b> if message was sent successfully or <b>false</b>
        /// otherwise.</returns>
        /// 
        /// <remarks>This method assumes that message starts from the start of the
        /// specified buffer.</remarks>
        /// 
        public bool SendMessage( byte[] message, int length )
        {
            return SendMessage( message, 0, length );
        }

        /// <summary>
        /// Send message to NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="message">Buffer containing the message to send.</param>
        /// <param name="offset">Offset of the message in the buffer.</param>
        /// <param name="length">Length of the message to send.</param>
        /// 
        /// <returns>Returns <b>true</b> if message was sent successfully or <b>false</b>
        /// otherwise.</returns>
        /// 
        public bool SendMessage( byte[] message, int offset, int length )
        {
            // check connection status
            if ( !port.IsOpen )
            {
                throw new NullReferenceException( "Serial port is not opened" );
            }

            // check message size
            if ( length > MaxMessageSize )
            {
                throw new ArgumentException( "Too large message" );
            }

            try
            {
                // prepare request buffer
                byte[] requestBuffer = new byte[length + 2];
                requestBuffer[0] = (byte) ( length & 0xFF );
                requestBuffer[1] = (byte) ( ( length & 0xFF00 ) >> 8 );
                Array.Copy( message, offset, requestBuffer, 2, length );
                // send actual request
                port.Write( requestBuffer, 0, requestBuffer.Length );
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Read message from NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="buffer">Buffer to use for message reading.</param>
        /// <param name="length">On successful return the variable keeps message length.</param>
        /// 
        /// <returns>Returns <b>true</b> if message was read successfully or <b>false</b>
        /// otherwise.</returns>
        /// 
        public bool ReadMessage( byte[] buffer, out int length )
        {
            return ReadMessage( buffer, 0, out length );
        }

        /// <summary>
        /// Read message from NXT brick over the communication interface.
        /// </summary>
        /// 
        /// <param name="buffer">Buffer to use for message reading.</param>
        /// <param name="offset">Offset in the buffer for message.</param>
        /// <param name="length">On successful return the variable keeps message length.</param>
        /// 
        /// <returns>Returns <b>true</b> if message was read successfully or <b>false</b>
        /// otherwise.</returns>
        /// 
        public bool ReadMessage( byte[] buffer, int offset, out int length )
        {
            // check connection status
            if ( !port.IsOpen )
            {
                throw new NullReferenceException( "Serial port is not opened" );
            }

            length = 0;

            try
            {
                // read 2 bytes of message length
                int lsb = port.ReadByte( );
                int msb = port.ReadByte( );
                int toRead = ( msb << 8 ) + lsb;
                // check buffer size
                if ( toRead > buffer.Length - offset )
                {
                    // remove incomming message from the port
                    while ( toRead != 0 )
                    {
                        port.ReadByte( );
                        toRead--;
                    }
                    throw new ArgumentException( "Reply buffer is too small" );
                }
                // read the message
                length = port.Read( buffer, offset, toRead );

                while ( length < toRead )
                {
                    buffer[offset + length] = (byte) port.ReadByte( );
                    length++;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
