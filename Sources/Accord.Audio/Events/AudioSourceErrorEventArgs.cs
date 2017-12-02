// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
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

    /// <summary>
    ///   Arguments for audio source error event from audio source.
    /// </summary>
    ///
    public class AudioSourceErrorEventArgs : EventArgs
    {
        private readonly string _description;
        private readonly Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSourceErrorEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="description">Error description.</param>
        /// 
        public AudioSourceErrorEventArgs(string description)
            : this(description, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSourceErrorEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="exception">Error exception.</param>
        /// 
        public AudioSourceErrorEventArgs(Exception exception)
            : this(exception.Message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioSourceErrorEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="description">Error description.</param>
        /// <param name="exception">Error exception.</param>
        /// 
        public AudioSourceErrorEventArgs(string description, Exception exception)
        {
            _description = description;
            _exception = exception;
        }

        /// <summary>
        /// Audio source error description.
        /// </summary>
        /// 
        public string Description
        {
            get { return _description; }
        }

        /// <summary>
        /// Audio source exception causing the error
        /// </summary>
        /// 
        public Exception Exception
        {
            get { return _exception; }
        }

    }

}