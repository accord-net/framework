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

namespace Accord.Audio.Filters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Volume adjustment filter.
    /// </summary>
    /// 
    public class VolumeFilter : BaseInPlaceFilter
    {

        /// <summary>
        ///   Gets or sets the volume multiplier.
        /// </summary>
        /// 
        public float Volume { get; set; }

        /// <summary>
        ///   Constructs a new Volume adjustment filter using the given alpha.
        /// </summary>
        /// 
        /// <param name="volume">Volume multiplier.</param>
        /// 
        public VolumeFilter(float volume)
        {
            Volume = volume;

            FormatTranslations[SampleFormat.Format32BitIeeeFloat] = SampleFormat.Format32BitIeeeFloat;
        }


        /// <summary>
        ///   Processes the filter.
        /// </summary>
        /// 
        protected override void ProcessFilter(Signal sourceData, Signal destinationData)
        {
            SampleFormat format = sourceData.SampleFormat;
            int channels = sourceData.Channels;
            int length = sourceData.Length;

            if (format == SampleFormat.Format32BitIeeeFloat)
            {
                unsafe
                {
                    float* src = (float*)sourceData.Data.ToPointer();
                    float* dst = (float*)destinationData.Data.ToPointer();

                    for (int i = 0; i < length * channels; i++, src++, dst++)
                    {
                        *dst = Volume * (*src);
                    }
                }
            }
            else if (format == SampleFormat.Format16Bit)
            {
                unsafe
                {
                    short* src = (short*)sourceData.Data.ToPointer();
                    short* dst = (short*)destinationData.Data.ToPointer();

                    for (int i = 0; i < length * channels; i++, src++, dst++)
                    {
                        *dst = (short)(Volume * (*src));
                    }
                }
            }
            else if (format == SampleFormat.Format32Bit)
            {
                unsafe
                {
                    int* src = (int*)sourceData.Data.ToPointer();
                    int* dst = (int*)destinationData.Data.ToPointer();

                    for (int i = 0; i < length * channels; i++, src++, dst++)
                    {
                        *dst = (int)(Volume * (*src));
                    }
                }
            }

        }

    }
}
