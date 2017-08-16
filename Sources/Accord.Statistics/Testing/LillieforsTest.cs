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
    using System.Threading.Tasks;

    /// <summary>
    ///   One sample Lilliefors' corrected Kolmogorov-Smirnov (KS) test.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///  In statistics, the Lilliefors test, named after Hubert Lilliefors, professor of statistics at George
    ///  Washington University, is a test based on the Kolmogorov–Smirnov test. It is used to test the null 
    ///  hypothesis that data come from a normally distributed population, when the null hypothesis does not 
    ///  specify which normal distribution; i.e., it does not specify the expected value and variance of the
    ///  distribution.</para>
    ///  
    /// <para>
    ///  Contrary to the <see cref="KolmogorovSmirnovTest">Kolmogorov-Smirnov</see> test, this test can be used
    ///  to assess the likelihood that a given sample could have been generated from a distribution that has been
    ///  fitted from the data.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="https://en.wikipedia.org/wiki/Lilliefors_test">
    ///       Wikipedia, The Free Encyclopedia. Lilliefors Test. 
    ///       Available on: https://en.wikipedia.org/wiki/Lilliefors_test </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   In this first example, suppose we got a new sample, and we would 
    ///   like to test whether this sample has been originated from a uniform
    ///   continuous distribution. Unlike <see cref="KolmogorovSmirnovTest"/>,
    ///   we can actually use this test whether the data fits a distribution
    ///   that has been estimated from the data.</para>
    /// 
    /// <code source="Unit Tests\Accord.Tests.Statistics\Testing\LillieforsTestTest.cs" region="doc_uniform"/>
    /// 
    /// <para>
    ///  We can also check whether a Normal distribution fitted on the data 
    ///  is a good candidate model for the samples:</para>
    ///  
    /// <code source="Unit Tests\Accord.Tests.Statistics\Testing\LillieforsTestTest.cs" region="doc_normal"/>
    /// </example>
    /// 
    /// <seealso cref="Accord.Statistics.Distributions.Univariate.KolmogorovSmirnovDistribution"/>
    /// 
    [Serializable]
    public class LillieforsTest : HypothesisTest<EmpiricalDistribution>,
        IHypothesisTest<EmpiricalDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis
        ///   can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public KolmogorovSmirnovTestHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Gets the hypothesized distribution for the samples.
        /// </summary>
        /// 
        public ISampleableDistribution<double> TheoreticalDistribution { get; private set; }

        /// <summary>
        ///   Gets the empirical distribution measured from the sample.
        /// </summary>
        /// 
        public EmpiricalDistribution EmpiricalDistribution { get; private set; }

        ///// <summary>
        /////   Gets or sets the parallelization options used when simulating 
        /////   the KS statistic distribution using the Monte-Carlo method.
        ///// </summary>
        ///// 
        //public ParallelOptions ParallelOptions { get; set; }

        /// <summary>
        ///   Gets the number of observations in the sample being tested.
        /// </summary>
        /// 
        public int NumberOfSamples { get; private set; }

        private LillieforsTest()
        {
            // this.ParallelOptions = new ParallelOptions();
        }

        /// <summary>
        ///   Creates a new One-Sample Lilliefors' Kolmogorov-Smirnov test.
        /// </summary>
        /// 
        /// <param name="sample">The sample we would like to test as belonging to the <paramref name="hypothesizedDistribution"/>.</param>
        /// <param name="hypothesizedDistribution">A fully specified distribution (which could have been estimated from the data).</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// <param name="iterations">The number of Monte-Carlo iterations to perform. Default is 10,000.</param>
        /// <param name="reestimate">Whether the target distribution should be re-estimated from the sampled data
        ///   at each Monte-Carlo iteration. Pass true in case <paramref name="hypothesizedDistribution"/> has been
        ///   estimated from the data.</param>
        /// 
        public LillieforsTest(double[] sample, ISampleableDistribution<double> hypothesizedDistribution,
            KolmogorovSmirnovTestHypothesis alternate = KolmogorovSmirnovTestHypothesis.SampleIsDifferent,
            int iterations = 10000, bool reestimate = true)
            : this()
        {
            if (reestimate)
            {
                if (!(hypothesizedDistribution is IFittableDistribution<double>))
                    throw new InvalidOperationException("The estimate option can only be used with distributions that implement IFittableDistribution<double>.");
            }

            this.Hypothesis = alternate;

            // Create a copy of the samples to prevent altering the
            // constructor's original arguments in the sorting step 
            double[] orderedSamples = sample.Sorted();

            // Create the theoretical and empirical distributions
            this.TheoreticalDistribution = hypothesizedDistribution;
            this.EmpiricalDistribution = new EmpiricalDistribution(orderedSamples, smoothing: 0);
            this.NumberOfSamples = sample.Length;

            StatisticDistribution = GetSimulatedDistribution(hypothesizedDistribution,
                reestimate, iterations, alternate);

            // Finally, compute the test statistic and perform actual testing.
            base.Statistic = KolmogorovSmirnovTest.GetStatistic(orderedSamples, TheoreticalDistribution, alternate);
            this.Tail = (DistributionTail)alternate;
            base.PValue = StatisticToPValue(Statistic);
        }

        private EmpiricalDistribution GetSimulatedDistribution(ISampleableDistribution<double> hypothesizedDistribution,
            bool reestimate, int iterations, KolmogorovSmirnovTestHypothesis alternate)
        {
            double[] samples = new double[iterations];

            if (reestimate)
            {
                Parallel.For(0, iterations, i => // TODO: Use ParallelOptions
                {
                    double[] s = hypothesizedDistribution.Generate(samples: NumberOfSamples);
                    Vector.Sort<double>(s);

                    var fittable = (IFittableDistribution<double>)hypothesizedDistribution.Clone();
                    fittable.Fit(s);

                    samples[i] = KolmogorovSmirnovTest.GetStatistic((double[])s, (IDistribution<double>)fittable, alternate);
                });
            }
            else
            {
                Parallel.For(0, iterations, i => // TODO: Use ParallelOptions
                {
                    double[] s = hypothesizedDistribution.Generate(samples: NumberOfSamples);
                    Vector.Sort<double>(s);
                    samples[i] = KolmogorovSmirnovTest.GetStatistic((double[])s, (IDistribution<double>)hypothesizedDistribution, alternate);
                });
            }
            

            return new EmpiricalDistribution(samples, smoothing: 0);
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
            return StatisticDistribution.ComplementaryDistributionFunction(x);
        }

        /// <summary>
        ///   Performs a Goodness-of-Fit method by automatically creating and fitting
        ///   the chosen distribution to the samples and computing a <see cref="LillieforsTest"/>
        ///   against this fitted distribution.
        /// </summary>
        /// 
        /// <typeparam name="TDistribution">The type of the distribution.</typeparam>
        /// 
        /// <param name="samples">The samples used to fit the distribution.</param>
        /// 
        /// <returns>A Lilliefor Test assessing whether it is likely that the samples
        ///   could have been generated by the chosen distribution.</returns>
        /// 
        public static LillieforsTest GoodnessOfFit<TDistribution>(double[] samples)
            where TDistribution : IFittableDistribution<double[]>,
                                  ISampleableDistribution<double>,
                                  new()
        {
            var dist = new TDistribution();
            dist.Fit(samples);
            return new LillieforsTest(samples, dist);
        }
    }
}