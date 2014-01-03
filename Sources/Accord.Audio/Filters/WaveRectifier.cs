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
    using System.Collections.Generic;
    using AForge.Math;

    /// <summary>
    ///   Wave Rectifier filter.
    /// </summary>
    /// 
    public class WaveRectifier : BaseFilter
    {

        private Dictionary<SampleFormat, SampleFormat> formatTranslations = new Dictionary<SampleFormat, SampleFormat>();

        /// <summary>
        ///   Format translations dictionary.
        /// </summary>
        /// <value>The format translations.</value>
        /// <remarks>
        ///   The dictionary defines which sample formats are supported for
        ///   source signals and which sample format will be used for resulting signal.
        /// </remarks>
        public override Dictionary<SampleFormat, SampleFormat> FormatTranslations
        {
            get { return formatTranslations; }
        }

        /// <summary>
        ///   Gets or sets whether half rectification should be performed.
        /// </summary>
        public bool Half { get; set; }

        /// <summary>
        ///   Constructs a new Wave rectifier.
        /// </summary>
        public WaveRectifier(bool halfRectificationOnly)
        {
            Half = halfRectificationOnly;

            formatTranslations[SampleFormat.Format128BitComplex] = SampleFormat.Format128BitComplex;
            formatTranslations[SampleFormat.Format32BitIeeeFloat] = SampleFormat.Format32BitIeeeFloat;
        }

        /// <summary>
        ///   Applies the filter to a signal.
        /// </summary>
        protected override void ProcessFilter(Signal sourceData, Signal destinationData)
        {
            SampleFormat format = sourceData.SampleFormat;
            int channels = sourceData.Channels;
            int length = sourceData.Length;
            int samples = sourceData.Samples;

            if (format == SampleFormat.Format32BitIeeeFloat)
            {
                unsafe
                {
                    float* src = (float*)sourceData.Data.ToPointer();
                    float* dst = (float*)destinationData.Data.ToPointer();

                    for (int i = 0; i < samples; i++, dst++, src++)
                        *dst = System.Math.Abs(*src);
                }
            }
            else if (format == SampleFormat.Format128BitComplex)
            {
                unsafe
                {
                    Complex* src = (Complex*)sourceData.Data.ToPointer();
                    Complex* dst = (Complex*)destinationData.Data.ToPointer();

                    Complex c = new Complex();

                    for (int i = 0; i < samples; i++, dst++, src++)
                    {
                        c.Re = (*src).Magnitude;
                        *dst = c;
                    }
                }
            }

        }



    }
}
