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
    using Accord.Math;

    /// <summary>
    ///   Additive merge filter.
    /// </summary>
    /// 
    public class AddFilter : BaseFilter
    {

        /// <summary>
        ///   Gets or sets the signal to be overlayed (merged through
        ///   addition) with another signal.
        /// </summary>
        /// 
        public Signal OverlaySignal { get; set; }

        /// <summary>
        ///   Gets or sets a value indicating whether the resulting
        ///   signal should be normalized to be between -1 and 1 after
        ///   the merging has finished. Default is true.
        /// </summary>
        /// 
        public bool Normalize { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddFilter"/> class.
        /// </summary>
        /// 
        /// <param name="overlaySignal">The signal to be combined with another signal.</param>
        /// 
        public AddFilter(Signal overlaySignal)
        {
            this.OverlaySignal = overlaySignal;
            this.Normalize = true;

            FormatTranslations[SampleFormat.Format32BitIeeeFloat] = SampleFormat.Format32BitIeeeFloat;
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
                float* src = (float*)sourceData.Data.ToPointer() ;
                float* ovl = (float*)OverlaySignal.Data.ToPointer() ;
                float* dst = (float*)destinationData.Data.ToPointer() ;

                for (int i = 0; i < length; i++)
                    for (int j = 0; j < channels; j++, src++, dst++, ovl++)
                        *dst = *src + *ovl;

                if (Normalize)
                {
                    dst = (float*)destinationData.Data.ToPointer();

                    float[] min, max;
                    float[] len = UnsafeTools.GetRange(dst, channels, length, out min, out max);

                    for (int i = 0; i < length; i++)
                        for (int j = 0; j < channels; j++, dst++)
                            *dst = ((*dst - min[j]) / len[j]) * 2 - 1;

#if DEBUG
                    dst = (float*)destinationData.Data.ToPointer();
                    len = UnsafeTools.GetRange(dst, channels, length, out min, out max);
                    Accord.Diagnostics.Debug.Assert(len.IsEqual(2, atol: 1e-6f));
                    Accord.Diagnostics.Debug.Assert(min.IsEqual(-1, atol: 1e-6f));
                    Accord.Diagnostics.Debug.Assert(max.IsEqual(+1, atol: 1e-6f));
#endif
                }
            }

        }


    }
}
