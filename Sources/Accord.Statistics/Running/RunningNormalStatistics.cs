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
    using System;

    /// <summary>
    ///   Running (normal) statistics.
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// <para>
    ///   This class computes the running variance using Welford’s method. Running statistics 
    ///   need only one pass over the data, and do not require all data to be available prior
    ///   to computing.
    /// </para>
    /// 
    /// <para>    
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://www.johndcook.com/standard_deviation.html">
    ///       John D. Cook. Accurately computing running variance. Available on:
    ///       http://www.johndcook.com/standard_deviation.html</a></description></item>
    ///     <item><description>
    ///       Chan, Tony F.; Golub, Gene H.; LeVeque, Randall J. (1983). Algorithms for 
    ///       Computing the Sample Variance: Analysis and Recommendations. The American
    ///       Statistician 37, 242-247.</description></item>
    ///     <item><description>
    ///       Ling, Robert F. (1974). Comparison of Several Algorithms for Computing Sample
    ///       Means and Variances. Journal of the American Statistical Association, Vol. 69,
    ///       No. 348, 859-866.</description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    public class RunningNormalStatistics : IRunningStatistics
    {
        private int count;

        private double mean;
        private double sigma;

        private double lastMean;
        private double lastSigma;

        /// <summary>
        /// Gets the current mean of the gathered values.
        /// </summary>
        /// 
        /// <value>The mean of the values.</value>
        /// 
        public double Mean { get { return mean; } }

        /// <summary>
        /// Gets the current variance of the gathered values.
        /// </summary>
        /// 
        /// <value>The variance of the values.</value>
        /// 
        public double Variance
        {
            get { return (count > 1) ? sigma / (count - 1) : 0.0; }
        }

        /// <summary>
        /// Gets the current standard deviation of the gathered values.
        /// </summary>
        /// 
        /// <value>The standard deviation of the values.</value>
        /// 
        public double StandardDeviation
        {
            get { return Math.Sqrt(Variance); }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RunningNormalStatistics"/> class.
        /// </summary>
        /// 
        public RunningNormalStatistics() { }

        /// <summary>
        ///   Registers the occurrence of a value.
        /// </summary>
        /// 
        /// <param name="value">The value to be registered.</param> 
        /// 
        public void Push(double value)
        {
            count++;

            // See Knuth TAOCP vol 2, 3rd edition, page 232
            // http://www.johndcook.com/standard_deviation.html

            if (count == 1)
            {
                mean = lastMean = value;
                sigma = lastSigma = 0.0;
            }
            else
            {
                mean = lastMean + (value - lastMean) / count;
                sigma = (lastSigma + (value - lastMean) * (value - Mean));

                lastMean = mean;
                lastSigma = sigma;
            }
        }

        /// <summary>
        ///   Clears all measures previously computed.
        /// </summary>
        /// 
        public void Clear()
        {
            count = 0;

            mean = lastMean = 0;
            sigma = lastSigma = 0;
        }
    }
}
