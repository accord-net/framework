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
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Math;

    /// <summary>
    ///   Binomial test.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   In statistics, the binomial test is an exact test of the statistical significance
    ///   of deviations from a theoretically expected distribution of observations into two
    ///   categories. The most common use of the binomial test is in the case where the null
    ///   hypothesis is that two categories are equally likely to occur (such as a coin toss).</para>
    /// <para>
    ///   When there are more than two categories, and an exact test is required, the
    ///   <see cref="MultinomialTest"> multinomial test</see>, based on the multinomial
    ///   distribution, must be used instead of the binomial test.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Binomial_test">
    ///        Wikipedia, The Free Encyclopedia. Binomial-Test. Available from:
    ///        http://en.wikipedia.org/wiki/Binomial_test </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    ///   <para>
    ///     This is the second example from Wikipedia's page on hypothesis testing. In this example, 
    ///     a person is tested for clairvoyance (ability of gaining information about something through
    ///     extra sensory perception; detecting something without using the known human senses.</para>
    ///     
    /// <code>
    /// // A person is shown the reverse of a playing card 25 times and is
    /// // asked which of the four suits the card belongs to. Every time
    /// // the person correctly guesses the suit of the card, we count this
    /// // result as a correct answer. Let's suppose the person obtained 13
    /// // correctly answers out of the 25 cards.
    /// 
    /// // Since each suit appears 1/4 of the time in the card deck, we 
    /// // would assume the probability of producing a correct answer by
    /// // chance alone would be of 1/4.
    /// 
    /// // And finally, we must consider we are interested in which the
    /// // subject performs better than what would expected by chance. 
    /// // In other words, that the person's probability of predicting
    /// // a card is higher than the chance hypothesized value of 1/4.
    /// 
    /// BinomialTest test = new BinomialTest(
    ///     successes: 13, trials: 25,
    ///     hypothesizedProbability: 1.0 / 4.0,
    ///     alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);
    /// 
    /// Console.WriteLine("Test p-Value: " + test.PValue);     // ~ 0.003
    /// Console.WriteLine("Significant? " + test.Significant); // True.
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class BinomialTest : HypothesisTest<BinomialDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public OneSampleHypothesis Hypothesis { get; protected set; }

        /// <summary>
        ///   Tests the probability of two outcomes in a series of experiments.
        /// </summary>
        /// 
        /// <param name="trials">The experimental trials.</param>
        /// <param name="hypothesizedProbability">The hypothesized occurrence probability.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public BinomialTest(bool[] trials, double hypothesizedProbability = 0.5,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {

            if (trials == null) throw new ArgumentNullException("trials");

            if (hypothesizedProbability < 0 || hypothesizedProbability > 1.0)
                throw new ArgumentOutOfRangeException("hypothesizedProbability");

            int successes = 0;

            for (int i = 0; i < trials.Length; i++)
                if (trials[i]) successes++;

            Compute(successes, trials.Length, hypothesizedProbability, alternate);
        }

        /// <summary>
        ///   Tests the probability of two outcomes in a series of experiments.
        /// </summary>
        /// 
        /// <param name="successes">The number of successes in the trials.</param>
        /// <param name="trials">The total number of experimental trials.</param>
        /// <param name="hypothesizedProbability">The hypothesized occurrence probability.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public BinomialTest(int successes, int trials, double hypothesizedProbability = 0.5,
            OneSampleHypothesis alternate = OneSampleHypothesis.ValueIsDifferentFromHypothesis)
        {
            if (successes > trials)
                throw new ArgumentOutOfRangeException("successes");

            if (hypothesizedProbability < 0 || hypothesizedProbability > 1.0)
                throw new ArgumentOutOfRangeException("hypothesizedProbability");

            Compute(successes, trials, hypothesizedProbability, alternate);
        }


        /// <summary>
        ///   Creates a Binomial test.
        /// </summary>
        /// 
        protected BinomialTest()
        {
        }


        /// <summary>
        ///   Computes the Binomial test.
        /// </summary>
        /// 
        protected void Compute(double statistic, int m, double p, OneSampleHypothesis alternate)
        {
            this.Statistic = statistic;
            this.StatisticDistribution = new BinomialDistribution(m, p);
            this.Hypothesis = alternate;
            this.Tail = (DistributionTail)alternate;
            this.PValue = StatisticToPValue(Statistic);

            this.OnSizeChanged();
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
            double p;
            switch (Tail)
            {
                case DistributionTail.TwoTail:
                    p = wilsonSterne(x);
                    break;

                case DistributionTail.OneUpper:
                    p = StatisticDistribution.ComplementaryDistributionFunction((int)x, inclusive: true);
                    break;

                case DistributionTail.OneLower:
                    p = StatisticDistribution.DistributionFunction((int)x);
                    break;

                default:
                    throw new InvalidOperationException();
            }
            return p;
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
            double b;
            switch (Tail)
            {
                case DistributionTail.OneLower:
                    b = StatisticDistribution.InverseDistributionFunction(p);
                    break;
                case DistributionTail.OneUpper:
                    b = StatisticDistribution.InverseDistributionFunction(1.0 - p);
                    break;
                case DistributionTail.TwoTail:
                    throw new NotSupportedException();

                default: throw new InvalidOperationException();
            }

            return b;
        }


        /// <summary>
        ///   Computes the two-tail probability using the Wilson-Sterne rule,
        ///   which defines the tail of the distribution based on a ordering
        ///   of the null probabilities of X. (Smirnoff, 2003)
        /// </summary>
        /// 
        /// <remarks>
        ///   References: Jeffrey S. Simonoff, Analyzing 
        ///   Categorical Data, Springer, 2003 (pg 64).
        /// </remarks>
        /// 
        private double wilsonSterne(double x)
        {
            double mean = StatisticDistribution.Mean;

            if (x == mean)
                return 1;


            int trials = StatisticDistribution.NumberOfTrials;

            // Construct a map of values and point probabilities
            double[] probabilities = new double[trials];


            for (int i = 0; i < probabilities.Length; i++)
                probabilities[i] = StatisticDistribution.ProbabilityMassFunction(i);


            int[] values;

            // Build the ordered Wilson-Sterne table
            probabilities.StableSort(out values);


            // Now, compute the cumulative probability
            double[] cumulative = new double[trials];
            cumulative[0] = probabilities[0];
            for (int i = 1; i < cumulative.Length; i++)
                cumulative[i] += cumulative[i - 1] + probabilities[i];

            int v = 0;
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] == (int)x)
                {
                    v = i;
                    while (v < probabilities.Length && probabilities[i] == probabilities[v]) v++;
                }
            }


            return cumulative[v - 1];
        }

    }
}
