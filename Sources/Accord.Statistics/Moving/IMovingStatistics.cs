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

namespace Accord.Statistics.Moving
{
    using Accord.Statistics.Running;

    /// <summary>
    ///   Common interface for moving-window statistics.
    /// </summary>
    /// 
    /// <remarks>
    ///   Moving-window statistics such as moving average and moving variance,
    ///   are a type of finite impulse response filters used to analyze a set
    ///   of data points by creating a series of averages of different subsets
    ///   of the full data set.
    /// </remarks>
    /// 
    public interface IMovingStatistics : IRunningStatistics
    {

        /// <summary>
        ///   Gets the size of the window.
        /// </summary>
        /// 
        /// <value>The window's size.</value>
        /// 
        int Window { get; }

        /// <summary>
        ///   Gets the number of samples within the window.
        /// </summary>
        /// 
        /// <value>The number of samples within the window.</value>
        /// 
        int Count { get; }

    }

  
}
