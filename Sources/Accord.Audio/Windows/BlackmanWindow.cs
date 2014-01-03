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

namespace Accord.Audio.Windows
{
    using System;

    /// <summary>
    ///   Blackman window.
    /// </summary>
    /// 
    /// <remarks>
    ///   By common convention, the unqualified term Blackman window refers to α=0.16.
    /// </remarks>
    /// 
    public class BlackmanWindow : WindowBase
    {

        /// <summary>
        ///   Constructs a new Blackman window.
        /// </summary>
        /// 
        /// <param name="length">The length for the window.</param>
        /// 
        public BlackmanWindow(int length)
            : this(0.16, length)
        {
        }

        /// <summary>
        ///   Constructs a new Blackman window.
        /// </summary>
        /// 
        /// <param name="alpha">Blackman's alpha</param>
        /// <param name="length">The length for the window.</param>
        /// 
        public BlackmanWindow(double alpha, int length) : base(length)
        {
            double a0 = (1.0 - alpha) / 2.0;
            double a1 = 0.5;
            double a2 = alpha / 2.0;


            for (int i = 0; i < length; i++)
                this[i] = (float)(a0 - a1 * Math.Cos((2.0 * System.Math.PI * i) / (length - 1))
                    + a2 * Math.Cos((4.0 * System.Math.PI * i) / (length - 1)));
        }


    }
}
