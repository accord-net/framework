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

namespace Accord.Statistics.Testing
{
    using Accord.Statistics.Distributions;

    /// <summary>
    ///   Common interface for Hypothesis tests depending on a statistical distribution.
    /// </summary>
    /// 
    /// <typeparam name="TDistribution">The test statistic distribution.</typeparam>
    /// 
    public interface IHypothesisTest<out TDistribution> : IHypothesisTest
        where TDistribution : IDistribution
    {
        /// <summary>
        ///   Gets the distribution associated
        ///   with the test statistic.
        /// </summary>
        /// 
        TDistribution StatisticDistribution { get; }
    }

    /// <summary>
    ///   Common interface for Hypothesis tests depending on a statistical distribution.
    /// </summary>
    /// 
    public interface IHypothesisTest
    {
        /// <summary>
        ///   Gets the test type.
        /// </summary>
        /// 
        DistributionTail Tail { get; }

        /// <summary>
        ///   Gets whether the null hypothesis should be rejected.
        /// </summary>
        /// 
        /// <remarks>
        ///   A test result is said to be statistically significant when the
        ///   result would be very unlikely to have occurred by chance alone.
        /// </remarks>
        /// 
        bool Significant { get; }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="x">The value of the test statistic.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        double StatisticToPValue(double x);

        /// <summary>
        ///   Converts a given p-value to a test statistic.
        /// </summary>
        /// 
        /// <param name="p">The p-value.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        double PValueToStatistic(double p);
    }
}
