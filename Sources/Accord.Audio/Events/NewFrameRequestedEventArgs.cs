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
    ///   Arguments for new frame request from an audio output device.
    /// </summary>
    /// 
    public class NewFrameRequestedEventArgs : EventArgs
    {
        private float[] buffer;

        /// <summary>
        ///   Gets or sets the buffer to be played in the audio source.
        /// </summary>
        /// 
        public float[] Buffer
        {
            get { return buffer; }
            set
            {
                if (value.Length > Frames)
                    throw new ArgumentException("The length of the buffer should be equal or less than the requested number of samples.");

                buffer = value;
            }
        }

        /// <summary>
        ///   Gets or sets whether the playing should stop.
        /// </summary>
        /// 
        public bool Stop { get; set; }

        /// <summary>
        ///   Gets the number of samples which should be placed in the buffer.
        /// </summary>
        /// 
        public int Frames { get; set; }

        /// <summary>
        ///   Optional field to inform the player which
        ///   is the current index of the frame being played.
        /// </summary>
        /// 
        public int FrameIndex { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NewFrameRequestedEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="frames">The number of samples being requested.</param>
        /// 
        public NewFrameRequestedEventArgs(int frames)
        {
            this.Frames = frames;
            this.buffer = new float[frames];
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NewFrameRequestedEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="buffer">The initial buffer.</param>
        /// 
        public NewFrameRequestedEventArgs(float[] buffer)
        {
            this.Frames = buffer.Length;
            this.buffer = buffer;
        }


    }

}