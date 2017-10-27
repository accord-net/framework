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
    ///   Converts a multi-channel (e.g. stereo) stream into a mono stream.
    /// </summary>
    /// 
    public class MonoFilter : BaseFilter
    {

        /// <summary>
        ///   Creates a new <see cref="MonoFilter"/> filter.
        /// </summary>
        /// 
        public MonoFilter()
        {
            FormatTranslations[SampleFormat.Format32BitIeeeFloat] = SampleFormat.Format32BitIeeeFloat;
            FormatTranslations[SampleFormat.Format16Bit] = SampleFormat.Format16Bit;
            FormatTranslations[SampleFormat.Format32Bit] = SampleFormat.Format32Bit;
        }

        /// <summary>
        ///   Creates a new signal from the given signal parameters. This
        ///   method can be overridden on child classes to modify how
        ///   output signals are created.
        /// </summary>
        /// 
        protected override Signal NewSignal(int channels, int samples, int rate, SampleFormat dstSampleFormat)
        {
            return new Signal(1, samples, rate, dstSampleFormat);
        }

        /// <summary>
        ///   Processes the filter.
        /// </summary>
        /// 
        protected unsafe override void ProcessFilter(Signal sourceData, Signal destinationData)
        {
            SampleFormat format = sourceData.SampleFormat;
            int channels = sourceData.Channels;
            int length = sourceData.Length;

            if (format == SampleFormat.Format32BitIeeeFloat)
            {
                var src = (float*)sourceData.Data.ToPointer();
                var dst = (float*)destinationData.Data.ToPointer();

                for (int i = 0; i < length; i++, dst++)
                {
                    float avg = 0;
                    for (int c = 0; c < channels; c++, src++)
                        avg += *src;
                    *dst = avg / channels;
                }
            }
            else if (format == SampleFormat.Format16Bit)
            {
                var src = (short*)sourceData.Data.ToPointer();
                var dst = (short*)destinationData.Data.ToPointer();

                for (int i = 0; i < length; i++, dst++)
                {
                    short avg = 0;
                    for (int c = 0; c < channels; c++, src++)
                        avg += *src;
                    *dst = (short)(avg / channels);
                }
            }
            else if (format == SampleFormat.Format32Bit)
            {
                var src = (int*)sourceData.Data.ToPointer();
                var dst = (int*)destinationData.Data.ToPointer();

                for (int i = 0; i < length; i++, dst++)
                {
                    int avg = 0;
                    for (int c = 0; c < channels; c++, src++)
                        avg += *src;
                    *dst = avg / channels;
                }
            }
        }
    }
}
