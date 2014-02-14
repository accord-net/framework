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
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;

    /// <summary>
    ///   Two-sample Kolmogorov-Smirnov (KS) test.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The Kolmogorov-Smirnov test tries to determine if two samples have been 
    ///   drawn from the same probability distribution. The Kolmogorov-Smirnov test
    ///   has an interesting advantage in which it does not requires any assumptions
    ///   about the data. The distribution of the K-S test statistic does not depend
    ///   on which distribution is being tested.</para>
    /// <para>
    ///   The K-S test has also the advantage of being an exact test (other tests, such as the
    ///   chi-square goodness-of-fit test depends on an adequate sample size). One disadvantage
    ///   is that it requires a fully defined distribution which should not have been estimated
    ///   from the data. If the parameters of the theoretical distribution have been estimated
    ///   from the data, the critical region of the K-S test will be no longer valid.</para>
    /// <para>
    ///   The two-sample KS test is one of the most useful and general nonparametric methods for
    ///   comparing two samples, as it is sensitive to differences in both location and shape of
    ///   the empirical cumulative distribution functions of the two samples.</para>
    /// <para>
    ///   This class uses an efficient and high-accuracy algorithm based on work by Richard
    ///   Simard (2010). Please see <see cref="KolmogorovSmirnovDistribution"/> for more details.</para>
    ///   
    /// <para>
    ///   References:
    ///   <list type="bullet">
    ///     <item><description><a href="http://en.wikipedia.org/wiki/Kolmogorov%E2%80%93Smirnov_test">
    ///       Wikipedia, The Free Encyclopedia. Kolmogorov-Smirnov Test. 
    ///       Available at: http://en.wikipedia.org/wiki/Kolmogorov%E2%80%93Smirnov_test </a></description></item>
    ///     <item><description><a href="http://www.itl.nist.gov/div898/handbook/eda/section3/eda35g.htm">
    ///       NIST/SEMATECH e-Handbook of Statistical Methods. Kolmogorov-Smirnov Goodness-of-Fit Test.
    ///       Available at: http://www.itl.nist.gov/div898/handbook/eda/section3/eda35g.htm </a></description></item>
    ///     <item><description><a href="http://www.iro.umontreal.ca/~lecuyer/myftp/papers/ksdist.pdf">
    ///       Richard Simard, Pierre L’Ecuyer. Computing the Two-Sided Kolmogorov-Smirnov Distribution.
    ///       Journal of Statistical Software. Volume VV, Issue II. Available at:
    ///       http://www.iro.umontreal.ca/~lecuyer/myftp/papers/ksdist.pdf </a></description></item>
    ///     <item><description><a href="http://www.physics.csbsju.edu/stats/">
    ///       Kirkman, T.W. (1996) Statistics to Use. Available at:
    ///       http://www.physics.csbsju.edu/stats/ </a></description></item>
    ///   </list></para>
    /// </remarks>
    /// 
    /// <example>
    /// <para>
    ///   In the following example, we will be creating a K-S test to verify
    ///   if two samples have been drawn from different populations. In this
    ///   example, we will first generate a number of samples from two different
    ///   distributions and then check if the K-S can indeed see the difference
    ///   between them:</para>
    /// <code>
    /// // Generate 15 points from a Normal distribution with mean 5 and sigma 2
    /// double[] sample1 = new NormalDistribution(mean: 5, stdDev: 1).Generate(25);
    /// 
    /// // Generate 15 points from an uniform distribution from 0 to 10
    /// double[] sample2 = new UniformContinuousDistribution(a: 0, b: 10).Generate(25);
    /// 
    /// // Now we can create a K-S test and test the unequal hypothesis:
    /// var test = new TwoSampleKolmogorovSmirnovTest(sample1, sample2,
    ///     TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal);
    /// 
    /// bool significant = test.Significant; // outputs true
    /// </code>
    /// 
    /// <para>
    ///   The following example comes from the stats page of the College of Saint Benedict and Saint John's
    ///   University (Kirkman, 1996). It is a very interesting example as it shows a case in which a t-test
    ///   fails to see a difference between the samples because of the non-normality of the sample's
    ///   distributions. The Kolmogorov-Smirnov nonparametric test, on the other hand, succeeds.</para>
    /// <para>
    ///   The example deals with the preference of bees between two nearby blooming trees in an empty field.
    ///   The experimenter has collected data measuring how much time does a bee spent near a particular
    ///   tree. The time starts to be measured when a bee first touches the tree, and is stopped when the bee
    ///   moves more than 1 meter far from it. The samples below represents the measured time, in seconds, of
    ///   the observed bees for each of the trees.</para>
    ///   
    /// <code>
    /// double[] redwell = 
    /// {
    ///     23.4, 30.9, 18.8, 23.0, 21.4, 1, 24.6, 23.8, 24.1, 18.7, 16.3, 20.3,
    ///     14.9, 35.4, 21.6, 21.2, 21.0, 15.0, 15.6, 24.0, 34.6, 40.9, 30.7, 
    ///     24.5, 16.6, 1, 21.7, 1, 23.6, 1, 25.7, 19.3, 46.9, 23.3, 21.8, 33.3, 
    ///     24.9, 24.4, 1, 19.8, 17.2, 21.5, 25.5, 23.3, 18.6, 22.0, 29.8, 33.3,
    ///     1, 21.3, 18.6, 26.8, 19.4, 21.1, 21.2, 20.5, 19.8, 26.3, 39.3, 21.4, 
    ///     22.6, 1, 35.3, 7.0, 19.3, 21.3, 10.1, 20.2, 1, 36.2, 16.7, 21.1, 39.1,
    ///     19.9, 32.1, 23.1, 21.8, 30.4, 19.62, 15.5 
    /// };
    /// 
    /// double[] whitney = 
    /// {
    ///     16.5, 1, 22.6, 25.3, 23.7, 1, 23.3, 23.9, 16.2, 23.0, 21.6, 10.8, 12.2,
    ///     23.6, 10.1, 24.4, 16.4, 11.7, 17.7, 34.3, 24.3, 18.7, 27.5, 25.8, 22.5,
    ///     14.2, 21.7, 1, 31.2, 13.8, 29.7, 23.1, 26.1, 25.1, 23.4, 21.7, 24.4, 13.2,
    ///     22.1, 26.7, 22.7, 1, 18.2, 28.7, 29.1, 27.4, 22.3, 13.2, 22.5, 25.0, 1,
    ///     6.6, 23.7, 23.5, 17.3, 24.6, 27.8, 29.7, 25.3, 19.9, 18.2, 26.2, 20.4,
    ///     23.3, 26.7, 26.0, 1, 25.1, 33.1, 35.0, 25.3, 23.6, 23.2, 20.2, 24.7, 22.6,
    ///     39.1, 26.5, 22.7
    /// };
    /// 
    /// // Create a t-test as a first attempt.
    /// var t = new TwoSampleTTest(redwell, whitney);
    /// 
    /// Console.WriteLine("T-Test");
    /// Console.WriteLine("Test p-value: " + t.PValue);    // ~0.837
    /// Console.WriteLine("Significant? " + t.Significant); // false
    /// 
    /// // Create a non-parametric Kolmogorov-Smirnov test
    /// var ks = new TwoSampleKolmogorovSmirnovTest(redwell, whitney);
    /// 
    /// Console.WriteLine("KS-Test");
    /// Console.WriteLine("Test p-value: " + ks.PValue);    // ~0.038
    /// Console.WriteLine("Significant? " + ks.Significant); // true
    /// </code>
    /// </example>
    /// 
    [Serializable]
    public class TwoSampleKolmogorovSmirnovTest : HypothesisTest<KolmogorovSmirnovDistribution>
    {

        /// <summary>
        ///   Gets the alternative hypothesis under test. If the test is
        ///   <see cref="IHypothesisTest.Significant"/>, the null hypothesis can be rejected
        ///   in favor of this alternative hypothesis.
        /// </summary>
        /// 
        public TwoSampleKolmogorovSmirnovTestHypothesis Hypothesis { get; private set; }

        /// <summary>
        ///   Gets the first empirical distribution being tested.
        /// </summary>
        /// 
        public EmpiricalDistribution EmpiricalDistribution1 { get; private set; }

        /// <summary>
        ///   Gets the second empirical distribution being tested.
        /// </summary>
        /// 
        public EmpiricalDistribution EmpiricalDistribution2 { get; private set; }


        /// <summary>
        ///   Creates a new Two-Sample Kolmogorov test.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// 
        public TwoSampleKolmogorovSmirnovTest(double[] sample1, double[] sample2)
            : this(sample1, sample2, TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal)
        { }

        /// <summary>
        ///   Creates a new Two-Sample Kolmogorov test.
        /// </summary>
        /// 
        /// <param name="sample1">The first sample.</param>
        /// <param name="sample2">The second sample.</param>
        /// <param name="alternate">The alternative hypothesis (research hypothesis) to test.</param>
        /// 
        public TwoSampleKolmogorovSmirnovTest(double[] sample1, double[] sample2,
            TwoSampleKolmogorovSmirnovTestHypothesis alternate)
        {

            this.Hypothesis = alternate;
            int n1 = sample1.Length;
            int n2 = sample2.Length;

            // Compute the effective number of parameters (degrees of freedom)
            double n = (n1 * n2) / (double)(n1 + n2);

            // Create the test statistic distribution with given d. f.
            StatisticDistribution = new KolmogorovSmirnovDistribution(n);

            // Create the two empirical distributions from the data
            EmpiricalDistribution1 = new EmpiricalDistribution(sample1, 0);
            EmpiricalDistribution2 = new EmpiricalDistribution(sample2, 0);

            // Merge the samples to measure the empirical CDFs in same steps
            double[] Y = new double[n1 + n2 + 1];
            double[] D = new double[Y.Length];

            Y[0] = Double.NegativeInfinity;
            for (int i = 0; i < sample1.Length; i++) Y[i + 1] = sample1[i];
            for (int i = 0; i < sample2.Length; i++) Y[i + n1 + 1] = sample2[i];

            // Sort the samples
            Array.Sort(Y);

            // Create the two empirical CDFs
            Func<double, double> F = EmpiricalDistribution1.DistributionFunction;
            Func<double, double> G = EmpiricalDistribution2.DistributionFunction;

            // Finally, compute the test statistic and perform actual testing.
            if (alternate == TwoSampleKolmogorovSmirnovTestHypothesis.SamplesDistributionsAreUnequal)
            {
                // Test if the samples have been drawn from different distributions.

                // This is a correction on the common formulation found in many places
                //  such as in Wikipedia. Please see the Engineering Statistics Handbook,
                //  section "1.3.5.16. Kolmogorov-Smirnov Goodness-of-Fit Test" for more
                //  details: http://www.itl.nist.gov/div898/handbook/eda/section3/eda35g.htm

                for (int i = 0; i < Y.Length - 1; i++)
                    D[i] = Math.Max(Math.Abs(F(Y[i]) - G(Y[i + 1])), Math.Abs(F(Y[i]) - G(Y[i])));

                base.Statistic = D.Max(); // This is the two-sided "Dn" statistic.
                base.PValue = StatisticDistribution.ComplementaryDistributionFunction(Statistic);
                base.Tail = Testing.DistributionTail.TwoTail;
            }
            else if (alternate == TwoSampleKolmogorovSmirnovTestHypothesis.FirstSampleIsLargerThanSecond)
            {
                // Test if the first sample's distribution is significantly "larger"
                // than the second sample's distribution, in a statistical sense.

                for (int i = 0; i < Y.Length - 1; i++)
                    D[i] = Math.Max(F(Y[i]) - G(Y[i + 1]), F(Y[i]) - G(Y[i]));

                base.Statistic = D.Max(); // This is the one-sided "Dn+" statistic.
                base.PValue = StatisticDistribution.OneSideDistributionFunction(Statistic);
                base.Tail = Testing.DistributionTail.OneUpper;
            }
            else
            {
                // Test if the first sample's distribution is significantly "smaller"
                // than the second sample's distribution, in a statistical sense.

                for (int i = 0; i < Y.Length - 1; i++)
                    D[i] = Math.Max(G(Y[i + 1]) - F(Y[i]), G(Y[i]) - F(Y[i]));

                base.Statistic = D.Max(); // This is the one-sided "Dn-" statistic.
                base.PValue = StatisticDistribution.OneSideDistributionFunction(Statistic);
                base.Tail = Testing.DistributionTail.OneLower;
            }
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
            throw new NotSupportedException();
        }

    }
}