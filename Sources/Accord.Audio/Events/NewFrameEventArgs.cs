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
    ///   Arguments for new block event from audio source.
    /// </summary>
    ///
    public class NewFrameEventArgs : EventArgs
    {
        private Signal signal;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewFrameEventArgs"/> class.
        /// </summary>
        ///
        /// <param name="signal">New signal frame.</param>
        ///
        public NewFrameEventArgs(Signal signal)
        {
            this.signal = signal;
        }

        /// <summary>
        ///   New Frame from audio source.
        /// </summary>
        ///
        public Signal Signal
        {
            get { return signal; }
        }

    }

}