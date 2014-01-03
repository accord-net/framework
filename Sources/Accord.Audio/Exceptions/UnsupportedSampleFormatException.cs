// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Audio
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///   Unsupported sample format exception.
    /// </summary>
    /// 
    /// <remarks>
    ///   <para>
    ///   The unsupported sample format exception is thrown in the case when a signal
    ///   is passed to a signal processing routine which is not prepared to handle its
    ///   format.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class UnsupportedSampleFormatException : ArgumentException
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="UnsupportedSampleFormatException"/> class.
        /// </summary>
        /// 
        public UnsupportedSampleFormatException() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="UnsupportedSampleFormatException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public UnsupportedSampleFormatException(string message) :
            base(message) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="UnsupportedSampleFormatException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="paramName">Name of the invalid parameter.</param>
        /// 
        public UnsupportedSampleFormatException(string message, string paramName) :
            base(message, paramName) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="UnsupportedSampleFormatException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public UnsupportedSampleFormatException(string message, Exception innerException) :
            base(message, innerException) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="UnsupportedSampleFormatException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// 
        protected UnsupportedSampleFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }
}
