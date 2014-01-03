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
    using Accord.Audio;
    using SlimDX.Multimedia;

    /// <summary>
    ///   Extension methods.
    /// </summary>
    /// 
    public static class Extensions
    {

        /// <summary>
        ///   Converts a sample format into an appropriate <see cref="WaveFormatTag"/>.
        /// </summary>
        /// 
        /// <param name="sampleFormat">The sample format.</param>
        /// 
        public static WaveFormatTag ToWaveFormat(this SampleFormat sampleFormat)
        {
            switch (sampleFormat)
            {
                case SampleFormat.Format8Bit:
                case SampleFormat.Format32Bit:
                case SampleFormat.Format16Bit:
                    return WaveFormatTag.Pcm;

                case SampleFormat.Format64BitIeeeFloat:
                case SampleFormat.Format32BitIeeeFloat:
                    return WaveFormatTag.IeeeFloat;
            }

            throw new ArgumentOutOfRangeException("sampleFormat", "Unsupported sample format.");
        }

        /// <summary>
        ///   Converts a <see cref="WaveFormatTag"/> and bits per sample information
        ///   into an appropriate <see cref="SampleFormat"/>.
        /// </summary>
        /// 
        /// <param name="tag">The wave format tag.</param>
        /// <param name="bitsPerSample">The bits per sample.</param>
        /// 
        public static SampleFormat ToSampleFormat(this WaveFormatTag tag, int bitsPerSample)
        {
            if (tag == WaveFormatTag.Pcm)
            {
                if (bitsPerSample == 16)
                    return SampleFormat.Format16Bit;
                else if (bitsPerSample == 32)
                    return SampleFormat.Format32Bit;
            }
            else if (tag == WaveFormatTag.IeeeFloat)
            {
                if (bitsPerSample == 32)
                    return SampleFormat.Format32BitIeeeFloat;
                else if (bitsPerSample == 64)
                    return SampleFormat.Format64BitIeeeFloat;
            }

            throw new ArgumentOutOfRangeException("tag", "Unsupported format tag.");
        }
    }
}
