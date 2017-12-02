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
    using System;
    using Distributions.Univariate;
    using Accord.Math;
    using Accord.Statistics;
    using Accord.Compat;

    /// <summary>
    ///   Grubb's Test for Outliers (for approximately Normal distributions).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   Grubbs' test (named after Frank E. Grubbs, who published the test in 1950),
    ///   also known as the maximum normed residual test or extreme studentized deviate
    ///   test, is a statistical test used to detect outliers in a univariate data set
    ///   assumed to come from a normally distributed population.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Grubbs%27_test_for_outliers">
    ///        Wikipedia, The Free Encyclopedia. Grubb's test for outliers. Available on:
    ///        https://en.wikipedia.org/wiki/Grubbs%27_test_for_outliers </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.NormalDistribution"/>
    /// 
    [Serializable]
    public class GrubbTest : HypothesisTest<GrubbDistribution>
    {
        private double min;
        private double max;
        private double abs;

        /// <summary>
        /// Gets the number of observations in the sample.
        /// </summary>
        /// 
        /// <value>The number of observations in the sample.</value>
        /// 
        public int NumberOfSamples { get; private set; }

        /// <summary>
        /// Gets the sample's mean.
        /// </summary>
        /// 
        public double Mean { get; private set; }

        /// <summary>
        /// Gets the sample's standard deviation.
        /// </summary>
        /// 
        public double StandardDeviation { get; private set; }

        /// <summary>
        /// Gets the difference between the minimum value and the mean.
        /// </summary>
        /// 
        public double Min { get { return min; } }

        /// <summary>
        /// Gets the difference between the maximum value and the mean.
        /// </summary>
        /// 
        public double Max { get { return max; } }

        /// <summary>
        /// Gets the maximum absolute difference between 
        /// an observation in the sample and the mean.
        /// </summary>
        /// 
        public double Abs { get { return abs; } }

        /// <summary>
        /// Gets the <see cref="GrubbTestHypothesis"/> being tested.
        /// </summary>
        /// 
        public GrubbTestHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Constructs a Grubb's test.
        /// </summary>
        /// 
        public GrubbTest(double[] samples, GrubbTestHypothesis hypothesis = GrubbTestHypothesis.ThereAreOutliers)
        {
            this.NumberOfSamples = samples.Length;
            this.Mean = samples.Mean();
            this.StandardDeviation = samples.StandardDeviation(Mean);

            // Compute Grubb's statistic
            this.min = this.max = samples[0] - Mean;
            for (int i = 1; i < samples.Length; i++)
            {
                double z = samples[i] - Mean;
                if (z > max)
                    max = z;
                if (z < min)
                    min = z;
            }
            this.abs = Math.Max(-min, max);

            this.Statistic = g(hypothesis) / StandardDeviation;
            this.StatisticDistribution = new GrubbDistribution(NumberOfSamples);
            this.Tail = DistributionTail.OneUpper;
            this.Hypothesis = hypothesis;

            // Compute the transformed p-value
            this.PValue = StatisticToPValue(Statistic);
        }

        private double g(GrubbTestHypothesis hypothesis)
        {
            switch (hypothesis)
            {
                case GrubbTestHypothesis.ThereAreOutliers:
                    return abs;
                case GrubbTestHypothesis.TheMaximumIsAnOutlier:
                    return max;
                case GrubbTestHypothesis.TheMinimumIsAnOutlier:
                    return -min;
            }

            throw new ArgumentOutOfRangeException("hypothesis");
        }

        /// <summary>
        /// Converts a given test statistic to a p-value.
        /// </summary>
        /// <param name="x">The value of the test statistic.</param>
        /// <returns>The p-value for the given statistic.</returns>
        public override double StatisticToPValue(double x)
        {
            double p;
            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    p = 2.0 * StatisticDistribution.ComplementaryDistributionFunction(Math.Abs(x));
                    break;

                case DistributionTail.OneUpper:
                    p = StatisticDistribution.ComplementaryDistributionFunction(x);
                    break;

                case DistributionTail.OneLower:
                    p = StatisticDistribution.DistributionFunction(x);
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return p;
        }

        /// <summary>
        /// Converts a given p-value to a test statistic.
        /// </summary>
        /// <param name="p">The p-value.</param>
        /// <returns>The test statistic which would generate the given p-value.</returns>
        public override double PValueToStatistic(double p)
        {
            double t;
            switch (Tail)
            {
                case DistributionTail.OneLower:
                    t = StatisticDistribution.InverseDistributionFunction(p);
                    break;
                case DistributionTail.OneUpper:
                    t = StatisticDistribution.InverseDistributionFunction(1.0 - p);
                    break;
                case DistributionTail.TwoTail:
                    t = StatisticDistribution.InverseDistributionFunction(1.0 - p / 2.0);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return t;
        }

    }
}
