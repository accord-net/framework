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
    using System.Collections.Generic;
    using Accord.Audio;

    /// <summary>
    ///  Time-domain envelope detector.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///  To extract the envelope of a time-domain signal, we must first compute
    ///  the absolute signal values and then pass it through a low-pass filter.</para>
    /// </remarks>
    /// 
    public class EnvelopeFilter : BaseFilter
    {
        private float alpha = 0.001f;

        /// <summary>
        ///   Alpha
        /// </summary>
        /// 
        public float Alpha
        {
            get { return alpha; }
            set { alpha = value; }
        }

        /// <summary>
        ///   Constructs a new Envelope filter
        /// </summary>
        /// 
        public EnvelopeFilter(float alpha)
        {
            this.alpha = alpha;

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
                float* src = (float*)sourceData.Data.ToPointer();
                float* dst = (float*)destinationData.Data.ToPointer();

                for (int i = 0; i < channels; i++, src++, dst++)
                    *dst = System.Math.Abs(*src);

                for (int i = channels; i < length; i++, src++, dst++)
                {
                    float abs = System.Math.Abs(*src);
                    *dst = dst[-channels] + Alpha * (abs - dst[-channels]);
                }
            }
        }

    }
}
