// Accord Core Library
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

namespace Accord
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;

    /// <summary>
    ///   An extension of <see cref="System.Threading.Interlocked"/> providing
    ///   atomic operations such as Add and Increment to floating point numbers.
    /// </summary>
    /// 
    public class InterlockedEx
    {
        /// <summary>
        ///   Adds two 32-bit floating point values and replaces the first
        ///   double value with their sum, as an atomic operation.
        /// </summary>
        /// 
        /// <param name="location1">The first variable to be added.</param>
        /// <param name="value">The second variable to be added.</param>
        /// 
        /// <returns>The updated value of the first variable.</returns>
        /// 
        public static double Add(ref double location1, double value)
        {
            double newCurrentValue = 0;
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + value;
                newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
                if (newCurrentValue == currentValue)
                    return newValue;
            }
        }

        /// <summary>
        ///   Increments a specified variable and stores the result, as an atomic operation.
        /// </summary>
        /// 
        /// <param name="location1">The variable to be incremented.</param>
        /// 
        /// <returns>The updated value of the variable.</returns>
        /// 
        public static double Increment(ref double location1)
        {
            double newCurrentValue = 0;
            while (true)
            {
                double currentValue = newCurrentValue;
                double newValue = currentValue + 1;
                newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);
                if (newCurrentValue == currentValue)
                    return newValue;
            }
        }
    }
}
