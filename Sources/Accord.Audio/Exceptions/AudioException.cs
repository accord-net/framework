// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
// Copyright © AForge.NET, 2005-2011
// contacts@aforgenet.com
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
    ///   Audio related exception.
    /// </summary>
    /// 
    /// <remarks><para>The exception is thrown in the case of some audio related issues, like
    /// failure of initializing codec, compression, etc.</para></remarks>
    /// 
    [Serializable]
    public class AudioException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioException"/> class.
        /// </summary>
        /// 
        public AudioException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Exception's message.</param>
        /// 
        public AudioException(string message) :
            base(message) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public AudioException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The object that holds the serialized object data.</param>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// 
        protected AudioException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

    }
}
