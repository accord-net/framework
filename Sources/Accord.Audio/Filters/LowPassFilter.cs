﻿// Accord Audio Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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
    ///   Low band pass filter.
    /// </summary>
    /// 
    public class LowPassFilter : BaseFilter
    {

        private Dictionary<SampleFormat, SampleFormat> formatTranslations = new Dictionary<SampleFormat, SampleFormat>();

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
        ///   Gets or sets the low-pass alpha.
        /// </summary>
        /// 
        public float Alpha { get; set; }

        /// <summary>
        ///   Constructs a new Low-Pass Filter using the given alpha.
        /// </summary>
        /// 
        /// <param name="alpha">Band pass alpha.</param>
        /// 
        public LowPassFilter(float alpha)
        {
            Alpha = alpha;

            formatTranslations[SampleFormat.Format32BitIeeeFloat] = SampleFormat.Format32BitIeeeFloat;
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
                float* src = (float*)sourceData.Data.ToPointer() + channels;
                float* dst = (float*)destinationData.Data.ToPointer() + channels;

                for (int i = channels; i < length; i++, src++, dst++)
                {
                    *dst = dst[-channels] + Alpha * (src[0] - dst[-channels]);
                }
            }

        }

    }
}
