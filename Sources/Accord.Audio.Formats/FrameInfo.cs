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

namespace Accord.Audio.Formats
{
    using System;
    using System.ComponentModel;

    /// <summary>
    ///   Information about a audio frame.
    /// </summary>
    /// 
    /// <remarks><para>This is a base class, which keeps basic information about a frame sample, like its
    /// sampling rate, bits per sample, etc. Classes, which inherit from this, may define more properties
    /// describing certain audio formats.</para></remarks>
    /// 
    public class FrameInfo : ICloneable
    {
        private int channels;
        private int samplingRate;
        private int bitsPerSample;
        private int frameIndex;
        private int totalFrames;


        /// <summary>
        ///   Number of channels.
        /// </summary>
        /// 
        [Category("General")]
        public int Channels
        {
            get { return channels; }
            set { channels = value; }
        }

        /// <summary>
        ///   Sampling rate.
        /// </summary>
        /// 
        [Category("General")]
        public int SamplingRate
        {
            get { return samplingRate; }
            set { samplingRate = value; }
        }

        /// <summary>
        ///   Number of bits per audio sample.
        /// </summary>
        /// 
        [Category("General")]
        public int BitsPerSample
        {
            get { return bitsPerSample; }
            set { bitsPerSample = value; }
        }

        /// <summary>
        ///   Frame's index.
        /// </summary>
        /// 
        [Category("General")]
        public int FrameIndex
        {
            get { return frameIndex; }
            set { frameIndex = value; }
        }

        /// <summary>
        ///   Total frames in the audio.
        /// </summary>
        /// 
        [Category("General")]
        public int TotalFrames
        {
            get { return totalFrames; }
            set { totalFrames = value; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FrameInfo"/> class.
        /// </summary>
        /// 
        public FrameInfo() { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FrameInfo"/> class.
        /// </summary>
        /// 
        public FrameInfo(int channels, int samplingRate, int bitsPerSample, int frameIndex, int totalFrames)
        {
            this.channels = channels;
            this.samplingRate = samplingRate;
            this.bitsPerSample = bitsPerSample;
            this.frameIndex = frameIndex;
            this.totalFrames = totalFrames;
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance. 
        /// </summary>
        /// 
        /// <returns>A new object that is a copy of this instance.</returns>
        /// 
        public virtual object Clone()
        {
            return new FrameInfo(channels, samplingRate, bitsPerSample, frameIndex, totalFrames);
        }
    }
}