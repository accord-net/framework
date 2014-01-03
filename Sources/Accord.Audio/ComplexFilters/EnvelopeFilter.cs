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

namespace Accord.Audio.ComplexFilters
{
    using System;
    using Accord.Audio;
    using AForge.Math;

    /// <summary>
    ///   Hilbert transform based envelope detector.
    /// </summary>
    /// 
    /// <remarks>
    ///  <para>
    ///   This method works by creating the analytic signal of the input by
    ///   using a Hilbert transform. An analytic signal is a complex signal,
    ///   where the real part is the original signal and the imaginary part
    ///   is the Hilbert transform of the original signal.</para>
    ///  <para>
    ///   The complex envelope of a signal can be found by taking the absolute
    ///   (magnitude) value of the analytic signal.</para>
    ///  <para>
    ///    References: http://en.wikipedia.org/wiki/Hilbert_transform
    ///  </para>
    /// </remarks>
    /// 
    public class EnvelopeFilter : BaseComplexFilter
    {

        /// <summary>
        ///   Constructs a new Envelope filter.
        /// </summary>
        public EnvelopeFilter()
        {
        }

        /// <summary>
        ///   Processes the filter.
        /// </summary>
        protected override void ProcessFilter(ComplexSignal sourceData, ComplexSignal destinationData)
        {
            if (sourceData.Status != ComplexSignalStatus.Analytic)
                throw new ArgumentException("Signal must be in analytic form.", "sourceData");

            SampleFormat format = sourceData.SampleFormat;
            int channels = sourceData.Channels;
            int length = sourceData.Length;
            int samples = sourceData.Samples;

            Complex d = new Complex();

            unsafe
            {
                Complex* src = (Complex*)sourceData.Data.ToPointer();
                Complex* dst = (Complex*)destinationData.Data.ToPointer();

                for (int i = 0; i < samples; i++, src++, dst++)
                {
                    d.Re = (*dst).Magnitude;
                    *dst = d;
                }
            }
        }

    }
}
