// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2016
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

namespace Accord.Tests.Statistics
{
    using System;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Testing;
    using NUnit.Framework;

    [TestFixture]
    public class AndersonDarlingTestTest
    {


        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }



        [Test]
        public void AndersonDarlingConstructorTest()
        {
            // Test against a standard Uniform distribution
            // References: http://www.math.nsysu.edu.tw/~lomn/homepage/class/92/kstest/kolmogorov.pdf


            // Suppose we got a new sample, and we would like to test whether this
            // sample seems to have originated from a uniform continuous distribution.
            //
            double[] sample = 
            { 
                0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382
            };

            // First, we create the distribution we would like to test against:
            //
            var distribution = UniformContinuousDistribution.Standard;

            // Now we can define our hypothesis. The null hypothesis is that the sample
            // comes from a standard uniform distribution, while the alternate is that
            // the sample is not from a standard uniform distribution.
            //
            var adtest = new AndersonDarlingTest(sample, distribution);

            double statistic = adtest.Statistic; //  1.3891622091168489561
            double pvalue = adtest.PValue; // 0.2052

            bool significant = adtest.Significant; // false

            // Since the null hypothesis could not be rejected, then the sample
            // can perhaps be from a uniform distribution. However, please note
            // that this doesn't means that the sample *is* from the uniform, it
            // only means that we could not rule out the possibility.

            Assert.AreEqual(distribution, adtest.TheoreticalDistribution);
            Assert.AreEqual(DistributionTail.TwoTail, adtest.Tail);

            Assert.AreEqual(1.3891622091168489561, statistic, 1e-10);
            Assert.AreEqual(0.2052039626840637121, pvalue, 1e-10);
            Assert.IsFalse(Double.IsNaN(pvalue));

            // Tested against R's package ADGofTest

            /* 
                X <- c( 0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382)
                ad.test(X)

                Anderson-Darling GoF Test

                data:  X 
                AD = 1.3891999999999999904, p-value = 0.2052
                alternative hypothesis: NA 
            */

            Assert.IsFalse(adtest.Significant);
        }

        [Test]
        public void AndersonDarlingConstructorTest2()
        {
            // Test against a Normal distribution

            // This time, let's see if the same sample from the previous example
            // could have originated from a standard Normal (Gaussian) distribution.
            //
            double[] sample =
            { 
                0.621, 0.503, 0.203, 0.477, 0.710, 0.581, 0.329, 0.480, 0.554, 0.382
            };

            NormalDistribution distribution = NormalDistribution.Estimate(sample);

            var adtest = new AndersonDarlingTest(sample, distribution);

            double statistic = adtest.Statistic; // 0.1796
            double pvalue = adtest.PValue; // 0.8884

            bool significant = adtest.Significant; // false

         

            Assert.AreEqual(distribution, adtest.TheoreticalDistribution);
            Assert.AreEqual(DistributionTail.TwoTail, adtest.Tail);

            Assert.AreEqual(0.1796, adtest.Statistic, 1e-4);
            Assert.AreEqual(0.8884, adtest.PValue, 1e-4);
            Assert.IsFalse(Double.IsNaN(adtest.Statistic));

            Assert.IsFalse(adtest.Significant);
        }

    }
}