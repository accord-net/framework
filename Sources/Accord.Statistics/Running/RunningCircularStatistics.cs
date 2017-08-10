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
    ///   Running circular statistics.
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
    [Serializable]
    public class RunningCircularStatistics : IRunningStatistics
    {
        private int count;

        /// <summary>
        ///   Gets the sum of the sines of the angles within the window.
        /// </summary>
        /// 
        public double SumOfSines { get; private set; }

        /// <summary>
        ///   Gets the sum of the cosines of the angles within the window.
        /// </summary>
        /// 
        public double SumOfCosines { get; private set; }


        /// <summary>
        /// Gets the current mean of the gathered values.
        /// </summary>
        /// 
        /// <value>The mean of the values.</value>
        /// 
        public double Mean { get; private set; }

        /// <summary>
        /// Gets the current variance of the gathered values.
        /// </summary>
        /// 
        /// <value>The variance of the values.</value>
        /// 
        public double Variance { get; private set; }

        /// <summary>
        /// Gets the current standard deviation of the gathered values.
        /// </summary>
        /// 
        /// <value>The standard deviation of the values.</value>
        /// 
        public double StandardDeviation { get; private set; }

        /// <summary>
        /// Gets the current length of the sample mean resultant vector of the gathered values.
        /// </summary>
        /// 
        public double Rho { get; private set; }

        /// <summary>
        /// Gets the current count of values seen.
        /// </summary>
        /// 
        /// <value>The number of samples seen.</value>
        /// 
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="RunningCircularStatistics"/> class.
        /// </summary>
        /// 
        public RunningCircularStatistics() { }

        /// <summary>
        ///   Registers the occurrence of a value.
        /// </summary>
        /// 
        /// <param name="value">The value to be registered.</param> 
        /// 
        public void Push(double value)
        {
            count++;

            double cos = Math.Cos(value);
            double sin = Math.Sin(value);

            SumOfSines += sin;
            SumOfCosines += cos;

            double N = count;

            Rho = Math.Sqrt(SumOfSines * SumOfSines + SumOfCosines * SumOfCosines);
            Mean = Math.Atan2(SumOfSines / N, SumOfCosines / N);
            Variance = Math.Max(0, 1.0 - Rho / N);
            StandardDeviation = Math.Sqrt(-2.0 * Math.Log(Rho / N));
        }

        /// <summary>
        ///   Clears all measures previously computed.
        /// </summary>
        /// 
        public void Clear()
        {
            count = 0;

            Rho = 0;
            Mean = 0;
            Variance = 0;
            StandardDeviation = 0;

            SumOfSines = 0;
            SumOfCosines = 0;
        }
    }
}
