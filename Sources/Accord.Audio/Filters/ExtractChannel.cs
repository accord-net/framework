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

namespace Accord.Audio.Filters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///   Extracts specified channel of a multiple-channel signal and returns it as a mono signal.
    /// </summary>
    /// 
    public class ExtractChannel : BaseFilter
    {

        private Dictionary<SampleFormat, SampleFormat> formatTranslations
            = new Dictionary<SampleFormat, SampleFormat>();


        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// 
        /// <value>The format translations.</value>
        /// 
        /// <remarks>
        ///   The dictionary defines which sample formats are supported for
        ///   source signals and which sample format will be used for resulting signal.
        /// </remarks>
        /// 
        public override Dictionary<SampleFormat, SampleFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Gets or sets the index of the channel
        ///   that should be extracted from signals.
        /// </summary>
        /// 
        public int Channel { get; set; }

        /// <summary>
        ///   Creates a new <see cref="ExtractChannel"/> filter.
        /// </summary>
        /// 
        /// <param name="channel">The index of the channel to be extracted.</param>
        /// 
        public ExtractChannel(int channel)
        {
            Channel = channel;

            formatTranslations[SampleFormat.Format32BitIeeeFloat] = SampleFormat.Format32BitIeeeFloat;
            formatTranslations[SampleFormat.Format16Bit] = SampleFormat.Format16Bit;
            formatTranslations[SampleFormat.Format32Bit] = SampleFormat.Format32Bit;
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
            int samples = sourceData.Samples;

            if (Channel < 0 || Channel > channels)
                throw new InvalidOperationException("The signal doesn't contains the specified channel.");

            if (format == SampleFormat.Format32BitIeeeFloat)
            {
                float* src = (float*)sourceData.Data.ToPointer() + Channel;
                float* dst = (float*)destinationData.Data.ToPointer();

                for (int i = 0; i < length; i++, src += channels, dst++)
                    *dst = *src;
            }
            else if (format == SampleFormat.Format16Bit)
            {
                short* src = (short*)sourceData.Data.ToPointer() + Channel;
                short* dst = (short*)destinationData.Data.ToPointer();

                for (int i = 0; i < length; i++, src += channels, dst++)
                    *dst = *src;
            }
            else if (format == SampleFormat.Format32Bit)
            {
                int* src = (int*)sourceData.Data.ToPointer() + Channel;
                int* dst = (int*)destinationData.Data.ToPointer();

                for (int i = 0; i < length; i++, src += channels, dst++)
                    *dst = *src;
            }
        }
    }
}
