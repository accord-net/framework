// Accord Statistics Library
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

namespace Accord.Statistics.Links
{
    using System;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Threshold link function.
    /// </summary>
    /// 
    [Serializable]
    public class ThresholdLinkFunction : ILinkFunction
    {

        /// <summary>
        ///   Threshold coefficient b.
        /// </summary>
        /// 
        public double Threshold { get; set; }

        /// <summary>
        ///   Creates a new Absolute link function.
        /// </summary>
        /// 
        /// <param name="threshold">The threshold value.</param>
        /// 
        public ThresholdLinkFunction(double threshold)
        {
            this.Threshold = threshold;
        }

        /// <summary>
        ///   Creates a new Absolute link function.
        /// </summary>
        /// 
        public ThresholdLinkFunction() : this(0) { }

        /// <summary>
        ///   The Absolute link function.
        /// </summary>
        /// 
        /// <param name="x">An input value.</param>
        /// 
        /// <returns>The transformed input value.</returns>
        /// 
        public double Function(double x)
        {
            return x;
        }

        /// <summary>
        ///   The mean function.
        /// </summary>
        /// 
        /// <param name="x">A transformed value.</param>
        /// 
        /// <returns>The reverse transformed value.</returns>
        /// 
        public double Inverse(double x)
        {
            return (x > Threshold) ? +1 : -1;
        }

        /// <summary>
        ///   First derivative of the <see cref="Inverse"/> function.
        /// </summary>
        /// 
        /// <param name="x">The input value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        public double Derivative(double x)
        {
            return 0;
        }

        /// <summary>
        ///   First derivative of the <see cref="Inverse"/>
        ///   function expressed in terms of it's output.
        /// </summary>
        /// 
        /// <param name="y">The reverse transformed value.</param>
        /// 
        /// <returns>The first derivative of the input value.</returns>
        /// 
        public double Derivative2(double y)
        {
            return 0;
        }

        /// <summary>
        ///   Creates a new object that is a copy of the current instance.
        /// </summary>
        /// 
        /// <returns>
        ///   A new object that is a copy of this instance.
        /// </returns>
        /// 
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
