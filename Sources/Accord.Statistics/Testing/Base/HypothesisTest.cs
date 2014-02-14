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

namespace Accord.Statistics.Testing
{
    using System;
    using Accord.Statistics.Distributions;

    /// <summary>
    ///   Base class for Hypothesis Tests.
    /// </summary>
    /// 
    /// <remarks>
    ///   A statistical hypothesis test is a method of making decisions using data, whether from
    ///   a controlled experiment or an observational study (not controlled). In statistics, a 
    ///   result is called statistically significant if it is unlikely to have occurred by chance
    ///   alone, according to a pre-determined threshold probability, the significance level.
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Statistical_hypothesis_testing">
    ///       Wikipedia, The Free Encyclopedia. Statistical Hypothesis Testing. </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    [Serializable]
    public abstract class HypothesisTest<TDistribution> : IFormattable, IHypothesisTest<TDistribution>
        where TDistribution : IUnivariateDistribution
    {

        private double alpha = 0.05;


        /// <summary>
        ///   Initializes a new instance of the class.
        /// </summary>
        /// 
        protected HypothesisTest() { }

        /// <summary>
        ///   Gets the distribution associated
        ///   with the test statistic.
        /// </summary>
        /// 
        public TDistribution StatisticDistribution { get; protected set; }

        /// <summary>
        ///   Gets the P-value associated with this test.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   In statistical hypothesis testing, the p-value is the probability of
        ///   obtaining a test statistic at least as extreme as the one that was
        ///   actually observed, assuming that the null hypothesis is true.</para>
        /// <para>
        ///   The lower the p-value, the less likely the result can be explained
        ///   by chance alone, assuming the null hypothesis is true.</para>  
        /// </remarks>
        /// 
        public double PValue { get; protected set; }

        /// <summary>
        ///   Gets the test statistic.
        /// </summary>
        /// 
        public double Statistic { get; protected set; }

        /// <summary>
        ///   Gets the test type.
        /// </summary>
        /// 
        public DistributionTail Tail { get; protected set; }

        /// <summary>
        ///   Gets the significance level for the
        ///   test. Default value is 0.05 (5%).
        /// </summary>
        /// 
        public double Size
        {
            get { return alpha; }
            set
            {
                alpha = value;
                OnSizeChanged();
            }
        }

        /// <summary>
        ///   Gets whether the null hypothesis should be rejected.
        /// </summary>
        /// 
        /// <remarks>
        ///   A test result is said to be statistically significant when the
        ///   result would be very unlikely to have occurred by chance alone.
        /// </remarks>
        /// 
        public bool Significant
        {
            get { return PValue < alpha; }
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="x">The value of the test statistic.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public abstract double StatisticToPValue(double x);

        /// <summary>
        ///   Converts a given p-value to a test statistic.
        /// </summary>
        /// 
        /// <param name="p">The p-value.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public abstract double PValueToStatistic(double p);

        /// <summary>
        ///   Called whenever the test <see cref="Size">significance level</see> changes.
        /// </summary>
        /// 
        protected virtual void OnSizeChanged() { }


        /// <summary>
        ///   Converts the numeric P-Value of this test to its equivalent string representation.
        /// </summary>
        /// 
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return PValue.ToString(format, formatProvider);
        }

        /// <summary>
        ///   Converts the numeric P-Value of this test to its equivalent string representation.
        /// </summary>
        /// 
        public override string ToString()
        {
            return PValue.ToString(System.Globalization.CultureInfo.CurrentCulture);
        }


    }
}
