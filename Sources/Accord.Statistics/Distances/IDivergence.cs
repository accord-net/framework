// Accord Math Library
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

namespace Accord.Statistics.Distances
{
    using Accord.Math.Distances;
    using Accord.Statistics.Distributions;

    /// <summary>
    ///   Common interface for divergence measures (between
    ///   probability distributions).
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the first distribution to be compared.</typeparam>
    /// <typeparam name="U">The type of the second distribution to be compared.</typeparam>
    /// 
    public interface IDivergence<in T, in U> : IDistance<T, U>
        where T : IDistribution
        where U : IDistribution
    {
    }

    /// <summary>
    ///   Common interface for divergence measures (between
    ///   probability distributions).
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the distributions to be compared.</typeparam>
    /// 
    public interface IDivergence<in T> : IDivergence<T, T>, IDistance<T>
        where T : IDistribution
    {
    }
}
