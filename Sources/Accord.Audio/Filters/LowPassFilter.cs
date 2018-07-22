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
    ///   Low band pass filter.
    /// </summary>
    /// 
    /// <remarks>
    /// The low-pass filter is computed using
    /// <code>
    ///     for (int t = 0; i &lt; length; t++)
    ///         y = y[t-1] + Alpha * (x[t] - y[t-1]);
    /// </code>
    /// </remarks>
    /// 
    /// <example>
    /// <code source="Unit Tests\Accord.Tests.Audio\Filters\LowPassFilterTest.cs" region="doc_example1" />
    /// </example>
    /// 
    public class LowPassFilter : BaseFilter
    {

        /// <summary>
        ///   Gets or sets the low-pass alpha value.
        /// </summary>
        /// 
        public float Alpha { get; set; }

        /// <summary>
        ///   Gets the alpha value that can be used to achieve a given
        ///   cut-off frequency under a given sampling rate.
        /// </summary>
        /// 
        /// <param name="frequency">The desired cut-off frequency.</param>
        /// <param name="sampleRate">The signal sampling rate.</param>
        /// 
        /// <returns>A value for <see cref="Alpha"/> that creates a filter
        ///   that can filter out the given cut-off frequency.</returns>
        /// 
        public static float GetAlpha(double frequency, double sampleRate)
        {
            double rc = 1.0 / (2 * Math.PI * frequency);
            double dt = 1.0 / sampleRate;
            return (float)(dt / (dt + rc));
        }

        /// <summary>
        ///   Constructs a new Low-Pass Filter using the given cut-off frequency and sample rate.
        /// </summary>
        /// 
        /// <param name="frequency">The desired cut-off frequency.</param>
        /// <param name="sampleRate">The signal sampling rate.</param>
        /// 
        public LowPassFilter(double frequency, double sampleRate)
            : this(GetAlpha(frequency, sampleRate))
        {
        }

        /// <summary>
        ///   Constructs a new Low-Pass Filter using the given alpha.
        /// </summary>
        /// 
        /// <param name="alpha">Band pass alpha.</param>
        /// 
        public LowPassFilter(float alpha)
        {
            Alpha = alpha;

            FormatTranslations[SampleFormat.Format32BitIeeeFloat] = SampleFormat.Format32BitIeeeFloat;
        }

        /// <summary>
        ///   Processes the filter.
        /// </summary>
        /// 
        protected override void ProcessFilter(Signal sourceData, Signal destinationData)
        {
            SampleFormat format = sourceData.SampleFormat;
            int channels = sourceData.NumberOfChannels;
            int length = sourceData.Length;

            if (format == SampleFormat.Format32BitIeeeFloat)
            {
                unsafe
                {
                    float* src = (float*)sourceData.Data.ToPointer();
                    float* dst = (float*)destinationData.Data.ToPointer();

                    // Copy the first frame
                    for (int j = 0; j < channels; j++)
                        dst[j] = src[j];

                    // Process rest of the frames
                    for (int i = 1; i < length; i++)
                        for (int j = 0; j < channels; j++, src++, dst++)
                            dst[0] = dst[-channels] + Alpha * (src[0] - dst[-channels]);
                }
            }

        }

    }
}
