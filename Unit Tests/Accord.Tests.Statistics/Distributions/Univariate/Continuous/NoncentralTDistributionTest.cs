// Accord Unit Tests
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

namespace Accord.Tests.Statistics
{

    using Accord.Statistics.Distributions.Univariate;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Globalization;

    [TestClass()]
    public class NoncentralTDistributionTest
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


        [TestMethod()]
        public void ConstructorTest2()
        {
            var distribution = new NoncentralTDistribution(
                degreesOfFreedom: 4, noncentrality: 2.42);

            double mean = distribution.Mean;     // 3.0330202123035104
            double median = distribution.Median; // 2.6034842414893795
            double var = distribution.Variance;  // 4.5135883917583683

            double cdf = distribution.DistributionFunction(x: 1.4); // 0.15955740661144721
            double pdf = distribution.ProbabilityDensityFunction(x: 1.4); // 0.23552141805184526
            double lpdf = distribution.LogProbabilityDensityFunction(x: 1.4); // -1.4459534225195116

            double ccdf = distribution.ComplementaryDistributionFunction(x: 1.4); // 0.84044259338855276
            double icdf = distribution.InverseDistributionFunction(p: cdf); // 1.4000000000123853

            double hf = distribution.HazardFunction(x: 1.4); // 0.28023498559521387
            double chf = distribution.CumulativeHazardFunction(x: 1.4); // 0.17382662901507062

            string str = distribution.ToString(CultureInfo.InvariantCulture); // T(x; df = 4, μ = 2.42)

            Assert.AreEqual(3.0330202123035104, mean);
            Assert.AreEqual(2.6034842414893795, median);
            Assert.AreEqual(4.5135883917583683, var);
            Assert.AreEqual(0.17382662901507062, chf);
            Assert.AreEqual(0.15955740661144721, cdf);
            Assert.AreEqual(0.23552141805184526, pdf);
            Assert.AreEqual(-1.4459534225195116, lpdf);
            Assert.AreEqual(0.28023498559521387, hf);
            Assert.AreEqual(0.84044259338855276, ccdf);
            Assert.AreEqual(1.4000000000123853, icdf);
            Assert.AreEqual("T(x; df = 4, μ = 2.42)", str);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {
            double[,] table = 
            {
                //   x    d     df      expected
                {  3.00,  0.0,  1,    0.8975836176504333 },
                {  3.00,  0.0,  2,    0.9522670169       },
                {  3.00,  0.0,  3,    0.9711655571887813 },
                {  3.00,  0.5,  1,    0.8231218863999999 },
                {  3.00,  0.5,  2,    0.904902151        },
                {  3.00,  0.5,  3,    0.9363471834       },
                {  3.00,  1.0,  1,    0.7301025986       },
                {  3.00,  1.0,  2,    0.8335594263       },
                {  3.00,  1.0,  3,    0.8774010255       },
                {  3.00,  2.0,  1,    0.5248571617       },
                {  3.00,  2.0,  2,    0.6293856597       },
                {  3.00,  2.0,  3,    0.6800271741       },
                {  3.00,  4.0,  1,    0.20590131975      },
                {  3.00,  4.0,  2,    0.2112148916       },
                {  3.00,  4.0,  3,    0.2074730718       },
                { 15.00,  7.0, 15,    0.9981130072       },
                { 15.00,  7.0, 20,    0.999487385        },
                { 15.00,  7.0, 25,    0.9998391562       },
                {  0.05,  1.0,  1,    0.168610566972     },
                {  0.05,  1.0,  2,    0.16967950985      },
                {  0.05,  1.0,  3,    0.1701041003       },
                {  4.00,  2.0, 10,    0.9247683363       },
                {  4.00,  3.0, 10,    0.7483139269       },
                {  4.00,  4.0, 10,    0.4659802096       },
                {  5.00,  2.0, 10,    0.9761872541       },
                {  5.00,  3.0, 10,    0.8979689357       },
                {  5.00,  4.0, 10,    0.7181904627       },
                {  6.00,  2.0, 10,    0.9923658945       },
                {  6.00,  3.0, 10,    0.9610341649       },
                {  6.00,  4.0, 10,    0.868800735        },
            };

            for (int i = 0; i < table.GetLength(0); i++)
            {
                double x = table[i, 0];
                double delta = table[i, 1];
                double df = table[i, 2];

                var target = new NoncentralTDistribution(df, delta);

                double expected = table[i, 3];
                double actual = target.DistributionFunction(x);

                Assert.AreEqual(expected, actual, 1e-10);
            }
        }

        [TestMethod()]
        public void ProbabilityFunctionTest()
        {
            double[,] table = 
            {
                //   x    d     df      expected
                {  3.00,  0.0,  1,    0.03183098861      },
                {  3.00,  0.0,  2,    0.02741012223      },
                {  3.00,  0.0,  3,    0.02297203730      },
                {  3.00,  0.5,  1,    0.05359565579      },
                {  3.00,  0.5,  2,    0.05226515196      },
                {  3.00,  0.5,  3,    0.04788249161      },
                {  3.00,  7.0, 15,    0.0009236578208725 },
                { 15.00,  7.0, 15,    0.0013850587855    },
                { 15.00,  7.0, 25,    0.00018206084230   },
                {  0.00,  7.0, 25,    0.0000000000090438 },
                {  0.00,  2.0,  1,    0.0430785586036    },
                {  0.00,  2.0,  2,    0.047848248255205  },
                {  0.00,  2.0,  3,    0.0497428348122    },
                {  0.00,  4.0,  1,    0.000106781070     },
                {  0.00,  4.0,  2,    0.000118603949     },
            };

            for (int i = 0; i < table.GetLength(0); i++)
            {
                double x = table[i, 0];
                double delta = table[i, 1];
                double df = table[i, 2];

                var target = new NoncentralTDistribution(df, delta);

                double expected = table[i, 3];
                double actual = target.ProbabilityDensityFunction(x);

               Assert.AreEqual(expected, actual, 1e-10);
            }
        }


        [TestMethod()]
        public void MeanTest()
        {
            NoncentralTDistribution target;

            target = new NoncentralTDistribution(3, 5);
            Assert.AreEqual(6.90988298942671, target.Mean);
            Assert.AreEqual(30.2535170724314, target.Variance);

            target = new NoncentralTDistribution(1.1, 5);
            Assert.AreEqual(44.672931414521223, target.Mean);
            Assert.IsTrue(Double.IsNaN(target.Variance));

            target = new NoncentralTDistribution(4.2, -2);
            Assert.AreEqual(-2.4746187622053673, target.Mean);
            Assert.AreEqual(3.42171652719572, target.Variance);

            target = new NoncentralTDistribution(0.047, 55);
            Assert.IsTrue(Double.IsNaN(target.Mean));
            Assert.IsTrue(Double.IsNaN(target.Variance));

            target = new NoncentralTDistribution(2.1, -0.42);
            Assert.AreEqual(-0.71446479359810855, target.Mean);
            Assert.AreEqual(24.19394005870879, target.Variance);

            target = new NoncentralTDistribution(5.97, -42);
            Assert.AreEqual(-48.390832208385575, target.Mean);
            Assert.AreEqual(312.49612392294557, target.Variance);
        }

        [TestMethod()]
        public void MedianTest()
        {
            NoncentralTDistribution target = new NoncentralTDistribution(3.2, 4.57);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

    }
}
