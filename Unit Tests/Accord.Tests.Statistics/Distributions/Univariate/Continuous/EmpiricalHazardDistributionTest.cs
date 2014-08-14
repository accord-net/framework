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
    using Accord.Math;

    [TestClass()]
    public class EmpiricalHazardDistributionTest
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
        public void EmpiricalHazardConstructorTest3()
        {
            double[] times = { 11, 10, 9, 8, 6, 5, 4, 2 };
            double[] values = { 0.22, 0.67, 1.00, 0.18, 1.00, 1.00, 1.00, 0.55 };
            

            EmpiricalHazardDistribution distribution = new EmpiricalHazardDistribution(times, values);


            double mean = distribution.Mean; // 0.93696461879063664
            double median = distribution.Median; // 3.9999999151458066
            double var = distribution.Variance; // 2.0441627748096289
            double chf = distribution.CumulativeHazardFunction(x: 4.2); // 1.55
            double cdf = distribution.DistributionFunction(x: 4.2); // 0.7877520261732569
            double pdf = distribution.ProbabilityDensityFunction(x: 4.2); // 0.046694554241883471
            double lpdf = distribution.LogProbabilityDensityFunction(x: 4.2); // -3.0641277326297756
            double hf = distribution.HazardFunction(x: 4.2); // 0.22
            double ccdf = distribution.ComplementaryDistributionFunction(x: 4.2); // 0.21224797382674304
            double icdf = distribution.InverseDistributionFunction(p: cdf); // 4.3483975243778978

            string str = distribution.ToString(); // H(x; v, t)

            Assert.AreEqual(0.93696461879063664, mean);
            Assert.AreEqual(3.9999999151458066, median, 1e-6);
            Assert.AreEqual(2.0441627748096289, var);
            Assert.AreEqual(1.55, chf);
            Assert.AreEqual(0.7877520261732569, cdf);
            Assert.AreEqual(0.046694554241883471, pdf);
            Assert.AreEqual(-3.0641277326297756, lpdf);
            Assert.AreEqual(0.22, hf);
            Assert.AreEqual(0.21224797382674304, ccdf);
            Assert.AreEqual(4.3483975243778978, icdf, 1e-8);
            Assert.AreEqual("H(x; v, t)", str);
        }

        [TestMethod()]
        public void DistributionFunctionTest()
        {

            double[] values = 
            {
                1.0000000000000000, 0.8724284533876597, 0.9698946958777951,
                1.0000000000000000, 0.9840887140861863, 1.0000000000000000,
                1.0000000000000000, 1.0000000000000000, 1.0000000000000000,
                0.9979137773216293, 1.0000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };

            EmpiricalHazardDistribution target = EmpiricalHazardDistribution
                .FromSurvivalValues(times, values);


            // Data from: http://www.sph.emory.edu/~cdckms/CoxPH/prophaz2.html
            double[] expectedBaselineSurvivalFunction = 
            {
                1.0000, 0.9979, 0.9979, 0.9979,
                0.9979, 0.9979, 0.9820, 
                0.9820, 0.9525, 0.8310, 0.8310,
            };


            double[] hazardFunction = new double[expectedBaselineSurvivalFunction.Length];
            double[] survivalFunction = new double[expectedBaselineSurvivalFunction.Length];

            for (int i = 0; i < 11; i++)
                hazardFunction[i] = target.CumulativeHazardFunction(i + 1);

            for (int i = 0; i < 11; i++)
                survivalFunction[i] = target.ComplementaryDistributionFunction(i + 1);


            for (int i = 0; i < expectedBaselineSurvivalFunction.Length; i++)
            {
                Assert.AreEqual(expectedBaselineSurvivalFunction[i], survivalFunction[i], 0.01);

                // Ho = -log(So)
                Assert.AreEqual(hazardFunction[i], -Math.Log(survivalFunction[i]), 0.01);

                // So = exp(-Ho)
                Assert.AreEqual(survivalFunction[i], Math.Exp(-hazardFunction[i]), 0.01);
            }
        }

        [TestMethod()]
        public void MedianTest()
        {
            double[] values = 
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.0000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };


            EmpiricalHazardDistribution target =
                new EmpiricalHazardDistribution(times, values);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [TestMethod()]
        public void DistributionFunctionTest2()
        {

            double[] values = 
            {
               0.0000000000000000, 0.0351683340828711, 0.0267358118285064,
               0.0000000000000000, 0.0103643094219679, 0.0000000000000000,
               0.0000000000000000, 0.0000000000000000, 0.0000000000000000,
               0.000762266794052363, 0.000000000000000
            };

            double[] times =
            {
                11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1
            };


            EmpiricalHazardDistribution target =
                new EmpiricalHazardDistribution(times, values);

            double[] expected = 
            {
			    1.000000000000000,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.999238023657475,	
			    0.98893509519066469,	
			    0.98893509519066469,
			    0.96284543081744489,
			    0.92957227114936058,	
			    0.92957227114936058,	
            };


            double[] hazardFunction = new double[expected.Length];
            double[] survivalFunction = new double[expected.Length];
            double[] complementaryDistribution = new double[expected.Length];

            for (int i = 0; i < 11; i++)
                hazardFunction[i] = target.CumulativeHazardFunction(i + 1);

            for (int i = 0; i < 11; i++)
                survivalFunction[i] = target.ComplementaryDistributionFunction(i + 1);


            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], survivalFunction[i], 1e-5);

                // Ho = -log(So)
                Assert.AreEqual(hazardFunction[i], -Math.Log(survivalFunction[i]), 1e-5);

                // So = exp(-Ho)
                Assert.AreEqual(survivalFunction[i], Math.Exp(-hazardFunction[i]), 1e-5);
            }
        }

    }
}
