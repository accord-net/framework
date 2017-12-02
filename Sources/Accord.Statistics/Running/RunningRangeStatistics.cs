// Accord Statistics Library
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

namespace Accord.Statistics.Running
{
    using System;
    using Accord.Compat;

    /// <summary>
    ///   Running (range) statistics.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   This class computes the running minimum and maximum values in a stream of values. 
    ///   Running statistics need only one pass over the data, and do not require all data 
    ///   to be available prior to computing.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class RunningRangeStatistics : IRunning<double>
    {
        /// <summary>
        ///   Gets the number of samples seen.
        /// </summary>
        /// 
        public int Count { get; private set; }

        /// <summary>
        ///   Gets the minimum value seen.
        /// </summary>
        /// 
        public double Min { get; private set; }

        /// <summary>
        ///   Gets the maximum value seen.
        /// </summary>
        /// 
        public double Max { get; private set; }


        /// <summary>
        ///   Initializes a new instance of the <see cref="RunningRangeStatistics"/> class.
        /// </summary>
        /// 
        public RunningRangeStatistics()
        {
        }

        /// <summary>
        ///   Registers the occurrence of a value.
        /// </summary>
        /// 
        /// <param name="value">The value to be registered.</param> 
        /// 
        public void Push(double value)
        {
            Count++;

            if (Count == 1)
            {
                Min = Max = value;
            }
            else
            {
                if (value > Max)
                    Max = value;
                if (value < Min)
                    Min = value;
            }
        }

        /// <summary>
        ///   Clears all measures previously computed.
        /// </summary>
        /// 
        public void Clear()
        {
            Min = Max = Count = 0;
        }
    }
}
