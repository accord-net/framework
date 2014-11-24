// AForge Lego Robotics Library
// AForge.NET framework
//
// Copyright © Andrew Kirillov, 2007-2008
// andrew.kirillov@gmail.com
//

namespace AForge.Robotics.Lego.Internals
{
    using System;

    /// <summary>
    /// Interface, which wraps communication functions with Lego Mindstorms NXT brick.
    /// </summary>
    /// 
    internal interface INXTCommunicationInterface
    {
        /// <summary>
        /// Get connection status.
        /// </summary>
        /// 
        bool IsConnected { get; }

        /// <summary>
        /// Connect to NXT brick.
        /// </summary>
        /// 
        /// <returns>Returns <b>true</b> if connection was established successfully or <b>false</b>
        /// otherwise.</returns>
        /// 
        bool Connect( );

        /// <summary>
        /// Disconnect from NXT brick.
        /// </summary>
        /// 
        void Disconnect( );

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
        bool SendMessage( byte[] message );

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
        bool SendMessage( byte[] message, int length );

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
        bool SendMessage( byte[] message, int offset, int length );

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
        bool ReadMessage( byte[] buffer, out int length );

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
        bool ReadMessage( byte[] buffer, int offset, out int length );
    }
}
