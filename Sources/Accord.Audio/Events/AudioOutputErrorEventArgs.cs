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

    /// <summary>
    ///   Arguments for audio source error event from audio source.
    /// </summary>
    ///
    public class AudioOutputErrorEventArgs : EventArgs
    {
        private string description;

        /// <summary>
        ///   Initializes a new instance of the <see cref="AudioSourceErrorEventArgs"/> class.
        /// </summary>
        ///
        /// <param name="description">Error description.</param>
        ///
        public AudioOutputErrorEventArgs(string description)
        {
            this.description = description;
        }

        /// <summary>
        ///   Audio source error description.
        /// </summary>
        ///
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        ///   Represents an event with no event data.
        /// </summary>
        public static readonly new AudioOutputErrorEventArgs Empty = new AudioOutputErrorEventArgs(String.Empty);

    }
}