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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Distributions;
    using Accord.Compat;

    /// <summary>
    ///   One-sample Kolmogorov-Smirnov (KS) test.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Kolmogorov-Smirnov test tries to determine if a sample differs significantly
    ///   from an hypothesized theoretical probability distribution. The Kolmogorov-Smirnov
    ///   test has an interesting advantage in which it does not requires any assumptions
    ///   about the data. The distribution of the K-S test statistic does not depend on
    ///   which distribution is being tested.</para>
    /// <para>
    ///   The K-S test has also the advantage of being an exact test (other tests, such as the
    ///   chi-square goodness-of-fit test depends on an adequate sample size). One disadvantage
    ///   is that it requires a fully defined distribution which should not have been estimated
    ///   from the data. If the parameters of the theoretical distribution have been estimated
    ///   from the data, the critical region of the K-S test will be no longer valid.</para>
    /// <para>
    ///   This class uses an efficient and high-accuracy algorithm based on work by Richard
    ///   Simard (2010). Please see <see cref="KolmogorovSmirnovDistribution"/> for more details.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Kolmogorov%E2%80%93Smirnov_test">
    ///       Wikipedia, The Free Encyclopedia. Kolmogorov-Smirnov Test. 
    ///       Available on: http://en.wikipedia.org/wiki/Kolmogorov%E2%80%93Smirnov_test </a></description></item>
    ///     <item><description><a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda35g.htm">
    ///       NIST/SEMATECH e-Handbook of Statistical Methods. Kolmogorov-Smirnov Goodness-of-Fit Test.
    ///       Available on: http://www.itl.nist.gov/div898/handbook/eda/section3/eda35g.htm </a></description></item>
    ///     <item><description><a href="http://www.iro.umontreal.ca/~lecuyer/myftp/papers/ksdist.pdf">
    ///       Richard Simard, Pierre L’Ecuyer. Computing the Two-Sided Kolmogorov-Smirnov Distribution.
    ///       Journal of Statistical Software. Volume VV, Issue II. Available on:
    ///       http://www.iro.umontreal.ca/~lecuyer/myftp/papers/ksdist.pdf </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   In this first example, suppose we got a new sample, and we would 
    ///   like to test whether this sample has been originated from a uniform
    ///   continuous distribution.</para>
    /// 
    /// <code>
    /// double[] sample = 
    /// { 
    ///     0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382
    /// };
    /// 
    /// // First, we create the distribution we would like to test against:
    /// //
    /// var distribution = UniformContinuousDistribution.Standard;
    /// 
    /// // Now we can define our hypothesis. The null hypothesis is that the sample
    /// // comes from a standard uniform distribution, while the alternate is that
    /// // the sample is not from a standard uniform distribution.
    /// //
    /// var kstest = new KolmogorovSmirnovTest(sample, distribution);
    /// 
    /// double statistic = kstest.Statistic; // 0.29
    /// double pvalue = kstest.PValue;       // 0.3067
    /// 
    /// bool significant = kstest.Significant; // false
    /// </code>
    /// <para>
    ///   Since the null hypothesis could not be rejected, then the sample
    ///   can perhaps be from a uniform distribution. However, please note
    ///   that this doesn't means that the sample *is* from the uniform, it
    ///   only means that we could not rule out the possibility.</para>
    ///   
    /// <para>
    ///  Before we could not rule out the possibility that the sample came from
    ///  a uniform distribution, which means the sample was not very far from
    ///  uniform. This would be an indicative that it would be far from what
    ///  would be expected from a Normal distribution:</para>
    ///  
    /// <code>
    /// // First, we create the distribution we would like to test against:
    /// //
    /// NormalDistribution distribution = NormalDistribution.Standard;
    /// 
    /// // Now we can define our hypothesis. The null hypothesis is that the sample
    /// // comes from a standard Normal distribution, while the alternate is that
    /// // the sample is not from a standard Normal distribution.
    /// //
    /// var kstest = new KolmogorovSmirnovTest(sample, distribution);
    /// 
    /// double statistic = kstest.Statistic; // 0.580432
    /// double pvalue = kstest.PValue;       // 0.000999
    /// 
    /// bool significant = kstest.Significant; // true
    /// </code>
    /// 
    /// <para>
    ///   Since the test says that the null hypothesis should be rejected, then
    ///   this can be regarded as a strong indicative that the sample does not
    ///   comes from a Normal distribution, just as we expected.</para>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.KolmogorovSmirnovDistribution"/>
    /// 
    [Serializable]
    public class KolmogorovSmirnovTest : HypothesisTest<KolmogorovSmirnovDistribution>,
        IHypothesisTest<KolmogorovSmirnovDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public KolmogorovSmirnovTestHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Gets the theoretical, hypothesized distribution for the samples,
        ///   which should have been stated <i>before</i> any measurements.
        /// </summary>
        /// 
        public IDistribution<double> TheoreticalDistribution { get; private set; }

        /// <summary>
        ///   Gets the empirical distribution measured from the sample.
        /// </summary>
        /// 
        public EmpiricalDistribution EmpiricalDistribution { get; private set; }

        /// <summary>
        ///   Creates a new One-Sample Kolmogorov test.
        /// </summary>
        /// 
        /// <param name="sample">The sample we would like to test as belonging to the <paramref name="hypothesizedDistribution"/>.</param>
        /// <param name="hypothesizedDistribution">A fully specified distribution (which must NOT have been estimated from the data).</param>
        /// 
        public KolmogorovSmirnovTest(double[] sample, IDistribution<double> hypothesizedDistribution)
            : this(sample, hypothesizedDistribution, KolmogorovSmirnovTestHypothesis.SampleIsDifferent)
        {
        }

        /// <summary>
        ///   Creates a new One-Sample Kolmogorov test.
        /// </summary>
        /// 
        /// <param name="sample">The sample we would like to test as belonging to the <paramref name="hypothesizedDistribution"/>.</param>
        /// <param name="hypothesizedDistribution">A fully specified distribution (which must NOT have been estimated from the data).</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public KolmogorovSmirnovTest(double[] sample, IDistribution<double> hypothesizedDistribution,
            KolmogorovSmirnovTestHypothesis alternate = KolmogorovSmirnovTestHypothesis.SampleIsDifferent)
        {
            this.Hypothesis = alternate;

            // Create the test statistic distribution with given degrees of freedom
            StatisticDistribution = new KolmogorovSmirnovDistribution(sample.Length);

            // Create a copy of the samples to prevent altering the
            // constructor's original arguments in the sorting step 
            double[] sortedSamples = sample.Sorted();

            // Create the theoretical and empirical distributions
            this.TheoreticalDistribution = hypothesizedDistribution;
            this.EmpiricalDistribution = new EmpiricalDistribution(sortedSamples, smoothing: 0);

            // Finally, compute the test statistic and perform actual testing.
            base.Statistic = GetStatistic(sortedSamples, TheoreticalDistribution, alternate);
            this.Tail = (DistributionTail)alternate;
            base.PValue = StatisticToPValue(Statistic);
        }

        /// <summary>
        ///   Gets the appropriate Kolmogorov-Sminorv D statistic for the samples and target distribution.
        /// </summary>
        /// 
        /// <param name="sortedSamples">The sorted samples.</param>
        /// <param name="distribution">The target distribution.</param>
        /// <param name="alternate">The alternate hypothesis for the KS test. For <see cref="KolmogorovSmirnovTestHypothesis.SampleIsDifferent"/>, this
        ///   is the two-sided Dn statistic; for <see cref="KolmogorovSmirnovTestHypothesis.SampleIsGreater"/> this is the one sided Dn+ statistic;
        ///   and for <see cref="KolmogorovSmirnovTestHypothesis.SampleIsSmaller"/> this is the one sided Dn- statistic.</param>
        /// 
        public static double GetStatistic(double[] sortedSamples, IDistribution<double> distribution, KolmogorovSmirnovTestHypothesis alternate)
        {
            // Finally, compute the test statistic and perform actual testing.
            switch (alternate)
            {
                case KolmogorovSmirnovTestHypothesis.SampleIsDifferent:
                    return TwoSide(sortedSamples, distribution);
                case KolmogorovSmirnovTestHypothesis.SampleIsGreater:
                    return OneSideUpper(sortedSamples, distribution);
                case KolmogorovSmirnovTestHypothesis.SampleIsSmaller:
                    return OneSideLower(sortedSamples, distribution);
            }

            throw new ArgumentOutOfRangeException("alternate");
        }

        /// <summary>
        ///   Gets the one-sided "Dn-" Kolmogorov-Sminorv statistic for the samples and target distribution.
        /// </summary>
        /// 
        /// <param name="sortedSamples">The sorted samples.</param>
        /// <param name="distribution">The target distribution.</param>
        /// 
        public static double OneSideLower(double[] sortedSamples, IDistribution<double> distribution)
        {
            double N = sortedSamples.Length;
            double[] Y = sortedSamples;
            Func<double, double> F = distribution.DistributionFunction;

            // Test if the sample's distribution is "smaller" than the
            // given theoretical distribution, in a statistical sense.

            double max = Math.Max(F(Y[0]), F(Y[0]) - 1 / N);
            for (int i = 1; i < Y.Length; i++)
            {
                double a = F(Y[i]) - i / N;
                double b = F(Y[i]) - (i + 1) / N;
                if (a > max) max = a;
                if (b > max) max = b;
            }

            return max; // This is the one-sided "Dn-" statistic.
        }

        /// <summary>
        ///   Gets the one-sided "Dn+" Kolmogorov-Sminorv statistic for the samples and target distribution.
        /// </summary>
        /// 
        /// <param name="sortedSamples">The sorted samples.</param>
        /// <param name="distribution">The target distribution.</param>
        /// 
        public static double OneSideUpper(double[] sortedSamples, IDistribution<double> distribution)
        {
            double N = sortedSamples.Length;
            double[] Y = sortedSamples;
            Func<double, double> F = distribution.DistributionFunction;

            // Test if the sample's distribution is "larger" than the
            // given theoretical distribution, in a statistical sense.

            double max = Math.Max(-F(Y[0]), 1 / N - F(Y[0]));
            for (int i = 1; i < Y.Length; i++)
            {
                double a = i / N - F(Y[i]);
                double b = (i + 1) / N - F(Y[i]);
                if (a > max) max = a;
                if (b > max) max = b;
            }

            return max; // This is the one-sided "Dn+" statistic.
        }

        /// <summary>
        ///   Gets the two-sided "Dn" Kolmogorov-Sminorv statistic for the samples and target distribution.
        /// </summary>
        /// 
        /// <param name="sortedSamples">The sorted samples.</param>
        /// <param name="distribution">The target distribution.</param>
        /// 
        public static double TwoSide(double[] sortedSamples, IDistribution<double> distribution)
        {
            double N = sortedSamples.Length;
            double[] Y = sortedSamples;
            Func<double, double> F = distribution.DistributionFunction;

            // Test if the sample's distribution is just significantly
            //   "different" than the given theoretical distribution.

            // This is a correction on the common formulation found in many places
            //  such as in Wikipedia. Please see the Engineering Statistics Handbook,
            //  section "1.3.5.16. Kolmogorov-Smirnov Goodness-of-Fit Test" for more
            //  details: http://www.itl.nist.gov/div898/handbook/eda/section3/eda35g.htm

            double max = Math.Max(Math.Abs(F(Y[0])), Math.Abs(1 / N - F(Y[0])));
            for (int i = 1; i < Y.Length; i++)
            {
                double a = Math.Abs(F(Y[i]) - i / N);
                double b = Math.Abs((i + 1) / N - F(Y[i]));
                if (a > max) max = a;
                if (b > max) max = b;
            }

            return max; // This is the two-sided "Dn" statistic.
        }

        /// <summary>
        ///   Converts a given p-value to a test statistic.
        /// </summary>
        /// 
        /// <param name="p">The p-value.</param>
        /// 
        /// <returns>The test statistic which would generate the given p-value.</returns>
        /// 
        public override double PValueToStatistic(double p)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Converts a given test statistic to a p-value.
        /// </summary>
        /// 
        /// <param name="x">The value of the test statistic.</param>
        /// 
        /// <returns>The p-value for the given statistic.</returns>
        /// 
        public override double StatisticToPValue(double x)
        {
            if (Tail == Testing.DistributionTail.TwoTail)
                return StatisticDistribution.ComplementaryDistributionFunction(Statistic);
            return StatisticDistribution.OneSideDistributionFunction(Statistic);
        }
    }
}