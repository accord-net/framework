// AForge Core Library
// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

namespace AForge
{
    using System;

    /// <summary>
    /// Event arguments holding a buffer sent or received during some communication process.
    /// </summary>
    public class CommunicationBufferEventArgs : EventArgs
    {
        private readonly byte[] message;
        private readonly int index;
        private readonly int length;

        /// <summary>
        /// Length of the transfered message.
        /// </summary>
        public int MessageLength
        {
            get { return length; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationBufferEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message being transfered during communication process.</param>
        /// 
        public CommunicationBufferEventArgs( byte[] message )
 		{
 			this.message = message;
            this.index = 0;
            this.length = message.Length;
 		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationBufferEventArgs"/> class.
        /// </summary>
        ///
        /// <param name="buffer">Buffer containing the message being transferred during communication process.</param>
        /// <param name="index">Starting index of the message within the buffer.</param>
        /// <param name="length">Length of the message within the buffer.</param>
        ///
        public CommunicationBufferEventArgs( byte[] buffer, int index, int length )
        {
            this.message = buffer;
            this.index = index;
            this.length = length;
        }

        /// <summary>
        /// Get the transfered message.
        /// </summary>
        /// 
        /// <returns>Returns copy of the transfered message.</returns>
        /// 
        public byte[] GetMessage( )
        {
            byte[] ret = new byte[length];
            Array.Copy( message, index, ret, 0, length );
            return ret;
        }

        /// <summary>
        /// Get the transferred message as string.
        /// </summary>
        /// 
        /// <returns>Returns string encoding the transferred message.</returns>
        ///
        public string GetMessageString( )
        {
            return BitConverter.ToString( message, index, length );
        }
    }
}
