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
    public class PlayFrameEventArgs : EventArgs
    {
        private int frameIndex;
        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewFrameEventArgs"/> class.
        /// </summary>
        ///
        /// <param name="frameIndex">New frame index.</param>
        /// <param name="count">The number of frames to play.</param>
        ///
        public PlayFrameEventArgs(int frameIndex, int count)
        {
            this.frameIndex = frameIndex;
            this.count = count;
        }

        /// <summary>
        ///   New block from audio source.
        /// </summary>
        ///
        public int FrameIndex
        {
            get { return frameIndex; }
        }

        /// <summary>
        ///   Gets how many frames
        ///   are going to be played.
        /// </summary>
        /// 
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        ///   Represents an event with no event data.
        /// </summary>
        /// 
        public static readonly new PlayFrameEventArgs Empty = new PlayFrameEventArgs(0, 0);

    }

}