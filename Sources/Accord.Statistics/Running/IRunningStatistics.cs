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

namespace Accord.Statistics.Running
{

    /// <summary>
    ///   Common interface for running statistics.
    /// </summary>
    /// <remarks>
    ///   Running statistics are measures computed as data becomes available.
    ///   When using running statistics, there is no need to know the number of
    ///   samples a priori, such as in the case of the direct <see cref="Tools.Mean(double[])"/>.
    /// </remarks>
    /// 
    public interface IRunning<in TValue>
    {

        /// <summary>
        ///   Registers the occurrence of a value.
        /// </summary>
        /// 
        /// <param name="value">The value to be registered.</param>
        /// 
        void Push(TValue value);

        /// <summary>
        ///   Clears all measures previously computed.
        /// </summary>
        /// 
        void Clear();

    }

    /// <summary>
    ///   Common interface for running statistics.
    /// </summary>
    /// <remarks>
    ///   Running statistics are measures computed as data becomes available.
    ///   When using running statistics, there is no need to know the number of
    ///   samples a priori, such as in the case of the direct <see cref="Tools.Mean(double[])"/>.
    /// </remarks>
    /// 
    public interface IRunningStatistics : IRunning<double>
    {

        /// <summary>
        ///   Gets the current mean of the gathered values.
        /// </summary>
        /// 
        /// <value>The mean of the values.</value>
        /// 
        double Mean { get; }

        /// <summary>
        ///   Gets the current variance of the gathered values.
        /// </summary>
        /// 
        /// <value>The variance of the values.</value>
        /// 
        double Variance { get; }

        /// <summary>
        ///   Gets the current standard deviation of the gathered values.
        /// </summary>
        /// 
        /// <value>The standard deviation of the values.</value>
        /// 
        double StandardDeviation { get; }

    }
}
