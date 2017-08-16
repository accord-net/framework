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

namespace Accord.Audio.ComplexFilters
{
    using Accord.Compat;
    using System.Numerics;

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
        protected unsafe override void ProcessFilter(ComplexSignal sourceData, ComplexSignal destinationData)
        {
            int length = sourceData.Length;

            Complex* src = (Complex*)sourceData.Data.ToPointer();
            Complex* dst = (Complex*)destinationData.Data.ToPointer();

            for (int i = 0; i < length - 1; i++, src++, dst++)
            {
                double re = src[i + 1].Real - src[i].Real;

                // Retain only if difference is positive
                *dst = (re > 0) ? new Complex(re, 0) : Complex.Zero;
            }
        }

    }
}
