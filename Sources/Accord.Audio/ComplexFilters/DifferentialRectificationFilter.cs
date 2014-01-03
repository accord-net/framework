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
    using AForge.Math;

    /// <summary>
    ///   Differential Rectification filter.
    /// </summary>
    /// 
    public class DifferentialRectificationFilter : BaseComplexFilter
    {



        /// <summary>
        ///   Constructs a new Differential Rectification filter.
        /// </summary>
        /// 
        public DifferentialRectificationFilter()
        {
        }

        /// <summary>
        ///   Processes the filter.
        /// </summary>
        /// 
        protected override void ProcessFilter(ComplexSignal sourceData, ComplexSignal destinationData)
        {
            SampleFormat format = sourceData.SampleFormat;
            int channels = sourceData.Channels;
            int length = sourceData.Length;
            int samples = sourceData.Samples;

            unsafe
            {
                Complex* src = (Complex*)sourceData.Data.ToPointer();
                Complex* dst = (Complex*)destinationData.Data.ToPointer();

                Complex d = new Complex();

                for (int i = 0; i < length - 1; i++, src++, dst++)
                {
                    d.Re = src[i + 1].Re - src[i].Re;

                    // Retain only if difference is positive
                    *dst = (d.Re > 0) ? d : Complex.Zero;
                }
            }
        }


    }
}
